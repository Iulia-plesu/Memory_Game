using MAP_Game.ViewModel;
using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MAP_Game.View
{
    public partial class GameWindow : Window
    {
        private string _selectedCategory;
        private DispatcherTimer _timer;
        private int _remainingTime;

        private readonly Dictionary<string, string> CategoryPaths = new()
        {
            { "Category 1", @"C:/Users/Plesu/Desktop/WPF_Game/MAP_Game/Categories/Mediterranean" },
            { "Category 2", @"C:/Users/Plesu/Desktop/WPF_Game/MAP_Game/Categories/Antarctica" },
            { "Category 3", @"C:/Users/Plesu/Desktop/WPF_Game/MAP_Game/Categories/Rustic" }
        };

        public GameWindow(string selectedCategory = null, int timeLimitInSeconds = 0, int rows = 4, int columns = 4)
        {
            InitializeComponent();

            // Use default category if none provided
            _selectedCategory = string.IsNullOrWhiteSpace(selectedCategory) ? "Rustic" : selectedCategory;

            // Debugging output
            Console.WriteLine($"Selected Category: {_selectedCategory}");

            // Use default time if not provided or invalid
            _remainingTime = timeLimitInSeconds > 0 ? timeLimitInSeconds : 60;

            Console.WriteLine($"Game initialized with time limit: {_remainingTime}");

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;
            _timer.Start();

            var images = LoadCategoryImages();
            Console.WriteLine($"Loaded {images.Count} images from category");

            if (images.Count < 8)
            {
                MessageBox.Show($"Need at least 8 images, found {images.Count}. Using test data.");
                images = Enumerable.Range(1, 8)
                    .Select(i => new Uri(Path.Combine(Environment.CurrentDirectory, $"Assets/TestImage{i}.png")).AbsoluteUri)
                    .ToList();
            }


            /// Get screen size (working area)
            double screenWidth = SystemParameters.WorkArea.Width * 0.95;
            double screenHeight = SystemParameters.WorkArea.Height * 0.90;

            // Estimate max card size to fit all rows/columns on screen
            double maxCardWidth = (screenWidth - 100) / columns;
            double maxCardHeight = (screenHeight - 150) / rows;

            // Maintain card aspect ratio (2:3 or similar)
            double cardWidth = Math.Min(maxCardWidth, maxCardHeight * 2 / 3);
            double cardHeight = cardWidth * 1.5;

            // Set window size to fit all cards plus padding
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
                });
            };

            DataContext = viewModel;


            DataContext = viewModel;


        }


        private void Timer_Tick(object sender, EventArgs e)
        {
            // Update the timer text
            var minutes = _remainingTime / 60;
            var seconds = _remainingTime % 60;
            TimerTextBlock.Text = $"{minutes:D2}:{seconds:D2}"; // Ensure it's updating in the UI

            // Decrease the remaining time
            if (_remainingTime == 0)
            {
                _timer.Stop();
                // Here we call EndGame on the ViewModel to disable the cards
                ((GameViewModel)DataContext).EndGame();
                MessageBox.Show("Time's up! The game is over.");
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
                string imagesFolder = Path.Combine(baseDir, "..", "..", "..", "Categories", _selectedCategory);
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



    }
}
