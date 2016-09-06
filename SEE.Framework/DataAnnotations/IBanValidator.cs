using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace System.ComponentModel.DataAnnotations
{
    public class IBanValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (string.IsNullOrWhiteSpace(value.ToString())) return true;

            string iban = value.ToString().Replace(" ", null);

            if (!Regex.IsMatch(iban, @"^\d{26}$")) return false;

            int checksum = 0;
            if (Int32.TryParse(iban.Substring(0, 2), out checksum))
            {
                iban = iban.Substring(2, iban.Length - 2) + "252100";
                int modulo = 0;
                foreach (var c in iban)
                {
                    int digit = 0;
                    if (Int32.TryParse(c.ToString(), out digit))
                    {
                        modulo = (10 * modulo + digit) % 97;
                    }
                }
                modulo = 98 - modulo;
                if (modulo == checksum)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
