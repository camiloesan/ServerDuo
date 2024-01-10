﻿using ClienteDuo.DataService;
using ClienteDuo.Utilities;
using System.Windows;

namespace ClienteDuo.Pages.Sidebars
{
    public partial class PopUpKickPlayer : Window
    {
        MatchManagerClient _client;
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

        public void SetClient(MatchManagerClient client)
        {
            _client = client;
        }

        public void SetClient(LobbyManagerClient client)
        {
            _lobbyManager = client;
        }

        private void BtnKickEvent(object sender, RoutedEventArgs e)
        {
            if (!KickReasonComboBox.Text.Equals(""))
            {
                if (_client != null)
                {
                    _client.KickPlayerFromGame(SessionDetails.LobbyCode, KickedUsername, KickReasonComboBox.Text);
                }
                else
                {
                    _lobbyManager.NotifyKickPlayer(SessionDetails.LobbyCode, KickedUsername, KickReasonComboBox.Text);
                }
                Close();
            }
            else
            {
                LblErrorMessage.Content = Properties.Resources.DlgEmptyKickingReason;
            }
        }
    }
}