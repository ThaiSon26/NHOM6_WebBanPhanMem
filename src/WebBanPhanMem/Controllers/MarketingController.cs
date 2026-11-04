using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanPhanMem.Data;
using WebBanPhanMem.Models;

namespace WebBanPhanMem.Controllers
{
    [Authorize(Roles = "Quản trị")]
    public class MarketingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MarketingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/Marketing
        [Route("Admin/Marketing")]
        [Route("Marketing")]
        public async Task<IActionResult> Index(string? tab, string? search, bool? conHoatDong, bool? daDuyet, uint? sanPhamId)
        {
            tab = string.IsNullOrWhiteSpace(tab) ? "voucher" : tab.ToLower();
            ViewBag.ActiveTab = tab;

            // Nạp dữ liệu Tab 1: Mã giảm giá
            var voucherQuery = _context.Magiamgia
                .Include(m => m.Donhangs)
                .AsQueryable();

            if (tab == "voucher")
            {
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var term = search.Trim();
                    voucherQuery = voucherQuery.Where(m => m.MaGiamGia.Contains(term) || (m.MoTa != null && m.MoTa.Contains(term)));
                }
                if (conHoatDong.HasValue)
                {
                    voucherQuery = voucherQuery.Where(m => m.ConHoatDong == conHoatDong.Value);
                }
            }
            var vouchers = await voucherQuery.OrderByDescending(m => m.Id).ToListAsync();

            // Nạp dữ liệu Tab 2: Đánh giá
            var reviewQuery = _context.Danhgia
                .Include(d => d.SanPham)
                .Include(d => d.TaiKhoan)
                .AsQueryable();

            if (tab == "review")
            {
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var term = search.Trim();
                    reviewQuery = reviewQuery.Where(d => d.TenNguoiDanhGia.Contains(term) 
                                                      || (d.NoiDung != null && d.NoiDung.Contains(term))
                                                      || d.SanPham.TenSanPham.Contains(term));
                }
                if (daDuyet.HasValue)
                {
                    reviewQuery = reviewQuery.Where(d => d.DaDuyet == daDuyet.Value);
                }
                if (sanPhamId.HasValue && sanPhamId > 0)
                {
                    reviewQuery = reviewQuery.Where(d => d.SanPhamId == sanPhamId.Value);
                }
            }
            var reviews = await reviewQuery.OrderByDescending(d => d.NgayTao).ToListAsync();

            ViewBag.Vouchers = vouchers;
            ViewBag.Reviews = reviews;
            ViewBag.SanPhamList = await _context.Sanphams.OrderBy(s => s.TenSanPham).ToListAsync();

            ViewBag.SearchTerm = search;
            ViewBag.SelectedStatus = conHoatDong;
            ViewBag.SelectedDaDuyet = daDuyet;
            ViewBag.SelectedSanPhamId = sanPhamId;

            return View();
        }

        // ===================== CRUD MÃ GIẢM GIÁ (VOUCHER) =====================

        // POST: Tạo mã giảm giá mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateVoucher(string maGiamGia, string? moTa, string kieuGiam, decimal giaTriGiam, decimal donToiThieu, decimal? giamToiDa, int? soLanToiDa, DateTime? thoiDiemBatDau, DateTime? thoiDiemKetThuc, bool conHoatDong)
        {
            if (string.IsNullOrWhiteSpace(maGiamGia) || giaTriGiam <= 0)
            {
                TempData["ErrorMessage"] = "Mã voucher và giá trị giảm không hợp lệ!";
                return RedirectToAction(nameof(Index), new { tab = "voucher" });
            }

            var cleanCode = maGiamGia.Trim().ToUpper();
            bool exists = await _context.Magiamgia.AnyAsync(m => m.MaGiamGia == cleanCode);
            if (exists)
            {
                TempData["ErrorMessage"] = $"Mã voucher \"{cleanCode}\" đã tồn tại!";
                return RedirectToAction(nameof(Index), new { tab = "voucher" });
            }

            var voucher = new Magiamgium
            {
                MaGiamGia = cleanCode,
                MoTa = moTa?.Trim(),
                KieuGiam = (kieuGiam == "Số tiền") ? "Số tiền" : "Phần trăm",
                GiaTriGiam = giaTriGiam,
                DonToiThieu = donToiThieu >= 0 ? donToiThieu : 0,
                GiamToiDa = giamToiDa,
                SoLanToiDa = soLanToiDa,
                SoLanDaDung = 0,
                ThoiDiemBatDau = thoiDiemBatDau,
                ThoiDiemKetThuc = thoiDiemKetThuc,
                ConHoatDong = conHoatDong
            };

            _context.Magiamgia.Add(voucher);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã tạo mã giảm giá \"{voucher.MaGiamGia}\" thành công!";
            return RedirectToAction(nameof(Index), new { tab = "voucher" });
        }

        // POST: Cập nhật mã giảm giá
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditVoucher(uint id, string? moTa, string kieuGiam, decimal giaTriGiam, decimal donToiThieu, decimal? giamToiDa, int? soLanToiDa, DateTime? thoiDiemBatDau, DateTime? thoiDiemKetThuc, bool conHoatDong)
        {
            var voucher = await _context.Magiamgia.FindAsync(id);
            if (voucher == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy mã giảm giá!";
                return RedirectToAction(nameof(Index), new { tab = "voucher" });
            }

            voucher.MoTa = moTa?.Trim();
            voucher.KieuGiam = (kieuGiam == "Số tiền") ? "Số tiền" : "Phần trăm";
            voucher.GiaTriGiam = giaTriGiam;
            voucher.DonToiThieu = donToiThieu >= 0 ? donToiThieu : 0;
            voucher.GiamToiDa = giamToiDa;
            voucher.SoLanToiDa = soLanToiDa;
            voucher.ThoiDiemBatDau = thoiDiemBatDau;
            voucher.ThoiDiemKetThuc = thoiDiemKetThuc;
            voucher.ConHoatDong = conHoatDong;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Cập nhật mã giảm giá \"{voucher.MaGiamGia}\" thành công!";
            return RedirectToAction(nameof(Index), new { tab = "voucher" });
        }

        // POST: Khóa / Mở voucher nhanh
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleVoucherStatus(uint id)
        {
            var voucher = await _context.Magiamgia.FindAsync(id);
            if (voucher != null)
            {
                voucher.ConHoatDong = !(voucher.ConHoatDong ?? true);
                await _context.SaveChangesAsync();
                string statusText = (voucher.ConHoatDong == true) ? "kích hoạt" : "tạm dừng";
                TempData["SuccessMessage"] = $"Đã {statusText} mã giảm giá \"{voucher.MaGiamGia}\"!";
            }
            return RedirectToAction(nameof(Index), new { tab = "voucher" });
        }

        // POST: Xóa voucher
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteVoucher(uint id)
        {
            var voucher = await _context.Magiamgia.Include(m => m.Donhangs).FirstOrDefaultAsync(m => m.Id == id);
            if (voucher == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy voucher để xóa!";
                return RedirectToAction(nameof(Index), new { tab = "voucher" });
            }

            if (voucher.Donhangs.Any())
            {
                TempData["ErrorMessage"] = $"Mã \"{voucher.MaGiamGia}\" đã được dùng cho đơn hàng, không thể xóa (hãy tắt hoạt động)!";
                return RedirectToAction(nameof(Index), new { tab = "voucher" });
            }

            _context.Magiamgia.Remove(voucher);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã xóa mã voucher \"{voucher.MaGiamGia}\"!";
            return RedirectToAction(nameof(Index), new { tab = "voucher" });
        }

        // ===================== KIỂM DUYỆT ĐÁNH GIÁ (REVIEWS) =====================

        // POST: Duyệt / Bỏ duyệt đánh giá
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleReviewStatus(uint id)
        {
            var review = await _context.Danhgia.FindAsync(id);
            if (review != null)
            {
                review.DaDuyet = !review.DaDuyet;
                await _context.SaveChangesAsync();

                string statusText = review.DaDuyet ? "duyệt hiển thị" : "ẩn / chưa duyệt";
                TempData["SuccessMessage"] = $"Đã chuyển đánh giá thành: [{statusText}]!";
            }
            return RedirectToAction(nameof(Index), new { tab = "review" });
        }

        // POST: Xóa đánh giá spam
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReview(uint id)
        {
            var review = await _context.Danhgia.FindAsync(id);
            if (review != null)
            {
                _context.Danhgia.Remove(review);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa đánh giá thành công!";
            }
            return RedirectToAction(nameof(Index), new { tab = "review" });
        }
    }
}

