using ClienteDuo.Pages;
using ClienteDuo.Pages.Sidebars;
using System.Windows;

namespace ClienteDuo.Utilities
{
    public sealed class SessionDetails
    {
        private SessionDetails()
        {
        }

        public static int UserId { get; set; }
        public static string Username { get; set; }
        public static bool IsGuest { get; set; } = true;
        public static bool IsHost { get; set; }
        public static string Email { get; set; }
        public static int PartyCode { get; set; }
        public static int TotalWins { get; set; }
        public static int PictureID { get; set; }

        public static void CleanSessionDetails()
        {
            UserId = 0;
            Username = string.Empty;
            IsGuest = true;
            IsHost = false;
            Email = string.Empty;
            PartyCode = 0;
            TotalWins = 0;
            PictureID = 0;
        }

        public static void AbortOperation()
        {
            MainWindow.NotifyLogOut(UserId, IsGuest);
            CleanSessionDetails();
            MessageBox.Show(Properties.Resources.DlgConnectionError);
            Application.Current.Shutdown();
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
        }
    }
}
