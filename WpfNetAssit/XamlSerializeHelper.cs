using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace WpfNetAssit
{
    class XamlSerializeHelper<T> where T: new()
    {
        public static T Load(string filename)
        {
            try
            {
                using (StreamReader fs = new StreamReader(filename, Encoding.UTF8))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    return (T)xmlSerializer.Deserialize(fs);
                }
            }
            catch
            {
                return new T();
            }
        }

        public static bool Save(string filename, T data)
        {
            try
            {
                using (StreamWriter fs = new StreamWriter(filename, false, Encoding.UTF8))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(fs, data);
                }
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
