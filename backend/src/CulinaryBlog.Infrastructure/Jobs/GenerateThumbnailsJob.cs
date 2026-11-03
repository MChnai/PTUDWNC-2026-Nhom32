using CulinaryBlog.Application.Common.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;
using ImageMagick;

namespace CulinaryBlog.Infrastructure.Jobs;

public class GenerateThumbnailsJob
{
    private readonly IStorageService _storageService;
    private readonly IMinioClient _minioClient;
    private readonly ILogger<GenerateThumbnailsJob> _logger;

    public GenerateThumbnailsJob(
        IStorageService storageService,
        IMinioClient minioClient,
        ILogger<GenerateThumbnailsJob> logger)
    {
        _storageService = storageService;
        _minioClient = minioClient;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Fail)]
    public async Task ExecuteAsync(string originalFileName, string bucketName)
    {
        _logger.LogInformation("Đang sinh Thumbnail cho ảnh: {FileName}", originalFileName);

        using var originalStream = new MemoryStream();
        var getObjectArgs = new GetObjectArgs()
            .WithBucket(bucketName)
            .WithObject(originalFileName)
            .WithCallbackStream(stream => stream.CopyTo(originalStream));

        await _minioClient.GetObjectAsync(getObjectArgs);

        if (originalStream.Length == 0)
        {
            _logger.LogWarning("Tệp ảnh rỗng hoặc không tồn tại: {FileName}", originalFileName);
            return;
        }

        var sizes = new (int Width, int Height, string Suffix)[]
        {
            (300, 300, "300x300"),
            (800, 600, "800x600")
        };

        var extension = Path.GetExtension(originalFileName);
        var baseName = Path.GetFileNameWithoutExtension(originalFileName);

        foreach (var (width, height, suffix) in sizes)
        {
            originalStream.Position = 0;

            using var image = new MagickImage(originalStream);
            image.AutoOrient();
            image.Resize((uint)width, (uint)height);
            image.Strip();
            image.Format = MagickFormat.WebP;
            using var thumbStream = new MemoryStream();
            image.Write(thumbStream);
            thumbStream.Position = 0;

            var thumbnailFileName = $"{baseName}_{suffix}.webp";

            IFormFile formFile = new FormFile(
                baseStream: thumbStream,
                baseStreamOffset: 0,
                length: thumbStream.Length,
                name: "file",
                fileName: thumbnailFileName
            )
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/webp"
            };

            var thumbnailUrl = await _storageService.UploadFileAsync(formFile, bucketName);

            _logger.LogInformation("Sinh thành công Thumbnail [{Suffix}]: {Url}", suffix, thumbnailUrl);
        }
    }
}