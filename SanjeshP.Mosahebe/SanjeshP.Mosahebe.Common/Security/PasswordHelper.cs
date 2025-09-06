using System;
using System.Security.Cryptography;
using System.Text;

namespace SanjeshP.Mosahebe.Security
{
    public class PasswordHelper
    {
        public static string EncodePasswordMd5(string password)
        {
            Byte[] orginalBytes;
            Byte[] encodeBytes;
            MD5 md5;
            md5 = new MD5CryptoServiceProvider();
            orginalBytes = ASCIIEncoding.Default.GetBytes(password);
            encodeBytes = md5.ComputeHash(orginalBytes);
            return BitConverter.ToString(encodeBytes);
        }

        public static string GetSha256Hash(string input)
        {
            //using (var sha256 = new SHA256CryptoServiceProvider())
            using (var sha256 = SHA256.Create())
            {
                var byteValue = Encoding.UTF8.GetBytes(input);
                var byteHash = sha256.ComputeHash(byteValue);
                return Convert.ToBase64String(byteHash);
                //return BitConverter.ToString(byteHash).Replace("-", "").ToLower();
            }
        }

        public static string HashPasswordBCrypt(string password)
        {
            // تولید هش برای کلمه عبور
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPasswordBCrypt(string password, string hashedPassword)
        {
            // بررسی کلمه عبور
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
    }
}
