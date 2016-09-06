using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEE.Framework
{
    public static class DecimalExtensions
    {
        public static string ToStringify(this decimal value)
        {
            int i = (int)value;
            int f = (int)(value * 100) % 100;
            return $"{i.ToStringify(CurrencyType.Zloty)} {f.ToStringify(CurrencyType.Grosz)}";
        }


    }
}
