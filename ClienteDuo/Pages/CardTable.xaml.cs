using ClienteDuo.DataService;
using ClienteDuo.Utilities;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using Label = System.Windows.Controls.Label;

namespace ClienteDuo.Pages
{
    public partial class CardTable : Page, DataService.IMatchManagerCallback
    {
        private bool _hasDrawnCard;
        private DataService.Card[] _tableCards = new DataService.Card[3];
        private GameMenu _gameMenu;
        private PlayerIcon[] _playerIcons = new PlayerIcon[3];
        private List<Card> _selectedCards = new List<Card>();
        private Label[] _cardLabels = new Label[3];
        private Rectangle[] _cardColors = new Rectangle[3];
        private int _matchingColors;

        public CardTable()
        {
            InitializeComponent();

            InitializeAttributes();
            LoadSettingsMenu();

            for (int i = 0; i < 5; i++)
            {
                DealPlayerCard();
            }

            _hasDrawnCard = false;
        }

        private void InitializeAttributes()
        {
            _cardLabels[0] = LblLeftCard;
            _cardLabels[1] = LblMiddleCard;
            _cardLabels[2] = LblRightCard;

            _cardColors[0] = LeftCardColor;
            _cardColors[1] = MiddleCardColor;
            _cardColors[2] = RightCardColor;

            _tableCards[0] = new DataService.Card();
            _tableCards[1] = new DataService.Card();
            _tableCards[2] = new DataService.Card();

            _playerIcons[0] = TopPlayerIcon;
            _playerIcons[1] = LeftPlayerIcon;
            _playerIcons[2] = RightPlayerIcon;
        }

        private void LoadSettingsMenu()
        {
            try
            {
                InstanceContext instanceContext = new InstanceContext(this);
                MatchManagerClient client = new MatchManagerClient(instanceContext);

                _gameMenu = new GameMenu();
                _gameMenu.setClient(client);
                _gameMenu.Margin = new Thickness(550, 0, 0, 0);
                _gameMenu.Visibility = Visibility.Collapsed;

                TableBackground.Children.Add(_gameMenu);
            }
            catch (CommunicationException)
            {
                Launcher launcher = new Launcher();
                Application.Current.MainWindow.Content = launcher;
                MainWindow.ShowMessageBox(Properties.Resources.DlgServiceException, MessageBoxImage.Exclamation);
            }
        }

        public void LoadPlayers()
        {
            try
            {
                InstanceContext instanceContext = new InstanceContext(this);
                MatchManagerClient client = new MatchManagerClient(instanceContext);
                List<string> matchPlayers = new List<string>(client.GetPlayerList(SessionDetails.PartyCode));
                LblCurrentTurn.Content = client.GetCurrentTurn(SessionDetails.PartyCode);

                List<string> otherPlayers = new List<string>();
                foreach (string player in matchPlayers)
                {
                    if (!player.Equals(SessionDetails.Username))
                    {
                        otherPlayers.Add(player);
                    }
                }

                for (int i = 0; i < otherPlayers.Count; i++)
                {
                    _playerIcons[i].Username = otherPlayers[i];
                    _playerIcons[i].Visibility = Visibility.Visible;

                    if (!_playerIcons[i].Username.Contains("guest"))
                    {
                        UserDTO userDTO = new UsersManagerClient().GetUserInfoByUsername(_playerIcons[i].Username);
                        _playerIcons[i].SetProfilePicture(userDTO.PictureID);
                    }
                }

                _gameMenu.LoadPlayers(otherPlayers, client);
            }
            catch (CommunicationException)
            {
                Launcher launcher = new Launcher();
                Application.Current.MainWindow.Content = launcher;
                MainWindow.ShowMessageBox(Properties.Resources.DlgServiceException, MessageBoxImage.Exclamation);
            }
        }

        public bool SelectCard(Card selectedCard)
        {
            bool result = false;

            if (_selectedCards.Count < 2)
            {
                _selectedCards.Add(selectedCard);
                result = true;
            }

            return result;
        }

        public void UnselectCard(Card unselectedCard)
        {
            _selectedCards.Remove(unselectedCard);
        }

        public void PlayerLeftGame(string username, string reason)
        {
            MusicManager.PlayPlayerLeftSound();

            if (username.Equals(SessionDetails.Username))
            {
                if (SessionDetails.IsGuest)
                {
                    Launcher launcher = new Launcher();
                    Application.Current.MainWindow.Content = launcher;
                }
                else
                {
                    MainMenu mainMenu = new MainMenu();
                    Application.Current.MainWindow.Content = mainMenu;
                }

                string message = Properties.Resources.DlgKickedPlayer + Properties.Resources.DlgKickingReason + reason;
                MainWindow.ShowMessageBox(message, MessageBoxImage.Exclamation);
            }
            else
            {
                _gameMenu.RemovePlayer(username);

                foreach (PlayerIcon playerIcon in _playerIcons)
                {
                    if (playerIcon.Username.Equals(username))
                    {
                        playerIcon.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public void UpdateTableCards()
        {
            try
            {
                CardManagerClient client = new CardManagerClient();
                _tableCards = client.GetCards(SessionDetails.PartyCode);

                for (int i = 0; i < _tableCards.Length; i++)
                {
                    if (_tableCards[i].Number != "")
                    {
                        _cardLabels[i].Content = _tableCards[i].Number;
                        _cardColors[i].Fill = (SolidColorBrush)new BrushConverter().ConvertFrom(_tableCards[i].Color);
                    }
                    else
                    {
                        _cardLabels[i].Content = "";
                        _cardColors[i].Fill = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF969696");
                    }
                }
            }
            catch (CommunicationException)
            {
                Launcher launcher = new Launcher();
                Application.Current.MainWindow.Content = launcher;
                MainWindow.ShowMessageBox(Properties.Resources.DlgServiceException, MessageBoxImage.Exclamation);
            }
        }

        public void TurnFinished(string currentTurn)
        {
            LblCurrentTurn.Content = currentTurn;

            UpdateTableCards();
        }

        public async void GameOver()
        {
            BtnEndTurn.Visibility = Visibility.Collapsed;
            _gameMenu.Visibility = Visibility.Collapsed;
            BtnShowGameMenu.Visibility = Visibility.Collapsed;

            GameOver gameOverScreen = new GameOver();
            InstanceContext instanceContext = new InstanceContext(this);
            MatchManagerClient client = new MatchManagerClient(instanceContext);
            client.SetGameScore(SessionDetails.PartyCode, SessionDetails.Username, PlayerDeck.Children.Count);

            MatchOverLabel matchOverLabel = new MatchOverLabel();
            matchOverLabel.Visibility = Visibility.Visible;

            TableBackground.Children.Add(matchOverLabel);
            MusicManager.PlayMatchFinishedSound();

            await Task.Delay(5000);

            Dictionary<string, int> playerScores = client.GetMatchResults(SessionDetails.PartyCode);
            gameOverScreen.LoadPlayers(playerScores);

            gameOverScreen.Visibility = Visibility.Visible;
            TableBackground.Children.Add(gameOverScreen);
        }

        private void DealPlayerCard()
        {
            try
            {
                Card card = new Card();
                CardManagerClient client = new CardManagerClient();
                DataService.Card dealtCard = client.DrawCard();
                _hasDrawnCard = true;

                card.Number = dealtCard.Number;
                card.Color = dealtCard.Color;
                card.Visibility = Visibility.Visible;
                card.GameTable = this;

                PlayerDeck.Children.Add(card);
            }
            catch (CommunicationException)
            {
                Launcher launcher = new Launcher();
                Application.Current.MainWindow.Content = launcher;
                MainWindow.ShowMessageBox(Properties.Resources.DlgServiceException, MessageBoxImage.Exclamation);
            }
        }

        private void BtnShowGameMenuEvent(object sender, RoutedEventArgs e)
        {
            _gameMenu.Visibility = Visibility.Visible;
        }

        private void BtnDeckEvent(object sender, RoutedEventArgs e)
        {
            if (LblCurrentTurn.Content.Equals(SessionDetails.Username))
            {
                DealPlayerCard();
            }
        }

        private bool isValidMove(int position)
        {
            bool result = false;
            int selectionSum = 0;

            if (LblCurrentTurn.Content.Equals(SessionDetails.Username))
            {
                for (int i = 0; i < _selectedCards.Count; i++)
                {
                    if (_selectedCards[i].Color.Equals("#000000") || _selectedCards[i].Color.Equals(_tableCards[position].Color))
                    {
                        _matchingColors++;
                    }
                    else
                    {
                        _matchingColors = -1;
                    }

                    if (_selectedCards[i].Number.Equals("#") && !_tableCards[position].Number.Equals(""))
                    {
                        result = true;
                    }
                    else
                    {
                        selectionSum += int.Parse(_selectedCards[i].Number);
                    }
                }

                if (_tableCards[position].Number == selectionSum.ToString())
                {
                    result = true;
                }
            }

            return result;
        }

        private void PlayCard(int position)
        {
            try
            {
                DataService.CardManagerClient client = new DataService.CardManagerClient();
                DataService.Card playedCard = new DataService.Card();
                playedCard.Number = _selectedCards[0].Number;
                playedCard.Color = _selectedCards[0].Color;

                client.PlayCard(SessionDetails.PartyCode, position, playedCard);
                MusicManager.PlayCardFlippedSound();

                for (int i = 0; i < _selectedCards.Count; i++)
                {
                    _tableCards[position].Number = "PLAYED CARD";
                    _cardLabels[position].Content = _selectedCards[i].Number;
                    _cardColors[position].Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom(_selectedCards[i].Color));

                    PlayerDeck.Children.Remove(_selectedCards[i]);
                }

                _selectedCards.Clear();

                if (PlayerDeck.Children.Count == 0)
                {
                    InstanceContext context = new InstanceContext(this);
                    MatchManagerClient matchClient = new MatchManagerClient(context);

                    matchClient.EndGame(SessionDetails.PartyCode);
                }
                else
                {
                    BtnEndTurn.Visibility = Visibility.Visible;
                }
            }
            catch (CommunicationException)
            {
                Launcher launcher = new Launcher();
                Application.Current.MainWindow.Content = launcher;
                MainWindow.ShowMessageBox(Properties.Resources.DlgServiceException, MessageBoxImage.Exclamation);
            }
        }

        private void PlayCardLeft(object sender, RoutedEventArgs e)
        {
            if (isValidMove(0))
            {
                PlayCard(0);
            }
        }

        private void PlayCardMiddle(object sender, RoutedEventArgs e)
        {
            if (isValidMove(1))
            {
                PlayCard(1);
            }
        }

        private void PlayCardRight(object sender, RoutedEventArgs e)
        {
            if (_tableCards[2].Number.Equals("") && _selectedCards.Count == 1)
            {
                if (_matchingColors >= 1 || _hasDrawnCard)
                {
                    PlayCard(2);
                }
            }
            else
            {
                if (isValidMove(2))
                {
                    PlayCard(2);
                }
            }
        }

        private void BtnEndTurnEvent(object sender, RoutedEventArgs e)
        {
            try
            {
                InstanceContext instanceContext = new InstanceContext(this);
                MatchManagerClient client = new MatchManagerClient(instanceContext);
                BtnEndTurn.Visibility = Visibility.Collapsed;
                _matchingColors = 0;

                CardManagerClient cardClient = new CardManagerClient();
                cardClient.DealCards(SessionDetails.PartyCode);

                client.EndTurn(SessionDetails.PartyCode);
            }
            catch (CommunicationException)
            {
                Launcher launcher = new Launcher();
                Application.Current.MainWindow.Content = launcher;
                MainWindow.ShowMessageBox(Properties.Resources.DlgServiceException, MessageBoxImage.Exclamation);
            }
        }
    }
}