using System;

namespace Utils
{
    public static class ConvertParser
    {
        public static T GetConvertValue<T>(object value)
        {
            if (value == DBNull.Value || value == null || value is string && String.IsNullOrEmpty((string)value))
            {
                if (typeof(T) == typeof(string))
                    value = String.Empty;
                else
                    return default(T);
            }
            //не менять порядок if-ов
            if (IsFloatingPointType(value.GetType()) && typeof(T) == typeof(string))
                value = StringHelper.GetFloatingPointObjectToString(value);

            if (value is string && IsFloatingPointType(typeof(T)))
                return GetValueTypeOfFloatingPoint<T>(value.ToString());

            return (T)Convert.ChangeType(value, typeof(T));
        }

        private static T GetValueTypeOfFloatingPoint<T>(string text)
        {
            string normalizedStr = StringHelper.NormalizeDecimalSeparator(text);
            return (T)Convert.ChangeType(normalizedStr, typeof(T));
        }

        private static bool IsFloatingPointType(Type type)
        {
            return type == typeof(double) || type == typeof(float) || type == typeof(decimal);
        }
    }
}