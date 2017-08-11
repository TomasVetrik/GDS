using System;
using System.Collections.Generic;
using System.Text;

namespace GDS_Client
{
    public enum DataIdentifier
    {
        Null,
        SYN_FLAG_WINPE,
        SYN_FLAG,
        START_TASK,
        ERROR_MESSAGE,
        CLIENT_TO_WINPE,
        CLONE_BASE_IMAGE_CONFIG,
        CLONE_DRIVEE_IMAGE_CONFIG,
        CLONING,
        CLONING_DONE,
        CLONING_ERROR,
        CLONING_SUCCESSFUL,
        CLONING_STATUS,
        CONFIG,
        TO_OPERATING_SYSTEM,
        RESTART_AFTER_CLONE,
        RESTART,
        SHUTDOWN,
        START_COPY_FILES,
        FINISH_COPY_FILES,
        RUN_COMMAND,
        FINISH_RUN_COMMAND,
        FINISH_MESSAGE,
        LogOut,
        Login,
        SEND_CONFIG,
        CLOSE
    }

    public class Packet
    {
        #region Private Members
        private DataIdentifier dataIdentifier;
        private string macAddress;
        private string message;
        #endregion

        #region Public Properties
        public DataIdentifier DataIdentifier
        {
            get { return dataIdentifier; }
            set { dataIdentifier = value; }
        }

        public string MacAddress
        {
            get { return macAddress; }
            set { macAddress = value; }
        }

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
        #endregion

        #region Methods

        // Default Constructor
        public Packet(DataIdentifier ID = DataIdentifier.Null, string _macAddress = "", string _message = "")
        {
            this.dataIdentifier = ID;
            this.message = _message;
            this.macAddress = _macAddress;
        }

        public Packet(byte[] dataStream)
        {
            // Read the data identifier from the beginning of the stream (4 bytes)
            this.dataIdentifier = (DataIdentifier)BitConverter.ToInt32(dataStream, 0);

            // Read the length of the name (4 bytes)
            int nameLength = BitConverter.ToInt32(dataStream, 4);

            // Read the length of the message (4 bytes)
            int msgLength = BitConverter.ToInt32(dataStream, 8);

            // Read the name field
            if (nameLength > 0)
                this.macAddress = Encoding.UTF8.GetString(dataStream, 12, nameLength);
            else
                this.macAddress = null;

            // Read the message field
            if (msgLength > 0)
                this.message = Encoding.UTF8.GetString(dataStream, 12 + nameLength, msgLength);
            else
                this.message = null;
        }

        // Converts the packet into a byte array for sending/receiving 
        public byte[] GetDataStream()
        {
            List<byte> dataStream = new List<byte>();

            // Add the dataIdentifier
            dataStream.AddRange(BitConverter.GetBytes((int)this.dataIdentifier));

            // Add the name length
            if (this.macAddress != null)
                dataStream.AddRange(BitConverter.GetBytes(this.macAddress.Length));
            else
                dataStream.AddRange(BitConverter.GetBytes(0));

            // Add the message length
            if (this.message != null)
                dataStream.AddRange(BitConverter.GetBytes(this.message.Length));
            else
                dataStream.AddRange(BitConverter.GetBytes(0));

            // Add the name
            if (this.macAddress != null)
                dataStream.AddRange(Encoding.UTF8.GetBytes(this.macAddress));

            // Add the message
            if (this.message != null)
                dataStream.AddRange(Encoding.UTF8.GetBytes(this.message));

            return dataStream.ToArray();
        }

        #endregion
    }
    }
