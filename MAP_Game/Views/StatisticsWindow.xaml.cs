using MAP_Game.Model;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace MAP_Game.View
{
    public partial class StatisticsWindow : Window
    {
        public ObservableCollection<LoginModel> Users { get; set; }

        public StatisticsWindow()
        {
            InitializeComponent();
            Users = LoadUsers();
            // Set the DataContext to the current window so that we can bind to it
            this.DataContext = this;
        }

        // Method to load users from the JSON file
        private ObservableCollection<LoginModel> LoadUsers()
        {
            var userFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "users.json");

            try
            {
                if (File.Exists(userFilePath))
                {
                    var json = File.ReadAllText(userFilePath);
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
    }

    // Helper class to match the JSON structure
    public class UsersData
    {
        public List<LoginModel> Users { get; set; }
    }
}
