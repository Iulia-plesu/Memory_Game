using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Navigation;

namespace MAP_Game.View
{
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void EmailHyperlink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string email = "email@gmail.ro";
                string subject = "Regarding Tavern Chronicles Game";
                string body = "Dear Tavern Keeper,\n\n";

                string mailtoUri = $"mailto:{email}?subject={Uri.EscapeDataString(subject)}&body={Uri.EscapeDataString(body)}";

                Process.Start(new ProcessStartInfo(mailtoUri)
                {
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not open email client: {ex.Message}",
                              "Error",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }
    }
}