﻿using System.IO;
using System.Xml.Serialization;

namespace GDS_SERVER_WPF
{
    public class FileHandler
    {
        public static T Load<T>(string FileSpec)
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

        public static void Save<T>(T ToSerialize, string FileSpec)
        {
            Directory.CreateDirectory(FileSpec.Substring(0, FileSpec.LastIndexOf('\\')));
            var outFile = File.Create(FileSpec);
            var formatter = new XmlSerializer(typeof(T));

            formatter.Serialize(outFile, ToSerialize);
            outFile.Close();
        }

    }
}