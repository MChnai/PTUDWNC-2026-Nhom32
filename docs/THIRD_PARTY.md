# Nguồn tài nguyên và phụ thuộc

## Ảnh

Ảnh trong `frontend/public/images` được tải từ Unsplash và tối ưu WebP. Chúng được dùng để minh họa, không phải ảnh được chứng nhận là kết quả của tác giả mẫu. Tên ảnh mô tả món/nhóm món trong starter catalog.

| File | Nguồn |
| --- | --- |
| hero.webp | https://images.unsplash.com/photo-1512621776951-a57141f2eefd |
| salad-cau-vong.webp | https://images.unsplash.com/photo-1546069901-ba9599a7e63c |
| pasta-pesto.webp | https://images.unsplash.com/photo-1473093295043-cdd812d0e601 |
| salad-ngu-coc.webp | https://images.unsplash.com/photo-1547592180-85f173990554 |
| ramen-trung.webp | https://images.unsplash.com/photo-1569718212165-3a8278d5f624 |
| salad-dau-ga.webp | https://images.unsplash.com/photo-1540189549336-e6e99c3679fe |
| pizza-rau-cu.webp | https://images.unsplash.com/photo-1565299624946-b28f40a0ae38 |
| ca-ri-ga.webp | https://images.unsplash.com/photo-1505253758473-96b7015fcd40 |

`placeholder.svg` và `src/app/icon.svg` là tài nguyên vector viết cho bản nâng cấp. Các ảnh upload từ source gốc được giữ nguyên để không làm hỏng tham chiếu dữ liệu cũ; không được dùng làm ảnh chủ đạo của giao diện mới.

## Font và icon

Be Vietnam Pro và Lora: lấy từ các package Fontsource, có subset tiếng Việt/Latin và license kèm trong `frontend/public/fonts/*LICENSE.txt`. Lucide React dùng cho icon giao diện.

## Phụ thuộc chính

- Next.js/React/TypeScript: https://nextjs.org/docs/app
- Google token verification: https://developers.google.com/identity/gsi/web/guides/verify-google-id-token
- Magick.NET: https://github.com/dlemstra/Magick.NET — giữ license của package/native dependencies khi phân phối bản build; không bao gồm chúng trực tiếp trong source zip.
- MinIO source: https://github.com/minio/minio/releases/tag/RELEASE.2025-10-15T17-29-55Z
- MinIO client source: https://github.com/minio/mc/releases/tag/RELEASE.2025-08-13T08-35-41Z
- MinIO/MC có giấy phép upstream AGPL; Dockerfile build source upstream, không thay đổi license của chúng.
- Hướng dẫn an toàn thực phẩm trong góc học: https://www.foodsafety.gov/keep-food-safe/4-steps-to-food-safety

Các dependency được khóa trong `package-lock.json`/csproj. Bản build đã loại bỏ cảnh báo package downgrade và cảnh báo NuGet phát hiện trong lần restore được ghi nhận; điều này không thay thế audit toàn bộ chuỗi phụ thuộc.
