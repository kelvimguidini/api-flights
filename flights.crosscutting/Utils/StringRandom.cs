using System;
using System.Linq;

namespace flights.crosscutting.Utils
{
    public static class StringRandom
    {
        public static string GenerateStringRandom(int size)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, size)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());
            return result;
        }
    }
}
