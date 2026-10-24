using System;
using System.Collections.Generic;

namespace WebBanPhanMem.Models;

public partial class Sanpham
{
    public uint Id { get; set; }

    public uint DanhMucId { get; set; }

    public uint? DoiTacId { get; set; }

    public string TenSanPham { get; set; } = null!;

    public string DuongDan { get; set; } = null!;

    public string? MoTaNgan { get; set; }

    public string? MoTaChiTiet { get; set; }

    public string? AnhDaiDien { get; set; }

    public uint LuotXem { get; set; }

    public bool LaNoiBat { get; set; }

    public bool? ConHoatDong { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgayCapNhat { get; set; }

    public virtual ICollection<Anhsanpham> Anhsanphams { get; set; } = new List<Anhsanpham>();

    public virtual Danhmuc DanhMuc { get; set; } = null!;

    public virtual ICollection<Danhgium> Danhgia { get; set; } = new List<Danhgium>();

    public virtual Doitac? DoiTac { get; set; }

    public virtual ICollection<Goibanquyen> Goibanquyens { get; set; } = new List<Goibanquyen>();
}
