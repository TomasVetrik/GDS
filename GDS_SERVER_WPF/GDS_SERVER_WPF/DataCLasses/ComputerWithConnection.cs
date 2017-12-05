using NetworkCommsDotNet.Connections;

namespace GDS_SERVER_WPF.DataCLasses
{
    public class ComputerWithConnection
    {
        public ComputerDetailsData ComputerData { get; set; }
        public Connection connection { get; set; }
    }
}
