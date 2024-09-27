using Org.BouncyCastle.Crypto.Generators;

namespace LibraryManagement.Helpers
{
    public static class PasswordHelper
    {
        public static string HashPassword(string password)
        {
            try
            {
                return BCrypt.Net.BCrypt.HashPassword(password);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while hashing the password.", ex);
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while verifying the password.", ex);
            }
        }
    }
}
