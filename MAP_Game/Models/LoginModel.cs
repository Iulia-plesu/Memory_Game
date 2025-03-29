namespace MAP_Game.Model
{
    public class LoginModel
    {
        public string Username { get; set; }

        private string _imagePath;
        public string ImagePath
        {
            get => string.IsNullOrEmpty(_imagePath)
                ? "pack://application:,,,/Assets/image1.png"  
                : _imagePath;
            set => _imagePath = value;
        }
    }
}
