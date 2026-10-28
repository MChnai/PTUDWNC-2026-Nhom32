namespace CulinaryBlog.Application.Features.Recipes.DTOs;

public class RecipeDetailDto
{
    // Thông tin cơ bản
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Instructions { get; set; }
    public string? Notes { get; set; }

    // Thời gian và độ khó
    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public int Servings { get; set; }
    public int Difficulty { get; set; }

    // Trạng thái
    public int Status { get; set; }
    public DateTime? PublishedAt { get; set; }

    // Thống kê
    public int ViewCount { get; set; }
    public decimal RatingAverage { get; set; }
    public int RatingCount { get; set; }

    // Danh mục
    public Guid CategoryId { get; set; }
    public RecipeDetailCategoryDto? Category { get; set; }

    // Tác giả
    public Guid AuthorId { get; set; }
    public RecipeDetailAuthorDto? Author { get; set; }

    // Dữ liệu chi tiết
    public List<RecipeDetailIngredientDto> Ingredients { get; set; } = [];
    public List<RecipeDetailStepDto> Steps { get; set; } = [];
    public List<RecipeDetailImageDto> Images { get; set; } = [];

    public RecipeDetailNutritionDto? Nutrition { get; set; }

    // Metadata
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // PostgreSQL xmin - Optimistic Concurrency
    public uint RowVersion { get; set; }
}


// Danh mục
public record RecipeDetailCategoryDto(
    Guid Id,
    string Name,
    string Slug
);


// Tác giả
public record RecipeDetailAuthorDto(
    Guid Id,
    string DisplayName,
    string? AvatarUrl,
    string? Bio
);


// Nguyên liệu
public record RecipeDetailIngredientDto(
    Guid Id,
    Guid RecipeId,
    string Name,
    decimal? Quantity,
    string? Unit,
    string? Notes,
    int OrderIndex
);


// Bước thực hiện
public record RecipeDetailStepDto(
    Guid Id,
    Guid RecipeId,
    int StepNumber,
    string Title,
    string Description,
    int? TimerMinutes,
    string? ImageUrl
);


// Hình ảnh
public record RecipeDetailImageDto(
    Guid Id,
    Guid RecipeId,
    string OriginalUrl,
    string? ThumbnailUrl,
    string? AltText,
    bool IsPrimary,
    int OrderIndex
);


// Dinh dưỡng
public record RecipeDetailNutritionDto(
    int Calories,
    decimal Protein,
    decimal Carbohydrates,
    decimal Fat,
    decimal? Fiber,
    decimal? Sugar,
    decimal? Sodium
);