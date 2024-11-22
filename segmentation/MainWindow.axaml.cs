using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using segmentation.Helpers;

namespace segmentation;

public partial class MainWindow : Window {
    private IStorageFile _originalImage = null!;
    private Color _selectedColor = Color.FromArgb(255, 0, 0, 0);
    public MainWindow() {
        InitializeComponent();
    }
    
    private async void LoadImage_Click(object sender, RoutedEventArgs e) {
        var storageProvider = new Window().StorageProvider;
        var result = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions() {
            AllowMultiple = false,
            FileTypeFilter =  new []{ FilePickerFileTypes.ImageJpg, FilePickerFileTypes.ImagePng}
        });
        
        if (result.Count == 0)
            return;

        try {
            // Result is a list of chosen files (we allow only one file to be chosen)
            var stream = await result[0].OpenReadAsync();
            var bitmap = new Bitmap(stream);
            // Display the image in the UI
            DisplayedImage.Source = bitmap;
            _originalImage = result[0];
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during loading image: {ex.Message}");
        }
    }
    
    private async void DisplayedImage_PointerPressed(object sender, PointerPressedEventArgs e) {
        var point = e.GetPosition(DisplayedImage);
        var writeableBitmap = WriteableBitmap.Decode(await _originalImage.OpenReadAsync());
        // Color from ColorPicker
        _selectedColor = SelectedColor.Color;
        // Color from clicked point
        var color = PixelHelper.GetPixelColor(writeableBitmap, (int)point.X, (int)point.Y);
        
        var x = await Segmentation.FloodFill(_originalImage, point, color, _selectedColor);
        
        DisplayedImage.Source = x;
        
        Console.WriteLine($"Color at {point.X}, {point.Y}: {color}");
        Console.WriteLine(_selectedColor);
    }
}