using CyreneAuth.Data;
using CyreneAuth.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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
    private DispatcherTimer _totpTimer;

    public AccountPage()
    {
        InitializeComponent();

        LoadAccounts();
        
        // 初始化所有账号的验证码
        foreach (var item in AccountItems)
        {
            if (item is AccountData account && !string.IsNullOrEmpty(account.Secret))
            {
                account.CurrentCode = TotpService.GenerateTotp(
                    account.Secret,
                    account.Digits,
                    account.Period,
                    account.Algorithm);
                account.RemainingSeconds = TotpService.GetRemainingSeconds(account.Period);
            }
        }
        
        // 创建定时器，每秒更新一次验证码
        _totpTimer = new DispatcherTimer();
        _totpTimer.Interval = TimeSpan.FromMilliseconds(500); // 使用500毫秒的间隔以确保不会错过更新
        _totpTimer.Tick += TotpTimer_Tick;
        _totpTimer.Start();
    }

    private void TotpTimer_Tick(object sender, object e)
    {
        // 更新所有账号的验证码和剩余时间
        foreach (var item in AccountItems)
        {
            if (item is AccountData account && !string.IsNullOrEmpty(account.Secret))
            {
                // 更新剩余时间
                int remainingSeconds = TotpService.GetRemainingSeconds(account.Period);
                account.RemainingSeconds = remainingSeconds;
                
                // 如果剩余时间为0或验证码为空，则重新生成验证码
                if (remainingSeconds == account.Period || string.IsNullOrEmpty(account.CurrentCode))
                {
                    string newCode = TotpService.GenerateTotp(
                        account.Secret,
                        account.Digits,
                        account.Period,
                        account.Algorithm);
                    
                    // 只有当验证码真正变化时才更新，避免不必要的UI刷新
                    if (account.CurrentCode != newCode)
                    {
                        account.CurrentCode = newCode;
                    }
                }
            }
        }
    }

    private void LoadAccounts()
    {
        // 示例账号，实际应用中应从存储中加载
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

        // 获取选择的哈希算法
        string algorithm = (AlgorithmComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "SHA1";
        
        // 获取验证码位数和更新周期
        int digits = (int)DigitsNumberBox.Value;
        int period = (int)PeriodNumberBox.Value;

        // 创建新账号
        var newAccount = new AccountData
        {
            Name = accountName,
            Description = username,
            HeadIcon = "ms-appx:///Assets/Icon.png",
            Secret = secret,
            Algorithm = algorithm,
            Digits = digits,
            Period = period
        };

        // 生成初始验证码
        newAccount.CurrentCode = TotpService.GenerateTotp(
            newAccount.Secret,
            newAccount.Digits,
            newAccount.Period,
            newAccount.Algorithm);
        
        newAccount.RemainingSeconds = TotpService.GetRemainingSeconds(newAccount.Period);

        // 添加到列表
        AccountItems.Insert(AccountItems.Count - 1, newAccount);
    }

    private void GenerateSecretButton_Click(object sender, RoutedEventArgs e)
    {
        // 生成随机密钥
        string secret = TotpService.GenerateRandomSecret();
        SecretKeyBox.Password = secret;
    }

    private async void ScanQRCodeButton_Click(object sender, RoutedEventArgs e)
    {
        // TODO
    }
}