using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MAP_Game.Model;
using MAP_Game.Helpers;
using System.IO;
using System.Text.Json;


namespace MAP_Game.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private const string UserFilePath = "Assets/users.json"; // adjust path if needed

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
                    var list = JsonSerializer.Deserialize<ObservableCollection<LoginModel>>(json);
                    return list ?? new ObservableCollection<LoginModel>();
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
                var json = JsonSerializer.Serialize(Users, new JsonSerializerOptions { WriteIndented = true });
                Directory.CreateDirectory(Path.GetDirectoryName(UserFilePath));
                File.WriteAllText(UserFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save users: {ex.Message}");
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
            var newUser = new LoginModel { Username = "New User", ImagePath = "Assets/default.png" };
            Users.Add(newUser);
            SaveUsers();
            OnPropertyChanged(nameof(Users));
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

}
