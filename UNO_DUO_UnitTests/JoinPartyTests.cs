using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClienteDuo.DataService;
using ClienteDuo.Utilities;
using ClienteDuo.TestClasses;
using System.Threading;

namespace ClienteDuo.Pages.Tests
{
    [TestClass()]
    public class JoinPartyTests
    {
        readonly int _partyCode = 1234;
        readonly string _hostUsername = "camilo";

        [TestInitialize]
        public void Init()
        {
            TestPartyManager testPartyManager = new TestPartyManager();
            testPartyManager.NotifyCreateParty(_partyCode, _hostUsername);
            Thread.Sleep(2000);
        }

        [TestCleanup]
        public void Cleanup()
        {
            TestPartyManager testPartyManager = new TestPartyManager();
            testPartyManager.NotifyCloseParty(_partyCode, _hostUsername, "");
            Thread.Sleep(2000);
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
        public void IsPartyCodeExistentTest()
        {
            JoinParty joinParty = new JoinParty();
            Assert.IsTrue(joinParty.IsPartyCodeExistent(_partyCode));
        }

        [TestMethod()]
        public void IsPartyCodeNotExistentTest()
        {
            JoinParty joinParty = new JoinParty();
            int nonExistentPartyCode = _partyCode - 1;
            Assert.IsFalse(joinParty.IsPartyCodeExistent(nonExistentPartyCode));
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
        public void IsUserBlockedByPlayerInLobbyTrueTest()
        {
            UsersManagerClient usersManagerClient = new UsersManagerClient();
            string player0Username = "pepe0142";
            string player1Username = "pepe1109";
            UsersManager.AddUserToDatabase(player0Username, "host@gmail.com", "Tokyo11!23");
            UsersManager.AddUserToDatabase(player1Username, "player1mail@gmail.com", "Tokyo11!23");

            TestPartyManager testPartyManager = new TestPartyManager();
            testPartyManager.NotifyJoinParty(_partyCode, player0Username);
            Thread.Sleep(2000);

            usersManagerClient.BlockUserByUsername(player0Username, player1Username);
            JoinParty joinParty = new JoinParty();
            bool result = joinParty.IsUserBlockedByPlayerInParty(player1Username, _partyCode);

            usersManagerClient.DeleteUserFromDatabaseByUsername(player0Username);
            usersManagerClient.DeleteUserFromDatabaseByUsername(player1Username);

            Assert.IsTrue(result);
        }

        [TestMethod()]
        public void IsUserBlockedByPlayerInLobbyFalseTest()
        {
            JoinParty joinParty = new JoinParty();
            bool result = joinParty.IsUserBlockedByPlayerInParty("cardone", _partyCode);

            Assert.IsFalse(result);
        }
    }
}