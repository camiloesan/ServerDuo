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
    /// Interaction logic for SidebarFriendRequests.xaml
    /// </summary>
    public partial class SidebarFriendRequests : UserControl
    {
        public SidebarFriendRequests()
        {
            InitializeComponent();
            FillFriendRequestsPanel();
        }

        private void FillFriendRequestsPanel()
        {
            PanelFriendRequests.Children.Clear();
            IEnumerable<FriendRequestDTO> friendRequestsList = new List<FriendRequestDTO>();
            try
            {
                friendRequestsList = UsersManager.GetFriendRequestsByUserId(SessionDetails.UserId);
            }
            catch (CommunicationException)
            {
                SessionDetails.AbortOperation();
            }
            catch (TimeoutException)
            {
                SessionDetails.AbortOperation();
            }

            foreach (FriendRequestDTO friendRequest in friendRequestsList)
            {
                CreateFriendRequestPanel(friendRequest);
            }
        }

        private void CreateFriendRequestPanel(FriendRequestDTO friendRequest)
        {
            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            PanelFriendRequests.Children.Add(stackPanel);

            Label lblSender = new Label
            {
                Content = friendRequest.SenderUsername
            };
            stackPanel.Children.Add(lblSender);

            Button btnAccept = new Button
            {
                Content = Properties.Resources.BtnAccept,
                DataContext = friendRequest
            };
            btnAccept.Click += AcceptFriendRequestEvent;
            stackPanel.Children.Add(btnAccept);

            Button btnReject = new Button
            {
                Content = Properties.Resources.BtnReject,
                DataContext = friendRequest
            };
            btnReject.Click += DeclineFriendRequestEvent;
            stackPanel.Children.Add(btnReject);
        }

        private void AcceptFriendRequestEvent(object sender, RoutedEventArgs e)
        {
            var friendRequest = ((FrameworkElement)sender).DataContext as FriendRequestDTO;

            bool result = false;
            try
            {
                result = UsersManager.AcceptFriendRequest(friendRequest);
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
                MainWindow.ShowMessageBox(Properties.Resources.DlgNowFriends, MessageBoxImage.Information);
                FillFriendRequestsPanel();
                MainMenu mainMenu = new MainMenu();
                Application.Current.MainWindow.Content = mainMenu;
            }
        }

        private void DeclineFriendRequestEvent(object sender, RoutedEventArgs e)
        {
            FriendRequestDTO friendRequest = ((FrameworkElement)sender).DataContext as FriendRequestDTO;

            bool result = false;
            try
            {
                result = UsersManager.DeclineFriendRequest(friendRequest);
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
                MainWindow.ShowMessageBox(Properties.Resources.DlgFriendRequestDeleted, MessageBoxImage.Information);
                FillFriendRequestsPanel();
            }
        }

        private void BtnCancelEvent(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
