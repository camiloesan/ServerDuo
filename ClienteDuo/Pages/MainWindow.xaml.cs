using ClienteDuo.DataService;
using ClienteDuo.Pages.Sidebars;
using ClienteDuo.Utilities;
using System;
using System.ComponentModel;
using System.ServiceModel;
using System.Windows;

namespace ClienteDuo.Pages
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ResizeMode = ResizeMode.NoResize;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Closing += OnWindowClosing;
            var launcher = new Launcher();
            Content = launcher;
        }

        public static void ShowMessageBox(string message, MessageBoxImage messageBoxImage)
        {
            Application.Current.MainWindow.IsEnabled = false;
            PopUpMessage popUpMessage = new PopUpMessage
            {
                Height = 220,
                Width = 370,
                Message = message,
                Topmost = true
            };
            popUpMessage.Show();
        }

        public static void NotifyLogOut(int userId, bool isGuest)
        {
            var userConnectionHandlerClient = new UserConnectionHandlerClient();
            if (!isGuest)
            {
                var user = new UserDTO
                {
                    ID = userId,
                    UserName = SessionDetails.Username,
                    PartyCode = SessionDetails.LobbyCode
                };
                try
                {
                    userConnectionHandlerClient.NotifyLogOut(user, SessionDetails.IsHost);
                }
                catch (TimeoutException)
                {
                    MessageBox.Show(Properties.Resources.DlgConnectionError);
                }
                catch (CommunicationException)
                {
                    MessageBox.Show(Properties.Resources.DlgConnectionError);
                }
                catch (Exception)
                {
                    MessageBox.Show(Properties.Resources.DlgConnectionError);
                }
            } 
            else
            {
                try
                {
                    userConnectionHandlerClient.NotifyGuestLeft(SessionDetails.LobbyCode, SessionDetails.Username);
                }
                catch (TimeoutException)
                {
                    MessageBox.Show(Properties.Resources.DlgConnectionError);
                }
                catch (CommunicationException)
                {
                    MessageBox.Show(Properties.Resources.DlgConnectionError);
                }
                catch (Exception)
                {
                    MessageBox.Show(Properties.Resources.DlgConnectionError);
                }
            }

        }

        private void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (SessionDetails.IsLogged)
            {
                NotifyLogOut(SessionDetails.UserId, SessionDetails.IsGuest);
            }

            if (SessionDetails.IsPlaying)
            {
                MatchPlayerManagerClient client = new MatchPlayerManagerClient();
                client.ExitMatch(SessionDetails.LobbyCode, SessionDetails.Username);
            }
        }

        public static bool ShowConfirmationBox(string message)
        {
            string messageBoxText = message;
            string caption = Properties.Resources.TitleAlert;

            MessageBoxButton buttons = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Question;

            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, buttons, icon);
            return result == MessageBoxResult.Yes;
        }
    }
}
