using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEE.Framework
{

    public static class IntegerExtensions
    {
        private static string zero = "zero";
        private static string[] digits = { "", "jeden", "dwa", "trzy", "cztery", "pięć", "sześć", "siedem", "osiem", "dziewięć" };
        private static string[] dozens = { "", "dziesięć", "dwadzieścia", "trzydzieści", "czterdzieści", "pięćdziesiąt", "sześćdziesiąt", "siedemdziesiąt", "osiemdziesiąt", "dziewięćdziesiąt" };
        private static string[] teens = { "dziesięć", "jedenaście", "dwanaście", "trzynaście", "czternaście", "piętnaście", "szesnaście", "siedemnaście", "osiemnaście", "dziewiętnaście" };
        private static string[] hundreds = { "", "sto", "dwieście", "trzysta", "czterysta", "pięćset", "sześćset", "siedemset", "osiemset", "dziewięćset" };
        private static string[,] intervals =
        {
            {"miliard","miliardy","miliardów"},
            {"milion","miliony","milionów"},
            {"tysiąc","tysiące","tysięcy"},
        };
        private static Dictionary<CurrencyType, string[]> currencies = new Dictionary<CurrencyType, string[]>
        {
            { CurrencyType.Zloty, new string[] { "złoty", "złote", "złotych" } },
            { CurrencyType.Grosz, new string[] { "grosz", "grosze", "groszy" } }
        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string ToStringify(this int value, CurrencyType type)
        {
            string result = value.ToStringify();
            int index = 2;
            if (value == 1)
            {
                index = 0;
            }
            value = value % 100;
            if (value < 10 || value > 20)
            {
                value = value % 10;
                if (value >= 2 && value <= 4)
                {
                    index = 1;
                }
            }
            result = $"{result} {currencies[type][index]}";
            return result;
        }

        public static string ToStringify(this int value)
        {
            if (value == 0) return zero;

            List<string> parts = new List<string>();
            if (value < 10)
            {
                return digits[value].Trim();
            }
            int[] modulo = new int[] { 1000000000, 1000000, 1000, 0 };
            for (int i = 0; i < modulo.Length; i++)
            {
                if (value >= modulo[i])
                {
                    int part = value;
                    if (modulo[i] > 0)
                    {
                        part = (value - value % modulo[i]) / modulo[i];
                        value = value - part * modulo[i];
                    }
                    parts.AddRange(Parts(part));
                    parts.Add(GetInterval(part, i));
                }
            }
            if (parts.Count == 1)
            {
                return parts[0];
            }
            return parts.Aggregate((a, b) => a + " " + b).Trim();
        }

        public static int[] ToDigits(this int value)
        {
            string stringValue = value.ToString();
            int[] result = new int[stringValue.Length];
            for (int i = 0; i < stringValue.Length; i++)
            {
                int cipher = 0;
                if (Int32.TryParse(stringValue[i].ToString(), out cipher))
                {
                    result[i] = cipher;
                }
            }
            return result;
        }

        private static List<string> Parts(int value)
        {
            List<string> result = new List<string>();
            int digit = 0;
            digit = (value - value % 100) / 100;
            if (digit != 0)
            {
                result.Add(hundreds[digit]);
            }
            value = value % 100;
            if (value > 0 && value < 10)
            {
                result.Add(digits[value]);
            }
            if (value >= 10 && value < 20)
            {
                value = value % 10;
                result.Add(teens[value]);
            }
            if (value >= 20)
            {
                digit = (value - value % 10) / 10;
                result.Add(dozens[digit]);
                digit = value - digit * 10;
                result.Add(digits[digit]);
            }
            return result;
        }


        private static string GetInterval(int value, int level)
        {
            string result = string.Empty;
            if (level < 3)
            {
                int index = 2; // default
                if (value == 1)
                {
                    index = 0;
                }
                else
                {
                    value = value % 100;
                    if (value != 0 && (value > 20 || value < 10))
                    {
                        value = value % 10;
                        if (value >= 2 && value <= 4)
                        {
                            index = 1;
                        }
                    }
                }
                result = intervals[level, index];
            }
            return result;
        }
    }
}
