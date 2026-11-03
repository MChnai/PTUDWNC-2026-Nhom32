using CulinaryBlog.Application.Common.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
namespace CulinaryBlog.Infrastructure.Services;

public class FileStorageService(IWebHostEnvironment environment) : IFileStorageService
{
    private string Root => Path.GetFullPath(Path.Combine(environment.ContentRootPath, "wwwroot", "uploads"));
    private string Resolve(string relative)
    {
        if (!relative.StartsWith("/uploads/", StringComparison.Ordinal)) throw new ArgumentException("Không phải tệp tải lên của hệ thống.");
        var path = Path.GetFullPath(Path.Combine(Root, relative[9..]));
        if (!path.StartsWith(Root + Path.DirectorySeparatorChar, StringComparison.Ordinal)) throw new ArgumentException("Đường dẫn không hợp lệ.");
        return path;
    }
    public async Task<string> UploadFileAsync(IFormFile file, string folder, CancellationToken ct = default)
    {
        if (folder.Contains("..") || Path.IsPathRooted(folder)) throw new ArgumentException("Thư mục không hợp lệ.");
        var extension = file.ContentType switch { "image/png" => ".png", "image/webp" => ".webp", "image/avif" => ".avif", _ => ".jpg" };
        var relative = $"/uploads/{folder.Trim('/')}/{Guid.NewGuid():N}{extension}";
        var path = Resolve(relative);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        await using var output = File.Create(path);
        await file.CopyToAsync(output, ct);
        return relative;
    }
    public Task<Stream> OpenReadAsync(string path, CancellationToken ct = default) => Task.FromResult<Stream>(File.OpenRead(Resolve(path)));
    public Task DeleteFileAsync(string path, CancellationToken ct = default)
    {
        // Existing seed images and external references are never deleted.
        if (path.StartsWith("/uploads/", StringComparison.Ordinal)) File.Delete(Resolve(path));
        return Task.CompletedTask;
    }
}
