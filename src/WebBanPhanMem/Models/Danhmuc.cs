using System;
using System.Collections.Generic;

namespace WebBanPhanMem.Models;

public partial class Danhmuc
{
    public uint Id { get; set; }

    public uint? DanhMucChaId { get; set; }

    public string TenDanhMuc { get; set; } = null!;

    public string DuongDan { get; set; } = null!;

    public string? BieuTuong { get; set; }

    public int ThuTu { get; set; }

    public bool? ConHoatDong { get; set; }

    public virtual Danhmuc? DanhMucCha { get; set; }

    public virtual ICollection<Danhmuc> InverseDanhMucCha { get; set; } = new List<Danhmuc>();

    public virtual ICollection<Sanpham> Sanphams { get; set; } = new List<Sanpham>();
}
