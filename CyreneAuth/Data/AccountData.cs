using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CyreneAuth.Data;

public partial class AccountData : INotifyPropertyChanged
{
    private string _name = "";
    private string _description = "";
    private string _headIcon = "";
    private string _secret = "";
    private int _digits = 6;
    private int _period = 30;
    private string _algorithm = "SHA1";
    private string _currentCode = "";
    private int _remainingSeconds = 0;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string HeadIcon
    {
        get => _headIcon;
        set => SetProperty(ref _headIcon, value);
    }

    public string Secret
    {
        get => _secret;
        set => SetProperty(ref _secret, value);
    }

    public int Digits
    {
        get => _digits;
        set => SetProperty(ref _digits, value);
    }

    public int Period
    {
        get => _period;
        set => SetProperty(ref _period, value);
    }

    public string Algorithm
    {
        get => _algorithm;
        set => SetProperty(ref _algorithm, value);
    }

    public string CurrentCode
    {
        get => _currentCode;
        set => SetProperty(ref _currentCode, value);
    }

    public int RemainingSeconds
    {
        get => _remainingSeconds;
        set => SetProperty(ref _remainingSeconds, value);
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}