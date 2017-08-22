using System.Collections.Generic;

namespace GDS_Client.DataClasses
{
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
