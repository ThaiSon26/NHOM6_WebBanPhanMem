using System.Security.Cryptography;
using System.Text;

namespace WebBanPhanMem.Helpers
{
    public static class PasswordHelper
    {
        private const string Salt = "WebBanPhanMem_SoftDev_SaltKey_2026";

        /// <summary>
        /// Băm mật khẩu bằng thuật toán SHA-256 kèm salt bảo mật
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;

            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + Salt));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// Xác thực mật khẩu nhập vào với mật khẩu đã lưu trong database.
        /// Hỗ trợ cả mật khẩu băm SHA-256 và mật khẩu thuần (plaintext fallback) từ seed dữ liệu ban đầu.
        /// </summary>
        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            if (string.IsNullOrEmpty(inputPassword) || string.IsNullOrEmpty(storedHash))
                return false;

            // 1. Kiểm tra khớp theo mã băm SHA-256 (có salt)
            string hashedInput = HashPassword(inputPassword);
            if (string.Equals(hashedInput, storedHash, StringComparison.OrdinalIgnoreCase))
                return true;

            // 2. Kiểm tra khớp theo SHA-256 thuần (không salt)
            using (var sha256 = SHA256.Create())
            {
                byte[] rawHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(inputPassword));
                string hex = BitConverter.ToString(rawHash).Replace("-", "").ToLowerInvariant();
                if (string.Equals(hex, storedHash, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            // 3. Fallback cho mật khẩu thuần (nếu dữ liệu mẫu trong DB chưa kịp mã hóa)
            if (string.Equals(inputPassword, storedHash, StringComparison.Ordinal))
                return true;

            return false;
        }
    }
}

