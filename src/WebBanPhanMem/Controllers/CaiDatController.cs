using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanPhanMem.Data;
using WebBanPhanMem.Models;

namespace WebBanPhanMem.Controllers
{
    [Authorize(Roles = "Quản trị")]
    public class CaiDatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CaiDatController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Admin/CaiDat hoặc /CaiDat
        [Route("Admin/CaiDat")]
        [Route("CaiDat")]
        public async Task<IActionResult> Index()
        {
            var list = await _context.Cauhinhs.ToListAsync();
            var configDict = list.ToDictionary(c => c.Khoa, c => c.GiaTri ?? "");

            // Giá trị mặc định nếu CSDL chưa có
            string GetVal(string key, string defaultVal) => configDict.ContainsKey(key) ? configDict[key] : defaultVal;

            ViewBag.SiteName = GetVal("SiteName", "SoftDev Store - Bán Bản Quyền Phần Mềm");
            ViewBag.Hotline = GetVal("Hotline", "1900 6868");
            ViewBag.ContactEmail = GetVal("ContactEmail", "hotro@softdev.vn");
            ViewBag.Address = GetVal("Address", "Tầng 5, Tòa nhà Công Nghệ, TP. Hà Nội");
            ViewBag.DefaultVatRate = GetVal("DefaultVatRate", "10");
            ViewBag.MaintenanceMode = GetVal("MaintenanceMode", "false");
            ViewBag.SeoTitle = GetVal("SeoTitle", "Mua bản quyền phần mềm chính hãng giá rẻ");
            ViewBag.SeoDescription = GetVal("SeoDescription", "Chuyên cung cấp key Windows, Office, diệt virus và phần mềm đồ họa bản quyền chính hãng 100%");
            ViewBag.FacebookUrl = GetVal("FacebookUrl", "https://facebook.com/softdev");

            return View();
        }

        // POST: Lưu toàn bộ cấu hình
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveSettings(IFormCollection form)
        {
            var keys = new[]
            {
                "SiteName", "Hotline", "ContactEmail", "Address",
                "DefaultVatRate", "MaintenanceMode", "SeoTitle",
                "SeoDescription", "FacebookUrl"
            };

            foreach (var key in keys)
            {
                string value = form[key].ToString()?.Trim() ?? "";
                if (key == "MaintenanceMode")
                {
                    value = form.ContainsKey("MaintenanceMode") ? "true" : "false";
                }

                var item = await _context.Cauhinhs.FirstOrDefaultAsync(c => c.Khoa == key);
                if (item != null)
                {
                    item.GiaTri = value;
                }
                else
                {
                    _context.Cauhinhs.Add(new Cauhinh { Khoa = key, GiaTri = value });
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã lưu cài đặt hệ thống thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}

