using GDS_SERVER_WPF.DataCLasses;
using ProtoBuf;

namespace GDS_SERVER_WPF
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
        CLOSE,
        REFRESH_COMPUTER_DETAILS_DATA
    }

    [ProtoContract]
    public class Packet
    {
        [ProtoMember(1)]
        public FLAG ID { get; set; }
        [ProtoMember(2)]
        public TaskData taskData { get; set; }
        [ProtoMember(3)]
        public ComputerDetailsData computerDetailsData { get; set; }
        [ProtoMember(4)]
        public ComputerConfigData computerConfigData { get; set; }
        [ProtoMember(5)]
        public string clonningMessage { get; set; }

        public Packet()
        {
        }

        public Packet(FLAG _ID)
        {
            this.ID = _ID;
        }

        public Packet(FLAG _ID, ComputerDetailsData _computerDetailsData)
        {
            this.ID = _ID;
            this.computerDetailsData = _computerDetailsData;
        }

        public Packet(FLAG _ID, ComputerDetailsData _computerDetailsData, string _message)
        {
            this.ID = _ID;
            this.computerDetailsData = _computerDetailsData;
            this.clonningMessage = _message;
        }
    }
}
