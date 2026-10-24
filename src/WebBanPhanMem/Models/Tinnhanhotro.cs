using System;
using System.Collections.Generic;

namespace WebBanPhanMem.Models;

public partial class Tinnhanhotro
{
    public uint Id { get; set; }

    public uint YeuCauHoTroId { get; set; }

    public string NguoiGui { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    public DateTime NgayTao { get; set; }

    public virtual Yeucauhotro YeuCauHoTro { get; set; } = null!;
}
