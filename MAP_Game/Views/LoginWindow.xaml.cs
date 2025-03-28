using System.Windows;

namespace MAP_Game.View
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new ViewModel.LoginViewModel();
        }
    }
}
