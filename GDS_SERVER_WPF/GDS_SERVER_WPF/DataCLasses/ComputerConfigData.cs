using ProtoBuf;
using System;
using System.Collections.Generic;

namespace GDS_SERVER_WPF.DataCLasses
{
    [ProtoContract]
    public class ComputerConfigData
    {
        [ProtoMember(1)]
        public string Name { get; set; }
        [ProtoMember(2)]
        public string Workgroup { get; set; }
        [ProtoMember(3)]
        public List<string> PostInstalls { get; set; }

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

        public void LoadDataFromList(List<string> list)
        {
            foreach (string line in list)
            {
                if (line != "")
                {
                    if (line.Contains("PCName||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        Name = splitter[1];
                    }
                    if (line.Contains("Workgroup||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        Workgroup = splitter[1];
                    }
                    if (line.Contains("Post Install||"))
                    {
                        string[] splitter = line.Split(new string[] { "||" }, StringSplitOptions.None);
                        if(splitter[1].Contains(","))
                        {
                            foreach(string postInstall in splitter[1].Split(','))
                            {
                                if(postInstall != "")
                                {
                                    PostInstalls.Add(postInstall);
                                }
                            }
                        }                        
                    }
                }
            }
        }
    }
}
