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
            try
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
            }
            catch(Exception ex)
            {
                Console.WriteLine("Problem with repeater: " + ex.ToString());
            }
            Repeater();
        }

        static bool ConsoleEventCallback(int eventType)
        {
            try
            {
                if (listener.connection != null)
                {
                    try {
                        listener.connection.CloseConnection(true);
                    }
                    catch { }
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