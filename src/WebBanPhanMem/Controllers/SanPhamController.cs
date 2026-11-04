using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanPhanMem.Data;
using WebBanPhanMem.Models;
using WebBanPhanMem.Models.ViewModels;

namespace WebBanPhanMem.Controllers
{
    [Authorize(Roles = "Quản trị")]
    public class SanPhamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SanPhamController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // GET: /Admin/SanPham
        [Route("Admin/SanPham")]
        [Route("SanPham")]
        public async Task<IActionResult> Index(string? search, uint? danhMucId, bool? conHoatDong)
        {
            var query = _context.Sanphams
                .Include(s => s.DanhMuc)
                .Include(s => s.DoiTac)
                .Include(s => s.Goibanquyens)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(s => s.TenSanPham.Contains(search) || s.DuongDan.Contains(search));

            if (danhMucId.HasValue)
                query = query.Where(s => s.DanhMucId == danhMucId);

            if (conHoatDong.HasValue)
                query = query.Where(s => s.ConHoatDong == conHoatDong);

            var list = await query.OrderByDescending(s => s.NgayTao).ToListAsync();

            ViewBag.DanhMucList = await _context.Danhmucs
                .Where(d => d.ConHoatDong == true)
                .OrderBy(d => d.ThuTu)
                .ToListAsync();

            ViewBag.DoiTacList = await _context.Doitacs
                .Where(d => d.ConHoatDong == true)
                .OrderBy(d => d.TenDoiTac)
                .ToListAsync();

            ViewBag.SearchTerm = search;
            ViewBag.SelectedDanhMuc = danhMucId;
            ViewBag.SelectedStatus = conHoatDong;

            return View(list);
        }

        // GET: /Admin/SanPham/Detail/5
        [HttpGet]
        [Route("Admin/SanPham/Detail/{id}")]
        public async Task<IActionResult> Detail(uint id)
        {
            var sp = await _context.Sanphams
                .Include(s => s.DanhMuc)
                .Include(s => s.DoiTac)
                .Include(s => s.Goibanquyens)
                .Include(s => s.Anhsanphams)
                .Include(s => s.Danhgia)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sp == null) return NotFound();
            return View(sp);
        }

        // ===================== HELPER: Lưu file ảnh =====================
        private async Task<string?> SaveUploadedImage(IFormFile? file, string? currentPath = null)
        {
            if (file == null || file.Length == 0) return currentPath;

            var allowedExt = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExt.Contains(ext)) return currentPath;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "products");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{ext}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/uploads/products/{fileName}";
        }

        // POST: Tạo mới sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] SanPhamCreateViewModel vm, IFormFile? anhDaiDienFile)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu không hợp lệ. Vui lòng kiểm tra lại!";
                return RedirectToAction(nameof(Index));
            }

            // Xử lý upload ảnh
            var anhPath = await SaveUploadedImage(anhDaiDienFile);

            var sp = new Sanpham
            {
                TenSanPham = vm.TenSanPham.Trim(),
                DanhMucId = vm.DanhMucId,
                DoiTacId = vm.DoiTacId == 0 ? null : vm.DoiTacId,
                DuongDan = string.IsNullOrWhiteSpace(vm.DuongDan)
                                ? GenerateSlug(vm.TenSanPham)
                                : vm.DuongDan.Trim(),
                MoTaNgan = vm.MoTaNgan,
                MoTaChiTiet = vm.MoTaChiTiet,
                AnhDaiDien = anhPath,
                LaNoiBat = vm.LaNoiBat,
                ConHoatDong = vm.ConHoatDong,
                LuotXem = 0,
                NgayTao = DateTime.Now,
                NgayCapNhat = DateTime.Now
            };

            _context.Sanphams.Add(sp);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã thêm sản phẩm \"{sp.TenSanPham}\" vào MySQL!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Cập nhật sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] SanPhamCreateViewModel vm, IFormFile? anhDaiDienFile)
        {
            var sp = await _context.Sanphams.FindAsync(vm.Id);
            if (sp == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm!";
                return RedirectToAction(nameof(Index));
            }

            // Xử lý upload ảnh — giữ ảnh cũ nếu không upload file mới
            var anhPath = await SaveUploadedImage(anhDaiDienFile, sp.AnhDaiDien);

            sp.TenSanPham = vm.TenSanPham.Trim();
            sp.DanhMucId = vm.DanhMucId;
            sp.DoiTacId = vm.DoiTacId == 0 ? null : vm.DoiTacId;
            sp.DuongDan = string.IsNullOrWhiteSpace(vm.DuongDan)
                                ? GenerateSlug(vm.TenSanPham)
                                : vm.DuongDan.Trim();
            sp.MoTaNgan = vm.MoTaNgan;
            sp.MoTaChiTiet = vm.MoTaChiTiet;
            sp.AnhDaiDien = anhPath;
            sp.LaNoiBat = vm.LaNoiBat;
            sp.ConHoatDong = vm.ConHoatDong;
            sp.NgayCapNhat = DateTime.Now;

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Cập nhật sản phẩm \"{sp.TenSanPham}\" thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Đổi trạng thái nhanh
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(uint id)
        {
            var sp = await _context.Sanphams.FindAsync(id);
            if (sp != null)
            {
                sp.ConHoatDong = !(sp.ConHoatDong ?? true);
                sp.NgayCapNhat = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Cập nhật trạng thái \"{sp.TenSanPham}\" thành công!";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Xóa sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(uint id)
        {
            var sp = await _context.Sanphams.FindAsync(id);
            if (sp != null)
            {
                _context.Sanphams.Remove(sp);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã xóa sản phẩm khỏi MySQL!";
            }
            return RedirectToAction(nameof(Index));
        }

        // ===================== GÓI BẢN QUYỀN (CRUD inline) =====================

        // POST: Thêm gói bản quyền vào sản phẩm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGoiBanQuyen([FromForm] Goibanquyen goi)
        {
            if (goi.SanPhamId == 0)
            {
                TempData["ErrorMessage"] = "Sản phẩm không hợp lệ!";
                return RedirectToAction(nameof(Index));
            }

            // Kiểm tra trùng Mã hàng (SKU) do bảng goibanquyen có Unique constraint uq_goi_ma_hang
            goi.MaHang = goi.MaHang?.Trim().ToUpperInvariant() ?? "";
            if (string.IsNullOrWhiteSpace(goi.MaHang))
            {
                TempData["ErrorMessage"] = "Mã hàng (SKU) không được để trống!";
                return RedirectToAction("Detail", new { id = goi.SanPhamId });
            }

            var isDuplicateSku = await _context.Goibanquyens.AnyAsync(g => g.MaHang == goi.MaHang);
            if (isDuplicateSku)
            {
                TempData["ErrorMessage"] = $"Mã hàng \"{goi.MaHang}\" đã tồn tại trong hệ thống! Vui lòng đặt mã hàng (SKU) khác (Ví dụ: {goi.MaHang}-01, WIN11-1Y-01...).";
                return RedirectToAction("Detail", new { id = goi.SanPhamId });
            }

            try
            {
                goi.ConHoatDong  = goi.ConHoatDong ?? true;
                goi.SoKeyConLai  = 0;
                _context.Goibanquyens.Add(goi);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Thêm gói \"{goi.TenGoiBanQuyen}\" (Mã: {goi.MaHang}) thành công!";
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = $"Lỗi khi lưu gói bản quyền: {ex.InnerException?.Message ?? ex.Message}";
            }

            return RedirectToAction("Detail", new { id = goi.SanPhamId });
        }

        // POST: Cập nhật gói bản quyền
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGoiBanQuyen([FromForm] Goibanquyen goi)
        {
            var item = await _context.Goibanquyens.FindAsync(goi.Id);
            if (item != null)
            {
                var newSku = goi.MaHang?.Trim().ToUpperInvariant() ?? "";
                if (string.IsNullOrWhiteSpace(newSku))
                {
                    TempData["ErrorMessage"] = "Mã hàng (SKU) không được để trống!";
                    return RedirectToAction("Detail", new { id = item.SanPhamId });
                }

                // Kiểm tra trùng SKU với các gói khác
                var isDuplicateSku = await _context.Goibanquyens.AnyAsync(g => g.MaHang == newSku && g.Id != item.Id);
                if (isDuplicateSku)
                {
                    TempData["ErrorMessage"] = $"Mã hàng \"{newSku}\" đã được sử dụng ở gói khác! Vui lòng chọn mã khác.";
                    return RedirectToAction("Detail", new { id = item.SanPhamId });
                }

                try
                {
                    item.TenGoiBanQuyen = goi.TenGoiBanQuyen;
                    item.MaHang         = newSku;
                    item.Gia            = goi.Gia;
                    item.GiaKhuyenMai   = goi.GiaKhuyenMai == 0 ? null : goi.GiaKhuyenMai;
                    item.ThoiHanNgay    = goi.ThoiHanNgay;
                    item.SoMayToiDa     = goi.SoMayToiDa;
                    item.BaoHanhNgay    = goi.BaoHanhNgay;
                    item.ConHoatDong    = goi.ConHoatDong;
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Cập nhật gói \"{item.TenGoiBanQuyen}\" thành công!";
                }
                catch (DbUpdateException ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi khi cập nhật gói bản quyền: {ex.InnerException?.Message ?? ex.Message}";
                }

                return RedirectToAction("Detail", new { id = item.SanPhamId });
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Xóa gói bản quyền
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGoiBanQuyen(uint id, uint sanPhamId)
        {
            var item = await _context.Goibanquyens.FindAsync(id);
            if (item != null)
            {
                _context.Goibanquyens.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa gói bản quyền!";
            }
            return RedirectToAction("Detail", new { id = sanPhamId });
        }

        // ===================== HELPER =====================
        private static string GenerateSlug(string text)
        {
            var slug = text.ToLower().Trim();
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[àáâãăạảấầẩẫậắằẳẵặ]", "a");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[èéêẹẻẽếềểễệ]", "e");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[ìíịỉĩ]", "i");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[òóôõơọỏốồổỗộớờởỡợ]", "o");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[ùúũưụủứừửữự]", "u");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[ỳýỵỷỹ]", "y");
            slug = slug.Replace("đ", "d");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
            return slug;
        }
    }
}

