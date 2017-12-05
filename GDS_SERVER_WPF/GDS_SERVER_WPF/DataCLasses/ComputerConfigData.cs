using ProtoBuf;
using System.Collections.Generic;

namespace GDS_SERVER_WPF
{
    [ProtoContract]
    public class ComputerConfigData
    {
        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public string Workgroup { get; set; }
        [ProtoMember(3)]
        public List<string> PostInstalls{ get; set; }

        public ComputerConfigData()
        {
            this.PostInstalls = new List<string>();
        }

        public ComputerConfigData(string _name, string _workgroup)
        {
            this.Name = _name;
            this.Workgroup = _workgroup;
            this.PostInstalls = new List<string>();
        }
    }
}