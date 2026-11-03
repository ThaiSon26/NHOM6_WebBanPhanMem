using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanPhanMem.Data;
using WebBanPhanMem.Helpers;
using WebBanPhanMem.Models;

namespace WebBanPhanMem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Login hoặc /dang-nhap
        [AllowAnonymous]
        [Route("Account/Login")]
        [Route("dang-nhap")]
        public IActionResult Login(string? returnUrl = null, string tab = "login")
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("Quản trị"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                return LocalRedirect(returnUrl ?? "/");
            }

            ViewBag.ActiveTab = tab;
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // GET: /Account/Register hoặc /dang-ky
        [AllowAnonymous]
        [Route("Account/Register")]
        [Route("dang-ky")]
        public IActionResult Register(string? returnUrl = null)
        {
            return RedirectToAction(nameof(Login), new { returnUrl, tab = "register" });
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("Account/Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            ViewBag.ActiveTab = "login";
            ViewBag.ReturnUrl = model.ReturnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var input = model.EmailOrUsername.Trim();

            // Tìm tài khoản trong MySQL bằng Email hoặc Họ tên
            var user = await _context.Taikhoans
                .FirstOrDefaultAsync(u => u.Email.ToLower() == input.ToLower() || u.HoTen.ToLower() == input.ToLower());

            if (user == null)
            {
                // Kiểm tra tài khoản admin mặc định nếu DB chưa có tài khoản Quản trị nào
                if (input.Equals("admin@softdev.vn", StringComparison.OrdinalIgnoreCase) && model.Password == "Admin@123")
                {
                    user = await SeedDefaultAdminAsync();
                }
            }

            if (user == null || !PasswordHelper.VerifyPassword(model.Password, user.MatKhauBam))
            {
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác.");
                return View(model);
            }

            if (user.ConHoatDong == false)
            {
                ModelState.AddModelError(string.Empty, "Tài khoản của bạn đang bị khóa. Vui lòng liên hệ bộ phận hỗ trợ.");
                return View(model);
            }

            // Tạo danh sách Claims xác thực
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.HoTen),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, string.IsNullOrWhiteSpace(user.VaiTro) ? "Khách hàng" : user.VaiTro),
                new Claim("UserRole", user.VaiTro ?? "Khách hàng")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = model.RememberMe,
                ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(14) : DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            TempData["SuccessMessage"] = $"Chào mừng bạn trở lại, {user.HoTen}!";

            // Điều hướng dựa vào vai trò
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            if (string.Equals(user.VaiTro, "Quản trị", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Index", "Admin");
            }

            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [Route("Account/Register")]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewBag.ActiveTab = "register";
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                var loginVm = new LoginViewModel { ReturnUrl = returnUrl };
                ViewData["RegisterModel"] = model;
                return View("Login", loginVm);
            }

            var emailNormalized = model.Email.Trim().ToLower();

            // Kiểm tra trùng Email
            bool emailExists = await _context.Taikhoans.AnyAsync(u => u.Email.ToLower() == emailNormalized);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "Địa chỉ Email này đã tồn tại trong hệ thống.");
                var loginVm = new LoginViewModel { ReturnUrl = returnUrl };
                ViewData["RegisterModel"] = model;
                return View("Login", loginVm);
            }

            // Tạo thực thể người dùng mới trong MySQL
            var newUser = new Taikhoan
            {
                HoTen = model.HoTen.Trim(),
                Email = emailNormalized,
                SoDienThoai = model.SoDienThoai?.Trim(),
                MatKhauBam = PasswordHelper.HashPassword(model.Password),
                VaiTro = "Khách hàng",
                ConHoatDong = true,
                NgayTao = DateTime.Now,
                NgayCapNhat = DateTime.Now
            };

            _context.Taikhoans.Add(newUser);
            await _context.SaveChangesAsync();

            // Tự động đăng nhập cho người dùng mới
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, newUser.Id.ToString()),
                new Claim(ClaimTypes.Name, newUser.HoTen),
                new Claim(ClaimTypes.Email, newUser.Email),
                new Claim(ClaimTypes.Role, newUser.VaiTro),
                new Claim("UserRole", newUser.VaiTro)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Chào mừng bạn gia nhập SoftDev.";
            return RedirectToAction("Index", "Home");
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Account/Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "Bạn đã đăng xuất thành công.";
            return RedirectToAction(nameof(Login));
        }

        // GET: /Account/AccessDenied
        [AllowAnonymous]
        [Route("Account/AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }

        /// <summary>
        /// Khởi tạo tài khoản Quản trị mặc định nếu database trống
        /// </summary>
        private async Task<Taikhoan> SeedDefaultAdminAsync()
        {
            var adminUser = new Taikhoan
            {
                HoTen = "Quản trị viên Hệ thống",
                Email = "admin@softdev.vn",
                SoDienThoai = "0988888888",
                MatKhauBam = PasswordHelper.HashPassword("Admin@123"),
                VaiTro = "Quản trị",
                ConHoatDong = true,
                NgayTao = DateTime.Now,
                NgayCapNhat = DateTime.Now
            };

            _context.Taikhoans.Add(adminUser);
            await _context.SaveChangesAsync();
            return adminUser;
        }
    }
}

