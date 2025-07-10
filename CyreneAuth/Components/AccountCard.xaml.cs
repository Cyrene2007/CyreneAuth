using System;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Shapes;

namespace CyreneAuth.Components;

public sealed partial class AccountCard : UserControl
{
    public AccountCard()
    {
        InitializeComponent();
    }

    #region Properties

    public static readonly DependencyProperty AccountNameProperty =
        DependencyProperty.Register(nameof(AccountName), typeof(string), typeof(AccountCard), new PropertyMetadata(string.Empty, OnAccountNameChanged));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(nameof(Description), typeof(string), typeof(AccountCard), new PropertyMetadata(string.Empty, OnDescriptionChanged));

    public static readonly DependencyProperty AvatarSourceProperty =
        DependencyProperty.Register(nameof(AvatarSource), typeof(string), typeof(AccountCard), new PropertyMetadata(string.Empty, OnAvatarSourceChanged));

    public static readonly DependencyProperty TotpCodeProperty =
        DependencyProperty.Register(nameof(TotpCode), typeof(string), typeof(AccountCard), new PropertyMetadata(string.Empty, OnTotpCodeChanged));

    public static readonly DependencyProperty RemainingSecondsProperty =
        DependencyProperty.Register(nameof(RemainingSeconds), typeof(int), typeof(AccountCard), new PropertyMetadata(0, OnRemainingSecondsChanged));

    public static readonly DependencyProperty PeriodProperty =
        DependencyProperty.Register(nameof(Period), typeof(int), typeof(AccountCard), new PropertyMetadata(30));

    public string AccountName
    {
        get => (string)GetValue(AccountNameProperty);
        set => SetValue(AccountNameProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public string AvatarSource
    {
        get => (string)GetValue(AvatarSourceProperty);
        set => SetValue(AvatarSourceProperty, value);
    }

    public string TotpCode
    {
        get => (string)GetValue(TotpCodeProperty);
        set => SetValue(TotpCodeProperty, value);
    }

    public int RemainingSeconds
    {
        get => (int)GetValue(RemainingSecondsProperty);
        set => SetValue(RemainingSecondsProperty, value);
    }

    public int Period
    {
        get => (int)GetValue(PeriodProperty);
        set => SetValue(PeriodProperty, value);
    }

    #endregion

    #region Events

    private static void OnAccountNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not AccountCard card) return;
        card.AccountNameText.Text = e.NewValue?.ToString() ?? "";
    }

    private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not AccountCard card) return;
        card.DescriptionText.Text = e.NewValue?.ToString() ?? "";
    }

    private static void OnAvatarSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not AccountCard card) return;
        if (e.NewValue is not string source || string.IsNullOrEmpty(source)) return;

        try
        {
            var image = new BitmapImage(new Uri(source));

            var imageBrush = new ImageBrush()
            {
                ImageSource = image,
                Stretch = Stretch.UniformToFill
            };

            var ellipse = card.FindName("AvatarEllipse") as Ellipse;
            if (ellipse != null) ellipse.Fill = imageBrush;
        }
        catch
        {
            var ellipse = card.FindName("AvatarEllipse") as Ellipse;
            if (ellipse != null) ellipse.Fill = new SolidColorBrush(Colors.Gray);
        }
    }

    private static void OnTotpCodeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not AccountCard card) return;
        
        string code = e.NewValue?.ToString() ?? "";
        var textBlock = card.FindName("TotpCodeText") as TextBlock;
        if (textBlock == null) return;

        textBlock.Text = string.IsNullOrEmpty(code) ? "------" : code;
    }

    private static void OnRemainingSecondsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not AccountCard card) return;
        if (e.NewValue is not int seconds) return;
        
        var progressRing = card.FindName("TotpProgressRing") as ProgressRing;
        if (progressRing == null) return;
        
        // 计算进度百分比
        double percentage = (double)seconds / card.Period * 100;
        progressRing.Value = percentage;
        
        // 根据剩余时间设置不同颜色
        if (seconds <= 5)
        {
            progressRing.Foreground = new SolidColorBrush(Colors.Red);
        }
        else if (seconds <= 10)
        {
            progressRing.Foreground = new SolidColorBrush(Colors.Orange);
        }
        else
        {
            progressRing.Foreground = new SolidColorBrush(Colors.Green);
        }
    }

    #endregion
} 