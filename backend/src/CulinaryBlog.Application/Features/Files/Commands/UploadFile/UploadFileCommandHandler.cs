using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.Common.Models;
using CulinaryBlog.Application.Features.Files.DTOs;
using MediatR;
namespace CulinaryBlog.Application.Features.Files.Commands.UploadFile;
public class UploadFileCommandHandler(IFileStorageService storage, IImageInspector inspector) : IRequestHandler<UploadFileCommand, Result<FileUploadedDto>>
{
    public async Task<Result<FileUploadedDto>> Handle(UploadFileCommand request, CancellationToken ct)
    {
        var error = await inspector.ValidateAsync(request.File, ct);
        if (error != null) return Result<FileUploadedDto>.Failure("INVALID_FILE", error);
        var url = await storage.UploadFileAsync(request.File, "media", ct);
        return Result<FileUploadedDto>.Success(new FileUploadedDto { FileName = Path.GetFileName(url), Url = url, ContentType = request.File.ContentType, Size = request.File.Length });
    }
}
