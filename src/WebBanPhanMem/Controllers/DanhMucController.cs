using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanPhanMem.Data;
using WebBanPhanMem.Models;

namespace WebBanPhanMem.Controllers
{
    public class DanhMucController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DanhMucController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /DanhMuc/ hoặc /Admin/DanhMuc
        [Route("Admin/DanhMuc")]
        [Route("DanhMuc")]
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách danh mục từ MySQL kèm thông tin danh mục cha
            var list = await _context.Danhmucs
                .Include(d => d.DanhMucCha)
                .OrderBy(d => d.ThuTu)
                .ToListAsync();

            ViewBag.ParentCategories = list.Where(d => d.DanhMucChaId == null).ToList();

            return View(list);
        }

        // POST: Tạo mới danh mục
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Danhmuc danhmuc)
        {
            if (ModelState.IsValid)
            {
                // Tự động sinh đường dẫn (slug) nếu để trống
                if (string.IsNullOrWhiteSpace(danhmuc.DuongDan))
                {
                    danhmuc.DuongDan = danhmuc.TenDanhMuc.ToLower()
                        .Trim()
                        .Replace(" ", "-")
                        .Replace("đ", "d");
                }

                danhmuc.ConHoatDong = danhmuc.ConHoatDong ?? true;
                _context.Danhmucs.Add(danhmuc);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm danh mục mới vào MySQL thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Cập nhật danh mục
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Danhmuc danhmuc)
        {
            var item = await _context.Danhmucs.FindAsync(danhmuc.Id);
            if (item != null)
            {
                item.TenDanhMuc = danhmuc.TenDanhMuc;
                item.DuongDan = string.IsNullOrWhiteSpace(danhmuc.DuongDan)
                    ? danhmuc.TenDanhMuc.ToLower().Trim().Replace(" ", "-").Replace("đ", "d")
                    : danhmuc.DuongDan.Trim();
                item.DanhMucChaId = danhmuc.DanhMucChaId == 0 ? null : danhmuc.DanhMucChaId;
                item.BieuTuong = danhmuc.BieuTuong;
                item.ThuTu = danhmuc.ThuTu;
                item.ConHoatDong = danhmuc.ConHoatDong ?? true;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Đổi trạng thái hoạt động nhanh
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(uint id)
        {
            var item = await _context.Danhmucs.FindAsync(id);
            if (item != null)
            {
                item.ConHoatDong = !(item.ConHoatDong ?? true);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã cập nhật trạng thái danh mục '{item.TenDanhMuc}'!";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Xóa danh mục
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(uint id)
        {
            var item = await _context.Danhmucs.FindAsync(id);
            if (item != null)
            {
                _context.Danhmucs.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa danh mục thành công khỏi MySQL!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
