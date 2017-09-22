using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace GDS_Client
{
    class Program
    {      
        static void Main(string[] args)
        {
            Repeater();
        }
        static Listener listener;
        static void Repeater()
        {
            handler = new ConsoleEventDelegate(ConsoleEventCallback);
            SetConsoleCtrlHandler(handler, true);
            listener = new Listener();
            listener.running = true;
            listener.StartListener();
            Thread.Sleep(5000);
            while (listener.running)
            {
                Thread.Sleep(10000);
            }
            Repeater();
        }

        static bool ConsoleEventCallback(int eventType)
        {
            try
            {
                if (listener.clientSocket != null)
                {
                    try {
                        listener.SendMessage(new Packet(DataIdentifier.CLOSE, listener.computerDetails.computerDetailsData));
                    }
                    catch { }
                    listener.serverStream.Close();
                    listener.clientSocket.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Closing Error: " + ex.Message, "UDP Client");
            }            
            return false;
        }
        static ConsoleEventDelegate handler;   // Keeps it from getting garbage collected
                                               // Pinvoke
        private delegate bool ConsoleEventDelegate(int eventType);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleCtrlHandler(ConsoleEventDelegate callback, bool add);
    }    
}