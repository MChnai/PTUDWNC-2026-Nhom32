using CulinaryBlog.Application.Common.Interfaces;
using CulinaryBlog.Application.Common.Models;
using MediatR;
namespace CulinaryBlog.Application.Features.Files.Commands.DeleteFile;
public class DeleteFileCommandHandler(IFileStorageService storage) : IRequestHandler<DeleteFileCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteFileCommand request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.FilePathOrUrl)) return Result<bool>.Failure("INVALID_FILE", "Cần đường dẫn tệp để xóa.");
        await storage.DeleteFileAsync(request.FilePathOrUrl.Trim(), ct);
        return Result<bool>.Success(true);
    }
}
