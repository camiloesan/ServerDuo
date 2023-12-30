using ClienteDuo.DataService;
using ClienteDuo.Utilities;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClienteDuo.Pages.Sidebars
{
    public partial class PopUpKickPlayer : Window
    {
        MatchManagerClient _client;
        PartyManagerClient _partyManager;

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

        public void SetClient(PartyManagerClient client)
        {
            _partyManager = client;
        }

        private void BtnKickEvent(object sender, RoutedEventArgs e)
        {
            if (!KickReasonComboBox.Text.Equals(""))
            {
                if (_client != null)
                {
                    _client.KickPlayerFromGame(SessionDetails.PartyCode, KickedUsername, KickReasonComboBox.Text);
                } 
                else
                {
                    _partyManager.NotifyKickPlayer(SessionDetails.PartyCode, KickedUsername, KickReasonComboBox.Text);
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
