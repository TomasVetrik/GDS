using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace GDS_SERVER_WPF.Handlers
{
    public class TCP_UNICAST
    {
        public List<string> TempFiles;
        public string destinationDir;
        public string srcDirName;
        public string ipAddress;
        public int PORT;
        public bool error;
        public bool cancel;
        TcpListener Listener;
        TcpClient client;
        Socket socket;
        IPAddress IP;
        NetworkStream netstream;

        public TCP_UNICAST(List<string> _TempFiles, string _destinationDir, string _srcDirName, string _ipAddress, int _PORT)
        {
            error = false;
            cancel = false;
            TempFiles = _TempFiles;
            destinationDir = _destinationDir;
            srcDirName = _srcDirName;
            ipAddress = _ipAddress;
            PORT = _PORT;
            IP = IPAddress.Parse(ipAddress);            
        }

        string DestinationFileName(string SendingFile)
        {
            String destFileName = "";
            try
            {
                String[] Splitter = null;
                
                String DestPath = "";

                Splitter = SendingFile.Split('\\');

                destFileName = Splitter[Splitter.Length - 1];

                Splitter = SendingFile.Replace(srcDirName, "").Split('\\');

                DestPath = destinationDir + "\\";

                for (int i = 0; i < Splitter.Length - 1; i++)
                {
                    DestPath += Splitter[i] + "\\";
                }

                destFileName = System.IO.Path.Combine(DestPath, destFileName);
            }
            catch(Exception ex)
            {
                MessageBox.Show("Chyba destination file name: " + ex.ToString());
            }
            return destFileName;
        }


        public void CreateConnection()
        {
            try
            {
                Listener = new TcpListener(IPAddress.Any, PORT);
                Listener.Start();
                socket = Listener.AcceptSocket();                
            }
            catch (SocketException ex)
            {
                Thread.Sleep(1000);
                PORT++;
                Console.WriteLine(ex.ToString());
                if (!cancel && !error)
                {
                    CreateConnection();
                }
            }
            catch (Exception ex)
            {                
                Thread.Sleep(1000);
                Console.WriteLine(ex.ToString());
                if (!cancel && !error)
                {
                    CreateConnection();
                }
            }
        }

        public void DestroyConnection()
        {
            try
            {
                Listener.Stop();
                client.Close();
                socket.Close();
                netstream.Close();
            }
            catch
            {
                Console.WriteLine("Destroy Connection failed");
            }
        }

        public void SendSynchInfo(string info)
        {
            try
            {
                byte[] data = new byte[FilePiece.data_size];
                int k = socket.Receive(data);
                byte[] data_Upgrade = new byte[k];
                Buffer.BlockCopy(data, 0, data_Upgrade, 0, k);
                string string_Data = System.Text.Encoding.UTF8.GetString(data_Upgrade);
                if (string_Data == "Ready")
                {
                    socket.Send(System.Text.Encoding.UTF8.GetBytes(info));
                }
                else
                {
                }
            }
            catch { }
        }

        public void SendFileSendingOver()
        {
            ASCIIEncoding AsciiE = new ASCIIEncoding();
            socket.Send(AsciiE.GetBytes("OVER"));
        }

        public void SendingFiles()
        {
            try
            {
                CreateConnection();
                SendSynchInfo("TOTAL FILES||" + TempFiles.Count.ToString());
                foreach (string SendingFile in TempFiles)
                {
                    FileInfo f = new FileInfo(SendingFile);
                    long fileLength = f.Length;                    
                    SendSynchInfo(DestinationFileName(SendingFile) + "||" + fileLength.ToString());                    
                    if (!SendingTCP(SendingFile))
                    {
                        break;
                    }
                }
                SendSynchInfo("OVER SENDING");
                DestroyConnection();
            }
            catch { error = true; }
        }

        public bool SendingTCP(string SendingFile)
        {
            try
            {
                client = new TcpClient(ipAddress, PORT + 1000);
                netstream = client.GetStream();
                FileStreamer file_stream = new FileStreamer(SendingFile);
                FileInfo f = new FileInfo(SendingFile);
                file_stream = new FileStreamer(SendingFile);
                byte[] bytes = new byte[FilePiece.data_size];
                FilePiece fp = file_stream.GetNextPiece();
                if (fp != null) bytes = fp.data;
                while (fp != null && bytes != null)
                {                    
                    netstream.Write(bytes, 0, (int)bytes.Length);
                    fp = file_stream.GetNextPiece();
                    if (fp != null) bytes = fp.data;
                }
                netstream.Write(new byte[0], 0, 0);
                netstream.Close();
                client.Close();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
