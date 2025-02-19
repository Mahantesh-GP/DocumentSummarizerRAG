using System;
using System.Text;

namespace DocumentSummarizer.API.Utility
{

    public class UrlSafeBase64
    {
        public static string Encode(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes)
                          .Replace("+", "-") // ✅ Replace `+` with `-`
                          .Replace("/", "_") // ✅ Replace `/` with `_`
                          .TrimEnd('='); // ✅ Remove padding `=`
        }

        public static string Decode(string encodedInput)
        {
            string base64 = encodedInput.Replace("-", "+").Replace("_", "/");

            // ✅ Add back padding if needed
            while (base64.Length % 4 != 0)
            {
                base64 += "=";
            }

            byte[] bytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bytes);
        }
    }

}
