using ClienteDuo.DataService;
using ClienteDuo.Utilities;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClienteDuo.Pages.Sidebars
{
    public partial class SidebarFriends : UserControl
    {
        private SidebarAddFriend _sidebarAddFriend;
        private SidebarFriendRequests _sidebarFriendRequests;
        private SidebarBlockedUsers _sidebarBlockedUsers;
        private readonly IEnumerable<FriendshipDTO> _onlineFriends;

        public SidebarFriends()
        {
            InitializeComponent();
            _onlineFriends = UsersManager.GetOnlineFriends(SessionDetails.UserId);
            InitializeBars();
            FillFriendsPanel();
        }

        private void InitializeBars()
        {
            _sidebarAddFriend = new SidebarAddFriend
            {
                Visibility = Visibility.Collapsed
            };
            FriendsBar.Children.Add(_sidebarAddFriend);

            _sidebarFriendRequests = new SidebarFriendRequests
            {
                Visibility = Visibility.Collapsed
            };
            FriendsBar.Children.Add(_sidebarFriendRequests);

            _sidebarBlockedUsers = new SidebarBlockedUsers
            {
                Visibility = Visibility.Collapsed
            };
            FriendsBar.Children.Add(_sidebarBlockedUsers);
        }

        private void FillFriendsPanel()
        {
            PanelFriends.Children.Clear();
            IEnumerable<FriendshipDTO> friendsList = new List<FriendshipDTO>();
            try
            {
                friendsList = UsersManager.GetFriendsListByUserId(SessionDetails.UserId);
            }
            catch (CommunicationException)
            {
                SessionDetails.AbortOperation();
            }
            catch (TimeoutException)
            {
                SessionDetails.AbortOperation();
            }

            foreach (FriendshipDTO friend in friendsList)
            {
                if (friend.Friend1ID != SessionDetails.UserId)
                {
                    CreateFriendPanel(friend.Friend1Username, friend.FriendshipID);
                }
                else if (friend.Friend2ID != SessionDetails.UserId)
                {
                    CreateFriendPanel(friend.Friend2Username, friend.FriendshipID);
                }
            }
        }

        private void CreateFriendPanel(string username, int friendshipId)
        {
            StackPanel stackPanelContainer = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(0, 7, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#0D1C30"),
                Width = 200,
                Height = 80
            };
            PanelFriends.Children.Add(stackPanelContainer);

            StackPanel stackPanelUsername = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = 40
            };
            stackPanelContainer.Children.Add(stackPanelUsername);

            StackPanel stackPanelButtons = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Height = 40
            };
            stackPanelContainer.Children.Add(stackPanelButtons);

            foreach (var onlineFriend in _onlineFriends)
            {
                if (onlineFriend.FriendshipID == friendshipId)
                {
                    Label activeStatus = new Label
                    {
                        Foreground = new SolidColorBrush(Colors.White),
                        Content = "●",
                        Margin = new Thickness(10, 0, 5, 0),
                        FontSize = 20,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    stackPanelUsername.Children.Add(activeStatus);
                }
            }

            Label usernameName = new Label
            {
                Foreground = new SolidColorBrush(Colors.White),
                Content = username,
                Margin = new Thickness(5, 0, 10, 0),
                FontSize = 16,
                VerticalAlignment = VerticalAlignment.Center
            };
            stackPanelUsername.Children.Add(usernameName);

            Tuple<int, string> friendshipUsernameTuple = Tuple.Create(friendshipId, username);
            var btnViewProfile = new Button
            {
                Content = Properties.Resources.BtnProfile,
                FontSize = 14,
                BorderThickness = new Thickness(3, 3, 3, 3),
                Foreground = new SolidColorBrush(Colors.White),
                Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF6B472B"),
                BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF452308"),
                Margin = new Thickness(0, 0, 5, 0),
                DataContext = friendshipUsernameTuple
            };
            btnViewProfile.Click += ViewProfileEvent;
            stackPanelButtons.Children.Add(btnViewProfile);

            Button btnUnfriend = new Button
            {
                Content = Properties.Resources.BtnUnfriend,
                FontSize = 14,
                BorderThickness = new Thickness(3, 3, 3, 3),
                Foreground = new SolidColorBrush(Colors.White),
                Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF6B472B"),
                BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF452308"),
                Margin = new Thickness(5, 0, 0, 0),
                DataContext = friendshipId
            };
            btnUnfriend.Click += UnfriendEvent;
            stackPanelButtons.Children.Add(btnUnfriend);
        }

        private void ViewProfileEvent(object sender, RoutedEventArgs e)
        {
            Tuple<int, string> friendshipUsernameTuple
                = ((FrameworkElement)sender).DataContext as Tuple<int, string>;
            MainMenu.ShowPopUpUserDetails(friendshipUsernameTuple.Item1, friendshipUsernameTuple.Item2);
        }

        private void UnfriendEvent(object sender, RoutedEventArgs e)
        {
            int friendshipId = (int)((FrameworkElement)sender).DataContext;
            bool confirmation = MainWindow.ShowConfirmationBox(Properties.Resources.DlgUnfriendConfirmation);

            if (confirmation)
            {
                bool result = false;
                try
                {
                    result = UsersManager.DeleteFriendshipById(friendshipId);
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
                    MainWindow.ShowMessageBox(Properties.Resources.DlgUnfriend, MessageBoxImage.Information);
                }
                FillFriendsPanel();
            }
        }

        private void BtnCancelEvent(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }

        private void BtnFriendRequestsEvent(object sender, RoutedEventArgs e)
        {
            _sidebarFriendRequests.Visibility = Visibility.Visible;
        }

        private void BtnAddFriendEvent(object sender, RoutedEventArgs e)
        {
            _sidebarAddFriend.Visibility = Visibility.Visible;
        }

        private void BtnBlockedUsersEvent(object sender, RoutedEventArgs e)
        {
            _sidebarBlockedUsers.Visibility = Visibility.Visible;
        }
    }
}
