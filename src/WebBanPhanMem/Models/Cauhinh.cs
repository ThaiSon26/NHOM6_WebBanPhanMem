using System;
using System.Collections.Generic;

namespace WebBanPhanMem.Models;

public partial class Cauhinh
{
    public uint Id { get; set; }

    public string Khoa { get; set; } = null!;

    public string? GiaTri { get; set; }
}
