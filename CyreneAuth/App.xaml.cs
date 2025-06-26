using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using UnhandledExceptionEventArgs = Microsoft.UI.Xaml.UnhandledExceptionEventArgs;

namespace CyreneAuth;

public partial class App : Application
{
    private static readonly MainWindow MainWindow = new();
    public App()
    {
        InitializeComponent();
        UnhandledException += HandleExceptions;
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {

        #if DEBUG

        if (Debugger.IsAttached)
            DebugSettings.BindingFailed += DebugSettings_BindingFailed;

        #endif

        MainWindow.ExtendsContentIntoTitleBar = true;
        MainWindow.Activate();
    }

    private void DebugSettings_BindingFailed(object sender, BindingFailedEventArgs e)
    {
        throw new Exception($"A debug binding failed: " + e.Message);
    }

    private void HandleExceptions(object sender, UnhandledExceptionEventArgs e)
    {
        e.Handled = true; // Don't crash the app.

        // Create the notification.
        var notification = new AppNotificationBuilder()
            .AddText("An exception was thrown.")
            .AddText($"Type: {e.Exception.GetType()}")
            .AddText($"Message: {e.Message}\r\n" +
                     $"HResult: {e.Exception.HResult}")
            .BuildNotification();

        // Show the notification
        AppNotificationManager.Default.Show(notification);
    }
}