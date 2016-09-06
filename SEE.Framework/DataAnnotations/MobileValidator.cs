using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;

namespace System.ComponentModel.DataAnnotations
{
	public class MobileValidationAttribute : ValidationAttribute
	{
		public override bool IsValid(object value)
		{
			if (value == null) return true;
			if (value.ToString().Length != 9) return false;

			int number = 0;
			return Int32.TryParse(value.ToString(), out number);
		}
	}
}