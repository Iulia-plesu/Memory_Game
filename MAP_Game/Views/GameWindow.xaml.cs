using System.Windows;

namespace MAP_Game.View
{
    public partial class GameWindow : Window
    {
        public GameWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel.GameViewModel();
        }
    }
}
