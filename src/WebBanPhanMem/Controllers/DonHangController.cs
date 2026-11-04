using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanPhanMem.Data;
using WebBanPhanMem.Models;

namespace WebBanPhanMem.Controllers
{
    [Authorize(Roles = "Quản trị")]
    public class DonHangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DonHangController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/DonHang hoặc /DonHang
        [Route("Admin/DonHang")]
        [Route("DonHang")]
        public async Task<IActionResult> Index(string? search, string? trangThai, DateTime? tuNgay, DateTime? denNgay)
        {
            var query = _context.Donhangs
                .Include(d => d.Chitietdonhangs)
                .Include(d => d.Hoadons)
                .Include(d => d.Thanhtoans)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(d => d.MaDonHang.Contains(term)
                                      || d.TenKhachHang.Contains(term)
                                      || d.EmailKhachHang.Contains(term)
                                      || (d.SoDienThoai != null && d.SoDienThoai.Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(d => d.TrangThai == trangThai);
            }

            if (tuNgay.HasValue)
            {
                query = query.Where(d => d.NgayTao >= tuNgay.Value.Date);
            }

            if (denNgay.HasValue)
            {
                var endOfDay = denNgay.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(d => d.NgayTao <= endOfDay);
            }

            var list = await query.OrderByDescending(d => d.NgayTao).ToListAsync();

            ViewBag.SearchTerm = search;
            ViewBag.SelectedTrangThai = trangThai;
            ViewBag.TuNgay = tuNgay?.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay?.ToString("yyyy-MM-dd");

            return View(list);
        }

        // GET: /Admin/DonHang/Detail/5
        [HttpGet]
        [Route("Admin/DonHang/Detail/{id}")]
        public async Task<IActionResult> Detail(uint id)
        {
            var donHang = await _context.Donhangs
                .Include(d => d.TaiKhoan)
                .Include(d => d.MaGiamGia)
                .Include(d => d.Chitietdonhangs)
                    .ThenInclude(ct => ct.GoiBanQuyen)
                        .ThenInclude(g => g.SanPham)
                .Include(d => d.Hoadons)
                .Include(d => d.Thanhtoans)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donHang == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng yêu cầu!";
                return RedirectToAction(nameof(Index));
            }

            // Lấy danh sách key đã gán theo các chi tiết đơn hàng
            var ctdhIds = donHang.Chitietdonhangs.Select(c => (uint?)c.Id).ToList();
            var danhSachKey = await _context.Khokeys
                .Where(k => ctdhIds.Contains(k.ChiTietDonHangId))
                .ToListAsync();

            ViewBag.DanhSachKey = danhSachKey;

            return View(donHang);
        }

        // POST: Cập nhật trạng thái đơn hàng & ghi chú quản trị
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(uint id, string trangThai, string? ghiChuQuanTri)
        {
            var donHang = await _context.Donhangs.FindAsync(id);
            if (donHang == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng!";
                return RedirectToAction(nameof(Index));
            }

            donHang.TrangThai = trangThai;
            donHang.GhiChuQuanTri = ghiChuQuanTri?.Trim();
            donHang.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã cập nhật trạng thái đơn #{donHang.MaDonHang} thành: \"{trangThai}\"!";
            return RedirectToAction("Detail", new { id = donHang.Id });
        }

        // POST: Tạo hóa đơn VAT mới cho đơn hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateHoaDon(uint donHangId, string tenDonVi, string maSoThue, string diaChi, string email, decimal? thueSuat)
        {
            var donHang = await _context.Donhangs.FindAsync(donHangId);
            if (donHang == null)
            {
                TempData["ErrorMessage"] = "Đơn hàng không tồn tại!";
                return RedirectToAction(nameof(Index));
            }

            decimal vatRate = thueSuat ?? 10.00m;
            decimal tongTien = donHang.TongThanhToan;

            var hoaDon = new Hoadon
            {
                DonHangId = donHangId,
                SoHoaDon = "HD-" + DateTime.Now.ToString("yyyyMMdd") + "-" + donHang.Id,
                TenDonVi = string.IsNullOrWhiteSpace(tenDonVi) ? donHang.TenKhachHang : tenDonVi.Trim(),
                MaSoThue = string.IsNullOrWhiteSpace(maSoThue) ? null : maSoThue.Trim(),
                DiaChi = string.IsNullOrWhiteSpace(diaChi) ? null : diaChi.Trim(),
                Email = string.IsNullOrWhiteSpace(email) ? donHang.EmailKhachHang : email.Trim(),
                SoTien = tongTien,
                ThueSuat = vatRate,
                TrangThai = "Đã phát hành",
                NgayPhatHanh = DateTime.Now,
                NgayTao = DateTime.Now
            };

            _context.Hoadons.Add(hoaDon);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã xuất hóa đơn {hoaDon.SoHoaDon} thành công!";
            return RedirectToAction("Detail", new { id = donHangId });
        }

        // GET: /Admin/DonHang/InHoaDon/5
        [HttpGet]
        [Route("Admin/DonHang/InHoaDon/{id}")]
        public async Task<IActionResult> InHoaDon(uint id)
        {
            var hoaDon = await _context.Hoadons
                .Include(h => h.DonHang)
                    .ThenInclude(d => d.Chitietdonhangs)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hoaDon == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy hóa đơn yêu cầu!";
                return RedirectToAction(nameof(Index));
            }

            return View(hoaDon);
        }
    }
}

