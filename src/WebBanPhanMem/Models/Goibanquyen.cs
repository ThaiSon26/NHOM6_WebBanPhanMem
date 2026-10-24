using System;
using System.Collections.Generic;

namespace WebBanPhanMem.Models;

public partial class Goibanquyen
{
    public uint Id { get; set; }

    public uint SanPhamId { get; set; }

    public string MaHang { get; set; } = null!;

    public string TenGoiBanQuyen { get; set; } = null!;

    public int? ThoiHanNgay { get; set; }

    public int SoMayToiDa { get; set; }

    public int BaoHanhNgay { get; set; }

    public decimal Gia { get; set; }

    public decimal? GiaKhuyenMai { get; set; }

    public int SoKeyConLai { get; set; }

    public bool? ConHoatDong { get; set; }

    public virtual ICollection<Chitietdonhang> Chitietdonhangs { get; set; } = new List<Chitietdonhang>();

    public virtual ICollection<Khokey> Khokeys { get; set; } = new List<Khokey>();

    public virtual Sanpham SanPham { get; set; } = null!;
}
