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
            Console.WriteLine("Nejak som sa sem dostal");
            Console.ReadLine();
        }
        static Listener listener;
        static void Repeater()
        {
            try
            {
                Console.WriteLine("START");
                listener = new Listener();
                listener.running = false;
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
            Thread.Sleep(10000);
            Repeater();
        }
    }    
}