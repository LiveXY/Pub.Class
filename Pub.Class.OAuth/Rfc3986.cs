using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Globalization;

namespace Pub.Class
{
    /// <summary>
    /// Performs RFC 3986 encoding and decoding.
    /// http://www.apps.ietf.org/rfc/rfc3986.html
    /// </summary>
    public static class Rfc3986
    {
        /// <summary>
        /// RFC 3986 percent encoding escape sequence
        /// </summary>
        public static readonly Regex Rfc3986EscapeSequence =
            new Regex("%([0-9A-Fa-f]{2})", RegexOptions.Compiled);

        /// <summary>
        /// Join the name-value pairs into a string seperated with ampersands.
        /// Each name and value is first RFC 3986 encoded and values are separated
        /// from names with equal signs.
        /// </summary>
        /// <param name="values">The name value collection to encode and join</param>
        /// <returns>An RFC 3986 compliant string</returns>
        public static string EncodeAndJoin(NameValueCollection values)
        {
            if (values == null)
                return string.Empty;

            StringBuilder enc = new StringBuilder();

            bool first = true;
            foreach (string key in values.Keys)
            {
                string encKey = Rfc3986.Encode(key);
                foreach (string value in values.GetValues(key))
                {
                    if (!first)
                        enc.Append("&");
                    else
                        first = false;

                    enc.Append(encKey).Append("=").Append(Rfc3986.Encode(value));
                }
            }

            return enc.ToString();
        }

        /// <summary>
        /// Splits a ampersand-separated list of key-value pairs, decodes the keys and 
        /// values, and adds them to a NameValueCollection. Keys and values are separated
        /// by equals signs.
        /// </summary>
        /// <param name="input">The key-value pair list</param>
        /// <returns>A name value collection, which may be empty.</returns>
        /// <exception cref="System.FormatException">
        /// If the string is not a series of key-value pairs separated by ampersands,
        /// or if one of the keys is null or empty, or if one of the keys or values is 
        /// not properly encoded.
        /// </exception>
        public static NameValueCollection SplitAndDecode(string input)
        {
            NameValueCollection parameters = new NameValueCollection();

            if (string.IsNullOrEmpty(input))
                return parameters;

            foreach (string pair in input.Split('&'))
            {
                string[] parts = pair.Split('=');

                if (parts.Length != 2)
                    throw new FormatException("Pair is not a key-value pair");

                string key = Rfc3986.Decode(parts[0]);
                if (string.IsNullOrEmpty(key))
                    throw new FormatException("Key cannot be null or empty");

                string value = Rfc3986.Decode(parts[1]);

                parameters.Add(key, value);
            }

            return parameters;
        }

        /// <summary>
        /// Perform RFC 3986 Percent-encoding on a string.
        /// </summary>
        /// <param name="input">The input string</param>
        /// <returns>The RFC 3986 Percent-encoded string</returns>
        public static string Encode(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return Encoding.ASCII.GetString(EncodeToBytes(input, Encoding.UTF8));
        }

        /// <summary>
        /// Perform RFC 3986 Percent-decoding on a string.
        /// </summary>
        /// <param name="input">The input RFC 3986 Percent-encoded string</param>
        /// <returns>The decoded string</returns>
        public static string Decode(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            return Rfc3986EscapeSequence.Replace(
                input,
                (Match match) =>
                {
                    if (match.Success)
                    {
                        Group hexgrp = match.Groups[1];

                        return string.Format(
                            CultureInfo.InvariantCulture,
                            "{0}",
                            (char)int.Parse(hexgrp.Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture));
                    }

                    throw new FormatException("Could not RFC 3986 decode string");
                });
        }

        private static byte[] EncodeToBytes(string input, Encoding enc)
        {
            if (string.IsNullOrEmpty(input))
                return new byte[0];

            byte[] inbytes = enc.GetBytes(input);

            // Count unsafe characters
            int unsafeChars = 0;
            char c;
            foreach (byte b in inbytes)
            {
                c = (char)b;

                if (NeedsEscaping(c))
                    unsafeChars++;
            }

            // Check if we need to do any encoding
            if (unsafeChars == 0)
                return inbytes;

            byte[] outbytes = new byte[inbytes.Length + (unsafeChars * 2)];
            int pos = 0;

            for (int i = 0; i < inbytes.Length; i++)
            {
                byte b = inbytes[i];

                if (NeedsEscaping((char)b))
                {
                    outbytes[pos++] = (byte)'%';
                    outbytes[pos++] = (byte)IntToHex((b >> 4) & 0xf);
                    outbytes[pos++] = (byte)IntToHex(b & 0x0f);
                }
                else
                    outbytes[pos++] = b;
            }

            return outbytes;
        }

        private static bool NeedsEscaping(char c)
        {
            return !((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9')
                    || c == '-' || c == '_' || c == '.' || c == '~');
        }

        private static char IntToHex(int n)
        {
            if (n < 0 || n >= 16)
                throw new ArgumentOutOfRangeException("n");

            if (n <= 9)
                return (char)(n + (int)'0');
            else
                return (char)(n - 10 + (int)'A');
        }
    }
}
