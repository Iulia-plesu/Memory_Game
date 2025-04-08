using System.Windows;

namespace MAP_Game.View
{
    public partial class FileWindow : Window
    {
        public string SelectedCategory { get; private set; }

        public FileWindow()
        {
            InitializeComponent();
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            // Show the category selection window
            var categoryWindow = new CategorySelectionWindow();
            categoryWindow.ShowDialog();

            if (!string.IsNullOrWhiteSpace(categoryWindow.SelectedCategory))
            {
                SelectedCategory = categoryWindow.SelectedCategory;
                MessageBox.Show($"Category selected: {SelectedCategory}");
            }
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            // Ensure a category is selected
            if (string.IsNullOrEmpty(SelectedCategory))
            {
                MessageBox.Show("Please select a category first.");
                return;
            }

            // Start the game with the selected category
            var gameWindow = new GameWindow(SelectedCategory);
            gameWindow.Show();
            this.Close(); // Close the FileWindow when the game starts
        }
    }
}
