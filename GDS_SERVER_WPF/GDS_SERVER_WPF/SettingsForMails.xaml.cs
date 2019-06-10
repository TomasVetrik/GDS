using GDS_SERVER_WPF.DataCLasses;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for SettingsForMails.xaml
    /// </summary>
    public partial class SettingsForMails : Window
    {
        string path = @".\MailsTo.my";
        string pathInActive = @".\MailsTo_InActive.my";
        public MailsData mailsData = new MailsData();
        public MailsData mailsDataInActive = new MailsData();

        public SettingsForMails()
        {
            InitializeComponent();
        }
        private void RemoveMailFromData(ListView list, MailsData Mails)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Are you sure you want to remove?", "Removing Confirmation", System.Windows.MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes)
            {
                if (list.SelectedIndex == -1)
                {
                    return;
                }
                list.Items.Remove(list.SelectedItem);
                Mails.Emails.Clear();
                foreach (string mail in list.Items)
                {
                    Mails.Emails.Add(mail);
                }
            }
        }
        private void MoveMailFromToData(ListView listFrom, MailsData MailsFrom, ListView listTo, MailsData MailsTo)
        {
            if (listFrom.SelectedIndex == -1)
            {
                return;
            }
            listTo.Items.Add(listFrom.SelectedItem);
            listFrom.Items.Remove(listFrom.SelectedItem);
            MailsFrom.Emails.Clear();
            foreach (string mail in listFrom.Items)
            {
                MailsFrom.Emails.Add(mail);
            }
            MailsTo.Emails.Clear();
            foreach (string mail in listTo.Items)
            {
                MailsTo.Emails.Add(mail);
            }
        }
        private void MenuItemRemove_Click(object sender, RoutedEventArgs e)
        {
            RemoveMailFromData(listViewMails, mailsData);
        }
        
        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void buttonOK_Click(object sender, RoutedEventArgs e)
        {
            FileHandler.Save<MailsData>(mailsData, path);
            FileHandler.Save<MailsData>(mailsDataInActive, pathInActive);
            this.Close();
        }

        private void buttonNew_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxMail.Text != "")
            {
                if (!listViewMails.Items.Contains(textBoxMail.Text) && !listViewMailsInActive.Items.Contains(textBoxMail.Text))
                {
                    mailsData.Emails.Add(textBoxMail.Text);
                    listViewMails.Items.Clear();
                    for (int i = 0; i < mailsData.Emails.Count; i++)
                        listViewMails.Items.Add(mailsData.Emails[i]);
                }
            }
        }

        private void ListViewMails_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RemoveMailFromData(listViewMails, mailsData);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(path))
            {
                FileHandler.Save<MailsData>(mailsData, path);
            }            
            mailsData = FileHandler.Load<MailsData>(path);
            if (mailsData == null)
            {
                mailsData = new MailsData();
            }            
            foreach (string mail in mailsData.Emails)
            {
                listViewMails.Items.Add(mail);
            }
            if (!File.Exists(pathInActive))
            {
                FileHandler.Save<MailsData>(mailsDataInActive, pathInActive);
            }
            mailsDataInActive = FileHandler.Load<MailsData>(pathInActive);
            if (mailsDataInActive == null)
            {
                mailsDataInActive = new MailsData();
            }
            foreach (string mail in mailsDataInActive.Emails)
            {
                listViewMailsInActive.Items.Add(mail);
            }
        }

        private void MenuItemRemoveInActive_Click(object sender, RoutedEventArgs e)
        {
            RemoveMailFromData(listViewMailsInActive, mailsDataInActive);
        }

        private void ButtonToActive_Click(object sender, RoutedEventArgs e)
        {
            MoveMailFromToData(listViewMailsInActive, mailsDataInActive, listViewMails, mailsData);
        }

        private void ButtonToInActive_Click(object sender, RoutedEventArgs e)
        {
            MoveMailFromToData(listViewMails, mailsData, listViewMailsInActive, mailsDataInActive);
        }

        private void ListViewMailsInActive_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RemoveMailFromData(listViewMailsInActive, mailsDataInActive);
        }
    }
}
