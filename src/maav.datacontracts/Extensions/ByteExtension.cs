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
    }
}
