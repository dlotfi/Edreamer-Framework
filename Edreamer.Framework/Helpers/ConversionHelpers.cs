// Based on original work of Thorsten Bruning - http://www.codeproject.com/Articles/248440/Universal-Type-Converter

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Edreamer.Framework.Helpers
{
    public static class ConversionHelpers
    {

        /// <summary>
        /// Converts the given value to the given type using the invarant CultureInfo.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="value">The value wich is converted.</param>
        /// <param name="destinationType">The type to which the given value is converted.</param>
        /// <param name="result">An Object instance of type <paramref name="destinationType">destinationType</paramref> whose value is equivalent to the given <paramref name="value">value</paramref> if the operation succeeded.</param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryConvert(object value, Type destinationType, out object result)
        {
            return TryConvert(value, destinationType, out result, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts the given value to the given type using the given CultureInfo.
        /// A return value indicates whether the operation succeeded.
        /// </summary>
        /// <param name="value">The value wich is converted.</param>
        /// <param name="destinationType">The type to which the given value is converted.</param>
        /// <param name="result">An Object instance of type <paramref name="destinationType">destinationType</paramref> whose value is equivalent to the given <paramref name="value">value</paramref> if the operation succeeded.</param>
        /// <param name="culture">The CultureInfo to use as the current culture.</param>
        /// <returns>true if <paramref name="value"/> was converted successfully; otherwise, false.</returns>
        public static bool TryConvert(object value, Type destinationType, out object result, CultureInfo culture)
        {
            Throw.IfArgumentNull(destinationType, "destinationType");

            if (destinationType == typeof(object))
            {
                result = value;
                return true;
            }

            if (value == null)
            {
                result = destinationType.IsValueType ? Activator.CreateInstance(destinationType) : null;
                return true;
            }

            if (destinationType.IsInstanceOfType(value))
            {
                result = value;
                return true;
            }

            var undeLyingDestinationType = Nullable.GetUnderlyingType(destinationType);
            object tmpResult = null;

            if (undeLyingDestinationType != null && TryConvertCore(value, undeLyingDestinationType, ref tmpResult, culture))
            {
                result = tmpResult;
                return true;
            }
            if (TryConvertCore(value, destinationType, ref tmpResult, culture))
            {
                result = tmpResult;
                return true;
            }
            result = destinationType.GetDefault();
            return false;
        }

        #region Core

        private const string ImplicitOperatorMethodName = "op_Implicit";
        private const string ExplicitOperatorMethodName = "op_Explicit";

        private static bool TryConvertCore(object value, Type destinationType, ref object result, CultureInfo culture)
        {
            if (value.GetType() == destinationType)
            {
                result = value;
                return true;
            }
            if (TryConvertByDefaultTypeConverters(value, destinationType, culture, ref result))
            {
                return true;
            }
            if (TryConvertByIConvertibleImplementation(value, destinationType, culture, ref result))
            {
                return true;
            }
            if (TryConvertXPlicit(value, destinationType, ExplicitOperatorMethodName, ref result))
            {
                return true;
            }
            if (TryConvertXPlicit(value, destinationType, ImplicitOperatorMethodName, ref result))
            {
                return true;
            }
            if (TryConvertByIntermediateConversion(value, destinationType, ref result, culture))
            {
                return true;
            }
            if (destinationType.IsEnum)
            {
                if (TryConvertToEnum(value, destinationType, ref result))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryConvertByDefaultTypeConverters(object value, Type destinationType, CultureInfo culture, ref object result)
        {
            var converter = TypeDescriptor.GetConverter(destinationType);
            if (converter.CanConvertFrom(value.GetType()))
            {
                try
                {
                    // ReSharper disable AssignNullToNotNullAttribute
                    result = converter.ConvertFrom(null, culture, value);
                    // ReSharper restore AssignNullToNotNullAttribute
                    return true;
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch
                {
                    // ReSharper restore EmptyGeneralCatchClause
                }
            }

            converter = TypeDescriptor.GetConverter(value);
            if (converter.CanConvertTo(destinationType))
            {
                try
                {
                    result = converter.ConvertTo(null, culture, value, destinationType);
                    return true;

                }
                // ReSharper disable EmptyGeneralCatchClause
                catch
                {
                    // ReSharper restore EmptyGeneralCatchClause
                }
            }
            return false;
        }

        private static bool TryConvertByIConvertibleImplementation(object value, Type destinationType, IFormatProvider formatProvider, ref object result)
        {
            var convertibleValue = value as IConvertible;
            if (convertibleValue != null)
            {
                try
                {
                    if (destinationType == typeof(Boolean))
                    {
                        result = convertibleValue.ToBoolean(formatProvider);
                        return true;
                    }
                    if (destinationType == typeof(Byte))
                    {
                        result = convertibleValue.ToByte(formatProvider);
                        return true;
                    }
                    if (destinationType == typeof(Char))
                    {
                        result = convertibleValue.ToChar(formatProvider);
                        return true;
                    }
                    if (destinationType == typeof(DateTime))
                    {
                        result = convertibleValue.ToDateTime(formatProvider);
                        return true;
                    }
                    if (destinationType == typeof(Decimal))
                    {
                        result = convertibleValue.ToDecimal(formatProvider);
                        return true;
                    }
                    if (destinationType == typeof(Double))
                    {
                        result = convertibleValue.ToDouble(formatProvider);
                        return true;
                    }
                    if (destinationType == typeof(Int16))
                    {
                        result = convertibleValue.ToInt16(formatProvider);
                        return true;
                    }
                    if (destinationType == typeof(Int32))
                    {
                        result = convertibleValue.ToInt32(formatProvider);
                        return true;
                    }
                    if (destinationType == typeof(Int64))
                    {
                        result = convertibleValue.ToInt64(formatProvider);
                        return true;
                    }
                    if (destinationType == typeof(SByte))
                    {
                        result = convertibleValue.ToSByte(formatProvider);
                        return true;
                    }
                    if (destinationType == typeof(Single))
                    {
                        result = convertibleValue.ToSingle(formatProvider);
                        return true;
                    }
                    if (destinationType == typeof(UInt16))
                    {
                        result = convertibleValue.ToUInt16(formatProvider);
                        return true;
                    }
                    if (destinationType == typeof(UInt32))
                    {
                        result = convertibleValue.ToUInt32(formatProvider);
                        return true;
                    }
                    if (destinationType == typeof(UInt64))
                    {
                        result = convertibleValue.ToUInt64(formatProvider);
                        return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        private static bool TryConvertXPlicit(object value, Type destinationType, string operatorMethodName, ref object result)
        {
            if (TryConvertXPlicit(value, value.GetType(), destinationType, operatorMethodName, ref result))
            {
                return true;
            }
            if (TryConvertXPlicit(value, destinationType, destinationType, operatorMethodName, ref result))
            {
                return true;
            }
            return false;
        }

        private static bool TryConvertXPlicit(object value, Type invokerType, Type destinationType, string xPlicitMethodName, ref object result)
        {
            var methods = invokerType.GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (var method in methods.Where(m => m.Name == xPlicitMethodName))
            {
                if (destinationType.IsAssignableFrom(method.ReturnType))
                {
                    var parameters = method.GetParameters();
                    if (parameters.Count() == 1 && parameters[0].ParameterType == value.GetType())
                    {
                        try
                        {
                            result = method.Invoke(null, new[] { value });
                            return true;
                        }
                        // ReSharper disable EmptyGeneralCatchClause
                        catch
                        {
                            // ReSharper restore EmptyGeneralCatchClause
                        }
                    }
                }
            }
            return false;
        }

        private static bool TryConvertByIntermediateConversion(object value, Type destinationType, ref object result, CultureInfo culture)
        {
            if (value is char && (destinationType == typeof(double) || destinationType == typeof(float)))
            {
                return TryConvertCore(Convert.ToInt16(value), destinationType, ref result, culture);
            }
            if ((value is double || value is float) && destinationType == typeof(char))
            {
                return TryConvertCore(Convert.ToInt16(value), destinationType, ref result, culture);
            }
            return false;
        }

        private static bool TryConvertToEnum(object value, Type destinationType, ref object result)
        {
            try
            {
                result = Enum.ToObject(destinationType, value);
                return true;
            }
            catch
            {
                return false;
            }
        } 

        #endregion
    }
}
