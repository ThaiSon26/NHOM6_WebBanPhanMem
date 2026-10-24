using System;
using System.Collections.Generic;

namespace WebBanPhanMem.Models;

public partial class Donhang
{
    public uint Id { get; set; }

    public string MaDonHang { get; set; } = null!;

    public uint? TaiKhoanId { get; set; }

    public string TenKhachHang { get; set; } = null!;

    public string EmailKhachHang { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public decimal TongTienHang { get; set; }

    public decimal TienGiamGia { get; set; }

    public decimal TongThanhToan { get; set; }

    public uint? MaGiamGiaId { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? GhiChuKhach { get; set; }

    public string? GhiChuQuanTri { get; set; }

    public string? DiaChiIp { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgayCapNhat { get; set; }

    public virtual ICollection<Chitietdonhang> Chitietdonhangs { get; set; } = new List<Chitietdonhang>();

    public virtual ICollection<Hoadon> Hoadons { get; set; } = new List<Hoadon>();

    public virtual Magiamgium? MaGiamGia { get; set; }

    public virtual Taikhoan? TaiKhoan { get; set; }

    public virtual ICollection<Thanhtoan> Thanhtoans { get; set; } = new List<Thanhtoan>();

    public virtual ICollection<Yeucauhotro> Yeucauhotros { get; set; } = new List<Yeucauhotro>();
}
