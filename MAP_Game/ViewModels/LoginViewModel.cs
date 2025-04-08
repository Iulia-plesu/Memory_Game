using MAP_Game.Helpers;
using MAP_Game.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using System.Windows;

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

            PlayCommand = new RelayCommand(ExecutePlay, CanExecutePlay);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            AddUserCommand = new RelayCommand(ExecuteAddUser);
            CancelCommand = new RelayCommand(ExecuteCancel);
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
                var gameWindow = new View.GameWindow();
                gameWindow.Show();
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
    }

    // Helper class to match the JSON structure
    public class UsersData
    {
        public List<LoginModel> Users { get; set; }
    }
}
