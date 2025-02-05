﻿using System.Configuration;
using System.Windows;
using MailKit.Net.Imap;
using MailKit.Security;

namespace GmailClient
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        string server = ConfigurationManager.AppSettings["gmail_server"]; // sets the server address
        int port = int.Parse(ConfigurationManager.AppSettings["gmail_port"]); //sets the server port

        private readonly string username; // paste here your email
        private readonly string password; // paste here your password 
        private bool autoFill = true;  // Turning off this flag will turn off autofill credentials

        public LoginWindow()
        {
            InitializeComponent();
            if (autoFill) AutoSetCredentials();
        }

        public void AutoSetCredentials()
        {
            txtboxUsername.Text = username;
            txtboxPassword.Password = password;
        }

        private async void buttonLogin_Click(object sender, RoutedEventArgs e)
        {
            using (var client = new ImapClient())
            {
                // Asynchronous connection of the client with the specified server and port
                await client.ConnectAsync(server, port, SecureSocketOptions.SslOnConnect);

                // Try to asynchronously authenticate using the login and password entered by the user
                try
                {
                    await client.AuthenticateAsync(txtboxUsername.Text, txtboxPassword.Password);

                    // If the authentication is successful, transfer the login data to the MainWindow window and start it
                    if (client.IsAuthenticated)
                    {
                        // Create MainWindow object
                        MainWindow mainWindow = new MainWindow(txtboxUsername.Text, txtboxPassword.Password);
                        // Open Mainwindow with given credentials for login
                        mainWindow.Show();
                        // Disconnecting client
                        await client.DisconnectAsync(true);
                        // Close LoginWindow
                        this.Close();
                    }
                    #region [ DEBUG ]
                    //var all = StatusItems.Recent;
                    //var folders = await client.GetFoldersAsync(client.PersonalNamespaces[0], all, true);
                    //foreach (var folder in folders)
                    //{
                    //    MessageBox.Show($"{folder.FullName}");
                    //}
                    #endregion
                }
                catch (AuthenticationException ex)
                {
                    MessageBox.Show($"{ex.Message}", "Authentication error");
                }
                finally
                {
                    await client.DisconnectAsync(true);
                }
            }
        }
    }
}
