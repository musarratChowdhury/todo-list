using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TodoList;

public class TodoItem : INotifyPropertyChanged
{
    private string _text = "";
    private bool _isDone;
    private int _priority;
    private DateTime _date;
    private double _durationHours;

    public string Text 
    { 
        get => _text; 
        set { _text = value; OnPropertyChanged(); } 
    }
    
    public bool IsDone 
    { 
        get => _isDone; 
        set { _isDone = value; OnPropertyChanged(); } 
    }
    
    public int Priority 
    { 
        get => _priority; 
        set { _priority = value; OnPropertyChanged(); } 
    }
    
    public DateTime Date 
    { 
        get => _date; 
        set { _date = value; OnPropertyChanged(); } 
    }

    public double DurationHours
    {
        get => _durationHours;
        set { _durationHours = value; OnPropertyChanged(); OnPropertyChanged(nameof(DurationLabel)); }
    }

    public string DurationLabel
    {
        get
        {
            if (_durationHours <= 0) return "";
            int h = (int)_durationHours;
            int m = (int)((_durationHours - h) * 60);
            if (h > 0 && m > 0) return $"⏱ {h}h {m}m";
            if (h > 0) return $"⏱ {h}h";
            return $"⏱ {m}m";
        }
    }

    // Transient — not persisted to JSON
    [System.Text.Json.Serialization.JsonIgnore]
    public bool IsRunning
    {
        get => _isRunning;
        set { _isRunning = value; OnPropertyChanged(); }
    }
    private bool _isRunning;

    public event PropertyChangedEventHandler? PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
