using System.Globalization;

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
        public static int PartyCode { get; set; } = 0;
        public static int TotalWins { get; set; } = 0;
        public static int PictureID { get; set; } = 0;

        public static void CleanSessionDetails()
        {
            UserId = 0;
            Username = string.Empty;
            Email = string.Empty;
            PartyCode = 0;
            IsGuest = true;
        }
    }
}
