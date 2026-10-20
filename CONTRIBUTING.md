# Hướng Dẫn Đóng Góp Dự Án - Nhóm 6

Chào mừng các thành viên tham gia phát triển dự án **Website Bán Phần Mềm Bản Quyền**.

## 1. Quy tắc làm việc với nhánh (Git Branch)
- Tuyệt đối không đẩy mã nguồn (commit/push) trực tiếp lên nhánh `main`.
- Mỗi tính năng hoặc sửa lỗi phải tạo trên một nhánh riêng biệt:
  - Tính năng mới: `feature/ten-tinh-nang` (Ví dụ: `feature/dang-nhap`, `feature/xuat-kho-key`)
  - Sửa lỗi: `fix/ten-loi` (Ví dụ: `fix/loi-gio-hang`)

## 2. Quy chuẩn đặt tên thông điệp Commit
- `feat:` Thêm một chức năng mới cho người dùng.
- `fix:` Sửa lỗi phát sinh trong chương trình.
- `docs:` Viết hoặc chỉnh sửa tài liệu hướng dẫn.
- `test:` Viết thêm các ca kiểm thử Unit Test.
- `chore:` Cài đặt thư viện, cấu hình công cụ dự án.

## 3. Quy trình gửi và duyệt Pull Request (PR)
1. Đẩy nhánh lên GitHub và tạo Pull Request vào nhánh `main`.
2. Hệ thống kiểm thử tự động (CI) sẽ chạy để kiểm tra lỗi build.
3. Người quản trị (Maintainer) sẽ đọc và nhận xét mã nguồn (Review code).
4. Sau khi được duyệt (Approve), mã nguồn mới chính thức được gộp (Merge) vào nhánh `main`.
