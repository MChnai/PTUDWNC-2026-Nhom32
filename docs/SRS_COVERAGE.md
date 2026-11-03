# Đối chiếu SRS Culinary Blog v1.0.0

Đối chiếu mã yêu cầu trong bản PDF đính kèm. “Đã cài đặt” là có đường thực thi trong source; không đồng nghĩa đã nghiệm thu mọi môi trường hoặc đạt toàn bộ NFR.

| Mã | Hiện thực v2 | Kiểm chứng / điểm còn lại |
| --- | --- | --- |
| FR-AUTH-001 | Đăng ký qua MediatR/validation, role Author, hash mật khẩu, enqueue welcome email | API: thành công, password yếu, email trùng; email thực chưa nghiệm thu |
| FR-AUTH-002 | Login, khóa tạm sau nhiều lần sai, JWT 15 phút | API: login Admin, sai credentials; unit: hash mới/hash cũ |
| FR-AUTH-003 | Google Identity Services + xác minh ID token tại server | Có mã và hướng dẫn cấu hình; chưa dùng tài khoản Google thật |
| FR-AUTH-004 | Refresh 7 ngày, token rotation, phát hiện reuse, thu hồi token family | API: rotation/reuse/family revoked; BFF: single-flight |
| FR-AUTH-005 | Logout xác thực và giới hạn token theo chủ sở hữu; xóa cookie | API và BFF tests |
| FR-AUTH-006/007 | `/auth/me` GET/PATCH, hồ sơ, tên hiển thị, avatar | API: đọc/sửa, chặn URL javascript; giao diện profile |
| FR-CAT-001 | Danh mục, count chỉ Published, public cache | API danh sách; Redis runtime chưa kiểm thử trong stack đầy đủ |
| FR-CAT-002 | Slug và phân trang; Published + Draft của chính tác giả | Endpoint có, giới hạn page size; frontend category dùng public recipe filter |
| FR-CAT-003/004/005 | Admin tạo/sửa/xóa, giữ slug, không xóa danh mục còn bài | API: Author bị chặn, Admin CRUD, category populated 409 |
| FR-RCP-001 | Listing Published, DTO đầy đủ, sort/filter/page | API public, clamp 50, enum/filter/sort lỗi |
| FR-RCP-002 | Detail theo ID/slug, visibility, ETag, view counter | API: guest không đọc draft/archived/child; slug và view |
| FR-RCP-003 | Tạo Draft, children/nutrition, author từ JWT | API: tạo đủ nội dung, draft không public |
| FR-RCP-004 | PUT metadata + nutrition + ingredients + steps trong một SaveChanges; If-Match | API: cập nhật đồng bộ, foreign child ID bị chặn, stale 409 |
| FR-RCP-005 | Publish/unpublish; ít nhất một bước; idempotent | API: publish, publish lần hai, thiếu bước 422 |
| FR-RCP-006 | Archive/unarchive về Draft | API: ẩn công khai và phục hồi |
| FR-RCP-007 | Hard delete/cascade và job dọn ảnh/variants | API xóa/mất truy cập; MinIO cleanup cần nghiệm thu stack |
| FR-RCP-008 | Upload magic/decode/size, chọn primary, xóa; job variants | API PNG hợp lệ, giả PNG, RIFF giả WebP, set/delete primary; AVIF và MinIO/variants chưa nghiệm thu đầy đủ |
| FR-RCP-009 | CRUD nguyên liệu, validation, ownership | Nội dung aggregate được kiểm thử; endpoint riêng có mã/validation |
| FR-RCP-010 | CRUD/reorder bước, renumber, ownership, không xóa bước cuối của Published | API: foreign delete 403, renumber 1, last-step 422 |
| FR-SRCH-001 | PostgreSQL GIN/tsquery prefix, unaccent, relevance | API tìm `mi` ra “Mì”; query rỗng/ngắn bị chặn; bỏ dependency hàm SQL còn thiếu |
| FR-SRCH-002 | AND filter category, difficulty, maxCookTime/maxTotalTime, minServings | API difficulty/time/servings; UI có bộ lọc kết hợp |
| FR-SRCH-003 | Sort title, time, createdAt, views, rating, rank; `sort=-field` | API sort hợp lệ/không hợp lệ; frontend shareable URL |
| FR-SRCH-004 | Page/pageSize tối đa 50, metadata, thứ tự phụ theo ID | API clamp/lỗi; UI phân trang |
| FR-FILE-001/002 | Abstraction Local/MinIO, kiểm tra ảnh; delete chung chỉ Admin, recipe delete theo ownership | Local upload được kiểm thử; MinIO thực chưa chạy trong môi trường này |
| FR-JOB-001 | SMTP HTML welcome email, retry job | Có mã; không còn Task.Delay giả gửi email; cần xác nhận Mailpit/SMTP nhận thư |
| FR-JOB-002 | Magick.NET thumbnail/medium, lưu URL vào database | Có job, không phụ thuộc ImageSharp yêu cầu license key; cần kiểm tra thực tế các format/provider |
| FR-JOB-003 | Sitemap job 02:00 UTC, khóa job, XML an toàn, Next sitemap/robots | Có mã; không dùng Google sitemap ping cũ; chưa kiểm thử collector/indexing ngoài |
| FR-OBS-001 | `/health`, `/health/live`, `/health/ready` theo dependency đang bật | API live/ready với PostgreSQL Local storage đã qua |
| FR-OBS-002 | Correlation ID được giới hạn ký tự, Serilog, thời gian handler, không log body auth | Có mã; chưa nghiệm thu rolling file/Seq trong production |
| FR-OBS-003 | OTel HTTP/EF/metrics, optional OTLP endpoint | Có cấu hình; chưa kiểm tra một collector thực |

## Các yêu cầu phi chức năng chưa được coi là đã đạt

| Nhóm | Trạng thái thực tế |
| --- | --- |
| Next.js/SSR/SEO | Đã chuyển App Router, public SSR, canonical, JSON-LD, sitemap/robots. Các trang public hiện dùng SSR/no-store cho tính tươi mới, chưa triển khai ISR theo các TTL nêu ở SRS. CSS thiết kế riêng thay cho Tailwind utility classes. Chưa chạy Google Rich Results Test/Lighthouse. |
| Bảo mật | Có hash password/token, cookie HttpOnly, Origin check, role/ownership, image validation, auth rate limit. Chưa pentest hoặc audit độc lập. |
| Identity/VerifiedAuthor | Giữ UUID/custom account để tương thích dữ liệu; chưa migrate sang Identity schema của Chương 7. Policy verified có định nghĩa, chưa có confirmation-email workflow. |
| Hiệu năng | Có cache/query/index/pagination; chưa đo p95, load test, SLA hay chứng nhận scalability. Search rank còn nạp tập ID match vào bộ nhớ trước phân trang relevance. |
| Test coverage ≥80% | Không tuyên bố đạt. 75 API checks, 13 unit tests, 10 Node checks là số ca đã chạy; chưa thu line coverage toàn Application. |
| E2E 5 luồng | Đã viết tại `tests/e2e/culinary.spec.ts`, chưa chạy đủ hệ thống trên browser. |
| Responsive/WCAG | Có media queries, label, focus, alt, native dialog và reduced motion; desktop đã xem; chưa audit WCAG hoặc xác minh đầy đủ mobile. |
| Hạ tầng | Có Docker stack, persistent volumes, PostgreSQL jobs, Redis, MinIO, SMTP. Không có Docker Engine trong môi trường kiểm tra để chạy toàn stack. |
| Backup/TLS/production | Có hướng dẫn và secrets qua env; chưa tự triển khai TLS, backup lịch 30 ngày, read replica/CDN hoặc cluster. |

SRS có một số mô tả không đồng nhất về wrapper, khóa người dùng và soft/hard delete. Các quyết định tương thích cụ thể được ghi ở `UPGRADE_GUIDE.md`, không gộp thành tuyên bố “đạt 100% SRS”.
