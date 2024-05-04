using System.Security.Cryptography;
using System.Text;

namespace RenderStar.Util
{
    public static class Hash
    {
        public static string Generate(string input)
        {
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hashBytes = MD5.HashData(inputBytes);

            StringBuilder buffer = new();

            for (int b = 0; b < hashBytes.Length; b++)
                buffer.Append(hashBytes[b].ToString("X2"));
            
            return buffer.ToString();
        }
    }
}
