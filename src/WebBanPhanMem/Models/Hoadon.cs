using System;
using System.Collections.Generic;

namespace WebBanPhanMem.Models;

public partial class Hoadon
{
    public uint Id { get; set; }

    public uint DonHangId { get; set; }

    public string? SoHoaDon { get; set; }

    public string TenDonVi { get; set; } = null!;

    public string? MaSoThue { get; set; }

    public string? DiaChi { get; set; }

    public string? Email { get; set; }

    public decimal SoTien { get; set; }

    public decimal ThueSuat { get; set; }

    public string TrangThai { get; set; } = null!;

    public DateTime? NgayPhatHanh { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual Donhang DonHang { get; set; } = null!;
}
