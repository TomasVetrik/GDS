namespace GDS_SERVER_WPF.DataCLasses
{
    public class ExecutedTaskData
    {
        public string name { get; set; }
        public int status { get; set; }
        public string started { get; set; }
        public string finished { get; set; }
        public int clients { get; set; }
        public int clientsDone { get; set; }
        public string machineGroup { get; set; }
        public TaskData taskData { get; set; }        
    }
}
