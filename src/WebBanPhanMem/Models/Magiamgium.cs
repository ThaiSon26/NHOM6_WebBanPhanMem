using System;
using System.Collections.Generic;

namespace WebBanPhanMem.Models;

public partial class Magiamgium
{
    public uint Id { get; set; }

    public string MaGiamGia { get; set; } = null!;

    public string? MoTa { get; set; }

    public string KieuGiam { get; set; } = null!;

    public decimal GiaTriGiam { get; set; }

    public decimal DonToiThieu { get; set; }

    public decimal? GiamToiDa { get; set; }

    public int? SoLanToiDa { get; set; }

    public int SoLanDaDung { get; set; }

    public DateTime? ThoiDiemBatDau { get; set; }

    public DateTime? ThoiDiemKetThuc { get; set; }

    public bool? ConHoatDong { get; set; }

    public virtual ICollection<Donhang> Donhangs { get; set; } = new List<Donhang>();
}
