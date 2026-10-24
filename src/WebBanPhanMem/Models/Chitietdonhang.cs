using System;
using System.Collections.Generic;

namespace WebBanPhanMem.Models;

public partial class Chitietdonhang
{
    public uint Id { get; set; }

    public uint DonHangId { get; set; }

    public uint GoiBanQuyenId { get; set; }

    public string TenSanPham { get; set; } = null!;

    public string TenGoi { get; set; } = null!;

    public decimal DonGia { get; set; }

    public int SoLuong { get; set; }

    public decimal ThanhTien { get; set; }

    public virtual Donhang DonHang { get; set; } = null!;

    public virtual Goibanquyen GoiBanQuyen { get; set; } = null!;
}
