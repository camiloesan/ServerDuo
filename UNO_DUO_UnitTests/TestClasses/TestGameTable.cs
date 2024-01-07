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
            CardManagerClient client = new CardManagerClient();
            Cards = client.GetCards(PartyCode);
        }

        public bool IsGameActive { get; set; } = true;
        public int PartyCode { get; set; }
        public string CurrentTurn { get; set; }
        public List<string> PlayerList { get; set; } = new List<string>();
        public DataService.Card[] Cards { get; set; } = new DataService.Card[3];
    }
}
