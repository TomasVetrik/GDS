using System;
using System.IO;
using System.Threading;
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
            catch (Exception ex)
            {
                Console.WriteLine("Chyba pri nacitani suboru: " + FileSpec + " " + ex);
                Thread.Sleep(1000);
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
            catch (Exception ex)
            {
                Console.WriteLine("Chyba pri zapise suboru: " + FileSpec+ " " + ex);
                Thread.Sleep(1000);
                Save<T>(ToSerialize, FileSpec);
            }
        }

    }
}
