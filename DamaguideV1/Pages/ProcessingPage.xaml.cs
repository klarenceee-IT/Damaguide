using DamaguideV1.Services;

namespace DamaguideV1.Pages;

public partial class ProcessingPage : ContentPage
{
    private readonly FileResult _photo;

    public ProcessingPage(FileResult photo)
    {
        InitializeComponent();
        _photo = photo;
        StartAnalysis();
    }

    private async void StartAnalysis()
    {
        try
        {
            var imageServices = new ImageServices();
            string imagePath =
                await imageServices.CopyToCacheAsync(_photo);

            var api = new ApiService();
            var result =
                await api.AnalyzeImageAsync(imagePath);

            await Navigation.PushAsync(new ResultsPage(result));
            Navigation.RemovePage(this);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Analysis Error", ex.Message, "OK");
            await Navigation.PopAsync();
        }
    }
}
