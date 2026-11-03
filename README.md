# Culinary Blog 2.0

Website học nấu ăn và chia sẻ công thức bằng tiếng Việt. Frontend được chuyển từ React/Vite sang **Next.js App Router**, giữ backend **.NET 10 / Clean Architecture / CQRS**, PostgreSQL 16 và UUID của dự án gốc.

## Chạy nhanh trên Windows

Cài Docker Desktop và bật Linux containers. Giải nén bản này **sang thư mục mới**, không chép đè lên thư mục source cũ vì bản nâng cấp có xóa các file Vite/mô phỏng.

Mở PowerShell tại thư mục chứa README này:

```powershell
Copy-Item .env.example .env
docker compose up --build -d
```

Hoặc chạy `./scripts/start.ps1`. Lần build đầu cần tải thư viện và biên dịch MinIO từ source chính thức nên có thể mất vài phút hoặc lâu hơn tùy mạng/máy. Không có cam kết khởi động dưới 5 phút cho lần build đầu.

| Địa chỉ local | Chức năng |
| --- | --- |
| http://localhost:3000 | Website |
| http://localhost:5000/scalar | Tài liệu API trong Development |
| http://localhost:8025 | Hộp thư Mailpit, nhận email thử nghiệm tại máy |
| http://localhost:9001 | Giao diện MinIO |
| http://localhost:5000/health/ready | Kiểm tra PostgreSQL, Redis, MinIO |
| http://localhost:3000/hangfire | Tác vụ nền; đăng nhập tài khoản Admin trước |

Tài khoản phát triển mặc định: `admin@culinary.local` / `Culinary_Local_2026!`. Các giá trị lấy từ `SEED_ADMIN_EMAIL` và `SEED_ADMIN_PASSWORD` trong `.env`. Đây là dữ liệu dành cho local; khi đưa lên máy chủ thật phải thay secrets, tắt seed, cấu hình domain/HTTPS và chuyển môi trường Production. API từ chối khóa JWT phát triển khi chạy Production.

MinIO được build từ tag upstream `RELEASE.2025-10-15T17-29-55Z`, client từ `RELEASE.2025-08-13T08-35-41Z`. Không phụ thuộc hai image `minio/minio` và `minio/mc` cũ đã bị rút khỏi Docker Hub. Dockerfile nằm tại `deploy/Dockerfile.minio`.

## Các luồng đã làm

- Trang chủ, danh mục, tìm kiếm tiếng Việt có/không dấu, lọc độ khó/thời gian/khẩu phần, sắp xếp và phân trang.
- Trang công thức có URL theo slug, ảnh, nguyên liệu, bước nấu, dinh dưỡng, JSON-LD Recipe, metadata, sitemap và robots.
- Điều chỉnh khẩu phần, đánh dấu nguyên liệu, nấu từng bước, đồng hồ cho từng bước, in, chia sẻ và sổ tay công thức trên thiết bị.
- Đăng ký, đăng nhập, refresh token, logout, cập nhật hồ sơ; Google ID token khi đã cấu hình client ID.
- Góc tác giả: tạo bản nháp, sửa toàn bộ nội dung, upload/xóa/chọn ảnh chính, xem trước, xuất bản/hủy xuất bản, lưu trữ/khôi phục, xóa công thức.
- Admin quản lý danh mục và xem Hangfire. Mọi thao tác tài nguyên vẫn kiểm tra quyền ở backend.
- Lưu metadata, nguyên liệu, bước và dinh dưỡng trong một transaction EF; `If-Match`/xmin ngăn ghi đè phiên bản cũ. File ảnh được tải lên sau khi lưu nội dung, ảnh thành công không bị tải lại khi thử lại lỗi.
- Token truy cập/refresh nằm trong cookie HttpOnly tại frontend. Backend lưu SHA-256 của refresh token. Có kiểm tra Origin, giới hạn request auth, xác minh chữ ký Google token và kiểm tra nội dung ảnh.

Catalog phát triển có **12 công thức, 6 danh mục, 4 tác giả mẫu và 1 Admin**. Không tự bổ sung dữ liệu mẫu nếu database đã có công thức. Bản dump và các script dữ liệu ban đầu được giữ lại. Các tác giả/công thức starter là dữ liệu hướng dẫn thử nghiệm, không phải kết quả nghiệm thu hay lời chứng thực của đầu bếp thực tế.

## Dùng database hiện tại

Đọc `docs/UPGRADE_GUIDE.md` trước. Không chạy lại `InitialCreate`, không chạy các script dịch nội dung cũ lên database mới. Với database đã có, script init của Docker chỉ chạy khi volume trống; cần áp dụng script tương thích và `04_upgrade_v2.sql` theo hướng dẫn, sau khi sao lưu.

## Chạy development không dùng container ứng dụng

Cần Node.js 22+, .NET SDK 10, PostgreSQL 16 đã được tạo schema. Có thể dùng stack Docker cho các dịch vụ hạ tầng.

```powershell
npm ci
Copy-Item frontend/.env.example frontend/.env.local
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:ASPNETCORE_URLS="http://localhost:5000"
dotnet run --project backend/src/CulinaryBlog.API
```

Mở terminal thứ hai:

```powershell
npm run dev
```

`BACKEND_URL` trong `frontend/.env.local` phải trỏ tới API, mặc định `http://127.0.0.1:5000`. API không tự chạy EF migration, không tự tạo database. Muốn seed catalog mới bằng cách này, bật `Seed__Enabled=true` và đặt `Seed__AdminPassword` trước khi khởi động API; chỉ hỗ trợ Development.

## Kiểm thử

```powershell
npm test
npm run build
dotnet test backend/tests/CulinaryBlog.Tests -c Release
dotnet build backend/src/CulinaryBlog.API -c Release
```

Kiểm thử API thực tạo tài khoản/công thức thử nghiệm. Chỉ chạy với database local dùng riêng cho kiểm thử:

```powershell
$env:SEED_ADMIN_PASSWORD="Culinary_Local_2026!"
python scripts/smoke_api.py --base http://localhost:5000
```

Đã cung cấp 5 kịch bản trình duyệt liên tiếp: đăng ký, đăng nhập, tạo draft, xuất bản, tìm kiếm và chế độ nấu. Chúng **chưa được chạy end-to-end** trong môi trường bàn giao này. Khi Docker local hoạt động:

```powershell
npx playwright install chromium
npm run test:e2e
```

Kịch bản E2E chỉ cho phép localhost để tránh tạo dữ liệu trên production. Xem kết quả đã chạy và các giới hạn tại `docs/VERIFICATION.md`; số lượng test không phải phần trăm coverage.

## Tài liệu bàn giao

| File | Nội dung |
| --- | --- |
| `CHANGES.md` | Danh sách đầy đủ file sửa, thêm và xóa |
| `FILE_CHANGES.json` | Manifest thay đổi kèm SHA-256 trước/sau |
| `docs/UPGRADE_GUIDE.md` | Cài đặt, nâng cấp database và cấu hình dịch vụ |
| `docs/SRS_COVERAGE.md` | Đối chiếu từng nhóm FR/NFR và các khác biệt còn lại |
| `docs/VERIFICATION.md` | Kết quả kiểm thử thực tế và giới hạn xác minh |
| `docs/THIRD_PARTY.md` | Nguồn ảnh, font và thư viện |
