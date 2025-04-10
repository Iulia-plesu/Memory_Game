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


        private LoginModel _selectedUser;  

        public FileWindow(LoginModel selectedUser)
        {
            InitializeComponent();
            _selectedUser = selectedUser;
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
            var categoryWindow = new CategorySelectionWindow();
            categoryWindow.ShowDialog();

            if (!string.IsNullOrWhiteSpace(categoryWindow.SelectedCategory))
            {
                SelectedCategory = categoryWindow.SelectedCategory;
            }
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedCategory))
            {
                MessageBox.Show("Please select a category first.");
                return;
            }

            if (SelectedTime == 0)
            {
                SelectedTime = 60;
            }

            var loginViewModel = (LoginViewModel)DataContext;

            if (loginViewModel.SelectedUser == null)
            {
                MessageBox.Show("No user selected.");
                return; 
            }

            var username = loginViewModel.SelectedUser.Username;

            var gameWindow = new GameWindow(username, loginViewModel, SelectedCategory, SelectedTime, GridRows, GridColumns);
            gameWindow.Show();
            this.Close(); 
        }

        private void TimeButton_Click(object sender, RoutedEventArgs e)
        {
            var timeInputWindow = new TimeInputWindow();
            if (timeInputWindow.ShowDialog() == true)
            {
                SelectedTime = timeInputWindow.TimeLimit;
            }
        }
        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            var loginViewModel = (LoginViewModel)DataContext;
            var users = loginViewModel.Users;

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
            var aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }
    }
}
