namespace DamaguideV1.Pages;

public partial class PreviewPage : ContentPage
{
    private readonly FileResult _photo;

    public PreviewPage(FileResult photo)
    {
        InitializeComponent();
        _photo = photo;
        PreviewImage.Source = ImageSource.FromFile(photo.FullPath);
    }

    private async void Analyze_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProcessingPage(_photo));
    }

    private async void ChooseAnother_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}
