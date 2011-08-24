using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZuneSocialTagger.Core.IO.Mp4Tagger
{
    public class ByteHelpers
    {
        public static byte[] GetBytesAsLi(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);

            return bytes;
        }
    }
}
