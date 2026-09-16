using System;
using System.Windows;
using System.Windows.Controls;

namespace TodoList;

public partial class EditWindow : Window
{
    private readonly TodoItem _item;
    private int _hours;
    private int _minutes;
    private static readonly int[] MinuteSteps = { 0, 15, 30, 45 };

    public EditWindow(TodoItem item)
    {
        InitializeComponent();
        _item = item;

        // Prefill text
        EditTextInput.Text = item.Text;

        // Prefill priority
        int idx = item.Priority switch { 1 => 0, 3 => 2, _ => 1 };
        EditPriorityCombo.SelectedIndex = idx;

        // Prefill duration
        double total = item.DurationHours;
        _hours = (int)total;
        double frac = total - _hours;
        int mins = (int)Math.Round(frac * 60);
        // Snap to nearest MinuteSteps value (or 0 if not matching)
        if (Array.IndexOf(MinuteSteps, mins) == -1)
        {
            // Find closest step
            int closest = MinuteSteps[0];
            int bestDiff = Math.Abs(mins - closest);
            foreach (var s in MinuteSteps)
            {
                int diff = Math.Abs(mins - s);
                if (diff < bestDiff) { bestDiff = diff; closest = s; }
            }
            mins = closest;
        }
        _minutes = mins;
        HourInput.Text = _hours.ToString();
        MinInput.Text = _minutes.ToString("D2");

        EditTextInput.Focus();
        EditTextInput.SelectAll();
    }

    private void HourUp_Click(object sender, RoutedEventArgs e)
    {
        _hours = Math.Min(_hours + 1, 23);
        HourInput.Text = _hours.ToString();
    }

    private void HourDown_Click(object sender, RoutedEventArgs e)
    {
        _hours = Math.Max(_hours - 1, 0);
        HourInput.Text = _hours.ToString();
    }

    private void MinUp_Click(object sender, RoutedEventArgs e)
    {
        int idx = Array.IndexOf(MinuteSteps, _minutes);
        _minutes = MinuteSteps[(idx + 1) % MinuteSteps.Length];
        MinInput.Text = _minutes.ToString("D2");
    }

    private void MinDown_Click(object sender, RoutedEventArgs e)
    {
        int idx = Array.IndexOf(MinuteSteps, _minutes);
        _minutes = MinuteSteps[(idx + MinuteSteps.Length - 1) % MinuteSteps.Length];
        MinInput.Text = _minutes.ToString("D2");
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        string text = EditTextInput.Text.Trim();
        if (string.IsNullOrEmpty(text))
        {
            MessageBox.Show("Task text cannot be empty.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        int priority = 2;
        if (EditPriorityCombo.SelectedItem is ComboBoxItem cbi && int.TryParse(cbi.Tag?.ToString(), out int p))
            priority = p;

        double totalHours = _hours + _minutes / 60.0;

        _item.Text = text;
        _item.Priority = priority;
        _item.DurationHours = totalHours;

        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
