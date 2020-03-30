using System;
using System.Text;

namespace Sining.Tools
{
    public class GenerateNumber
    {
        private static readonly char[] Numbers =
        {
            '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
            'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
        };

        private const int Length = 35;
        private static readonly Random Random = new Random();

        public static string Create(int length = 10)
        {
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < length; i++)
            {
                stringBuilder.Append(Numbers[Random.Next(Length)]);
            }

            return stringBuilder.ToString();
        }
    }
}