namespace MAP_Game.Model
{
    public class LoginModel
    {
        public string Username { get; set; }
        private string _imagePath;
        public string ImagePath
        {
            get => string.IsNullOrEmpty(_imagePath)
                ? "pack://application:,,,/Assets/image1.png" // Default Image
                : _imagePath;
            set
            {
                _imagePath = value;
                //OnPropertyChanged(nameof(ImagePath));
            }
        }
    }
}
