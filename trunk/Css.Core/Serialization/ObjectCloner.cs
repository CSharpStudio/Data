using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Css.Serialization
{
    /// <summary>
    /// Helper class for object clone.
    /// </summary>
    public static class ObjectCloner
    {
        /// <summary>
        /// Clone object. Call <seealso cref="ICloneable.Clone"/> method if the object implement ICloneable. 
        /// Otherwise serialize and deserialize to clone the object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object Clone(object obj)
        {
            if (object.ReferenceEquals(obj, null)) return null;
            if (obj is ICloneable)
                return ((ICloneable)obj).Clone();
            return DoBinaryClone(obj);
        }

        public static object BinaryClone(object obj)
        {
            if (object.ReferenceEquals(obj, null)) return null;
            return DoBinaryClone(obj);
        }

        static object DoBinaryClone(object obj)
        {
            using (MemoryStream buffer = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(buffer, obj);
                buffer.Position = 0;
                object temp = formatter.Deserialize(buffer);
                return temp;
            }
        }
    }
}
