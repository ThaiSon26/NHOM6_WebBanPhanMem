using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;
using WebBanPhanMem.Models;

namespace WebBanPhanMem.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Anhsanpham> Anhsanphams { get; set; }

    public virtual DbSet<Cauhinh> Cauhinhs { get; set; }

    public virtual DbSet<Chitietdonhang> Chitietdonhangs { get; set; }

    public virtual DbSet<Danhgium> Danhgia { get; set; }

    public virtual DbSet<Danhmuc> Danhmucs { get; set; }

    public virtual DbSet<Doitac> Doitacs { get; set; }

    public virtual DbSet<Donhang> Donhangs { get; set; }

    public virtual DbSet<Goibanquyen> Goibanquyens { get; set; }

    public virtual DbSet<Hoadon> Hoadons { get; set; }

    public virtual DbSet<Khokey> Khokeys { get; set; }

    public virtual DbSet<Magiamgium> Magiamgia { get; set; }

    public virtual DbSet<Sanpham> Sanphams { get; set; }

    public virtual DbSet<Taikhoan> Taikhoans { get; set; }

    public virtual DbSet<Thanhtoan> Thanhtoans { get; set; }

    public virtual DbSet<Tinnhanhotro> Tinnhanhotros { get; set; }

    public virtual DbSet<Yeucauhotro> Yeucauhotros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;port=3306;database=QLBanPhanMem;user=ThaiSon;password=SonThai@#01022004", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.46-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_unicode_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Anhsanpham>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("anhsanpham");

            entity.HasIndex(e => e.SanPhamId, "idx_asp_san_pham");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DuongDanAnh).HasMaxLength(255);
            entity.Property(e => e.SanPhamId).HasColumnName("SanPham_id");

            entity.HasOne(d => d.SanPham).WithMany(p => p.Anhsanphams)
                .HasForeignKey(d => d.SanPhamId)
                .HasConstraintName("fk_asp_san_pham");
        });

        modelBuilder.Entity<Cauhinh>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("cauhinh");

            entity.HasIndex(e => e.Khoa, "uq_cau_hinh_khoa").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.GiaTri).HasColumnType("text");
            entity.Property(e => e.Khoa).HasMaxLength(100);
        });

        modelBuilder.Entity<Chitietdonhang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("chitietdonhang");

            entity.HasIndex(e => e.GoiBanQuyenId, "fk_ctdh_goi");

            entity.HasIndex(e => e.DonHangId, "idx_ctdh_don_hang");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DonGia).HasPrecision(12, 2);
            entity.Property(e => e.DonHangId).HasColumnName("DonHang_id");
            entity.Property(e => e.GoiBanQuyenId).HasColumnName("GoiBanQuyen_id");
            entity.Property(e => e.SoLuong).HasDefaultValueSql("'1'");
            entity.Property(e => e.TenGoi).HasMaxLength(200);
            entity.Property(e => e.TenSanPham).HasMaxLength(200);
            entity.Property(e => e.ThanhTien).HasPrecision(12, 2);

            entity.HasOne(d => d.DonHang).WithMany(p => p.Chitietdonhangs)
                .HasForeignKey(d => d.DonHangId)
                .HasConstraintName("fk_ctdh_don_hang");

            entity.HasOne(d => d.GoiBanQuyen).WithMany(p => p.Chitietdonhangs)
                .HasForeignKey(d => d.GoiBanQuyenId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ctdh_goi");
        });

        modelBuilder.Entity<Danhgium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("danhgia");

            entity.HasIndex(e => e.TaiKhoanId, "fk_danh_gia_tai_khoan");

            entity.HasIndex(e => new { e.SanPhamId, e.DaDuyet }, "idx_danh_gia_san_pham");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.NoiDung).HasColumnType("text");
            entity.Property(e => e.SanPhamId).HasColumnName("SanPham_id");
            entity.Property(e => e.TaiKhoanId).HasColumnName("TaiKhoan_id");
            entity.Property(e => e.TenNguoiDanhGia).HasMaxLength(150);

            entity.HasOne(d => d.SanPham).WithMany(p => p.Danhgia)
                .HasForeignKey(d => d.SanPhamId)
                .HasConstraintName("fk_danh_gia_san_pham");

            entity.HasOne(d => d.TaiKhoan).WithMany(p => p.Danhgia)
                .HasForeignKey(d => d.TaiKhoanId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_danh_gia_tai_khoan");
        });

        modelBuilder.Entity<Danhmuc>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("danhmuc");

            entity.HasIndex(e => e.DanhMucChaId, "idx_danh_muc_cha");

            entity.HasIndex(e => e.DuongDan, "uq_danh_muc_duong_dan").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BieuTuong).HasMaxLength(100);
            entity.Property(e => e.ConHoatDong)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.DanhMucChaId).HasColumnName("DanhMucCha_id");
            entity.Property(e => e.DuongDan).HasMaxLength(140);
            entity.Property(e => e.TenDanhMuc).HasMaxLength(120);

            entity.HasOne(d => d.DanhMucCha).WithMany(p => p.InverseDanhMucCha)
                .HasForeignKey(d => d.DanhMucChaId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_danh_muc_cha");
        });

        modelBuilder.Entity<Doitac>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("doitac");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConHoatDong)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.Email).HasMaxLength(190);
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.NguoiLienHe).HasMaxLength(120);
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TenDoiTac).HasMaxLength(150);
        });

        modelBuilder.Entity<Donhang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("donhang");

            entity.HasIndex(e => e.MaGiamGiaId, "fk_don_hang_ma_giam_gia");

            entity.HasIndex(e => e.TaiKhoanId, "idx_don_hang_tai_khoan");

            entity.HasIndex(e => e.TrangThai, "idx_don_hang_trang_thai");

            entity.HasIndex(e => e.MaDonHang, "uq_don_hang_ma").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DiaChiIp)
                .HasMaxLength(45)
                .HasColumnName("DiaChi_ip");
            entity.Property(e => e.EmailKhachHang)
                .HasMaxLength(190)
                .HasColumnName("Email_KhachHang");
            entity.Property(e => e.GhiChuKhach).HasMaxLength(500);
            entity.Property(e => e.GhiChuQuanTri).HasMaxLength(500);
            entity.Property(e => e.MaDonHang).HasMaxLength(30);
            entity.Property(e => e.MaGiamGiaId).HasColumnName("MaGiamGia_id");
            entity.Property(e => e.NgayCapNhat)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.TaiKhoanId).HasColumnName("TaiKhoan_id");
            entity.Property(e => e.TenKhachHang).HasMaxLength(150);
            entity.Property(e => e.TienGiamGia).HasPrecision(12, 2);
            entity.Property(e => e.TongThanhToan).HasPrecision(12, 2);
            entity.Property(e => e.TongTienHang).HasPrecision(12, 2);
            entity.Property(e => e.TrangThai)
                .HasDefaultValueSql("'Chờ xử lý'")
                .HasColumnType("enum('Chờ xử lý','Đã thanh toán','Đang xử lý','Hoàn thành','Đã hủy','Hoàn tiền')");

            entity.HasOne(d => d.MaGiamGia).WithMany(p => p.Donhangs)
                .HasForeignKey(d => d.MaGiamGiaId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_don_hang_ma_giam_gia");

            entity.HasOne(d => d.TaiKhoan).WithMany(p => p.Donhangs)
                .HasForeignKey(d => d.TaiKhoanId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_don_hang_tai_khoan");
        });

        modelBuilder.Entity<Goibanquyen>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("goibanquyen");

            entity.HasIndex(e => e.SanPhamId, "idx_goi_san_pham");

            entity.HasIndex(e => e.MaHang, "uq_goi_ma_hang").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BaoHanhNgay).HasDefaultValueSql("'365'");
            entity.Property(e => e.ConHoatDong)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.Gia).HasPrecision(12, 2);
            entity.Property(e => e.GiaKhuyenMai).HasPrecision(12, 2);
            entity.Property(e => e.MaHang).HasMaxLength(60);
            entity.Property(e => e.SanPhamId).HasColumnName("SanPham_id");
            entity.Property(e => e.SoMayToiDa).HasDefaultValueSql("'1'");
            entity.Property(e => e.TenGoiBanQuyen).HasMaxLength(200);

            entity.HasOne(d => d.SanPham).WithMany(p => p.Goibanquyens)
                .HasForeignKey(d => d.SanPhamId)
                .HasConstraintName("fk_goi_san_pham");
        });

        modelBuilder.Entity<Hoadon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("hoadon");

            entity.HasIndex(e => e.DonHangId, "idx_hoa_don_don_hang");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DiaChi).HasMaxLength(300);
            entity.Property(e => e.DonHangId).HasColumnName("DonHang_id");
            entity.Property(e => e.Email).HasMaxLength(190);
            entity.Property(e => e.MaSoThue).HasMaxLength(20);
            entity.Property(e => e.NgayPhatHanh).HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.SoHoaDon).HasMaxLength(50);
            entity.Property(e => e.SoTien).HasPrecision(12, 2);
            entity.Property(e => e.TenDonVi).HasMaxLength(200);
            entity.Property(e => e.ThueSuat)
                .HasPrecision(5, 2)
                .HasDefaultValueSql("'10.00'");
            entity.Property(e => e.TrangThai)
                .HasDefaultValueSql("'Nháp'")
                .HasColumnType("enum('Nháp','Đã phát hành','Đã hủy')");

            entity.HasOne(d => d.DonHang).WithMany(p => p.Hoadons)
                .HasForeignKey(d => d.DonHangId)
                .HasConstraintName("fk_hoa_don_don_hang");
        });

        modelBuilder.Entity<Khokey>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("khokey");

            entity.HasIndex(e => e.DoiTacId, "fk_key_doi_tac");

            entity.HasIndex(e => e.ChiTietDonHangId, "idx_key_chi_tiet_don");

            entity.HasIndex(e => new { e.GoiBanQuyenId, e.TrangThai }, "idx_key_goi_trang_thai");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChiTietDonHangId).HasColumnName("ChiTietDonHang_id");
            entity.Property(e => e.DoiTacId).HasColumnName("DoiTac_id");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.GoiBanQuyenId).HasColumnName("GoiBanQuyen_id");
            entity.Property(e => e.LoaiKey)
                .HasDefaultValueSql("'Mã kích hoạt'")
                .HasColumnType("enum('Mã kích hoạt','Tài khoản','Link tải')");
            entity.Property(e => e.MaKey).HasMaxLength(500);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.ThoiDiemGiaoKey).HasColumnType("datetime");
            entity.Property(e => e.TrangThai)
                .HasDefaultValueSql("'Còn trống'")
                .HasColumnType("enum('Còn trống','Đã giữ chỗ','Đã bán','Lỗi','Thu hồi')");

            entity.HasOne(d => d.DoiTac).WithMany(p => p.Khokeys)
                .HasForeignKey(d => d.DoiTacId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_key_doi_tac");

            entity.HasOne(d => d.GoiBanQuyen).WithMany(p => p.Khokeys)
                .HasForeignKey(d => d.GoiBanQuyenId)
                .HasConstraintName("fk_key_goi");
        });

        modelBuilder.Entity<Magiamgium>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("magiamgia");

            entity.HasIndex(e => new { e.ConHoatDong, e.ThoiDiemKetThuc }, "idx_ma_giam_gia_hoat_dong");

            entity.HasIndex(e => e.MaGiamGia, "uq_ma_giam_gia").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConHoatDong)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.DonToiThieu).HasPrecision(12, 2);
            entity.Property(e => e.GiaTriGiam).HasPrecision(10, 2);
            entity.Property(e => e.GiamToiDa).HasPrecision(12, 2);
            entity.Property(e => e.KieuGiam).HasColumnType("enum('Phần trăm','Số tiền')");
            entity.Property(e => e.MaGiamGia).HasMaxLength(40);
            entity.Property(e => e.MoTa).HasMaxLength(255);
            entity.Property(e => e.ThoiDiemBatDau).HasColumnType("datetime");
            entity.Property(e => e.ThoiDiemKetThuc).HasColumnType("datetime");
        });

        modelBuilder.Entity<Sanpham>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("sanpham");

            entity.HasIndex(e => e.DoiTacId, "fk_san_pham_doi_tac");

            entity.HasIndex(e => e.DanhMucId, "idx_san_pham_danh_muc");

            entity.HasIndex(e => e.TenSanPham, "idx_san_pham_tim_kiem");

            entity.HasIndex(e => e.DuongDan, "uq_san_pham_duong_dan").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnhDaiDien).HasMaxLength(255);
            entity.Property(e => e.ConHoatDong)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.DanhMucId).HasColumnName("DanhMuc_id");
            entity.Property(e => e.DoiTacId).HasColumnName("DoiTac_id");
            entity.Property(e => e.DuongDan).HasMaxLength(220);
            entity.Property(e => e.MoTaChiTiet).HasColumnType("text");
            entity.Property(e => e.MoTaNgan).HasMaxLength(500);
            entity.Property(e => e.NgayCapNhat)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.TenSanPham).HasMaxLength(200);

            entity.HasOne(d => d.DanhMuc).WithMany(p => p.Sanphams)
                .HasForeignKey(d => d.DanhMucId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_san_pham_danh_muc");

            entity.HasOne(d => d.DoiTac).WithMany(p => p.Sanphams)
                .HasForeignKey(d => d.DoiTacId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_san_pham_doi_tac");
        });

        modelBuilder.Entity<Taikhoan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("taikhoan");

            entity.HasIndex(e => e.SoDienThoai, "idx_tai_khoan_sdt");

            entity.HasIndex(e => e.Email, "uq_tai_khoan_email").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConHoatDong)
                .IsRequired()
                .HasDefaultValueSql("'1'");
            entity.Property(e => e.Email).HasMaxLength(190);
            entity.Property(e => e.HoTen).HasMaxLength(150);
            entity.Property(e => e.MatKhauBam).HasMaxLength(255);
            entity.Property(e => e.NgayCapNhat)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.SoDienThoai).HasMaxLength(20);
            entity.Property(e => e.ThoiDiemXacThucEmail).HasColumnType("datetime");
            entity.Property(e => e.VaiTro)
                .HasDefaultValueSql("'Khách hàng'")
                .HasColumnType("enum('Khách hàng','Nhân viên','Quản trị')");
        });

        modelBuilder.Entity<Thanhtoan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("thanhtoan");

            entity.HasIndex(e => e.DonHangId, "idx_thanh_toan_don_hang");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DonHangId).HasColumnName("DonHang_id");
            entity.Property(e => e.MaGiaoDich).HasMaxLength(100);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.PhanHoiCongThanhToan).HasColumnType("json");
            entity.Property(e => e.PhuongThuc).HasColumnType("enum('Chuyển khoản','MoMo','VNPay','ZaloPay','Thẻ quốc tế','Tiền mặt')");
            entity.Property(e => e.SoTien).HasPrecision(12, 2);
            entity.Property(e => e.ThoiDiemThanhToan).HasColumnType("datetime");
            entity.Property(e => e.TrangThai)
                .HasDefaultValueSql("'Chờ xử lý'")
                .HasColumnType("enum('Chờ xử lý','Thành công','Thất bại','Hoàn tiền')");

            entity.HasOne(d => d.DonHang).WithMany(p => p.Thanhtoans)
                .HasForeignKey(d => d.DonHangId)
                .HasConstraintName("fk_thanh_toan_don_hang");
        });

        modelBuilder.Entity<Tinnhanhotro>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("tinnhanhotro");

            entity.HasIndex(e => e.YeuCauHoTroId, "idx_tin_nhan_yeu_cau");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.NguoiGui).HasColumnType("enum('Khách hàng','Nhân viên')");
            entity.Property(e => e.NoiDung).HasColumnType("text");
            entity.Property(e => e.YeuCauHoTroId).HasColumnName("YeuCauHoTro_id");

            entity.HasOne(d => d.YeuCauHoTro).WithMany(p => p.Tinnhanhotros)
                .HasForeignKey(d => d.YeuCauHoTroId)
                .HasConstraintName("fk_tin_nhan_yeu_cau");
        });

        modelBuilder.Entity<Yeucauhotro>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("yeucauhotro");

            entity.HasIndex(e => e.DonHangId, "fk_yeu_cau_don_hang");

            entity.HasIndex(e => e.NhanVienPhuTrachId, "fk_yeu_cau_nhan_vien");

            entity.HasIndex(e => e.TaiKhoanId, "fk_yeu_cau_tai_khoan");

            entity.HasIndex(e => e.TrangThai, "idx_yeu_cau_trang_thai");

            entity.HasIndex(e => e.MaYeuCau, "uq_yeu_cau_ma").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.DonHangId).HasColumnName("DonHang_id");
            entity.Property(e => e.Email).HasMaxLength(190);
            entity.Property(e => e.HoTen).HasMaxLength(150);
            entity.Property(e => e.Loai)
                .HasDefaultValueSql("'Khác'")
                .HasColumnType("enum('Kích hoạt','Bảo hành','Hoàn tiền','Khác')");
            entity.Property(e => e.MaYeuCau).HasMaxLength(30);
            entity.Property(e => e.NgayCapNhat)
                .ValueGeneratedOnAddOrUpdate()
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("datetime");
            entity.Property(e => e.NhanVienPhuTrachId).HasColumnName("NhanVienPhuTrach_id");
            entity.Property(e => e.TaiKhoanId).HasColumnName("TaiKhoan_id");
            entity.Property(e => e.TieuDe).HasMaxLength(255);
            entity.Property(e => e.TrangThai)
                .HasDefaultValueSql("'Mở'")
                .HasColumnType("enum('Mở','Đang xử lý','Đã giải quyết','Đã đóng')");

            entity.HasOne(d => d.DonHang).WithMany(p => p.Yeucauhotros)
                .HasForeignKey(d => d.DonHangId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_yeu_cau_don_hang");

            entity.HasOne(d => d.NhanVienPhuTrach).WithMany(p => p.YeucauhotroNhanVienPhuTraches)
                .HasForeignKey(d => d.NhanVienPhuTrachId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_yeu_cau_nhan_vien");

            entity.HasOne(d => d.TaiKhoan).WithMany(p => p.YeucauhotroTaiKhoans)
                .HasForeignKey(d => d.TaiKhoanId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_yeu_cau_tai_khoan");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
