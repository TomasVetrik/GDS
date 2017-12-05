using GDS_Client.DataClasses;
using System.Collections.Generic;

namespace GDS_Client
{
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

        public string ImageSource { get; set; }
        public string Name { get; set; }
        public string LastExecuted { get; set; }
        public List<ComputerDetailsData> TargetComputers { get; set; }
        public string MachineGroup { get; set; }

        public bool WakeOnLan { get; set; }
        public bool Configuration { get; set; }
        public bool ForceInstall { get; set; }
        public bool ShutDown { get; set; }

        public bool Cloning { get; set; }
        public string BaseImageSourcePath { get; set; }
        public ImageData BaseImageData { get; set; }
        public bool WithoutVHD { get; set; }
        public string DriveEImageSourcePath { get; set; }
        public ImageData DriveEImageData { get; set; }

        public bool SoftwareAndFileAction { get; set; }
        public string SourceDirectoryInOS { get; set; }
        public string DestinationDirectoryInOS { get; set; }
        public List<string> CommandsInOS { get; set; }
        public List<string> CopyFilesInOS { get; set; }

        public bool SoftwareAndFileAction_WINPE { get; set; }
        public string SourceDirectoryInWINPE { get; set; }
        public string DestinationDirectoryInWINPE { get; set; }
        public List<string> CopyFilesInWINPE { get; set; }
        public List<string> CommandsInWINPE { get; set; }

        public int WaitingTime { get; set; }
    }
}