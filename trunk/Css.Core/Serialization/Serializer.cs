using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using System.IO;

namespace Css.Serialization
{
    class Serializer
    {
        public static string XmlSerialize(object graph)
        {
            var xmlFormatter = new XmlSerializer(graph.GetType());
            StringWriter w = new StringWriter();
            xmlFormatter.Serialize(w, graph);
            return w.ToString();
        }

        public static object XmlDeserialize(Type type, string xml)
        {
            var xmlFormatter = new XmlSerializer(type);
            StringReader sr = new StringReader(xml);
            return xmlFormatter.Deserialize(sr);
        }

        public static T XmlDeserialize<T>(string xml)
        {
            return (T)XmlDeserialize(typeof(T), xml);
        }

        public static byte[] Serialize(object obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }

        public static object Deserialize(byte[] bytes)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream(bytes))
            {
                return formatter.Deserialize(stream);
            }
        }
    }
}
