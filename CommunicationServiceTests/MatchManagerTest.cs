using System.Collections.Concurrent;

namespace CommunicationServiceTests
{
    [TestClass]
    public class MatchManagerTest
    {
        [ClassInitialize]
        public static void ClassInitialize()
        {
            
        }

        [ClassCleanup()]
        public static void ClassCleanup() { 
            
        }

        [TestMethod]
        public void SubscribeTest(int partyCode, string username)
        {

        }

        [TestMethod]
        public void SetGameScoreTest(int partyCode, string username, int cardCount)
        {

        }

        [TestMethod]
        public void ExitMatchTest(int partyCode, string username)
        {

        }

        [TestMethod]
        public void KickPlayerFromGameTest(int partyCode, string username, string reason)
        {

        }

        [TestMethod]
        public void EndGame(int partyCode)
        {

        }

        [TestMethod]
        public void EndTurn(int partyCode)
        {

        }

        [TestMethod]
        public void GetCurrentTurnTest(int partyCode)
        {
            
        }

        [TestMethod]
        public void GetPlayerListTest(int partyCode)
        {

        }

        [TestMethod]
        public void GetMatchResultsTest(int partyCode)
        {

        }
    }

    public partial class TestGameTable
    {

    }
}