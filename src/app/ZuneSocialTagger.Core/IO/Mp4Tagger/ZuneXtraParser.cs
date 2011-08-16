using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZuneSocialTagger.Core.IO.Mp4Tagger
{
    /// <summary>
    /// Reads / creates the zune Xtra part which the zune software creates inside of the .mp4 file
    /// </summary>
    public static class ZuneXtraParser
    {
        public static IEnumerable<IBasePart> ParseRawData(byte[] data)
        {
            foreach (var part in ParseRawPartData(ParseXtraAtomRaw(data)))
            {
                yield return part;
            }
        }

        public static byte[] ConstructRawData(IEnumerable<IBasePart> parts)
        {
            var result = new List<byte>();

            foreach (IBasePart part in parts)
            {
                if (part.GetType() == typeof(GuidPart))
                {
                    result = result.Concat(ConstructRawPartFromGuidPart(part as GuidPart)).ToList();
                }
                if (part.GetType() == typeof(RawPart))
                {
                    IEnumerable<byte> constructRawPartFromOtherPart = ConstructRawPartFromOtherPart(part as RawPart);
                    result = result.Concat(constructRawPartFromOtherPart).ToList();
                }
            }

            return result.ToArray();
        }

        private static IEnumerable<RawPart> ParseRawPartData(IEnumerable<RawPart> data)
        {
            foreach (RawPart rawPartData in data)
            {
                if (rawPartData.Type == 72) // Guid
                {
                    yield return new GuidPart(rawPartData);
                }
                else //just save any other parts as raw data as we do not need to be able to parse it anyway
                {
                    yield return rawPartData;
                }
            }
        }

        private static IEnumerable<RawPart> ParseXtraAtomRaw(byte[] atomContents)
        {
            using (var memStream = new MemoryStream(atomContents))
            using (var binReader = new BinaryReader(memStream))
            {
                while (binReader.BaseStream.Position < binReader.BaseStream.Length)
                {
                    int partLength = getPartLength(binReader); //first 4 bytes donates the length of the part
                    int partNameLength = getPartLength(binReader); //get length of the part
                    string partName = getPartName(binReader, partNameLength); // get the name of the part
                    int partFlag = getPartLength(binReader); //get the part flag, should always be 1

                    if (partFlag == 1)
                    {
                        int partContentLength = getPartLength(binReader);
                        var partType = getPartType(binReader); // i think this is a 2 byte id for the field 48h / 72d = guid?

                        byte[] partContent = binReader.ReadBytes(partContentLength - 6);

                        yield return new RawPart
                        {
                            Content = partContent,
                            Name = partName,
                            Type = partType
                        };
                    }
                }
            }
        }

        private static int getPartLength(BinaryReader reader)
        {
            var atomLengthBuf = new byte[4];
            reader.Read(atomLengthBuf, 0, atomLengthBuf.Length);

            if (BitConverter.IsLittleEndian) Array.Reverse(atomLengthBuf);
            return BitConverter.ToInt32(atomLengthBuf, 0);
        }

        private static short getPartType(BinaryReader reader)
        {
            var atomLengthBuf = new byte[2];
            reader.Read(atomLengthBuf, 0, atomLengthBuf.Length);

            if (BitConverter.IsLittleEndian) Array.Reverse(atomLengthBuf);
            return BitConverter.ToInt16(atomLengthBuf, 0);
        }

        /// <summary>
        /// The atom name is always 4 bytes long and is a string
        /// </summary>
        /// <returns></returns>
        private static string getPartName(BinaryReader reader, long length)
        {
            var atomNameBuf = new byte[length];
            reader.Read(atomNameBuf, 0, atomNameBuf.Length);

            //return empty if the first byte is 0
            return atomNameBuf[0] == 0 ? String.Empty : Encoding.Default.GetString(atomNameBuf);
        }

        private static IEnumerable<byte> ConstructRawPartFromOtherPart(RawPart rawPart)
        {
            byte[] totalLength;
            byte[] nameLength = BitConExt.GetBytesIsLi(rawPart.Name.Length);
            byte[] name = Encoding.Default.GetBytes(rawPart.Name);
            byte[] guidPartFlag = new byte[] { 0x00, 0x00, 0x00, 0x01 }; // 4byte flag
            byte[] contentLength = BitConExt.GetBytesIsLi(rawPart.Content.Length + 6);
            byte[] partType = BitConExt.GetBytesIsLi(rawPart.Type);
            byte[] content = rawPart.Content;

            IEnumerable<byte> allConstructed =
                nameLength.Concat(name).Concat(guidPartFlag).Concat(contentLength).Concat(partType).Concat(content);

            totalLength = BitConExt.GetBytesIsLi(allConstructed.Count() + 4);

            return totalLength.Concat(allConstructed);
        }

        private static IEnumerable<byte> ConstructRawPartFromGuidPart(GuidPart guidPart)
        {
            var nameLength = BitConExt.GetBytesIsLi(guidPart.Name.Length);
            var name = Encoding.Default.GetBytes(guidPart.Name);
            var guidPartFlag = new byte[] { 0x00, 0x00, 0x00, 0x01 }; // 4byte flag
            var guidLength = new byte[] { 0x00, 0x00, 0x00, 0x16 }; //16 bytes guid length
            var partType = new byte[] { 0x00, 0x48 }; //2byte guid type

            var guid = guidPart.MediaId.ToByteArray(); // guid
            var partContentWithoutLength = nameLength.Concat(name).Concat(guidPartFlag).Concat(guidLength).Concat(partType).Concat(guid);

            byte[] totalPartLength = BitConExt.GetBytesIsLi(partContentWithoutLength.Count() + 4);

            return totalPartLength.Concat(partContentWithoutLength).ToArray();
        }
    }
}