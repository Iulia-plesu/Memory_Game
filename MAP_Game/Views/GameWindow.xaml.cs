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

        public GameWindow(string selectedCategory, int timeLimitInSeconds)
        {
            InitializeComponent();
            _selectedCategory = selectedCategory;

            // Debugging output: Ensure the time is passed correctly
            Console.WriteLine($"Game initialized with time limit: {timeLimitInSeconds}");

            _remainingTime = timeLimitInSeconds > 0 ? timeLimitInSeconds : 60; // Ensure it’s a positive time value

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += Timer_Tick;

            // Start the timer
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

            DataContext = new GameViewModel(selectedCategory, images);
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
