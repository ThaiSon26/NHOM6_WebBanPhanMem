namespace WebBanPhanMem.Models
{
    public class AdminDashboardViewModel
    {
        public decimal TongDoanhThu { get; set; }
        public int TongDonHang { get; set; }
        public int TongNguoiDung { get; set; }
        public int TongSanPham { get; set; }
        public int TongDanhMuc { get; set; }
        public List<Donhang> DonHangMoiNhat { get; set; } = new();
    }
}
