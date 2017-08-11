using System.Collections.Generic;
using System.Windows;

namespace GDS_SERVER_WPF
{
    /// <summary>
    /// Interaction logic for EditItem.xaml
    /// </summary>
    public partial class EditItem : Window
    {
        public List<string> Names = new List<string>();     

        public EditItem()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            foreach (string name in Names)
            {
                if (name == textBoxNewName.Text)
                {
                    MessageBox.Show("The name: '" + name + "' exists");
                    return;
                }
            }            
            this.Close();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
