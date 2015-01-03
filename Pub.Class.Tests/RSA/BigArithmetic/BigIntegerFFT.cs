using System;
using System.Text;
using System.Collections.Generic;

namespace Skyiv.Numeric.BigIntegerWithoutFFT {
    sealed class BigInteger : IEquatable<BigInteger>, IComparable<BigInteger> {
        static readonly int Len = 9; // int.MaxValue = 2,147,483,647
        static readonly int Base = (int)Math.Pow(10, Len);

        int sign;
        List<int> data;

        BigInteger(long x) {
            sign = (x == 0) ? 0 : ((x > 0) ? 1 : -1);
            data = new List<int>();
            for (ulong z = (x < 0) ? (ulong)-x : (ulong)x; z != 0; z /= (ulong)Base) data.Add((int)(z % (ulong)Base));
        }

        BigInteger(BigInteger x) {
            sign = x.sign;  // x != null
            data = new List<int>(x.data);
        }

        public static implicit operator BigInteger(long x) {
            return new BigInteger(x);
        }

        public static BigInteger Parse(string s) {
            if (s == null) return null;
            s = s.Trim().Replace(",", null);
            if (s.Length == 0) return 0;
            BigInteger z = 0;
            z.sign = (s[0] == '-') ? -1 : 1;
            if (s[0] == '-' || s[0] == '+') s = s.Substring(1);
            int r = s.Length % Len;
            z.data = new List<int>(new int[s.Length / Len + ((r != 0) ? 1 : 0)]);
            int i = z.data.Count - 1;
            if (r != 0) z.data[i--] = int.Parse(s.Substring(0, r));
            for (; i >= 0; i--, r += Len) z.data[i] = int.Parse(s.Substring(r, Len));
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
            z.sign = -x.sign;
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
            BigInteger z = new BigInteger(x);
            if (x.sign * y.sign == -1) z.AbsSubtract(y);
            else z.AbsAdd(y);
            return z;
        }

        public static BigInteger operator -(BigInteger x, BigInteger y) {
            if (x == null || y == null) return null;
            return x + (-y);
        }

        public static BigInteger operator *(BigInteger x, BigInteger y) {
            if (x == null || y == null) return null;
            BigInteger z = 0;
            z.sign = x.sign * y.sign;
            z.data = new List<int>(new int[x.data.Count + y.data.Count]);
            for (int i = x.data.Count - 1; i >= 0; i--)
                for (int j = y.data.Count - 1; j >= 0; j--) {
                    long n = Math.BigMul(x.data[i], y.data[j]);
                    z.data[i + j] += (int)(n % Base);
                    z.CarryUp(i + j);
                    z.data[i + j + 1] += (int)(n / Base);
                    z.CarryUp(i + j + 1);
                }
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
            remainder = 0;
            BigInteger quotient = 0;
            quotient.sign = dividend.sign * divisor.sign;
            quotient.data = new List<int>(new int[dividend.data.Count]);
            divisor = Abs(divisor); // NOT: divisor.sign = Math.Abs(divisor.sign);
            for (int i = dividend.data.Count - 1; i >= 0; i--) {
                remainder = remainder * Base + dividend.data[i];
                int iQuotient = remainder.BinarySearch(divisor, -1);
                quotient.data[i] = iQuotient;
                remainder -= divisor * iQuotient;
            }
            quotient.Shrink();
            if (remainder.sign != 0) remainder.sign = dividend.sign;
            return quotient;
        }

        public static BigInteger Sqrt(BigInteger x) {
            if (x == null || x.sign < 0) return null;
            BigInteger root = 0;
            root.sign = 1;
            root.data = new List<int>(new int[x.data.Count / 2 + 1]);
            for (int i = root.data.Count - 1; i >= 0; i--) root.data[i] = x.BinarySearch(root, i);
            root.Shrink();
            return root;
        }

        int BinarySearch(BigInteger divisor, int i) {
            int low = 0, high = Base - 1, mid = 0, cmp = 0;
            while (low <= high) {
                mid = (low + high) / 2;
                cmp = CompareTo(divisor, mid, i);
                if (cmp > 0) low = mid + 1;
                else if (cmp < 0) high = mid - 1;
                else return mid;
            }
            return (cmp < 0) ? (mid - 1) : mid;
        }

        int CompareTo(BigInteger divisor, int mid, int i) {
            if (i >= 0) divisor.data[i] = mid;
            return AbsCompareTo(divisor * ((i >= 0) ? divisor : mid));
        }

        void AbsAdd(BigInteger x) {
            for (int i = 0; i < x.data.Count; i++) {
                data[i] += x.data[i];
                CarryUp(i);
            }
        }

        void AbsSubtract(BigInteger x) {
            for (int i = 0; i < x.data.Count; i++) {
                data[i] -= x.data[i];
                CarryDown(i);
            }
            Shrink();
        }

        void CarryUp(int n) {
            for (; data[n] >= Base; n++) {
                if (n == data.Count - 1) data.Add(data[n] / Base);
                else data[n + 1] += data[n] / Base;
                data[n] %= Base;
            }
        }

        void CarryDown(int n) {
            for (; data[n] < 0; n++) {
                data[n + 1]--;
                data[n] += Base;
            }
            Shrink();
        }

        void Shrink() {
            for (int i = data.Count - 1; i >= 0 && data[i] == 0; i--) data.RemoveAt(i);
            if (data.Count == 0) sign = 0;
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
            StringBuilder sb = new StringBuilder();
            if (sign < 0) sb.Append('-');
            sb.Append((data.Count == 0) ? 0 : data[data.Count - 1]);
            for (int i = data.Count - 2; i >= 0; i--) sb.Append(data[i].ToString("D" + Len));
            return sb.ToString();
        }

        public override int GetHashCode() {
            int hash = sign;
            foreach (int n in data) hash ^= n;
            return hash;
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
            if (data.Count < other.data.Count) return -1;
            if (data.Count > other.data.Count) return 1;
            for (int i = data.Count - 1; i >= 0; i--)
                if (data[i] != other.data[i])
                    return (data[i] < other.data[i]) ? -1 : 1;
            return 0;
        }
    }
}