using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDS_SERVER_WPF
{
    public class MachinesGroupsData
    {
        public MachinesGroupsData(string _name, string _macAddress, string _IPAddress, string _realPCName, string _detail, string _imageSource = "Images/Offline.ico")
        {
            this.ImageSource = _imageSource;
            this.Name = _name;
            this.MacAddress = _macAddress;
            this.IPAddress = _IPAddress;
            this.RealPCName = _realPCName;
            this.Detail = _detail;
        }

        private string _name;
        public string Name
        {
            get { return this._name; }
            set { this._name = value; }
        }
        private string _imageSource;
        public string ImageSource
        {
            get { return this._imageSource; }
            set { this._imageSource = value; }
        }

        private string _macAddress;
        public string MacAddress
        {
            get { return this._macAddress; }
            set { this._macAddress = value; }
        }

        private string _IPAddress;
        public string IPAddress
        {
            get { return this._IPAddress; }
            set { this._IPAddress = value; }
        }

        private string _realPCName;
        public string RealPCName
        {
            get { return this._realPCName; }
            set { this._realPCName = value; }
        }

        private string _detail;
        public string Detail
        {
            get { return this._detail; }
            set { this._detail = value; }
        }

    }
}
