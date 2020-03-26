using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDS_Client
{
    public class FilePiece
    {
        /* 2 bytes: state
         * 8 bytes: piece number (Int64)
         * 
         * rest: data
         */

        //public const int data_size = 16384;
        public const int data_size = 16384;
        public const int header_size = 10;

        public byte[] data;

        public Int16 state;
        public Int64 position;

        public FilePiece(Int16 state1, Int64 position1, byte[] file_data)
        {
            data = file_data;
            position = position1;
            state = state1;
        }

        public int get_packet_size()
        {
            return header_size + data.Length;
        }

        public byte[] get_data()
        {
            return data;
        }

        public byte[] get_packet()
        {
            try
            {
                byte[] status = System.BitConverter.GetBytes(state);
                byte[] position_number = System.BitConverter.GetBytes(position);

                byte[] b = new byte[header_size + data.Length]; //  + data_size

                status.CopyTo(b, 0);
                position_number.CopyTo(b, 2);
                data.CopyTo(b, header_size);

                return b;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Piece: " + ex.Message);
                return new byte[0];
            }
        }

        static public FilePiece parse_packet(byte[] packet)
        {
            try
            {
                if (packet.Length < header_size)
                { // paket je prilis kratky
                    return null;
                }
                Int32 data_length = packet.Length - 10;
                if (data_length < 0)
                { // subor bez dat
                    return null;
                }

                Int16 status = System.BitConverter.ToInt16(packet, 0);
                Int64 position_number = System.BitConverter.ToInt64(packet, 2);

                byte[] data = new byte[data_length];

                Buffer.BlockCopy(packet, 10, data, 0, data_length);

                FilePiece piece = new FilePiece(status, position_number, data);

                return piece;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Piece: " + ex.Message);
                return new FilePiece(1, 0, new byte[0]);
            }
        }
    }
}
