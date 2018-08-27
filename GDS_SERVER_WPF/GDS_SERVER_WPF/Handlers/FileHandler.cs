﻿using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Xml.Serialization;

namespace GDS_SERVER_WPF
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
                if (ex.ToString().Contains("error in XML"))
                {
                    try
                    {
                        if (File.Exists(FileSpec))
                        {

                            File.Delete(FileSpec);

                            MessageBox.Show(FileSpec + " IS CORRUPTED");
                        }
                    }
                    catch { }
                    return default(T);
                }
                return Load<T>(FileSpec);
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
                    Save<T>(ToSerialize, FileSpec, counter++);
            }
        }

    }
}
