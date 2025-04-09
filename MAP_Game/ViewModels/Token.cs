using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

public class Token : INotifyPropertyChanged
{
    private string _imagePath;
    private bool _isFaceUp;
    private bool _isMatched;

    public string ImagePath
    {
        get => _imagePath;
        set
        {
            _imagePath = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(ImageBitmap));
        }
    }

    public BitmapImage ImageBitmap
    {
        get
        {
            try
            {
                return new BitmapImage(new Uri(ImagePath));
            }
            catch
            {
                return null;
            }
        }
    }

    public bool IsFaceUp
    {
        get => _isFaceUp;
        set { _isFaceUp = value; OnPropertyChanged(); }
    }

    public bool IsMatched
    {
        get => _isMatched;
        set { _isMatched = value; OnPropertyChanged(); }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
