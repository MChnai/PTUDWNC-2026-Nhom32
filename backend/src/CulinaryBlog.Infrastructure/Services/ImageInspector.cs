using CulinaryBlog.Application.Common.Helpers;
using CulinaryBlog.Application.Common.Interfaces;
using ImageMagick;
using Microsoft.AspNetCore.Http;
namespace CulinaryBlog.Infrastructure.Services;
public class ImageInspector : IImageInspector
{
    public async Task<string?> ValidateAsync(IFormFile file, CancellationToken ct = default)
    {
        var (valid, error) = await FileValidationHelper.ValidateImageAsync(file);
        if (!valid) return error;
        try
        {
            await using var stream = file.OpenReadStream();
            var info = new MagickImageInfo(stream);
            if (info.Width == 0 || info.Height == 0 || info.Width > 12000 || info.Height > 12000 || (ulong)info.Width * info.Height > 40000000)
                return "Ảnh tối đa 12000 px mỗi chiều và 40 triệu điểm ảnh.";
            return null;
        }
        catch (MagickException) { return "Tệp ảnh bị hỏng hoặc không giải mã được."; }
    }
}
