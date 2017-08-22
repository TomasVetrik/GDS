using System.IO;
using System.Xml.Serialization;

namespace GDS_Client
{
    public class FileHandler
    {
        public static T Load<T>(string FileSpec)
        {
            try
            {
                var formatter = new XmlSerializer(typeof(T));
                using (var aFile = new FileStream(FileSpec, FileMode.Open))
                {
                    byte[] buffer = new byte[aFile.Length];
                    aFile.Read(buffer, 0, (int)aFile.Length);
                    using (MemoryStream stream = new MemoryStream(buffer))
                    {
                        return (T)formatter.Deserialize(stream);
                    }
                }
            }
            catch
            {
                return Load<T>(FileSpec);
            }
        }

        public static void Save<T>(T ToSerialize, string FileSpec)
        {
            try
            {
                Directory.CreateDirectory(FileSpec.Substring(0, FileSpec.LastIndexOf('\\')));
                var outFile = File.Create(FileSpec);
                var formatter = new XmlSerializer(typeof(T));

                formatter.Serialize(outFile, ToSerialize);
                outFile.Close();
            }
            catch
            {
                Save<T>(ToSerialize, FileSpec);
            }
        }

    }
}
