using System;
using System.Threading;

namespace GDS_Client
{
    class Program
    {      
        static void Main(string[] args)
        {
            Repeater();
        }

        static void Repeater()
        {
            var listener = new Listener();
            listener.running = true;
            listener.StartListener();
            Thread.Sleep(5000);
            while (listener.running)
            {
                Thread.Sleep(10000);
            }
            Repeater();
        }
    }    
}