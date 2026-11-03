# Frontend — Next.js App Router

Chạy `npm ci` và `npm run dev` tại **thư mục gốc repository**. Không còn dùng Vite. Cấu hình local: sao chép `.env.example` trong thư mục này thành `.env.local`.

`BACKEND_URL` chỉ dùng phía server. `/api/v1/*` được route handler proxy tới .NET và quản lý cookie HttpOnly. Các trang công khai gọi API từ Server Components; không chuyển sang dữ liệu giả khi API không hoạt động.

- `src/app`: route trang, SEO, API proxy.
- `src/components`: bố cục, tìm kiếm, hướng dẫn nấu, tác giả và xác thực.
- `src/lib`: hợp đồng dữ liệu, API client và sổ tay local.
- `public/images`, `public/fonts`: tài nguyên cục bộ; nguồn tại `../docs/THIRD_PARTY.md`.

Tham khảo README ở thư mục gốc để chạy đầy đủ PostgreSQL/.NET/Redis/MinIO/SMTP.
