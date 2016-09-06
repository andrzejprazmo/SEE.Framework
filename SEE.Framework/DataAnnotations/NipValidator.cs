using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace System.ComponentModel.DataAnnotations
{
    public class NipValidationAttribute : ValidationAttribute
    {
        private readonly int[] multipliers = { 6, 5, 7, 2, 3, 4, 5, 6, 7 };

        public override bool IsValid(object value)
        {
            if (value == null)
                return true;

            string nip = value as string;
            nip = nip.Replace("-", "");
            if (nip == null || !Regex.IsMatch(nip, @"^[\d]{10}$"))
                return false;

            int sum = 0;
            for (int i = 8; i >= 0; i--)
                sum += multipliers[i] * int.Parse(nip.Substring(i, 1));

            return ((sum % 11) == 10 ? false : ((sum % 11) == int.Parse(nip.Substring(9, 1))));
        }
    }
}