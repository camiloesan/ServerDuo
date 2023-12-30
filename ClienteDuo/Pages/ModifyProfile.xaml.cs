using ClienteDuo.DataService;
using ClienteDuo.Pages.Sidebars;
using ClienteDuo.Utilities;
using System;
using System.Collections.Generic;
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

namespace ClienteDuo.Pages
{
    public partial class ModifyProfile : Page
    {
        int _selectedPictureId = 0;

        public ModifyProfile()
        {
            InitializeComponent();
            InitializeCurrentProfilePicture();
            InitializeAvailableProfilePictures();
        }

        private void InitializeCurrentProfilePicture()
        {
            SetCurrentProfilePicturePreview(SessionDetails.PictureID);
        }

        private void SetCurrentProfilePicturePreview(int pictureId)
        {
            _selectedPictureId = pictureId;
            BitmapImage bitmapImage = bitmapImage = new BitmapImage(new System.Uri("pack://application:,,,/ClienteDuo;component/Images/pfp" + pictureId + ".png"));
            ImageCurrentProfilePicture.Source = bitmapImage;
            ImageCurrentProfilePicture.Stretch = Stretch.UniformToFill;
        }

        private void InitializeAvailableProfilePictures()
        {
            ImagePfp0.Source = new BitmapImage(new System.Uri("pack://application:,,,/ClienteDuo;component/Images/pfp0.png"));
            ImagePfp1.Source = new BitmapImage(new System.Uri("pack://application:,,,/ClienteDuo;component/Images/pfp1.png"));
            ImagePfp2.Source = new BitmapImage(new System.Uri("pack://application:,,,/ClienteDuo;component/Images/pfp2.png"));
            ImagePfp3.Source = new BitmapImage(new System.Uri("pack://application:,,,/ClienteDuo;component/Images/pfp3.png"));
        }

        private void BtnContinueEvent(object sender, RoutedEventArgs e)
        {
            bool result = false;
            try
            {
                result = UpdateProfilePicture(_selectedPictureId);
            } 
            catch (CommunicationException)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgServiceException, MessageBoxImage.Error);
            }

            if (result)
            {
                SessionDetails.PictureID = _selectedPictureId;
                MainWindow.ShowMessageBox(Properties.Resources.DlgProfilePictureUpdated, MessageBoxImage.Information);
                MainMenu mainMenu = new MainMenu();
                Application.Current.MainWindow.Content = mainMenu;
            }
        }

        private bool UpdateProfilePicture(int pictureId)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.UpdateProfilePictureByUserId(SessionDetails.UserId, pictureId);
        }

        private void BtnCancelEvent(object sender, RoutedEventArgs e)
        {
            MainMenu mainMenu = new MainMenu();
            Application.Current.MainWindow.Content = mainMenu;
        }

        private void BtnChangePasswordEvent(object sender, RoutedEventArgs e)
        {
            EmailConfirmation emailConfirmation = new EmailConfirmation();
            Application.Current.MainWindow.Content = emailConfirmation;
        }

        private void BtnPfp0Event(object sender, RoutedEventArgs e)
        {
            SetCurrentProfilePicturePreview(0);
        }
        private void BtnPfp1Event(object sender, RoutedEventArgs e)
        {
            SetCurrentProfilePicturePreview(1);
        }

        private void BtnPfp2Event(object sender, RoutedEventArgs e)
        {
            SetCurrentProfilePicturePreview(2);
        }

        private void BtnPfp3Event(object sender, RoutedEventArgs e)
        {
            SetCurrentProfilePicturePreview(3);
        }
    }
}
