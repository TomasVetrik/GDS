using ProtoBuf;

namespace GDS_SERVER_WPF.DataCLasses
{
    [ProtoContract]
    public class RDPLastUsedSettings
    {
        [ProtoMember(1)]
        public string GateWay { get; set; }
        [ProtoMember(2)]
        public string RDSLogin{ get; set; }
        [ProtoMember(3)]
        public string RDSPassword { get; set; }
        [ProtoMember(4)]
        public string LocalLogin { get; set; }
        [ProtoMember(5)]
        public string LocalPassword { get; set; }

        public RDPLastUsedSettings()
        {            
        }
    }
}
