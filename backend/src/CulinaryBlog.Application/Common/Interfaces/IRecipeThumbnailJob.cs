namespace CulinaryBlog.Application.Common.Interfaces;
public interface IRecipeThumbnailJob { Task ExecuteAsync(Guid imageId); }
