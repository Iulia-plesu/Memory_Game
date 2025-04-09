using System.Windows;

namespace MAP_Game.View
{
    public partial class FileWindow : Window
    {
        public string SelectedCategory { get; private set; }
        public int SelectedTime { get; private set; } // Use this property to hold the selected time

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

            // Ensure a time limit is selected
            if (SelectedTime == 0)
            {
                MessageBox.Show("Please select a time limit first.");
                return;
            }

            // Start the game with the selected category and time limit
            var gameWindow = new GameWindow(SelectedCategory, SelectedTime);
            gameWindow.Show();
            this.Close(); // Close the FileWindow when the game starts
        }

        private void TimeButton_Click(object sender, RoutedEventArgs e)
        {
            // Show a dialog to enter time limit
            var timeInputWindow = new TimeInputWindow();
            if (timeInputWindow.ShowDialog() == true)
            {
                // The user entered a valid time, store it
                SelectedTime = timeInputWindow.TimeLimit;
            }
        }
    }
}
