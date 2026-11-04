using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanPhanMem.Data;
using WebBanPhanMem.Models;

namespace WebBanPhanMem.Controllers
{
    [Authorize(Roles = "Quản trị,Nhân viên")]
    public class HoTroController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HoTroController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/HoTro hoặc /HoTro
        [Route("Admin/HoTro")]
        [Route("HoTro")]
        public async Task<IActionResult> Index(string? search, string? loai, string? trangThai)
        {
            var query = _context.Yeucauhotros
                .Include(y => y.TaiKhoan)
                .Include(y => y.NhanVienPhuTrach)
                .Include(y => y.DonHang)
                .Include(y => y.Tinnhanhotros)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(y => y.MaYeuCau.Contains(term)
                                      || y.HoTen.Contains(term)
                                      || y.Email.Contains(term)
                                      || y.TieuDe.Contains(term));
            }

            if (!string.IsNullOrWhiteSpace(loai))
            {
                query = query.Where(y => y.Loai == loai);
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(y => y.TrangThai == trangThai);
            }

            var list = await query.OrderByDescending(y => y.NgayCapNhat).ToListAsync();

            ViewBag.SearchTerm = search;
            ViewBag.SelectedLoai = loai;
            ViewBag.SelectedTrangThai = trangThai;

            return View(list);
        }

        // GET: /Admin/HoTro/Detail/5
        [HttpGet]
        [Route("Admin/HoTro/Detail/{id}")]
        public async Task<IActionResult> Detail(uint id)
        {
            var ticket = await _context.Yeucauhotros
                .Include(y => y.TaiKhoan)
                .Include(y => y.NhanVienPhuTrach)
                .Include(y => y.DonHang!)
                    .ThenInclude(d => d.Chitietdonhangs)
                .Include(y => y.Tinnhanhotros.OrderBy(t => t.NgayTao))
                .FirstOrDefaultAsync(y => y.Id == id);

            if (ticket == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy yêu cầu hỗ trợ!";
                return RedirectToAction(nameof(Index));
            }

            // Danh sách nhân viên để phân công phụ trách
            ViewBag.NhanVienList = await _context.Taikhoans
                .Where(t => t.VaiTro == "Nhân viên" || t.VaiTro == "Quản trị")
                .OrderBy(t => t.HoTen)
                .ToListAsync();

            return View(ticket);
        }

        // POST: Gửi tin nhắn phản hồi vào ticket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendReply(uint yeuCauId, string noiDung, string? trangThaiMoi)
        {
            if (string.IsNullOrWhiteSpace(noiDung))
            {
                TempData["ErrorMessage"] = "Nội dung phản hồi không được để trống!";
                return RedirectToAction("Detail", new { id = yeuCauId });
            }

            var ticket = await _context.Yeucauhotros.FindAsync(yeuCauId);
            if (ticket == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy yêu cầu hỗ trợ!";
                return RedirectToAction(nameof(Index));
            }

            string senderName = User.Identity?.Name ?? "Nhân viên hỗ trợ";

            var msg = new Tinnhanhotro
            {
                YeuCauHoTroId = yeuCauId,
                NguoiGui = senderName,
                NoiDung = noiDung.Trim(),
                NgayTao = DateTime.Now
            };

            _context.Tinnhanhotros.Add(msg);

            // Cập nhật trạng thái ticket nếu có chọn
            if (!string.IsNullOrWhiteSpace(trangThaiMoi))
            {
                ticket.TrangThai = trangThaiMoi;
            }
            else if (ticket.TrangThai == "Mở")
            {
                ticket.TrangThai = "Đang xử lý";
            }

            ticket.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã gửi phản hồi thành công!";
            return RedirectToAction("Detail", new { id = yeuCauId });
        }

        // POST: Cập nhật phân công và trạng thái ticket
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAssignment(uint id, uint? nhanVienPhuTrachId, string trangThai)
        {
            var ticket = await _context.Yeucauhotros.FindAsync(id);
            if (ticket == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy yêu cầu hỗ trợ!";
                return RedirectToAction(nameof(Index));
            }

            ticket.NhanVienPhuTrachId = nhanVienPhuTrachId == 0 ? null : nhanVienPhuTrachId;
            ticket.TrangThai = trangThai;
            ticket.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã cập nhật trạng thái yêu cầu #{ticket.MaYeuCau} thành công!";
            return RedirectToAction("Detail", new { id = ticket.Id });
        }
    }
}
