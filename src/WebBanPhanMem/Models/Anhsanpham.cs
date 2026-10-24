using System;
using System.Collections.Generic;

namespace WebBanPhanMem.Models;

public partial class Anhsanpham
{
    public uint Id { get; set; }

    public uint SanPhamId { get; set; }

    public string DuongDanAnh { get; set; } = null!;

    public int ThuTu { get; set; }

    public virtual Sanpham SanPham { get; set; } = null!;
}
