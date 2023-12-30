using ClienteDuo.DataService;

using ClienteDuo.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClienteDuo.Pages.Sidebars
{
    /// <summary>
    /// Interaction logic for SidebarLeaderboard.xaml
    /// </summary>
    public partial class SidebarLeaderboard : UserControl
    {
        public SidebarLeaderboard()
        {
            InitializeComponent();
            FillFriendsPanel();
        }

        private void FillFriendsPanel()
        {
            PanelLeaderboard.Children.Clear();

            IEnumerable userList = new List<UserDTO>();
            try
            {
                userList = GetLeaderboard();
            }
            catch (CommunicationException)
            {
                MainWindow.ShowMessageBox(Properties.Resources.DlgConnectionError, MessageBoxImage.Error);
            }

            foreach (UserDTO user in userList)
            {
                CreateUserPanel(user);
            }
        }

        private void CreateUserPanel(UserDTO user)
        {
            StackPanel stackPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(0, 7, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                Background = new SolidColorBrush(Colors.DimGray),
                Width = 200,
                Height = 40
            };
            PanelLeaderboard.Children.Add(stackPanel);

            Label labelUsername = new Label
            {
                Foreground = new SolidColorBrush(Colors.Black),
                Content = user.UserName,
                Margin = new Thickness(5, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            stackPanel.Children.Add(labelUsername);

            Label labelTotalWins = new Label
            {
                Foreground = new SolidColorBrush(Colors.Black),
                Content = user.TotalWins,
                Margin = new Thickness(5, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            stackPanel.Children.Add(labelTotalWins);
        }

        private IEnumerable<UserDTO> GetLeaderboard()
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            return usersManagerClient.GetTopTenWinners();
        }

        private void BtnCancelEvent(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
