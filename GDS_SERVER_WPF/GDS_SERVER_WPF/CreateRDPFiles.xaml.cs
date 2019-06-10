using GDS_SERVER_WPF.DataCLasses;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace GDS_SERVER_WPF
{
    /// <summary>
    /// Interaction logic for CreateRDPFiles.xaml
    /// </summary>
    public partial class CreateRDPFiles : Window
    {
        public CreateRDPFiles()
        {
            InitializeComponent();
        }

        public List<string> TempFile;
        public string nodePath;
        public string gateWayHostName;
        public ListView list;

        string RDPLastUsedSettingsPath = ".\\RDPLastUsedSettings.my";
        RDPLastUsedSettings _RDPLastUsedSettings;

        private void BtnCreateRDPs_Click(object sender, RoutedEventArgs e)
        {
            _RDPLastUsedSettings.GateWay = TextBoxGateWay.Text;
            _RDPLastUsedSettings.RDSLogin = TextBoxRdsLogin.Text;
            _RDPLastUsedSettings.RDSPassword = TextBoxRdsPassword.Text;
            _RDPLastUsedSettings.LocalLogin = TextBoxLocalLogin.Text;
            _RDPLastUsedSettings.LocalPassword = TextBoxLocalPassword.Text;
            FileHandler.Save(_RDPLastUsedSettings, RDPLastUsedSettingsPath);
            for(int i = list.Items.Count - 1; i >= 0; i--)
            { 
                ComputerDetailsData computer = (ComputerDetailsData)list.Items[i];
                CreateRDP(computer.IPAddress, nodePath + "\\" + computer.Name + ".rdp", TempFile);
            }
            File.WriteAllText(nodePath + "\\_Logins.txt", "1.st authentication(*rds.class.skola.cz)\r\nLogin: " + _RDPLastUsedSettings.RDSLogin + "\r\nPassword: " + _RDPLastUsedSettings.RDSPassword + "\r\n\r\n2.nd authentication\r\nLogin: " + _RDPLastUsedSettings.LocalLogin + "\r\nPassword: " + _RDPLastUsedSettings.LocalPassword);
            

            System.Diagnostics.Process.Start(nodePath);
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            nodePath = ".\\RDPFiles\\" + new DirectoryInfo(nodePath).Name; ;
            if (!Directory.Exists(nodePath))
            {
                Directory.CreateDirectory(nodePath);
            }
            if(File.Exists(RDPLastUsedSettingsPath))
            {
                _RDPLastUsedSettings = FileHandler.Load<RDPLastUsedSettings>(RDPLastUsedSettingsPath);
                TextBoxGateWay.Text = _RDPLastUsedSettings.GateWay;
                TextBoxRdsLogin.Text = _RDPLastUsedSettings.RDSLogin;
                TextBoxRdsPassword.Text = _RDPLastUsedSettings.RDSPassword;
                TextBoxLocalLogin.Text = _RDPLastUsedSettings.LocalLogin;
                TextBoxLocalPassword.Text = _RDPLastUsedSettings.LocalPassword;
            }
            else
            {
                _RDPLastUsedSettings = new RDPLastUsedSettings
                {
                    GateWay = TextBoxGateWay.Text = gateWayHostName
                };
            }
        }

        private void CreateRDP(string IPAddress, string pathToSave , List<string> TempFile)
        {
            TempFile.Add("full address:s:" + IPAddress);
            TempFile.Add(@"username:s:" + TextBoxLocalLogin.Text);
            TempFile.Add("gatewayhostname:s:" + TextBoxGateWay.Text );
            File.WriteAllLines(pathToSave.Replace(".my", ".rdp"), TempFile.ToArray());
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
