using System.Windows;

namespace MAP_Game.View
{
    public partial class FileWindow : Window
    {
        public string SelectedCategory { get; private set; }
        public int SelectedTime { get; private set; }
        public int GridRows { get; private set; } = 4;
        public int GridColumns { get; private set; } = 4;


        public FileWindow()
        {
            InitializeComponent();
            SelectedCategory = "Rustic";
        }
        private void SizeButton_Click(object sender, RoutedEventArgs e)
        {
            var sizeWindow = new GridSizeInputWindow();
            if (sizeWindow.ShowDialog() == true)
            {
                GridRows = sizeWindow.Rows;
                GridColumns = sizeWindow.Columns;
            }
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
                SelectedTime = 60;
                return;
            }

            // Start the game with the selected category and time limit
            var gameWindow = new GameWindow(SelectedCategory, SelectedTime, GridRows, GridColumns);
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
