using System;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

namespace segmentation;

public partial class MainWindow : Window {
    private Image _originalImage = null!;
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
            var stream = await result[0].OpenReadAsync();
            var bitmap = new Bitmap(stream);
            DisplayedImage.Source = bitmap; 
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during loading image: {ex.Message}");
        }
    }
}