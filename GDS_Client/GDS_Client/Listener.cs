using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace GDS_Client
{
    public class Listener
    {
        static System.Net.Sockets.TcpClient clientSocket;        
        static byte[] dataStream = new byte[1024];
        public ComputerDetails computerDetails = new ComputerDetails();
        public bool running;
        public NetworkStream serverStream;
        public MessageHandler messageHandler;


        public void myReadCallBack(IAsyncResult ar)
        {
            var sendData = new Packet();
            try
            {
                Packet receivePacket = new Packet(dataStream);
                if (receivePacket.DataIdentifier != DataIdentifier.Null)
                {
                    var myNetworkStream = (NetworkStream)ar.AsyncState;
                    sendData.DataIdentifier = receivePacket.DataIdentifier;
                    sendData.MacAddress = receivePacket.MacAddress;
                    var message = receivePacket.Message;

                    messageHandler.HandleMessage(receivePacket);                    

                    myNetworkStream.BeginRead(dataStream, 0, dataStream.Length,
                                                              new AsyncCallback(myReadCallBack),
                                                              myNetworkStream);
                }
                else
                {
                    running = false;
                }
            }
            catch
            {
                Console.WriteLine("Server Offline repeat READING Crash");
                running = false;
            }
        }

        public void StartListener()
        {
            try
            {
                running = true;
                clientSocket = new System.Net.Sockets.TcpClient();
                //clientSocket.Connect("10.202.0.6", 100);
                //clientSocket.Connect("10.202.20.32", 100);
                //clientSocket.Connect("127.0.0.1", 100);
                clientSocket.Connect("192.168.0.73", 100);
                serverStream = clientSocket.GetStream();
                serverStream.BeginRead(dataStream, 0, dataStream.Length,
                                                   new AsyncCallback(myReadCallBack),
                                                   serverStream);                
                serverStream = clientSocket.GetStream();
                messageHandler = new MessageHandler(this);               
                computerDetails.SetComputerDetails();
                var result_string = String.Join("|..|", computerDetails.ToList().ToArray());
                SendMessage(DataIdentifier.SYN_FLAG, result_string);
            }
            catch
            {
                Console.WriteLine("Server Offline repeat");                
                running = false;
            }
        }

        public void SendMessage(DataIdentifier ID, string message)
        {            
            var sendData = new Packet(ID, computerDetails.macAddress, message);
            byte[] outStream = sendData.GetDataStream();
            serverStream.Write(outStream, 0, outStream.Length);
            serverStream.Flush();
        }
    }
}
