using CulinaryBlog.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
namespace CulinaryBlog.Infrastructure.Storage;

public class MinioFileStorageService(IMinioClient client, IOptions<MinioOptions> options) : IFileStorageService
{
    private MinioOptions Settings => options.Value;
    private string Prefix => Settings.CustomDomain.TrimEnd('/') + "/" + Settings.DefaultBucket + "/";
    private string? Key(string url) => url.StartsWith(Prefix, StringComparison.Ordinal) ? url[Prefix.Length..] : null;
    public async Task<string> UploadFileAsync(IFormFile file, string folder, CancellationToken ct = default)
    {
        if (folder.Contains("..") || folder.StartsWith('/')) throw new ArgumentException("Thư mục không hợp lệ.");
        var ext = file.ContentType switch { "image/png" => ".png", "image/webp" => ".webp", "image/avif" => ".avif", _ => ".jpg" };
        var key = $"{folder.Trim('/')}/{Guid.NewGuid():N}{ext}";
        await using var stream = file.OpenReadStream();
        await client.PutObjectAsync(new PutObjectArgs().WithBucket(Settings.DefaultBucket).WithObject(key)
            .WithStreamData(stream).WithObjectSize(file.Length).WithContentType(file.ContentType), ct);
        return Prefix + key;
    }
    public async Task<Stream> OpenReadAsync(string url, CancellationToken ct = default)
    {
        var key = Key(url) ?? throw new ArgumentException("Tệp không thuộc kho ảnh của hệ thống.");
        var output = new MemoryStream();
        try
        {
            await client.GetObjectAsync(new GetObjectArgs().WithBucket(Settings.DefaultBucket).WithObject(key)
                .WithCallbackStream(stream => stream.CopyTo(output)), ct);
            output.Position = 0;
            return output;
        }
        catch { output.Dispose(); throw; }
    }
    public async Task DeleteFileAsync(string url, CancellationToken ct = default)
    {
        var key = Key(url);
        if (key != null && !key.Contains(".."))
            await client.RemoveObjectAsync(new RemoveObjectArgs().WithBucket(Settings.DefaultBucket).WithObject(key), ct);
    }
}
