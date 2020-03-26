using ProtoBuf;
using System.Collections.Generic;

namespace GDS_Client
{
    [ProtoContract]
    public class ComputerConfigData
    {
        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public string Workgroup { get; set; }

        public ComputerConfigData()
        {
        }

        public ComputerConfigData(string _name, string _workgroup)
        {
            this.Name = _name;
            this.Workgroup = _workgroup;
        }
    }
}