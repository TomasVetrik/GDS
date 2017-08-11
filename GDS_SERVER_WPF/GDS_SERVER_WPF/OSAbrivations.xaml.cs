using GDS_SERVER_WPF.DataCLasses;
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
    /// Interaction logic for OSAbrivations.xaml
    /// </summary>
    public partial class OSAbrivations : Window
    {
        public OSAbrivations()
        {
            InitializeComponent();
        }

        public ListBox listBox;
        
        string path = @".\OsAbrivations.my";
        OSAbrivationsData osAbrivationsData = new OSAbrivationsData();        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(!File.Exists(path))
            {
                FileHandler.Save<OSAbrivationsData>(osAbrivationsData, path);
            }
            foreach (string item in listBox.Items)
            {
                listBoxOSAbbrivationsOutPut.Items.Add(item);
            }
            osAbrivationsData = FileHandler.Load<OSAbrivationsData>(path);
            listBoxOsAbbrivations.ItemsSource = osAbrivationsData.osAbrivations;
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void listBoxOSAbbrivations_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listBoxOSAbbrivationsOutPut.SelectedItems.Count != 0)
            {
                listBoxOSAbbrivationsOutPut.Items.Remove(listBoxOSAbbrivationsOutPut.SelectedItems[0]);
            }
        }

        private void MenuItemRemove_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxOsAbbrivations.SelectedIndex == -1)
            {
                return;
            }            
            osAbrivationsData.osAbrivations = (List<string>)listBoxOsAbbrivations.ItemsSource;            
            FileHandler.Save<OSAbrivationsData>(osAbrivationsData, path);
        }

        private void buttonNew_Click(object sender, RoutedEventArgs e)
        {
            if(textBoxNewAbrivation.Text != "")
            {
                if (!listBoxOsAbbrivations.Items.Contains(textBoxNewAbrivation.Text))
                {
                    osAbrivationsData.osAbrivations.Add(textBoxNewAbrivation.Text);
                    FileHandler.Save<OSAbrivationsData>(osAbrivationsData, path);
                    listBoxOsAbbrivations.ItemsSource = osAbrivationsData.osAbrivations;
                }
            }
        }

        

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            labelError.Visibility = Visibility.Hidden;
            if (listBoxOSAbbrivationsOutPut.Items.Count == 0)
            {
                labelError.Visibility = Visibility.Visible;
                return;
            }
            listBox.Items.Clear();
            foreach (string item in listBoxOSAbbrivationsOutPut.Items)
            {
                listBox.Items.Add(item);
            }   
            this.Close();
        }

        private void listBoxOsAbbrivations_MouseDoubleClick_1(object sender, MouseButtonEventArgs e)
        {
            if(listBoxOsAbbrivations.SelectedItems.Count != 0)
            {
                if (!listBoxOSAbbrivationsOutPut.Items.Contains(listBoxOsAbbrivations.SelectedItems[0]))
                    listBoxOSAbbrivationsOutPut.Items.Add(listBoxOsAbbrivations.SelectedItems[0]);
            }
        }
    }
}
