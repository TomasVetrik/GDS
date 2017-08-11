﻿using GDS_SERVER_WPF.DataCLasses;
using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GDS_SERVER_WPF
{
    /// <summary>
    /// Interaction logic for ImageOptions.xaml
    /// </summary>
    public partial class ImageOptions : Window
    {
        public string path = "";
        public bool baseImage = false;
        public string nodePath = "";
        public List<string> Names = new List<string>();        

        ImageData imageData = new ImageData();        

        public ImageOptions()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(path))
            {
                imageData = FileHandler.Load<ImageData>(path);
                textBoxImageName.Text = imageData.Name;
                textBoxSourcePath.Text = imageData.SourcePath;
                sliderPartitionDSize.Value = imageData.PartitionSize;
                checkBoxVHDResize.IsChecked = imageData.VHDResize;
                sliderVHDResizeSize.Value = imageData.VHDResizeSize;
                foreach (string VHDName in imageData.VHDNames)
                {                    
                    listBoxVHDNames.Items.Add(VHDName);
                }
                textBoxBootLabel.Text = imageData.BoolLabel;
                foreach (string OSAbrivation in imageData.OSAbrivations)
                {
                    listBoxOSAbbrivations.Items.Add(OSAbrivation);
                }
            }
            if(!baseImage)
            {
                listBoxOSAbbrivations.IsEnabled = false;
                listBoxVHDNames.IsEnabled = false;
                sliderPartitionDSize.IsEnabled = false;
                sliderVHDResizeSize.IsEnabled = false;
                checkBoxVHDResize.IsEnabled = false;
            }
        }

        private void listBoxOSAbbrivations_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(listBoxOSAbbrivations.SelectedItems.Count != 0)
            {
                listBoxOSAbbrivations.Items.Remove(listBoxOSAbbrivations.SelectedItems[0]);
            }
        }

        private void listBoxVHDNames_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listBoxVHDNames.SelectedItems.Count != 0)
            {
                listBoxVHDNames.Items.Remove(listBoxVHDNames.SelectedItems[0]);
            }
        }

        private void buttonBrowse_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();    
            openFileDialog.Filter = "WIM files (*.wim)|*.wim";
            if (File.Exists(textBoxSourcePath.Text))            
                openFileDialog.InitialDirectory = textBoxSourcePath.Text;

            if (openFileDialog.ShowDialog() == true)
            {
                textBoxSourcePath.Text = openFileDialog.FileName;
                if(baseImage)
                {
                    listBoxVHDNames.Items.Clear();
                    listBoxVHDNames.Items.Add(System.IO.Path.GetFileName(textBoxSourcePath.Text).Replace(".wim", ""));
                }
            }
        }

        private void buttonVHDNameAdd_Click(object sender, RoutedEventArgs e)
        {
            var addVHDNameDialog = new EditItem();
            foreach (string name in listBoxVHDNames.Items)
            {
                addVHDNameDialog.Names.Add(name);
            }
            addVHDNameDialog.ShowDialog();
            listBoxVHDNames.Items.Add(addVHDNameDialog.textBoxNewName.Text);
        }

        private void buttonOSAbrivationsAdd_Click(object sender, RoutedEventArgs e)
        {
            var osAbrivationsDialog = new OSAbrivations();
            osAbrivationsDialog.listBox = listBoxOSAbbrivations; 
            osAbrivationsDialog.ShowDialog();                                    
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void SetDefault()
        {
            labelError.Content = "";
            labelImageName.Foreground = labelBootLabel.Foreground = labelOSAbrivations.Foreground = labelParitionDSize.Foreground = labelVHDName.Foreground = checkBoxVHDResize.Foreground = Brushes.Black;
        }

        private void SetErrorMessage(Label label, string message)
        {
            label.Foreground = Brushes.Red;
            labelError.Content = message;
        }

        private void SetErrorMessage(CheckBox checkBox, string message)
        {
            checkBox.Foreground = Brushes.Red;
            labelError.Content = message;
        }

        private void SavaData()
        {
            SetDefault();
            if(textBoxImageName.Text == "" || textBoxImageName.Text.Contains(" "))
            {
                SetErrorMessage(labelImageName, "'Image Name' cannot be empty or contains spaces");
                return;
            }      
            if(Names.Contains(textBoxImageName.Text) && path != "")
            {
                SetErrorMessage(labelImageName, "'Image Name': " + textBoxImageName.Text + " exists");
                return;
            }      
            if(textBoxSourcePath.Text == "")
            {
                SetErrorMessage(labelSourcePath, "'Source Path' cannot be empty");
                return;
            }
            if (textBoxBootLabel.Text == "" || textBoxBootLabel.Text.IndexOfAny(new char[] { ',', '|', '#', '$', '.' }) != -1)
            {
                SetErrorMessage(labelBootLabel, "'Boot Label' cannot be empty or contains , | # $ .");
                return;
            }
            if (baseImage)
            {
                if (checkBoxVHDResize.IsChecked.Value && sliderPartitionDSize.Value < sliderVHDResizeSize.Value + 50)
                {
                    SetErrorMessage(checkBoxVHDResize, "Partition D size is too small, must be greater than " + (sliderVHDResizeSize.Value + 50));
                    return;
                }
                if(listBoxVHDNames.Items.Count == 0)
                {
                    SetErrorMessage(labelVHDName, "'VHD Name' cannot be empty");
                    return;
                }                          
                if(listBoxOSAbbrivations.Items.Count == 0)
                {
                    SetErrorMessage(labelOSAbrivations, "'OS Abrivations' cannot be empty");
                    return;
                }
                imageData.PartitionSize = (int)sliderPartitionDSize.Value;
                imageData.VHDResize = checkBoxVHDResize.IsChecked.Value;
                imageData.VHDResizeSize = (int)sliderVHDResizeSize.Value;
                imageData.VHDNames.Clear();
                foreach (string VHDName in listBoxVHDNames.Items)
                {
                    imageData.VHDNames.Add(VHDName);
                }
                imageData.OSAbrivations.Clear();
                foreach (string OSAbrivation in listBoxOSAbbrivations.Items)
                {
                    imageData.OSAbrivations.Add(OSAbrivation);
                }
            }
            imageData.Name = textBoxImageName.Text;
            imageData.SourcePath = textBoxSourcePath.Text;
            imageData.BoolLabel = textBoxBootLabel.Text;
            FileHandler.Save<ImageData>(imageData, nodePath + "\\" + imageData.Name + ".my");            
            this.Close();
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            SavaData();
        }       
    }
}
