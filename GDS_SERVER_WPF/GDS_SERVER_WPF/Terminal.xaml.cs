using GDS_SERVER_WPF.DataCLasses;
using Microsoft.Win32;
using NetworkCommsDotNet.Tools;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GDS_SERVER_WPF
{
    /// <summary>
    /// Interaction logic for Terminal.xaml
    /// </summary>
    public partial class Terminal : Window
    {
        public Dictionary<ShortGuid, ComputerWithConnection> ClientsDictionary = new Dictionary<ShortGuid, ComputerWithConnection>();

        LastUsedTerminal config = new LastUsedTerminal();
        string PathOfLastUsed = @".\LastUsedInTerminal.txt";
        public Terminal()
        {
            InitializeComponent();
        }

        private void Button_Done_Click(object sender, RoutedEventArgs e)
        {
            FileHandler.Save(config, PathOfLastUsed);
            this.Close();
        }

        private void Button_Run_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure?", "Run Commands Confirmation", System.Windows.MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes)
                {

                    if (listBoxComputers.Items.Count != 0)
                    {
                        config = new LastUsedTerminal();
                        foreach (string command in txtBlockTerminal.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
                        {
                            config.commands.Add(command);
                        }
                        config.userName = txtBoxUserName.Text;
                        config.userPassword = txtBoxPassword.Text;
                        List<ComputerDetailsData> computers = new List<ComputerDetailsData>();
                        foreach (ComputerDetailsData computer in listBoxComputers.Items)
                        {
                            string IP = computer.IPAddress;
                            SshClient sshclient = new SshClient(IP, config.userName, config.userPassword);
                            try
                            {
                                sshclient.ConnectionInfo.Timeout = new TimeSpan(0, 0, 1);
                                sshclient.Connect();
                                foreach (string command in config.commands)
                                {
                                    sshclient.CreateCommand(command).Execute();
                                }
                                sshclient.Disconnect();
                            }
                            catch (Exception ex)
                            {
                                if (!(ex.ToString().Contains("An established connection was aborted by the server")))
                                {
                                    MessageBox.Show(computer.Name + ": " + ex.ToString());
                                }
                                sshclient.Dispose();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Browse_Click(object sender, RoutedEventArgs e)
        {
            var browseComputersDialog = new BrowseComputers
            {
                ClientsDictionary = ClientsDictionary,
                listBoxOut = listBoxComputers
            };
            browseComputersDialog.ShowDialog();
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog1.Title = "Save a Commands";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                config = new LastUsedTerminal();
                foreach (string command in txtBlockTerminal.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
                {
                    config.commands.Add(command);
                }
                config.userName = txtBoxUserName.Text;
                config.userPassword = txtBoxPassword.Text;
                FileHandler.Save(config, saveFileDialog1.FileName);
            }
        }

        private void Button_Load_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                config = FileHandler.Load<LastUsedTerminal>(openFileDialog.FileName);
                txtBlockTerminal.Text = string.Join(Environment.NewLine, config.commands.ToArray());
            }
        }

        private void ListBoxComputers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listBoxComputers.SelectedItems.Count != 0)
            {
                listBoxComputers.Items.Remove(listBoxComputers.SelectedItems[0]);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(PathOfLastUsed))
            {
                config = FileHandler.Load<LastUsedTerminal>(PathOfLastUsed);
                txtBlockTerminal.Text = string.Join(Environment.NewLine, config.commands.ToArray());
                txtBoxUserName.Text = config.userName;
                txtBoxPassword.Text = config.userPassword;
            }
            else
            {
                FileHandler.Save(config, PathOfLastUsed);
            }
        }
    }
}
