using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MAP_Game.Model;
using MAP_Game.Helpers;

namespace MAP_Game.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<LoginModel> Users { get; set; }

        private LoginModel _selectedUser;
        public LoginModel SelectedUser
        {
            get => _selectedUser ?? new LoginModel { ImagePath = "pack://application:,,,/Assets/image1.png" }; // Ensure default image
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
            Users = new ObservableCollection<LoginModel>
            {
                new LoginModel { Username = "John", ImagePath = "pack://application:,,,/Assets/image2.png" },
                new LoginModel { Username = "Alice", ImagePath = "pack://application:,,,/Assets/image3.png" }
            };

            PlayCommand = new RelayCommand(ExecutePlay, CanExecutePlay);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            AddUserCommand = new RelayCommand(ExecuteAddUser);
            CancelCommand = new RelayCommand(ExecuteCancel);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
                OnPropertyChanged(nameof(Users));
                OnPropertyChanged(nameof(SelectedUser)); // Refresh the image
            }
        }

        private bool CanExecuteDelete(object parameter) => SelectedUser != null;

        private void ExecuteAddUser(object parameter)
        {
            Users.Add(new LoginModel { Username = "New User", ImagePath = "pack://application:,,,/Assets/default.png" });
            OnPropertyChanged(nameof(Users));
        }

        private void ExecuteCancel(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
}
