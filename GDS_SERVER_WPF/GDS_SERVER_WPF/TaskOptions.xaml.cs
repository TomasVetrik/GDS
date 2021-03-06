﻿using GDS_SERVER_WPF.DataCLasses;
using GDS_SERVER_WPF.Handlers;
using Microsoft.Win32;
using NetworkCommsDotNet.Tools;
using Ookii.Dialogs.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace GDS_SERVER_WPF
{
    /// <summary>
    /// Interaction logic for TaskOptions.xaml
    /// </summary>
    public partial class TaskOptions : Window
    {        
        public string path;
        public string nodePath;
        public Dictionary<ShortGuid, ComputerWithConnection> ClientsDictionary = new Dictionary<ShortGuid, ComputerWithConnection>();
        public List<string> Names = new List<string>();
        public bool executed = false;
        public List<ExecutedTaskHandler> ExecutedTasksHandlers;        

        public TaskData taskData;

        public TaskOptions()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            taskData = new TaskData();
            if (File.Exists(path))
            {
                taskData = FileHandler.Load<TaskData>(path);
                LoadData();
            }
            else
            {
                LoadNewData();
            }
            labelNumberOfMachines.Content = "Number Of PCs: " + listBoxTargetComputers.Items.Count;
            tabItemGeneral.IsSelected = true;
        }       

        private void LoadNewData()
        {
            taskData.Name = textBoxTaskName.Text;
            taskData.LastExecuted = "NONE";
        }        

        private void LoadData()
        {
            textBoxTaskName.Text = taskData.Name;
            labelMachineGroupContent.Content = taskData.MachineGroup;           
            LoadDataToListBox(taskData.TargetComputers, listBoxTargetComputers);            
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
            checkBoxSendWarningMails.IsChecked = taskData.SendWarningMails;
            checkBoxInfinityWaitingTime.IsChecked = taskData.InfinityWaitingTime = true;
            var commandsOS = string.Join("\n", taskData.CommandsInOS);
            richTextBoxCommandsInOS.AppendText(commandsOS);
            var commandsWINPE = string.Join("\n", taskData.CommandsInWINPE);
            richTextBoxCommandsInWINPE.AppendText(commandsWINPE);
            LoadDataToListBox(taskData.CopyFilesInOS, listBoxCopyFilesInOS);            
            LoadDataToListBox(taskData.CopyFilesInWINPE, listBoxCopyFilesInWINPE);            

            slider.Value = taskData.WaitingTime;            
        }

        string StringFromRichTextBox(RichTextBox rtb)
        {
            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
        
            return textRange.Text;
        }

        private void SetDefaultColors()
        {
            labelTaskName.Foreground = Brushes.Black;
            labelNumberOfMachines.Foreground = Brushes.Black;
            labelMachineGroup.Foreground = Brushes.Black;
            labelError.Content = "";
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private bool Save()
        {
            if (SaveControl())
            {
                try {
                    taskData.MachineGroup = labelMachineGroupContent.Content.ToString();
                    LoadDataToList(listBoxTargetComputers, taskData.TargetComputers);                    
                    taskData.BaseImageSourcePath = textBoxBaseImage.Text;
                    taskData.DestinationDirectoryInOS = textBoxDestinationFolderInOS.Text;
                    taskData.DestinationDirectoryInWINPE = textBoxDestinationFolderInWINPE.Text;
                    taskData.DriveEImageSourcePath = textBoxDriveEImage.Text;
                    taskData.WakeOnLan = checkBoxWakeOnLan.IsChecked.Value;
                    taskData.Configuration = checkBoxConfiguration.IsChecked.Value;
                    taskData.Cloning = checkBoxCloning.IsChecked.Value;
                    taskData.ForceInstall = checkBoxForceInstall.IsChecked.Value;
                    taskData.ShutDown = checkBoxShutdown.IsChecked.Value;
                    taskData.SoftwareAndFileAction = checkBoxSOFA.IsChecked.Value;
                    taskData.SoftwareAndFileAction_WINPE = checkBoxSOFAWinpe.IsChecked.Value;
                    taskData.WithoutVHD = checkBoxWithoutVHD.IsChecked.Value;
                    taskData.SendWarningMails = checkBoxSendWarningMails.IsChecked.Value;
                    taskData.InfinityWaitingTime = checkBoxInfinityWaitingTime.IsChecked.Value;
                    string[] commandsOS = StringFromRichTextBox(richTextBoxCommandsInOS).Split(new[] { Environment.NewLine }
                                              , StringSplitOptions.RemoveEmptyEntries);
                    taskData.CommandsInOS = new List<string>();
                    taskData.CommandsInOS = new List<string>(commandsOS);
                    LoadDataToList(listBoxCopyFilesInOS, taskData.CopyFilesInOS);
                    LoadDataToList(listBoxCopyFilesInWINPE, taskData.CopyFilesInWINPE);
                    string[] commandsWINPE = StringFromRichTextBox(richTextBoxCommandsInWINPE).Split(new[] { Environment.NewLine }
                                              , StringSplitOptions.RemoveEmptyEntries);
                    taskData.CommandsInWINPE = new List<string>(commandsWINPE);
                    taskData.WaitingTime = (int)slider.Value;
                    if (taskData.Name != textBoxTaskName.Text && taskData.Name != "")
                    {
                        var createRenameDialog = new CreateRenameCancel();
                        createRenameDialog.lblNewName.Content = textBoxTaskName.Text;
                        createRenameDialog.lblOldName.Content = taskData.Name;
                        createRenameDialog.ShowDialog();
                        if (createRenameDialog.renameOld)
                        {
                            if (File.Exists(nodePath + "\\" + taskData.Name + ".my"))
                            {
                                File.Delete(nodePath + "\\" + taskData.Name + ".my");
                            }
                            taskData.Name = textBoxTaskName.Text;
                            FileHandler.Save<TaskData>(taskData, nodePath + "\\" + taskData.Name + ".my");
                            return true;
                        }
                        if (createRenameDialog.createNew)
                        {
                            taskData.Name = textBoxTaskName.Text;
                            FileHandler.Save<TaskData>(taskData, nodePath + "\\" + taskData.Name + ".my");
                            return true;
                        }
                    }
                    else
                    {
                        taskData.Name = textBoxTaskName.Text;
                        FileHandler.Save<TaskData>(taskData, nodePath + "\\" + taskData.Name + ".my");
                        return true;
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            } 
            return false;
        }        

        private bool SaveControl()
        {
            try
            {
                SetDefaultColors();
                if (textBoxTaskName.Text == "")
                {
                    SetErrorMessage(labelTaskName, "'Task Name' cannot be an empty string\n");
                    return false;
                }
                if (textBoxTaskName.Text.IndexOfAny(new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' }) != -1)
                {
                    SetErrorMessage(labelTaskName, "'Task Name' cannot contains \\ / : * ? \" < > | \n");
                    return false;
                }
                if (Names.Contains(textBoxTaskName.Text))
                {
                    SetErrorMessage(labelTaskName, "'Task Name': " + textBoxTaskName.Text + " exists\n");
                    return false;
                }
                List<string> LockComputers = Directory.GetFiles(@".\Machine Groups\Lock\", "*.my", SearchOption.AllDirectories).ToList();
                foreach (string LockComputer in LockComputers)
                {
                    foreach (ComputerDetailsData computer in listBoxTargetComputers.Items)
                    {
                        if (LockComputer.Contains(computer.Name + ".my"))
                        {
                            SetErrorMessage(labelMachineGroup, "Computer: " + computer.Name + " is locked\n");
                            return false;
                        }
                    }
                }
                if (checkBoxCloning.IsChecked.Value && textBoxBaseImage.Text == "" && textBoxDriveEImage.Text == "")
                {
                    tabItemGeneral.IsSelected = true;
                    labelError.Content += "Cloning is not properly set up\n";
                    return false;
                }
                if (checkBoxCloning.IsChecked.Value)
                {
                    if (textBoxBaseImage.Text != "")
                    {
                        if (!File.Exists(textBoxBaseImage.Text + ".my"))
                        {
                            labelError.Content += "Base image settings does not exist\n";
                            return false;
                        }
                        else
                        {
                            taskData.BaseImageData = FileHandler.Load<ImageData>(textBoxBaseImage.Text + ".my");
                            if (!File.Exists(taskData.BaseImageData.SourcePath))
                            {
                                labelError.Content += "Source path of Base wim file does not exist\n";
                                return false;
                            }
                        }                        
                    }
                    if (textBoxDriveEImage.Text != "")
                    {
                        if (!File.Exists(textBoxDriveEImage.Text + ".my"))
                        {
                            labelError.Content += "DriveE image settings does not exist\n";
                            return false;
                        }
                        else
                        {
                            taskData.DriveEImageData = FileHandler.Load<ImageData>(textBoxDriveEImage.Text + ".my");
                            if(!File.Exists(taskData.DriveEImageData.SourcePath))
                            {
                                labelError.Content += "Source path of DriveE wim file does not exist\n";
                                return false;
                            }
                        }                        
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }
        }

        private void SetErrorMessage(Label label, string message)
        {
            label.Foreground = Brushes.Red;
            labelError.Content += message;
            tabItemGeneral.IsSelected = true;
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private bool IsMacAddressIn(List<string> array1, List<string> array2)
        {
            foreach (string text1 in array1)
            {
                foreach (string text2 in array2)
                {
                    if (text1 == text2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckMacsInREC(List<string> Macs, List<string> Recs)
        {
            if (Recs != null)
            {
                if (Macs.Count != 0 && Recs.Count != 0)
                    return IsMacAddressIn(Macs, Recs);
            }
            return false;
        }

        private void ButtonExecute_Click(object sender, RoutedEventArgs e)
        {
            SetDefaultColors();
            if (listBoxTargetComputers.Items.Count == 0)
            {
                SetErrorMessage(labelNumberOfMachines, "The property 'Target Machines' cannot be an empty string\n");
                return;
            }
            else
            {
                if (ExecutedTasksHandlers != null)
                {
                    foreach (ComputerDetailsData computer in listBoxTargetComputers.Items)
                    {
                        for (int i = ExecutedTasksHandlers.Count - 1; i >= 0; i--)
                        {
                            var item = ExecutedTasksHandlers[i];
                            foreach (ComputerInTaskHandler computerInTask in item.computers)
                            {
                                if (CheckMacsInREC(computer.macAddresses, computerInTask.computer.macAddresses) && !computerInTask.finish)
                                {
                                    SetErrorMessage(labelMachineGroup, computerInTask.computer + " is in task: " + item.executedTaskData.Name + "\n");
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Will you execute Task?", "Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                taskData.LastExecuted = DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss");
                if (Save())
                {
                    executed = true;
                    this.Close();
                }
            }
        }

        private void ButtonBrowseComputers_Click(object sender, RoutedEventArgs e)
        {
            var browseComputersDialog = new BrowseComputers
            {
                ClientsDictionary = ClientsDictionary,
                listBoxOut = listBoxTargetComputers
            };
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

        private void ButtonClear_Click(object sender, RoutedEventArgs e)
        {
            listBoxTargetComputers.Items.Clear();
            labelMachineGroupContent.Content = "NONE";
            labelNumberOfMachines.Content = "Number Of PCs: 0";
        }

        private void ShowBrowseImagesDialog(string path, TextBox textBox, bool baseImage)
        {
            var browseImagesWindows = new BrowseImages
            {
                path = path,
                baseImage = baseImage
            };
            browseImagesWindows.ShowDialog();
            if (browseImagesWindows.pathOutput != "")
            {
                textBox.Text = browseImagesWindows.pathOutput;
                if (path.Contains(@".\Base\"))
                    taskData.BaseImageData = FileHandler.Load<ImageData>(textBox.Text + ".my");
                else
                    taskData.DriveEImageData = FileHandler.Load<ImageData>(textBox.Text + ".my");
            }
        }

        private void ButtonBaseBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            ShowBrowseImagesDialog(@".\Base\", textBoxBaseImage, true);
        }

        private void ButtonDriveEBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            ShowBrowseImagesDialog(@".\DriveE", textBoxDriveEImage, false);
        }

        private void ButtonBaseClear_Click(object sender, RoutedEventArgs e)
        {
            textBoxBaseImage.Text = "";
        }

        private void ButtonDriveEClear_Click(object sender, RoutedEventArgs e)
        {
            textBoxDriveEImage.Text = "";
        }

        private string LoadCopyFiles(string sourceDirectory, string destinationDirectory, TextBox textBox, ListBox listBox, List<string> copyFiles)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true
            };
            if (sourceDirectory != "" && Directory.Exists(sourceDirectory))
            {
                openFileDialog.InitialDirectory = sourceDirectory;
            }
            openFileDialog.Filter = "All files (*.*)|*.*";
            openFileDialog.ShowDialog();
            string[] result = openFileDialog.FileNames;
            if (result.Length != 0)
            {
                copyFiles = new List<string>(result);
                sourceDirectory = Path.GetDirectoryName(openFileDialog.FileName);
                LoadDataToListBox(copyFiles, listBox);
                if (textBox.Text == "")
                {
                    destinationDirectory = textBox.Text = sourceDirectory;
                }
            }
            return sourceDirectory;
        }

        private void ButtonFilesInWINPE_Click(object sender, RoutedEventArgs e)
        {            
            taskData.SourceDirectoryInWINPE = LoadCopyFiles(taskData.SourceDirectoryInWINPE, taskData.DestinationDirectoryInWINPE, textBoxDestinationFolderInWINPE, listBoxCopyFilesInWINPE, taskData.CopyFilesInWINPE);
        }

        private void LoadDataToListBox(List<string> items, ListBox listBox)
        {            
            listBox.Items.Clear();
            if (items != null)
            {
                foreach (string item in items)
                {
                    listBox.Items.Add(item);
                }
            }
        }

        private void LoadDataToList(ListBox listBox, List<string> items)
        {            
            items.Clear();
            foreach (string item in listBox.Items)
            {
                items.Add(item);
            }
        }

        private void LoadDataToListBox(List<ComputerDetailsData> items, ListBox listBox)
        {
            listBox.Items.Clear();
            if (items != null)
            {
                foreach (ComputerDetailsData item in items)
                {
                    listBox.Items.Add(item);
                }
            }
        }
        private void LoadDataToList(ListBox listBox, List<ComputerDetailsData> items)
        {
            items.Clear();
            foreach (ComputerDetailsData item in listBox.Items)
            {
                items.Add(item);
            }
        }

        private void ListBoxCopyFilesInWINPE_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (listBoxCopyFilesInWINPE.SelectedIndex != -1)
            {
                listBoxCopyFilesInWINPE.Items.Remove(listBoxCopyFilesInWINPE.SelectedItem);
            }            
        }

        private void ButtonFilesInOS_Click(object sender, RoutedEventArgs e)
        {
            taskData.SourceDirectoryInOS = LoadCopyFiles(taskData.SourceDirectoryInOS, taskData.DestinationDirectoryInOS, textBoxDestinationFolderInOS, listBoxCopyFilesInOS, taskData.CopyFilesInOS);
        }

        private void ListBoxCopyFilesInOS_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (listBoxCopyFilesInOS.SelectedIndex != -1)
            {
                listBoxCopyFilesInOS.Items.Remove(listBoxCopyFilesInOS.SelectedItem);
            }
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    {
                        executed = false;
                        this.Close();
                        break;
                    }
            }
        }

        private string LoadCopyFolder(string sourceDirectory, string destinationDirectory, TextBox textBox, ListBox listBox, List<string> copyFiles)
        {
            VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog
            {
                ShowNewFolderButton = true
            };
            dlg.ShowDialog();
            string path = dlg.SelectedPath;
            if (path != "")
            {
                string[] result = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
                copyFiles = new List<string>(result);
                sourceDirectory = path;
                LoadDataToListBox(copyFiles, listBox);
                if (textBox.Text == "")
                {
                    destinationDirectory = textBox.Text = sourceDirectory;
                }
            }
            return sourceDirectory;
        }

        private void ButtonFolderInOS_Click(object sender, RoutedEventArgs e)
        {
            taskData.SourceDirectoryInOS = LoadCopyFolder(taskData.SourceDirectoryInOS, taskData.DestinationDirectoryInOS, textBoxDestinationFolderInOS, listBoxCopyFilesInOS, taskData.CopyFilesInOS);
        }

        private void ButtonClearInOS_Click(object sender, RoutedEventArgs e)
        {
            listBoxCopyFilesInOS.Items.Clear();
            textBoxDestinationFolderInOS.Clear();
        }

        private void ButtonFolderInWINPE_Click(object sender, RoutedEventArgs e)
        {
            taskData.SourceDirectoryInWINPE = LoadCopyFolder(taskData.SourceDirectoryInWINPE, taskData.DestinationDirectoryInWINPE, textBoxDestinationFolderInWINPE, listBoxCopyFilesInWINPE, taskData.CopyFilesInWINPE);
        }

        private void ButtonClearInWINPE_Click(object sender, RoutedEventArgs e)
        {
            listBoxCopyFilesInWINPE.Items.Clear();
            textBoxDestinationFolderInWINPE.Clear();
        }
    }   
}
