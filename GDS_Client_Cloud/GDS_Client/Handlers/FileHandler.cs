using System;
using System.IO;
using System.Threading;
using System.Xml.Serialization;

namespace GDS_Client
{
    public class FileHandler
    {
        public static T Load<T>(string FileSpec, int counter = 0)
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
                if (counter != 5)
                {
                    counter++;
                    return Load<T>(FileSpec, counter);
                }
                else
                {
                    return default(T);
                }
            }
        }

        public static void Save<T>(T ToSerialize, string FileSpec, int counter = 0)
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
                if (counter != 5)
                {
                    counter++;
                    Save<T>(ToSerialize, FileSpec, counter);
                }
                else
                {
                    Console.WriteLine("THERE IS PROBLEM WITH SAVING FILE");
                }
            }
        }
    }
}
