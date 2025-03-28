using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MAP_Game
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

        public LoginViewModel()
        {
            Users = new ObservableCollection<LoginModel>
        {
    new LoginModel { Username = "John", ImagePath = @"C:\Users\Plesu\Documents\Facultate_anul_II\Semestrul_II\MAP\MAP_Game\MAP_Game\image1.png" },
            new LoginModel { Username = "Alice", ImagePath = "alice.png" }
        };

            PlayCommand = new RelayCommand(ExecutePlay, CanExecutePlay);
            DeleteCommand = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            AddUserCommand = new RelayCommand(ExecuteAddUser);
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
    }

}
