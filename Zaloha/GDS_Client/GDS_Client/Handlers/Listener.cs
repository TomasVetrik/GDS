using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml.Serialization;

namespace GDS_Client
{
    public class Listener
    {
        public  System.Net.Sockets.TcpClient clientSocket;
        static int length = 60000;  
        static byte[] dataStream = new byte[length];
        public ComputerDetails computerDetails = new ComputerDetails();
        public bool running;
        public Stream serverStream;
        public MessageHandler messageHandler;
        public string serverIP;
        public int serverPORT;

        XmlSerializer xs = new XmlSerializer(typeof(Packet));
        
        public void myReadCallBack(IAsyncResult ar)
        {
            try
            {
                using (MemoryStream memStream = new MemoryStream(dataStream, 0, dataStream.Length, false))
                {
                    bool hasAllZeroes = dataStream.All(singleByte => singleByte == 0);
                    var myNetworkStream = (NetworkStream)ar.AsyncState;
                    if (!hasAllZeroes)
                    {                        
                        try
                        {
                            var packet = xs.Deserialize(memStream) as Packet;
                            if (packet.dataIdentifier != FLAG.Null)
                            {
                                messageHandler.HandleMessage(packet);
                            }
                        }
                        catch (Exception ex)
                        {                            
                            Console.WriteLine("Problem with parse: " + ex);
                        }
                    }
                    myNetworkStream.BeginRead(dataStream, 0, dataStream.Length,
                                                              new AsyncCallback(myReadCallBack),
                                                              myNetworkStream);
                }                
            }
            catch
            {
                running = false;
                Console.WriteLine("Something is wrong with socket reading");
            }
        }

        public void StartListener()
        {
            try
            {
                running = true;
                serverIP = "10.202.20.32";
                serverPORT = 65452;
                serverIP = "10.202.0.6";
//              serverIP = "127.0.0.1";

                clientSocket = new System.Net.Sockets.TcpClient();                               
                clientSocket.Connect(serverIP, serverPORT);
                serverStream = clientSocket.GetStream();    
                serverStream.BeginRead(dataStream, 0, dataStream.Length,
                                                       new AsyncCallback(myReadCallBack),
                                                       serverStream);                
                messageHandler = new MessageHandler(this);               
                computerDetails.SetComputerDetails();
                FLAG ID = FLAG.SYN_FLAG;
                if(computerDetails.computerDetailsData.inWinpe)
                {
                    ID = FLAG.SYN_FLAG_WINPE;
                }
                Packet packet = new Packet(ID, computerDetails.computerDetailsData);
                packet.computerDetailsData = computerDetails.computerDetailsData;
                SendMessage(packet);
                Console.WriteLine("Connected on IP: " + serverIP + " PORT: " + serverPORT);           
            }
            catch (Exception e)
            {
                Console.WriteLine("Server Offline repeat: " + e.ToString());
                //Console.WriteLine("Server Offline repeat");
                running = false;
            }
        }

        public void SendMessage(Packet packet)
        {
            try
            {                
                IAsyncResult result;
                Action action = () =>
                {
                    byte[] bytes = new byte[length];
                    using (MemoryStream memStream = new MemoryStream(bytes))
                    {
                        xs.Serialize(memStream, packet);
                        memStream.WriteTo(serverStream);
                    }
                };

                result = action.BeginInvoke(null, null);

                if (!result.AsyncWaitHandle.WaitOne(10000))
                {
                    Console.WriteLine("Server timed out.");
                    StartListener();
                }
                            
                Thread.Sleep(100);
            }
            catch ( Exception ex)
            {
                running = false;
                Console.WriteLine("Server OFF: " +ex);
                Thread.Sleep(500);
            }
        }        
    }
}
