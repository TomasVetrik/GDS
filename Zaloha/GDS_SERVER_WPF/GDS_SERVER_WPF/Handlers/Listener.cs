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
        public List<ClientHandler> clients;
        public Label labelOnlineClients;
        public Label labelOfflineClients;
        public Label labelAllClients;
        public ListViewMachinesAndTasksHandler listViewMachinesAndTasksHandler;
        public int clientsAll = 0;
        public List<ExecutedTaskHandler> executedTasksHandlers;
        public ListBox console;

        TcpClient clientSocket;
        TcpListener serverSocket;
        Thread checkingClients;

        public void StartListener()
        {
            serverSocket = new TcpListener(IPAddress.Any, 65452);
            clientSocket = default(TcpClient);
            clients = new List<ClientHandler>();
            listViewMachinesAndTasksHandler.clients = clients;
            clientsAll = Directory.GetFiles(@".\Machine Groups\", "*.my", SearchOption.AllDirectories).Length;
            Application.Current.Dispatcher.Invoke(() =>
            {
                labelOnlineClients.Content = "Online: " + clients.Count;
                labelOfflineClients.Content = "Offline: " + (clientsAll - clients.Count);
                labelAllClients.Content = "Clients: " + clientsAll;
            });

            try
            {
                serverSocket.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());                
            }            
            serverSocket.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback),serverSocket);
        }        

        public void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;            
            TcpClient clientSocket = listener.EndAcceptTcpClient(ar);
            var client = new ClientHandler();

            client.startClient(clientSocket, clients.Count, clients, labelOnlineClients, labelOfflineClients, listViewMachinesAndTasksHandler, executedTasksHandlers, labelAllClients, console, this);
            clients.Add(client);
            Application.Current.Dispatcher.Invoke(() =>
            {
                labelOnlineClients.Content = "Online: " + clients.Count;
                labelOfflineClients.Content = "Offline: " + (clientsAll - clients.Count);
            });
            listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);
        }
        
        public void Disconnect(ClientHandler client)
        {
            try
            {
                clients.Remove(client);
            }
            catch
            {
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                labelOnlineClients.Content = "Online: " + clients.Count;
                labelOfflineClients.Content = "Offline: " + (clientsAll - clients.Count);
            });
        }
    }
}
