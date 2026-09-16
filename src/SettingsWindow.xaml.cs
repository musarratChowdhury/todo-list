using System;
using System.Windows;
using System.Windows.Controls;

namespace TodoList;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        // Pre-select current setting without triggering the Checked handler yet
        RadioSmall.IsChecked  = AppSettings.Instance.DockedWindowSize == DockedSize.Small;
        RadioMedium.IsChecked = AppSettings.Instance.DockedWindowSize == DockedSize.Medium;
        RadioLarge.IsChecked  = AppSettings.Instance.DockedWindowSize == DockedSize.Large;

        HighColorInput.Text = AppSettings.Instance.HighColor;
        MedColorInput.Text = AppSettings.Instance.MedColor;
        LowColorInput.Text = AppSettings.Instance.LowColor;
    }

    private DockedSize _pending = AppSettings.Instance.DockedWindowSize;

    private void Size_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is RadioButton rb && rb.Tag is string tag)
            _pending = Enum.Parse<DockedSize>(tag);
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        AppSettings.Instance.DockedWindowSize = _pending;
        AppSettings.Instance.HighColor = HighColorInput.Text.Trim();
        AppSettings.Instance.MedColor = MedColorInput.Text.Trim();
        AppSettings.Instance.LowColor = LowColorInput.Text.Trim();
        AppSettings.Instance.Save();
        Close();
    }
}
