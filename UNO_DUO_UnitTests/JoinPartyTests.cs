using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClienteDuo.DataService;
using ClienteDuo.Utilities;
using ClienteDuo.TestClasses;
using System.Threading;
using System.Threading.Tasks;

namespace ClienteDuo.Pages.Tests
{
    [TestClass()]
    public class JoinPartyTests
    {
        readonly int _partyCode = 1234;
        readonly string _hostUsername = "camilo";

        [TestInitialize]
        public async Task Init()
        {
            TestPartyManager testPartyManager = new TestPartyManager();
            testPartyManager.NotifyCreateParty(_partyCode, _hostUsername);
            await Task.Delay(1500);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            TestPartyManager testPartyManager = new TestPartyManager();
            testPartyManager.NotifyCloseParty(_partyCode, _hostUsername, "");
            await Task.Delay(1500);
        }

        [TestMethod()]
        public void InputIsNotIntegerTest()
        {
            JoinParty joinParty = new JoinParty();
            Assert.IsFalse(joinParty.IsInputInteger("inputString"));
        }

        [TestMethod()]
        public void InputIsIntegerTest()
        {
            JoinParty joinParty = new JoinParty();
            Assert.IsTrue(joinParty.IsInputInteger("1232"));
        }

        [TestMethod()]
        public void InputIsIntegerEmptyTest()
        {
            JoinParty joinParty = new JoinParty();
            Assert.IsFalse(joinParty.IsInputInteger(""));
        }

        [TestMethod()]
        public void InputIsIntegerSymbolsTest()
        {
            JoinParty joinParty = new JoinParty();
            Assert.IsFalse(joinParty.IsInputInteger("-!·2"));
        }

        [TestMethod()]
        public void IsPartyCodeExistentTest()
        {
            JoinParty joinParty = new JoinParty();
            Assert.IsTrue(joinParty.IsLobbyCodeExistent(_partyCode));
        }

        [TestMethod()]
        public void IsPartyCodeNotExistentTest()
        {
            JoinParty joinParty = new JoinParty();
            int nonExistentPartyCode = _partyCode - 1;
            Assert.IsFalse(joinParty.IsLobbyCodeExistent(nonExistentPartyCode));
        }

        [TestMethod()]
        public void IsPartySpaceAvailableTest()
        {
            JoinParty joinParty = new JoinParty();
            Assert.IsTrue(joinParty.IsSpaceAvailable(_partyCode));
        }

        [TestMethod()]
        public void IsPartyFullTest()
        {
            JoinParty joinParty = new JoinParty();
            string player1 = "pepe1109";
            string player2 = "jorgejuan22";
            string player3 = "towngameplay";

            TestPartyManager testPartyManager = new TestPartyManager();
            testPartyManager.NotifyJoinParty(_partyCode, player1);
            testPartyManager.NotifyJoinParty(_partyCode, player2);
            testPartyManager.NotifyJoinParty(_partyCode, player3);

            Assert.IsFalse(joinParty.IsSpaceAvailable(_partyCode));
        }

        [TestMethod()]
        public async Task IsUserBlockedByPlayerInLobbyTrueTest()
        {
            JoinParty joinParty = new JoinParty();
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            string player0Username = "pepe0142";
            string player1Username = "pepe1109";
            UsersManager.AddUserToDatabase(player0Username, "host@gmail.com", "Tokyo11!23");
            UsersManager.AddUserToDatabase(player1Username, "player1mail@gmail.com", "Tokyo11!23");

            TestPartyManager testPartyManager = new TestPartyManager();
            testPartyManager.NotifyJoinParty(_partyCode, player0Username);
            await Task.Delay(1500);

            BlockManager.BlockUserByUsername(player0Username, player1Username);
            bool result = joinParty.IsUserBlockedByPlayerInLobby(player1Username, _partyCode);

            usersManagerClient.DeleteUserFromDatabaseByUsername(player0Username);
            usersManagerClient.DeleteUserFromDatabaseByUsername(player1Username);

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void IsUserBlockedByPlayerInLobbyFalseTest()
        {
            JoinParty joinParty = new JoinParty();
            bool result = joinParty.IsUserBlockedByPlayerInLobby("cardone", _partyCode);

            Assert.IsFalse(result);
        }

        [TestMethod()]
        public void IsUserBlockedByPlayerInLobbyNotExistsTest()
        {
            JoinParty joinParty = new JoinParty();
            bool result = joinParty.IsUserBlockedByPlayerInLobby("johnappleseed", _partyCode);

            Assert.IsFalse(result);
        }
    }
}