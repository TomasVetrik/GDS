using GDS_SERVER_WPF.DataCLasses;
using NetworkCommsDotNet.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace GDS_SERVER_WPF.Handlers
{
    public class ExecutedTaskHandler
    {
        public ExecutedTaskData executedTaskData;
        public Dictionary<ShortGuid, ComputerWithConnection> ClientsDictionary { get; set; }
        public List<ExecutedTaskHandler> handlers;
        public ListViewTaskDetailsHandler listViewHandler;
        public List<string> ipAddresses;
        public List<ComputerInTaskHandler> computers = new List<ComputerInTaskHandler>();
        public Semaphore semaphoreForSaveFile = new Semaphore(1, 1);
        public Semaphore semaphoreForFinishTask = new Semaphore(0, 1);
        public List<Thread> computersThreads = new List<Thread>();
        public ListView listViewAll;
        public ListView listViewSelected;
        public ListView listViewTaskDetailsProgress;
        public string imagePath = "Images/Done.ico";
        public List<string> MailsTo;

        bool stopped = false;

        public ExecutedTaskHandler(ExecutedTaskData _executedTaskData, List<string> _ipAddresses, ListView _listViewAll, ListView _listViewSelected, ListView _listViewTaskDetailsProgress, List<string> _MailsTo)
        {
            this.executedTaskData = _executedTaskData;
            this.ipAddresses = _ipAddresses;
            this.listViewAll = _listViewAll;
            this.listViewSelected = _listViewSelected;
            this.listViewTaskDetailsProgress = _listViewTaskDetailsProgress;
            this.MailsTo = _MailsTo;
            handlers = new List<ExecutedTaskHandler>();
        }

        private void RefreshList()
        {
            listViewHandler.Refresh();
        }

        public void Start()
        {
            try
            {
                FileHandler.Save<ExecutedTaskData>(executedTaskData, executedTaskData.GetFileName());
                if (executedTaskData._TaskData.Cloning)
                {
                    string sessionName = executedTaskData.Started + "_" + executedTaskData._TaskData.Name;
                    if (executedTaskData._TaskData.BaseImageSourcePath != "")
                        CreateSession(sessionName, executedTaskData._TaskData.BaseImageData.SourcePath);
                    if (executedTaskData._TaskData.DriveEImageSourcePath != "")
                        CreateSession(sessionName + "_DriveE", executedTaskData._TaskData.DriveEImageData.SourcePath);
                }
                RefreshList();
                for (int i = 0; i < executedTaskData._TaskData.TargetComputers.Count; i++)
                {
                    var computer = new ComputerInTaskHandler(executedTaskData, i, ipAddresses, semaphoreForSaveFile, listViewAll, listViewSelected, MailsTo)
                    {
                        ClientsDictionary = ClientsDictionary,
                        executedTaskHandler = this
                    };
                    computers.Add(computer);
                    var computerThread = new Thread(computer.Start);
                    computersThreads.Add(computerThread);
                    computerThread.Start();
                }
                AddComputersToListViewAll();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    listViewTaskDetailsProgress.Items.Add(executedTaskData);
                    listViewTaskDetailsProgress.SelectedItems.Clear();
                    listViewTaskDetailsProgress.SelectedItems.Add(listViewTaskDetailsProgress.Items[listViewTaskDetailsProgress.Items.Count - 1]);
                });
                Thread.Sleep(1000);

                foreach (ComputerInTaskHandler computer in computers)
                {
                    computer.semaphoreForTask.WaitOne();
                    if (!computer.stopped && !computer.failed)
                    {
                        executedTaskData.Done = (Convert.ToInt16(executedTaskData.Done) + 1).ToString();
                    }
                    else
                    {
                        if (computer.failed)
                        {
                            imagePath = "Images/Failed.ico";
                        }
                    }
                }
                Finish(imagePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Finish(string status)
        {
            try
            {
                if (stopped)
                {
                    status = "Images/Stopped.ico";
                }
                executedTaskData.Finished = DateTime.Now.ToString("dd.MM.yyyy HH-mm-ss");
                executedTaskData.Status = status;
                executedTaskData.Failed = (Convert.ToInt16(executedTaskData.Clients) - Convert.ToInt16(executedTaskData.Done)).ToString();
                semaphoreForSaveFile.WaitOne();
                FileHandler.Save<ExecutedTaskData>(executedTaskData, executedTaskData.GetFileName());
                RefreshList();
                semaphoreForSaveFile.Release();
                RemoveFromListViewTaskProgress();
                RemoveFromListViews();
                AutoClosingMessageBox.Show(executedTaskData.Name + "\n" + executedTaskData.MachineGroup, "DONE", 5000);
                if (executedTaskData._TaskData.Cloning)
                {
                    string sessionName = executedTaskData.Started + "_" + executedTaskData._TaskData.Name;
                    if (executedTaskData._TaskData.BaseImageSourcePath != "")
                        RemoveSession(sessionName);
                    if (executedTaskData._TaskData.DriveEImageSourcePath != "")
                        RemoveSession(sessionName + "_DriveE");
                }
                handlers.Remove(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void AddComputersToListViewAll()
        {
            try
            {
                foreach (ComputerInTaskHandler computer in computers)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        computer.progressComputerData = new ProgressComputerData("", computer.computer.Name, computer.step, executedTaskData.GetFileName(), "", computer.computer.MacAddress);
                        listViewAll.Items.Add(computer.progressComputerData);
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void AddComputersToListViewSelected()
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    listViewSelected.Items.Clear();
                    foreach (ComputerInTaskHandler computer in computers)
                    {
                        listViewSelected.Items.Add(computer.progressComputerData);
                        computer.listViewSelected = listViewSelected;
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }        

        private void RemoveFromListViewTaskProgress()
        {
            try
            {
                for (int i = listViewTaskDetailsProgress.Items.Count - 1; i >= 0; i--)
                {
                    ExecutedTaskData item = (ExecutedTaskData)listViewTaskDetailsProgress.Items[i];
                    if (item.Name == executedTaskData.Name && item.Started == executedTaskData.Started)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            listViewTaskDetailsProgress.Items.Remove(item);
                        });
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void RemoveFromListViews()
        {
            try
            {
                foreach (ComputerInTaskHandler computer in computers)
                {
                    computer.RemoveFromListViewAll();
                    computer.RemoveFromListViewSelected();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void Stop()
        {
            try
            {
                for (int i = computers.Count - 1; i >= 0; i--)
                    computers[i].Stop();
                stopped = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void CreateSession(string SessionName, string ImageSourcePath)
        {
            try
            {                
                string ImageFolder = Path.GetDirectoryName(ImageSourcePath);
                string args = "/New-Namespace /NamespaceType:AutoCast /FriendlyName:\"" + SessionName + "\" /Namespace:\"" + SessionName + "\" /ContentProvider:WDS /ConfigString:" + ImageFolder;
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Windows\System32\WDSUTIL.exe",
                    Arguments = args,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };                

                var process = Process.Start(processStartInfo);
                var output = process.StandardOutput.ReadToEnd();
                if (output.Contains("Transport Server is unavailable"))
                {
                    System.Diagnostics.Process.Start("net", "start WDSServer");
                    Thread.Sleep(5000);
                    CreateSession(SessionName, ImageSourcePath);
                }
                else
                {
                    if (!output.Contains("successfully") && !output.Contains("already"))
                    {
                        MessageBox.Show("Something is wrong: " + output);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public void RemoveSession(string SessionName)
        {
            try
            {
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Windows\System32\WDSUTIL.exe",
                    Arguments = "/remove-namespace /namespace:\"" + SessionName + "\" /Force",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                var process = Process.Start(processStartInfo);
                var output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }

    class AutoClosingMessageBox
    {
        System.Threading.Timer _timeoutTimer;
        readonly string _caption;
        AutoClosingMessageBox(string text, string caption, int timeout)
        {
            _caption = caption;
            _timeoutTimer = new System.Threading.Timer(OnTimerElapsed,
                null, timeout, System.Threading.Timeout.Infinite);
            MessageBox.Show(text, caption);
        }
        public static void Show(string text, string caption, int timeout)
        {
            new AutoClosingMessageBox(text, caption, timeout);
        }
        void OnTimerElapsed(object state)
        {
            IntPtr mbWnd = FindWindow(null, _caption);
            if (mbWnd != IntPtr.Zero)
                SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            _timeoutTimer.Dispose();
        }
        const int WM_CLOSE = 0x0010;
        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
    }
}
