namespace DamaguideV1.Pages;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    // Public method so ResultsPage can re-trigger camera directly
    public async Task TriggerCameraAsync()
    {
        TakePhoto_Clicked(this, EventArgs.Empty);
    }

    private async void TakePhoto_Clicked(object? sender, EventArgs e)
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                await DisplayAlert("Camera unavailable",
                    "This device does not support camera capture.", "OK");
                return;
            }

            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo == null) return;

            // Push fresh photo into PreviewPage
            await Navigation.PushAsync(new PreviewPage(photo));
        }
        catch (PermissionException)
        {
            await DisplayAlert("Permission needed",
                "Please allow camera access in your phone's app settings.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Camera Error", ex.Message, "OK");
        }
    }

    private async void UploadPhoto_Clicked(object? sender, EventArgs e)
    {
        try
        {
            var photos = await MediaPicker.Default.PickPhotosAsync(
                new MediaPickerOptions { SelectionLimit = 1 });

            var photo = photos?.FirstOrDefault();
            if (photo == null) return;

            await Navigation.PushAsync(new PreviewPage(photo));
        }
        catch (PermissionException)
        {
            await DisplayAlert("Permission needed",
                "Please allow photo access in your phone's app settings.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Photo Error", ex.Message, "OK");
        }
    }
}