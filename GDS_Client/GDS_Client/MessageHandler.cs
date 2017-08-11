using System;

namespace GDS_Client
{
    public class MessageHandler
    {
        public Listener listener;

        public MessageHandler(Listener _listener)
        {
            this.listener = _listener;
        }

        public void HandleMessage(Packet data)
        {
            switch (data.DataIdentifier)
            {
                case DataIdentifier.SYN_FLAG:
                    {
                        Console.WriteLine("ACK from server");
                        break;
                    }
                case DataIdentifier.CLOSE:
                    {
                        Console.WriteLine("Close client application");
                        Environment.Exit(0);
                        break;
                    }
            }
        }
    }
}
