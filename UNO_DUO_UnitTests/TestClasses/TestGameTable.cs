using ClienteDuo.DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClienteDuo.TestClasses
{
    public class TestGameTable : IMatchManagerCallback
    {
        public void GameOver()
        {
            IsGameActive = false;
        }

        public void PlayerLeftGame(string username, string reason)
        {
            PlayerList.Remove(username);
        }

        public void TurnFinished(string currentTurn)
        {
            CurrentTurn = currentTurn;
        }

        public void UpdateTableCards()
        {
            //Get cards from CardManagerClient
        }

        public static bool IsGameActive { get; set; } = true;
        public static int PartyCode { get; set; }
        public static string CurrentTurn { get; set; }
        public static List<string> PlayerList { get; set; }
    }
}
