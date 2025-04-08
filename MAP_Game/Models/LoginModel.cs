namespace MAP_Game.Model
{
    public class LoginModel
    {
        public string Username { get; set; }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set => _imagePath = value;
        }

        public string DisplayImagePath
        {
            get
            {
                if (string.IsNullOrEmpty(ImagePath))
                    return "pack://application:,,,/Assets/image1.png"; // fallback

                return $"pack://application:,,,/{ImagePath}";
            }
        }
    }
}
