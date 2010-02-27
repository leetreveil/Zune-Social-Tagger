using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ZuneSocialTagger.Core
{
    public static class XmlDeser
    {
        public static string XmlSerializeToString(this object objectInstance)
        {
            var serializer = new XmlSerializer(objectInstance.GetType());
            var sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, objectInstance);
            }

            return sb.ToString();
        }

        public static T XmlDeserializeFromString<T>(this string objectData)
        {
            return (T)XmlDeserializeFromString(objectData, typeof(T));
        }

        public static T XmlDeserializeFromStream<T>(this Stream stream)
        {
            return (T) XmlDeserializeFromStream(stream, typeof (T));
        }

        private static object XmlDeserializeFromString(string objectData, Type type)
        {
            var serializer = new XmlSerializer(type);

            using (TextReader reader = new StringReader(objectData))
                return serializer.Deserialize(reader);
        }

        private static object XmlDeserializeFromStream(Stream stream, Type type)
        {
            var serializer = new XmlSerializer(type);

            return serializer.Deserialize(stream);
        }
    }
}