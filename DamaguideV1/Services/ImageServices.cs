namespace DamaguideV1.Services;

public class ImageServices
{
    public async Task<string> CopyToCacheAsync(FileResult photo)
    {
        string fileName = $"{Guid.NewGuid()}_{photo.FileName}";
        string destination = Path.Combine(
            FileSystem.CacheDirectory, fileName);

        await using Stream input = await photo.OpenReadAsync();
        await using FileStream output = File.Create(destination);

        await input.CopyToAsync(output);
        return destination;
    }
}
