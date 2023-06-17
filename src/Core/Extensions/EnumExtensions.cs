using Core.Attributes;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Core.Extensions
{
    public static class EnumExtensions
    {
        public static string Description<T>(this T e) where T : IConvertible
        {
            try
            {
                if (e is Enum)
                {
                    Type type = e.GetType();
                    Array values = Enum.GetValues(type);

                    foreach (int val in values)
                    {
                        if (val == e.ToInt32(CultureInfo.InvariantCulture))
                        {
                            var memInfo = type.GetMember(type.GetEnumName(val));
                            var descriptionAttribute = memInfo[0]
                                .GetCustomAttributes(typeof(DescriptionAttribute), false)
                                .FirstOrDefault() as DescriptionAttribute;

                            return descriptionAttribute.Description;
                        }
                    }
                }
            }
            catch { }

            return string.Empty;
        }

        public static string Identifier<T>(this T e) where T : IConvertible
        {
            try
            {
                if (e is Enum)
                {
                    Type type = e.GetType();
                    Array values = Enum.GetValues(type);

                    foreach (int val in values)
                    {
                        if (val == e.ToInt32(CultureInfo.InvariantCulture))
                        {
                            var memInfo = type.GetMember(type.GetEnumName(val));
                            var attribute = memInfo[0]
                                .GetCustomAttributes(typeof(IdentifierAttribute), false)
                                .FirstOrDefault() as IdentifierAttribute;

                            return attribute.Identifier;
                        }
                    }
                }
            }
            catch { }

            return Guid.Empty.ToString();
        }

        public static int? ToInt<T>(this T e) where T : IConvertible
        {
            try
            {
                if (e is Enum)
                {
                    Type type = e.GetType();
                    Array values = Enum.GetValues(type);

                    foreach (int val in values)
                    {
                        if (val == e.ToInt32(CultureInfo.InvariantCulture))
                            return val;
                    }
                }
            }
            catch { }

            return null;
        }

        public static string ToString<T>(this T e) where T : IConvertible
        {
            try
            {
                if (e is Enum)
                {
                    var result = e.ToInt();

                    if (result != null)
                        return result.Value.ToString();
                }
            }
            catch { }

            return string.Empty;
        }
    }
}
