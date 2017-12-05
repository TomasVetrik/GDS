namespace GDS_SERVER_WPF
{
    public class ComputerConfigData
    {
        public string Name { get; set; }
        public string Workgroup { get; set; }

        public ComputerConfigData()
        { }

        public ComputerConfigData(string _name, string _workgroup)
        {
            this.Name = _name;
            this.Workgroup = _workgroup;
        }

    }
}