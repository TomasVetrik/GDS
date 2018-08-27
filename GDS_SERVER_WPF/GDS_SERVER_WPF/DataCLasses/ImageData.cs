using ProtoBuf;
using System;
using System.Collections.Generic;

namespace GDS_SERVER_WPF.DataCLasses
{
    [ProtoContract]
    public class ImageData
    {
        public ImageData()
        {
            Name = "";
            this.ImageSource = "Images/Image.ico";
            SourcePath = "";
            BoolLabel = "";
            PartitionSize = 90;
            VHDResizeSize = 50;
            VHDResize = false;
            VHDNames = new List<string>();
            OSAbrivations = new List<string>();
        }

        public ImageData(string _name, string _imageSource = "Images/Image.ico")
        {
            this.ImageSource = _imageSource;
            this.Name = _name;
            VHDNames = new List<string>();
            OSAbrivations = new List<string>();
        }

        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public string ImageSource { get; set; }
        [ProtoMember(3)]
        public string SourcePath { get; set; }
        [ProtoMember(4)]
        public string BoolLabel { get; set; }
        [ProtoMember(5)]
        public int PartitionSize { get; set; }
        [ProtoMember(6)]
        public int VHDResizeSize { get; set; }
        [ProtoMember(7)]
        public bool VHDResize { get; set; }
        [ProtoMember(8)]
        public List<string> VHDNames { get; set; }
        [ProtoMember(9)]
        public List<string> OSAbrivations { get; set; }

        public void LoadDataFromList(List<string> list)
        {
            foreach (string line in list)
            {
                if (line != "")
                {
                    if (line.Contains("Image Source Path||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        SourcePath = splitter[1];
                    }
                    if (line.Contains("Partition Size||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        PartitionSize = Convert.ToInt32(splitter[1])/1000;
                    }
                    if (line.Contains("Boot Label||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        BoolLabel = splitter[1];
                    }
                    if (line.Contains("OS Abrivation||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        if(splitter[1].Contains("&"))
                        {
                            foreach(string OSAbrv in splitter[1].Split('&'))
                            {
                                OSAbrivations.Add(OSAbrv.ToUpper());
                            }
                        }
                        else
                        {
                            OSAbrivations.Add(splitter[1].ToUpper());
                        }
                    }
                    if (line.Contains("VHD Name||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        if (splitter[1].Contains("&"))
                        {
                            foreach (string OSAbrv in splitter[1].Split('&'))
                            {
                                VHDNames.Add(OSAbrv);
                            }
                        }
                        else
                        {
                            VHDNames.Add(splitter[1]);
                        }
                    }
                    if (line.Contains("OS Size||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        VHDResizeSize = Convert.ToInt32(splitter[1])/1000;
                    }
                    if (line.Contains("ExtendSizeOS||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        VHDResize = Convert.ToBoolean(splitter[1]);
                    }
                }
            }
        }
    }
}
