using System.Collections.Generic;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MAP_Game.View
{
    public partial class NewUserWindow : Window
    {
        private readonly List<string> _imagePaths = new()
        {
            "image1.png",
            "image2.png",
            "image3.png",
            "image4.png",
            "image5.png",
        };

        private int _currentImageIndex = 0;

        public string Username { get; private set; }
        public string SelectedImagePath { get; private set; }

        public NewUserWindow()
        {
            InitializeComponent();
            UpdateImage();
        }

        private void UpdateImage()
        {
            string imageName = _imagePaths[_currentImageIndex];
            string relativePath = $"../Assets/{imageName}";  

            ImagePreview.Source = new BitmapImage(new System.Uri($"pack://application:,,,/Assets/{imageName}"));
            SelectedImagePath = relativePath;  
        }

        private void Left_Click(object sender, RoutedEventArgs e)
        {
            _currentImageIndex = (_currentImageIndex - 1 + _imagePaths.Count) % _imagePaths.Count;
            UpdateImage(); 
        }

        private void Right_Click(object sender, RoutedEventArgs e)
        {
            _currentImageIndex = (_currentImageIndex + 1) % _imagePaths.Count;
            UpdateImage();
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Please enter a name.");
                return;
            }

            Username = NameBox.Text.Trim();
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
