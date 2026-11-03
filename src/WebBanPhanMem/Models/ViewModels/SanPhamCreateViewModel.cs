using System.ComponentModel.DataAnnotations;

namespace WebBanPhanMem.Models.ViewModels
{
    public class SanPhamCreateViewModel
    {
        public uint Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [MaxLength(200)]
        public string TenSanPham { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public uint DanhMucId { get; set; }

        public uint? DoiTacId { get; set; }

        [MaxLength(200)]
        public string? DuongDan { get; set; }

        [MaxLength(500)]
        public string? MoTaNgan { get; set; }

        public string? MoTaChiTiet { get; set; }

        [MaxLength(500)]
        public string? AnhDaiDien { get; set; }

        public bool LaNoiBat { get; set; } = false;

        public bool? ConHoatDong { get; set; } = true;
    }
}

