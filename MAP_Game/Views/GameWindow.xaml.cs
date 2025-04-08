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
            InitializeComponent();
            _selectedCategory = selectedCategory;

            //CategoryText.Text = $"Selected Category: {_selectedCategory}";

            // Load images for the selected category
            LoadCategoryImages();
        }

        private void LoadCategoryImages()
        {
            // Check if the selected category exists in the dictionary
            if (CategoryPaths.ContainsKey(_selectedCategory))
            {
                string categoryPath = CategoryPaths[_selectedCategory];

                // Check if the directory exists
                if (Directory.Exists(categoryPath))
                {
                    // Get all PNG images in the selected category folder
                    var images = Directory.GetFiles(categoryPath, "*.png");

                    if (images.Length > 0)
                    {
                        // Here you would load the images into your game, for now, we'll output to console
                        foreach (var image in images)
                        {
                            Console.WriteLine(image); // Replace this with actual game logic to load and display images
                        }
                    }
                    else
                    {
                        MessageBox.Show("No images found in the selected category.");
                    }
                }
                else
                {
                    MessageBox.Show($"The selected category path does not exist: {categoryPath}");
                }
            }
            else
            {
                MessageBox.Show("Invalid category selected.");
            }
        }
    }
}
