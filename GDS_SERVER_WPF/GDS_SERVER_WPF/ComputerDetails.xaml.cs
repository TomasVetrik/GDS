using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
        string computerConfigFilePath;
        ComputerDetailsData computerData;
        ComputerConfigData computerConfigData;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            computerFilePath = computerPath + ".my";
            computerConfigFilePath = computerPath + ".cfg";
            if (File.Exists(computerFilePath))
            {
                computerData = FileHandler.Load<ComputerDetailsData>(computerFilePath);
                listBox.ItemsSource = computerData.GetItems();
            }
            else
                computerData = new ComputerDetailsData();
            if (File.Exists(computerConfigFilePath))
            {
                computerConfigData = FileHandler.Load<ComputerConfigData>(computerConfigFilePath);
                textBoxComputerName.Text = computerConfigData.Name;
                textBoxWorkGroup.Text = computerConfigData.Workgroup;
            }
            else
                computerConfigData = new ComputerConfigData();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            computerConfigData.Name = textBoxComputerName.Text;
            computerConfigData.Workgroup = textBoxWorkGroup.Text;                            
            FileHandler.Save<ComputerDetailsData>(computerData, computerFilePath);
            FileHandler.Save<ComputerConfigData>(computerConfigData, computerConfigFilePath);
            this.Close();
        }

        private void EventSetter_OnHandler(object sender, RoutedEventArgs e)
        {
            ((TextBox)sender).SelectAll();
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    {
                        this.Close();
                        break;
                    }
            }
        }
    }
}
