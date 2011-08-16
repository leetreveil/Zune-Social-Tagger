using System;

namespace ZuneSocialTagger.Core.IO.Mp4Tagger
{
    public class BitConExt
    {
        /// <summary>
        /// GetBytes with BitConverter.IsLittleEndian and Array.Reverse
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytesIsLi(bool value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);

            return bytes;
        }

        public static byte[] GetBytesIsLi(long value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);

            return bytes;
        }

        public static byte[] GetBytesIsLi(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);

            return bytes;
        }

        public static byte[] GetBytesIsLi(short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian) Array.Reverse(bytes);

            return bytes;
        }

        //public static byte[] GetBytes(uint value)
        //{
        //    byte[] bytes = BitConverter.GetBytes(value);

        //    if (BitConverter.IsLittleEndian) Array.Reverse(bytes);

        //    return bytes;
        //}

        //public static byte[] GetBytes(double value)
        //{
        //    byte[] bytes = BitConverter.GetBytes(value);

        //    if (BitConverter.IsLittleEndian) Array.Reverse(bytes);

        //    return bytes;
        //}
    }
}