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

        public int GamesPlayed { get; set; } = 0;
        public int GamesWon { get; set; } = 0;
        public int GamesLost { get; set; } = 0;
        public int BestTime { get; set; } = int.MaxValue; // Lower is better
        public int WorstTime { get; set; } = 0; // Higher is worse
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
