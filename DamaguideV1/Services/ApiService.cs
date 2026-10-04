using System.Text.Json;

namespace DamaguideV1.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    
    private const string BaseUrl = "https://damaguide-final.onrender.com"; 

    public ApiService()
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(60)
        };
    }

    public async Task<string> AnalyzeImageAsync(byte[] imageBytes)
    {
        using var content = new MultipartFormDataContent();
        var byteArrayContent = new ByteArrayContent(imageBytes);
        byteArrayContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

        content.Add(byteArrayContent, "image", "furniture.jpg");

        var response = await _httpClient.PostAsync($"{BaseUrl}/api/damage/analyze", content);

        if (!response.IsSuccessStatusCode)
        {
            var errorDetails = await response.Content.ReadAsStringAsync();
            throw new Exception($"Server error {response.StatusCode}: {errorDetails}");
        }

        return await response.Content.ReadAsStringAsync();
    }
}