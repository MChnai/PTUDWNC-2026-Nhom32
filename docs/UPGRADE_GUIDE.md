# Hướng dẫn nâng cấp và vận hành

## 1. Giữ bản gốc và dùng thư mục mới

Giữ file zip cũ hoặc commit hiện tại. Giải nén v2 sang thư mục mới. Không chỉ copy các file mới vào source cũ: những file bị xóa, đặc biệt các component giả lập và cấu hình Vite, có thể còn nằm lại và gây lỗi TypeScript.

`CHANGES.md` phân biệt file sửa/thêm/xóa. `FILE_CHANGES.json` chứa hash của từng file trước/sau; không chứa mật khẩu hoặc nội dung database.

## 2. Database mới

Chạy Docker Compose ở thư mục gốc. PostgreSQL chỉ tự chạy script init khi volume dữ liệu trống. Thứ tự:

1. `backend/scripts/init_postgresql16_schema_compatible.sql`.
2. `backend/scripts/04_upgrade_v2.sql`.
3. API seed catalog nếu `Seed__Enabled=true`, đang Development và chưa có bất kỳ recipe nào.

Starter không ghi đè database có công thức. Seed chỉ hỗ trợ Development và yêu cầu mật khẩu tối thiểu 12 ký tự. Không tự thay mật khẩu của tài khoản đã tồn tại.

## 3. Database đang dùng hoặc bản dump có sẵn

Giữ database gốc. Làm thử trên một database sao chép trước. Ví dụ với PostgreSQL tools trên máy:

```powershell
pg_dump -h localhost -U postgres -Fc -f culinaryblog_before_v2.dump culinaryblog
createdb -h localhost -U postgres culinaryblog_v2_copy
pg_restore -h localhost -U postgres --no-owner --no-acl -d culinaryblog_v2_copy culinaryblog_before_v2.dump
psql -h localhost -U postgres -d culinaryblog_v2_copy -v ON_ERROR_STOP=1 -f backend/scripts/init_postgresql16_schema_compatible.sql
psql -h localhost -U postgres -d culinaryblog_v2_copy -v ON_ERROR_STOP=1 -f backend/scripts/04_upgrade_v2.sql
```

Nếu dùng file `backend/culinaryblog_backup.dump` đã có, thay đối số tên dump ở lệnh `pg_restore`; vẫn phục hồi vào database mới. Không dùng `--clean` lên database đang có dữ liệu.

Đặt `ConnectionStrings__DefaultConnection` trỏ tới bản sao khi chạy API. Kiểm tra đăng nhập tài khoản cũ, ảnh, số công thức và các luồng sửa trước khi áp dụng cùng quy trình lên database chính. Không chạy `dotnet ef database update` với migration InitialCreate cũ trên schema được tạo bằng SQL.

Script v2 thêm cột medium image nếu thiếu, chuyển raw refresh token thành SHA-256, chuẩn hóa ràng buộc thứ tự bước dạng deferred và thêm index. Không cập nhật title, slug, nội dung, trạng thái hay password hash của tài khoản hiện có. Nó tương thích với schema UUID/chữ enum trong bản nộp; không phải công cụ chuyển đổi một schema khác hoàn toàn.

Các script `01_seed_data.sql`, `02_seed_data.sql`, `03_seed_english_to_main.sql` và generator cũ được giữ để tham chiếu, **không nằm trong đường khởi tạo v2**. Một số script này là bản cập nhật nội dung cho ID cũ, không phải seed một database rỗng.

## 4. Ảnh

Production/local Compose dùng MinIO. API kết nối `Minio__Endpoint=minio:9000`; trình duyệt đọc URL từ `Minio__CustomDomain`, mặc định `http://localhost:9000`. Hai địa chỉ phục vụ hai đối tượng khác nhau.

Nếu truy cập từ máy khác, `localhost:9000` không còn trỏ tới máy chủ. Cần đặt `MINIO_PUBLIC_URL` thành URL HTTPS công khai/reverse proxy phù hợp. Bucket `culinary-blog` chỉ cấp quyền tải xuống công khai, không cấp quyền ghi ẩn danh.

Chạy API trực tiếp mặc định dùng `Storage:Provider=Local`, lưu ở `backend/src/CulinaryBlog.API/wwwroot/uploads`. Frontend proxy `/uploads/*` tới API; không cần chép ảnh vào thư mục frontend. Không đưa đường dẫn `G:\...` vào trường URL.

Giới hạn: JPG/PNG/WebP/AVIF, 5 MB mỗi ảnh, tối đa 12 ảnh/công thức, 5 lượt tải/phút/IP; kiểm tra MIME, magic bytes, khả năng đọc ảnh và kích thước tối đa 40 triệu điểm ảnh. Tên file do hệ thống sinh. Thumbnail 300×300 và medium 800×600 được sinh qua job. Các URL ảnh gốc trong dữ liệu cũ được giữ nguyên; ảnh không truy cập được có hình thay thế.

## 5. Google đăng nhập

Tạo Google OAuth Web client cho domain đang dùng. Authorized JavaScript origins phải có origin frontend. Cấu hình cùng một ID:

- `GOOGLE_CLIENT_ID` → backend `Google__ClientId`.
- `NEXT_PUBLIC_GOOGLE_CLIENT_ID` → build frontend.

Chạy lại build frontend khi đổi biến NEXT_PUBLIC. Chỉ khi có client ID, nút Google mới xuất hiện. API nhận `{ idToken }`, xác minh chữ ký, audience, thời hạn và email verified bằng thư viện Google trước khi dùng thông tin tài khoản. Không chấp nhận email/provider key do client tự khai báo.

Chưa nghiệm thu bằng một tài khoản Google thực. Đăng nhập email vẫn sử dụng độc lập.

## 6. Email, job, cache và quan sát

Compose có Mailpit để nhận email tại máy, không gửi ra hộp thư ngoài. Thay `Smtp__Host/Port/Username/Password/EnableSsl/From` khi dùng SMTP thực.

Hangfire mặc định lưu queue trong PostgreSQL. `Jobs__InMemory=true` chỉ là lựa chọn dành cho test/local tạm. Sitemap job chạy 02:00 UTC; dashboard yêu cầu Admin. Mở dashboard qua frontend `/hangfire` để cookie được chuyển thành Bearer token ở phía server. Sau khi access cookie hết hạn, dashboard có thể đưa về trang đăng nhập; phiên website thông thường vẫn có refresh tự động.

Redis làm output-cache cho public listing/categories; mọi mutation thành công làm mất hiệu lực tag recipe. Request có thông tin xác thực và recipe detail không được chia sẻ cache. Live/ready health, Serilog và OpenTelemetry đã có cấu hình. Muốn gửi telemetry ra collector, đặt `OTLP_ENDPOINT`; mặc định không cố kết nối collector chưa có.

## 7. Các quyết định tương thích cần biết

- Tiếp tục dùng `users.id` UUID và password hasher tương thích source gốc; chưa chuyển schema sang ASP.NET Core Identity string ID. Password mới dùng ASP.NET Core Identity PBKDF2-HMACSHA512 600.000 vòng; verifier vẫn đọc hash cũ 100.000 vòng.
- Giữ response `{ isSuccess, value }` của source gốc. SRS có nhiều mô tả wrapper khác nhau; lỗi HTTP được chuẩn hóa theo Problem Details.
- Xóa recipe và dữ liệu con là hard delete theo FR-RCP-007; category vẫn soft delete và không xóa khi còn recipe. SRS có NFR nhắc soft delete recipe, mâu thuẫn với FR-RCP-007; bản này ưu tiên FR cụ thể.
- Slug được giữ ổn định cả khi sửa title để không phá liên kết. Không có lịch sử đổi slug/301 vì không cho sửa slug.
- Tài khoản đăng ký email có `EmailConfirmed=false`. Google được xác thực hợp lệ sẽ đánh dấu true. Policy VerifiedAuthor được định nghĩa, nhưng chưa có luồng gửi link xác nhận email cục bộ và chưa dùng policy này để chặn CRUD; phân quyền chính là Author/Admin và ownership.
- Sổ tay và dấu nguyên liệu lưu tại trình duyệt; không đồng bộ giữa thiết bị. Đồng hồ nấu dừng khi đóng chế độ nấu hoặc rời trang.
- Ảnh tải lên là thao tác sau transaction nội dung. Nếu upload lỗi, nội dung có thể đã lưu; trình viết giữ ID và chỉ thử lại các file còn lại. Lỗi 409 không tự đổi RowVersion để ghi đè nội dung của người khác.

## 8. Kiểm tra trước khi dùng ngoài local

Xác nhận Docker build/health, SMTP thực, Google, upload MinIO và thumbnail; chạy bộ E2E; đo hiệu năng và accessibility trên thiết bị đích. Các mục này chưa được chứng nhận chỉ từ việc build thành công. Chi tiết kết quả hiện tại nằm trong `VERIFICATION.md`.

## 9. Các route trong SRS

Các URL `/auth/login`, `/auth/register`, `/dashboard`, `/dashboard/recipes`, `/dashboard/recipes/new`, `/dashboard/recipes/{id}/edit`, `/dashboard/categories` và `/search` được chuyển hướng sang các trang tương ứng `/login`, `/register`, `/studio` và `/recipes`. Backend hỗ trợ cả `/recipes/{slug}` lẫn `/recipes/by-slug/{slug}`, cùng route ID dạng GUID cho màn hình chỉnh sửa.

Rate limit mặc định: auth 10/phút/IP (sliding window), API chung 100/phút/IP, upload 5/phút/IP; trả 429 và Retry-After. Kiểm thử nhiều bước dùng cấu hình riêng `RateLimits__AuthPermitLimit=100` để không dùng hết quota auth trong một phút; không đổi default shipping. Nếu dùng reverse proxy/BFF nhiều người dùng, cần cấu hình IP forwarding tin cậy trước khi nghiệm thu giới hạn theo IP ở production.
