using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace System.ComponentModel.DataAnnotations
{
  public class RegonValidationAttribute : ValidationAttribute
  {
    public override bool IsValid(object value)
    {
      if (value == null)
        return true;
      string regonString = value as string;
      long regon;

      if (long.TryParse(regonString, out regon))
      {
        int[] digitArray = new int[regonString.Length];

        for (int i = 0; i != digitArray.Length; i++)
        {
          digitArray[i] = int.Parse(regonString[i].ToString());
        }

        if (digitArray.Length == 9)
        {
          int sum = digitArray[0] * 8
              + digitArray[1] * 9
              + digitArray[2] * 2
              + digitArray[3] * 3
              + digitArray[4] * 4
              + digitArray[5] * 5
              + digitArray[6] * 6
              + digitArray[7] * 7;

          int mod = sum % 11;

          if (mod == 10)
            mod = 0;

          if (mod == digitArray[8])
            return true;
        }
        else if (digitArray.Length == 14)
        {
          int sum = digitArray[0] * 2
              + digitArray[1] * 4
              + digitArray[2] * 8
              + digitArray[3] * 5
              + digitArray[4] * 0
              + digitArray[5] * 9
              + digitArray[6] * 7
              + digitArray[7] * 3
              + digitArray[8] * 6
              + digitArray[9] * 1
              + digitArray[10] * 2
              + digitArray[11] * 4
              + digitArray[12] * 8;

          int mod = sum % 11;

          if (mod == 10)
            mod = 0;

          if (mod == digitArray[13])
            return true;
        }
      }
      return false;
    }

  }
}