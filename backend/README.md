# Backend — .NET 10

Giữ bốn layer: Domain, Application, Infrastructure, API. PostgreSQL dùng UUID, enum Difficulty/Status lưu chuỗi và concurrency token xmin như schema hiện tại của dự án.

Chạy stack từ `../docker-compose.yml` hoặc đọc `../README.md`. API không tự chạy EF migration lên database được tạo bằng SQL; không áp dụng `InitialCreate` lên bản dump hiện có.

Các thay đổi chính: auth qua MediatR có validation, refresh-token hashing/rotation, endpoint hồ sơ/logout/Google, API slug/manage, kiểm tra quyền con của recipe, cập nhật nội dung nguyên tử, bảo vệ publish, kiểm tra ảnh, output cache/Redis, Hangfire/PostgreSQL, SMTP, sitemap và health checks.

Database mới: `scripts/init_postgresql16_schema_compatible.sql`, sau đó `scripts/04_upgrade_v2.sql`. Database cũ: sao lưu trước và làm theo `../docs/UPGRADE_GUIDE.md`.

Kiểm thử: `dotnet test tests/CulinaryBlog.Tests -c Release`; smoke API: `python ../scripts/smoke_api.py` với một API/database local dùng riêng cho test.
