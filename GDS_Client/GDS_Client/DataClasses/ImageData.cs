using ProtoBuf;
using System.Collections.Generic;

namespace GDS_Client.DataClasses
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
    }
}
