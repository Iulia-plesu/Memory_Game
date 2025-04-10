using MAP_Game.ViewModel;
using System.Windows;

namespace MAP_Game.View
{
    public partial class GameWindow : Window
    {
        public GameWindow(string username, LoginViewModel loginViewModel,
                         string selectedCategory = null, int timeLimitInSeconds = 0,
                         int rows = 4, int columns = 4,
                         int restoredMatchedPairs = 0, List<string> restoredMatchedImagePaths = null)
        {
            InitializeComponent();

            var viewModel = new GameViewModel(username, loginViewModel,
                                            selectedCategory, timeLimitInSeconds,
                                            rows, columns,
                                            restoredMatchedPairs, restoredMatchedImagePaths);

            DataContext = viewModel;

            Width = viewModel.CardWidth * columns + 100;
            Height = viewModel.CardHeight * rows + 150;
        }
    }
}