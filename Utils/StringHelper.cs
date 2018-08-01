using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Utils
{
    public static class StringHelper
    {
        public static string Join(List<string> items, string separator)
        {
            return String.Join(separator, items.FindAll(IsValid).ConvertAll(ConvertParser.GetConvertValue<string>).ToArray());
        }

        public static string GetTreeViewItemSaveValue(long treeViewItemEntityTypeId, long treeViewItemId)
        {
            return String.Format("{0}:{1}", treeViewItemEntityTypeId, treeViewItemId);
        }

        public static long GetTreeViewItemEntityTypeId(string treeViewItem)
        {
            return ConvertParser.GetConvertValue<long>(treeViewItem.Split(':')[0].Trim());
        }

        public static long GetTreeViewItemId(string treeViewItem)
        {
            return ConvertParser.GetConvertValue<long>(treeViewItem.Split(':')[1].Trim());
        }

        private static bool IsValid<T>(T value)
        {
            if (typeof(T) == typeof(string))
                return !String.IsNullOrEmpty(ConvertParser.GetConvertValue<string>(value));

            return value != null;
        }

        public static string NormalizeDecimalSeparator(string str)
        {
            return NormalizeDecimalSeparator(str, System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);
        }

        public static string GetFloatingPointObjectToString(object value)
        {
            if (value == null)
                return String.Empty;

            string stringValue = value.ToString();
            if (value.GetType() == typeof(double) || value.GetType() == typeof(float))
                stringValue = GetFloatingPointTypeToStringInvariantCulture((IFormattable)value);
            else if (value.GetType() == typeof(string) && stringValue.Contains("E"))
                stringValue = GetConvertExponentialFormatStringInvariantCulture(stringValue);

            return NormalizeDecimalSeparator(stringValue);
        }

        private static string GetFloatingPointTypeToStringInvariantCulture(IFormattable value)
        {
            string R = value.ToString("R", CultureInfo.InvariantCulture);
            if (!R.Contains("E"))
                return R;

            string G17 = value.ToString("G17", CultureInfo.InvariantCulture);

            if (!G17.Contains("E"))
                return G17;

            return GetConvertExponentialFormatStringInvariantCulture(R);

        }

        private static string NormalizeDecimalSeparator(string str, string requiredSeparator)
        {
            return NormalizeDecimalSeparator(str, requiredSeparator, ",.");
        }

        private static string NormalizeDecimalSeparator(string str, string requiredSeparator, string possibleSeparators)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char ch in str)
                if (possibleSeparators.IndexOf(ch) != -1)
                    sb.Append(requiredSeparator);
                else
                    sb.Append(ch);

            return sb.ToString();
        }

        private static string GetConvertExponentialFormatStringInvariantCulture(string value)
        {
            string valueString = NormalizeDecimalSeparator(value, NumberFormatInfo.InvariantInfo.NumberDecimalSeparator);
            int i = valueString.IndexOf('E');
            string beforeTheE = valueString.Substring(0, i);
            int E = Convert.ToInt32(valueString.Substring(i + 1));

            i = beforeTheE.IndexOf('.');

            if (i < 0)
                i = beforeTheE.Length;
            else
                beforeTheE = beforeTheE.Replace(".", String.Empty);

            i += E;

            while (i < 1)
            {
                beforeTheE = "0" + beforeTheE;
                i++;
            }

            while (i > beforeTheE.Length)
                beforeTheE += "0";

            if (i == beforeTheE.Length)
                return beforeTheE;

            return String.Format("{0}.{1}", beforeTheE.Substring(0, i), beforeTheE.Substring(i));
        }

    }
}