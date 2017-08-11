using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;

namespace GDS_SERVER_WPF
{
    public class Listener
    {
        public ListBox listBox1;
        public List<ClientHandler> clients;
        public Label labelOnlineClients;
        public ListViewMachinesAndTasksHandler listViewMachinesAndTasksHandler; 

        public void StartListener()
        {            
            var serverSocket = new TcpListener(IPAddress.Any, 100);
            var clientSocket = default(TcpClient);            
            clients = new List<ClientHandler>();
            listViewMachinesAndTasksHandler.clients = clients;
            Application.Current.Dispatcher.Invoke(() =>
            {
                labelOnlineClients.Content = "Online: " + clients.Count;
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
                    client.startClient(clientSocket, clients.Count, listBox1, clients, labelOnlineClients, listViewMachinesAndTasksHandler);
                    clients.Add(client);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        labelOnlineClients.Content = "Online: " + clients.Count;
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
