using System.ComponentModel;
using System.Runtime.CompilerServices;

public class Token : INotifyPropertyChanged
{
    private string _imagePath;
    private bool _isFaceUp;
    private bool _isMatched;

    public string ImagePath
    {
        get => _imagePath;
        set { _imagePath = value; OnPropertyChanged(); }
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
