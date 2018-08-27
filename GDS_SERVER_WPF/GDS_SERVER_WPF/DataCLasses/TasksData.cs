using GDS_SERVER_WPF.DataCLasses;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace GDS_SERVER_WPF
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
            this.TargetComputers = new List<ComputerDetailsData>();
            this.CopyFilesInOS = new List<string>();
            this.CopyFilesInWINPE = new List<string>();
            this.CommandsInOS = new List<string>();
            this.CommandsInWINPE = new List<string>();
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


        public void LoadDataFromList(List<string> list)
        {
            bool targetDirectory = false;
            bool targetDirectory_WINPE = false;
            bool commands = false;
            bool commands_WINPE = false;
            foreach (string line in list)
            {
                if (line != "")
                {
                    if (line.Contains("LastExecution||"))
                    {
                        LastExecuted = "NONE";
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        if (splitter[1] != "")
                        {
                            LastExecuted = splitter[1];
                        }
                    }
                    if (line.Contains("WaitingTime||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        WaitingTime = Convert.ToInt16(splitter[1]);
                    }
                    if (line.Contains("Clone||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        if (Convert.ToBoolean(splitter[1]))
                        {
                            Cloning = true;
                        }
                        else
                        {
                            Cloning = false;
                        }
                    }
                    if (line.Contains("WOL||"))
                    {
                        commands = false;
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        if (Convert.ToBoolean(splitter[1]))
                        {
                            WakeOnLan = true;
                        }
                        else
                        {
                            WakeOnLan = false;
                        }
                    }
                    if (line.Contains("Force Install||"))
                    {
                        targetDirectory = false;
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        if (Convert.ToBoolean(splitter[1]))
                        {
                            ForceInstall = true;
                        }
                        else
                        {
                            ForceInstall = false;
                        }
                    }
                    if (line.Contains("WithoutVHD||"))
                    {
                        targetDirectory = false;
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        if (Convert.ToBoolean(splitter[1]))
                        {
                            WithoutVHD = true;
                        }
                        else
                        {
                            WithoutVHD = false;
                        }
                    }
                    if (line.Contains("Shutdown||"))
                    {
                        commands_WINPE = false;
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        if (Convert.ToBoolean(splitter[1]))
                        {
                            ShutDown = true;
                        }
                        else
                        {
                            ShutDown = false;
                        }
                    }
                    if (line != "")
                    {
                        if (commands)
                        {
                            CommandsInOS.Add(line);
                        }
                        if (commands_WINPE)
                        {
                            CommandsInWINPE.Add(line);
                        }
                        if (targetDirectory)
                        {
                            CopyFilesInOS.Add(line);
                        }
                    }
                    if (line.Contains("Commands||"))
                    {
                        targetDirectory_WINPE = false;
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        CommandsInOS.Add(splitter[1]);
                        commands = true;
                    }
                    if (line != "")
                    {
                        if (targetDirectory_WINPE)
                        {
                            CopyFilesInWINPE.Add(line);
                        }
                    }
                    if (line.Contains("Commands(WINPE)||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        CommandsInWINPE.Add(splitter[1]);
                        commands_WINPE = true;
                    }
                    if (line.Contains("SAFA||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        if (Convert.ToBoolean(splitter[1]))
                        {
                            SoftwareAndFileAction = true;
                        }
                        else
                        {
                            SoftwareAndFileAction = false;
                        }
                    }
                    if (line.Contains("SAFA(WINPE)||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        if (Convert.ToBoolean(splitter[1]))
                        {
                            SoftwareAndFileAction_WINPE = true;
                        }
                        else
                        {
                            SoftwareAndFileAction_WINPE = false;
                        }
                    }
                    if (line.Contains("Image Source Path||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        BaseImageSourcePath = splitter[1].Replace("Images","Base");
                    }
                    if (line.Contains("DriveE Source Path||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        DriveEImageSourcePath = splitter[1];
                    }
                    if (line.Contains("Configuration||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        if (Convert.ToBoolean(splitter[1]))
                        {
                            Configuration = true;
                        }
                        else
                        {
                            Configuration = false;
                        }
                    }
                    if (line.Contains("TargetDirectory||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        DestinationDirectoryInOS = splitter[1];
                        targetDirectory = true;
                    }
                    if (line.Contains("TargetDirectory(WINPE)||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        DestinationDirectoryInWINPE = splitter[1];
                        targetDirectory_WINPE = true;
                    }
                    if (line.Contains("SourceDirectory||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        SourceDirectoryInOS = splitter[1];
                    }
                    if (line.Contains("SourceDirectory(WINPE)||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        SourceDirectoryInWINPE = splitter[1];
                    }
                }            
            }
        }
    }
}
