using CyreneAuth.Data;
using CyreneAuth.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;

namespace CyreneAuth.Pages.Views;

public partial class CardTypeSelector : DataTemplateSelector
{
    public DataTemplate Data { get; set; } = new();
    public DataTemplate Add { get; set; } = new();

    protected override DataTemplate SelectTemplateCore(object item)
    {
        if (item is AccountData)
            return Data!;
        else
            return Add!;
    }
}

public sealed partial class AccountPage : Page
{
    private ObservableCollection<object> AccountItems { get; } = [];
    private DispatcherTimer TotpTimer { get; set; }= new();

    public AccountPage()
    {
        InitializeComponent();

        LoadAccounts();
        foreach (var item in AccountItems)
        {
            if (item is not AccountData account || string.IsNullOrEmpty(account.Secret)) continue;

            account.CurrentCode = TotpService.GenerateTotp(
                account.Secret, account.Digits, account.Period, account.Algorithm);
            account.RemainingSeconds = TotpService.GetRemainingSeconds(account.Period);
        }

        TotpTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(50)
        };
        TotpTimer.Tick += TotpTimer_Tick;
        TotpTimer.Start();
    }

    private void TotpTimer_Tick(object? sender, object e)
    {
        foreach (var item in AccountItems)
        {
            if (item is not AccountData account || string.IsNullOrEmpty(account.Secret)) continue;

            account.RemainingSeconds = TotpService.GetRemainingSeconds(account.Period);
            if (account.RemainingSeconds == account.Period || string.IsNullOrEmpty(account.CurrentCode))
            {
                var newCode = TotpService.GenerateTotp(
                    account.Secret, account.Digits, account.Period, account.Algorithm);

                if (account.CurrentCode != newCode) account.CurrentCode = newCode;
            }
        }
    }

    private void LoadAccounts()
    {
        for (int i = 0; i < 20; i++)
            AccountItems.Add(new AccountData
            {
                Name = "Cyrene's Home(AAA)",
                Description = "由本地网站生成用作测试",
                HeadIcon = "ms-appx:///Assets/Icon.png",
                Secret = "G5DEUL3HEZDXWLSKFBDF4JRFMRIDWOKQMVBG6426MVAV2LCGNV5Q",
                Algorithm = "SHA1",
                Digits = 6,
                Period = 30
            });

        AccountItems.Add(new object()); // Empty for AddAccountCard
    }

    private async void AddAccountCard_Click(object sender, RoutedEventArgs e)
    {
        AccountNameTextBox.Text = "";
        UsernameTextBox.Text = "";
        SecretKeyBox.Password = "";
        AlgorithmComboBox.SelectedIndex = 0;
        DigitsNumberBox.Value = 6;
        PeriodNumberBox.Value = 30;

        await AddAccountDialog.ShowAsync();
    }

    private void AddAccountDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
    {
        var accountName = AccountNameTextBox.Text.Trim();
        var username = UsernameTextBox.Text.Trim();
        var secret = SecretKeyBox.Password.Trim();

        if (string.IsNullOrEmpty(accountName))
        {
            args.Cancel = true;
            return;
        }
        if (string.IsNullOrEmpty(username))
        {
            args.Cancel = true;
            return;
        }
        if (string.IsNullOrEmpty(secret))
        {
            args.Cancel = true;
            return;
        }

        var algorithm = (AlgorithmComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "SHA1";
        var digits = DigitsNumberBox.Value;
        var period = PeriodNumberBox.Value;

        var newAccount = new AccountData
        {
            Name = accountName,
            Description = username,
            HeadIcon = "ms-appx:///Assets/Icon.png",
            Secret = secret,
            Algorithm = algorithm,
            Digits = (int)digits,
            Period = (int)period
        };

        newAccount.CurrentCode = TotpService.GenerateTotp(
            newAccount.Secret, newAccount.Digits, newAccount.Period, newAccount.Algorithm);
        newAccount.RemainingSeconds = TotpService.GetRemainingSeconds(newAccount.Period);

        AccountItems.Insert(AccountItems.Count - 1, newAccount);
    }

    private void GenerateSecretButton_Click(object sender, RoutedEventArgs e)
    {
        var secret = TotpService.GenerateRandomSecret();
        SecretKeyBox.Password = secret;
    }

    private void ScanQRCodeButton_Click(object sender, RoutedEventArgs e)
    {
        // TODO
    }
}