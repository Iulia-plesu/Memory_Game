using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Newtonsoft.Json.Linq;
using System.Windows; // Add this line

namespace MAP_Game.ViewModel
{
    public class Token
    {
        public string ImagePath { get; set; }
        public bool IsFaceUp { get; set; }
        public bool IsMatched { get; set; }
    }

    // GameViewModel.cs modifications
    public class GameViewModel : INotifyPropertyChanged
    {
        private int _rows = 4; // Default values
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

        private static readonly Dictionary<string, string> CategoryPaths = new()
    {
        { "Category 1", @"C:\Users\Plesu\Desktop\WPF_Game\MAP_Game\Categories\Category_1" },
        { "Category 2", @"C:\Users\Plesu\Desktop\WPF_Game\MAP_Game\Categories\Category_2" },
        { "Category 3", @"C:\Users\Plesu\Desktop\WPF_Game\MAP_Game\Categories\Category_3" }
    };

        public event PropertyChangedEventHandler PropertyChanged;

        public GameViewModel(string selectedCategory, List<string> imagePaths)
        {
            try
            {
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

        private List<string> _availableImages; // Add this field

        private void LoadTokens()
        {
            try
            {
                int requiredPairs = (Rows * Columns) / 2;
                Console.WriteLine($"Required pairs: {requiredPairs}"); // Debug

                if (_availableImages == null || _availableImages.Count < requiredPairs)
                {
                    string errorMsg = _availableImages == null
                        ? "Image list is null"
                        : $"Need {requiredPairs} unique images but only found {_availableImages.Count}";

                    throw new Exception(errorMsg);
                }

                var selectedImages = _availableImages.Take(requiredPairs).ToList();
                Console.WriteLine($"Selected {selectedImages.Count} images for pairs"); // Debug

                var allTokens = selectedImages.SelectMany(imagePath => new[]
                {
            new Token { ImagePath = imagePath, IsFaceUp = false, IsMatched = false },
            new Token { ImagePath = imagePath, IsFaceUp = false, IsMatched = false }
        }).OrderBy(t => Guid.NewGuid()).ToList();

                Console.WriteLine($"Created {allTokens.Count} tokens"); // Debug
                Tokens = allTokens;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoadTokens: {ex.Message}");
                throw;
            }
        }
        private bool _isProcessing;

        public void FlipToken(Token token)
        {
            if (_isProcessing || token.IsMatched)
                return;

            // Flip the card
            token.IsFaceUp = !token.IsFaceUp;

            if (token.IsFaceUp)
            {
                _flippedTokens.Add(token);

                if (_flippedTokens.Count == 2)
                {
                    _isProcessing = true;

                    if (_flippedTokens[0].ImagePath == _flippedTokens[1].ImagePath)
                    {
                        // Match found
                        _flippedTokens.ForEach(t => t.IsMatched = true);
                        _flippedTokens.Clear();
                        _isProcessing = false;
                    }
                    else
                    {
                        // No match - flip back after delay
                        Task.Delay(1000).ContinueWith(_ =>
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                _flippedTokens.ForEach(t => t.IsFaceUp = false);
                                _flippedTokens.Clear();
                                _isProcessing = false;
                            });
                        });
                    }
                }
            }
        }
        private List<string> LoadImagesFromCategory(string category)
        {
            var categoryPath = CategoryPaths[category];
            return Directory.GetFiles(categoryPath, "*.png")
                           .Select(f => new Uri(f).AbsoluteUri)
                           .ToList();
        }   

        //public void FlipToken(Token token)
        //{
        //    if (token.IsMatched)
        //        return;

        //    token.IsFaceUp = !token.IsFaceUp;
        //    if (token.IsFaceUp)
        //    {
        //        _flippedTokens.Add(token);

        //        if (_flippedTokens.Count == 2)
        //        {
        //            if (_flippedTokens[0].ImagePath == _flippedTokens[1].ImagePath)
        //            {
        //                _flippedTokens[0].IsMatched = true;
        //                _flippedTokens[1].IsMatched = true;
        //            }
        //            else
        //            {
        //                Task.Delay(1000).ContinueWith(t =>
        //                {
        //                    _flippedTokens[0].IsFaceUp = false;
        //                    _flippedTokens[1].IsFaceUp = false;
        //                    _flippedTokens.Clear();

        //                    OnPropertyChanged(nameof(Tokens));
        //                });
        //            }

        //            _flippedTokens.Clear();
        //        }
        //    }

        //    OnPropertyChanged(nameof(Tokens));
        //}

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
