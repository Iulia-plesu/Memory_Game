namespace MAP_Game.Model
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string ImagePath { get; set; }
        public int LastMatchedPairs { get; set; }
        // Stats
        public int GamesPlayed { get; set; }
        public int GamesWon { get; set; }
        public int GamesLost { get; set; }
        public int BestTime { get; set; }
        public int WorstTime { get; set; }

        // Last Game Details
        public string LastCategory { get; set; }
        public int LastGridRows { get; set; }
        public int LastGridColumns { get; set; }
        public int LastTimeLeft { get; set; } // Time remaining when leaving

        public List<string> LastMatchedImagePaths { get; set; } = new List<string>();
        public string DisplayImagePath => string.IsNullOrEmpty(ImagePath)
            ? "pack://application:,,,/Assets/image1.png"
            : $"pack://application:,,,/{ImagePath}";
    
    }
}
