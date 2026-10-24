using System;
using System.Collections.Generic;

namespace WebBanPhanMem.Models;

public partial class Taikhoan
{
    public uint Id { get; set; }

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public string MatKhauBam { get; set; } = null!;

    public string VaiTro { get; set; } = null!;

    public DateTime? ThoiDiemXacThucEmail { get; set; }

    public bool? ConHoatDong { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgayCapNhat { get; set; }

    public virtual ICollection<Danhgium> Danhgia { get; set; } = new List<Danhgium>();

    public virtual ICollection<Donhang> Donhangs { get; set; } = new List<Donhang>();

    public virtual ICollection<Yeucauhotro> YeucauhotroNhanVienPhuTraches { get; set; } = new List<Yeucauhotro>();

    public virtual ICollection<Yeucauhotro> YeucauhotroTaiKhoans { get; set; } = new List<Yeucauhotro>();
}
