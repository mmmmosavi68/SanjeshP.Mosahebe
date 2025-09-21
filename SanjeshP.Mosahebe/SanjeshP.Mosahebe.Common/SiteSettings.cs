
namespace SanjeshP.Mosahebe.Common
{
    public class SiteSettings
    {
        public string ElmahPath { get; set; }
        public JwtSettings JwtSettings { get; set; }
        public IdentitySettings IdentitySettings { get; set; }
        public AuthenticationSettings authenticationSettings { get; set; }
        public AuthenticationStudentSettings authenticationStudentSettings { get; set; }
        public PayamSMS payamSMS { get; set; }

    }

    public class IdentitySettings
    {
        public bool PasswordRequireDigit { get; set; }
        public int PasswordRequiredLength { get; set; }
        public bool PasswordRequireNonAlphanumeric { get; set; }
        public bool PasswordRequireUppercase { get; set; }
        public bool PasswordRequireLowercase { get; set; }
        public bool RequireUniqueEmail { get; set; }
    }
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string EncryptKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int NotBeforeMinutes { get; set; }
        public int ExpirationMinutes { get; set; }

    }

    public class AuthenticationSettings
    {
        public string LoginPath { get; set; }
        public string LogoutPath { get; set; }
        public int ExpireTimeSpanInMinutes { get; set; }
    }
    public class AuthenticationStudentSettings
    {
        public string StudentLoginPath { get; set; }
        public string StudentLogoutPath { get; set; }
        public int StudentExpireTimeSpanInMinutes { get; set; }
    }

    public class PayamSMS
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class ClientConfig
    {
        public string BaseUrl { get; set; }
        public string ApiPrefix { get; set; }
        public string Environment { get; set; }
    }
}