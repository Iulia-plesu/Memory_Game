using MAP_Game.Model;
using MAP_Game.ViewModel;
using System.Windows;

namespace MAP_Game.View
{
    public partial class FileWindow : Window
    {
        public string SelectedCategory { get; private set; }
        public int SelectedTime { get; private set; }
        public int GridRows { get; private set; } = 4;
        public int GridColumns { get; private set; } = 4;


        private LoginModel _selectedUser;  // Field to store the selected user

        public FileWindow(LoginModel selectedUser)
        {
            InitializeComponent();
            _selectedUser = selectedUser;
            SelectedCategory = "Rustic";  // Default category
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
            }

            // Access the LoginViewModel from DataContext
            var loginViewModel = (LoginViewModel)DataContext;

            // Check if SelectedUser is null
            if (loginViewModel.SelectedUser == null)
            {
                MessageBox.Show("No user selected.");
                return; // Prevent further processing if no user is selected
            }

            // Assuming LoginViewModel has a SelectedUser property
            var username = loginViewModel.SelectedUser.Username;

            // Start the game with the selected parameters
            var gameWindow = new GameWindow(username, loginViewModel, SelectedCategory, SelectedTime, GridRows, GridColumns);
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
        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            // Access the LoginViewModel to get the list of users
            var loginViewModel = (LoginViewModel)DataContext;
            var users = loginViewModel.Users;

            // Open the statistics window and pass the list of users
            var statisticsWindow = new StatisticsWindow();
            statisticsWindow.Show();
        }
        private void RestoreLastGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedUser.LastCategory == null)
            {
                MessageBox.Show("No previous game found to restore.");
                return;
            }

            var loginViewModel = (LoginViewModel)DataContext;
            var gameWindow = new GameWindow(
                _selectedUser.Username,
                loginViewModel,
                _selectedUser.LastCategory,
                _selectedUser.LastTimeLeft,
                _selectedUser.LastGridRows,
                _selectedUser.LastGridColumns,
                _selectedUser.LastMatchedPairs,
                _selectedUser.LastMatchedImagePaths
            );

            gameWindow.Show();
            this.Close();
        }
        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            // Open the AboutWindow when the "About" button is clicked
            var aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }
    }
}
