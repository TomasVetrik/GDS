using GDS_SERVER_WPF.Handlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace GDS_SERVER_WPF
{
    public class Listener
    {
        public ListBox listBox1;
        public List<ClientHandler> clients;
        public Label labelOnlineClients;
        public Label labelOfflineClients;
        public ListViewMachinesAndTasksHandler listViewMachinesAndTasksHandler;
        public int clientsAll = 0;
        public List<ExecutedTaskHandler> executedTasksHandlers;
        

        public void StartListener()
        {            
            var serverSocket = new TcpListener(IPAddress.Any, 65452);
            var clientSocket = default(TcpClient);            
            clients = new List<ClientHandler>();
            listViewMachinesAndTasksHandler.clients = clients;
            clientsAll = Directory.GetFiles(@".\Machine Groups\", "*.my", SearchOption.AllDirectories).Length;
            Application.Current.Dispatcher.Invoke(() =>
            {
                labelOnlineClients.Content = "Online: " + clients.Count;
                labelOfflineClients.Content = "Offline: " + (clientsAll - clients.Count);
            });

            try
            {
                serverSocket.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }                  
            
            while (true)
            {
                try
                {                    
                    clientSocket = serverSocket.AcceptTcpClient();                                        
                    var client = new ClientHandler();
                    client.startClient(clientSocket, clients.Count, listBox1, clients, labelOnlineClients, labelOfflineClients, listViewMachinesAndTasksHandler, executedTasksHandlers);
                    clients.Add(client);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        labelOnlineClients.Content = "Online: " + clients.Count;
                        labelOfflineClients.Content = "Offline: " + (clientsAll - clients.Count);
                    });
                }
                catch (Exception ex)
                {                    
                    Console.WriteLine(ex.ToString());
                    throw;
                }
            }            
        }   
    }
}
