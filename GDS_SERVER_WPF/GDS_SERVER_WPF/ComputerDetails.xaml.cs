using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GDS_SERVER_WPF
{    
    public partial class ComputerDetails : Window
    {
        public ComputerDetails()
        {
            InitializeComponent();
        }

        public string computerPath;
        string computerFilePath;
        string configurationFilePath;
        ComputerConfigData configData;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            computerFilePath = computerPath + ".my";
            configurationFilePath = computerPath + ".cfg";
            if (File.Exists(computerFilePath))
            {
                var computerData = FileHandler.Load<ComputerDetailsData>(computerFilePath);
                listBox.ItemsSource = computerData.GetItems();
            }
            if (File.Exists(configurationFilePath))
            {
                configData = FileHandler.Load<ComputerConfigData>(configurationFilePath);
                textBoxComputerName.Text = configData.Name;
                textBoxWorkGroup.Text = configData.WorkGroup;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            configData.Name = textBoxComputerName.Text;
            configData.WorkGroup = textBoxWorkGroup.Text;                            
            FileHandler.Save<ComputerConfigData>(configData, configurationFilePath);
            this.Close();
        }

        private void EventSetter_OnHandler(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }
    }
}
