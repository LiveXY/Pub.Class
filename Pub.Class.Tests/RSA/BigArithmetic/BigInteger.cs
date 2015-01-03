using System;
using System.Text;

namespace Skyiv.Numeric {
    public sealed class BigInteger : IEquatable<BigInteger>, IComparable<BigInteger> {
        static readonly byte Len = 2;
        static readonly byte Base = (byte)Math.Pow(10, Len);

        sbyte sign;  // 符号，取值：-1, 0, 1。
        byte[] data; // 字节数组以 100 为基，字节数组中第一个元素存储的数字是最高有效位。

        BigInteger() {
        }

        BigInteger(long x) {
            sign = (sbyte)((x == 0) ? 0 : ((x > 0) ? 1 : -1));
            data = new byte[10]; // long.MinValue = -9,223,372,036,854,775,808
            ulong z = (x < 0) ? (ulong)-x : (ulong)x;
            for (int i = data.Length - 1; z != 0; i--, z /= Base) data[i] = (byte)(z % Base);
            Shrink();
        }

        BigInteger(BigInteger x) {
            sign = x.sign; // x != null
            data = new byte[x.data.Length];
            Array.Copy(x.data, data, data.Length);
        }

        public static implicit operator BigInteger(long x) {
            return new BigInteger(x);
        }

        public static BigInteger Parse(string s) {
            if (s == null) return null;
            if (s.Length == 0) return 0;
            BigInteger z = new BigInteger();
            z.sign = (sbyte)((s[0] == '-') ? -1 : 1);
            if (s[0] == '-' || s[0] == '+') s = s.Substring(1);
            int r = s.Length % Len;
            z.data = new byte[s.Length / Len + ((r != 0) ? 1 : 0)];
            int i = 0;
            if (r != 0) z.data[i++] = byte.Parse(s.Substring(0, r));
            for (; i < z.data.Length; i++, r += Len) z.data[i] = (byte)((s[r] - '0') * 10 + (s[r + 1] - '0'));
            z.Shrink();
            return z;
        }

        public static BigInteger Abs(BigInteger x) {
            if (x == null) return null;
            BigInteger z = new BigInteger(x);
            z.sign = Math.Abs(x.sign);
            return z;
        }

        public static BigInteger Pow(BigInteger x, long y) {
            if (x == null) return null;
            BigInteger z = 1, n = x;
            for (; y > 0; y >>= 1, n *= n) if ((y & 1) != 0) z *= n;
            return z;
        }

        public static BigInteger operator +(BigInteger x) {
            if (x == null) return null;
            return new BigInteger(x);
        }

        public static BigInteger operator -(BigInteger x) {
            if (x == null) return null;
            BigInteger z = new BigInteger(x);
            z.sign = (sbyte)-x.sign;
            return z;
        }

        public static BigInteger operator ++(BigInteger x) {
            return x + 1;
        }

        public static BigInteger operator --(BigInteger x) {
            return x - 1;
        }

        public static BigInteger operator +(BigInteger x, BigInteger y) {
            if (x == null || y == null) return null;
            if (x.AbsCompareTo(y) < 0) Utility.Swap(ref x, ref y);
            BigInteger z = new BigInteger();
            z.sign = x.sign;
            byte[] bs = Utility.Expand(y.data, x.data.Length);
            bool isAdd = x.sign * y.sign == 1;
            z.data = new byte[x.data.Length + (isAdd ? 1 : 0)];
            if (isAdd) BigArithmetic.Add(z.data, x.data, bs, bs.Length);
            else BigArithmetic.Subtract(z.data, x.data, bs, bs.Length);
            z.Shrink();
            return z;
        }

        public static BigInteger operator -(BigInteger x, BigInteger y) {
            if (x == null || y == null) return null;
            return x + (-y);
        }

        public static BigInteger operator *(BigInteger x, BigInteger y) {
            if (x == null || y == null) return null;
            if (x.sign * y.sign == 0) return 0;
            BigInteger z = new BigInteger();
            z.sign = (sbyte)(x.sign * y.sign);
            z.data = new byte[x.data.Length + y.data.Length];
            BigArithmetic.Multiply(z.data, x.data, x.data.Length, y.data, y.data.Length);
            z.Shrink();
            return z;
        }

        public static BigInteger operator /(BigInteger dividend, BigInteger divisor) {
            BigInteger remainder;
            return DivRem(dividend, divisor, out remainder);
        }

        public static BigInteger operator %(BigInteger dividend, BigInteger divisor) {
            BigInteger remainder;
            DivRem(dividend, divisor, out remainder);
            return remainder;
        }

        public static BigInteger DivRem(BigInteger dividend, BigInteger divisor, out BigInteger remainder) {
            remainder = null;
            if (dividend == null || divisor == null) return null;
            if (divisor.sign == 0) throw new DivideByZeroException();
            if (dividend.AbsCompareTo(divisor) < 0) {
                remainder = new BigInteger(dividend);
                return 0;
            }
            BigInteger quotient = new BigInteger();
            remainder = new BigInteger();
            quotient.data = new byte[dividend.data.Length - divisor.data.Length + 1];
            remainder.data = new byte[divisor.data.Length];
            BigArithmetic.DivRem(quotient.data, remainder.data, dividend.data,
              dividend.data.Length, divisor.data, divisor.data.Length);
            quotient.sign = (sbyte)(dividend.sign * divisor.sign);
            remainder.sign = dividend.sign;
            quotient.Shrink();
            remainder.Shrink();
            return quotient;
        }

        public static BigInteger Sqrt(BigInteger x) {
            if (x == null || x.sign < 0) return null;
            if (x.sign == 0) return 0;
            if (x.data.Length == 1) return new BigInteger((long)Math.Sqrt(x.data[0]));
            BigInteger z = new BigInteger();
            z.sign = 1;
            z.data = new byte[x.data.Length / 2 + 3];
            z.data = Adjust(BigArithmetic.Sqrt(z.data, z.data, z.data.Length, x.data, x.data.Length), x.data.Length);
            z.Shrink();
            BigInteger z1 = z + 1; // 平方根有可能比实际小 1。
            return (z1 * z1 <= x) ? z1 : z;
        }

        static byte[] Adjust(byte[] bs, int digits) {
            if (bs[0] >= 10) throw new OverflowException("sqrt adjust");
            byte[] nbs = new byte[(digits + 1) / 2];
            if (digits % 2 == 0)
                for (int k = bs[0], i = 0; i < nbs.Length; i++, k = bs[i] % 10)
                    nbs[i] = (byte)(k * 10 + bs[i + 1] / 10);
            else Array.Copy(bs, nbs, nbs.Length);
            return nbs;
        }

        void Shrink() {
            int i;
            for (i = 0; i < data.Length; i++) if (data[i] != 0) break;
            if (i != 0) {
                byte[] bs = new byte[data.Length - i];
                Array.Copy(data, i, bs, 0, bs.Length);
                data = bs;
            }
            if (data.Length == 0) sign = 0;
        }

        public static bool operator ==(BigInteger x, BigInteger y) {
            if (object.ReferenceEquals(x, null)) return object.ReferenceEquals(y, null);
            return x.Equals(y);
        }

        public static bool operator !=(BigInteger x, BigInteger y) {
            if (object.ReferenceEquals(x, null)) return !object.ReferenceEquals(y, null);
            return !x.Equals(y);
        }

        public static bool operator <(BigInteger x, BigInteger y) {
            if (object.ReferenceEquals(x, null)) return !object.ReferenceEquals(y, null);
            return x.CompareTo(y) < 0;
        }

        public static bool operator >(BigInteger x, BigInteger y) {
            if (object.ReferenceEquals(x, null)) return false;
            return x.CompareTo(y) > 0;
        }

        public static bool operator <=(BigInteger x, BigInteger y) {
            if (object.ReferenceEquals(x, null)) return true;
            return x.CompareTo(y) <= 0;
        }

        public static bool operator >=(BigInteger x, BigInteger y) {
            if (object.ReferenceEquals(x, null)) return object.ReferenceEquals(y, null);
            return x.CompareTo(y) >= 0;
        }

        public override string ToString() {
            if (data.Length == 0) return "0";
            var sb = new StringBuilder(Length, Length);
            sb.Length = Length;
            var k = 0;
            if (sign < 0) sb[k++] = '-';
            if (data[0] >= 10) sb[k++] = (char)(data[0] / 10 + '0');
            sb[k++] = (char)(data[0] % 10 + '0');
            for (var i = 1; i < data.Length; i++) {
                sb[k++] = (char)(data[i] / 10 + '0');
                sb[k++] = (char)(data[i] % 10 + '0');
            }
            return sb.ToString();
        }

        public int Length {
            get { return (data.Length == 0) ? 1 : (((sign < 0) ? 1 : 0) + ((data[0] < 10) ? -1 : 0) + data.Length * Len); }
        }

        public override int GetHashCode() {
            int n = sign, i;
            for (i = data.Length - 1; i >= 4; i -= 4)
                n = (data[i] | (data[i - 1] << 8) | (data[i - 2] << 16) | (data[i - 3] << 24)) ^ (n + (n << 5) + (n >> 27));
            if (i >= 0) {
                int m = data[i];
                if (i > 0) m |= (data[i - 1] << 8);
                if (i > 1) m |= (data[i - 2] << 16);
                if (i > 2) m |= (data[i - 3] << 24);
                n = m ^ (n + (n << 5) + (n >> 27));
            }
            return n * 0x5d588b65;
        }

        public override bool Equals(object other) {
            if (other == null || GetType() != other.GetType()) return false;
            return Equals(other as BigInteger);
        }

        public bool Equals(BigInteger other) {
            return CompareTo(other) == 0;
        }

        public int CompareTo(BigInteger other) {
            if (object.ReferenceEquals(other, null)) return 1;
            if (sign < other.sign) return -1;
            if (sign > other.sign) return 1;
            if (sign == 0) return 0;
            return sign * AbsCompareTo(other);
        }

        int AbsCompareTo(BigInteger other) {
            if (data.Length < other.data.Length) return -1;
            if (data.Length > other.data.Length) return 1;
            return BigArithmetic.Compare(data, other.data, data.Length);
        }
    }
}