using System.Globalization;
using System.Text;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace WinterSlopeTool.WinUI3;

public sealed partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private async void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        var picker = new FileOpenPicker
        {
            SuggestedStartLocation = PickerLocationId.DocumentsLibrary
        };
        picker.FileTypeFilter.Add(".lua");
        picker.FileTypeFilter.Add("*");

        var hwnd = WindowNative.GetWindowHandle(this);
        InitializeWithWindow.Initialize(picker, hwnd);

        var file = await picker.PickSingleFileAsync();
        if (file is not null)
        {
            SaveFileBox.Text = file.Path;
            ShowStatus("Ready.", InfoBarSeverity.Informational);
        }
    }

    private async void AddButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var filePath = SaveFileBox.Text.Trim();
            if (!File.Exists(filePath))
            {
                ShowStatus("Choose an existing .lua save file first.", InfoBarSeverity.Warning);
                return;
            }

            var slope = new SlopeInfo(
                SlopeBox.Text.Trim(),
                ToInt(InfoLayerBox.Value),
                NameBox.Text.Trim(),
                FromBox.Text.Trim(),
                ToBox.Text.Trim(),
                DifficultyBox.SelectedIndex + 1,
                ToInt(LengthBox.Value),
                ToInt(CapacityBox.Value));

            var validationError = slope.Validate();
            if (validationError is not null)
            {
                ShowStatus(validationError, InfoBarSeverity.Warning);
                return;
            }

            var original = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
            var updated = SlopeFileEditor.InsertSlope(original, slope);
            var backupPath = filePath + ".backup-" + DateTime.Now.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture);

            File.Copy(filePath, backupPath, overwrite: false);
            await File.WriteAllTextAsync(filePath, updated, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

            ShowStatus("Added slope and created backup: " + Path.GetFileName(backupPath), InfoBarSeverity.Success);
        }
        catch (Exception ex)
        {
            ShowStatus(ex.Message, InfoBarSeverity.Error);
        }
    }

    private static int ToInt(double value)
    {
        if (double.IsNaN(value))
        {
            return 0;
        }

        return Convert.ToInt32(value, CultureInfo.InvariantCulture);
    }

    private void ShowStatus(string message, InfoBarSeverity severity)
    {
        StatusBar.Message = message;
        StatusBar.Severity = severity;
        StatusBar.IsOpen = true;
    }
}
