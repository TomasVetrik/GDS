﻿using System.Collections.Generic;

namespace GDS_SERVER_WPF.DataCLasses
{
    public class ImageData
    {
        public ImageData()
        {            
            this.ImageSource = "Images/Tasks.png";     
        }

        public ImageData(string _name, string _imageSource = "Images/Tasks.png")
        {
            this.ImageSource = _imageSource;
            this.Name = _name;
        }

        public string Name { get; set; }
        public string ImageSource { get; set; }
        public string SourcePath { get; set; }
        public string BoolLabel { get; set; }
        public int PartitionSize { get; set; }
        public int VHDResizeSize { get; set; }
        public bool VHDResize { get; set; }
        public List<string> VHDNames { get; set; }
        public List<string> OSAbrivations { get; set; }
    }
}