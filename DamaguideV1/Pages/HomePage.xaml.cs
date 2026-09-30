using Microsoft.Maui.Media;

namespace DamaguideV1.Pages;

public partial class HomePage : ContentPage
{
    public HomePage() { InitializeComponent(); }

    private async void TakePhoto_Clicked(object sender, EventArgs e)
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
            await Navigation.PushAsync(new PreviewPage(photo));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Camera Error", ex.Message, "OK");
        }
    }

    private async void UploadPhoto_Clicked(object sender, EventArgs e)
    {
        try
        {
            var photo = await MediaPicker.Default.PickPhotoAsync();
            if (photo == null) return;
            await Navigation.PushAsync(new PreviewPage(photo));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Photo Error", ex.Message, "OK");
        }
    }
}
