using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GDS_Client.Handlers
{
    public class TCP_UNICAST
    {
        public int PORT;
        public string srcDirName;
        public bool cancel;
        public bool error;
        public string Message = "";
        TcpListener Listener;
        TcpClient tcpClient;
        Stream socket;
        IPAddress IP;
        TcpClient client;
        NetworkStream netstream;

        public TCP_UNICAST(string ipAddress, int _PORT)
        {
            PORT = _PORT;
            IP = IPAddress.Parse(ipAddress);
            error = false;
            cancel = false;
            CreateConnection();
            Receiving();
        }

        public void ReceiveFile(string FileName, long FileLength)
        {
            FileStream Fs = null;
            try
            {
                byte[] RecData = new byte[FilePiece.data_size];
                int RecBytes;
                client = Listener.AcceptTcpClient();
                netstream = client.GetStream();   
                if (FileName != string.Empty)
                {
                    Fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write);
                    while ((RecBytes = netstream.Read(RecData, 0, RecData.Length)) > 0)
                    {                        
                        Fs.Write(RecData, 0, RecBytes);
                    }
                    Fs.Close();
                    netstream.Close();
                    client.Close();
                }
            }
            catch (IOException ex)
            {
                Fs.Close();
                DestroyConnection();
                Message = "COPY: Not enough space on disk: " + ex;
                Console.WriteLine(Message);
                error = true;
            }
            catch
            {
                error = true;
                Fs.Close();
            }
        }

        public void Receiving()
        {
            try
            {
                string str_data = "";
                string FilesCount = "";
                while ((str_data = SendReady()) != "OVER SENDING")
                {
                    if (str_data.Contains("TOTAL FILES||"))
                    {
                        FilesCount = str_data.Split(new string[] { "||" }, StringSplitOptions.None)[1];
                        Console.WriteLine("COPY: TOTAL FILES = " + FilesCount);
                    }
                    else
                    {                        
                        string FileName = str_data.Split(new string[] { "||" }, StringSplitOptions.None)[0];                        
                        long FileLength = Convert.ToInt64(str_data.Split(new string[] { "||" }, StringSplitOptions.None)[1]);
                        VytvorPriecinok(FileName);
                        ReceiveFile(FileName, FileLength);
                    }
                }
                DestroyConnection();
            }
            catch { error = true; }
        }

        public void CreateConnection()
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(IP, PORT);
                socket = tcpClient.GetStream();
                Listener = new TcpListener(IPAddress.Any, PORT + 1000);
                Listener.Start();
            }
            catch
            {
                Console.WriteLine("COPY: Waiting for server");
                Thread.Sleep(1000);
                CreateConnection();
            }
        }

        public string SendReady()
        {
            try
            {
                String str = "Ready";
                ASCIIEncoding asen = new ASCIIEncoding();
                byte[] ba = asen.GetBytes(str);

                socket.Write(ba, 0, ba.Length);

                byte[] data = new byte[FilePiece.data_size];
                int k = socket.Read(data, 0, FilePiece.data_size);
                byte[] data_Upgrade = new byte[k];
                Buffer.BlockCopy(data, 0, data_Upgrade, 0, k);
                string string_Data = System.Text.Encoding.UTF8.GetString(data_Upgrade);

                if (string_Data == "OVER SENDING")
                {
                    return "OVER SENDING";
                }
                return string_Data;
            }
            catch { error = true; return "OVER SENDING"; }
        }

        public void DestroyConnection()
        {
            try
            {
                socket.Close();
                tcpClient.Close();
                Listener.Stop();
                netstream.Close();
                client.Close();
            }
            catch
            {
                Console.WriteLine("COPY: Destroy Connection failed");
            }
        }


        private void buttonCancel_Click(object sender, EventArgs e)
        {
            cancel = true;
            DestroyConnection();
        }

        void VytvorPriecinok(string path)
        {
            try
            {
                string subor = Path.GetFileName(path);
                path = path.Replace(subor, "");
                if (path.Substring(0, 1) == @"\")
                {
                    path = path.Substring(1, path.Length - 1);
                }
                if (!(Directory.Exists(path)))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch
            {
                error = true;
                Message = "COPY: Nemozem vytvorit priecinok";
                Console.WriteLine(Message);
                DestroyConnection();
            }
        }


    }
}
