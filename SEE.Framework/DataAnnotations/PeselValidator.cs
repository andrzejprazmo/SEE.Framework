using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace System.ComponentModel.DataAnnotations
{
	public class PeselValidationAttribute : ValidationAttribute
	{
		private static readonly int[] multipliers = { 1, 3, 7, 9, 1, 3, 7, 9, 1, 3, 1 };

		public override bool IsValid(object value)
		{
			if (value == null)
				return true;

			string pesel = value as string;

			if (pesel == null || !Regex.IsMatch(pesel, @"^\d{11}$"))
				return false;

			int sum = 0;

			for (int i = 0; i < 11; i++)
				sum += multipliers[i] * int.Parse(pesel.Substring(i, 1));

			return sum % 10 == 0;
		}
	}
}