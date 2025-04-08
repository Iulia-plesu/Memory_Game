using MAP_Game.ViewModel;
using System;
using System.IO;
using System.Windows;

namespace MAP_Game.View
{
    public partial class GameWindow : Window
    {
        private string _selectedCategory;

        // Define the paths for each category
        private readonly Dictionary<string, string> CategoryPaths = new()
        {
            { "Category 1", @"C:\Users\Plesu\Desktop\WPF_Game\MAP_Game\Categories\Category_1" },
            { "Category 2", @"C:\Users\Plesu\Desktop\WPF_Game\MAP_Game\Categories\Category_2" },
            { "Category 3", @"C:\Users\Plesu\Desktop\WPF_Game\MAP_Game\Categories\Category_3" }
        };

        public GameWindow(string selectedCategory)
        {
            try
            {
                InitializeComponent();
                _selectedCategory = selectedCategory;

                var images = LoadCategoryImages();
                Console.WriteLine($"Loaded {images.Count} images from category"); // Debug

                if (images.Count < 8) // Minimum for 4x4 grid
                {
                    MessageBox.Show($"Need at least 8 images, found {images.Count}. Using test data.");
                    images = Enumerable.Range(1, 8).Select(i => $"/Assets/TestImage{i}.png").ToList();
                }

                DataContext = new GameViewModel(selectedCategory, images);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to initialize game: {ex.Message}");
                Close();
            }
            Loaded += (s, e) => {
                var vm = DataContext as GameViewModel;
                Console.WriteLine($"Token count: {vm?.Tokens?.Count}");
                if (vm?.Tokens != null)
                {
                    foreach (var token in vm.Tokens)
                    {
                        Console.WriteLine($"Token: {token.ImagePath}, FaceUp: {token.IsFaceUp}");
                    }
                }
            };
        }

        private List<string> LoadCategoryImages()
        {
            if (CategoryPaths.ContainsKey(_selectedCategory))
            {
                string categoryPath = CategoryPaths[_selectedCategory];

                if (Directory.Exists(categoryPath))
                {
                    var images = Directory.GetFiles(categoryPath, "*.png").ToList();
                    Console.WriteLine($"Found {images.Count} images in category");
                    return images;
                }
            }
            MessageBox.Show("Category path not found or no images available");
            return new List<string>();
        }

    }
}
