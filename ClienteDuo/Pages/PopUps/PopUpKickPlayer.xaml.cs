using ClienteDuo.DataService;
using ClienteDuo.Utilities;
using System;
using System.ComponentModel;
using System.ServiceModel;
using System.Windows;

namespace ClienteDuo.Pages.Sidebars
{
    public partial class PopUpKickPlayer : Window
    {
        LobbyManagerClient _lobbyManager;

        public PopUpKickPlayer()
        {
            InitializeComponent();

            Width = 500;
            Height = 300;

            KickReasonComboBox.Items.Add(Properties.Resources.RsnCheating);
            KickReasonComboBox.Items.Add(Properties.Resources.RsnInappropriateBehaviour);
            KickReasonComboBox.Items.Add(Properties.Resources.RsnSpam);
        }

        public string KickedUsername { get; set; }

        public void SetClient(LobbyManagerClient client)
        {
            _lobbyManager = client;
        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            System.Windows.Application.Current.MainWindow.IsEnabled = true;
        }

        private void BtnKickEvent(object sender, RoutedEventArgs e)
        {
            if (!KickReasonComboBox.Text.Equals(""))
            {
                System.Windows.Application.Current.MainWindow.IsEnabled = true;
                Close();

                try
                {
                    if (SessionDetails.IsPlaying)
                    {
                        MatchPlayerManagerClient client = new MatchPlayerManagerClient();

                        client.KickPlayerFromGame(SessionDetails.LobbyCode, KickedUsername, KickReasonComboBox.Text);
                    }
                    else
                    {
                        _lobbyManager.NotifyKickPlayer(SessionDetails.LobbyCode, KickedUsername, KickReasonComboBox.Text);
                    }
                }
                catch (CommunicationException)
                {
                    SessionDetails.AbortOperation();
                }
                catch (TimeoutException)
                {
                    SessionDetails.AbortOperation();
                }
            }
            else
            {
                LblErrorMessage.Content = Properties.Resources.DlgEmptyKickingReason;
            }
        }
    }
}
