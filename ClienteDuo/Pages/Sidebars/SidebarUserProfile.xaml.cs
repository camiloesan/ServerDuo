using ClienteDuo.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ClienteDuo.Pages.Sidebars
{
    public partial class SidebarUserProfile : UserControl
    {
        public SidebarUserProfile()
        {
            InitializeComponent();
            SetProfilePicture();
            FillLabels();
        }

        private void FillLabels()
        {
            LblPlayerId.Content = SessionDetails.UserId;
            LblUsername.Content = SessionDetails.Username;
            LblEmail.Content = SessionDetails.Email;
            LblTotalWins.Content = SessionDetails.TotalWins;
        }

        private void SetProfilePicture()
        {
            BitmapImage bitmapImage = new BitmapImage(new System.Uri("pack://application:,,,/ClienteDuo;component/Images/pfp" + SessionDetails.PictureID + ".png"));
            ImageProfilePicture.Source = bitmapImage;
            ImageProfilePicture.Stretch = Stretch.UniformToFill;
        }

        private void BtnCancelEvent(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        private void BtnModifyProfileEvent(object sender, RoutedEventArgs e)
        {
            ModifyProfile modifyProfile = new ModifyProfile();
            Application.Current.MainWindow.Content = modifyProfile;
        }
    }
}
