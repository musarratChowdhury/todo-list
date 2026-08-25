using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace TodoList;

public partial class CountdownWindow : Window
{
    private readonly TodoItem _task;
    private TimeSpan _remaining;
    private readonly TimeSpan _total;
    private readonly DispatcherTimer _timer;
    private bool _isPaused;

    // Mini-dock overlay
    private Window? _miniWindow;
    private TextBlock? _miniCountdownText;
    private bool _isMini;

    public CountdownWindow(TodoItem task)
    {
        InitializeComponent();
        _task = task;

        _total = TimeSpan.FromHours(task.DurationHours > 0 ? task.DurationHours : 1);
        _remaining = _total;

        TaskNameText.Text = task.Text;
        UpdateDisplay();

        _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        if (_isPaused) return;

        _remaining = _remaining.Subtract(TimeSpan.FromSeconds(1));

        if (_remaining <= TimeSpan.Zero)
        {
            _remaining = TimeSpan.Zero;
            _timer.Stop();
            StatusText.Text = "Time's up! 🎉";
            CountdownText.Foreground = new SolidColorBrush(Color.FromRgb(243, 139, 168)); // red-ish
            PauseButton.IsEnabled = false;
        }

        UpdateDisplay();
        if (_miniCountdownText != null)
            _miniCountdownText.Text = FormatTime(_remaining);
    }

    private void UpdateDisplay()
    {
        CountdownText.Text = FormatTime(_remaining);
        if (_remaining > TimeSpan.Zero)
            StatusText.Text = _isPaused ? "Paused" : "Running";
    }

    private static string FormatTime(TimeSpan t)
        => $"{(int)t.TotalHours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";

    private void PauseButton_Click(object sender, RoutedEventArgs e)
    {
        _isPaused = !_isPaused;
        PauseButton.Content = _isPaused ? "▶ Resume" : "⏸ Pause";
        StatusText.Text = _isPaused ? "Paused" : "Running";
    }

    private void ResetButton_Click(object sender, RoutedEventArgs e)
    {
        _remaining = _total;
        _isPaused = false;
        PauseButton.Content = "⏸ Pause";
        StatusText.Text = "Running";
        CountdownText.Foreground = new SolidColorBrush(Color.FromRgb(166, 227, 161));
        PauseButton.IsEnabled = true;
        if (!_timer.IsEnabled) _timer.Start();
        UpdateDisplay();
    }

    private void MiniModeButton_Click(object sender, RoutedEventArgs e)
    {
        if (_isMini) return;
        _isMini = true;
        this.Hide();

        var (dockW, dockH, dockFont) = AppSettings.Instance.GetDockedDimensions();

        // Build the mini docked window
        _miniWindow = new Window
        {
            Title = "",
            Width = dockW,
            Height = dockH,
            ResizeMode = ResizeMode.NoResize,
            WindowStyle = WindowStyle.None,
            Topmost = true,
            Background = new SolidColorBrush(Color.FromRgb(30, 30, 46)),
            ShowInTaskbar = false,
            AllowsTransparency = false
        };

        // Position bottom-right of work area
        var screen = SystemParameters.WorkArea;
        _miniWindow.Left = screen.Right - _miniWindow.Width - 10;
        _miniWindow.Top = screen.Bottom - _miniWindow.Height - 10;

        // Mini layout
        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

        _miniCountdownText = new TextBlock
        {
            Text = FormatTime(_remaining),
            FontSize = dockFont,
            FontFamily = new FontFamily("Consolas"),
            FontWeight = FontWeights.Bold,
            Foreground = new SolidColorBrush(Color.FromRgb(166, 227, 161)),
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(10, 0, 0, 0)
        };
        Grid.SetColumn(_miniCountdownText, 0);

        var expandBtn = new Button
        {
            Content = "⬆",
            Width = 28,
            Height = 28,
            Background = new SolidColorBrush(Color.FromRgb(69, 71, 90)),
            Foreground = Brushes.White,
            BorderThickness = new Thickness(0),
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(4, 0, 4, 0),
            FontSize = 13
        };
        Grid.SetColumn(expandBtn, 1);
        expandBtn.Click += (s, e2) => RestoreFromMini();

        var closeBtn = new Button
        {
            Content = "✕",
            Width = 28,
            Height = 28,
            Background = new SolidColorBrush(Color.FromRgb(69, 71, 90)),
            Foreground = Brushes.White,
            BorderThickness = new Thickness(0),
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 6, 0),
            FontSize = 13
        };
        Grid.SetColumn(closeBtn, 2);
        closeBtn.Click += (s, e2) =>
        {
            _miniWindow.Close();
            _timer.Stop();
            Close();
        };

        grid.Children.Add(_miniCountdownText);
        grid.Children.Add(expandBtn);
        grid.Children.Add(closeBtn);

        _miniWindow.Content = grid;
        _miniWindow.Closed += (s, e2) =>
        {
            if (_isMini) { _isMini = false; RestoreFromMini(); }
        };
        _miniWindow.Show();
    }

    private void RestoreFromMini()
    {
        _isMini = false;
        _miniWindow?.Close();
        _miniWindow = null;
        _miniCountdownText = null;
        this.Show();
        this.Activate();
        UpdateDisplay();
    }

    private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        _timer.Stop();
        _miniWindow?.Close();
    }
}
