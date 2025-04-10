using MAP_Game.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;
using MAP_Game.View;

namespace MAP_Game.ViewModel
{
    public class GameViewModel : INotifyPropertyChanged
    {
        // Properties
        private int _matchedPairsCount;
        public int MatchedPairsCount
        {
            get => _matchedPairsCount;
            set
            {
                _matchedPairsCount = value;
                OnPropertyChanged(nameof(MatchedPairsCount));
            }
        }

        private bool _isGameActive;
        public bool IsGameActive
        {
            get => _isGameActive;
            set
            {
                if (_isGameActive != value)
                {
                    _isGameActive = value;
                    OnPropertyChanged(nameof(IsGameActive));
                }
            }
        }

        private double _cardWidth;
        public double CardWidth
        {
            get => _cardWidth;
            set { _cardWidth = value; OnPropertyChanged(nameof(CardWidth)); }
        }

        private double _cardHeight;
        public double CardHeight
        {
            get => _cardHeight;
            set { _cardHeight = value; OnPropertyChanged(nameof(CardHeight)); }
        }

        private int _rows = 4;
        public int Rows
        {
            get => _rows;
            set
            {
                _rows = value;
                OnPropertyChanged(nameof(Rows));
                LoadTokens();
            }
        }

        private int _columns = 4;
        public int Columns
        {
            get => _columns;
            set
            {
                _columns = value;
                OnPropertyChanged(nameof(Columns));
                LoadTokens();
            }
        }

        private string _timeDisplay;
        public string TimeDisplay
        {
            get => _timeDisplay;
            set
            {
                _timeDisplay = value;
                OnPropertyChanged(nameof(TimeDisplay));
            }
        }

        private List<Token> _tokens;
        public List<Token> Tokens
        {
            get => _tokens;
            private set
            {
                _tokens = value;
                OnPropertyChanged(nameof(Tokens));
            }
        }

        // Commands
        public ICommand CardClickCommand { get; }
        public ICommand LeaveCommand { get; }

        // Private fields
        private readonly List<string> _availableImages;
        private List<Token> _flippedTokens;
        private bool _isProcessing = false;
        private DispatcherTimer _timer;
        private int _remainingTime;
        private string _selectedCategory;
        private readonly string _currentUsername;
        private readonly LoginViewModel _loginViewModel;
        private readonly int _originalTimeLimit;
        private readonly Dictionary<string, string> _categoryPaths = new()
        {
            { "Mediterranean", @"Categories\Mediterranean" },
            { "Antarctica", @"Categories\Antarctica" },
            { "Rustic", @"Categories\Rustic" }
        };

        public GameViewModel(string username, LoginViewModel loginViewModel,
                           string selectedCategory = null, int timeLimitInSeconds = 0,
                           int rows = 4, int columns = 4,
                           int restoredMatchedPairs = 0, List<string> restoredMatchedImagePaths = null)
        {
            _currentUsername = username;
            _loginViewModel = loginViewModel;
            _originalTimeLimit = timeLimitInSeconds;
            _remainingTime = timeLimitInSeconds > 0 ? timeLimitInSeconds : 60;

            // Initialize commands
            CardClickCommand = new RelayCommand<Token>(FlipToken);
            LeaveCommand = new RelayCommand(LeaveGame);

            // Load game
            _availableImages = LoadCategoryImages(selectedCategory ?? "Rustic");
            if (_availableImages.Count < 8)
            {
                _availableImages = Enumerable.Range(1, 8)
                    .Select(i => new Uri(Path.Combine(Environment.CurrentDirectory, $"Assets/TestImage{i}.png")).AbsoluteUri)
                    .ToList();
            }

            Rows = rows;
            Columns = columns;

            // Calculate card dimensions
            CalculateCardDimensions();

            // Restore game state if needed
            if (restoredMatchedPairs > 0 && restoredMatchedImagePaths != null)
            {
                RestoreMatchedPairs(restoredMatchedPairs, restoredMatchedImagePaths);
            }

            // Initialize game
            IsGameActive = true;
            _flippedTokens = new List<Token>();
            StartTimer();
        }

        private void CalculateCardDimensions()
        {
            double screenWidth = SystemParameters.WorkArea.Width * 0.95;
            double screenHeight = SystemParameters.WorkArea.Height * 0.90;

            double maxCardWidth = (screenWidth - 100) / Columns;
            double maxCardHeight = (screenHeight - 150) / Rows;

            CardWidth = Math.Min(maxCardWidth, maxCardHeight * 2 / 3);
            CardHeight = CardWidth * 1.5;
        }

        private List<string> LoadCategoryImages(string category)
        {
            try
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string imagesFolder = Path.Combine(baseDir, _categoryPaths[category]);
                imagesFolder = Path.GetFullPath(imagesFolder);

                var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

                if (!Directory.Exists(imagesFolder))
                {
                    MessageBox.Show($"Directory not found: {imagesFolder}");
                    return new List<string>();
                }

                return Directory.GetFiles(imagesFolder)
                    .Where(file => imageExtensions.Contains(Path.GetExtension(file).ToLower()))
                    .Select(file => new Uri(file).AbsoluteUri)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading images: {ex.Message}");
                return new List<string>();
            }
        }

        private void StartTimer()
        {
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, e) =>
            {
                _remainingTime--;
                UpdateTimeDisplay();

                if (_remainingTime <= 0)
                {
                    _timer.Stop();
                    EndGame(false);
                }
            };
            _timer.Start();
            UpdateTimeDisplay();
        }

        private void UpdateTimeDisplay()
        {
            var minutes = _remainingTime / 60;
            var seconds = _remainingTime % 60;
            TimeDisplay = $"{minutes:D2}:{seconds:D2}";
        }

        public void EndGame(bool isWin)
        {
            IsGameActive = false;
            _timer?.Stop();

            int timeSpent = _originalTimeLimit - _remainingTime;
            _loginViewModel.UpdateStats(_currentUsername, isWin, timeSpent);

            if (isWin)
            {
                MessageBox.Show("🎉 Congratulations! You matched all the cards and won the game!",
                              "You Win", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Time's up! The game is over.",
                              "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void LeaveGame()
        {
            var matchedImagePaths = Tokens
                .Where(t => t.IsMatched)
                .Select(t => t.ImagePath)
                .Distinct()
                .ToList();

            _loginViewModel.UpdateLastGameStats(
                _currentUsername,
                _selectedCategory,
                Rows,
                Columns,
                _remainingTime,
                MatchedPairsCount,
                matchedImagePaths
            );

            Application.Current.Windows.OfType<GameWindow>().FirstOrDefault()?.Close();
        }

        private void LoadTokens()
        {
            try
            {
                int requiredPairs = (Rows * Columns) / 2;

                if (_availableImages == null || _availableImages.Count < requiredPairs)
                {
                    throw new Exception(_availableImages == null
                        ? "Image list is null"
                        : $"Need {requiredPairs} unique images but only found {_availableImages.Count}");
                }

                var selectedImages = _availableImages.Take(requiredPairs).ToList();
                Tokens = selectedImages.SelectMany(imagePath => new[]
                {
                    new Token { ImagePath = imagePath, IsFaceUp = false, IsMatched = false },
                    new Token { ImagePath = imagePath, IsFaceUp = false, IsMatched = false }
                }).OrderBy(t => Guid.NewGuid()).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tokens: {ex.Message}");
                Tokens = new List<Token>();
            }
        }

        public void FlipToken(Token token)
        {
            if (_isProcessing || token.IsMatched || token.IsFaceUp || !IsGameActive)
                return;

            token.IsFaceUp = true;
            _flippedTokens.Add(token);

            if (_flippedTokens.Count == 2)
            {
                _isProcessing = true;

                if (_flippedTokens[0].ImagePath == _flippedTokens[1].ImagePath)
                {
                    HandleMatch();
                }
                else
                {
                    HandleMismatch();
                }
            }
        }

        private void HandleMatch()
        {
            _flippedTokens[0].IsMatched = true;
            _flippedTokens[1].IsMatched = true;
            _flippedTokens.Clear();
            _isProcessing = false;
            MatchedPairsCount++;

            if (Tokens.All(t => t.IsMatched))
            {
                EndGame(true);
            }
        }

        private void HandleMismatch()
        {
            Task.Delay(500).ContinueWith(t =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _flippedTokens[0].IsFaceUp = false;
                    _flippedTokens[1].IsFaceUp = false;
                    _flippedTokens.Clear();
                    _isProcessing = false;
                });
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        public void RestoreMatchedPairs(int matchedPairsCount, List<string> matchedImagePaths)
        {
            MatchedPairsCount = matchedPairsCount;
            foreach (var token in Tokens)
            {
                if (matchedImagePaths.Contains(token.ImagePath))
                {
                    token.IsMatched = true;
                    token.IsFaceUp = true;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}