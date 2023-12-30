using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClienteDuo.Pages
{
    public partial class Card : UserControl
    {
        bool _isSelected;
        string _number;
        string _color;

        public Card()
        {
            InitializeComponent();
            _number = "2";
            _color = "000000";
            _isSelected = false;
        }

        public string Color
        {
            set
            {
                _color = value;
                _colorRectangle.Fill = (SolidColorBrush)(new BrushConverter().ConvertFrom(_color));
            }
            get => _color;
        }
        public string Number
        {
            set
            {
                _number = value;
                _numberLabel.Content = _number.ToString();
            }
            get => _number;
        }

        public CardTable GameTable { set; get; }

        public void SelectCard(object sender, RoutedEventArgs e)
        {
            if (_isSelected)
            {
                _cardButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
                _isSelected = false;
                GameTable.UnselectCard(this);
            }
            else
            {
                if (GameTable.SelectCard(this))
                {
                    _cardButton.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFD700"));
                    _isSelected = true;
                }
            }
        }
    }
}
