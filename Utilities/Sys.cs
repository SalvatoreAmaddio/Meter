using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Meter.Utilities {

    public static class Sys {
        public static bool IsValidEmail(string val) => val.Contains("@");
        public static bool IsNumeric(object val) =>int.TryParse(val.ToString(), out _);
        public static bool IsNumericInt64(object val) => Int64.TryParse(val.ToString(), out _);

        public static bool IsDouble(string val) => double.TryParse(val, out _);

        public static string PhoneFormat(ref string val)
        {
            switch (val.Length) {
                case 2:
                    val = val + " ";
                break;
                    case 4:
                    val = val + " ";
                    break;
            }
            return val;
        }

        public static string FirstLetterUpper(ref string val) {

            try
            {
                val= val.First().ToString().ToUpper() + val.Substring(1).ToLower();
                return val;
            }
            catch { return string.Empty; }
        }

        public static string EachFirstLetterUpper(ref string val)
        {

            try
            {

                val = val.Trim();
                var x = val.Split(' ');
                string val2 = "";
                for (int i = 0; i < x.Count();i++) {
                        val2 = val2 + FirstLetterUpper(ref x[i]) + " ";
                }

                return val2.Trim();
            }
            catch { return string.Empty; }
        }

        public static M Nz<M>(M var, M ifNull) {
            if (string.IsNullOrEmpty(var.ToString())) {
                var = ifNull;
            }
            return (M)var;
        }
    }
}
