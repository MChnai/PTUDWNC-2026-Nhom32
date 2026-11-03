# Kết quả kiểm tra bản nâng cấp

Ngày kiểm tra: 03/11/2026. Môi trường: Linux x64, .NET SDK 10.0.403, PostgreSQL 16.15, Next.js 16.4.4. Kiểm tra trên source nâng cấp trong thư mục làm việc; không sửa database đang chạy của người dùng.

| Kiểm tra | Kết quả đã quan sát |
| --- | --- |
| Backend Release build | Thành công, 0 warning, 0 error |
| Backend xUnit | 13/13 qua |
| API với PostgreSQL thật | 75/75 qua; chi tiết tại `verification/api-results.json` |
| Node/BFF/catalog checks | 10/10 qua |
| Next.js production build + TypeScript | Thành công |
| Compose YAML | Đọc được cấu trúc 7 dịch vụ; chưa chạy Docker build/up |
| Desktop browser | Trang chủ và góc học render được; ảnh/font, điều hướng và accordion bài học đã xem; không tràn ngang tại viewport quan sát |
| Mobile browser | Chưa xác minh trực quan trên viewport mobile |
| Browser E2E đầy đủ | Chưa chạy; có 5 kịch bản cho người dùng chạy tại local |

## Những hồi quy đã được kiểm tra

- Khách không đọc được Draft/Archived hoặc nguyên liệu/bước của bài riêng tư.
- Tác giả khác không sửa bài và không xóa bước của người khác.
- Metadata/nguyên liệu/bước/dinh dưỡng lưu cùng transaction; ID con không thuộc bài bị từ chối; version cũ trả 409.
- Publish yêu cầu ít nhất một bước; publish lặp idempotent; không xóa bước cuối của bài đang Published.
- Page size được chặn ở 50; input page, enum, sort và từ khóa không hợp lệ được xử lý.
- Tìm `mi` khớp “Mì” qua PostgreSQL unaccent/FTS.
- MIME/magic giả bị chặn; tên upload chứa đường dẫn không trở thành đường dẫn file trên server.
- Token rotation, reuse detection, logout; cookie HttpOnly; BFF không trả token cho JavaScript; CSRF Origin check; refresh single-flight; If-Match được giữ khi proxy; request quá lớn bị chặn.
- Hash mật khẩu cũ tiếp tục được kiểm tra, hash mới có salt và chi phí cao hơn.

## Phạm vi chưa xác minh

Không có Docker Engine để chạy toàn stack trong môi trường này. Thử khởi động .NET trong môi trường browser preview bị lỗi khởi tạo CoreCLR, trong khi cùng backend đã chạy và kiểm thử được ở môi trường kiểm thử API. Vì vậy browser preview không kết nối .NET: màn hình thông báo gián đoạn đã được quan sát, không dùng dữ liệu giả thay API để báo thành công.

Chưa nghiệm thu Google sign-in bằng tài khoản thật; SMTP giao thư; MinIO upload/delete/thumbnail AVIF; Redis output cache trong stack; Hangfire persistent queue qua restart; OTLP collector. Cấu hình và source của các chức năng này có trong bản bàn giao, nhưng không ghi “đã pass” thay cho những lần chạy chưa thực hiện.

Các test API dùng Local storage và Hangfire in-memory để kiểm tra logic/API. Chưa có kiểm thử tải, đo SLA/p95, Lighthouse, Google Rich Results, WCAG audit, kiểm tra trình duyệt mobile hay coverage toàn bộ Application ≥80%.

Đếm ca kiểm thử không tương đương đo line coverage và không được diễn giải thành tỷ lệ đáp ứng SRS. Đọc `SRS_COVERAGE.md` để thấy các khác biệt còn lại trước khi nghiệm thu theo rubric của môn học.

Production npm audit (`--omit=dev`) ghi nhận 0 advisory ở lần kiểm tra này. Không xem đây là kết quả pentest. Auth limit được tăng lên 100/phút riêng trong smoke test để kiểm tra nhiều luồng cùng phút; default source vẫn 10/phút.
