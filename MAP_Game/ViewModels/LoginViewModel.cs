using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MAP_Game.Model;
using MAP_Game.Helpers;
using MAP_Game.Helpers;
using MAP_Game.Model;

namespace MAP_Game.ViewModel
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<LoginModel> Users { get; set; }

        private LoginModel _selectedUser;
        public LoginModel SelectedUser
        {
            get { return _selectedUser; }
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
                new LoginModel { Username = "John", ImagePath = "Assets\\image1.png" },
                new LoginModel { Username = "Alice", ImagePath = "alice.png" }
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
                MessageBox.Show($"Starting game for {SelectedUser.Username}");
            }
        }

        private bool CanExecutePlay(object parameter) => SelectedUser != null;

        private void ExecuteDelete(object parameter)
        {
            if (SelectedUser != null)
            {
                Users.Remove(SelectedUser);
                OnPropertyChanged(nameof(Users));
            }
        }

        private bool CanExecuteDelete(object parameter) => SelectedUser != null;

        private void ExecuteAddUser(object parameter)
        {
            Users.Add(new LoginModel { Username = "New User", ImagePath = "default.png" });
            OnPropertyChanged(nameof(Users));
        }

        private void ExecuteCancel(object parameter)
        {
            Application.Current.Shutdown();
        }
    }
}
