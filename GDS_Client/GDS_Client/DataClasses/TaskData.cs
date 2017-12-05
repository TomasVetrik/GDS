using GDS_Client.DataClasses;
using ProtoBuf;
using System.Collections.Generic;

namespace GDS_Client
{
    [ProtoContract]
    public class TaskData
    {
        public TaskData()
        {
            this.LastExecuted = "NONE";
            this.ImageSource = "Images/Tasks.ico";
            this.MachineGroup = "NONE";
            this.TargetComputers = new List<ComputerDetailsData>();
            this.CopyFilesInOS = new List<string>();
            this.CopyFilesInWINPE = new List<string>();
        }

        public TaskData(string _name, string _lastExecuted, string _machineGroups, List<ComputerDetailsData> _computers, string _imageSource = "Images/Tasks.ico")
        {
            this.ImageSource = _imageSource;
            this.Name = _name;
            this.LastExecuted = _lastExecuted;
            this.MachineGroup = _machineGroups;
            this.TargetComputers = _computers;
        }

        [ProtoMember(1)]
        public string ImageSource { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
        [ProtoMember(3)]
        public string LastExecuted { get; set; }
        [ProtoMember(4)]
        public List<ComputerDetailsData> TargetComputers { get; set; }
        [ProtoMember(5)]
        public string MachineGroup { get; set; }
        [ProtoMember(6)]
        public bool WakeOnLan { get; set; }
        [ProtoMember(7)]
        public bool Configuration { get; set; }
        [ProtoMember(8)]
        public bool ForceInstall { get; set; }
        [ProtoMember(9)]
        public bool ShutDown { get; set; }
        [ProtoMember(10)]
        public bool Cloning { get; set; }
        [ProtoMember(11)]
        public string BaseImageSourcePath { get; set; }
        [ProtoMember(12)]
        public ImageData BaseImageData { get; set; }
        [ProtoMember(13)]
        public bool WithoutVHD { get; set; }
        [ProtoMember(14)]
        public string DriveEImageSourcePath { get; set; }
        [ProtoMember(15)]
        public ImageData DriveEImageData { get; set; }
        [ProtoMember(16)]
        public bool SoftwareAndFileAction { get; set; }
        [ProtoMember(17)]
        public string SourceDirectoryInOS { get; set; }
        [ProtoMember(18)]
        public string DestinationDirectoryInOS { get; set; }
        [ProtoMember(19)]
        public List<string> CommandsInOS { get; set; }
        [ProtoMember(20)]
        public List<string> CopyFilesInOS { get; set; }
        [ProtoMember(21)]
        public bool SoftwareAndFileAction_WINPE { get; set; }
        [ProtoMember(22)]
        public string SourceDirectoryInWINPE { get; set; }
        [ProtoMember(23)]
        public string DestinationDirectoryInWINPE { get; set; }
        [ProtoMember(24)]
        public List<string> CopyFilesInWINPE { get; set; }
        [ProtoMember(25)]
        public List<string> CommandsInWINPE { get; set; }
        [ProtoMember(26)]
        public int WaitingTime { get; set; }
    }
}