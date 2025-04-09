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
            { "Category 1", @"C:/Users/Plesu/Desktop/WPF_Game/MAP_Game/Categories/Mediterranean" }
        };

        public GameWindow(string selectedCategory)
        {
            InitializeComponent();
            _selectedCategory = selectedCategory;

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
