using System.Security.Cryptography;
using System.Text;

namespace MAAV.DataContracts.Extensions
{
    static public class ByteExtension
    {
        static public string ToHexString(this byte[] array)
        {
            StringBuilder hex = new StringBuilder(array.Length * 2);
            foreach (byte b in array)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        static public string ToHexString2(this byte[] bytes)
        {
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }

            return builder.ToString();
        }

        static public string SHA1HashStringForUTF8String(this string s)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            using (var sha1 = SHA1.Create())
            {
                byte[] hashBytes = sha1.ComputeHash(bytes);

                return ToHexString(hashBytes);
            }
        }
    }
}
