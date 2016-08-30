using System;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Edreamer.Framework.Helpers
{
    /// <summary>
    /// Provides extension methods for strings.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Removes all leading and trailing occurrences of a set of strings specified
        /// in an array from the current System.String object.
        /// </summary>
        /// <param name="string">The string to format.</param>
        /// <param name="trimStrings">An array of strings to remove, or null.</param>
        /// <returns>
        /// The string that remains after all occurrences of the strings in the trimStrings
        /// parameter are removed from the start and end of the current string. If trimStrings
        /// is null or an empty array, white-space characters are removed instead.
        /// </returns>
        public static string Trim(this string @string, params string[] trimStrings)
        {
            return Trim(@string, StringComparison.CurrentCulture, trimStrings);
        }

        /// <summary>
        /// Removes all leading and trailing occurrences of a set of strings specified
        /// in an array from the current System.String object.
        /// </summary>
        /// <param name="string">The string to format.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how this string and trimStrings are compared.</param>
        /// <param name="trimStrings">An array of strings to remove, or null.</param>
        /// <returns>
        /// The string that remains after all occurrences of the strings in the trimStrings
        /// parameter are removed from the start and end of the current string. If trimStrings
        /// is null or an empty array, white-space characters are removed instead.
        /// </returns>
        public static string Trim(this string @string, StringComparison comparisonType, params string[] trimStrings)
        {
            Throw.IfArgumentNull(@string, "string");
            var result = @string;
            result = result.TrimStart(comparisonType, trimStrings);
            result = result.TrimEnd(comparisonType, trimStrings);
            return result;
        }

        /// <summary>
        /// Removes all trailing occurrences of a set of strings specified in an array
        /// from the current System.String object.
        /// </summary>
        /// <param name="string">The string to format.</param>
        /// <param name="trimStrings">An array of strings to remove, or null.</param>
        /// <returns>
        /// The string that remains after all occurrences of the strings in the trimStrings
        /// parameter are removed from the end of the current string. If trimStrings
        /// is null or an empty array, white-space characters are removed instead.
        /// </returns>
        public static string TrimEnd(this string @string, params string[] trimStrings)
        {
            return TrimEnd(@string, StringComparison.CurrentCulture, trimStrings);
        }

        /// <summary>
        /// Removes all trailing occurrences of a set of strings specified in an array
        /// from the current System.String object.
        /// </summary>
        /// <param name="string">The string to format.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how this string and trimStrings are compared.</param>
        /// <param name="trimStrings">An array of strings to remove, or null.</param>
        /// <returns>
        /// The string that remains after all occurrences of the strings in the trimStrings
        /// parameter are removed from the end of the current string. If trimStrings
        /// is null or an empty array, white-space characters are removed instead.
        /// </returns>
        public static string TrimEnd(this string @string, StringComparison comparisonType, params string[] trimStrings)
        {
            Throw.IfArgumentNull(@string, "string");
            if (CollectionHelpers.IsNullOrEmpty(trimStrings))
            {
                return @string.TrimEnd();
            }

            var str = "";
            var result = @string;

            for (int i = @string.Length - 1; i > 0; i--)
            {
                str = @string[i] + str;
                if (trimStrings.Any(s => s.Equals(str, comparisonType)))
                {
                    result = @string.Substring(0, i);
                    str = "";
                }
            }

            return result;
        }

        /// <summary>
        /// Removes all leading occurrences of a set of strings specified in an array
        /// from the current System.String object.
        /// </summary>
        /// <param name="string">The string to format.</param>
        /// <param name="trimStrings">An array of strings to remove, or null.</param>
        /// <returns>
        /// The string that remains after all occurrences of the strings in the trimStrings
        /// parameter are removed from the start of the current string. If trimStrings
        /// is null or an empty array, white-space characters are removed instead.
        /// </returns>
        public static string TrimStart(this string @string, params string[] trimStrings)
        {
            return TrimStart(@string, StringComparison.CurrentCulture, trimStrings);
        }

        /// <summary>
        /// Removes all leading occurrences of a set of strings specified in an array
        /// from the current System.String object.
        /// </summary>
        /// <param name="string">The string to format.</param>
        /// <param name="comparisonType">One of the enumeration values that determines how this string and trimStrings are compared.</param>
        /// <param name="trimStrings">An array of strings to remove, or null.</param>
        /// <returns>
        /// The string that remains after all occurrences of the strings in the trimStrings
        /// parameter are removed from the start of the current string. If trimStrings
        /// is null or an empty array, white-space characters are removed instead.
        /// </returns>
        public static string TrimStart(this string @string, StringComparison comparisonType, params string[] trimStrings)
        {
            Throw.IfArgumentNull(@string, "string");
            if (CollectionHelpers.IsNullOrEmpty(trimStrings))
            {
                return @string.TrimStart();
            }

            var str = "";
            var result = @string;

            for (int i = 0; i < @string.Length; i++)
            {
                str = str + @string[i];
                if (trimStrings.Any(s => s.Equals(str, comparisonType)))
                {
                    result = @string.Substring(i + 1);
                    str = "";
                }
            }

            return result;
        }

        /// <summary>
        /// Determines whether the current System.String object encloses by the specified strings.
        /// </summary>
        /// <param name="string">The current instance of string.</param>
        /// <param name="start">The string to compare to the substring at the start of this instance.</param>
        /// <param name="end">The string to compare to the substring at the end of this instance.</param>
        /// <returns>true if this instance encloses between start and end strings; otherwise, false.</returns>
        public static bool EnclosedBy(this string @string, string start, string end)
        {
            Throw.IfArgumentNull(@string, "string");
            return @string.StartsWith(start) && @string.EndsWith(end);
        }


        /// <summary>
        /// Determines whether this instance and another specified <see cref="String"/> object have the same value,
        /// using <see cref="StringComparison.OrdinalIgnoreCase"/> rule.
        /// </summary>
        /// <param name="string">The current instance of string.</param>
        /// <param name="value">The string to compare to this instance.</param>
        /// <returns>true if the value of the value parameter is the same as this instance; otherwise, false.</returns>
        public static bool EqualsIgnoreCase(this string @string, string value)
        {
            return String.Equals(@string, @value, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Replaces the format item in the current string instance with the string representation
        /// of a corresponding object in a specified array using invariant culture.
        /// </summary>
        /// <param name="string">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns></returns>
        [StringFormatMethod("string")]
        public static string FormatWith(this string @string, params object[] args)
        {
            return String.Format(@string, args);
        }

        public static string UnformatWith(this string @string, params string[] placeHolders)
        {
            var escapedString = @string.Replace("{", "{{").Replace("}", "}}");
            for (int i = 0; i < placeHolders.Length; i++)
            {
                escapedString = escapedString.Replace(placeHolders[i], "{" + i + "}");
            }
            return escapedString;
        }

        // From Orchard CMS
        public static byte[] ToByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length).
                Where(x => 0 == x % 2).
                Select(x => Convert.ToByte(hex.Substring(x, 2), 16)).
                ToArray();
        }

        // From Orchard CMS
        public static string ToHexString(this byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        // From Orchard CMS
        public static string Ellipsize(this string text, int characterCount)
        {
            //return text.Ellipsize(characterCount, "&#160;&#8230;");
            return text.Ellipsize(characterCount, "...");
        }

        // From Orchard CMS
        public static string Ellipsize(this string text, int characterCount, string ellipsis)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            if (characterCount < 0 || text.Length <= characterCount)
                return text;

            return Regex.Replace(text.Substring(0, characterCount + 1), @"\s+\S*$", "") + ellipsis;
        }
    }
}