using Microsoft.AspNetCore.Http;
namespace CulinaryBlog.Application.Common.Interfaces;
public interface IImageInspector { Task<string?> ValidateAsync(IFormFile file, CancellationToken ct = default); }
