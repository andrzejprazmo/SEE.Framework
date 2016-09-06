using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEE.Framework
{
    public static class StringExtensions
    {
        /// <summary>
        /// Parse date or datetime string and returns <see cref="DateTime"/> or null if parsing failed.
        /// </summary>
        /// <param name="value">String data to parse.</param>
        /// <returns><see cref="DateTime"/> or null if parsing failed</returns>
        public static DateTime? ToDateOrNull(this string value)
        {
            DateTime date = new DateTime();
            if (!string.IsNullOrWhiteSpace(value) && DateTime.TryParse(value, out date))
            {
                return date;
            }
            return null;

        }
    }
}
