using CulinaryBlog.Application.Common.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ImageMagick;
namespace CulinaryBlog.Infrastructure.Services;

public class RecipeThumbnailJob(IApplicationDbContext db, IFileStorageService storage) : IRecipeThumbnailJob
{
    [AutomaticRetry(Attempts = 3)]
    public async Task ExecuteAsync(Guid imageId)
    {
        var entity = await db.RecipeImages.FirstOrDefaultAsync(x => x.Id == imageId);
        if (entity == null || entity.ThumbnailUrl != null && entity.MediumUrl != null) return;
        await using var source = await storage.OpenReadAsync(entity.OriginalUrl);
        using var original = new MagickImage(source);
        if ((ulong)original.Width * original.Height > 40000000) throw new InvalidDataException("Ảnh vượt quá 40 triệu điểm ảnh.");
        async Task<string> Make(int width, int height)
        {
            using var resized = original.Clone();
            resized.AutoOrient();
            resized.Resize(new MagickGeometry((uint)width, (uint)height) { FillArea = true });
            resized.Crop((uint)width, (uint)height, Gravity.Center);
            resized.ResetPage();
            resized.Strip();
            resized.Format = MagickFormat.WebP;
            resized.Quality = 82;
            await using var bytes = new MemoryStream();
            resized.Write(bytes);
            bytes.Position = 0;
            var file = new FormFile(bytes, 0, bytes.Length, "file", "thumbnail.webp") { Headers = new HeaderDictionary(), ContentType = "image/webp" };
            return await storage.UploadFileAsync(file, $"recipes/{entity.RecipeId}/thumbnails");
        }
        entity.ThumbnailUrl ??= await Make(300, 300);
        entity.MediumUrl ??= await Make(800, 600);
        await db.SaveChangesAsync(CancellationToken.None);
    }
}
