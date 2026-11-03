using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanPhanMem.Data;
using WebBanPhanMem.Models;

namespace WebBanPhanMem.Controllers
{
    [Authorize(Roles = "Quản trị")]
    public class DoiTacController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoiTacController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/DoiTac hoặc /DoiTac
        [Route("Admin/DoiTac")]
        [Route("DoiTac")]
        public async Task<IActionResult> Index(string? search, bool? conHoatDong)
        {
            var query = _context.Doitacs
                .Include(d => d.Sanphams)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(d => d.TenDoiTac.Contains(term) 
                                      || (d.Email != null && d.Email.Contains(term)) 
                                      || (d.NguoiLienHe != null && d.NguoiLienHe.Contains(term))
                                      || (d.SoDienThoai != null && d.SoDienThoai.Contains(term)));
            }

            if (conHoatDong.HasValue)
            {
                query = query.Where(d => d.ConHoatDong == conHoatDong);
            }

            var list = await query.OrderByDescending(d => d.NgayTao).ToListAsync();

            ViewBag.SearchTerm = search;
            ViewBag.SelectedStatus = conHoatDong;

            return View(list);
        }

        // POST: Tạo mới đối tác
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Doitac doitac)
        {
            if (string.IsNullOrWhiteSpace(doitac.TenDoiTac))
            {
                TempData["ErrorMessage"] = "Tên đối tác không được để trống!";
                return RedirectToAction(nameof(Index));
            }

            doitac.TenDoiTac = doitac.TenDoiTac.Trim();
            doitac.NguoiLienHe = doitac.NguoiLienHe?.Trim();
            doitac.Email = doitac.Email?.Trim();
            doitac.SoDienThoai = doitac.SoDienThoai?.Trim();
            doitac.GhiChu = doitac.GhiChu?.Trim();
            doitac.ConHoatDong = doitac.ConHoatDong ?? true;
            doitac.NgayTao = DateTime.Now;

            _context.Doitacs.Add(doitac);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Thêm đối tác \"{doitac.TenDoiTac}\" thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Cập nhật đối tác
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Doitac doitac)
        {
            var item = await _context.Doitacs.FindAsync(doitac.Id);
            if (item == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin đối tác!";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(doitac.TenDoiTac))
            {
                TempData["ErrorMessage"] = "Tên đối tác không được để trống!";
                return RedirectToAction(nameof(Index));
            }

            item.TenDoiTac = doitac.TenDoiTac.Trim();
            item.NguoiLienHe = doitac.NguoiLienHe?.Trim();
            item.Email = doitac.Email?.Trim();
            item.SoDienThoai = doitac.SoDienThoai?.Trim();
            item.GhiChu = doitac.GhiChu?.Trim();
            item.ConHoatDong = doitac.ConHoatDong ?? true;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Cập nhật đối tác \"{item.TenDoiTac}\" thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Bật / Tắt trạng thái hoạt động nhanh
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(uint id)
        {
            var item = await _context.Doitacs.FindAsync(id);
            if (item != null)
            {
                item.ConHoatDong = !(item.ConHoatDong ?? true);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã cập nhật trạng thái của \"{item.TenDoiTac}\"!";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Xóa đối tác
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(uint id)
        {
            var item = await _context.Doitacs
                .Include(d => d.Sanphams)
                .Include(d => d.Khokeys)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (item == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đối tác để xóa!";
                return RedirectToAction(nameof(Index));
            }

            if (item.Sanphams.Any() || item.Khokeys.Any())
            {
                TempData["ErrorMessage"] = $"Không thể xóa đối tác \"{item.TenDoiTac}\" vì đang có sản phẩm hoặc kho key liên kết!";
                return RedirectToAction(nameof(Index));
            }

            _context.Doitacs.Remove(item);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã xóa đối tác \"{item.TenDoiTac}\" khỏi hệ thống!";
            return RedirectToAction(nameof(Index));
        }
    }
}

