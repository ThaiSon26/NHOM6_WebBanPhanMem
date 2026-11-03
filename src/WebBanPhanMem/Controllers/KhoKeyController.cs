using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanPhanMem.Data;
using WebBanPhanMem.Models;

namespace WebBanPhanMem.Controllers
{
    [Authorize(Roles = "Quản trị")]
    public class KhoKeyController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KhoKeyController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/KhoKey hoặc /KhoKey
        [Route("Admin/KhoKey")]
        [Route("KhoKey")]
        public async Task<IActionResult> Index(string? search, uint? goiId, uint? doiTacId, string? trangThai)
        {
            var query = _context.Khokeys
                .Include(k => k.GoiBanQuyen)
                    .ThenInclude(g => g.SanPham)
                .Include(k => k.DoiTac)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(k => k.MaKey.Contains(term) 
                                      || (k.GhiChu != null && k.GhiChu.Contains(term))
                                      || k.GoiBanQuyen.TenGoiBanQuyen.Contains(term)
                                      || k.GoiBanQuyen.SanPham.TenSanPham.Contains(term));
            }

            if (goiId.HasValue && goiId > 0)
            {
                query = query.Where(k => k.GoiBanQuyenId == goiId);
            }

            if (doiTacId.HasValue && doiTacId > 0)
            {
                query = query.Where(k => k.DoiTacId == doiTacId);
            }

            if (!string.IsNullOrWhiteSpace(trangThai))
            {
                query = query.Where(k => k.TrangThai == trangThai);
            }

            var list = await query.OrderByDescending(k => k.NgayTao).ToListAsync();

            ViewBag.GoiBanQuyenList = await _context.Goibanquyens
                .Include(g => g.SanPham)
                .Where(g => g.ConHoatDong == null || g.ConHoatDong == true)
                .OrderBy(g => g.SanPham.TenSanPham)
                .ThenBy(g => g.TenGoiBanQuyen)
                .ToListAsync();

            ViewBag.SanPhamList = await _context.Sanphams
                .Where(s => s.ConHoatDong == null || s.ConHoatDong == true)
                .OrderBy(s => s.TenSanPham)
                .ToListAsync();

            ViewBag.DoiTacList = await _context.Doitacs
                .Where(d => d.ConHoatDong == true)
                .OrderBy(d => d.TenDoiTac)
                .ToListAsync();

            ViewBag.SearchTerm = search;
            ViewBag.SelectedGoiId = goiId;
            ViewBag.SelectedDoiTacId = doiTacId;
            ViewBag.SelectedTrangThai = trangThai;

            return View(list);
        }

        // POST: Tạo nhanh Gói bản quyền ngay tại trang Kho Key
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuickGoi(uint sanPhamId, string tenGoi, string maHang, decimal gia, int soMay, int baoHanh)
        {
            if (sanPhamId == 0 || string.IsNullOrWhiteSpace(tenGoi))
            {
                TempData["ErrorMessage"] = "Vui lòng chọn sản phẩm và nhập tên gói bản quyền!";
                return RedirectToAction(nameof(Index));
            }

            var goi = new Goibanquyen
            {
                SanPhamId = sanPhamId,
                TenGoiBanQuyen = tenGoi.Trim(),
                MaHang = string.IsNullOrWhiteSpace(maHang) ? ("SKU-" + DateTime.Now.Ticks.ToString().Substring(12)) : maHang.Trim(),
                Gia = gia,
                SoMayToiDa = soMay > 0 ? soMay : 1,
                BaoHanhNgay = baoHanh > 0 ? baoHanh : 30,
                SoKeyConLai = 0,
                ConHoatDong = true
            };

            _context.Goibanquyens.Add(goi);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã tạo gói \"{goi.TenGoiBanQuyen}\" thành công! Bạn có thể nhập Key cho gói này ngay bây giờ.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Thêm 1 key lẻ
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Khokey key)
        {
            if (string.IsNullOrWhiteSpace(key.MaKey))
            {
                TempData["ErrorMessage"] = "Mã License Key không được để trống!";
                return RedirectToAction(nameof(Index));
            }

            key.MaKey = key.MaKey.Trim();
            key.LoaiKey = string.IsNullOrWhiteSpace(key.LoaiKey) ? "Retail" : key.LoaiKey.Trim();
            key.TrangThai = string.IsNullOrWhiteSpace(key.TrangThai) ? "Chưa bán" : key.TrangThai.Trim();
            key.DoiTacId = key.DoiTacId == 0 ? null : key.DoiTacId;
            key.GhiChu = key.GhiChu?.Trim();
            key.NgayTao = DateTime.Now;

            _context.Khokeys.Add(key);
            
            // Cập nhật số lượng key còn lại trong gói bản quyền nếu là Chưa bán
            if (key.TrangThai == "Chưa bán")
            {
                var goi = await _context.Goibanquyens.FindAsync(key.GoiBanQuyenId);
                if (goi != null)
                {
                    goi.SoKeyConLai += 1;
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã thêm License Key vào kho thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Nhập key hàng loạt (Batch Import)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BatchImport(uint goiBanQuyenId, uint? doiTacId, string loaiKey, string rawKeys, string? ghiChu)
        {
            if (string.IsNullOrWhiteSpace(rawKeys))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập danh sách mã key!";
                return RedirectToAction(nameof(Index));
            }

            var lines = rawKeys.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
                               .Select(k => k.Trim())
                               .Where(k => !string.IsNullOrWhiteSpace(k))
                               .Distinct()
                               .ToList();

            if (!lines.Any())
            {
                TempData["ErrorMessage"] = "Không tìm thấy mã key hợp lệ nào!";
                return RedirectToAction(nameof(Index));
            }

            int countAdded = 0;
            foreach (var keyStr in lines)
            {
                var keyItem = new Khokey
                {
                    GoiBanQuyenId = goiBanQuyenId,
                    DoiTacId = doiTacId == 0 ? null : doiTacId,
                    MaKey = keyStr,
                    LoaiKey = string.IsNullOrWhiteSpace(loaiKey) ? "Retail" : loaiKey,
                    TrangThai = "Chưa bán",
                    GhiChu = ghiChu?.Trim(),
                    NgayTao = DateTime.Now
                };
                _context.Khokeys.Add(keyItem);
                countAdded++;
            }

            var goi = await _context.Goibanquyens.FindAsync(goiBanQuyenId);
            if (goi != null)
            {
                goi.SoKeyConLai += countAdded;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã nhập hàng loạt thành công {countAdded} License Key vào kho!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Cập nhật thông tin key
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Khokey key)
        {
            var item = await _context.Khokeys.FindAsync(key.Id);
            if (item == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy License Key!";
                return RedirectToAction(nameof(Index));
            }

            string oldStatus = item.TrangThai;

            item.GoiBanQuyenId = key.GoiBanQuyenId;
            item.DoiTacId = key.DoiTacId == 0 ? null : key.DoiTacId;
            item.MaKey = key.MaKey.Trim();
            item.LoaiKey = key.LoaiKey;
            item.TrangThai = key.TrangThai;
            item.GhiChu = key.GhiChu?.Trim();

            // Cập nhật số key khả dụng của gói nếu thay đổi trạng thái
            if (oldStatus != item.TrangThai)
            {
                var goi = await _context.Goibanquyens.FindAsync(item.GoiBanQuyenId);
                if (goi != null)
                {
                    if (oldStatus == "Chưa bán" && item.TrangThai != "Chưa bán")
                    {
                        goi.SoKeyConLai = Math.Max(0, goi.SoKeyConLai - 1);
                    }
                    else if (oldStatus != "Chưa bán" && item.TrangThai == "Chưa bán")
                    {
                        goi.SoKeyConLai += 1;
                    }
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Cập nhật License Key thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Đổi trạng thái nhanh
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(uint id, string status)
        {
            var item = await _context.Khokeys.FindAsync(id);
            if (item != null)
            {
                string oldStatus = item.TrangThai;
                item.TrangThai = status;

                var goi = await _context.Goibanquyens.FindAsync(item.GoiBanQuyenId);
                if (goi != null)
                {
                    if (oldStatus == "Chưa bán" && status != "Chưa bán")
                    {
                        goi.SoKeyConLai = Math.Max(0, goi.SoKeyConLai - 1);
                    }
                    else if (oldStatus != "Chưa bán" && status == "Chưa bán")
                    {
                        goi.SoKeyConLai += 1;
                    }
                }

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã cập nhật trạng thái Key thành: {status}!";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Xóa key
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(uint id)
        {
            var item = await _context.Khokeys.FindAsync(id);
            if (item == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy key để xóa!";
                return RedirectToAction(nameof(Index));
            }

            if (item.TrangThai == "Đã bán")
            {
                TempData["ErrorMessage"] = "Không thể xóa Key đã được bán cho khách hàng!";
                return RedirectToAction(nameof(Index));
            }

            if (item.TrangThai == "Chưa bán")
            {
                var goi = await _context.Goibanquyens.FindAsync(item.GoiBanQuyenId);
                if (goi != null)
                {
                    goi.SoKeyConLai = Math.Max(0, goi.SoKeyConLai - 1);
                }
            }

            _context.Khokeys.Remove(item);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã xóa License Key khỏi kho!";
            return RedirectToAction(nameof(Index));
        }
    }
}

