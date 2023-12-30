using ClienteDuo.DataService;
using ClienteDuo.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel;
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
    /// <summary>
    /// Interaction logic for EmailConfirmation.xaml
    /// </summary>
    public partial class EmailConfirmation : Page
    {
        private readonly UsersManagerClient _usersManagerClient = new UsersManagerClient();

        public EmailConfirmation()
        {
            InitializeComponent();
        }

        private void BtnContinueEvent(object sender, RoutedEventArgs e)
        {
            string email = TBoxEmail.Text.Trim();
            string language = System.Threading.Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName;
            try
            {
                RequestCode(email, language);
            } 
            catch(CommunicationException)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgServiceException, MessageBoxImage.Error);
            }
        }

        private void RequestCode(string email, string lang)
        {
            if (!_usersManagerClient.IsEmailTaken(email))
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgEmailNonExistent, MessageBoxImage.Information);
            }
            else
            {
                int confirmationCode = _usersManagerClient.SendConfirmationCode(email, lang);

                if (confirmationCode != -1)
                {
                    EmailCodeVerification emailCodeVerification = new EmailCodeVerification(email, confirmationCode);
                    Application.Current.MainWindow.Content = emailCodeVerification;
                }
            }
        }

        private void BtnCancelEvent(object sender, RoutedEventArgs e)
        {
            if (SessionDetails.IsGuest)
            {
                Login login = new Login();
                Application.Current.MainWindow.Content = login;
            }
            else
            {
                MainMenu mainMenu = new MainMenu();
                Application.Current.MainWindow.Content = mainMenu;
            }
        }
    }
}
