using GDS_Client.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.ServiceProcess;
using System.Threading;
using System.Timers;

namespace GDS_Client
{
    class Program
    {      
        static void Main(string[] args)
        {
            DisableConsoleQuickEdit.SetQuickEdit(true);
            SetTimer();
            Repeater();
            Console.WriteLine("Nejak som sa sem dostal");
            timerForKillingUpdates.Stop();
            timerForKillingUpdates.Dispose();
            Console.ReadLine();
        }
        static Listener listener;

        readonly static string FileName = @"D:\Temp\GDSClient\GDS_Client_LOG.txt";
        private static System.Timers.Timer timerForKillingUpdates;

        private static void SetTimer()
        {
            // Create a timer with a two second interval.
            timerForKillingUpdates = new System.Timers.Timer(10000);
            // Hook up the Elapsed event for the timer. 
            timerForKillingUpdates.Elapsed += OnTimedStopWindowsUpdateService;
            timerForKillingUpdates.AutoReset = true;
            timerForKillingUpdates.Enabled = true;
        }

        private static void OnTimedStopWindowsUpdateService(Object source, ElapsedEventArgs e)
        {
            try
            {
                List<ServiceController> services = new List<ServiceController>();
                services.Add(new ServiceController("wuauserv"));
                services.Add(new ServiceController("WaaSMedicSvc"));
                if (File.Exists(@"D:\Temp\GDSClient\WindowsUpdateCheck.txt"))
                {
                    string text = System.IO.File.ReadAllText(@"D:\Temp\GDSClient\WindowsUpdateCheck.txt");
                    if (text.Contains("TRUE"))
                    {
                        foreach (ServiceController service in services)
                        {

                            service.Refresh();
                            if ((service.Status.Equals(ServiceControllerStatus.Running)))
                            {
                                listener.WriteToLogs("Stopping service: " + service.ServiceName);
                                service.Stop();
                            }
                        }
                    }
                }
            }
            catch { }
        }

        static void Repeater()
        {
            try
            {
                try
                {
                    if (!File.Exists(FileName))
                    {
                        using (StreamWriter sw = File.AppendText(FileName))
                        {
                            sw.WriteLine(DateTime.Now.ToString() + ": CREATE NEW LOG FILE");
                        }
                    }
                }
                catch { }
                listener = new Listener
                {
                    running = false
                };
                listener.WriteToLogs("START");
                listener.StartListener();
                
                Thread.Sleep(5000);
                while (listener.running)
                {
                    Thread.Sleep(10000);                    
                }
            }
            catch (Exception ex)
            {
                listener.WriteToLogs("Problem with repeater:" + ex.ToString());
            }
            Thread.Sleep(10000);
            Repeater();
        }
    }    
}