using MAP_Game.ViewModel;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Linq;

namespace MAP_Game.View
{
    public partial class GameWindow : Window
    {
        private string _selectedCategory;
        private DispatcherTimer _timer;
        private int _remainingTime;

        private readonly string _currentUsername;
        private readonly LoginViewModel _loginViewModel;
        private readonly int _originalTimeLimit;

        private readonly Dictionary<string, string> CategoryPaths = new()
        {
            { "Mediterranean", @"Categories\Mediterranean" },
            { "Antarctica", @"Categories\Antarctica" },
            { "Rustic", @"Categories\Rustic" }
        };

        public GameWindow(string username, LoginViewModel loginViewModel,
                          string selectedCategory = null, int timeLimitInSeconds = 0,
                          int rows = 4, int columns = 4,
                          int restoredMatchedPairs = 0, List<string> restoredMatchedImagePaths = null)
        {
            InitializeComponent();

            _currentUsername = username;
            _loginViewModel = loginViewModel;
            _originalTimeLimit = timeLimitInSeconds;

            _selectedCategory = string.IsNullOrWhiteSpace(selectedCategory) ? "Rustic" : selectedCategory;
            _remainingTime = timeLimitInSeconds > 0 ? timeLimitInSeconds : 60;

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();

            var images = LoadCategoryImages();

            if (images.Count < 8)
            {
                MessageBox.Show($"Need at least 8 images, found {images.Count}. Using test data.");
                images = Enumerable.Range(1, 8)
                    .Select(i => new Uri(Path.Combine(Environment.CurrentDirectory, $"Assets/TestImage{i}.png")).AbsoluteUri)
                    .ToList();
            }

            double screenWidth = SystemParameters.WorkArea.Width * 0.95;
            double screenHeight = SystemParameters.WorkArea.Height * 0.90;

            double maxCardWidth = (screenWidth - 100) / columns;
            double maxCardHeight = (screenHeight - 150) / rows;

            double cardWidth = Math.Min(maxCardWidth, maxCardHeight * 2 / 3);
            double cardHeight = cardWidth * 1.5;

            Width = cardWidth * columns + 100;
            Height = cardHeight * rows + 150;

            var viewModel = new GameViewModel(_selectedCategory, images)
            {
                Rows = rows,
                Columns = columns,
                CardWidth = cardWidth,
                CardHeight = cardHeight
            };

            viewModel.OnGameWon = () =>
            {
                Dispatcher.Invoke(() =>
                {
                    _timer.Stop();
                    viewModel.EndGame();
                    MessageBox.Show("🎉 Congratulations! You matched all the cards and won the game!", "You Win", MessageBoxButton.OK, MessageBoxImage.Information);

                    int timeSpent = timeLimitInSeconds - _remainingTime;
                    _loginViewModel.UpdateStats(_currentUsername, isWin: true, timeInSeconds: timeSpent);
                });
            };

            DataContext = viewModel;

            if (restoredMatchedPairs > 0 && restoredMatchedImagePaths != null)
            {
                viewModel.RestoreMatchedPairs(restoredMatchedPairs, restoredMatchedImagePaths);
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var minutes = _remainingTime / 60;
            var seconds = _remainingTime % 60;
            TimerTextBlock.Text = $"{minutes:D2}:{seconds:D2}";

            if (_remainingTime == 0)
            {
                _timer.Stop();
                ((GameViewModel)DataContext).EndGame();
                MessageBox.Show("Time's up! The game is over.");

                int timeSpent = _originalTimeLimit - _remainingTime;
                _loginViewModel.UpdateStats(_currentUsername, isWin: false, timeInSeconds: timeSpent);
            }
            else
            {
                _remainingTime--;
            }
        }

        private List<string> LoadCategoryImages()
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string imagesFolder = Path.Combine(baseDir, CategoryPaths[_selectedCategory]);
                imagesFolder = Path.GetFullPath(imagesFolder);

                var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

                if (!Directory.Exists(imagesFolder))
                {
                    MessageBox.Show($"Directory not found: {imagesFolder}");
                    return new List<string>();
                }

                var images = Directory.GetFiles(imagesFolder)
                    .Where(file => imageExtensions.Contains(Path.GetExtension(file).ToLower()))
                    .Select(file => new Uri(file).AbsoluteUri)
                    .ToList();

                Console.WriteLine($"Found {images.Count} images in {imagesFolder}");
                return images;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading images: {ex.Message}");
                return new List<string>();
            }
        }

        private void LeaveButton_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = (GameViewModel)DataContext;

            var matchedImagePaths = viewModel.Tokens
                .Where(t => t.IsMatched)
                .Select(t => t.ImagePath)
                .Distinct()
                .ToList();

            _loginViewModel.UpdateLastGameStats(
                _currentUsername,
                _selectedCategory,
                viewModel.Rows,
                viewModel.Columns,
                _remainingTime,
                viewModel.MatchedPairsCount,
                matchedImagePaths
            );

            this.Close();
        }
    }
}
