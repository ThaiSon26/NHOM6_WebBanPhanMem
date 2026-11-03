using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanPhanMem.Data;
using WebBanPhanMem.Models;

namespace WebBanPhanMem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin hoặc /Admin/Index
        [Route("Admin")]
        [Route("Admin/Index")]
        [Route("Admin/Dashboard")]
        public async Task<IActionResult> Index()
        {
            var viewModel = new AdminDashboardViewModel();

            try
            {
                // Truy vấn dữ liệu thực tế từ MySQL QLBanPhanMem
                viewModel.TongDanhMuc = await _context.Danhmucs.CountAsync();
                viewModel.TongSanPham = await _context.Sanphams.CountAsync();
                viewModel.TongDonHang = await _context.Donhangs.CountAsync();
                viewModel.TongNguoiDung = await _context.Taikhoans.CountAsync();

                if (viewModel.TongDonHang > 0)
                {
                    viewModel.TongDoanhThu = await _context.Donhangs.SumAsync(d => d.TongThanhToan);
                    viewModel.DonHangMoiNhat = await _context.Donhangs
                        .OrderByDescending(d => d.NgayTao)
                        .Take(5)
                        .ToListAsync();
                }
            }
            catch
            {
                // Trong trường hợp database chưa có bản ghi hoặc đang khởi tạo
            }

            return View(viewModel);
        }
    }
}
