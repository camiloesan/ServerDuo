﻿using ClienteDuo.DataService;
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
            _client.Subscribe(_partyCode, _player2);
            List<string> playerList = new List<string>(_client.GetPlayerList(_partyCode));

            Assert.IsTrue(playerList.Contains<string>(_player2));
        }

        [TestMethod()]
        public void KickPlayerFromGameTest()
        {   
            TestGameTable table = new TestGameTable();
            InstanceContext context = new InstanceContext(_gameTable);
            MatchManagerClient client = new MatchManagerClient(_context);
            client.Subscribe(_partyCode, _player2);
            TestGameTable.PlayerList = new List<string>(_client.GetPlayerList(_partyCode));

            _client.KickPlayerFromGame(_partyCode, _player2, "Testing Kick player Method");

            List<string> playerList = new List<string>(_client.GetPlayerList(_partyCode));
            Assert.IsFalse(playerList.Contains<string>(_player2));
        }

        [TestMethod()]
        public void KickPlayerFromGame_IsPlayerTurnTest()
        {
            TestGameTable table = new TestGameTable();
            InstanceContext context = new InstanceContext(_gameTable);
            MatchManagerClient client = new MatchManagerClient(_context);
            client.Subscribe(_partyCode, _player2);
            TestGameTable.PlayerList = new List<string>(_client.GetPlayerList(_partyCode));
            TestGameTable.CurrentTurn = _client.GetCurrentTurn(_partyCode);

            while (TestGameTable.CurrentTurn.Equals(_player2))
            {
                client.EndTurn(_partyCode);
            }

            _client.KickPlayerFromGame(_partyCode, _player1, "Kicking the current turn player");

            List<string> playerList = new List<string>(_client.GetPlayerList(_partyCode));
            Assert.IsFalse(playerList.Contains<string>(_player1));
        }

        [TestMethod()]
        public void ExitMatchTest()
        {
            TestGameTable table = new TestGameTable();
            InstanceContext context = new InstanceContext(_gameTable);
            MatchManagerClient client = new MatchManagerClient(_context);
            client.Subscribe(_partyCode, _player2);
            TestGameTable.PlayerList = new List<string>(_client.GetPlayerList(_partyCode));

            _client.ExitMatch(_partyCode, _player1);

            List<string> playerList = new List<string>(_client.GetPlayerList(_partyCode));
            Assert.IsFalse(playerList.Contains<string>(_player1));
        }

        [TestMethod()]
        public void ExitMatch_IsPlayerTurnTest()
        {
            TestGameTable table = new TestGameTable();
            InstanceContext context = new InstanceContext(_gameTable);
            MatchManagerClient client = new MatchManagerClient(_context);
            
            client.Subscribe(_partyCode, _player2);
            TestGameTable.PlayerList = new List<string>(_client.GetPlayerList(_partyCode));

            while (!client.GetCurrentTurn(_partyCode).Equals(_player2))
            {
                client.EndTurn(_partyCode);
            }

            _client.ExitMatch(_partyCode, _player2);

            List<string> playerList = new List<string>(_client.GetPlayerList(_partyCode));
            Assert.IsFalse(playerList.Contains<string>(_player2));
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
        public void SetGameScoreTest()
        {
            List<string> playerList = new List<string>(_client.GetPlayerList(_partyCode));

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
