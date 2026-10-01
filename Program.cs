using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Paste your OpenRouter key here
string openRouterApiKey = builder.Configuration["OPENROUTER_API_KEY"] ?? "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";

app.MapGet("/", () => "Damaguide OpenRouter Vision API is live!");

app.MapPost("/api/damage/analyze", async (HttpRequest request) =>
{
    try
    {
        var form = await request.ReadFormAsync();
        var file = form.Files.GetFile("image");

        if (file == null || file.Length == 0)
        {
            return Results.BadRequest(new { message = "No image file uploaded." });
        }

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        byte[] imageBytes = ms.ToArray();
        string base64Image = Convert.ToBase64String(imageBytes);
        string mimeType = string.IsNullOrEmpty(file.ContentType) ? "image/jpeg" : file.ContentType;

        var promptText = @"
Analyze the furniture item in this image for physical damage. 
Return ONLY a valid raw JSON object (without markdown code blocks or ```json tags) using exactly these keys:
{
  ""FurnitureType"": ""Type of furniture (e.g. Wooden Chair, Leather Sofa, Cabinet)"",
  ""DamageStatus"": ""Status (e.g. Damaged, Intact, Severely Broken)"",
  ""DamageLocation"": ""Specific area affected (e.g. Backrest, Front Left Leg, Armrest)"",
  ""DamageSeverity"": ""Severity rating (Low, Moderate, High, Critical)"",
  ""Confidence"": 90,
  ""RepairRecommendation"": ""Clear step-by-step instructions on how to repair this damage.""
}";

        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", openRouterApiKey.Trim());

        var payload = new
        {
            // openrouter/free automatically routes to available free vision models
            model = "openrouter/free", 
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = new object[]
                    {
                        new { type = "text", text = promptText },
                        new
                        {
                            type = "image_url",
                            image_url = new
                            {
                                url = $"data:{mimeType};base64,{base64Image}"
                            }
                        }
                    }
                }
            }
        };

        var response = await httpClient.PostAsJsonAsync("https://openrouter.ai/api/v1/chat/completions", payload);
        string responseString = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"[OpenRouter Error] Status: {response.StatusCode}\nDetails: {responseString}");
            return Results.Problem($"OpenRouter API error ({response.StatusCode}): {responseString}");
        }

        using var doc = JsonDocument.Parse(responseString);
        string aiText = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString() ?? "{}";

        // Clean any code block tags returned by the AI
        aiText = aiText.Replace("```json", "").Replace("```", "").Trim();

        Console.WriteLine($"[Raw AI Output]: {aiText}");

        // Safely parse or wrap string response if AI outputs plain text instead of raw JSON
        if (aiText.StartsWith("{") || aiText.StartsWith("["))
        {
            var parsedJson = JsonSerializer.Deserialize<object>(aiText);
            return Results.Ok(parsedJson);
        }
        else
        {
            return Results.Ok(new
            {
                FurnitureType = "Analyzed Item",
                DamageStatus = "Analyzed",
                DamageLocation = "See Details",
                DamageSeverity = "Moderate",
                Confidence = 85,
                RepairRecommendation = aiText
            });
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[Server Exception] {ex.Message}\n{ex.StackTrace}");
        return Results.Problem($"Internal Server Exception: {ex.Message}");
    }
});

app.Run();