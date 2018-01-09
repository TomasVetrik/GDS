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
    /// Interaction logic for ChangeNote.xaml
    /// </summary>
    public partial class ChangeNote : Window
    {
        public string path = "";
        public ChangeNote()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(path, txtBoxPostInstalls.Text);
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    {
                        MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Do you want save note?", "Confirmation", System.Windows.MessageBoxButton.YesNo);
                        if (messageBoxResult == MessageBoxResult.Yes)
                        {
                            File.WriteAllText(path, txtBoxPostInstalls.Text);
                        }
                        this.Close();
                        break;
                    }
            }
        }
    }
}
