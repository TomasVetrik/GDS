using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GDS_SERVER_WPF.Handlers
{
    public class FileStreamer
    {
        string path;
        FileStream fs;
        public Int64 position;

        public FileStreamer(string file_to_read)
        {
            try
            {
                path = file_to_read;
                FileInfo fi = new FileInfo(path);
                if (fi.Exists)
                {
                    fs = File.OpenRead(path);
                }
                else
                {
                    fs = null;
                    Console.WriteLine("Subor Neexistuje!");
                }
                position = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error File Streamer: " + ex.Message);
            }
        }

        public long Length()
        {
            return fs.Length;
        }

        public byte[] GetSpecificChunk(Int64 number)
        {
            byte[] b = new byte[FilePiece.data_size];
            try
            {
                long cur_position = fs.Position;
                fs.Seek(number * FilePiece.data_size, SeekOrigin.Begin);
                int data_length = fs.Read(b, 0, b.Length);
                if (data_length <= 0)
                {
                    b = null;
                }
                else if (data_length < FilePiece.data_size)
                {
                    byte[] b2 = new byte[data_length];
                    Buffer.BlockCopy(b, 0, b2, 0, data_length);
                    return b2;
                }
                fs.Seek(cur_position, SeekOrigin.Begin);
                return b;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error File Streamer: " + ex.Message);
                return b;
            }
        }

        public byte[] GetNextChunk()
        {
            byte[] b2 = new byte[FilePiece.data_size];
            try
            {
                if (fs != null)
                {
                    int data_length = fs.Read(b2, 0, b2.Length);
                    byte[] b = new byte[data_length];
                    Buffer.BlockCopy(b2, 0, b, 0, data_length);

                    if (data_length > 0)
                    {
                        position += 1;
                        return b;
                    }
                }
                return null;
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error File Streamer: " + ex.Message);
                return null;
            }
        }

        public FilePiece GetNextPiece()
        {
            byte[] b = GetNextChunk();
            FilePiece piece = new FilePiece(1, 2, b); ;
            try
            {
                if (b != null)
                {
                    double preresult = fs.Position / FilePiece.data_size;
                    if (fs.Position % b.Length != 0)
                    {
                        preresult = preresult + 0.5;
                    }
                    double result = Math.Round(Convert.ToDouble(preresult - 1), MidpointRounding.AwayFromZero);


                    piece = new FilePiece(1, Convert.ToInt64(result), b);

                }
                else
                {
                    piece = null;
                }
                return piece;
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error File Streamer: " + ex.Message);
                return piece;
            }
        }

    }
}
