using Microsoft.AspNetCore.Http;
using System.Text;
namespace CulinaryBlog.Application.Common.Helpers;
public static class FileValidationHelper
{
    public static async Task<(bool IsValid, string? ErrorMessage)> ValidateImageAsync(IFormFile file, long maxFileSizeInBytes = 5 * 1024 * 1024)
    {
        if (file == null || file.Length == 0) return (false, "File ảnh không được để trống.");
        if (file.Length > maxFileSizeInBytes) return (false, "Dung lượng ảnh vượt quá giới hạn 5 MB.");
        using var stream = file.OpenReadStream();
        var bytes = new byte[32]; var count = await stream.ReadAtLeastAsync(bytes, 12, throwOnEndOfStream: false);
        var valid = file.ContentType.ToLowerInvariant() switch
        {
            "image/jpeg" => count >= 3 && bytes[0] == 0xff && bytes[1] == 0xd8 && bytes[2] == 0xff,
            "image/png" => count >= 8 && bytes.AsSpan(0, 8).SequenceEqual(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
            "image/webp" => count >= 12 && Encoding.ASCII.GetString(bytes, 0, 4) == "RIFF" && Encoding.ASCII.GetString(bytes, 8, 4) == "WEBP",
            "image/avif" => count >= 16 && Encoding.ASCII.GetString(bytes, 4, 4) == "ftyp" && Encoding.ASCII.GetString(bytes, 8, count - 8).Contains("avif", StringComparison.Ordinal),
            _ => false
        };
        return valid ? (true, null) : (false, "Chỉ nhận JPEG, PNG, WebP, AVIF có nội dung đúng định dạng.");
    }
}
