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

        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int BestTime { get; set; }
        public int WorstTime { get; set; }
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
