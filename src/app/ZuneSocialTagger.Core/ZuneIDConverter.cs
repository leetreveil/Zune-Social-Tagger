using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZuneSocialTagger.Core
{
    public static class ZuneIDConverter

    {
        /// <summary>
        /// Takes a well formed id and returns the byte data, notice how the first 3 sections are flipped and a byte is appended to the start
        /// </summary>
        /// <param name="id">Should look like this: 28cc0700-0600-11db-89ca-0019b92a3933</param>
        /// <returns>Should output like this: 000007cc280006db1189ca0019b92a3933</returns>
        public static byte[] Convert(string id)
        {
            if (id.Length != 36)
                throw new Exception("The id is not the correct length");

            string[] split = id.ToUpper().Split('-');
            string resultantString = string.Empty;

            foreach (var str in split.Take(3))
                resultantString += String.Join("", SplitStringIntoBitsOf(str, 2).Reverse().ToArray());

            foreach (var str in split.Skip(3))
                resultantString += str;


            List<byte> data = new List<byte>{0};
            data.AddRange(StringToByteArray(resultantString));

            return data.ToArray();
        }

        private static string[] SplitStringIntoBitsOf(string @string ,int parts)
        {
            int numberOfParts = @string.Length / 2;

            var temp = new string[numberOfParts];

            for (int i = 0; i < numberOfParts; i++)
                temp[i] = @string.Substring(i*parts, parts);

            return temp;
        }


        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = System.Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }


    }
}