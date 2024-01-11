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
        string _player1 = "Xavi";
        string _player2 = "Jorge";

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
            _client.Subscribe(_partyCode, _player1);
            _gameTable.PartyCode = _partyCode;

            MatchPlayerManagerClient playerManagerClient = new MatchPlayerManagerClient();
            _gameTable.PlayerList = new List<string>(playerManagerClient.GetPlayerList(_partyCode));
        }

        [TestCleanup]
        public void Cleanup()
        {
            _client.EndGame(_partyCode);
        }

        [TestMethod()]
        public void SubscribeTest()
        {
            _client.Subscribe(_partyCode, _player2);
            MatchPlayerManagerClient matchPlayerManagerClient = new MatchPlayerManagerClient();
            List<string> playerList = new List<string>(matchPlayerManagerClient.GetPlayerList(_partyCode));

            Assert.IsTrue(playerList.Contains<string>(_player2));
        }

        [TestMethod()]
        public void EndTurnTest()
        {
            TestGameTable table = new TestGameTable();
            InstanceContext context = new InstanceContext(_gameTable);
            MatchManagerClient client = new MatchManagerClient(_context);
            client.Subscribe(_partyCode, _player2);
            string currentTurn = _client.GetCurrentTurn(_partyCode);

            _client.EndTurn(_partyCode);

            Assert.AreNotEqual(currentTurn, table.CurrentTurn);
        }

        [TestMethod()]
        public void GetCurrentTurnTest()
        {
            MatchPlayerManagerClient matchPlayerManagerClient = new MatchPlayerManagerClient();
            List<string> playerList = new List<string>(matchPlayerManagerClient.GetPlayerList(_partyCode));
            string currentTurn = _client.GetCurrentTurn(_partyCode);

            Assert.IsTrue(playerList.Contains(currentTurn));
        }

        [TestMethod()]
        public void SetGameScoreTest()
        {
            MatchPlayerManagerClient matchPlayerManagerClient = new MatchPlayerManagerClient();
            List<string> playerList = new List<string>(matchPlayerManagerClient.GetPlayerList(_partyCode));

            foreach (string player in playerList)
            {
                _client.SetGameScore(_partyCode, player, 0);
            }

            Dictionary<string, int> matchResults = _client.GetMatchResults(_partyCode);
            Assert.IsTrue(matchResults.Count == playerList.Count);
        }

        [TestMethod()]
        public void GetMatchResultsTest()
        {
            MatchPlayerManagerClient matchPlayerManagerClient = new MatchPlayerManagerClient();
            List<string> playerList = new List<string>(matchPlayerManagerClient.GetPlayerList(_partyCode));

            foreach (string player in playerList)
            {
                _client.SetGameScore(_partyCode, player, 0);
            }

            Dictionary<string, int> matchResults = _client.GetMatchResults(_partyCode);
            Assert.AreEqual(playerList.Count, matchResults.Count);
        }
    }
}
