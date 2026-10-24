using System;
using System.Collections.Generic;

namespace WebBanPhanMem.Models;

public partial class Doitac
{
    public uint Id { get; set; }

    public string TenDoiTac { get; set; } = null!;

    public string? NguoiLienHe { get; set; }

    public string? Email { get; set; }

    public string? SoDienThoai { get; set; }

    public string? GhiChu { get; set; }

    public bool? ConHoatDong { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual ICollection<Khokey> Khokeys { get; set; } = new List<Khokey>();

    public virtual ICollection<Sanpham> Sanphams { get; set; } = new List<Sanpham>();
}
