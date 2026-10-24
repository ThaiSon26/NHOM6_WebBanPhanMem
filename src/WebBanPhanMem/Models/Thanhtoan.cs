using System;
using System.Collections.Generic;

namespace WebBanPhanMem.Models;

public partial class Thanhtoan
{
    public uint Id { get; set; }

    public uint DonHangId { get; set; }

    public string PhuongThuc { get; set; } = null!;

    public decimal SoTien { get; set; }

    public string? MaGiaoDich { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? PhanHoiCongThanhToan { get; set; }

    public DateTime? ThoiDiemThanhToan { get; set; }

    public DateTime NgayTao { get; set; }

    public virtual Donhang DonHang { get; set; } = null!;
}
