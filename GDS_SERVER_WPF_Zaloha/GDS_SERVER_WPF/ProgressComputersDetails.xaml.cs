using GDS_SERVER_WPF.DataCLasses;
using System.Threading;
using System.Windows;

namespace GDS_SERVER_WPF
{
    /// <summary>
    /// Interaction logic for ProgressComputersDetails.xaml
    /// </summary>
    public partial class ProgressComputersDetails : Window
    {
        public ProgressComputersDetails()
        {
            InitializeComponent();
        }

        public ExecutedTaskData executedTaskData;
        

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(executedTaskData != null)
            {
                executedTaskData = FileHandler.Load<ExecutedTaskData>(executedTaskData.GetFileName());
                foreach (ProgressComputerData progressComputerData in executedTaskData.progressComputerData)
                {
                    listViewProgressDetails.Items.Add(progressComputerData);
                }
            }
        }
    }
}
