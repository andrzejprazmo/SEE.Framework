﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace System.ComponentModel.DataAnnotations
{
	public class EmailValidationAttribute : ValidationAttribute
	{
		bool invalid = false;

		public override bool IsValid(object value)
		{
			if (value == null)
			{
				return true;
			}
			string strIn = value.ToString();

			if (String.IsNullOrEmpty(strIn)) return true;

			invalid = false;

			// Use IdnMapping class to convert Unicode domain names. 
			try
			{
				strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper, RegexOptions.None);
			}
			catch (Exception)
			{
				return false;
			}

			if (invalid) return false;

			// Return true if strIn is in valid e-mail format. 
			try
			{
				return Regex.IsMatch(strIn,
							@"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
							@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
							RegexOptions.IgnoreCase);
			}
			catch (Exception)
			{
				return false;
			}
		}

		private string DomainMapper(Match match)
		{
			// IdnMapping class with default property values.
			IdnMapping idn = new IdnMapping();

			string domainName = match.Groups[2].Value;
			try
			{
				domainName = idn.GetAscii(domainName);
			}
			catch (ArgumentException)
			{
				invalid = true;
			}
			return match.Groups[1].Value + domainName;
		}
	}
}
