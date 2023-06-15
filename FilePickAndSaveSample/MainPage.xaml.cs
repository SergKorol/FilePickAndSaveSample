using System.Text;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Storage;

namespace FilePickAndSaveSample;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }
    
    private async void SaveFile_Clicked(object sender, EventArgs e)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;
        
        try
        {
            await SaveFile(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw new OperationCanceledException("It was cancelled");
        }
    }
    
    private async Task SaveFile(CancellationToken cancellationToken)
    {
        using var stream = new MemoryStream(Encoding.Default.GetBytes("Hello from the Dev.to Community!"));
        var fileName = Application.Current?.MainPage?.DisplayPromptAsync("Type file name", "Choose filename") ?? Task.FromResult("text.txt");
        var fileSaverResult = await FileSaver.Default.SaveAsync(await fileName, stream, cancellationToken);
        if (fileSaverResult.IsSuccessful)
        {
            await Toast.Make($"The file was saved successfully to location: {fileSaverResult.FilePath}").Show(cancellationToken);
        }
        else
        {
            await Toast.Make($"The file was not saved successfully with error: {fileSaverResult.Exception.Message}").Show(cancellationToken);
        }
    }
    
    private async void PickFolder_Clicked(object sender, EventArgs e)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken = cancellationTokenSource.Token;
        
        try
        {
            await PickFolder(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw new OperationCanceledException("It was cancelled");
        }
    }
    
    private async Task PickFolder(CancellationToken cancellationToken)
    {
        var folderResult = await FolderPicker.PickAsync("DCIM", cancellationToken);
        if (folderResult.IsSuccessful)
        {
            var filesCount = Directory.EnumerateFiles(folderResult.Folder.Path).Count();
            await Toast.Make($"Folder picked: Name - {folderResult.Folder.Name}, Path - {folderResult.Folder.Path}, Files count - {filesCount}", ToastDuration.Long).Show(cancellationToken);
        }
        else
        {
            await Toast.Make($"Folder is not picked, {folderResult.Exception.Message}").Show(cancellationToken);
        }
    }
    
    private async void PickPhoto_Clicked(object sender, EventArgs e)
    {
        FileResult result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Pick a Photo",
            FileTypes = FilePickerFileType.Images
        });

        if (result == null) return;

        FileName.Text = result.FileName;
    }

    private async void PickFile_Clicked(object sender, EventArgs e)
    {
        var customFileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            {
                DevicePlatform.iOS, new[]
                {

                    "com.microsoft.word.doc",
                    "public.plain-text",
                    "org.openxmlformats.wordprocessingml.document"
                }
            },
            {
                DevicePlatform.Android, new[]
                {
                    "application/msword",
                    "text-plain",
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                }
            },
            {
                DevicePlatform.WinUI, new[]
                {
                    "doc","docx", "txt"
                }
            },
        });


        FileResult result = await FilePicker.PickAsync(new PickOptions
        {
            PickerTitle = "Pick a File",
            FileTypes = customFileTypes
        });

        if (result == null) return;

        FileName.Text = result.FileName;
    }
}