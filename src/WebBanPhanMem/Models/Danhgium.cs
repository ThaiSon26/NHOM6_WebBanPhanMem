using System;
using System.Collections.Generic;

namespace WebBanPhanMem.Models;

public partial class Danhgium
{
    public uint Id { get; set; }

    public uint SanPhamId { get; set; }

    public uint? TaiKhoanId { get; set; }

    public string TenNguoiDanhGia { get; set; } = null!;

    public sbyte DiemDanhGia { get; set; }

    public string? NoiDung { get; set; }

    public bool DaDuyet { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual Sanpham SanPham { get; set; } = null!;

    public virtual Taikhoan? TaiKhoan { get; set; }
}
