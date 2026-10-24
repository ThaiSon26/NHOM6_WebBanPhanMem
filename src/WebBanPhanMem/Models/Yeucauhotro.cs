using System;
using System.Collections.Generic;

namespace WebBanPhanMem.Models;

public partial class Yeucauhotro
{
    public uint Id { get; set; }

    public string MaYeuCau { get; set; } = null!;

    public uint? TaiKhoanId { get; set; }

    public uint? DonHangId { get; set; }

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string TieuDe { get; set; } = null!;

    public string Loai { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public uint? NhanVienPhuTrachId { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime NgayCapNhat { get; set; }

    public virtual Donhang? DonHang { get; set; }

    public virtual Taikhoan? NhanVienPhuTrach { get; set; }

    public virtual Taikhoan? TaiKhoan { get; set; }

    public virtual ICollection<Tinnhanhotro> Tinnhanhotros { get; set; } = new List<Tinnhanhotro>();
}
