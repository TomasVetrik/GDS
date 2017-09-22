using System.Collections.Generic;

namespace GDS_SERVER_WPF.DataCLasses
{
    public class PhonesForTask
    {
        public object phoneSynFlag = new object();
        public object phoneSynFlagWinpe = new object();
        public object phoneForTask = new object();
        public List<object> phones = new List<object>();

        public PhonesForTask()
        {
            phones.Add(phoneSynFlag);
            phones.Add(phoneSynFlagWinpe);
        }
    }
}
