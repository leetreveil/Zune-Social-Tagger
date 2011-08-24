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
        public static byte[] ConstructRawData(IEnumerable<IBasePart> parts)
        {
            var result = new List<byte>();

            foreach (IBasePart part in parts)
            {
                result.AddRange(part.Render());
            }

            return result.ToArray();
        }

        public static IEnumerable<IBasePart> ParseRawData(byte[] atomContents)
        {
            var parts = new List<IBasePart>();

            using (var memStream = new MemoryStream(atomContents))
            using (var binReader = new BinaryReader(memStream))
            {
                while (binReader.BaseStream.Position < binReader.BaseStream.Length)
                {
                    //at a minimum we should be able to parse the length, name length and name

                    int partLength = readInt(binReader); //first 4 bytes donates the length of the part
                    int partNameLength = readInt(binReader); //get length of the part
                    string partName = getPartName(binReader, partNameLength); // get the name of the part

                    int toRead = partLength - (4 + 4 + partName.Length);
                    byte[] restOfPart = new byte[toRead];
                    binReader.Read(restOfPart, 0, toRead);

                    try
                    {
                        if (isGuidPart(restOfPart)) 
                        {
                            parts.Add(new GuidPart(partName, restOfPart));
                        } 
                        else
                        {
                            parts.Add(new RawPart{Name = partName, Content = restOfPart});
                        }
                    }
                    catch (Exception ex)
                    {
                        parts.Add(new RawPart() {Name = partName, Content = restOfPart});
                    }
                }
            }
            return parts;
        }

        private static bool isGuidPart(byte[] partData) 
        {
            using (var memStream = new MemoryStream(partData))
            using (var binReader = new BinaryReader(memStream)) 
            {
                int partFlag = readInt(binReader);

                if (partFlag == 1)
                {
                    int partContentLength = readInt(binReader);
                    short partType = readShort(binReader); // this is a 2 byte id which tells us what it is

                    if (partType == 72) // 72 == guid
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static int readInt(BinaryReader reader)
        {
            var atomLengthBuf = new byte[4];
            reader.Read(atomLengthBuf, 0, atomLengthBuf.Length);

            if (BitConverter.IsLittleEndian) Array.Reverse(atomLengthBuf);
            return BitConverter.ToInt32(atomLengthBuf, 0);
        }

        private static short readShort(BinaryReader reader)
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

            if (atomNameBuf.Count() > 0)
            {
                return atomNameBuf[0] == 0 ? String.Empty : Encoding.Default.GetString(atomNameBuf);
            }
            else
            {
                return String.Empty;
            }
        }
    }
}