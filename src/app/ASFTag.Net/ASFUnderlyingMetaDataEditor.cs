using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ASFTag.Net
{
    internal class ASFUnderlyingMetaDataEditor
    {
        private readonly IWMMetadataEditor _metadataEditor;
        private readonly IWMHeaderInfo3 _headerInfo;

        public ASFUnderlyingMetaDataEditor(string fileName)
        {
            try
            {
                WMFSDKFunctions.WMCreateEditor(out _metadataEditor);

                _metadataEditor.Open(fileName);

                _headerInfo = (IWMHeaderInfo3) _metadataEditor;
            }
            catch (COMException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void Close()
        {
            _metadataEditor.Close();
        }

        public void WriteToFile()
        {
            _metadataEditor.Flush();
        }


        /// <summary>
        /// Returns all available attributes for the file (stream 0)
        /// </summary>
        public IEnumerable<Attribute> Attributes { get { return GetAttributes(); } }

        //------------------------------------------------------------------------------
        // Name: GetAttributes()
        // Desc: Displays all attributes for the specified stream, with support
        //       for GetAttributeByIndexEx.
        //------------------------------------------------------------------------------
        private IEnumerable<Attribute> GetAttributes()
        {
            var list = new List<Attribute>();

            try
            {
                ushort wAttributeCount;


                _headerInfo.GetAttributeCountEx(0, out wAttributeCount);


                for (ushort wAttribIndex = 0; wAttribIndex < wAttributeCount; wAttribIndex++)
                {
                    WMT_ATTR_DATATYPE wAttribType;
                    ushort wLangIndex;
                    string pwszAttribName = null;
                    byte[] pbAttribValue = null;
                    ushort wAttribNameLen = 0;
                    uint dwAttribValueLen = 0;

                    _headerInfo.GetAttributeByIndexEx(0,
                                                      wAttribIndex,
                                                      pwszAttribName,
                                                      ref wAttribNameLen,
                                                      out wAttribType,
                                                      out wLangIndex,
                                                      pbAttribValue,
                                                      ref dwAttribValueLen);

                    pwszAttribName = new String((char) 0, wAttribNameLen);
                    pbAttribValue = new byte[dwAttribValueLen];

                    _headerInfo.GetAttributeByIndexEx(0,
                                                      wAttribIndex,
                                                      pwszAttribName,
                                                      ref wAttribNameLen,
                                                      out wAttribType,
                                                      out wLangIndex,
                                                      pbAttribValue,
                                                      ref dwAttribValueLen);

                    list.Add(ConvertRawDataToAttribute(pwszAttribName, wAttribType, pbAttribValue,
                                            dwAttribValueLen));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return list;
        }

        internal bool AddOrModifyAttribute(Attribute attribute)
        {
            bool ok = true;

            try
            {
                //null terminate the attribute name if it isn't already
                //if (!attribute.Name.EndsWith("\0"))
                //    attribute.Name += (char)0;

                ushort langIdx = 0;
                ushort[] nullIdx = null;
                ushort attribCount = 0;
                _headerInfo.GetAttributeIndices(0, attribute.Name, ref langIdx, nullIdx, ref attribCount);
                ushort[] attribIndices = new ushort[attribCount];
                _headerInfo.GetAttributeIndices(0, attribute.Name, ref langIdx, attribIndices, ref attribCount);

                byte[] pbAttribValue;
                int nAttribValueLen;

                if (!TranslateAttrib(attribute, out pbAttribValue, out nAttribValueLen))
                    return false;


                if (attribCount == 0) //attribute does not exist so we should add a new one
                {
                    ushort newattribIndex;

                    _headerInfo.AddAttribute(0, attribute.Name, out newattribIndex, attribute.Type, 0, pbAttribValue,
                                             (uint) nAttribValueLen);
                }
                else if (attribIndices.Length > 0) //attribute does exist so we should update it
                {
                    _headerInfo.ModifyAttribute(0, attribIndices[0], attribute.Type, 0, pbAttribValue,
                                                (uint) nAttribValueLen);
                }
            }
            catch
            {
                ok = false;
            }

            return ok;
        }

        //------------------------------------------------------------------------------
        // Name: TranslateAttrib()
        // Desc: Converts attributes to byte arrays.
        //------------------------------------------------------------------------------
        private static bool TranslateAttrib(Attribute attribute, out byte[] pbValue,
                                            out int nValueLength)
        {
            switch (attribute.Type)
            {
                case WMT_ATTR_DATATYPE.WMT_TYPE_DWORD:

                    nValueLength = 4;
                    uint[] pdwAttribValue = new uint[1] { Convert.ToUInt32(attribute.Value) };

                    pbValue = new Byte[nValueLength];
                    Buffer.BlockCopy(pdwAttribValue, 0, pbValue, 0, nValueLength);

                    return (true);

                case WMT_ATTR_DATATYPE.WMT_TYPE_WORD:

                    nValueLength = 2;
                    ushort[] pwAttribValue = new ushort[1] { Convert.ToUInt16(attribute.Value) };

                    pbValue = new Byte[nValueLength];
                    Buffer.BlockCopy(pwAttribValue, 0, pbValue, 0, nValueLength);

                    return (true);

                case WMT_ATTR_DATATYPE.WMT_TYPE_QWORD:

                    nValueLength = 8;
                    ulong[] pqwAttribValue = new ulong[1] { Convert.ToUInt64(attribute.Value) };

                    pbValue = new Byte[nValueLength];
                    Buffer.BlockCopy(pqwAttribValue, 0, pbValue, 0, nValueLength);

                    return (true);

                case WMT_ATTR_DATATYPE.WMT_TYPE_STRING:

                    nValueLength = (ushort)((attribute.Value.Length + 1) * 2);
                    pbValue = new Byte[nValueLength];

                    Buffer.BlockCopy(attribute.Value.ToCharArray(), 0, pbValue, 0, attribute.Value.Length * 2);
                    pbValue[nValueLength - 2] = 0;
                    pbValue[nValueLength - 1] = 0;

                    return (true);

                case WMT_ATTR_DATATYPE.WMT_TYPE_BOOL:

                    nValueLength = 4;
                    pdwAttribValue = new uint[1] { Convert.ToUInt32(attribute.Value) };
                    if (pdwAttribValue[0] != 0)
                    {
                        pdwAttribValue[0] = 1;
                    }

                    pbValue = new Byte[nValueLength];
                    Buffer.BlockCopy(pdwAttribValue, 0, pbValue, 0, nValueLength);

                    return (true);

                case WMT_ATTR_DATATYPE.WMT_TYPE_GUID:

                    nValueLength = 16;
                    byte[] guidBytes = new Guid(attribute.Value).ToByteArray();

                    pbValue = new byte[16];
                    Buffer.BlockCopy(guidBytes, 0, pbValue, 0, pbValue.Length);

                    return true;
                default:

                    pbValue = null;
                    nValueLength = 0;
                    Console.WriteLine("Unsupported data type.");

                    return (false);
            }
        }

        //------------------------------------------------------------------------------
        // Name: ConvertRawDataToAttribute()
        // Desc: Coverts the raw data into a readable string format
        //------------------------------------------------------------------------------
        private static Attribute ConvertRawDataToAttribute(string pwszName, WMT_ATTR_DATATYPE attribDataType, byte[] pbValue, uint dwValueLen)
        {
            string pwszValue = String.Empty;

            //
            // The attribute value.
            //
            switch (attribDataType)
            {
                // String
                case WMT_ATTR_DATATYPE.WMT_TYPE_STRING:
      
                        if ((0xFE == Convert.ToInt16(pbValue[0])) &&
                            (0xFF == Convert.ToInt16(pbValue[1])))
                        {
                            if (4 <= dwValueLen)
                            {
                                for (int i = 0; i < pbValue.Length - 2; i += 2)
                                {
                                    pwszValue += Convert.ToString(BitConverter.ToChar(pbValue, i));
                                }
                            }
                        }
                        else if ((0xFF == Convert.ToInt16(pbValue[0])) &&
                                 (0xFE == Convert.ToInt16(pbValue[1])))
                        {
                            if (4 <= dwValueLen)
                            {
                                for (int i = 0; i < pbValue.Length - 2; i += 2)
                                {
                                    pwszValue += Convert.ToString(BitConverter.ToChar(pbValue, i));
                                }
                            }

                        }
                        else
                        {
                            if (2 <= dwValueLen)
                            {
                                for (int i = 0; i < pbValue.Length - 2; i += 2)
                                {
                                    pwszValue += Convert.ToString(BitConverter.ToChar(pbValue, i));
                                }
                            }
                        }
   
                    break;

                // Binary
                case WMT_ATTR_DATATYPE.WMT_TYPE_BINARY:

                    pwszValue = "[" + dwValueLen + " bytes]";
                    break;

                // Boolean
                case WMT_ATTR_DATATYPE.WMT_TYPE_BOOL:

                    pwszValue = BitConverter.ToBoolean(pbValue, 0) ? "True" : "False";
                    break;

                // DWORD
                case WMT_ATTR_DATATYPE.WMT_TYPE_DWORD:

                    uint dwValue = BitConverter.ToUInt32(pbValue, 0);
                    pwszValue = dwValue.ToString();
                    break;

                // QWORD
                case WMT_ATTR_DATATYPE.WMT_TYPE_QWORD:

                    ulong qwValue = BitConverter.ToUInt64(pbValue, 0);
                    pwszValue = qwValue.ToString();
                    break;

                // WORD
                case WMT_ATTR_DATATYPE.WMT_TYPE_WORD:

                    uint wValue = BitConverter.ToUInt16(pbValue, 0);
                    pwszValue = wValue.ToString();
                    break;

                // GUID
                case WMT_ATTR_DATATYPE.WMT_TYPE_GUID:

                    pwszValue = new Guid(pbValue).ToString();
                    break;

                default:

                    break;
            }

            return new Attribute(pwszName.TrimEnd('\0'),pwszValue,attribDataType);
        }
    }
}