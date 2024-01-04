using ClienteDuo.DataService;
using ClienteDuo.TestClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace ClienteDuo.Pages.Tests
{
    [TestClass()]
    public class CardManagerTests
    {
        int _partyCode;
        string player = "Xavi";
        Random _numberGenerator = new Random();
        TestGameTable _gameTable = new TestGameTable();
        InstanceContext _context;
        MatchManagerClient _matchClient;
        CardManagerClient _cardClient;

        [TestInitialize]
        public void Init()
        {
            _context = new InstanceContext(_gameTable);
            _matchClient = new MatchManagerClient(_context);
            _partyCode = _numberGenerator.Next(0, 10000);
            _matchClient.Subscribe(_partyCode, player);
            TestGameTable.PartyCode = _partyCode;

            _cardClient = new CardManagerClient();
            _cardClient.DealCards(_partyCode);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _matchClient.EndGame(_partyCode);
        }

        [TestMethod]
        public void ValidCardNumberTest()
        {
            DataService.Card card = _cardClient.DrawCard();

            while (card.Number.Equals("#"))
            {
                card = _cardClient.DrawCard();
            }

            Assert.IsTrue(int.Parse(card.Number) > 0);
        }

        [TestMethod]
        public void DrawWildcardTest()
        {
            DataService.Card card = _cardClient.DrawCard();

            while (!card.Number.Equals("#"))
            {
                card = _cardClient.DrawCard();
            }

            Assert.AreEqual(card.Number, "#");
        }

        [TestMethod]
        public void GetCards()
        {
            List<DataService.Card> cardList = new List<DataService.Card>(_cardClient.GetCards(_partyCode));

            Assert.IsTrue(cardList.Count == 3); // The table must always have 3 cards
        }

        [TestMethod]
        public void PlayCard()
        {
            DataService.Card oldCard = _cardClient.GetCards(_partyCode)[0]; // Can also be the card in position '1' of the array
            DataService.Card playingCard = new DataService.Card();

            playingCard.Color = oldCard.Color;
            playingCard.Number = oldCard.Number;
            _cardClient.PlayCard(_partyCode, 0, playingCard);
            DataService.Card newCard = _cardClient.GetCards(_partyCode)[0];

            Assert.AreNotEqual(oldCard, newCard); //Cards played are reshuffled to allow other numbers to be played
        }
    }
}
