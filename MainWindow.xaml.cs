using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace TodoList;

public partial class MainWindow : Window
{
    private ObservableCollection<TodoItem> _allTodos;
    private ICollectionView _todoView;
    private const string SaveFile = "todos.json";

    public MainWindow()
    {
        InitializeComponent();
        
        _allTodos = LoadTodos();
        _todoView = CollectionViewSource.GetDefaultView(_allTodos);
        
        if (_todoView is ListCollectionView listCollectionView)
        {
            listCollectionView.IsLiveSorting = true;
            listCollectionView.LiveSortingProperties.Add("IsDone");
            listCollectionView.LiveSortingProperties.Add("Priority");
        }
        
        _todoView.SortDescriptions.Add(new SortDescription("IsDone", ListSortDirection.Ascending));
        _todoView.SortDescriptions.Add(new SortDescription("Priority", ListSortDirection.Ascending));
        _todoView.Filter = FilterTodo;
        TodoListBox.ItemsSource = _todoView;
    }

    private ObservableCollection<TodoItem> LoadTodos()
    {
        if (File.Exists(SaveFile))
        {
            try
            {
                var json = File.ReadAllText(SaveFile);
                return JsonSerializer.Deserialize<ObservableCollection<TodoItem>>(json) ?? new ObservableCollection<TodoItem>();
            }
            catch { }
        }
        return new ObservableCollection<TodoItem>();
    }

    private void SaveTodos()
    {
        var json = JsonSerializer.Serialize(_allTodos);
        File.WriteAllText(SaveFile, json);
    }

    private bool FilterTodo(object item)
    {
        if (item is TodoItem todo)
        {
            if (TodayOnlyCheck.IsChecked == true)
            {
                return todo.Date.Date == DateTime.Today;
            }
            return true;
        }
        return false;
    }

    private void Filter_Changed(object sender, RoutedEventArgs e)
    {
        _todoView?.Refresh();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        AddTodo();
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        var settings = new SettingsWindow { Owner = this };
        settings.ShowDialog();
        _todoView?.Refresh();
    }

    private void TodoInput_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            AddTodo();
        }
    }

    // Duration stepper state
    private int _durationHours = 0;
    private int _durationMinutes = 0;
    private static readonly int[] MinuteSteps = { 0, 15, 30, 45 };

    private void HourUp_Click(object sender, RoutedEventArgs e)
    {
        _durationHours = Math.Min(_durationHours + 1, 23);
        HourInput.Text = _durationHours.ToString();
    }

    private void HourDown_Click(object sender, RoutedEventArgs e)
    {
        _durationHours = Math.Max(_durationHours - 1, 0);
        HourInput.Text = _durationHours.ToString();
    }

    private void MinUp_Click(object sender, RoutedEventArgs e)
    {
        int idx = Array.IndexOf(MinuteSteps, _durationMinutes);
        _durationMinutes = MinuteSteps[(idx + 1) % MinuteSteps.Length];
        MinInput.Text = _durationMinutes.ToString("D2");
    }

    private void MinDown_Click(object sender, RoutedEventArgs e)
    {
        int idx = Array.IndexOf(MinuteSteps, _durationMinutes);
        _durationMinutes = MinuteSteps[(idx + MinuteSteps.Length - 1) % MinuteSteps.Length];
        MinInput.Text = _durationMinutes.ToString("D2");
    }

    private void AddTodo()
    {
        string text = TodoInput.Text.Trim();
        if (!string.IsNullOrEmpty(text))
        {
            int priority = 2;
            if (PriorityCombo.SelectedItem is ComboBoxItem cbi && int.TryParse(cbi.Tag?.ToString(), out int p))
                priority = p;

            double totalHours = _durationHours + _durationMinutes / 60.0;

            var newItem = new TodoItem
            {
                Text = text,
                Priority = priority,
                Date = DateTime.Today,
                IsDone = false,
                DurationHours = totalHours
            };

            _allTodos.Add(newItem);
            TodoInput.Clear();
            SaveTodos();
        }
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is TodoItem todoItem)
        {
            if (todoItem.IsRunning) return; // already has an active countdown

            todoItem.IsRunning = true;
            var countdownWindow = new CountdownWindow(todoItem);
            countdownWindow.Closed += (_, _) => todoItem.IsRunning = false;
            countdownWindow.Show();
        }
    }

    private void MarkDoneButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is TodoItem todoItem)
        {
            todoItem.IsDone = !todoItem.IsDone;
            SaveTodos();
        }
    }

    private void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is TodoItem todoItem)
        {
            _allTodos.Remove(todoItem);
            SaveTodos();
        }
    }

    private void Window_Closed(object? sender, EventArgs e)
    {
        SaveTodos();
    }
}