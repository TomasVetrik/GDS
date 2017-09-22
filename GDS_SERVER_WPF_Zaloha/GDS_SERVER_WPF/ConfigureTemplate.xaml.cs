using System;
using System.Collections.Generic;
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
    /// Interaction logic for ConfigureTemplate.xaml
    /// </summary>
    public partial class ConfigureTemplate : Window
    {
        public bool cancel = true;
        bool clickOnEnter = false;

        private void SetDefault()
        {
            labelError.Content = "";
            labelWorkgroup.Foreground = labelOldName.Foreground = Brushes.Black;
        }

        private void SetErrorMessage(Label label, string message)
        {
            label.Foreground = Brushes.Red;
            labelError.Content = message;
        }

        public ConfigureTemplate()
        {
            InitializeComponent();
        }

        private void ClickOK()
        {
            SetDefault();
            if(textBoxNewName.Text == "")
            {
                SetErrorMessage(labelWorkgroup, "'Workgroup' cannot be empty string");
                return;
            }
            if (textBoxNewName.Text.IndexOfAny(new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' }) != -1)
            {
                SetErrorMessage(labelWorkgroup, "'Workgroup' cannot contains \\ / : * ? \" < > |");
                return;
            }
            cancel = false;
            this.Close();
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            ClickOK();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    {
                        if (!clickOnEnter)
                        {
                            clickOnEnter = true;
                            ClickOK();
                            clickOnEnter = false;
                        }
                        break;
                    }
                case Key.Escape:
                    {
                        this.Close();
                        break;
                    }
            }
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
