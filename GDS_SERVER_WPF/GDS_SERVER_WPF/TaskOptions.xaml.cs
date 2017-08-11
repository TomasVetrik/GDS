using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace GDS_SERVER_WPF
{
    /// <summary>
    /// Interaction logic for TaskOptions.xaml
    /// </summary>
    public partial class TaskOptions : Window
    {
        TaskData taskData;
        public string path;
        public string nodePath;
        public List<ClientHandler> clients;

        public TaskOptions()
        {
            InitializeComponent();
        }

        private void SetDefaultColors()
        {
            labelTaskName.Foreground = Brushes.Black;
            labelNumberOfMachines.Foreground = Brushes.Black;
            labelToolTip.Content = "";
        }

        private void LoadNewData()
        {
            taskData.Name = textBoxTaskName.Text;
            taskData.LastExecuted = "NONE";
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void LoadData()
        {
            textBoxTaskName.Text = taskData.Name;
            labelMachineGroupContent.Content = taskData.MachineGroup;
            listBoxTargetComputers.ItemsSource = taskData.TargetComputers;
            textBoxBaseImage.Text = taskData.BaseImageSourcePath;
            textBoxDestinationFolderInOS.Text = taskData.DestinationDirectoryInOS;
            textBoxDestinationFolderInWINPE.Text = taskData.DestinationDirectoryInWINPE;
            textBoxDriveEImage.Text = taskData.DriveEImageSourcePath;                        
            checkBoxWakeOnLan.IsChecked = taskData.WakeOnLan;
            checkBoxConfiguration.IsChecked = taskData.Configuration;
            checkBoxCloning.IsChecked = taskData.Cloning;
            checkBoxForceInstall.IsChecked = taskData.ForceInstall;
            checkBoxShutdown.IsChecked = taskData.ShutDown;
            checkBoxSOFA.IsChecked = taskData.SoftwareAndFileAction;
            checkBoxSOFAWinpe.IsChecked = taskData.SoftwareAndFileAction_WINPE;
            checkBoxWithoutVHD.IsChecked = taskData.WithoutVHD;
            var commandsOS = string.Join("\n", taskData.CommnadsInOS);
            richTextBoxCommandsInOS.AppendText(commandsOS);
            var commandsWINPE = string.Join("\n", taskData.CommnadsInWINPE);
            richTextBoxCommandsInWINPE.AppendText(commandsWINPE);
            listBoxCopyFilesInOS.ItemsSource = taskData.CopyFilesInOS;
            listBoxCopyFilesInWINPE.ItemsSource = taskData.CopyFilesInWINPE;

            slider.Value = taskData.WaitingTime;            
        }

        string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
        
            return textRange.Text;
        }

        private void Save()
        {
            if (SaveControl())
            {
                taskData.Name = textBoxTaskName.Text;
                taskData.MachineGroup = labelMachineGroupContent.Content.ToString();
                taskData.TargetComputers = (List<string>)listBoxTargetComputers.ItemsSource;
                taskData.BaseImageSourcePath = textBoxBaseImage.Text;
                taskData.DestinationDirectoryInOS = textBoxDestinationFolderInOS.Text;
                taskData.DestinationDirectoryInWINPE = textBoxDestinationFolderInWINPE.Text;
                taskData.DriveEImageSourcePath = textBoxDriveEImage.Text;
                taskData.SourceDirectory = "";                
                taskData.WakeOnLan = checkBoxWakeOnLan.IsChecked.Value;
                taskData.Configuration = checkBoxConfiguration.IsChecked.Value;
                taskData.Cloning = checkBoxCloning.IsChecked.Value;
                taskData.ForceInstall = checkBoxForceInstall.IsChecked.Value;
                taskData.ShutDown = checkBoxShutdown.IsChecked.Value;
                taskData.SoftwareAndFileAction = checkBoxSOFA.IsChecked.Value;
                taskData.SoftwareAndFileAction_WINPE = checkBoxSOFAWinpe.IsChecked.Value;
                taskData.WithoutVHD = checkBoxWithoutVHD.IsChecked.Value;
                string[] commandsOS = StringFromRichTextBox(richTextBoxCommandsInOS).Split(new[] { Environment.NewLine }
                                          , StringSplitOptions.RemoveEmptyEntries);
                taskData.CommnadsInOS = new List<string>(commandsOS);
                taskData.CopyFilesInOS = (List<string>)listBoxCopyFilesInOS.ItemsSource;
                taskData.CopyFilesInWINPE = (List<string>)listBoxCopyFilesInWINPE.ItemsSource;
                string[] commandsWINPE = StringFromRichTextBox(richTextBoxCommandsInWINPE).Split(new[] { Environment.NewLine }
                                          , StringSplitOptions.RemoveEmptyEntries);
                taskData.CommnadsInOS = new List<string>(commandsWINPE);                
                taskData.WaitingTime = (int)slider.Value;

                FileHandler.Save<TaskData>(taskData, nodePath + "\\" + textBoxTaskName.Text + ".my");
            }
        }

        private bool SaveControl()
        {
            SetDefaultColors();
            if (textBoxTaskName.Text == "")
            {
                labelTaskName.Foreground = Brushes.Red;
                labelToolTip.Content += "The property 'Task Name' cannot be an empty string\n";
                return false;
            }
            return true;
        }

        private void buttonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonExecute_Click(object sender, RoutedEventArgs e)
        {
            SetDefaultColors();
            if (listBoxTargetComputers.Items.Count == 0)
            {
                labelNumberOfMachines.Foreground = Brushes.Red;
                labelToolTip.Content += "The property 'Target Machines' cannot be an empty string\n";
                return;
            }
            Save();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            taskData = new TaskData();
            if(File.Exists(path))
            {
                taskData = FileHandler.Load<TaskData>(path);
                LoadData();
            }
            else
            {
                LoadNewData();
            }
            labelNumberOfMachines.Content = "Number Of PCs: " + listBoxTargetComputers.Items.Count;
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            var browseComputersDialog = new BrowseComputers();
            browseComputersDialog.clients = clients;
            browseComputersDialog.listBoxOut = listBoxTargetComputers;
            browseComputersDialog.ShowDialog();
            if (listBoxTargetComputers.Items.Count != 0)
            {
                labelMachineGroupContent.Content = (browseComputersDialog.treeView.SelectedItem as TreeViewItem).Header;
            }
            else
            {
                labelMachineGroupContent.Content = "NONE";
            }
            labelNumberOfMachines.Content = "Number Of PCs: " + listBoxTargetComputers.Items.Count;
        }

        private void buttonClear_Click(object sender, RoutedEventArgs e)
        {
            listBoxTargetComputers.ItemsSource = taskData.TargetComputers = new List<string> { };
            labelMachineGroupContent.Content = "NONE";
        }

        private void buttonBaseBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            var browseImagesWindows = new BrowseImages();
            browseImagesWindows.path = @".\Base\";
            browseImagesWindows.baseImage = true;            
            browseImagesWindows.ShowDialog();

        }

        private void buttonBaseClear_Click(object sender, RoutedEventArgs e)
        {
            textBoxBaseImage.Text = "";
        }

        private void buttonDriveEClear_Click(object sender, RoutedEventArgs e)
        {
            textBoxDriveEImage.Text = "";
        }

        private void buttonDriveEBrowseImage_Click(object sender, RoutedEventArgs e)
        {

            var browseImagesWindows = new BrowseImages();
            browseImagesWindows.baseImage = false;
            browseImagesWindows.path = @".\DriveE";
            browseImagesWindows.ShowDialog();
        }
    }
}
