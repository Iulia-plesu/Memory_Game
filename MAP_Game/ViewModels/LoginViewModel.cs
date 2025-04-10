using MAP_Game.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using System.Windows;
using MAP_Game.View;

namespace MAP_Game.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private static string UserFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "users.json");

        public ObservableCollection<LoginModel> Users { get; set; }

        private LoginModel _selectedUser;
        public LoginModel SelectedUser
        {
            get => _selectedUser ?? new LoginModel { ImagePath = "Assets/image1.png" };
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }

        public ICommand PlayCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand AddUserCommand { get; }
        public ICommand CancelCommand { get; }

        public LoginViewModel()
        {
            Users = LoadUsers();

            PlayCommand = new RelayCommand<object>((parameter) => ExecutePlay(parameter));  // Pass parameter to ExecutePlay
            DeleteCommand = new RelayCommand<object>((parameter) => ExecuteDelete(parameter));  // Pass parameter to ExecuteDelete
            AddUserCommand = new RelayCommand<object>((parameter) => ExecuteAddUser(parameter));  // Pass parameter to ExecuteAddUser
            CancelCommand = new RelayCommand<object>((parameter) => ExecuteCancel(parameter));  // Pass parameter to ExecuteCancel
        }
        public void UpdateStats(string username, bool isWin, int timeInSeconds)
        {
            var user = Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return;

            user.GamesPlayed++;

            if (isWin)
            {
                user.GamesWon++;
                if (timeInSeconds < user.BestTime)
                    user.BestTime = timeInSeconds;
            }
            else
            {
                user.GamesLost++;
            }

            if (timeInSeconds > user.WorstTime)
                user.WorstTime = timeInSeconds;

            SaveUsers();
        }

        private ObservableCollection<LoginModel> LoadUsers()
        {
            try
            {
                if (File.Exists(UserFilePath))
                {
                    var json = File.ReadAllText(UserFilePath);
                    var usersData = JsonSerializer.Deserialize<UsersData>(json);

                    return usersData?.Users != null ? new ObservableCollection<LoginModel>(usersData.Users) : new ObservableCollection<LoginModel>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load users: {ex.Message}");
            }

            return new ObservableCollection<LoginModel>();
        }

        private void SaveUsers()
        {
            try
            {
                var usersData = new UsersData
                {
                    Users = Users.ToList()
                };

                var json = JsonSerializer.Serialize(usersData, new JsonSerializerOptions { WriteIndented = true });

                var directory = Path.GetDirectoryName(UserFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(UserFilePath, json);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save users to {UserFilePath}: {ex.Message}");
            }
        }

        private void ExecutePlay(object parameter)
        {
            if (SelectedUser != null)
            {
                // In the part where you are opening FileWindow
                var fileWindow = new FileWindow(SelectedUser); // Make sure selectedUser is passed correctly
                fileWindow.DataContext = this;  // Set DataContext to LoginViewModel
                fileWindow.Show();


                Application.Current.Windows[0]?.Close();
            }
        }

        private bool CanExecutePlay(object parameter) => SelectedUser != null;

        private void ExecuteDelete(object parameter)
        {
            if (SelectedUser != null)
            {
                Users.Remove(SelectedUser);
                SaveUsers();
                OnPropertyChanged(nameof(Users));
                OnPropertyChanged(nameof(SelectedUser));
            }
        }

        private bool CanExecuteDelete(object parameter) => SelectedUser != null;

        private void ExecuteAddUser(object parameter)
        {
            var addUserWindow = new View.NewUserWindow
            {
                Owner = Application.Current.MainWindow
            };

            if (addUserWindow.ShowDialog() == true)
            {
                var newUser = new LoginModel
                {
                    Username = addUserWindow.Username,
                    ImagePath = addUserWindow.SelectedImagePath 
                };

                Users.Add(newUser);

                SaveUsers();
                OnPropertyChanged(nameof(Users));
            }
        }

        private void ExecuteCancel(object parameter)
        {
            Application.Current.Shutdown();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void UpdateLastGameStats(string username, string category, int rows, int columns,
                              int timeLeft, int matchedPairs, List<string> matchedImagePaths)
        {
            var user = Users.FirstOrDefault(u => u.Username == username);
            if (user == null) return;

            user.LastCategory = category;
            user.LastGridRows = rows;
            user.LastGridColumns = columns;
            user.LastTimeLeft = timeLeft;
            user.LastMatchedPairs = matchedPairs;
            user.LastMatchedImagePaths = matchedImagePaths ?? new List<string>();

            SaveUsers();
        }
    }


    // Helper class to match the JSON structure
    public class UsersData
    {
        public List<LoginModel> Users { get; set; }
    }
}
