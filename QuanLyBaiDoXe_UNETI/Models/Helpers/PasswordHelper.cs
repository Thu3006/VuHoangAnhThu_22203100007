using System.Security.Cryptography;
using System.Text;

namespace QuanLyBaiDoXe_UNETI.Models
{
    // Họ và tên: Vũ Hoàng Anh Thư
    // Nội dung: Lớp tiện ích mã hóa mật khẩu một chiều sử dụng thuật toán SHA-256
    public static class PasswordHelper
    {
        // Chuỗi Salt bảo mật cố định của hệ thống
        private const string Salt = "SmartParking_UNETI_SecuritySalt_2026";

        /// <summary>
        /// Băm mật khẩu dạng rõ sang chuỗi Hex SHA-256 kèm Salt
        /// </summary>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) return string.Empty;

            using (var sha256 = SHA256.Create())
            {
                var combinedPassword = password + Salt;
                var bytes = Encoding.UTF8.GetBytes(combinedPassword);
                var hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToHexString(hashBytes); // Trả về chuỗi Hex 64 ký tự
            }
        }

        /// <summary>
        /// Kiểm tra mật khẩu nhập vào có khớp với mã băm trong DB hay không
        /// Hỗ trợ cả trường hợp dữ liệu cũ dạng chữ rõ
        /// </summary>
        public static bool VerifyPassword(string inputPassword, string storedHash)
        {
            if (string.IsNullOrEmpty(inputPassword) || string.IsNullOrEmpty(storedHash))
                return false;

            // Nếu mật khẩu trong DB chính là mã băm SHA-256
            var inputHash = HashPassword(inputPassword);
            if (string.Equals(inputHash, storedHash, StringComparison.OrdinalIgnoreCase))
                return true;

            // Kiểm tra tương thích nếu DB còn lưu dạng plain-text cũ
            return inputPassword == storedHash;
        }
    }
}