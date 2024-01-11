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
    public class MatchPlayerManagerTests
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
        public void KickPlayerFromGameTest()
        {   
            TestGameTable table = new TestGameTable();
            InstanceContext context = new InstanceContext(_gameTable);
            MatchManagerClient client = new MatchManagerClient(_context);
            client.Subscribe(_partyCode, _player2);

            MatchPlayerManagerClient matchPlayerManagerClient = new MatchPlayerManagerClient();
            table.PlayerList = new List<string>(matchPlayerManagerClient.GetPlayerList(_partyCode));

            matchPlayerManagerClient.KickPlayerFromGame(_partyCode, _player2, "Testing Kick player Method");

            List<string> playerList = new List<string>(matchPlayerManagerClient.GetPlayerList(_partyCode));
            Assert.IsFalse(playerList.Contains<string>(_player2));
        }

        [TestMethod()]
        public void KickPlayerFromGame_IsPlayerTurnTest()
        {
            TestGameTable table = new TestGameTable();
            InstanceContext context = new InstanceContext(_gameTable);
            MatchManagerClient client = new MatchManagerClient(_context);
            client.Subscribe(_partyCode, _player2);

            MatchPlayerManagerClient playerManagerClient = new MatchPlayerManagerClient();
            table.PlayerList = new List<string>(playerManagerClient.GetPlayerList(_partyCode));
            table.CurrentTurn = _client.GetCurrentTurn(_partyCode);

            while (!table.CurrentTurn.Equals(_player1))
            {
                client.EndTurn(_partyCode);
            }

            playerManagerClient.KickPlayerFromGame(_partyCode, _player1, "Kicking the current turn player");

            List<string> playerList = new List<string>(playerManagerClient.GetPlayerList(_partyCode));
            Assert.IsFalse(playerList.Contains<string>(_player1));
        }

        [TestMethod()]
        public void ExitMatchTest()
        {
            TestGameTable table = new TestGameTable();
            InstanceContext context = new InstanceContext(_gameTable);
            MatchManagerClient matchClient = new MatchManagerClient(_context);
            matchClient.Subscribe(_partyCode, _player2);

            MatchPlayerManagerClient playerManagerClient = new MatchPlayerManagerClient();
            table.PlayerList = new List<string>(playerManagerClient.GetPlayerList(_partyCode));

            playerManagerClient.ExitMatch(_partyCode, _player1);

            List<string> playerList = new List<string>(playerManagerClient.GetPlayerList(_partyCode));
            Assert.IsFalse(playerList.Contains<string>(_player1));
        }

        [TestMethod()]
        public void ExitMatch_IsPlayerTurnTest()
        {
            TestGameTable table = new TestGameTable();
            InstanceContext context = new InstanceContext(_gameTable);
            MatchManagerClient client = new MatchManagerClient(_context);
            client.Subscribe(_partyCode, _player2);

            MatchPlayerManagerClient matchPlayerManagerClient = new MatchPlayerManagerClient();
            table.PlayerList = new List<string>(matchPlayerManagerClient.GetPlayerList(_partyCode));

            while (!client.GetCurrentTurn(_partyCode).Equals(_player2))
            {
                client.EndTurn(_partyCode);
            }

            matchPlayerManagerClient.ExitMatch(_partyCode, _player2);

            List<string> playerList = new List<string>(matchPlayerManagerClient.GetPlayerList(_partyCode));
            Assert.IsFalse(playerList.Contains<string>(_player2));
        }
    }
}
