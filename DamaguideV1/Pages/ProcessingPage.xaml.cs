using System.Text.Json;
using DamaguideV1.Models;
using DamaguideV1.Services;
using Microsoft.Maui.Storage;

namespace DamaguideV1.Pages;

public partial class ProcessingPage : ContentPage
{
    private readonly ApiService _apiService;
    private readonly FileResult _fileResult;

    public ProcessingPage(FileResult fileResult)
    {
        InitializeComponent();
        _fileResult = fileResult;
        _apiService = new ApiService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await StartAnalysisAsync();
    }

    private async Task StartAnalysisAsync()
    {
        try
        {
            if (_fileResult == null) return;

            using var stream = await _fileResult.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();

            string jsonResponse = await _apiService.AnalyzeImageAsync(imageBytes);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            DamageAnalysis analysisResult = JsonSerializer.Deserialize<DamageAnalysis>(jsonResponse, options);

            if (analysisResult != null)
            {
                await Navigation.PushAsync(new ResultsPage(analysisResult));
            }
            else
            {
                throw new Exception("Failed to parse analysis response.");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Analysis Error", ex.Message, "OK");
            await Navigation.PopAsync();
        }
    }
}