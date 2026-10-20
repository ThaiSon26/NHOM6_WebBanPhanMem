# Website Bán phần mềm bản quyền
Xây dựng Website bán phần mềm bản quyền là quá trình phân tích, thiết kế và lập trình một hệ thống thương mại điện tử chuyên kinh doanh các sản phẩm số (Digital Products) — cụ thể là cung cấp quyền sử dụng phần mềm hợp pháp thông qua việc phân phối các mã kích hoạt (License Key / Serial Key), tài khoản bản quyền hoặc các tệp cài đặt chính hãng đến tay người tiêu dùng qua Internet.
# Công nghệ, Thư viện và Phiên bản chi tiết
Dự án được xây dựng dựa trên các công nghệ và phiên bản cụ thể sau:
| Thành phần | Tên công nghệ / Thư viện | Phiên bản | Mục đích sử dụng |
| :--- | :--- | :---: | :--- |
| **Nền tảng cốt lõi** | .NET SDK | `8.0.x` (LTS) | Bộ công cụ phát triển phần mềm chính |
| **Ngôn ngữ lập trình** | C# | `12.0` | Ngôn ngữ lập trình chính cho Backend |
| **Kiến trúc ứng dụng**| ASP.NET Core MVC | `8.0.x` | Xây dựng ứng dụng theo mô hình Model - View - Controller |
| **Cơ sở dữ liệu** | MySQL Community Server | `8.0.x` *(hoặc MariaDB 10.4+)* | Hệ quản trị cơ sở dữ liệu quan hệ lưu trữ dữ liệu |
| **Thư viện kết nối DB**| `Pomelo.EntityFrameworkCore.MySql` | `8.0.2` | ORM ánh xạ đối tượng C# sang bảng MySQL |
| **Công cụ Migration** | `Microsoft.EntityFrameworkCore.Design`| `8.0.x` | Hỗ trợ sinh và cập nhật cấu trúc database tự động |
| **Khung kiểm thử** | xUnit | `2.8.x` | Viết và thực thi các bài kiểm thử tự động (Unit Test) |
| **Tích hợp liên tục** | GitHub Actions (`ubuntu-latest`) | `v4` | Tự động biên dịch (Build) và chạy Test mỗi khi có PR/Push |
| **Quản lý phiên bản** | Git & GitHub | Git `2.40+` | Quản lý mã nguồn phân tán và cộng tác nhóm |
| **Môi trường phát triển**| Visual Studio Code | `1.90+` | Trình soạn thảo mã nguồn chính kèm C# Dev Kit |
---
