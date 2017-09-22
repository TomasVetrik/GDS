using System;

namespace GDS_SERVER_WPF
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

    public class Packet
    {
        public DataIdentifier dataIdentifier { get; set; }
        public TaskData taskData { get; set; }
        public ComputerDetailsData computerDetailsData { get; set; }
        public DateTime IDTime { get; set; }
        public ComputerConfigData computerConfigData { get; set; }
        public string clonningMessage { get; set; }

        public Packet()
        {
            this.IDTime = DateTime.Now;
        }

        public Packet(DataIdentifier _ID)
        {
            this.dataIdentifier = _ID;
            this.IDTime = DateTime.Now;
        }       
    }
}
