using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace CyreneAuth.Components;

public sealed partial class AddAccountCard : UserControl
{
    public event RoutedEventHandler? Click;

    public AddAccountCard()
    {
        InitializeComponent();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        Click?.Invoke(this, e);
    }
} 