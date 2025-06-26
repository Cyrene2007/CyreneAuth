using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using CyreneAuth.Views;

namespace CyreneAuth;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        ContentFrame.Navigate(typeof(HomePage));
        MainNavigationView.SelectedItem = MainNavigationView.MenuItems[0];
    }

    private void MainNavigationView_SelectionChanged(NavigationView _, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is not NavigationViewItem item) return;

        ContentFrame.Navigate(item.Tag switch
        {
            "HomePage" => typeof(HomePage),
            "AccountPage" => typeof(AccountPage),
            "SettingsPage" => typeof(SettingsPage),
            _ => typeof(HomePage),
        });
    }
}