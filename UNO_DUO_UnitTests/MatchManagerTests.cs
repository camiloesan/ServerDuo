using ClienteDuo.DataService;
using ClienteDuo.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ClienteDuo.Pages.Tests
{
    [TestClass()]
    public class MatchManagerTests
    {
        int _partyCode;
        string player = "Xavi";
        Random _numberGenerator = new Random();
        TestGameTable _gameTable = new TestGameTable();
        InstanceContext _context;
        MatchManagerClient _client;

        [TestInitialize]
        public void Init()
        {
            _context = new InstanceContext(_gameTable);
            _client = new MatchManagerClient(_context);
            _partyCode = _numberGenerator.Next(0, 10000);
            _client.Subscribe(_partyCode, player);
            TestGameTable.PartyCode = _partyCode;            
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client.EndGame(_partyCode);
        }

        [TestMethod()]
        public void SubscribeTest()
        {
            _client.Subscribe(_partyCode, "Jorge");
            List<string> playerList = new List<string>(_client.GetPlayerList(_partyCode));

            Assert.IsTrue(playerList.Contains<string>("Jorge"));
        }

        [TestMethod()]
        public void KickPlayerFromGameTest()
        {   
            TestGameTable table = new TestGameTable();
            InstanceContext context = new InstanceContext(_gameTable);
            MatchManagerClient client = new MatchManagerClient(_context);
            client.Subscribe(_partyCode, "Jorge");
            TestGameTable.PlayerList = new List<string>(_client.GetPlayerList(_partyCode));

            _client.KickPlayerFromGame(_partyCode, "Jorge", "Testing Kick player Method");

            List<string> playerList = new List<string>(_client.GetPlayerList(_partyCode));
            Assert.IsFalse(playerList.Contains<string>("Jorge"));
        }

        [TestMethod()]
        public void EndTurnTest()
        {
            TestGameTable table = new TestGameTable();
            InstanceContext context = new InstanceContext(_gameTable);
            MatchManagerClient client = new MatchManagerClient(_context);
            client.Subscribe(_partyCode, "Jorge");
            string currentTurn = _client.GetCurrentTurn(_partyCode);

            _client.EndTurn(_partyCode);

            Assert.AreNotEqual(currentTurn, TestGameTable.CurrentTurn);
        }

        [TestMethod()]
        public void GetCurrentTurnTest()
        {
            List<string> playerList = new List<string>(_client.GetPlayerList(_partyCode));
            string currentTurn = _client.GetCurrentTurn(_partyCode);

            Assert.IsTrue(playerList.Contains(currentTurn));
        }

        [TestMethod()]
        public void GetMatchResultsTest()
        {
            List<string> playerList = new List<string>(_client.GetPlayerList(_partyCode));

            foreach (string player in playerList)
            {
                _client.SetGameScore(_partyCode, player, 0);
            }

            Dictionary<string, int> matchResults = _client.GetMatchResults(_partyCode);
            Assert.AreEqual(playerList.Count, matchResults.Count);
        }
    }
}
