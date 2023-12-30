using ClienteDuo.DataService;
using ClienteDuo.Utilities;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
namespace ClienteDuo.Pages.Sidebars
{
    /// <summary>
    /// Interaction logic for SidebarBlockedUsers.xaml
    /// </summary>
    public partial class SidebarBlockedUsers : UserControl
    {
        public SidebarBlockedUsers()
        {
            InitializeComponent();
            FillBlockedUsersPanel();
        }

        private void FillBlockedUsersPanel()
        {
            PanelBlockedUsers.Children.Clear();
            IEnumerable<UserBlockedDTO> blockedUsersList = new List<UserBlockedDTO>();
            try
            {
                blockedUsersList = UsersManager.GetBlockedUsersListByUserId(SessionDetails.UserId);
            }
            catch (CommunicationException)
            {
                SessionDetails.AbortOperation();
            }
            catch (TimeoutException)
            {
                SessionDetails.AbortOperation();
            }

            foreach (UserBlockedDTO blockedUser in blockedUsersList)
            {
                CreateBlockedUserPanel(blockedUser);
            }
        }

        private void CreateBlockedUserPanel(UserBlockedDTO blockedUser)
        {
            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            PanelBlockedUsers.Children.Add(stackPanel);

            Label lblSender = new Label
            {
                Content = blockedUser.BlockedUsername
            };
            stackPanel.Children.Add(lblSender);

            Button btnUnblock = new Button
            {
                Content = Properties.Resources.BtnUnblockUser,
                DataContext = blockedUser
            };
            btnUnblock.Click += UnblockUserEvent;
            stackPanel.Children.Add(btnUnblock);
        }

        private void UnblockUserEvent(object sender, RoutedEventArgs e)
        {
            UserBlockedDTO blockedUser = ((FrameworkElement)sender).DataContext as UserBlockedDTO;

            bool result = false;
            try
            {
                result = UsersManager.UnblockUserByBlockId(blockedUser.BlockID) == 1;
            }
            catch (CommunicationException)
            {
                SessionDetails.AbortOperation();
            }
            catch (TimeoutException)
            {
                SessionDetails.AbortOperation();
            }

            if (result)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgUnblockSuccess, MessageBoxImage.Information);
                MainMenu mainMenu = new MainMenu();
                Application.Current.MainWindow.Content = mainMenu;
            }
        }

        private void BtnCancelEvent(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
