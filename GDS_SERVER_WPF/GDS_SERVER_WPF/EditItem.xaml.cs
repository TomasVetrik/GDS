using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GDS_SERVER_WPF
{
    /// <summary>
    /// Interaction logic for EditItem.xaml
    /// </summary>
    public partial class EditItem : Window
    {
        public List<string> Names = new List<string>();
        public bool cancel = true;
        public bool clickOnEnter = false;
        public bool skipControll = false;

        public EditItem()
        {
            InitializeComponent();
        }

        private void SetErrorMessage(Label label, string message)
        {
            label.Foreground = Brushes.Red;
            labelError.Content = message;
        }

        private void SetDefault()
        {
            labelError.Content = "";
            labelNewText.Foreground = labelOldText.Foreground =  Brushes.Black;
        }

        private void ClickOK()
        {
            SetDefault();            
            foreach (string name in Names)
            {
                if (name == textBoxNewText.Text)
                {
                    SetErrorMessage(labelOldText, "'" + name + "' exists");                    
                    return;
                }
            }
            if (!skipControll)
            {
                if (textBoxNewText.Text.IndexOfAny(new char[] { '\\', '/', ':', '*', '?', '"', '<', '>', '|' }) != -1)
                {
                    SetErrorMessage(labelNewText, "Cannot contains \\ / : * ? \" < > |");
                    return;
                }
            }
            cancel = false;
            this.Close();
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            ClickOK();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBoxNewText.Focus();
            textBoxNewText.SelectAll();
            if (labelOldText.Content.ToString() == "")
                labelOldNameStatic.Visibility = Visibility.Hidden;
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    {
                        if(!clickOnEnter)
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
    }
}
