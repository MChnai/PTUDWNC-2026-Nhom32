# Danh sách file thay đổi — Culinary Blog 2.0

Đối chiếu trực tiếp từng file với ZIP gốc đính kèm, bằng SHA-256. Các đường dẫn tính từ thư mục dự án.

| Phạm vi | Sửa | Thêm | Xóa |
| --- | ---: | ---: | ---: |
| Source/tài liệu/cấu hình/tài nguyên | 77 | 109 | 54 |
| Toàn bộ file, kể cả build/cache cũ | 77 | 109 | 425 |

Có 371 file build/cache cũ được loại khỏi gói source. Danh sách này mô tả thay đổi trong bản ZIP bàn giao; không phải thao tác xóa dữ liệu trên máy hoặc database đang dùng của bạn.

## Nội dung nâng cấp

- Frontend Next.js App Router với giao diện tiếng Việt nền kem/xanh olive; trang chủ, danh mục, tìm kiếm, học nấu ăn, chi tiết, sổ tay, hồ sơ và góc tác giả.
- Luồng công thức nguyên tử, kiểm tra quyền sở hữu, publish/archive, ảnh và metadata SEO; giữ backend Clean Architecture/.NET 10/PostgreSQL.
- Auth qua BFF/cookie HttpOnly, token hashing/rotation, Identity password hasher tương thích hash cũ, rate limit và kiểm tra ảnh.
- Bổ sung Docker stack, cấu hình dịch vụ, hướng dẫn nâng cấp, test và bảng đối chiếu SRS. Xem VERIFICATION để phân biệt phần đã chạy và phần chưa nghiệm thu.

## File được sửa

| Đường dẫn | Mục đích |
| --- | --- |
| `.env.example` | Cập nhật cho bản nâng cấp v2. |
| `.gitignore` | Cập nhật cho bản nâng cấp v2. |
| `backend/CulinaryBlog.sln` | Đồng bộ dependency/cấu hình dự án. |
| `backend/CulinaryBlog.slnx` | Đồng bộ dependency/cấu hình dự án. |
| `backend/README.md` | Tài liệu nâng cấp, đối chiếu yêu cầu và kết quả kiểm chứng. |
| `backend/docker-compose.yml` | Khởi động, CI hoặc đóng gói hệ thống. |
| `backend/src/CulinaryBlog.API/CulinaryBlog.API.csproj` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.API/Dockerfile` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.API/Endpoints/AuthEndpoints.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.API/Endpoints/CategoryEndpoints.cs` | Danh mục, validation, count public và phân trang. |
| `backend/src/CulinaryBlog.API/Endpoints/FileEndpoints.cs` | Upload/delete theo storage provider và quyền phù hợp. |
| `backend/src/CulinaryBlog.API/Endpoints/HealthCheckEndpoints.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.API/Endpoints/JobEndpoints.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.API/Endpoints/RecipeEndpoints.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.API/Middleware/ExceptionHandlingMiddleware.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.API/Middlewares/CorrelationIdMiddleware.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.API/Models/Requests/UpdateRecipeRequest.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.API/Program.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.API/appsettings.Development.json` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.API/appsettings.json` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Application/Common/Helpers/FileValidationHelper.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Application/Common/Interfaces/IFileStorageService.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Application/CulinaryBlog.Application.csproj` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Application/DependencyInjection.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Application/Features/Auth/Commands/GoogleLogin/GoogleLoginCommand.cs` | Xác thực/validation/rotation, hash tương thích và bảo vệ token. |
| `backend/src/CulinaryBlog.Application/Features/Auth/Commands/Login/LoginCommandHandler.cs` | Xác thực/validation/rotation, hash tương thích và bảo vệ token. |
| `backend/src/CulinaryBlog.Application/Features/Auth/Commands/Logout/LogoutCommand.cs` | Xác thực/validation/rotation, hash tương thích và bảo vệ token. |
| `backend/src/CulinaryBlog.Application/Features/Auth/Commands/RefreshToken/RefreshTokenCommand.cs` | Xác thực/validation/rotation, hash tương thích và bảo vệ token. |
| `backend/src/CulinaryBlog.Application/Features/Auth/Commands/Register/RegisterCommand.cs` | Xác thực/validation/rotation, hash tương thích và bảo vệ token. |
| `backend/src/CulinaryBlog.Application/Features/Auth/Commands/Register/RegisterCommandValidator.cs` | Xác thực/validation/rotation, hash tương thích và bảo vệ token. |
| `backend/src/CulinaryBlog.Application/Features/Categories/Queries/GetCategories/GetCategoriesQuery.cs` | Danh mục, validation, count public và phân trang. |
| `backend/src/CulinaryBlog.Application/Features/Categories/Queries/GetCategoryBySlug/GetCategoryBySlugQuery.cs` | Danh mục, validation, count public và phân trang. |
| `backend/src/CulinaryBlog.Application/Features/Files/Commands/DeleteFile/DeleteFileCommandHandler.cs` | Upload/delete theo storage provider và quyền phù hợp. |
| `backend/src/CulinaryBlog.Application/Features/Files/Commands/UploadFile/UploadFileCommandHandler.cs` | Upload/delete theo storage provider và quyền phù hợp. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Commands/AddIngredient/AddIngredientCommand.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Commands/AddStep/AddRecipeStepCommand.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Commands/ArchiveRecipe/ArchiveRecipeCommand.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Commands/ChangeRecipeStatus/ChangeRecipeStatusCommand.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Commands/CreateRecipe/CreateRecipeCommand.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Commands/CreateRecipe/CreateRecipeCommandHandler.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Commands/CreateRecipe/CreateRecipeCommandValidator.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Commands/DeleteIngredient/DeleteIngredientCommand.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Commands/DeleteRecipe/DeleteRecipeCommand.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Commands/DeleteRecipeImage/DeleteRecipeImageCommand.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Commands/DeleteStep/DeleteRecipeStepCommand.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Commands/PublishRecipe/PublishRecipeCommand.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Commands/SetPrimaryImage/SetPrimaryRecipeImageCommand.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Commands/UpdateIngredient/UpdateIngredientCommand.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Commands/UpdateRecipe/UpdateRecipeCommand.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Commands/UpdateStep/UpdateRecipeStepCommand.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Commands/UploadRecipeImage/UploadRecipeImageCommand.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/DTOs/RecipeDto.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Queries/GetManagedRecipes/GetManagedRecipesQueryHandler.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Queries/GetRecipes/GetRecipesQuery.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Queries/GetRecipes/GetRecipesQueryHandler.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Queries/SearchRecipes/SearchRecipesQueryHandler.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Domain/CulinaryBlog.Domain.csproj` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Domain/Entities/Recipe.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Domain/Entities/RecipeImage.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Infrastructure/CulinaryBlog.Infrastructure.csproj` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Infrastructure/DependencyInjection.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Infrastructure/Jobs/GenerateThumbnailsJob.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Infrastructure/Jobs/SitemapJob.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Infrastructure/Persistence/ApplicationDbContext.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Infrastructure/Persistence/Configurations/RecipeImageConfiguration.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Infrastructure/Services/EmailService.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Infrastructure/Services/FileCleanupService.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Infrastructure/Services/FileStorageService.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Infrastructure/Services/JwtService.cs` | Xác thực/validation/rotation, hash tương thích và bảo vệ token. |
| `backend/src/CulinaryBlog.Infrastructure/Services/PasswordHasher.cs` | Xác thực/validation/rotation, hash tương thích và bảo vệ token. |
| `frontend/README.md` | Tài liệu nâng cấp, đối chiếu yêu cầu và kết quả kiểm chứng. |
| `frontend/package.json` | Đồng bộ dependency/cấu hình dự án. |
| `frontend/src/components/Header.tsx` | Giao diện và luồng sử dụng thực tế: học, nấu, xác thực, tác giả. |
| `frontend/src/components/RecipeCard.tsx` | Giao diện và luồng sử dụng thực tế: học, nấu, xác thực, tác giả. |
| `frontend/tsconfig.json` | Đồng bộ dependency/cấu hình dự án. |
| `package-lock.json` | Đồng bộ dependency/cấu hình dự án. |
| `package.json` | Đồng bộ dependency/cấu hình dự án. |

## File được thêm

| Đường dẫn | Mục đích |
| --- | --- |
| `.dockerignore` | Cập nhật cho bản nâng cấp v2. |
| `.github/workflows/ci.yml` | Khởi động, CI hoặc đóng gói hệ thống. |
| `CHANGES.md` | Tài liệu nâng cấp, đối chiếu yêu cầu và kết quả kiểm chứng. |
| `FILE_CHANGES.json` | Đồng bộ dependency/cấu hình dự án. |
| `README.md` | Tài liệu nâng cấp, đối chiếu yêu cầu và kết quả kiểm chứng. |
| `backend/.dockerignore` | Cập nhật cho bản nâng cấp v2. |
| `backend/scripts/04_upgrade_v2.sql` | Cập nhật cho bản nâng cấp v2. |
| `backend/src/CulinaryBlog.API/Data/starter-catalog.json` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.API/Endpoints/ApiResults.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.API/Middleware/AdminDashboardAuthorization.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Application/Common/Behaviors/LoggingBehavior.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Application/Common/Helpers/TokenHash.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Application/Common/Interfaces/IImageInspector.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Application/Common/Interfaces/IRecipeThumbnailJob.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Application/Features/Categories/Commands/CategoryValidators.cs` | Danh mục, validation, count public và phân trang. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Commands/ChildCommandValidators.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Commands/RecordRecipeView/RecordRecipeViewCommand.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/Commands/UpdateRecipe/UpdateRecipeCommandValidator.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/DTOs/RecipeContent.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Application/Features/Recipes/DTOs/RecipeProjection.cs` | API công thức, visibility/ownership, cập nhật nguyên tử và trạng thái. |
| `backend/src/CulinaryBlog.Infrastructure/Persistence/Seeders/StarterCatalogSeeder.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Infrastructure/Services/ImageInspector.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Infrastructure/Services/RecipeThumbnailJob.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/src/CulinaryBlog.Infrastructure/Storage/MinioFileStorageService.cs` | Cấu hình/hạ tầng, lưu trữ, job, quan sát hoặc tính toàn vẹn dữ liệu. |
| `backend/tests/CulinaryBlog.Tests/BusinessRulesTests.cs` | Kiểm thử hồi quy và kịch bản nghiệm thu. |
| `backend/tests/CulinaryBlog.Tests/CulinaryBlog.Tests.csproj` | Kiểm thử hồi quy và kịch bản nghiệm thu. |
| `deploy/Dockerfile.minio` | Khởi động, CI hoặc đóng gói hệ thống. |
| `docker-compose.yml` | Khởi động, CI hoặc đóng gói hệ thống. |
| `docs/SRS_COVERAGE.md` | Tài liệu nâng cấp, đối chiếu yêu cầu và kết quả kiểm chứng. |
| `docs/THIRD_PARTY.md` | Tài liệu nâng cấp, đối chiếu yêu cầu và kết quả kiểm chứng. |
| `docs/UPGRADE_GUIDE.md` | Tài liệu nâng cấp, đối chiếu yêu cầu và kết quả kiểm chứng. |
| `docs/VERIFICATION.md` | Tài liệu nâng cấp, đối chiếu yêu cầu và kết quả kiểm chứng. |
| `frontend/.env.example` | Cập nhật cho bản nâng cấp v2. |
| `frontend/Dockerfile` | Khởi động, CI hoặc đóng gói hệ thống. |
| `frontend/next-env.d.ts` | Cập nhật cho bản nâng cấp v2. |
| `frontend/next.config.ts` | Cập nhật cho bản nâng cấp v2. |
| `frontend/public/fonts/be-vietnam-pro-latin-400-normal.woff2` | Font tiếng Việt cục bộ và license. |
| `frontend/public/fonts/be-vietnam-pro-latin-600-normal.woff2` | Font tiếng Việt cục bộ và license. |
| `frontend/public/fonts/be-vietnam-pro-latin-700-normal.woff2` | Font tiếng Việt cục bộ và license. |
| `frontend/public/fonts/be-vietnam-pro-vietnamese-400-normal.woff2` | Font tiếng Việt cục bộ và license. |
| `frontend/public/fonts/be-vietnam-pro-vietnamese-600-normal.woff2` | Font tiếng Việt cục bộ và license. |
| `frontend/public/fonts/be-vietnam-pro-vietnamese-700-normal.woff2` | Font tiếng Việt cục bộ và license. |
| `frontend/public/fonts/fontsource-be-vietnam-pro-5.3.0-LICENSE.txt` | Font tiếng Việt cục bộ và license. |
| `frontend/public/fonts/fontsource-lora-5.3.0-LICENSE.txt` | Font tiếng Việt cục bộ và license. |
| `frontend/public/fonts/lora-latin-400-italic.woff2` | Font tiếng Việt cục bộ và license. |
| `frontend/public/fonts/lora-latin-400-normal.woff2` | Font tiếng Việt cục bộ và license. |
| `frontend/public/fonts/lora-vietnamese-400-italic.woff2` | Font tiếng Việt cục bộ và license. |
| `frontend/public/fonts/lora-vietnamese-400-normal.woff2` | Font tiếng Việt cục bộ và license. |
| `frontend/public/images/ca-ri-ga.webp` | Ảnh món ăn WebP hoặc SVG phục vụ thiết kế mới. |
| `frontend/public/images/hero.webp` | Ảnh món ăn WebP hoặc SVG phục vụ thiết kế mới. |
| `frontend/public/images/pasta-pesto.webp` | Ảnh món ăn WebP hoặc SVG phục vụ thiết kế mới. |
| `frontend/public/images/pizza-rau-cu.webp` | Ảnh món ăn WebP hoặc SVG phục vụ thiết kế mới. |
| `frontend/public/images/placeholder.svg` | Ảnh món ăn WebP hoặc SVG phục vụ thiết kế mới. |
| `frontend/public/images/ramen-trung.webp` | Ảnh món ăn WebP hoặc SVG phục vụ thiết kế mới. |
| `frontend/public/images/salad-cau-vong.webp` | Ảnh món ăn WebP hoặc SVG phục vụ thiết kế mới. |
| `frontend/public/images/salad-dau-ga.webp` | Ảnh món ăn WebP hoặc SVG phục vụ thiết kế mới. |
| `frontend/public/images/salad-ngu-coc.webp` | Ảnh món ăn WebP hoặc SVG phục vụ thiết kế mới. |
| `frontend/src/app/about/page.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/api/v1/[...path]/route.ts` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/categories/[slug]/page.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/categories/page.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/error.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/globals.css` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/hangfire/[[...path]]/route.ts` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/icon.svg` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/layout.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/learn/page.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/loading.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/login/page.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/not-found.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/page.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/profile/layout.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/profile/page.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/recipes/[slug]/page.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/recipes/page.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/register/page.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/robots.ts` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/saved/page.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/sitemap.ts` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/studio/[id]/edit/page.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/studio/[id]/preview/page.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/studio/categories/page.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/studio/layout.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/studio/new/page.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/app/studio/page.tsx` | Route/SSR/SEO, thiết kế responsive hoặc API proxy Next.js. |
| `frontend/src/components/AccountGate.tsx` | Giao diện và luồng sử dụng thực tế: học, nấu, xác thực, tác giả. |
| `frontend/src/components/AuthForm.tsx` | Giao diện và luồng sử dụng thực tế: học, nấu, xác thực, tác giả. |
| `frontend/src/components/Footer.tsx` | Giao diện và luồng sử dụng thực tế: học, nấu, xác thực, tác giả. |
| `frontend/src/components/Pagination.tsx` | Giao diện và luồng sử dụng thực tế: học, nấu, xác thực, tác giả. |
| `frontend/src/components/PrivateRecipe.tsx` | Giao diện và luồng sử dụng thực tế: học, nấu, xác thực, tác giả. |
| `frontend/src/components/Providers.tsx` | Giao diện và luồng sử dụng thực tế: học, nấu, xác thực, tác giả. |
| `frontend/src/components/RecipeEditor.tsx` | Giao diện và luồng sử dụng thực tế: học, nấu, xác thực, tác giả. |
| `frontend/src/components/RecipeExperience.tsx` | Giao diện và luồng sử dụng thực tế: học, nấu, xác thực, tác giả. |
| `frontend/src/components/RecipeFilters.tsx` | Giao diện và luồng sử dụng thực tế: học, nấu, xác thực, tác giả. |
| `frontend/src/components/ServiceNotice.tsx` | Giao diện và luồng sử dụng thực tế: học, nấu, xác thực, tác giả. |
| `frontend/src/lib/api.ts` | Cập nhật cho bản nâng cấp v2. |
| `frontend/src/lib/saved.ts` | Cập nhật cho bản nâng cấp v2. |
| `frontend/src/lib/server-api.ts` | Cập nhật cho bản nâng cấp v2. |
| `frontend/src/lib/types.ts` | Cập nhật cho bản nâng cấp v2. |
| `playwright.config.ts` | Kiểm thử hồi quy và kịch bản nghiệm thu. |
| `scripts/dev.mjs` | Khởi động, CI hoặc đóng gói hệ thống. |
| `scripts/smoke_api.py` | Khởi động, CI hoặc đóng gói hệ thống. |
| `scripts/start.ps1` | Khởi động, CI hoặc đóng gói hệ thống. |
| `scripts/start.sh` | Khởi động, CI hoặc đóng gói hệ thống. |
| `tests/bff.test.mjs` | Kiểm thử hồi quy và kịch bản nghiệm thu. |
| `tests/contracts.test.mjs` | Kiểm thử hồi quy và kịch bản nghiệm thu. |
| `tests/e2e/culinary.spec.ts` | Kiểm thử hồi quy và kịch bản nghiệm thu. |
| `verification/api-results.json` | Tài liệu nâng cấp, đối chiếu yêu cầu và kết quả kiểm chứng. |
| `verification/check-summary.json` | Tài liệu nâng cấp, đối chiếu yêu cầu và kết quả kiểm chứng. |

## File source/cấu hình cũ bị xóa

| Đường dẫn | Lý do |
| --- | --- |
| `frontend/.env` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/eslint.config.js` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/index.html` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/package-lock.json` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/postcss.config.js` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/public/favicon.svg` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/public/icons.svg` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/App.css` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/App.tsx` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/assets/hero.png` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/assets/react.svg` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/assets/vite.svg` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/components/ApiExplorer.tsx` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/components/ArchitectureViewer.tsx` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/components/AuthModal.tsx` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/components/CategoryManagerModal.tsx` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/components/Dashboard.tsx` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/components/RecipeDetailModal.tsx` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/components/RecipeWizardModal.tsx` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/components/SearchModal.tsx` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/components/SystemMonitor.tsx` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/components/common/Footer.tsx` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/components/common/Header.tsx` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/components/common/LoadingSpinner.tsx` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/components/common/Pagination.tsx` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/components/common/ProtectedRoute.tsx` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/data/seedData.ts` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/index.css` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/layouts/MainLayout.tsx` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/lib/apiDocs.ts` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/lib/storage.ts` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/lib/utils.ts` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/main.tsx` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/pages/HomePage.tsx` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/services/api-client.ts` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/services/category-service.ts` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/store/auth-store.ts` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/types.ts` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/types/api.ts` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/types/auth.ts` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/types/file.ts` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/types/recipe.ts` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/src/utils/error-handler.ts` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/tailwind.config.js` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/tsconfig.app.json` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/tsconfig.node.json` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `frontend/vite.config.ts` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `index.html` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `metadata.json` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `public/CulinaryBlog-NET10-Backend-Source.zip` | Bỏ gói backend ZIP trùng lặp khỏi web root. |
| `public/culinaryblog-dotnet10-backend.zip` | Bỏ gói backend ZIP trùng lặp khỏi web root. |
| `tatus` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `tsconfig.json` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |
| `vite.config.ts` | Thay frontend Vite/mô phỏng bằng Next.js; cấu hình hoặc file cũ không còn dùng. |

## File build/cache cũ không đưa vào source mới

Các DLL, runtime, bản build, dependency metadata và Python cache bên dưới không phải source chức năng bị bỏ. Build sẽ tạo lại các thành phần cần thiết theo dependency mới.

```text
backend/scripts/__pycache__/generate_seed.cpython-310.pyc
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Asp.Versioning.Abstractions.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Asp.Versioning.Http.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Asp.Versioning.Mvc.ApiExplorer.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Asp.Versioning.Mvc.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Bogus.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/Microsoft.Build.Locator.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.exe
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.exe.config
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/Microsoft.IO.Redist.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/Newtonsoft.Json.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/System.Buffers.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/System.Collections.Immutable.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/System.CommandLine.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/System.Memory.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/System.Numerics.Vectors.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/System.Runtime.CompilerServices.Unsafe.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/System.Threading.Tasks.Extensions.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/cs/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/de/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/es/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/fr/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/it/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/ja/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/ko/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/pl/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/pt-BR/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/ru/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/tr/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/zh-Hans/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-net472/zh-Hant/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/Microsoft.Build.Locator.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.deps.json
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.dll.config
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.runtimeconfig.json
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/Newtonsoft.Json.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/System.Collections.Immutable.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/System.CommandLine.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/cs/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/de/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/es/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/fr/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/it/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/ja/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/ko/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/pl/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/pt-BR/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/ru/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/tr/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/zh-Hans/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/BuildHost-netcore/zh-Hant/System.CommandLine.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/CommunityToolkit.HighPerformance.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/CulinaryBlog.API.deps.json
backend/src/CulinaryBlog.API/bin/Debug/net10.0/CulinaryBlog.API.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/CulinaryBlog.API.exe
backend/src/CulinaryBlog.API/bin/Debug/net10.0/CulinaryBlog.API.pdb
backend/src/CulinaryBlog.API/bin/Debug/net10.0/CulinaryBlog.API.runtimeconfig.json
backend/src/CulinaryBlog.API/bin/Debug/net10.0/CulinaryBlog.API.staticwebassets.endpoints.json
backend/src/CulinaryBlog.API/bin/Debug/net10.0/CulinaryBlog.API.staticwebassets.runtime.json
backend/src/CulinaryBlog.API/bin/Debug/net10.0/CulinaryBlog.Application.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/CulinaryBlog.Application.pdb
backend/src/CulinaryBlog.API/bin/Debug/net10.0/CulinaryBlog.Domain.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/CulinaryBlog.Domain.pdb
backend/src/CulinaryBlog.API/bin/Debug/net10.0/CulinaryBlog.Infrastructure.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/CulinaryBlog.Infrastructure.pdb
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Dapper.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/FluentValidation.DependencyInjectionExtensions.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/FluentValidation.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Hangfire.AspNetCore.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Hangfire.Core.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Hangfire.InMemory.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Hangfire.NetCore.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/HealthChecks.Redis.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Humanizer.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/MediatR.Contracts.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/MediatR.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.AspNetCore.Authentication.JwtBearer.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.AspNetCore.OpenApi.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.Bcl.AsyncInterfaces.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.Build.Locator.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.CodeAnalysis.CSharp.Workspaces.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.CodeAnalysis.CSharp.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.CodeAnalysis.Workspaces.MSBuild.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.CodeAnalysis.Workspaces.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.CodeAnalysis.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.EntityFrameworkCore.Abstractions.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.EntityFrameworkCore.Design.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.EntityFrameworkCore.Relational.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.EntityFrameworkCore.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.Extensions.DependencyModel.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.IdentityModel.Abstractions.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.IdentityModel.JsonWebTokens.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.IdentityModel.Logging.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.IdentityModel.Protocols.OpenIdConnect.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.IdentityModel.Protocols.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.IdentityModel.Tokens.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Microsoft.OpenApi.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Minio.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Mono.TextTemplating.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Newtonsoft.Json.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Npgsql.EntityFrameworkCore.PostgreSQL.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Npgsql.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/OpenTelemetry.Api.ProviderBuilderExtensions.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/OpenTelemetry.Api.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/OpenTelemetry.Exporter.OpenTelemetryProtocol.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/OpenTelemetry.Extensions.Hosting.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/OpenTelemetry.Instrumentation.AspNetCore.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/OpenTelemetry.Instrumentation.EntityFrameworkCore.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/OpenTelemetry.Instrumentation.Http.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/OpenTelemetry.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Pipelines.Sockets.Unofficial.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Scalar.AspNetCore.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Serilog.AspNetCore.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Serilog.Extensions.Hosting.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Serilog.Extensions.Logging.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Serilog.Formatting.Compact.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Serilog.Settings.Configuration.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Serilog.Sinks.Console.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Serilog.Sinks.Debug.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Serilog.Sinks.File.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/Serilog.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/SixLabors.ImageSharp.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/StackExchange.Redis.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/System.CodeDom.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/System.Composition.AttributedModel.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/System.Composition.Convention.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/System.Composition.Hosting.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/System.Composition.Runtime.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/System.Composition.TypedParts.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/System.IO.Hashing.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/System.IdentityModel.Tokens.Jwt.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/System.Reactive.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/appsettings.Development.json
backend/src/CulinaryBlog.API/bin/Debug/net10.0/appsettings.json
backend/src/CulinaryBlog.API/bin/Debug/net10.0/ca/Hangfire.Core.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/cs/Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/cs/Microsoft.CodeAnalysis.CSharp.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/cs/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/cs/Microsoft.CodeAnalysis.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/cs/Microsoft.CodeAnalysis.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/de/Hangfire.Core.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/de/Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/de/Microsoft.CodeAnalysis.CSharp.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/de/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/de/Microsoft.CodeAnalysis.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/de/Microsoft.CodeAnalysis.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/es/Hangfire.Core.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/es/Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/es/Microsoft.CodeAnalysis.CSharp.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/es/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/es/Microsoft.CodeAnalysis.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/es/Microsoft.CodeAnalysis.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/fa/Hangfire.Core.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/fr/Hangfire.Core.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/fr/Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/fr/Microsoft.CodeAnalysis.CSharp.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/fr/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/fr/Microsoft.CodeAnalysis.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/fr/Microsoft.CodeAnalysis.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/it/Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/it/Microsoft.CodeAnalysis.CSharp.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/it/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/it/Microsoft.CodeAnalysis.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/it/Microsoft.CodeAnalysis.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/ja/Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/ja/Microsoft.CodeAnalysis.CSharp.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/ja/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/ja/Microsoft.CodeAnalysis.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/ja/Microsoft.CodeAnalysis.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/ko/Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/ko/Microsoft.CodeAnalysis.CSharp.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/ko/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/ko/Microsoft.CodeAnalysis.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/ko/Microsoft.CodeAnalysis.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/nb/Hangfire.Core.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/nl/Hangfire.Core.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/pl/Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/pl/Microsoft.CodeAnalysis.CSharp.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/pl/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/pl/Microsoft.CodeAnalysis.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/pl/Microsoft.CodeAnalysis.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/pt-BR/Hangfire.Core.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/pt-BR/Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/pt-BR/Microsoft.CodeAnalysis.CSharp.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/pt-BR/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/pt-BR/Microsoft.CodeAnalysis.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/pt-BR/Microsoft.CodeAnalysis.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/pt-PT/Hangfire.Core.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/pt/Hangfire.Core.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/ru/Hangfire.Core.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/ru/Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/ru/Microsoft.CodeAnalysis.CSharp.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/ru/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/ru/Microsoft.CodeAnalysis.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/ru/Microsoft.CodeAnalysis.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/sv/Hangfire.Core.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/tr-TR/Hangfire.Core.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/tr/Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/tr/Microsoft.CodeAnalysis.CSharp.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/tr/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/tr/Microsoft.CodeAnalysis.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/tr/Microsoft.CodeAnalysis.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/zh-Hans/Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/zh-Hans/Microsoft.CodeAnalysis.CSharp.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/zh-Hans/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/zh-Hans/Microsoft.CodeAnalysis.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/zh-Hans/Microsoft.CodeAnalysis.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/zh-Hant/Microsoft.CodeAnalysis.CSharp.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/zh-Hant/Microsoft.CodeAnalysis.CSharp.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/zh-Hant/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/zh-Hant/Microsoft.CodeAnalysis.Workspaces.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/zh-Hant/Microsoft.CodeAnalysis.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/zh-TW/Hangfire.Core.resources.dll
backend/src/CulinaryBlog.API/bin/Debug/net10.0/zh/Hangfire.Core.resources.dll
backend/src/CulinaryBlog.API/obj/CulinaryBlog.API.csproj.nuget.dgspec.json
backend/src/CulinaryBlog.API/obj/CulinaryBlog.API.csproj.nuget.g.props
backend/src/CulinaryBlog.API/obj/CulinaryBlog.API.csproj.nuget.g.targets
backend/src/CulinaryBlog.API/obj/Debug/net10.0/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs
backend/src/CulinaryBlog.API/obj/Debug/net10.0/Culinary.F7040BED.Up2Date
backend/src/CulinaryBlog.API/obj/Debug/net10.0/CulinaryBlog.API.AssemblyInfo.cs
backend/src/CulinaryBlog.API/obj/Debug/net10.0/CulinaryBlog.API.AssemblyInfoInputs.cache
backend/src/CulinaryBlog.API/obj/Debug/net10.0/CulinaryBlog.API.GeneratedMSBuildEditorConfig.editorconfig
backend/src/CulinaryBlog.API/obj/Debug/net10.0/CulinaryBlog.API.GlobalUsings.g.cs
backend/src/CulinaryBlog.API/obj/Debug/net10.0/CulinaryBlog.API.MvcApplicationPartsAssemblyInfo.cache
backend/src/CulinaryBlog.API/obj/Debug/net10.0/CulinaryBlog.API.MvcApplicationPartsAssemblyInfo.cs
backend/src/CulinaryBlog.API/obj/Debug/net10.0/CulinaryBlog.API.assets.cache
backend/src/CulinaryBlog.API/obj/Debug/net10.0/CulinaryBlog.API.csproj.AssemblyReference.cache
backend/src/CulinaryBlog.API/obj/Debug/net10.0/CulinaryBlog.API.csproj.CoreCompileInputs.cache
backend/src/CulinaryBlog.API/obj/Debug/net10.0/CulinaryBlog.API.csproj.FileListAbsolute.txt
backend/src/CulinaryBlog.API/obj/Debug/net10.0/CulinaryBlog.API.dll
backend/src/CulinaryBlog.API/obj/Debug/net10.0/CulinaryBlog.API.genruntimeconfig.cache
backend/src/CulinaryBlog.API/obj/Debug/net10.0/CulinaryBlog.API.pdb
backend/src/CulinaryBlog.API/obj/Debug/net10.0/CulinaryBlog.API.sourcelink.json
backend/src/CulinaryBlog.API/obj/Debug/net10.0/apphost.exe
backend/src/CulinaryBlog.API/obj/Debug/net10.0/ref/CulinaryBlog.API.dll
backend/src/CulinaryBlog.API/obj/Debug/net10.0/refint/CulinaryBlog.API.dll
backend/src/CulinaryBlog.API/obj/Debug/net10.0/rjimswa.dswa.cache.json
backend/src/CulinaryBlog.API/obj/Debug/net10.0/rjsmcshtml.dswa.cache.json
backend/src/CulinaryBlog.API/obj/Debug/net10.0/rjsmrazor.dswa.cache.json
backend/src/CulinaryBlog.API/obj/Debug/net10.0/rpswa.dswa.cache.json
backend/src/CulinaryBlog.API/obj/Debug/net10.0/staticwebassets.build.endpoints.json
backend/src/CulinaryBlog.API/obj/Debug/net10.0/staticwebassets.build.json
backend/src/CulinaryBlog.API/obj/Debug/net10.0/staticwebassets.build.json.cache
backend/src/CulinaryBlog.API/obj/Debug/net10.0/staticwebassets.development.json
backend/src/CulinaryBlog.API/obj/Debug/net10.0/swae.build.ex.cache
backend/src/CulinaryBlog.API/obj/project.assets.json
backend/src/CulinaryBlog.API/obj/project.nuget.cache
backend/src/CulinaryBlog.Application/bin/Debug/net10.0/CulinaryBlog.Application.deps.json
backend/src/CulinaryBlog.Application/bin/Debug/net10.0/CulinaryBlog.Application.dll
backend/src/CulinaryBlog.Application/bin/Debug/net10.0/CulinaryBlog.Application.pdb
backend/src/CulinaryBlog.Application/bin/Debug/net10.0/CulinaryBlog.Domain.dll
backend/src/CulinaryBlog.Application/bin/Debug/net10.0/CulinaryBlog.Domain.pdb
backend/src/CulinaryBlog.Application/obj/CulinaryBlog.Application.csproj.nuget.dgspec.json
backend/src/CulinaryBlog.Application/obj/CulinaryBlog.Application.csproj.nuget.g.props
backend/src/CulinaryBlog.Application/obj/CulinaryBlog.Application.csproj.nuget.g.targets
backend/src/CulinaryBlog.Application/obj/Debug/net10.0/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs
backend/src/CulinaryBlog.Application/obj/Debug/net10.0/Culinary.995F3C4F.Up2Date
backend/src/CulinaryBlog.Application/obj/Debug/net10.0/CulinaryBlog.Application.AssemblyInfo.cs
backend/src/CulinaryBlog.Application/obj/Debug/net10.0/CulinaryBlog.Application.AssemblyInfoInputs.cache
backend/src/CulinaryBlog.Application/obj/Debug/net10.0/CulinaryBlog.Application.GeneratedMSBuildEditorConfig.editorconfig
backend/src/CulinaryBlog.Application/obj/Debug/net10.0/CulinaryBlog.Application.GlobalUsings.g.cs
backend/src/CulinaryBlog.Application/obj/Debug/net10.0/CulinaryBlog.Application.assets.cache
backend/src/CulinaryBlog.Application/obj/Debug/net10.0/CulinaryBlog.Application.csproj.AssemblyReference.cache
backend/src/CulinaryBlog.Application/obj/Debug/net10.0/CulinaryBlog.Application.csproj.CoreCompileInputs.cache
backend/src/CulinaryBlog.Application/obj/Debug/net10.0/CulinaryBlog.Application.csproj.FileListAbsolute.txt
backend/src/CulinaryBlog.Application/obj/Debug/net10.0/CulinaryBlog.Application.dll
backend/src/CulinaryBlog.Application/obj/Debug/net10.0/CulinaryBlog.Application.pdb
backend/src/CulinaryBlog.Application/obj/Debug/net10.0/CulinaryBlog.Application.sourcelink.json
backend/src/CulinaryBlog.Application/obj/Debug/net10.0/ref/CulinaryBlog.Application.dll
backend/src/CulinaryBlog.Application/obj/Debug/net10.0/refint/CulinaryBlog.Application.dll
backend/src/CulinaryBlog.Application/obj/project.assets.json
backend/src/CulinaryBlog.Application/obj/project.nuget.cache
backend/src/CulinaryBlog.Domain/bin/Debug/net10.0/CulinaryBlog.Domain.deps.json
backend/src/CulinaryBlog.Domain/bin/Debug/net10.0/CulinaryBlog.Domain.dll
backend/src/CulinaryBlog.Domain/bin/Debug/net10.0/CulinaryBlog.Domain.pdb
backend/src/CulinaryBlog.Domain/obj/CulinaryBlog.Domain.csproj.nuget.dgspec.json
backend/src/CulinaryBlog.Domain/obj/CulinaryBlog.Domain.csproj.nuget.g.props
backend/src/CulinaryBlog.Domain/obj/CulinaryBlog.Domain.csproj.nuget.g.targets
backend/src/CulinaryBlog.Domain/obj/Debug/net10.0/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs
backend/src/CulinaryBlog.Domain/obj/Debug/net10.0/CulinaryBlog.Domain.AssemblyInfo.cs
backend/src/CulinaryBlog.Domain/obj/Debug/net10.0/CulinaryBlog.Domain.AssemblyInfoInputs.cache
backend/src/CulinaryBlog.Domain/obj/Debug/net10.0/CulinaryBlog.Domain.GeneratedMSBuildEditorConfig.editorconfig
backend/src/CulinaryBlog.Domain/obj/Debug/net10.0/CulinaryBlog.Domain.GlobalUsings.g.cs
backend/src/CulinaryBlog.Domain/obj/Debug/net10.0/CulinaryBlog.Domain.assets.cache
backend/src/CulinaryBlog.Domain/obj/Debug/net10.0/CulinaryBlog.Domain.csproj.CoreCompileInputs.cache
backend/src/CulinaryBlog.Domain/obj/Debug/net10.0/CulinaryBlog.Domain.csproj.FileListAbsolute.txt
backend/src/CulinaryBlog.Domain/obj/Debug/net10.0/CulinaryBlog.Domain.dll
backend/src/CulinaryBlog.Domain/obj/Debug/net10.0/CulinaryBlog.Domain.pdb
backend/src/CulinaryBlog.Domain/obj/Debug/net10.0/CulinaryBlog.Domain.sourcelink.json
backend/src/CulinaryBlog.Domain/obj/Debug/net10.0/ref/CulinaryBlog.Domain.dll
backend/src/CulinaryBlog.Domain/obj/Debug/net10.0/refint/CulinaryBlog.Domain.dll
backend/src/CulinaryBlog.Domain/obj/project.assets.json
backend/src/CulinaryBlog.Domain/obj/project.nuget.cache
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/Microsoft.Build.Locator.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.exe
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.exe.config
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/Microsoft.IO.Redist.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/Newtonsoft.Json.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/System.Buffers.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/System.Collections.Immutable.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/System.CommandLine.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/System.Memory.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/System.Numerics.Vectors.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/System.Runtime.CompilerServices.Unsafe.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/System.Threading.Tasks.Extensions.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/cs/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/de/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/es/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/fr/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/it/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/ja/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/ko/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/pl/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/pt-BR/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/ru/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/tr/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/zh-Hans/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-net472/zh-Hant/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/Microsoft.Build.Locator.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.deps.json
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.dll.config
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/Microsoft.CodeAnalysis.Workspaces.MSBuild.BuildHost.runtimeconfig.json
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/Newtonsoft.Json.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/System.Collections.Immutable.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/System.CommandLine.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/cs/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/de/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/es/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/fr/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/it/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/ja/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/ko/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/pl/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/pt-BR/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/ru/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/tr/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/zh-Hans/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/BuildHost-netcore/zh-Hant/System.CommandLine.resources.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/CulinaryBlog.Application.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/CulinaryBlog.Application.pdb
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/CulinaryBlog.Domain.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/CulinaryBlog.Domain.pdb
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/CulinaryBlog.Infrastructure.deps.json
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/CulinaryBlog.Infrastructure.dll
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/CulinaryBlog.Infrastructure.pdb
backend/src/CulinaryBlog.Infrastructure/bin/Debug/net10.0/CulinaryBlog.Infrastructure.runtimeconfig.json
backend/src/CulinaryBlog.Infrastructure/obj/CulinaryBlog.Infrastructure.csproj.nuget.dgspec.json
backend/src/CulinaryBlog.Infrastructure/obj/CulinaryBlog.Infrastructure.csproj.nuget.g.props
backend/src/CulinaryBlog.Infrastructure/obj/CulinaryBlog.Infrastructure.csproj.nuget.g.targets
backend/src/CulinaryBlog.Infrastructure/obj/Debug/net10.0/.NETCoreApp,Version=v10.0.AssemblyAttributes.cs
backend/src/CulinaryBlog.Infrastructure/obj/Debug/net10.0/Culinary.1726739D.Up2Date
backend/src/CulinaryBlog.Infrastructure/obj/Debug/net10.0/CulinaryBlog.Infrastructure.AssemblyInfo.cs
backend/src/CulinaryBlog.Infrastructure/obj/Debug/net10.0/CulinaryBlog.Infrastructure.AssemblyInfoInputs.cache
backend/src/CulinaryBlog.Infrastructure/obj/Debug/net10.0/CulinaryBlog.Infrastructure.GeneratedMSBuildEditorConfig.editorconfig
backend/src/CulinaryBlog.Infrastructure/obj/Debug/net10.0/CulinaryBlog.Infrastructure.GlobalUsings.g.cs
backend/src/CulinaryBlog.Infrastructure/obj/Debug/net10.0/CulinaryBlog.Infrastructure.assets.cache
backend/src/CulinaryBlog.Infrastructure/obj/Debug/net10.0/CulinaryBlog.Infrastructure.csproj.AssemblyReference.cache
backend/src/CulinaryBlog.Infrastructure/obj/Debug/net10.0/CulinaryBlog.Infrastructure.csproj.CoreCompileInputs.cache
backend/src/CulinaryBlog.Infrastructure/obj/Debug/net10.0/CulinaryBlog.Infrastructure.csproj.FileListAbsolute.txt
backend/src/CulinaryBlog.Infrastructure/obj/Debug/net10.0/CulinaryBlog.Infrastructure.dll
backend/src/CulinaryBlog.Infrastructure/obj/Debug/net10.0/CulinaryBlog.Infrastructure.genruntimeconfig.cache
backend/src/CulinaryBlog.Infrastructure/obj/Debug/net10.0/CulinaryBlog.Infrastructure.pdb
backend/src/CulinaryBlog.Infrastructure/obj/Debug/net10.0/CulinaryBlog.Infrastructure.sourcelink.json
backend/src/CulinaryBlog.Infrastructure/obj/Debug/net10.0/ref/CulinaryBlog.Infrastructure.dll
backend/src/CulinaryBlog.Infrastructure/obj/Debug/net10.0/refint/CulinaryBlog.Infrastructure.dll
backend/src/CulinaryBlog.Infrastructure/obj/project.assets.json
backend/src/CulinaryBlog.Infrastructure/obj/project.nuget.cache
```

## File/dữ liệu giữ nguyên

Bản dump, các script dữ liệu ban đầu, migration cũ và các upload gốc vẫn có trong source nếu không được liệt kê là thay đổi. Không chạy migration hoặc reset database người dùng trong quá trình nâng cấp.

`FILE_CHANGES.json` chứa SHA-256 trước/sau của các file thay đổi. Hash của chính manifest được để null để tránh tự tham chiếu.
