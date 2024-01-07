using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace ClienteDuo.TestClasses.Tests
{
    [TestClass()]
    public class TestPartyManagerTests
    {
        readonly string _hostUsername = "camilo";

        [TestMethod()]
        public async Task PartyCreatedTest()
        {
            TestPartyManager testPartyManager = new TestPartyManager();
            int partyCode = 1110;

            testPartyManager.NotifyCreateParty(partyCode, _hostUsername);
            int expected = 1;
            await Task.Delay(1500);
            int result = TestPartyManager.PlayersInParty.Count;
            testPartyManager.NotifyCloseParty(partyCode, _hostUsername, "kick");
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public async Task MessageReceivedTest()
        {
            TestPartyManager testPartyManager = new TestPartyManager();
            int partyCode = 1113;
            string expectedMessage = "helloWorld";
            testPartyManager.NotifyCreateParty(partyCode, "camilo");
            testPartyManager.NotifySendMessage("helloWorld");

            await Task.Delay(1500);
            string result = TestPartyManager.ReceivedMessage;

            testPartyManager.NotifyCloseParty(partyCode, _hostUsername, "kick");

            Assert.AreEqual(expectedMessage, result);
        }

        [TestMethod()]
        public async Task PlayerJoinedTest()
        {
            TestPartyManager testPartyManager = new TestPartyManager();
            int partyCode = 1114;
            testPartyManager.NotifyCreateParty(partyCode, "camilo");
            testPartyManager.NotifyJoinParty(partyCode, "christian");

            await Task.Delay(1500);
            int result = TestPartyManager.PlayersInParty.Count;

            testPartyManager.NotifyCloseParty(partyCode, _hostUsername, "kick");

            int expected = 2;
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public async Task PlayerKickedTest()
        {
            TestPartyManager testPartyManager = new TestPartyManager();
            int partyCode = 1115;
            string playerToKick = "christian";
            testPartyManager.NotifyCreateParty(partyCode, "camilo");
            testPartyManager.NotifyJoinParty(partyCode, playerToKick);
            testPartyManager.NotifyKickPlayer(partyCode, playerToKick, "spam");

            await Task.Delay(1500);
            string result = TestPartyManager.PlayerKickedReason;
            testPartyManager.NotifyCloseParty(partyCode, _hostUsername, "kick");

            string expected = "spam";
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public async Task PlayerLeftTest()
        {
            TestPartyManager testPartyManager = new TestPartyManager();
            int partyCode = 1116;
            string playerToKick = "christian";
            testPartyManager.NotifyCreateParty(partyCode, "camilo");
            testPartyManager.NotifyJoinParty(partyCode, playerToKick);
            testPartyManager.NotifyKickPlayer(partyCode, playerToKick, "spam");

            await Task.Delay(1500);
            int result = TestPartyManager.PlayersInParty.Count;
            testPartyManager.NotifyCloseParty(partyCode, _hostUsername, "kick");

            int expected = 1;
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public async Task GameStartedTest()
        {
            TestPartyManager testPartyManager = new TestPartyManager();
            int partyCode = 1112;
            string playerToKick = "christian";
            testPartyManager.NotifyCreateParty(partyCode, "camilo");
            testPartyManager.NotifyJoinParty(partyCode, playerToKick);
            testPartyManager.NotifyStartGame(partyCode);

            await Task.Delay(1500);
            bool result = TestPartyManager.IsGameStarted;
            Assert.IsTrue(result);
        }

        [TestMethod()]
        public async Task GameStartedPartyDoesNotExistTest()
        {
            TestPartyManager testPartyManager = new TestPartyManager();
            int partyCode = 1112;
            testPartyManager.NotifyStartGame(partyCode);

            await Task.Delay(1500);
            bool result = TestPartyManager.IsGameStarted;
            Assert.IsFalse(result);
        }
    }
}