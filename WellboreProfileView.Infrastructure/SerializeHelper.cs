using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace WellboreProfileView.Infrastructure
{
    public static class SerializeHelper
    {
        public static T DeSerialize<T>(string filePath)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (StreamReader reader = new StreamReader(filePath))
                return (T)serializer.Deserialize(reader);
        }

        public static T DeSerializeFromContents<T>(string contents)
        {
            var serializer = new XmlSerializer(typeof(T));
            byte[] byteArray = Encoding.UTF8.GetBytes(contents);
            using (MemoryStream reader = new MemoryStream(byteArray))
                return (T)serializer.Deserialize(reader);
        }

        public static void Serialize<T>(T obj, string filePath)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (var stream = File.OpenWrite(filePath))
                serializer.Serialize(stream, obj);
        }
    }
}