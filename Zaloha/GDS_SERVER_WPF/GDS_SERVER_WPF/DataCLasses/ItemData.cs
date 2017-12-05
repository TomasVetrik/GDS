namespace GDS_SERVER_WPF.DataCLasses
{
    public class ItemData
    {
        public ItemData(string _Header, string _Content)
        {
            this.Header = _Header;
            this.Content = _Content;
        }
        public string Header { get; set; }
        public string Content { get; set; }
    }
}
