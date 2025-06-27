using Microsoft.UI.Xaml;
using CyreneAuth.Pages;
using Microsoft.UI.Windowing;

namespace CyreneAuth;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        Title = "Cyrene Auth";
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        AppWindow.SetIcon("Assets/Icon.ico");
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

        ContentFrame.Navigate(typeof(NavigationPage));
    }
}