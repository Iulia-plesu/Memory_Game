using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using System.Windows;


namespace MAP_Game.ViewModel
{
   

    // GameViewModel.cs modifications
    public class GameViewModel : INotifyPropertyChanged
    {
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
            get { return _isGameActive; }
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
        private int _columns = 4;

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

        // Modify LoadTokens to use Rows*Columns for the grid size
        

        private string _selectedCategory;
        private List<Token> _tokens;
        private List<Token> _flippedTokens;

        public List<Token> Tokens
        {
            get { return _tokens; }
            private set
            {
                _tokens = value;
                OnPropertyChanged(nameof(Tokens));
            }
        }

        public ICommand CardClickCommand { get; private set; }


        public event PropertyChangedEventHandler PropertyChanged;

        public GameViewModel(string selectedCategory, List<string> imagePaths)
        {
            try
            {
                IsGameActive = true;
                _selectedCategory = selectedCategory;
                _availableImages = imagePaths ?? throw new ArgumentNullException(nameof(imagePaths));

                Console.WriteLine($"Available images count: {_availableImages.Count}"); // Debug

                LoadTokens();
                _flippedTokens = new List<Token>();
                CardClickCommand = new RelayCommand<Token>((token) => FlipToken(token));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing GameViewModel: {ex.Message}");
                throw; // Re-throw to see the error in debugger
            }
        }
        public void EndGame()
        {
            IsGameActive = false; // Disable the game actions
        }


        private List<string> _availableImages; // Add this field

        private void LoadTokens()
        {
            try
            {
                int requiredPairs = (Rows * Columns) / 2;
                Console.WriteLine($"Required pairs: {requiredPairs}");

                if (_availableImages == null || _availableImages.Count < requiredPairs)
                {
                    string errorMsg = _availableImages == null
                        ? "Image list is null"
                        : $"Need {requiredPairs} unique images but only found {_availableImages.Count}";
                    throw new Exception(errorMsg);
                }

                var selectedImages = _availableImages.Take(requiredPairs).ToList();
                Console.WriteLine($"Selected {selectedImages.Count} images for pairs");

                var allTokens = selectedImages.SelectMany(imagePath => new[]
                {
            new Token { ImagePath = imagePath, IsFaceUp = false, IsMatched = false },
            new Token { ImagePath = imagePath, IsFaceUp = false, IsMatched = false }
        }).OrderBy(t => Guid.NewGuid()).ToList();

                Console.WriteLine($"Created {allTokens.Count} tokens");
                Tokens = allTokens;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoadTokens: {ex.Message}");
                throw;
            }
        }
        private bool _isProcessing = false;

        public Action OnGameWon { get; set; } // Add this delegate for notifying the GameWindow

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
                    // Match found
                    _flippedTokens[0].IsMatched = true;
                    _flippedTokens[1].IsMatched = true;
                    _flippedTokens.Clear();
                    _isProcessing = false;

                    MatchedPairsCount++; // Increment matched pairs count

                    // Check for win
                    if (Tokens.All(t => t.IsMatched))
                    {
                        IsGameActive = false;
                        OnGameWon?.Invoke();
                    }
                }
                else
                {
                    // No match - flip back after delay
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
            }
        }
        public void RestoreMatchedPairs(int matchedPairsCount, List<string> matchedImagePaths)
        {
            MatchedPairsCount = matchedPairsCount;

            // Mark all tokens with matching image paths as matched
            foreach (var token in Tokens)
            {
                if (matchedImagePaths.Contains(token.ImagePath))
                {
                    token.IsMatched = true;
                    token.IsFaceUp = true;
                }
            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
