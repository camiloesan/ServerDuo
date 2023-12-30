using System;
using System.Collections.Generic;
using ClienteDuo.DataService;
using ClienteDuo.Utilities;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.ServiceModel;

namespace ClienteDuo.Pages.Sidebars
{
    /// <summary>
    /// Interaction logic for SidebarFriendRequests.xaml
    /// </summary>
    public partial class SidebarFriendRequests : UserControl
    {
        private readonly UsersManagerClient _usersManagerClient;

        public SidebarFriendRequests()
        {
            InitializeComponent();
            _usersManagerClient = new UsersManagerClient();
            FillFriendRequestsPanel();
        }

        private void FillFriendRequestsPanel()
        {
            PanelFriendRequests.Children.Clear();
            IEnumerable<FriendRequestDTO> friendRequestsList = new List<FriendRequestDTO>();
            try
            {
                friendRequestsList = GetFriendRequestsByUserId(SessionDetails.UserId);
            }
            catch (CommunicationException)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgServiceException, MessageBoxImage.Error);
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
                result = AcceptFriendRequest(friendRequest);
            } 
            catch (CommunicationException)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgServiceException, MessageBoxImage.Error);
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
                result = DeclineFriendRequest(friendRequest);
            } 
            catch (CommunicationException)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgServiceException, MessageBoxImage.Error);
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

        private bool AcceptFriendRequest(FriendRequestDTO friendRequest)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.AcceptFriendRequest(friendRequest);
        }

        private bool DeclineFriendRequest(FriendRequestDTO friendRequest)
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.RejectFriendRequest(friendRequest.FriendRequestID);
        }
        
        private IEnumerable<FriendRequestDTO> GetFriendRequestsByUserId(int userId)
        {
            return _usersManagerClient.GetFriendRequestsList(userId);
        }
    }
}
