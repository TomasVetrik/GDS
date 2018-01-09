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
    /// Interaction logic for CreateRenameCancel.xaml
    /// </summary>
    public partial class CreateRenameCancel : Window
    {
        public CreateRenameCancel()
        {
            InitializeComponent();
        }

        public bool cancel = true;
        public bool renameOld = false;
        public bool createNew = false;        

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {                        
            this.Close();
        }

        private void btnRenameOld_Click(object sender, RoutedEventArgs e)
        {
            renameOld = true;
            cancel = false;
            this.Close();
        }

        private void btnCreateNew_Click(object sender, RoutedEventArgs e)
        {
            createNew = true;
            cancel = false;
            this.Close();
        }
    }
}
