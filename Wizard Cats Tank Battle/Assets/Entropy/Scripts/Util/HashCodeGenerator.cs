namespace Vashta.Entropy.Util
{
    using System;
    using System.Linq;
    
    public class HashCodeGenerator
    {
        private static readonly Random random = new Random();
        // private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"; // full
        private const string chars = "ABCDEF0123456789"; // hex

        public static string GenerateRandomString(int length = 6)
        {
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}