using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanPhanMem.Data;
using WebBanPhanMem.Helpers;
using WebBanPhanMem.Models;

namespace WebBanPhanMem.Controllers
{
    [Authorize(Roles = "Quản trị")]
    public class NguoiDungController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NguoiDungController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/NguoiDung hoặc /Admin/TaiKhoan
        [Route("Admin/NguoiDung")]
        [Route("Admin/TaiKhoan")]
        [Route("NguoiDung")]
        public async Task<IActionResult> Index(string? search, string? vaiTro, bool? conHoatDong)
        {
            var query = _context.Taikhoans
                .Include(t => t.Donhangs)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(t => t.HoTen.Contains(term)
                                      || t.Email.Contains(term)
                                      || (t.SoDienThoai != null && t.SoDienThoai.Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(vaiTro))
            {
                query = query.Where(t => t.VaiTro == vaiTro);
            }

            if (conHoatDong.HasValue)
            {
                query = query.Where(t => t.ConHoatDong == conHoatDong.Value);
            }

            var list = await query.OrderByDescending(t => t.NgayTao).ToListAsync();

            ViewBag.SearchTerm = search;
            ViewBag.SelectedVaiTro = vaiTro;
            ViewBag.SelectedStatus = conHoatDong;

            return View(list);
        }

        // GET: /Admin/NguoiDung/Detail/5
        [HttpGet]
        [Route("Admin/NguoiDung/Detail/{id}")]
        public async Task<IActionResult> Detail(uint id)
        {
            var user = await _context.Taikhoans
                .Include(t => t.Donhangs)
                    .ThenInclude(d => d.Chitietdonhangs)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng yêu cầu!";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // POST: Tạo tài khoản mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string hoTen, string email, string matKhau, string? soDienThoai, string vaiTro, bool conHoatDong)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(matKhau) || string.IsNullOrWhiteSpace(hoTen))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ Họ tên, Email và Mật khẩu!";
                return RedirectToAction(nameof(Index));
            }

            var trimmedEmail = email.Trim().ToLower();
            bool emailExists = await _context.Taikhoans.AnyAsync(t => t.Email == trimmedEmail);
            if (emailExists)
            {
                TempData["ErrorMessage"] = $"Email \"{trimmedEmail}\" đã tồn tại trên hệ thống!";
                return RedirectToAction(nameof(Index));
            }

            var newUser = new Taikhoan
            {
                HoTen = hoTen.Trim(),
                Email = trimmedEmail,
                SoDienThoai = string.IsNullOrWhiteSpace(soDienThoai) ? null : soDienThoai.Trim(),
                MatKhauBam = PasswordHelper.HashPassword(matKhau.Trim()),
                VaiTro = string.IsNullOrWhiteSpace(vaiTro) ? "Khách hàng" : vaiTro,
                ConHoatDong = conHoatDong,
                ThoiDiemXacThucEmail = DateTime.Now,
                NgayTao = DateTime.Now,
                NgayCapNhat = DateTime.Now
            };

            _context.Taikhoans.Add(newUser);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã tạo tài khoản cho \"{newUser.HoTen}\" với vai trò [{newUser.VaiTro}] thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Chỉnh sửa thông tin & phân quyền
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(uint id, string hoTen, string? soDienThoai, string vaiTro, bool conHoatDong)
        {
            var user = await _context.Taikhoans.FindAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản!";
                return RedirectToAction(nameof(Index));
            }

            // Ngăn chặn tự đổi vai trò hoặc tự khóa tài khoản Admin đang đăng nhập
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId != null && uint.TryParse(currentUserId, out uint myId) && myId == id)
            {
                if (!conHoatDong)
                {
                    TempData["ErrorMessage"] = "Bạn không thể tự khóa tài khoản của chính mình!";
                    return RedirectToAction(nameof(Index));
                }
                if (vaiTro != "Quản trị")
                {
                    TempData["ErrorMessage"] = "Bạn không thể tự hạ quyền Quản trị của chính mình!";
                    return RedirectToAction(nameof(Index));
                }
            }

            user.HoTen = hoTen.Trim();
            user.SoDienThoai = string.IsNullOrWhiteSpace(soDienThoai) ? null : soDienThoai.Trim();
            user.VaiTro = vaiTro;
            user.ConHoatDong = conHoatDong;
            user.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Cập nhật tài khoản \"{user.Email}\" thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Đặt lại mật khẩu mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(uint id, string matKhauMoi)
        {
            if (string.IsNullOrWhiteSpace(matKhauMoi) || matKhauMoi.Length < 6)
            {
                TempData["ErrorMessage"] = "Mật khẩu mới phải từ 6 ký tự trở lên!";
                return RedirectToAction(nameof(Index));
            }

            var user = await _context.Taikhoans.FindAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản!";
                return RedirectToAction(nameof(Index));
            }

            user.MatKhauBam = PasswordHelper.HashPassword(matKhauMoi.Trim());
            user.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã đặt lại mật khẩu mới cho tài khoản \"{user.Email}\" thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Khóa / Mở khóa nhanh
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(uint id)
        {
            var user = await _context.Taikhoans.FindAsync(id);
            if (user != null)
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (currentUserId != null && uint.TryParse(currentUserId, out uint myId) && myId == id)
                {
                    TempData["ErrorMessage"] = "Bạn không thể tự khóa tài khoản của chính mình!";
                    return RedirectToAction(nameof(Index));
                }

                user.ConHoatDong = !(user.ConHoatDong ?? true);
                user.NgayCapNhat = DateTime.Now;
                await _context.SaveChangesAsync();

                string trangThaiText = (user.ConHoatDong == true) ? "kích hoạt mở khóa" : "khóa";
                TempData["SuccessMessage"] = $"Đã {trangThaiText} tài khoản \"{user.Email}\"!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

