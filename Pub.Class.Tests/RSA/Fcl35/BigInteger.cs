using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Fcl35.Numeric {
    sealed class ImmutableAttribute : Attribute {
    }

    sealed class PureAttribute : Attribute {
    }

    static class Contract {
        public static void Requires(Exception ex) {
            if (ex != null) throw ex;
        }

        public static void Requires(bool ok) {
            if (!ok) throw new Exception("Contract.Requires fail");
        }
    }

    static class Res {
        public static readonly string Overflow_Byte = "Overflow System.Byte";
        public static readonly string Overflow_SByte = "Overflow System.SBtye";
        public static readonly string Overflow_Int16 = "Overflow System.Int16";
        public static readonly string Overflow_UInt16 = "Overflow System.UInt16";
        public static readonly string Overflow_Int32 = "Overflow System.Int32";
        public static readonly string Overflow_UInt32 = "Overflow System.UInt32";
        public static readonly string Overflow_Int64 = "Overflow System.In64";
        public static readonly string Overflow_UInt64 = "Overflow System.UInt64";
        public static readonly string Overflow_Single = "Overflow System.Single";
        public static readonly string Overflow_Double = "Overflow System.Double";
        public static readonly string Overflow_Decimal = "Overflow System.Decimal";
        public static readonly string UnsupportedNumberStyle = "Unsupported number style";
        public static readonly string ParsedStringWasInvalid = "Parsed string was invalid";
        public static readonly string InvalidCharactersInString = "Invalid characters in string";
        public static readonly string MustBeBigInt = "Must be BigInteger";
        public static readonly string MustBePositive = "Must be positive";
        public static readonly string BigIntInfinity = "BigInteger infinity";
        public static readonly string NotANumber = "Not a number";
        public static readonly string NonNegative = "Non negative";
    }

    [Serializable, StructLayout(LayoutKind.Sequential), Immutable, ComVisible(false)]
    public struct BigInteger : IFormattable, IEquatable<BigInteger>, IComparable<BigInteger>, IComparable {
        private const int DecimalScaleFactorMask = 0xff0000;
        private const int DecimalSignMask = int.MinValue; // -2147483648;
        private const int BitsPerDigit = 0x20;
        private const ulong Base = uint.MaxValue + 1UL; // 0x100000000L;
        private const int UpperBoundForSchoolBookMultiplicationDigits = 0x40;
        private const int ForceSchoolBookMultiplicationThresholdDigits = 8;
        private static readonly uint[] maxCharsPerDigit;
        private static readonly uint[] groupRadixValues;
        private static readonly uint[] zeroArray;
        private readonly short _sign;
        private readonly uint[] _data;
        private int _length;
        public static BigInteger Zero { get { return new BigInteger(0, zeroArray); } }
        public static BigInteger One { get { return new BigInteger(1); } }
        public static BigInteger MinusOne { get { return new BigInteger(-1); } }

        public BigInteger(int value) {
            if (value == 0) {
                this._sign = 0;
                this._data = new uint[0];
            } else if (value < 0) {
                this._sign = -1;
                this._data = new uint[] { (uint)-value };
            } else {
                this._sign = 1;
                this._data = new uint[] { (uint)value };
            }
            this._length = -1;
        }

        public BigInteger(long value) {
            ulong num = 0L;
            if (value < 0L) {
                num = (ulong)-value;
                this._sign = -1;
            } else if (value > 0L) {
                num = (ulong)value;
                this._sign = 1;
            } else {
                this._sign = 0;
            }
            if (num >= Base /*0x100000000L*/) {
                this._data = new uint[] { (uint)num, (uint)(num >> BitsPerDigit /* 0x20 */) };
            } else {
                this._data = new uint[] { (uint)num };
            }
            this._length = -1;
        }

        [CLSCompliant(false)]
        public BigInteger(uint value) {
            if (value == 0) {
                this._sign = 0;
            } else {
                this._sign = 1;
            }
            this._data = new uint[] { value };
            this._length = -1;
        }

        [CLSCompliant(false)]
        public BigInteger(ulong value) {
            if (value == 0L) {
                this._sign = 0;
            } else {
                this._sign = 1;
            }
            if (value >= Base /* 0x100000000L*/) {
                this._data = new uint[] { (uint)value, (uint)(value >> BitsPerDigit /* 0x20 */) };
            } else {
                this._data = new uint[] { (uint)value };
            }
            this._length = -1;
        }

        public BigInteger(float value)
            : this((double)value) {
        }

        public BigInteger(double value) {
            Contract.Requires(!double.IsInfinity(value) ? null : new OverflowException(Res.BigIntInfinity));
            Contract.Requires(!double.IsNaN(value) ? null : new OverflowException(Res.NotANumber));
            byte[] bytes = BitConverter.GetBytes(value);
            ulong mantissa = Mantissa(bytes);
            int exp = Exponent(bytes);
            if (mantissa == 0) {
                if (exp == 0) {
                    this._sign = 0;
                    this._data = zeroArray;
                    this._length = 0;
                    return;
                }
                BigInteger x = IsNegative(bytes) ? Negate(One) : One;
                x = LeftShift(x, exp - 0x3ff);
                this._sign = x._sign;
                this._data = x._data;
            } else {
                mantissa |= 0x10000000000000UL;
                BigInteger x = new BigInteger(mantissa);
                x = (exp > 0x433) ? LeftShift(x, exp - 0x433) : RightShift(x, 0x433 - exp);
                this._sign = IsNegative(bytes) ? (short)-x._sign : x._sign;
                this._data = x._data;
            }
            this._length = -1;
        }

        public BigInteger(decimal value) {
            int[] bits = decimal.GetBits(decimal.Truncate(value));
            int num = 3;
            while ((num > 0) && (bits[num - 1] == 0)) {
                num--;
            }
            this._length = num;
            if (num == 0) {
                this._sign = 0;
                this._data = new uint[0];
            } else {
                uint[] numArray2 = new uint[num];
                numArray2[0] = (uint)bits[0];
                if (num > 1) {
                    numArray2[1] = (uint)bits[1];
                }
                if (num > 2) {
                    numArray2[2] = (uint)bits[2];
                }
                this._sign = (short)(((bits[3] & DecimalSignMask /* -2147483648 */) != 0) ? -1 : 1);
                this._data = numArray2;
            }
        }

        public BigInteger(byte[] value)
            : this(value, false) {
        }

        public BigInteger(byte[] value, bool negative) {
            Contract.Requires((value != null) ? null : new ArgumentNullException("value"));
            int index = value.Length / 4;
            int num2 = value.Length % 4;
            if (num2 > 0) {
                this._data = new uint[index + 1];
            } else {
                this._data = new uint[index];
            }
            Buffer.BlockCopy(value, 0, this._data, 0, index * 4);
            if (num2 > 0) {
                uint num3 = 0;
                for (int i = 0; i < num2; i++) {
                    num3 |= (uint)(value[(index * 4) + i] << (8 * i));
                }
                this._data[index] = num3;
            }
            this._sign = (short)(negative ? -1 : 1);
            this._length = -1;
            if (this.Length == 0) {
                this._sign = 0;
                this._data = zeroArray;
            }
        }

        private BigInteger(int _sign, params uint[] _data) {
            Contract.Requires(_data != null);
            Contract.Requires((_sign >= -1) && (_sign <= 1));
            Contract.Requires((_sign != 0) || (GetLength(_data) == 0));
            if (GetLength(_data) == 0) {
                _sign = 0;
            }
            this._data = _data;
            this._sign = (short)_sign;
            this._length = -1;
        }

        public static BigInteger Abs(BigInteger x) {
            if (x._sign == -1) {
                return -x; // op_UnaryNegation(x);
            }
            return x;
        }

        public static BigInteger GreatestCommonDivisor(BigInteger x, BigInteger y) {
            BigInteger integer;
            BigInteger integer2;
            Contract.Requires((x.Sign != 0) ? null : new ArgumentOutOfRangeException("x", Res.MustBePositive));
            Contract.Requires((y.Sign != 0) ? null : new ArgumentOutOfRangeException("y", Res.MustBePositive));
            x = Abs(x);
            y = Abs(y);
            int num = Compare(x, y);
            if (num == 0) {
                return x;
            }
            if (num < 1) {
                integer = x;
                integer2 = y;
            } else {
                integer = y;
                integer2 = x;
            }
            do {
                BigInteger integer3;
                BigInteger integer4 = integer2;
                DivRem(integer, integer2, out integer3);
                integer2 = integer3;
                integer = integer4;
            }
            while (integer2 != 0);
            return integer;
        }

        public static BigInteger Remainder(BigInteger dividend, BigInteger divisor) {
            BigInteger integer;
            DivRem(dividend, divisor, out integer);
            return integer;
        }

        public static BigInteger Negate(BigInteger x) {
            BigInteger integer = new BigInteger(-x._sign, (x._data == null) ? zeroArray : x._data);
            integer._length = x._length;
            return integer;
        }

        public static BigInteger Pow(BigInteger baseValue, BigInteger exponent) {
            Contract.Requires((exponent >= 0) ? null : new ArgumentOutOfRangeException("exponent", Res.NonNegative));
            if (exponent == 0) {
                return One;
            }
            BigInteger integer = baseValue;
            BigInteger one = One;
            while (exponent > 0) {
                if ((exponent._data[0] & 1) != 0) {
                    one *= integer;
                }
                if (exponent == 1) {
                    return one;
                }
                integer = integer.Square();
                exponent = RightShift(exponent, 1);
            }
            return one;
        }

        public static BigInteger ModPow(BigInteger baseValue, BigInteger exponent, BigInteger modulus) {
            Contract.Requires((exponent >= 0) ? null : new ArgumentOutOfRangeException("exponent", Res.NonNegative));
            if (exponent == 0) {
                return One;
            }
            BigInteger integer = baseValue;
            BigInteger one = One;
            while (exponent > 0) {
                if ((exponent._data[0] & 1) != 0) {
                    one *= integer;
                    one = one % modulus; // op_Modulus(one, modulus);
                }
                if (exponent == 1) {
                    return one;
                }
                integer = integer.Square();
                exponent = RightShift(exponent, 1);
            }
            return one;
        }

        private BigInteger Square() {
            return (this * this);
        }

        public byte[] ToByteArray() {
            bool flag;
            return this.ToByteArray(out flag);
        }

        public byte[] ToByteArray(out bool isNegative) {
            int length = this.Length;
            byte[] dst = new byte[length * 4];
            Buffer.BlockCopy(this._data, 0, dst, 0, length * 4);
            isNegative = this._sign == -1;
            return dst;
        }

        public int Sign {
            [Pure]
            get {
                return this._sign;
            }
        }
        public static BigInteger operator +(BigInteger value) {
            return value;
        }

        public static BigInteger operator -(BigInteger value) {
            return Negate(value);
        }

        public static BigInteger operator ++(BigInteger value) {
            if (value._sign >= 0) {
                return new BigInteger(1, add0(value._data, value.Length, new uint[] { 1 }, 1));
            }
            if ((value.Length == 1) && (value._data[0] == 1)) {
                return Zero;
            }
            return new BigInteger(-1, sub(value._data, value.Length, new uint[] { 1 }, 1));
        }

        public static BigInteger operator --(BigInteger value) {
            uint[] numArray;
            int num;
            int length = value.Length;
            if (value._sign == 1) {
                if ((length == 1) && (value._data[0] == 1)) {
                    return Zero;
                }
                numArray = sub(value._data, length, new uint[] { 1 }, 1);
                num = 1;
            } else {
                numArray = add0(value._data, length, new uint[] { 1 }, 1);
                num = -1;
            }
            return new BigInteger(num, numArray);
        }

        public static BigInteger operator %(BigInteger x, BigInteger y) {
            BigInteger integer;
            if ((x._sign == y._sign) && (x.Length < y.Length)) {
                return x;
            }
            DivRem(x, y, out integer);
            return integer;
        }

        public static explicit operator byte(BigInteger value) {
            if (value._sign == 0) {
                return 0;
            }
            if (value.Length > 1) {
                throw new OverflowException(Res.Overflow_Byte);
            }
            if (value._data[0] > byte.MaxValue /* 0xff */) {
                throw new OverflowException(Res.Overflow_Byte);
            }
            if (value._sign < 0) {
                throw new OverflowException(Res.Overflow_Byte);
            }
            return (byte)value._data[0];
        }

        [CLSCompliant(false)]
        public static explicit operator sbyte(BigInteger value) {
            if (value._sign == 0) {
                return 0;
            }
            if (value.Length > 1) {
                throw new OverflowException(Res.Overflow_SByte);
            }
            if (value._data[0] > sbyte.MaxValue + 1 /* 0x80 */) {
                throw new OverflowException(Res.Overflow_SByte);
            }
            if ((value._data[0] == sbyte.MaxValue + 1 /* 0x80 */) && (value._sign == 1)) {
                throw new OverflowException(Res.Overflow_SByte);
            }
            return (sbyte)((sbyte)value._data[0] * ((sbyte)value._sign));
        }

        public static explicit operator short(BigInteger value) {
            if (value._sign == 0) {
                return 0;
            }
            if (value.Length > 1) {
                throw new OverflowException(Res.Overflow_Int16);
            }
            if (value._data[0] > short.MaxValue + 1 /* 0x8000*/) {
                throw new OverflowException(Res.Overflow_Int16);
            }
            if ((value._data[0] == short.MaxValue + 1 /* 0x8000 */) && (value._sign == 1)) {
                throw new OverflowException(Res.Overflow_Int16);
            }
            return (short)((short)value._data[0] * value._sign);
        }

        [CLSCompliant(false)]
        public static explicit operator ushort(BigInteger value) {
            if (value._sign == 0) {
                return 0;
            }
            if (value.Length > 1) {
                throw new OverflowException(Res.Overflow_UInt16);
            }
            if (value._data[0] > ushort.MaxValue /* 0xffff */) {
                throw new OverflowException(Res.Overflow_UInt16);
            }
            if (value._sign < 0) {
                throw new OverflowException(Res.Overflow_UInt16);
            }
            return (ushort)value._data[0];
        }

        public static explicit operator int(BigInteger value) {
            if (value._sign == 0) {
                return 0;
            }
            if (value.Length > 1) {
                throw new OverflowException(Res.Overflow_Int32);
            }
            if (value._data[0] > int.MaxValue + 1U /* 0x80000000 */) {
                throw new OverflowException(Res.Overflow_Int32);
            }
            if ((value._data[0] == int.MaxValue + 1U /* 0x80000000 */) && (value._sign == 1)) {
                throw new OverflowException(Res.Overflow_Int32);
            }
            return (int)value._data[0] * value._sign;
        }

        [CLSCompliant(false)]
        public static explicit operator uint(BigInteger value) {
            if (value._sign == 0) {
                return 0;
            }
            if (value.Length > 1) {
                throw new OverflowException(Res.Overflow_UInt32);
            }
            if (value._sign < 0) {
                throw new OverflowException(Res.Overflow_UInt32);
            }
            return value._data[0];
        }

        public static explicit operator long(BigInteger value) {
            if (value._sign == 0) {
                return 0L;
            }
            if (value.Length > 2) {
                throw new OverflowException(Res.Overflow_Int64);
            }
            if (value.Length == 1) {
                return value._sign * value._data[0];
            }
            ulong num2 = ((ulong)value._data[1] << BitsPerDigit /* 0x20 */) | value._data[0];
            if (num2 > long.MaxValue + 1UL /* 9223372036854775808L */) {
                throw new OverflowException(Res.Overflow_Int64);
            }
            if ((num2 == long.MaxValue + 1UL /* 9223372036854775808L */) && (value._sign == 1)) {
                throw new OverflowException(Res.Overflow_Int64);
            }
            return (long)num2 * value._sign;
        }

        [CLSCompliant(false)]
        public static explicit operator ulong(BigInteger value) {
            ulong num = 0L;
            if (value._sign == 0) {
                return 0L;
            }
            if (value._sign < 0) {
                throw new OverflowException(Res.Overflow_UInt64);
            }
            if (value.Length > 2) {
                throw new OverflowException(Res.Overflow_UInt64);
            }
            num = value._data[0];
            if (value.Length > 1) {
                num |= (ulong)value._data[1] << BitsPerDigit /* 0x20 */;
            }
            return num;
        }

        public static explicit operator float(BigInteger value) {
            float num;
            NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
            if (!float.TryParse(value.ToString(10, false, numberFormat), NumberStyles.Number, (IFormatProvider)numberFormat, out num)) {
                throw new OverflowException(Res.Overflow_Single);
            }
            return num;
        }

        public static explicit operator double(BigInteger value) {
            double num;
            NumberFormatInfo numberFormat = CultureInfo.InvariantCulture.NumberFormat;
            if (!double.TryParse(value.ToString(10, false, numberFormat), NumberStyles.Number, (IFormatProvider)numberFormat, out num)) {
                throw new OverflowException(Res.Overflow_Double);
            }
            return num;
        }

        public static explicit operator decimal(BigInteger value) {
            if (value._sign == 0) {
                return 0M;
            }
            int length = value.Length;
            if (length > 3) {
                throw new OverflowException(Res.Overflow_Decimal);
            }
            int lo = 0;
            int mid = 0;
            int hi = 0;
            if (length > 2) {
                hi = (int)value._data[2];
            }
            if (length > 1) {
                mid = (int)value._data[1];
            }
            if (length > 0) {
                lo = (int)value._data[0];
            }
            return new decimal(lo, mid, hi, value._sign < 0, 0);
        }

        public static explicit operator BigInteger(float value) {
            return new BigInteger(value);
        }

        public static explicit operator BigInteger(double value) {
            return new BigInteger(value);
        }

        public static explicit operator BigInteger(decimal value) {
            return new BigInteger(value);
        }

        public static implicit operator BigInteger(byte value) {
            return new BigInteger(value);
        }

        [CLSCompliant(false)]
        public static implicit operator BigInteger(sbyte value) {
            return new BigInteger(value);
        }

        public static implicit operator BigInteger(short value) {
            return new BigInteger(value);
        }

        [CLSCompliant(false)]
        public static implicit operator BigInteger(ushort value) {
            return new BigInteger(value);
        }

        [Pure]
        public static implicit operator BigInteger(int value) {
            return new BigInteger(value);
        }

        [CLSCompliant(false)]
        public static implicit operator BigInteger(uint value) {
            return new BigInteger(value);
        }

        public static implicit operator BigInteger(long value) {
            return new BigInteger(value);
        }

        [CLSCompliant(false)]
        public static implicit operator BigInteger(ulong value) {
            return new BigInteger(value);
        }

        private static bool IsNegative(byte[] doubleBits) {
            Contract.Requires(doubleBits.Length == 8);
            return (doubleBits[7] & 0x80) != 0;
        }

        private static ushort Exponent(byte[] doubleBits) {
            Contract.Requires(doubleBits.Length == 8);
            return (ushort)((((doubleBits[7] & 0x7f)) << 4) | ((ushort)(doubleBits[6] & 240) >> 4));
        }

        private static ulong Mantissa(byte[] doubleBits) {
            Contract.Requires(doubleBits.Length == 8);
            ulong num1 = (ulong)doubleBits[0] | ((ulong)doubleBits[1] << 8) | ((ulong)doubleBits[2] << 0x10) | ((ulong)doubleBits[3] << 0x18);
            ulong num2 = (ulong)doubleBits[4] | ((ulong)doubleBits[5] << 8) | (((ulong)doubleBits[6] & 15) << 0x10);
            return num1 | (num2 << 0x20);
        }

        private int Length {
            get {
                if (this._length == -1) {
                    this._length = GetLength(this._data);
                }
                return this._length;
            }
        }
        private static int GetLength(uint[] _data) {
            if (_data == null) {
                return 0;
            }
            int index = _data.Length - 1;
            while ((index >= 0) && (_data[index] == 0)) {
                index--;
            }
            return (index + 1);
        }

        private static uint[] copy(uint[] v) {
            uint[] destinationArray = new uint[v.Length];
            Array.Copy(v, destinationArray, v.Length);
            return destinationArray;
        }

        private static uint[] resize(uint[] v, int len) {
            if (v.Length == len) {
                return v;
            }
            uint[] destinationArray = new uint[len];
            int length = Math.Min(v.Length, len);
            Array.Copy(v, destinationArray, length);
            return destinationArray;
        }

        private static uint[] add0(uint[] x, int xl, uint[] y, int yl) {
            if (xl >= yl) {
                return InternalAdd(x, xl, y, yl);
            }
            return InternalAdd(y, yl, x, xl);
        }

        private static uint[] InternalAdd(uint[] x, int xl, uint[] y, int yl) {
            uint[] v = new uint[xl];
            ulong num2 = 0L;
            int index = 0;
            while (index < yl) {
                num2 = (num2 + x[index]) + y[index];
                v[index] = (uint)num2;
                num2 = num2 >> BitsPerDigit /* 0x20 */;
                index++;
            }
            while ((index < xl) && (num2 != 0L)) {
                num2 += x[index];
                v[index] = (uint)num2;
                num2 = num2 >> BitsPerDigit /* 0x20 */;
                index++;
            }
            if (num2 == 0L) {
                while (index < xl) {
                    v[index] = x[index];
                    index++;
                }
                return v;
            }
            v = resize(v, xl + 1);
            v[index] = (uint)num2;
            return v;
        }

        private static uint[] sub(uint[] x, int xl, uint[] y, int yl) {
            uint[] numArray = new uint[xl];
            bool flag = false;
            int index = 0;
            while (index < yl) {
                uint maxValue = x[index];
                uint num3 = y[index];
                if (flag) {
                    if (maxValue == 0) {
                        maxValue = uint.MaxValue;
                        flag = true;
                    } else {
                        maxValue--;
                        flag = false;
                    }
                }
                if (num3 > maxValue) {
                    flag = true;
                }
                numArray[index] = maxValue - num3;
                index++;
            }
            if (flag) {
                while (index < xl) {
                    uint num4 = x[index];
                    numArray[index] = num4 - 1;
                    if (num4 != 0) {
                        index++;
                        break;
                    }
                    index++;
                }
            }
            while (index < xl) {
                numArray[index] = x[index];
                index++;
            }
            return numArray;
        }

        [Pure]
        public static int Compare(BigInteger x, BigInteger y) {
            if (x._sign == y._sign) {
                int xLength = x.Length;
                int yLength = y.Length;
                if (xLength == yLength) {
                    for (int i = xLength - 1; i >= 0; i--) {
                        if (x._data[i] != y._data[i]) {
                            if (x._data[i] <= y._data[i]) {
                                return -x._sign;
                            }
                            return x._sign;
                        }
                    }
                    return 0;
                }
                if (xLength <= yLength) {
                    return -x._sign;
                }
                return x._sign;
            }
            if (x._sign <= y._sign) {
                return -1;
            }
            return 1;
        }

        [Pure]
        public static bool operator ==(BigInteger x, BigInteger y) {
            return (Compare(x, y) == 0);
        }

        [Pure]
        public static bool operator !=(BigInteger x, BigInteger y) {
            return (Compare(x, y) != 0);
        }

        [Pure]
        public static bool operator <(BigInteger x, BigInteger y) {
            return (Compare(x, y) < 0);
        }

        [Pure]
        public static bool operator <=(BigInteger x, BigInteger y) {
            return (Compare(x, y) <= 0);
        }

        [Pure]
        public static bool operator >(BigInteger x, BigInteger y) {
            return (Compare(x, y) > 0);
        }

        [Pure]
        public static bool operator >=(BigInteger x, BigInteger y) {
            return (Compare(x, y) >= 0);
        }

        public static BigInteger Add(BigInteger x, BigInteger y) {
            return (x + y);
        }

        public static BigInteger operator +(BigInteger x, BigInteger y) {
            if (x._sign == y._sign) {
                return new BigInteger(x._sign, add0(x._data, x.Length, y._data, y.Length));
            }
            return (x - (-y) /* op_UnaryNegation(y) */);
        }

        public static BigInteger Subtract(BigInteger x, BigInteger y) {
            return (x - y);
        }

        public static BigInteger operator -(BigInteger x, BigInteger y) {
            uint[] numArray;
            int sign = Compare(x, y);
            if (sign != 0) {
                if (x._sign != y._sign) {
                    return new BigInteger(sign, add0(x._data, x.Length, y._data, y.Length));
                }
                switch ((sign * x._sign)) {
                    case -1:
                        numArray = sub(y._data, y.Length, x._data, x.Length);
                        goto Label_008F;

                    case 1:
                        numArray = sub(x._data, x.Length, y._data, y.Length);
                        goto Label_008F;
                }
            }
            return Zero;
        Label_008F:
            return new BigInteger(sign, numArray);
        }

        public static BigInteger Multiply(BigInteger x, BigInteger y) {
            int xLength = x.Length;
            int yLength = y.Length;
            if ((((xLength + yLength) >= UpperBoundForSchoolBookMultiplicationDigits /* 0x40 */)
              && (xLength >= ForceSchoolBookMultiplicationThresholdDigits /* 8 */))
              && (yLength >= ForceSchoolBookMultiplicationThresholdDigits /* 8 */)) {
                return MultiplyKaratsuba(x, y);
            }
            return MultiplySchoolBook(x, y);
        }

        [Pure]
        public static BigInteger operator *(BigInteger x, BigInteger y) {
            return Multiply(x, y);
        }

        private static BigInteger MultiplySchoolBook(BigInteger x, BigInteger y) {
            int xLength = x.Length;
            int yLength = y.Length;
            int length = xLength + yLength;
            uint[] xData = x._data;
            uint[] yData = y._data;
            uint[] data = new uint[length];
            for (int i = 0; i < xLength; i++) {
                ulong xdi = xData[i];
                ulong di = 0L;
                int index = i;
                for (int j = 0; j < yLength; j++) {
                    di += xdi * yData[j] + data[index];
                    data[index++] = (uint)di;
                    di = di >> BitsPerDigit /* 0x20 */;
                }
                while (di != 0L) {
                    di += data[index];
                    data[index++] = (uint)di;
                    di = di >> BitsPerDigit /* 0x20 */;
                }
            }
            return new BigInteger(x._sign * y._sign, data);
        }

        private static BigInteger MultiplyKaratsuba(BigInteger x, BigInteger y) {
            int length = Math.Max(x.Length, y.Length) / 2;
            if (length <= 2 * ForceSchoolBookMultiplicationThresholdDigits /* 0x10 */
              || x.Length < 2 * ForceSchoolBookMultiplicationThresholdDigits /* 0x10 */
              || y.Length < 2 * ForceSchoolBookMultiplicationThresholdDigits /* 0x10 */)
                return MultiplySchoolBook(x, y);
            int shift = BitsPerDigit /* 0x20 */ * length;
            BigInteger i1 = RightShift(x, shift);
            BigInteger i2 = x.RestrictTo(length);
            BigInteger i3 = RightShift(y, shift);
            BigInteger i4 = y.RestrictTo(length);
            BigInteger i5 = Multiply(i1, i3);
            BigInteger i6 = Multiply(i2, i4);
            BigInteger i7 = Multiply(i1 + i2, i3 + i4) - (i5 + i6);
            return (i6 + LeftShift(i7 + LeftShift(i5, shift), shift));
        }

        private BigInteger RestrictTo(int numDigits) {
            Contract.Requires(numDigits > 0);
            int num = Math.Min(numDigits, this.Length);
            if (num == this.Length) {
                return this;
            }
            BigInteger integer = new BigInteger(this._sign, this._data);
            integer._length = num;
            return integer;
        }

        public static BigInteger Divide(BigInteger dividend, BigInteger divisor) {
            return (dividend / divisor);
        }

        public static BigInteger operator /(BigInteger dividend, BigInteger divisor) {
            BigInteger integer;
            return DivRem(dividend, divisor, out integer);
        }

        private static int GetNormalizeShift(uint value) {
            int num = 0;
            if ((value & 0xffff0000) == 0) {
                value = value << 0x10;
                num += 0x10;
            }
            if ((value & 0xff000000) == 0) {
                value = value << 8;
                num += 8;
            }
            if ((value & 0xf0000000) == 0) {
                value = value << 4;
                num += 4;
            }
            if ((value & 0xc0000000) == 0) {
                value = value << 2;
                num += 2;
            }
            if ((value & 0x80000000) == 0) {
                value = value << 1;
                num++;
            }
            return num;
        }

        [Conditional("DEBUG")]
        private static void TestNormalize(uint[] u, uint[] un, int shift) {
            new BigInteger(1, u);
            BigInteger x = new BigInteger(1, un);
            RightShift(x, shift);
        }

        [Conditional("DEBUG")]
        private static void TestDivisionStep(uint[] un, uint[] vn, uint[] q, uint[] u, uint[] v) {
            int length = GetLength(v);
            int normalizeShift = GetNormalizeShift(v[length - 1]);
            BigInteger integer = new BigInteger(1, un);
            BigInteger integer2 = new BigInteger(1, vn);
            BigInteger integer3 = new BigInteger(1, q);
            BigInteger x = new BigInteger(1, u);
            BigInteger integer1 = (integer2 * integer3) + integer;
            LeftShift(x, normalizeShift);
        }

        [Conditional("DEBUG")]
        private static void TestResult(uint[] u, uint[] v, uint[] q, uint[] r) {
            new BigInteger(1, u);
            BigInteger integer = new BigInteger(1, v);
            BigInteger integer2 = new BigInteger(1, q);
            BigInteger integer3 = new BigInteger(1, r);
            BigInteger integer4 = integer * integer2;
            BigInteger integer1 = integer4 + integer3;
        }

        private static void Normalize(uint[] u, int len, uint[] un, int shift) {
            int num2;
            uint num = 0;
            if (shift > 0) {
                int num3 = BitsPerDigit /* 0x20 */ - shift;
                for (num2 = 0; num2 < len; num2++) {
                    uint num4 = u[num2];
                    un[num2] = (num4 << shift) | num;
                    num = num4 >> num3;
                }
            } else {
                num2 = 0;
                while (num2 < len) {
                    un[num2] = u[num2];
                    num2++;
                }
            }
            while (num2 < un.Length) {
                un[num2++] = 0;
            }
            if (num != 0) {
                un[len] = num;
            }
        }

        private static void Unnormalize(uint[] un, out uint[] r, int shift) {
            int length = GetLength(un);
            r = new uint[length];
            if (shift > 0) {
                int num2 = BitsPerDigit /* 0x20 */ - shift;
                uint num3 = 0;
                for (int i = length - 1; i >= 0; i--) {
                    uint num5 = un[i];
                    r[i] = (num5 >> shift) | num3;
                    num3 = num5 << num2;
                }
            } else {
                for (int j = 0; j < length; j++) {
                    r[j] = un[j];
                }
            }
        }

        private static void DivModUnsigned(uint[] u, uint[] v, out uint[] q, out uint[] r) {
            int uLength = GetLength(u);
            int vLength = GetLength(v);
            if (vLength <= 1) {
                if (vLength == 0) {
                    throw new DivideByZeroException();
                }
                ulong num3 = 0L;
                uint num4 = v[0];
                q = new uint[uLength];
                r = new uint[1];
                for (int i = uLength - 1; i >= 0; i--) {
                    num3 *= Base /* 0x100000000L */;
                    num3 += u[i];
                    ulong num6 = num3 / num4;
                    num3 -= num6 * num4;
                    q[i] = (uint)num6;
                }
                r[0] = (uint)num3;
            } else if (uLength >= vLength) {
                int normalizeShift = GetNormalizeShift(v[vLength - 1]);
                uint[] un = new uint[uLength + 1];
                uint[] numArray2 = new uint[vLength];
                Normalize(u, uLength, un, normalizeShift);
                Normalize(v, vLength, numArray2, normalizeShift);
                q = new uint[(uLength - vLength) + 1];
                r = null;
                for (int j = uLength - vLength; j >= 0; j--) {
                    ulong num9 = Base /* 0x100000000L */ * un[j + vLength] + un[(j + vLength) - 1];
                    ulong num10 = num9 / numArray2[vLength - 1];
                    num9 -= num10 * numArray2[vLength - 1];
                    do {
                        if (num10 < Base && num10 * numArray2[vLength - 2] <= num9 * Base + un[(j + vLength) - 2]) {
                            break;
                        }
                        num10--;
                        num9 += numArray2[vLength - 1];
                    }
                    while (num9 < Base /* 0x100000000L */);
                    long num12 = 0L;
                    long num13 = 0L;
                    int index = 0;
                    while (index < vLength) {
                        ulong num14 = numArray2[index] * num10;
                        num13 = un[index + j] - (uint)num14 - num12;
                        un[index + j] = (uint)num13;
                        num14 = num14 >> BitsPerDigit /* 0x20 */;
                        num13 = num13 >> BitsPerDigit /* 0x20 */;
                        num12 = (long)num14 - num13;
                        index++;
                    }
                    num13 = un[j + vLength] - num12;
                    un[j + vLength] = (uint)num13;
                    q[j] = (uint)num10;
                    if (num13 < 0L) {
                        q[j]--;
                        ulong num15 = 0L;
                        for (index = 0; index < vLength; index++) {
                            num15 = (ulong)numArray2[index] + un[j + index] + num15;
                            un[j + index] = (uint)num15;
                            num15 = num15 >> BitsPerDigit /* 0x20 */;
                        }
                        num15 += un[j + vLength];
                        un[j + vLength] = (uint)num15;
                    }
                }
                Unnormalize(un, out r, normalizeShift);
            } else {
                q = zeroArray;
                r = u;
            }
        }

        public static BigInteger DivRem(BigInteger dividend, BigInteger divisor, out BigInteger remainder) {
            uint[] numArray;
            uint[] numArray2;
            DivModUnsigned((dividend._data == null) ? zeroArray : dividend._data,
              (divisor._data == null) ? zeroArray : divisor._data, out numArray, out numArray2);
            remainder = new BigInteger(dividend._sign, numArray2);
            return new BigInteger(dividend._sign * divisor._sign, numArray);
        }

        private static BigInteger LeftShift(BigInteger x, int shift) {
            if (shift == 0) {
                return x;
            }
            if (shift < 0) {
                return RightShift(x, -shift);
            }
            int num = shift / BitsPerDigit /* 0x20 */;
            int num2 = shift - num * BitsPerDigit /* 0x20 */;
            int length = x.Length;
            uint[] numArray = x._data;
            int num4 = length + num + 1;
            uint[] numArray2 = new uint[num4];
            if (num2 == 0) {
                for (int i = 0; i < length; i++) {
                    numArray2[i + num] = numArray[i];
                }
            } else {
                int num6 = BitsPerDigit /* 0x20 */ - num2;
                uint num7 = 0;
                int index = 0;
                while (index < length) {
                    uint num9 = numArray[index];
                    numArray2[index + num] = (num9 << num2) | num7;
                    num7 = num9 >> num6;
                    index++;
                }
                numArray2[index + num] = num7;
            }
            return new BigInteger(x._sign, numArray2);
        }

        private static BigInteger RightShift(BigInteger x, int shift) {
            if (shift == 0) {
                return x;
            }
            if (shift < 0) {
                return LeftShift(x, -shift);
            }
            int num = shift / BitsPerDigit /* 0x20 */;
            int num2 = shift - num * BitsPerDigit /* 0x20 */;
            int length = x.Length;
            uint[] numArray = x._data;
            int num4 = length - num;
            if (num4 < 0) {
                num4 = 0;
            }
            uint[] numArray2 = new uint[num4];
            if (num2 == 0) {
                for (int i = length - 1; i >= num; i--) {
                    numArray2[i - num] = numArray[i];
                }
            } else {
                int num6 = BitsPerDigit /* 0x20 */ - num2;
                uint num7 = 0;
                for (int j = length - 1; j >= num; j--) {
                    uint num9 = numArray[j];
                    numArray2[j - num] = (num9 >> num2) | num7;
                    num7 = num9 << num6;
                }
            }
            return new BigInteger(x._sign, numArray2);
        }

        public static BigInteger Parse(string s) {
            return Parse(s, CultureInfo.CurrentCulture);
        }

        public static BigInteger Parse(string s, IFormatProvider provider) {
            return Parse(s, NumberStyles.Integer, provider);
        }

        public static BigInteger Parse(string s, NumberStyles style) {
            return Parse(s, style, CultureInfo.CurrentCulture);
        }

        public static BigInteger Parse(string s, NumberStyles style, IFormatProvider provider) {
            BigInteger integer;
            string str;
            if (!TryParse(s, style, provider, out integer, out str)) {
                throw new FormatException(str);
            }
            return integer;
        }

        public static bool TryParse(string s, out BigInteger b) {
            string str;
            return TryParse(s, NumberStyles.Integer, CultureInfo.CurrentCulture, out b, out str);
        }

        public static bool TryParse(string s, NumberStyles style, IFormatProvider formatProvider, out BigInteger value) {
            string str;
            return TryParse(s, style, formatProvider, out value, out str);
        }

        private static bool TryParse(string s, NumberStyles style, IFormatProvider formatProvider, out BigInteger value, out string error) {
            Contract.Requires((s != null) ? null : ((Exception)new ArgumentNullException("s")));
            if (formatProvider == null) {
                formatProvider = CultureInfo.CurrentCulture;
            }
            if ((style & ~(NumberStyles.HexNumber | NumberStyles.AllowLeadingSign)) != NumberStyles.None) {
                throw new NotSupportedException(string.Format(CultureInfo.CurrentUICulture, Res.UnsupportedNumberStyle, new object[] { style }));
            }
            error = null;
            NumberFormatInfo format = (NumberFormatInfo)formatProvider.GetFormat(typeof(NumberFormatInfo));
            uint num = ((style & NumberStyles.AllowHexSpecifier) != NumberStyles.None) ? 0x10 : (uint)10;
            int indexA = 0;
            bool flag = false;
            if ((style & NumberStyles.AllowLeadingWhite) != NumberStyles.None) {
                while ((indexA < s.Length) && IsWhiteSpace(s[indexA])) {
                    indexA++;
                }
            }
            if ((style & NumberStyles.AllowLeadingSign) != NumberStyles.None) {
                int length = format.NegativeSign.Length;
                if (((length + indexA) < s.Length) && (string.Compare(s, indexA, format.NegativeSign, 0, length, false, CultureInfo.CurrentCulture) == 0)) {
                    flag = true;
                    indexA += format.NegativeSign.Length;
                }
            }
            value = Zero;
            BigInteger one = One;
            if (indexA == s.Length) {
                error = Res.ParsedStringWasInvalid;
                return false;
            }
            for (int i = s.Length - 1; i >= indexA; i--) {
                if (((style & NumberStyles.AllowTrailingWhite) != NumberStyles.None) && IsWhiteSpace(s[i])) {
                    int num5 = i;
                    while (num5 >= indexA) {
                        if (!IsWhiteSpace(s[num5])) {
                            break;
                        }
                        num5--;
                    }
                    if (num5 < indexA) {
                        error = Res.ParsedStringWasInvalid;
                        return false;
                    }
                    i = num5;
                }
                uint num6 = ParseSingleDigit(s[i], (ulong)num, out error);
                if (error != null) {
                    return false;
                }
                if (num6 != 0) {
                    value += num6 * one;
                }
                one *= num;
            }
            if ((value._sign == 1) && flag) {
                value = -value; // op_UnaryNegation(value);
            }
            return true;
        }

        private static uint ParseSingleDigit(char c, ulong radix, out string error) {
            error = null;
            if ((c >= '0') && (c <= '9')) {
                return (uint)(c - '0');
            }
            if (radix == 0x10L) {
                c = (char)(c & '￟');
                if ((c >= 'A') && (c <= 'F')) {
                    return (uint)((c - 'A') + 10);
                }
            }
            error = Res.InvalidCharactersInString;
            return uint.MaxValue;
        }

        private static bool IsWhiteSpace(char ch) {
            return ((ch == ' ') || ((ch >= '\t') && (ch <= '\r')));
        }

        public string ToString(string format) {
            return this.ToString(format, CultureInfo.CurrentCulture);
        }

        public string ToString(IFormatProvider formatProvider) {
            if (formatProvider == null) {
                formatProvider = CultureInfo.CurrentCulture;
            }
            return this.ToString(10, false, (NumberFormatInfo)formatProvider.GetFormat(typeof(NumberFormatInfo)));
        }

        public string ToString(string format, IFormatProvider formatProvider) {
            if (formatProvider == null) {
                formatProvider = CultureInfo.CurrentCulture;
            }
            uint radix = 10;
            bool useCapitalHexDigits = false;
            if (!string.IsNullOrEmpty(format)) {
                char ch = format[0];
                switch (ch) {
                    case 'X':
                    case 'x':
                        radix = 0x10;
                        useCapitalHexDigits = ch == 'X';
                        goto Label_0069;
                }
                if (((ch != 'G') && (ch != 'g')) && ((ch != 'D') && (ch != 'd'))) {
                    throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "Currently not supported format: {0}", new object[] { format }));
                }
            }
        Label_0069:
            return this.ToString(radix, useCapitalHexDigits, (NumberFormatInfo)formatProvider.GetFormat(typeof(NumberFormatInfo)));
        }

        public override string ToString() {
            return this.ToString(10, false, CultureInfo.CurrentCulture.NumberFormat);
        }

        private string ToString(uint radix, bool useCapitalHexDigits, NumberFormatInfo info) {
            Contract.Requires((radix >= 2) && (radix <= 0x24));
            if (this._sign == 0) {
                return "0";
            }
            int length = this.Length;
            List<uint> list = new List<uint>();
            uint[] n = copy(this._data);
            int nl = this.Length;
            uint d = groupRadixValues[radix];
            while (nl > 0) {
                uint item = div(n, ref nl, d);
                list.Add(item);
            }
            StringBuilder buf = new StringBuilder();
            if (this._sign == -1) {
                buf.Append(info.NegativeSign);
            }
            int num4 = list.Count - 1;
            char[] tmp = new char[maxCharsPerDigit[radix]];
            AppendRadix(list[num4--], radix, useCapitalHexDigits, tmp, buf, false);
            while (num4 >= 0) {
                AppendRadix(list[num4--], radix, useCapitalHexDigits, tmp, buf, true);
            }
            return buf.ToString();
        }

        private static void AppendRadix(uint rem, uint radix, bool useCapitalHexDigits, char[] tmp, StringBuilder buf, bool leadingZeros) {
            string str = useCapitalHexDigits ? "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ" : "0123456789abcdefghijklmnopqrstuvwxyz";
            int length = tmp.Length;
            int startIndex = length;
            while ((startIndex > 0) && (leadingZeros || (rem != 0))) {
                uint num3 = rem % radix;
                rem /= radix;
                tmp[--startIndex] = str[(int)num3];
            }
            if (leadingZeros) {
                buf.Append(tmp);
            } else {
                buf.Append(tmp, startIndex, length - startIndex);
            }
        }

        private static uint div(uint[] n, ref int nl, uint d) {
            ulong num = 0L;
            int index = nl;
            bool flag = false;
            while (--index >= 0) {
                num = num << BitsPerDigit /* 0x20 */;
                num |= n[index];
                uint num3 = (uint)(num / d);
                n[index] = num3;
                if (num3 == 0) {
                    if (!flag) {
                        nl--;
                    }
                } else {
                    flag = true;
                }
                num = num % d;
            }
            return (uint)num;
        }

        public override int GetHashCode() {
            if (this._sign == 0) {
                return 0;
            }
            return (int)this._data[0];
        }

        public bool Equals(BigInteger other) {
            if (this._sign != other._sign) {
                return false;
            }
            int xLength = this.Length;
            int yLength = other.Length;
            if (xLength != yLength) {
                return false;
            }
            for (uint i = 0; i < xLength; i++) {
                if (this._data[i] != other._data[i]) {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj) {
            return obj != null && obj is BigInteger && this.Equals((BigInteger)obj);
        }

        public int CompareTo(BigInteger other) {
            return Compare(this, other);
        }

        public int CompareTo(object obj) {
            if (obj == null) {
                return 1;
            }
            if (!(obj is BigInteger)) {
                throw new ArgumentException(Res.MustBeBigInt);
            }
            return Compare(this, (BigInteger)obj);
        }

        static BigInteger() {
            maxCharsPerDigit = new uint[] 
      { 
        0, 0, 31, 20, 15, 13, 12, 11, 10, 10, 9, 9, 8, 8, 8, 8, 7, 7, 7, 7, 7, 7, 7, 7, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6
      };
            groupRadixValues = new uint[] 
      { 
        0, 0, 0x80000000, 0xcfd41b91, 0x40000000, 0x48c27395, 0x81bf1000, 0x75db9c97, 0x40000000, 0xcfd41b91, 0x3b9aca00,
        0x8c8b6d2b, 0x19a10000, 0x309f1021, 0x57f6c100, 0x98c29b81, 0x10000000, 0x18754571, 0x247dbc80, 0x3547667b, 0x4c4b4000,
        0x6b5a6e1d, 0x94ace180, 0xcaf18367, 0xb640000, 0xe8d4a51, 0x1269ae40, 0x17179149, 0x1cb91000, 0x23744899, 0x2b73a840,
        0x34e63b41, 0x40000000, 0x4cfa3cc1, 0x5c13d840, 0x6d91b519, 0x81bf1000
      };
            zeroArray = new uint[0];
        }
    }
}