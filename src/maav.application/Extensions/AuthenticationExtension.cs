
using System;
using System.Security.Cryptography;
using System.Text;

namespace MAAV.Application.Extensions
{
    static public class AuthenticationExtension
    {
        public static (byte[] PasswordHash, byte[] PasswordSalt) Encrypt(this string password)
        {
            using (var hmac = new HMACSHA512())
            {
                return (hmac.ComputeHash(Encoding.UTF8.GetBytes(password)), hmac.Key);
            }
        }

        public static bool IsValid(this string password, string hashBase64, string saltBase64)
        {
            byte[] hashArray = Convert.FromBase64String(hashBase64);
            byte[] saltArray = Convert.FromBase64String(saltBase64);

            using (var hmac = new HMACSHA512(saltArray))
            {
                byte[] computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != hashArray[i])
                    {
                        return false;
                    }
                }
                return true;
            }
        }
    }
}