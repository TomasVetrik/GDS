using System;
using System.Xml.Serialization;

namespace GDS_Client
{
    public enum FLAG
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
        SHUTDOWN_DONE,
        START_COPY_FILES,
        FINISH_COPY_FILES,
        RUN_COMMAND,
        FINISH_RUN_COMMAND,
        FINISH_MESSAGE,
        LogOut,
        Login,
        SEND_TASK_CONFIG,
        SEND_CONFIG,
        CLOSE
    }
    [XmlRoot(ElementName = "Packet")]
    public class Packet
    {        
        public FLAG dataIdentifier { get; set; }
        public TaskData taskData { get; set; }
        public ComputerDetailsData computerDetailsData { get; set; }
        public DateTime IDTime { get; set; }
        public ComputerConfigData computerConfigData { get; set; }
        public string clonningMessage { get; set; }

        public Packet()
        {
            this.IDTime = DateTime.Now;
        }

        public Packet(FLAG _ID)
        {
            this.dataIdentifier = _ID;
            this.IDTime = DateTime.Now;
        }

        public Packet(FLAG _ID, ComputerDetailsData _computerDetailsData)
        {
            this.dataIdentifier = _ID;
            this.computerDetailsData = _computerDetailsData;
            this.IDTime = DateTime.Now;
        }

        public Packet(FLAG _ID, ComputerDetailsData _computerDetailsData, string _message)
        {            
            this.dataIdentifier = _ID;
            this.computerDetailsData = _computerDetailsData;
            this.clonningMessage = _message;
            this.IDTime = DateTime.Now;
        }

    }
}
