using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace GDS_SERVER_WPF
{
    public class TaskData
    {
        public TaskData()
        {
            this.LastExecuted = "NONE";
            this.ImageSource = "Images/Tasks.png";
            this.MachineGroup = "NONE";
            this.TargetComputers = new List<string>();
        }

        public TaskData(string _name, string _lastExecuted, string _machineGroups, List<string> _computers, string _imageSource = "Images/Tasks.png")
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
        public List<string> TargetComputers { get; set; }
        public string MachineGroup { get; set; }

        public bool WakeOnLan { get; set; }
        public bool Configuration { get; set; }
        public bool ForceInstall { get; set; }
        public bool ShutDown { get; set; }
       
        public bool Cloning { get; set; }
        public string BaseImageSourcePath { get; set; }
        public bool WithoutVHD { get; set; }
        public string DriveEImageSourcePath { get; set; }

        public bool SoftwareAndFileAction { get; set; }
        public string SourceDirectoryInOS { get; set; }
        public string DestinationDirectoryInOS { get; set; }
        public List<string> CommnadsInOS { get; set; }
        public List<string> CopyFilesInOS { get; set; }

        public bool SoftwareAndFileAction_WINPE { get; set; }
        public string SourceDirectoryInWINPE { get; set; }
        public string DestinationDirectoryInWINPE { get; set; }
        public List<string> CopyFilesInWINPE { get; set; }
        public List<string> CommnadsInWINPE { get; set; }

        public int WaitingTime { get; set; }
    }
}
