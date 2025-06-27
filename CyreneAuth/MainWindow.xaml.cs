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

        AppWindow.Resize(new Windows.Graphics.SizeInt32(1400, 800));
        AppWindow.SetIcon("Assets/Icon.png");
        AppWindow.TitleBar.PreferredHeightOption = TitleBarHeightOption.Tall;

        if (AppWindow.Presenter is OverlappedPresenter presenter)
        {
            presenter.IsMaximizable = false;
            presenter.IsResizable = false;
        }

        ContentFrame.Navigate(typeof(NavigationPage));
    }
}