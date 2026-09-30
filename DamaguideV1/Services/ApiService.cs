using System.Net.Http.Headers;
using System.Text.Json;
using DamaguideV1.Models;

namespace DamaguideV1.Services;

public class ApiService
{
    private readonly HttpClient _httpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(120) // Important para sa Render free na natutulog
    };

    public async Task<DamageAnalysis> AnalyzeImageAsync(string imagePath)
    {
        using var content = new MultipartFormDataContent();
        await using var fileStream = File.OpenRead(imagePath);
        var imageContent = new StreamContent(fileStream);
        imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
        content.Add(imageContent, "file", Path.GetFileName(imagePath));

        var response = await _httpClient.PostAsync("https://damaguide-api.onrender.com/analyze", content);
        
        string json = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"API ERROR {response.StatusCode}: {json}");
            throw new Exception($"Server error {response.StatusCode}: {json}");
        }

        var result = JsonSerializer.Deserialize<DamageAnalysis>( 
            json, 
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (result == null) throw new Exception("The AI returned an invalid result. Raw: " + json);

        return result;
    }
}