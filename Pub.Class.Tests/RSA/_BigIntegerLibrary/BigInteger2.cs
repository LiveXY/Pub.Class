using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Security.Permissions;
using System.Text;
using System.Collections.Generic;


namespace BigIntegerLibrary {

    /// <summary>
    /// .NET 2.0 class for handling of very large integers, up to 10240 binary digits or
    /// approximately (safe to use) 3000 decimal digits.
    /// </summary>
    [Serializable, CLSCompliant(true)]
    public sealed class BigInteger : ISerializable, IEquatable<BigInteger>,
                                     IComparable, IComparable<BigInteger> {

        #region Fields

        /// <summary>
        /// 2^16 numeration base for internal computations, in order to benefit the most from the
        /// 32 bit (or 64 bit) integer processor registers.
        /// </summary>
        private const long NumberBase = 65536;

        /// <summary>
        /// Maximum size for numbers is up to 10240 binary digits or approximately (safe to use) 3000 decimal digits.
        /// The maximum size is, in fact, double the previously specified amount, in order to accommodate operations's
        /// overflow.
        /// </summary>
        internal const int MaxSize = 2 * 640;

        /// <summary>
        /// Ratio for the convertion of a BigInteger's size to a binary digits size.
        /// </summary>
        private const int RatioToBinaryDigits = 16;


        /// Integer constants
        private static readonly BigInteger Zero = new BigInteger();
        private static readonly BigInteger One = new BigInteger(1);
        private static readonly BigInteger Two = new BigInteger(2);
        private static readonly BigInteger Ten = new BigInteger(10);


        /// <summary>
        /// The array of digits of the number.
        /// </summary>
        private long[] digits;

        /// <summary>
        /// The actual number of digits of the number.
        /// </summary>
        private int size;

        /// <summary>
        /// The number sign.
        /// </summary>
        private Sign sign;


        #endregion


        #region Constructors


        /// <summary>
        /// Default constructor, intializing the BigInteger with zero.
        /// </summary>
        public BigInteger() {
            digits = new long[MaxSize];
            size = 1;
            digits[size] = 0;
            sign = Sign.Positive;
        }

        /// <summary>
        /// Constructor creating a new BigInteger as a conversion of a regular base-10 long.
        /// </summary>
        /// <param name="n">The base-10 long to be converted</param>
        public BigInteger(long n) {
            digits = new long[MaxSize];
            sign = Sign.Positive;

            if (n == 0) {
                size = 1;
                digits[size] = 0;
            } else {
                if (n < 0) {
                    n = -n;
                    sign = Sign.Negative;
                }

                size = 0;
                while (n > 0) {
                    digits[size] = n % NumberBase;
                    n /= NumberBase;
                    size++;
                }
            }
        }

        /// <summary>
        /// Constructor creating a new BigInteger as a copy of an existing BigInteger.
        /// </summary>
        /// <param name="n">The BigInteger to be copied</param>
        public BigInteger(BigInteger n) {
            digits = new long[MaxSize];
            size = n.size;
            sign = n.sign;

            for (int i = 0; i < n.size; i++)
                digits[i] = n.digits[i];
        }

        /// <summary>
        /// Constructor creating a BigInteger instance out of a base-10 formatted string.
        /// </summary>
        /// <param name="numberString">The base-10 formatted string.</param>
        /// <exception cref="BigIntegerException">Invalid numeric string exception</exception>
        public BigInteger(string numberString) {
            BigInteger number = new BigInteger();
            Sign numberSign = Sign.Positive;
            int i;

            for (i = 0; i < numberString.Length; i++) {
                if ((numberString[i] < '0') || (numberString[i] > '9')) {
                    if ((i == 0) && (numberString[i] == '-'))
                        numberSign = Sign.Negative;
                    else
                        throw new BigIntegerException("Invalid numeric string.", null);
                } else
                    number = number * Ten + long.Parse(numberString[i].ToString());
            }

            sign = numberSign;

            digits = new long[MaxSize];
            size = number.size;
            for (i = 0; i < number.size; i++)
                digits[i] = number.digits[i];
        }

        /// <summary>
        /// Constructor creating a positive BigInteger by extracting it's digits from a given byte array.
        /// </summary>
        /// <param name="byteArray">The byte array</param>
        /// <exception cref="BigIntegerException">The byte array's content exceeds the maximum size of a BigInteger
        /// exception</exception>
        public BigInteger(byte[] byteArray) {
            if (byteArray.Length / 4 > MaxSize)
                throw new BigIntegerException("The byte array's content exceeds the maximum size of a BigInteger.", null);

            digits = new long[MaxSize];
            sign = Sign.Positive;

            for (int i = 0; i < byteArray.Length; i += 2) {
                int currentDigit = (int)byteArray[i];
                if (i + 1 < byteArray.Length) {
                    currentDigit <<= 8;
                    currentDigit += (int)byteArray[i + 1];
                }

                digits[size++] = (long)currentDigit;
            }

            bool reducible = true;
            while ((size - 1 > 0) && (reducible == true)) {
                if (digits[size - 1] == 0)
                    size--;
                else reducible = false;
            }
        }

        /// <summary>
        /// Constructor deserializing a BigInteger.
        /// </summary>
        private BigInteger(SerializationInfo info, StreamingContext context) {
            bool signValue = (bool)info.GetValue("sign", typeof(bool));
            if (signValue == true)
                sign = Sign.Positive;
            else
                sign = Sign.Negative;

            size = (int)info.GetValue("size", typeof(short));
            digits = new long[MaxSize];

            int i;
            for (i = 0; i < size; i++)
                digits[i] = (long)(info.GetValue(string.Format("{0}{1}", "d_", i.ToString()),
                                                 typeof(ushort)));
        }


        #endregion


        #region Public Methods


        /// <summary>
        /// BigInteger serializing method, which should not be called manually.
        /// </summary>
        /// <param name="info">Serialization information object</param>
        /// <param name="context">Streaming context object</param>
        /// <permission cref="System.Security.PermissionSet">Public access</permission>
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            if (sign == Sign.Positive)
                info.AddValue("sign", true);
            else
                info.AddValue("sign", false);

            info.AddValue("size", (short)size);

            for (int i = 0; i < size; i++)
                info.AddValue(string.Format("{0}{1}", "d_", i.ToString()), (ushort)digits[i]);
        }

        /// <summary>
        /// Determines whether the specified BigInteger is equal to the current BigInteger.
        /// </summary>
        /// <param name="other">The BigInteger to compare with the current BigInteger</param>
        /// <returns>True if the specified BigInteger is equal to the current BigInteger,
        /// false otherwise</returns>
        public bool Equals(BigInteger other) {
            if (sign != other.sign)
                return false;
            if (size != other.size)
                return false;

            for (int i = 0; i < size; i++)
                if (digits[i] != other.digits[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current BigInteger.
        /// </summary>
        /// <param name="o">The System.Object to compare with the current BigInteger</param>
        /// <returns>True if the specified System.Object is equal to the current BigInteger,
        /// false otherwise</returns>
        public override bool Equals(object o) {
            if ((o is BigInteger) == false)
                return false;

            return Equals((BigInteger)o);
        }

        /// <summary>
        /// Serves as a hash function for the BigInteger type.
        /// </summary>
        /// <returns>A hash code for the current BigInteger</returns>
        public override int GetHashCode() {
            int result = 0;

            for (int i = 0; i < size; i++)
                result = result + (int)digits[i];

            return result;
        }

        /// <summary>
        /// String representation of the current BigInteger, converted to its base-10 representation.
        /// </summary>
        /// <returns>The string representation of the current BigInteger</returns>
        public override string ToString() {
            Base10BigInteger res = new Base10BigInteger();
            Base10BigInteger currentPower = new Base10BigInteger(1);

            for (int i = 0; i < size; i++) {
                res += digits[i] * currentPower;
                currentPower *= NumberBase;
            }

            res.NumberSign = sign;

            return res.ToString();
        }

        /// <summary>
        ///  Compares this instance to a specified BigInteger.
        /// </summary>
        /// <param name="other">The BigInteger to compare this instance with</param>
        /// <returns>-1 if the current instance is smaller than the given BigInteger,
        /// 0 if the two are equal, 1 otherwise</returns>
        public int CompareTo(BigInteger other) {
            if (Greater(this, other) == true)
                return 1;
            else if (Equals(this, other) == true)
                return 0;
            else
                return -1;
        }

        /// <summary>
        ///  Compares this instance to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare this instance with</param>
        /// <returns>-1 if the current instance is smaller than the given object,
        /// 0 if the two are equal, 1 otherwise</returns>
        /// <exception cref="ArgumentException">obj is not a BigInteger exception</exception>
        public int CompareTo(object obj) {
            if ((obj is BigInteger) == false)
                throw new ArgumentException("obj is not a BigInteger.");

            return CompareTo((BigInteger)obj);
        }

        /// <summary>
        /// Returns a BigInteger's size in binary digits.
        /// </summary>
        /// <param name="n">The BigInteger whose size in binary digits is to be determined</param>
        /// <returns>The BigInteger's size in binary digits</returns>
        public static int SizeInBinaryDigits(BigInteger n) {
            return n.size * RatioToBinaryDigits;
        }

        /// <summary>
        /// BigInteger inverse with respect to addition.
        /// </summary>
        /// <param name="n">The BigInteger whose opposite is to be computed</param>
        /// <returns>The BigInteger inverse with respect to addition</returns>
        public static BigInteger Opposite(BigInteger n) {
            BigInteger res = new BigInteger(n);

            if (res != Zero) {
                if (res.sign == Sign.Positive)
                    res.sign = Sign.Negative;
                else
                    res.sign = Sign.Positive;
            }

            return res;
        }

        /// <summary>
        /// Greater test between two BigIntegers.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>True if a &gt; b, false otherwise</returns>
        public static bool Greater(BigInteger a, BigInteger b) {
            if (a.sign != b.sign) {
                if ((a.sign == Sign.Negative) && (b.sign == Sign.Positive))
                    return false;

                if ((a.sign == Sign.Positive) && (b.sign == Sign.Negative))
                    return true;
            } else {
                if (a.sign == Sign.Positive) {
                    if (a.size > b.size)
                        return true;
                    if (a.size < b.size)
                        return false;
                    for (int i = (a.size) - 1; i >= 0; i--)
                        if (a.digits[i] > b.digits[i])
                            return true;
                        else if (a.digits[i] < b.digits[i])
                            return false;
                } else {
                    if (a.size < b.size)
                        return true;
                    if (a.size > b.size)
                        return false;
                    for (int i = (a.size) - 1; i >= 0; i--)
                        if (a.digits[i] < b.digits[i])
                            return true;
                        else if (a.digits[i] > b.digits[i])
                            return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Greater or equal test between two BigIntegers.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>True if a &gt;= b, false otherwise</returns>
        public static bool GreaterOrEqual(BigInteger a, BigInteger b) {
            return Greater(a, b) || Equals(a, b);
        }

        /// <summary>
        /// Smaller test between two BigIntegers.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>True if a &lt; b, false otherwise</returns>
        public static bool Smaller(BigInteger a, BigInteger b) {
            return !GreaterOrEqual(a, b);
        }

        /// <summary>
        /// Smaller or equal test between two BigIntegers.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>True if a &lt;= b, false otherwise</returns>
        public static bool SmallerOrEqual(BigInteger a, BigInteger b) {
            return !Greater(a, b);
        }

        /// <summary>
        /// Computes the absolute value of a BigInteger.
        /// </summary>
        /// <param name="n">The BigInteger whose absolute value is to be computed</param>
        /// <returns>The absolute value of the given BigInteger</returns>
        public static BigInteger Abs(BigInteger n) {
            BigInteger res = new BigInteger(n);
            res.sign = Sign.Positive;
            return res;
        }

        /// <summary>
        /// Addition operation of two BigIntegers.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>The BigInteger result of the addition</returns>
        public static BigInteger Addition(BigInteger a, BigInteger b) {
            BigInteger res = null;

            if ((a.sign == Sign.Positive) && (b.sign == Sign.Positive)) {
                if (a >= b)
                    res = Add(a, b);
                else
                    res = Add(b, a);

                res.sign = Sign.Positive;
            }

            if ((a.sign == Sign.Negative) && (b.sign == Sign.Negative)) {
                if (a <= b)
                    res = Add(-a, -b);
                else
                    res = Add(-b, -a);

                res.sign = Sign.Negative;
            }

            if ((a.sign == Sign.Positive) && (b.sign == Sign.Negative)) {
                if (a >= (-b)) {
                    res = Subtract(a, -b);
                    res.sign = Sign.Positive;
                } else {
                    res = Subtract(-b, a);
                    res.sign = Sign.Negative;
                }
            }

            if ((a.sign == Sign.Negative) && (b.sign == Sign.Positive)) {
                if ((-a) <= b) {
                    res = Subtract(b, -a);
                    res.sign = Sign.Positive;
                } else {
                    res = Subtract(-a, b);
                    res.sign = Sign.Negative;
                }
            }

            return res;
        }

        /// <summary>
        /// Subtraction operation of two BigIntegers.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>The BigInteger result of the subtraction</returns>
        public static BigInteger Subtraction(BigInteger a, BigInteger b) {
            BigInteger res = null;

            if ((a.sign == Sign.Positive) && (b.sign == Sign.Positive)) {
                if (a >= b) {
                    res = Subtract(a, b);
                    res.sign = Sign.Positive;
                } else {
                    res = Subtract(b, a);
                    res.sign = Sign.Negative;
                }
            }

            if ((a.sign == Sign.Negative) && (b.sign == Sign.Negative)) {
                if (a <= b) {
                    res = Subtract(-a, -b);
                    res.sign = Sign.Negative;
                } else {
                    res = Subtract(-b, -a);
                    res.sign = Sign.Positive;
                }
            }

            if ((a.sign == Sign.Positive) && (b.sign == Sign.Negative)) {
                if (a >= (-b))
                    res = Add(a, -b);
                else
                    res = Add(-b, a);

                res.sign = Sign.Positive;
            }

            if ((a.sign == Sign.Negative) && (b.sign == Sign.Positive)) {
                if ((-a) >= b)
                    res = Add(-a, b);
                else
                    res = Add(b, -a);

                res.sign = Sign.Negative;
            }

            return res;
        }

        /// <summary>
        /// Multiplication operation of two BigIntegers.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>The BigInteger result of the multiplication</returns>
        public static BigInteger Multiplication(BigInteger a, BigInteger b) {
            if ((a == Zero) || (b == Zero))
                return Zero;

            BigInteger res = Multiply(Abs(a), Abs(b));
            if (a.sign == b.sign)
                res.sign = Sign.Positive;
            else
                res.sign = Sign.Negative;

            return res;
        }

        /// <summary>
        /// Division operation of two BigIntegers a and b, b != 0.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>The BigInteger result of the division</returns>
        /// <exception cref="BigIntegerException">Cannot divide by zero exception</exception>
        public static BigInteger Division(BigInteger a, BigInteger b) {
            if (b == Zero)
                throw new BigIntegerException("Cannot divide by zero.", new DivideByZeroException());

            if (a == Zero)
                return Zero;
            if (Abs(a) < Abs(b))
                return Zero;

            BigInteger res;
            if (b.size == 1)
                res = DivideByOneDigitNumber(Abs(a), b.digits[0]);
            else
                res = DivideByBigNumber(Abs(a), Abs(b));

            if (a.sign == b.sign)
                res.sign = Sign.Positive;
            else
                res.sign = Sign.Negative;

            return res;
        }

        /// <summary>
        /// Modulo operation of two BigIntegers a and b, b != 0.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>The BigInteger result of the modulo</returns>
        /// <exception cref="BigIntegerException">Cannot divide by zero exception</exception>
        public static BigInteger Modulo(BigInteger a, BigInteger b) {
            if (b == Zero)
                throw new BigIntegerException("Cannot divide by zero.", new DivideByZeroException());

            BigInteger res;

            if (Abs(a) < Abs(b)) {
                res = new BigInteger(a);
                return res;
            } else
                res = a - ((a / b) * b);

            return res;
        }

        /// <summary>
        /// Returns the power of a BigInteger base to a non-negative exponent by using the
        /// fast exponentiation algorithm (right to left binary exponentiation).
        /// </summary>
        /// <param name="a">The BigInteger base</param>
        /// <param name="exponent">The non-negative exponent</param>
        /// <returns>The power of the BigInteger base to the non-negative exponent</returns>
        /// <exception cref="BigIntegerException">Cannot raise a BigInteger to a negative power exception.</exception>
        public static BigInteger Power(BigInteger a, int exponent) {
            if (exponent < 0)
                throw new BigIntegerException("Cannot raise an BigInteger to a negative power.", null);

            if (a == Zero)
                return new BigInteger();

            BigInteger res = new BigInteger(1);
            if (exponent == 0)
                return res;

            BigInteger factor = new BigInteger(a);

            while (exponent > 0) {
                if (exponent % 2 == 1)
                    res *= factor;

                exponent /= 2;

                if (exponent > 0)
                    factor *= factor;
            }

            return res;
        }

        /// <summary>
        /// Integer square root of the given BigInteger using Newton's numeric method.
        /// </summary>
        /// <param name="n">The BigInteger whose integer square root is to be computed</param>
        /// <returns>The integer square root of the given BigInteger</returns>
        /// <exception cref="BigIntegerException">Cannot compute the integer square root of a negative number exception</exception>
        public static BigInteger IntegerSqrt(BigInteger n) {
            if (n.sign == Sign.Negative)
                throw new BigIntegerException("Cannot compute the integer square root of a negative number.",
                                              null);

            BigInteger oldValue = new BigInteger(0);
            BigInteger newValue = new BigInteger(n);

            while (BigInteger.Abs(newValue - oldValue) >= One) {
                oldValue = newValue;
                newValue = (oldValue + (n / oldValue)) / Two;
            }

            return newValue;
        }

        /// <summary>
        /// Euclidean algorithm for computing the greatest common divisor of two non-negative BigIntegers.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>The greatest common divisor of the two given BigIntegers</returns>
        /// <exception cref="BigIntegerException">Cannot compute the Gcd of negative BigIntegers exception</exception>
        public static BigInteger Gcd(BigInteger a, BigInteger b) {
            if ((a.sign == Sign.Negative) || (b.sign == Sign.Negative))
                throw new BigIntegerException("Cannot compute the Gcd of negative BigIntegers.", null);

            BigInteger r;

            while (b > Zero) {
                r = a % b;
                a = b;
                b = r;
            }

            return a;
        }

        /// <summary>
        /// Extended Euclidian Gcd algorithm, returning the greatest common divisor of two non-negative BigIntegers,
        /// while also providing u and v, where: a*u + b*v = gcd(a,b).
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <param name="u">Output BigInteger parameter, where a*u + b*v = gcd(a,b)</param>
        /// <param name="v">Output BigInteger parameter, where a*u + b*v = gcd(a,b)</param>
        /// <returns>The greatest common divisor of the two given BigIntegers</returns>
        /// <exception cref="BigIntegerException">Cannot compute the Gcd of negative BigIntegers exception</exception>
        public static BigInteger ExtendedEuclidGcd(BigInteger a, BigInteger b,
                                                   out BigInteger u, out BigInteger v) {
            if ((a.sign == Sign.Negative) || (b.sign == Sign.Negative))
                throw new BigIntegerException("Cannot compute the Gcd of negative BigIntegers.", null);

            BigInteger u1 = new BigInteger();
            BigInteger u2 = new BigInteger(1);
            BigInteger v1 = new BigInteger(1);
            BigInteger v2 = new BigInteger();
            BigInteger q, r;

            u = new BigInteger();
            v = new BigInteger();

            while (b > Zero) {
                q = a / b;
                r = a - q * b;
                u = u2 - q * u1;
                v = v2 - q * v1;

                a = new BigInteger(b);
                b = new BigInteger(r);
                u2 = new BigInteger(u1);
                u1 = new BigInteger(u);
                v2 = new BigInteger(v1);
                v1 = new BigInteger(v);
                u = new BigInteger(u2);
                v = new BigInteger(v2);
            }

            return a;
        }

        /// <summary>
        /// Computes the modular inverse of a given BigInteger.
        /// </summary>
        /// <param name="a">The non-zero BigInteger whose inverse is to be computed</param>
        /// <param name="n">The BigInteger modulus, which must be greater than or equal to 2</param>
        /// <returns>The BigInteger equal to a^(-1) mod n</returns>
        /// <exception cref="BigIntegerException">Invalid number or modulus exception</exception>
        public static BigInteger ModularInverse(BigInteger a, BigInteger n) {
            if (n < Two)
                throw new BigIntegerException("Cannot perform a modulo operation against a BigInteger less than 2.", null);

            if (Abs(a) >= n)
                a %= n;
            if (a.sign == Sign.Negative)
                a += n;

            if (a == Zero)
                throw new BigIntegerException("Cannot obtain the modular inverse of 0.", null);

            if (Gcd(a, n) != One)
                throw new BigIntegerException("Cannot obtain the modular inverse of a number that is not coprime with the modulus.", null);

            BigInteger u, v;
            ExtendedEuclidGcd(n, a, out u, out v);
            if (v.sign == Sign.Negative)
                v += n;

            return v;
        }

        /// <summary>
        /// Returns the power of a BigInteger to a non-negative exponent modulo n, by using the
        /// fast exponentiation algorithm (right to left binary exponentiation) and modulo optimizations.
        /// </summary>
        /// <param name="a">The BigInteger base</param>
        /// <param name="exponent">The non-negative exponent</param>
        /// <param name="n">The modulus, which must be greater than or equal to 2</param>
        /// <returns>The power of the BigInteger to the non-negative exponent</returns>
        /// <exception cref="BigIntegerException">Invalid exponent or modulus exception</exception>
        public static BigInteger ModularExponentiation(BigInteger a, BigInteger exponent, BigInteger n) {
            if (exponent < 0)
                throw new BigIntegerException("Cannot raise a BigInteger to a negative power.", null);

            if (n < Two)
                throw new BigIntegerException("Cannot perform a modulo operation against a BigInteger less than 2.", null);

            if (Abs(a) >= n)
                a %= n;
            if (a.sign == Sign.Negative)
                a += n;

            if (a == Zero)
                return new BigInteger();

            BigInteger res = new BigInteger(1);
            BigInteger factor = new BigInteger(a);

            while (exponent > Zero) {
                if (exponent % Two == One)
                    res = (res * factor) % n;
                exponent /= Two;
                factor = (factor * factor) % n;
            }

            return res;
        }

        /// <summary>
        /// Tests whether the number provided is a prime.
        /// </summary>
        /// <param name="number">The number to be tested for primality</param>
        /// <returns>True, if the number given is a prime, false, otherwise</returns>
        /// <exception cref="BigIntegerException">Primality is not defined for negative numbers exception</exception>
        public static bool IsPrime(BigInteger number) {
            if (number.sign == Sign.Negative)
                throw new BigIntegerException("Primality is not defined for negative numbers.", null);

            return PrimalityTester.IsPrime(number);
        }


        #endregion


        #region Overloaded Operators


        /// <summary>
        /// Implicit conversion operator from long to BigInteger.
        /// </summary>
        /// <param name="n">The long to be converted to a BigInteger</param>
        /// <returns>The BigInteger converted from the given long</returns>
        public static implicit operator BigInteger(long n) {
            return new BigInteger(n);
        }

        /// <summary>
        /// Equality test between two BigIntegers.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>True if a == b, false otherwise</returns>
        public static bool operator ==(BigInteger a, BigInteger b) {
            return Equals(a, b);
        }

        /// <summary>
        /// Inequality test between two BigIntegers.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>True if a != b, false otherwise</returns>
        public static bool operator !=(BigInteger a, BigInteger b) {
            return !Equals(a, b);
        }

        /// <summary>
        /// Greater test between two BigIntegers.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>True if a &gt; b, false otherwise</returns>
        public static bool operator >(BigInteger a, BigInteger b) {
            return Greater(a, b);
        }

        /// <summary>
        /// Smaller test between two BigIntegers.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>True if a &lt; b, false otherwise</returns>
        public static bool operator <(BigInteger a, BigInteger b) {
            return Smaller(a, b);
        }

        /// <summary>
        /// Greater or equal test between two BigIntegers.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>True if a &gt;= b, false otherwise</returns>
        public static bool operator >=(BigInteger a, BigInteger b) {
            return GreaterOrEqual(a, b);
        }

        /// <summary>
        /// Smaller or equal test between two BigIntegers.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>True if a &lt;= b, false otherwise</returns>
        public static bool operator <=(BigInteger a, BigInteger b) {
            return SmallerOrEqual(a, b);
        }

        /// <summary>
        /// BigInteger inverse with respect to addition.
        /// </summary>
        /// <param name="n">The BigInteger whose opposite is to be computed</param>
        /// <returns>The BigInteger inverse with respect to addition</returns>
        public static BigInteger operator -(BigInteger n) {
            return Opposite(n);
        }

        /// <summary>
        /// Addition operation of two BigIntegers.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>The BigInteger result of the addition</returns>
        public static BigInteger operator +(BigInteger a, BigInteger b) {
            return Addition(a, b);
        }

        /// <summary>
        /// Subtraction operation of two BigIntegers.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>The BigInteger result of the subtraction</returns>
        public static BigInteger operator -(BigInteger a, BigInteger b) {
            return Subtraction(a, b);
        }

        /// <summary>
        /// Multiplication operation of two BigIntegers.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>The BigInteger result of the multiplication</returns>
        public static BigInteger operator *(BigInteger a, BigInteger b) {
            return Multiplication(a, b);
        }

        /// <summary>
        /// Division operation of two BigIntegers a and b, b != 0.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>The BigInteger result of the division</returns>
        /// <exception cref="BigIntegerException">Cannot divide by zero exception</exception>
        public static BigInteger operator /(BigInteger a, BigInteger b) {
            return Division(a, b);
        }

        /// <summary>
        /// Modulo operation of two BigIntegers a and b, b != 0.
        /// </summary>
        /// <param name="a">The 1st BigInteger</param>
        /// <param name="b">The 2nd BigInteger</param>
        /// <returns>The BigInteger result of the modulo</returns>
        /// <exception cref="BigIntegerException">Cannot divide by zero exception</exception>
        public static BigInteger operator %(BigInteger a, BigInteger b) {
            return Modulo(a, b);
        }

        /// <summary>
        /// Incremetation by one operation of a BigInteger.
        /// </summary>
        /// <param name="n">The BigInteger to be incremented by one</param>
        /// <returns>The BigInteger result of incrementing by one</returns>
        public static BigInteger operator ++(BigInteger n) {
            BigInteger res = n + One;
            return res;
        }

        /// <summary>
        /// Decremetation by one operation of a BigInteger.
        /// </summary>
        /// <param name="n">The BigInteger to be decremented by one</param>
        /// <returns>The BigInteger result of decrementing by one</returns>
        public static BigInteger operator --(BigInteger n) {
            BigInteger res = n - One;
            return res;
        }


        #endregion


        #region Private Methods


        /// <summary>
        /// Adds two BigNumbers a and b, where a >= b, a, b non-negative.
        /// </summary>
        private static BigInteger Add(BigInteger a, BigInteger b) {
            BigInteger res = new BigInteger(a);
            long trans = 0, temp;
            int i;

            for (i = 0; i < b.size; i++) {
                temp = res.digits[i] + b.digits[i] + trans;
                res.digits[i] = temp % NumberBase;
                trans = temp / NumberBase;
            }

            for (i = b.size; ((i < a.size) && (trans > 0)); i++) {
                temp = res.digits[i] + trans;
                res.digits[i] = temp % NumberBase;
                trans = temp / NumberBase;
            }

            if (trans > 0) {
                res.digits[res.size] = trans % NumberBase;
                res.size++;
                trans /= NumberBase;
            }

            return res;
        }

        /// <summary>
        /// Subtracts the BigInteger b from the BigInteger a, where a >= b, a, b non-negative.
        /// </summary>
        private static BigInteger Subtract(BigInteger a, BigInteger b) {
            BigInteger res = new BigInteger(a);
            int i;
            long temp, trans = 0;
            bool reducible = true;

            for (i = 0; i < b.size; i++) {
                temp = res.digits[i] - b.digits[i] - trans;
                if (temp < 0) {
                    trans = 1;
                    temp += NumberBase;
                } else trans = 0;
                res.digits[i] = temp;
            }

            for (i = b.size; ((i < a.size) && (trans > 0)); i++) {
                temp = res.digits[i] - trans;
                if (temp < 0) {
                    trans = 1;
                    temp += NumberBase;
                } else trans = 0;
                res.digits[i] = temp;
            }

            while ((res.size - 1 > 0) && (reducible == true)) {
                if (res.digits[res.size - 1] == 0)
                    res.size--;
                else reducible = false;
            }

            return res;
        }

        /// <summary>
        /// Multiplies two BigIntegers.
        /// </summary>
        private static BigInteger Multiply(BigInteger a, BigInteger b) {
            int i, j;
            long temp, trans = 0;

            BigInteger res = new BigInteger();
            res.size = a.size + b.size - 1;
            for (i = 0; i < res.size + 1; i++)
                res.digits[i] = 0;

            for (i = 0; i < a.size; i++)
                if (a.digits[i] != 0)
                    for (j = 0; j < b.size; j++)
                        if (b.digits[j] != 0)
                            res.digits[i + j] += a.digits[i] * b.digits[j];

            for (i = 0; i < res.size; i++) {
                temp = res.digits[i] + trans;
                res.digits[i] = temp % NumberBase;
                trans = temp / NumberBase;
            }

            if (trans > 0) {
                res.digits[res.size] = trans % NumberBase;
                res.size++;
                trans /= NumberBase;
            }

            return res;
        }

        /// <summary>
        /// Divides a BigInteger by a one-digit int.
        /// </summary>
        private static BigInteger DivideByOneDigitNumber(BigInteger a, long b) {
            BigInteger res = new BigInteger();
            int i = a.size - 1;
            long temp;

            res.size = a.size;
            temp = a.digits[i];

            while (i >= 0) {
                res.digits[i] = temp / b;
                temp %= b;
                i--;

                if (i >= 0)
                    temp = temp * NumberBase + a.digits[i];
            }

            if ((res.digits[res.size - 1] == 0) && (res.size != 1))
                res.size--;

            return res;
        }

        /// <summary>
        /// Divides a BigInteger by another BigInteger.
        /// </summary>
        private static BigInteger DivideByBigNumber(BigInteger a, BigInteger b) {
            int k, n = a.size, m = b.size;
            long f, qt;
            BigInteger d, dq, q, r;

            f = NumberBase / (b.digits[m - 1] + 1);
            q = new BigInteger();
            r = a * f;
            d = b * f;

            for (k = n - m; k >= 0; k--) {
                qt = Trial(r, d, k, m);
                dq = d * qt;

                if (DivideByBigNumberSmaller(r, dq, k, m)) {
                    qt--;
                    dq = d * qt;
                }

                q.digits[k] = qt;
                Difference(r, dq, k, m);
            }

            q.size = n - m + 1;
            if ((q.size != 1) && (q.digits[q.size - 1] == 0))
                q.size--;

            return q;
        }

        /// <summary>
        /// DivideByBigNumber auxiliary method. 
        /// </summary>
        private static bool DivideByBigNumberSmaller(BigInteger r, BigInteger dq, int k, int m) {
            int i = m, j = 0;

            while (i != j) {
                if (r.digits[i + k] != dq.digits[i])
                    j = i;
                else i--;
            }

            if (r.digits[i + k] < dq.digits[i])
                return true;
            else
                return false;
        }

        /// <summary>
        /// DivideByBigNumber auxilary method.
        /// </summary>
        private static void Difference(BigInteger r, BigInteger dq, int k, int m) {
            int i;
            long borrow = 0, diff;

            for (i = 0; i <= m; i++) {
                diff = r.digits[i + k] - dq.digits[i] - borrow + NumberBase;
                r.digits[i + k] = diff % NumberBase;
                borrow = 1 - diff / NumberBase;
            }
        }

        /// <summary>
        /// DivideByBigNumber auxilary method.
        /// </summary>
        private static long Trial(BigInteger r, BigInteger d, int k, int m) {
            long d2, km = k + m, r3, res;

            r3 = ((long)r.digits[km] * NumberBase + (long)r.digits[km - 1]) * NumberBase + (long)r.digits[km - 2];
            d2 = (long)d.digits[m - 1] * NumberBase + (long)d.digits[m - 2];
            res = r3 / d2;
            if (res < NumberBase - 1)
                return (int)res;
            else
                return NumberBase - 1;
        }


        #endregion

        public int bitCount() {
            while (size > 1 && digits[size - 1] == 0)
                size--;

            long value = digits[size - 1];
            long mask = 0x80000000;
            int bits = 32;

            while (bits > 0 && (value & mask) == 0) {
                bits--;
                mask >>= 1;
            }
            bits += ((size - 1) << 5);

            return bits;
        }
        public byte[] getBytes() {
            int numBits = bitCount();

            int numBytes = numBits >> 3;
            if ((numBits & 0x7) != 0)
                numBytes++;

            byte[] result = new byte[numBytes];

            //Console.WriteLine(result.Length);

            int pos = 0;
            long tempVal, val = digits[size - 1];

            if ((tempVal = (val >> 24 & 0xFF)) != 0)
                result[pos++] = (byte)tempVal;
            if ((tempVal = (val >> 16 & 0xFF)) != 0)
                result[pos++] = (byte)tempVal;
            if ((tempVal = (val >> 8 & 0xFF)) != 0)
                result[pos++] = (byte)tempVal;
            if ((tempVal = (val & 0xFF)) != 0)
                result[pos++] = (byte)tempVal;

            for (int i = size - 2; i >= 0; i--, pos += 4) {
                val = digits[i];
                result[pos + 3] = (byte)(val & 0xFF);
                val >>= 8;
                result[pos + 2] = (byte)(val & 0xFF);
                val >>= 8;
                result[pos + 1] = (byte)(val & 0xFF);
                val >>= 8;
                result[pos] = (byte)(val & 0xFF);
            }

            return result;
        }
        public BigInteger modPow(BigInteger exp, BigInteger n) {
            if ((exp.digits[MaxSize - 1] & 0x80000000) != 0)
                throw (new ArithmeticException("Positive exponents only."));

            BigInteger resultNum = 1;
            BigInteger tempNum;
            bool thisNegative = false;

            if ((this.digits[MaxSize - 1] & 0x80000000) != 0)   // negative this
            {
                tempNum = -this % n;
                thisNegative = true;
            } else
                tempNum = this % n;  // ensures (tempNum * tempNum) < b^(2k)

            if ((n.digits[MaxSize - 1] & 0x80000000) != 0)   // negative n
                n = -n;

            // calculate constant = b^(2k) / m
            BigInteger constant = new BigInteger();

            int i = n.size << 1;
            constant.digits[i] = 0x00000001;
            constant.size = i + 1;

            constant = constant / n;
            int totalBits = exp.bitCount();
            int count = 0;

            // perform squaring and multiply exponentiation
            for (int pos = 0; pos < exp.size; pos++) {
                uint mask = 0x01;
                //Console.WriteLine("pos = " + pos);

                for (int index = 0; index < 32; index++) {
                    if ((exp.digits[pos] & mask) != 0)
                        resultNum = BarrettReduction(resultNum * tempNum, n, constant);

                    mask <<= 1;

                    tempNum = BarrettReduction(tempNum * tempNum, n, constant);


                    if (tempNum.size == 1 && tempNum.digits[0] == 1) {
                        if (thisNegative && (exp.digits[0] & 0x1) != 0)    //odd exp
                            return -resultNum;
                        return resultNum;
                    }
                    count++;
                    if (count == totalBits)
                        break;
                }
            }

            if (thisNegative && (exp.digits[0] & 0x1) != 0)    //odd exp
                return -resultNum;

            return resultNum;
        }
        private BigInteger BarrettReduction(BigInteger x, BigInteger n, BigInteger constant) {
            int k = n.size,
                kPlusOne = k + 1,
                kMinusOne = k - 1;

            BigInteger q1 = new BigInteger();

            // q1 = x / b^(k-1)
            for (int i = kMinusOne, j = 0; i < x.size; i++, j++)
                q1.digits[j] = x.digits[i];
            q1.size = x.size - kMinusOne;
            if (q1.size <= 0)
                q1.size = 1;


            BigInteger q2 = q1 * constant;
            BigInteger q3 = new BigInteger();

            // q3 = q2 / b^(k+1)
            for (int i = kPlusOne, j = 0; i < q2.size; i++, j++)
                q3.digits[j] = q2.digits[i];
            q3.size = q2.size - kPlusOne;
            if (q3.size <= 0)
                q3.size = 1;


            // r1 = x mod b^(k+1)
            // i.e. keep the lowest (k+1) words
            BigInteger r1 = new BigInteger();
            int lengthToCopy = (x.size > kPlusOne) ? kPlusOne : x.size;
            for (int i = 0; i < lengthToCopy; i++)
                r1.digits[i] = x.digits[i];
            r1.size = lengthToCopy;


            // r2 = (q3 * n) mod b^(k+1)
            // partial multiplication of q3 and n

            BigInteger r2 = new BigInteger();
            for (int i = 0; i < q3.size; i++) {
                if (q3.digits[i] == 0) continue;

                ulong mcarry = 0;
                int t = i;
                for (int j = 0; j < n.size && t < kPlusOne; j++, t++) {
                    // t = i + j
                    ulong val = ((ulong)q3.digits[i] * (ulong)n.digits[j]) +
                                 (ulong)r2.digits[t] + mcarry;

                    r2.digits[t] = (uint)(val & 0xFFFFFFFF);
                    mcarry = (val >> 32);
                }

                if (t < kPlusOne)
                    r2.digits[t] = (uint)mcarry;
            }
            r2.size = kPlusOne;
            while (r2.size > 1 && r2.digits[r2.size - 1] == 0)
                r2.size--;

            r1 -= r2;
            if ((r1.digits[MaxSize - 1] & 0x80000000) != 0)        // negative
            {
                BigInteger val = new BigInteger();
                val.digits[kPlusOne] = 0x00000001;
                val.size = kPlusOne + 1;
                r1 += val;
            }

            while (r1 >= n)
                r1 -= n;

            return r1;
        }

    }


    /// <summary>
    /// The number's sign, where Positive also stands for the number zero.
    /// </summary>
    enum Sign { Positive, Negative };

    /// <summary>
    /// BigInteger-related exception class.
    /// </summary>
    [Serializable]
    public sealed class BigIntegerException : Exception {

        /// <summary>
        /// BigIntegerException constructor.
        /// </summary>
        /// <param name="message">The exception message</param>
        /// <param name="innerException">The inner exception</param>
        public BigIntegerException(string message, Exception innerException)
            : base(message, innerException) {
        }

    }


    /// <summary>
    /// Integer inefficiently represented internally using base-10 digits, in order to allow a
    /// visual representation as a base-10 string. Only for internal use.
    /// </summary>
    sealed class Base10BigInteger {

        #region Fields

        /// <summary>
        /// 10 numeration base for string representation, very inefficient for computations.
        /// </summary>
        private const long NumberBase = 10;

        /// <summary>
        /// Maximum size for numbers is up to 10240 binary digits or approximately (safe to use) 3000 decimal digits.
        /// The maximum size is, in fact, double the previously specified amount, in order to accommodate operations'
        /// overflow.
        /// </summary>
        private const int MaxSize = BigInteger.MaxSize * 5;


        /// Integer constants
        private static readonly Base10BigInteger Zero = new Base10BigInteger();
        private static readonly Base10BigInteger One = new Base10BigInteger(1);


        /// <summary>
        /// The array of digits of the number.
        /// </summary>
        private long[] digits;

        /// <summary>
        /// The actual number of digits of the number.
        /// </summary>
        private int size;

        /// <summary>
        /// The number sign.
        /// </summary>
        private Sign sign;


        #endregion


        #region Internal Fields


        /// <summary>
        /// Sets the number sign.
        /// </summary>
        internal Sign NumberSign {
            set { sign = value; }
        }


        #endregion


        #region Constructors


        /// <summary>
        /// Default constructor, intializing the Base10BigInteger with zero.
        /// </summary>
        public Base10BigInteger() {
            digits = new long[MaxSize];
            size = 1;
            digits[size] = 0;
            sign = Sign.Positive;
        }

        /// <summary>
        /// Constructor creating a new Base10BigInteger as a conversion of a regular base-10 long.
        /// </summary>
        /// <param name="n">The base-10 long to be converted</param>
        public Base10BigInteger(long n) {
            digits = new long[MaxSize];
            sign = Sign.Positive;

            if (n == 0) {
                size = 1;
                digits[size] = 0;
            } else {
                if (n < 0) {
                    n = -n;
                    sign = Sign.Negative;
                }

                size = 0;
                while (n > 0) {
                    digits[size] = n % NumberBase;
                    n /= NumberBase;
                    size++;
                }
            }
        }

        /// <summary>
        /// Constructor creating a new Base10BigInteger as a copy of an existing Base10BigInteger.
        /// </summary>
        /// <param name="n">The Base10BigInteger to be copied</param>
        public Base10BigInteger(Base10BigInteger n) {
            digits = new long[MaxSize];
            size = n.size;
            sign = n.sign;

            for (int i = 0; i < n.size; i++)
                digits[i] = n.digits[i];
        }


        #endregion


        #region Public Methods


        /// <summary>
        /// Determines whether the specified Base10BigInteger is equal to the current Base10BigInteger.
        /// </summary>
        /// <param name="other">The Base10BigInteger to compare with the current Base10BigInteger</param>
        /// <returns>True if the specified Base10BigInteger is equal to the current Base10BigInteger,
        /// false otherwise</returns>
        public bool Equals(Base10BigInteger other) {
            if (sign != other.sign)
                return false;
            if (size != other.size)
                return false;

            for (int i = 0; i < size; i++)
                if (digits[i] != other.digits[i])
                    return false;

            return true;
        }

        /// <summary>
        /// Determines whether the specified System.Object is equal to the current Base10BigInteger.
        /// </summary>
        /// <param name="o">The System.Object to compare with the current Base10BigInteger</param>
        /// <returns>True if the specified System.Object is equal to the current Base10BigInteger,
        /// false otherwise</returns>
        public override bool Equals(object o) {
            if ((o is Base10BigInteger) == false)
                return false;

            return Equals((Base10BigInteger)o);
        }

        /// <summary>
        /// Serves as a hash function for the Base10BigInteger type.
        /// </summary>
        /// <returns>A hash code for the current Base10BigInteger</returns>
        public override int GetHashCode() {
            int result = 0;

            for (int i = 0; i < size; i++)
                result = result + (int)digits[i];

            return result;
        }

        /// <summary>
        /// String representation of the current Base10BigInteger, converted to its base-10 representation.
        /// </summary>
        /// <returns>The string representation of the current Base10BigInteger</returns>
        public override string ToString() {
            StringBuilder output;

            if (sign == Sign.Negative) {
                output = new StringBuilder(size + 1);
                output.Append('-');
            } else
                output = new StringBuilder(size);

            for (int i = size - 1; i >= 0; i--)
                output.Append(digits[i]);

            return output.ToString();
        }

        /// <summary>
        /// Base10BigInteger inverse with respect to addition.
        /// </summary>
        /// <param name="n">The Base10BigInteger whose opposite is to be computed</param>
        /// <returns>The Base10BigInteger inverse with respect to addition</returns>
        public static Base10BigInteger Opposite(Base10BigInteger n) {
            Base10BigInteger res = new Base10BigInteger(n);

            if (res != Zero) {
                if (res.sign == Sign.Positive)
                    res.sign = Sign.Negative;
                else
                    res.sign = Sign.Positive;
            }

            return res;
        }

        /// <summary>
        /// Greater test between two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>True if a &gt; b, false otherwise</returns>
        public static bool Greater(Base10BigInteger a, Base10BigInteger b) {
            if (a.sign != b.sign) {
                if ((a.sign == Sign.Negative) && (b.sign == Sign.Positive))
                    return false;

                if ((a.sign == Sign.Positive) && (b.sign == Sign.Negative))
                    return true;
            } else {
                if (a.sign == Sign.Positive) {
                    if (a.size > b.size)
                        return true;
                    if (a.size < b.size)
                        return false;
                    for (int i = (a.size) - 1; i >= 0; i--)
                        if (a.digits[i] > b.digits[i])
                            return true;
                        else if (a.digits[i] < b.digits[i])
                            return false;
                } else {
                    if (a.size < b.size)
                        return true;
                    if (a.size > b.size)
                        return false;
                    for (int i = (a.size) - 1; i >= 0; i--)
                        if (a.digits[i] < b.digits[i])
                            return true;
                        else if (a.digits[i] > b.digits[i])
                            return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Greater or equal test between two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>True if a &gt;= b, false otherwise</returns>
        public static bool GreaterOrEqual(Base10BigInteger a, Base10BigInteger b) {
            return Greater(a, b) || Equals(a, b);
        }

        /// <summary>
        /// Smaller test between two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>True if a &lt; b, false otherwise</returns>
        public static bool Smaller(Base10BigInteger a, Base10BigInteger b) {
            return !GreaterOrEqual(a, b);
        }

        /// <summary>
        /// Smaller or equal test between two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>True if a &lt;= b, false otherwise</returns>
        public static bool SmallerOrEqual(Base10BigInteger a, Base10BigInteger b) {
            return !Greater(a, b);
        }

        /// <summary>
        /// Computes the absolute value of a Base10BigInteger.
        /// </summary>
        /// <param name="n">The Base10BigInteger whose absolute value is to be computed</param>
        /// <returns>The absolute value of the given BigInteger</returns>
        public static Base10BigInteger Abs(Base10BigInteger n) {
            Base10BigInteger res = new Base10BigInteger(n);
            res.sign = Sign.Positive;
            return res;
        }

        /// <summary>
        /// Addition operation of two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>The Base10BigInteger result of the addition</returns>
        public static Base10BigInteger Addition(Base10BigInteger a, Base10BigInteger b) {
            Base10BigInteger res = null;

            if ((a.sign == Sign.Positive) && (b.sign == Sign.Positive)) {
                if (a >= b)
                    res = Add(a, b);
                else
                    res = Add(b, a);

                res.sign = Sign.Positive;
            }

            if ((a.sign == Sign.Negative) && (b.sign == Sign.Negative)) {
                if (a <= b)
                    res = Add(-a, -b);
                else
                    res = Add(-b, -a);

                res.sign = Sign.Negative;
            }

            if ((a.sign == Sign.Positive) && (b.sign == Sign.Negative)) {
                if (a >= (-b)) {
                    res = Subtract(a, -b);
                    res.sign = Sign.Positive;
                } else {
                    res = Subtract(-b, a);
                    res.sign = Sign.Negative;
                }
            }

            if ((a.sign == Sign.Negative) && (b.sign == Sign.Positive)) {
                if ((-a) <= b) {
                    res = Subtract(b, -a);
                    res.sign = Sign.Positive;
                } else {
                    res = Subtract(-a, b);
                    res.sign = Sign.Negative;
                }
            }

            return res;
        }

        /// <summary>
        /// Subtraction operation of two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>The Base10BigInteger result of the subtraction</returns>
        public static Base10BigInteger Subtraction(Base10BigInteger a, Base10BigInteger b) {
            Base10BigInteger res = null;

            if ((a.sign == Sign.Positive) && (b.sign == Sign.Positive)) {
                if (a >= b) {
                    res = Subtract(a, b);
                    res.sign = Sign.Positive;
                } else {
                    res = Subtract(b, a);
                    res.sign = Sign.Negative;
                }
            }

            if ((a.sign == Sign.Negative) && (b.sign == Sign.Negative)) {
                if (a <= b) {
                    res = Subtract(-a, -b);
                    res.sign = Sign.Negative;
                } else {
                    res = Subtract(-b, -a);
                    res.sign = Sign.Positive;
                }
            }

            if ((a.sign == Sign.Positive) && (b.sign == Sign.Negative)) {
                if (a >= (-b))
                    res = Add(a, -b);
                else
                    res = Add(-b, a);

                res.sign = Sign.Positive;
            }

            if ((a.sign == Sign.Negative) && (b.sign == Sign.Positive)) {
                if ((-a) >= b)
                    res = Add(-a, b);
                else
                    res = Add(b, -a);

                res.sign = Sign.Negative;
            }

            return res;
        }

        /// <summary>
        /// Multiplication operation of two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>The Base10BigInteger result of the multiplication</returns>
        public static Base10BigInteger Multiplication(Base10BigInteger a, Base10BigInteger b) {
            if ((a == Zero) || (b == Zero))
                return Zero;

            Base10BigInteger res = Multiply(Abs(a), Abs(b));
            if (a.sign == b.sign)
                res.sign = Sign.Positive;
            else
                res.sign = Sign.Negative;

            return res;
        }


        #endregion


        #region Overloaded Operators


        /// <summary>
        /// Implicit conversion operator from long to Base10BigInteger.
        /// </summary>
        /// <param name="n">The long to be converted to a Base10BigInteger</param>
        /// <returns>The Base10BigInteger converted from the given long</returns>
        public static implicit operator Base10BigInteger(long n) {
            return new Base10BigInteger(n);
        }

        /// <summary>
        /// Equality test between two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>True if a == b, false otherwise</returns>
        public static bool operator ==(Base10BigInteger a, Base10BigInteger b) {
            return Equals(a, b);
        }

        /// <summary>
        /// Inequality test between two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>True if a != b, false otherwise</returns>
        public static bool operator !=(Base10BigInteger a, Base10BigInteger b) {
            return !Equals(a, b);
        }

        /// <summary>
        /// Greater test between two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>True if a &gt; b, false otherwise</returns>
        public static bool operator >(Base10BigInteger a, Base10BigInteger b) {
            return Greater(a, b);
        }

        /// <summary>
        /// Smaller test between two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>True if a &lt; b, false otherwise</returns>
        public static bool operator <(Base10BigInteger a, Base10BigInteger b) {
            return Smaller(a, b);
        }

        /// <summary>
        /// Greater or equal test between two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>True if a &gt;= b, false otherwise</returns>
        public static bool operator >=(Base10BigInteger a, Base10BigInteger b) {
            return GreaterOrEqual(a, b);
        }

        /// <summary>
        /// Smaller or equal test between two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>True if a &lt;= b, false otherwise</returns>
        public static bool operator <=(Base10BigInteger a, Base10BigInteger b) {
            return SmallerOrEqual(a, b);
        }

        /// <summary>
        /// Base10BigInteger inverse with respect to addition.
        /// </summary>
        /// <param name="n">The Base10BigInteger whose opposite is to be computed</param>
        /// <returns>The Base10BigInteger inverse with respect to addition</returns>
        public static Base10BigInteger operator -(Base10BigInteger n) {
            return Opposite(n);
        }

        /// <summary>
        /// Addition operation of two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>The Base10BigInteger result of the addition</returns>
        public static Base10BigInteger operator +(Base10BigInteger a, Base10BigInteger b) {
            return Addition(a, b);
        }

        /// <summary>
        /// Subtraction operation of two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>The Base10BigInteger result of the subtraction</returns>
        public static Base10BigInteger operator -(Base10BigInteger a, Base10BigInteger b) {
            return Subtraction(a, b);
        }

        /// <summary>
        /// Multiplication operation of two Base10BigIntegers.
        /// </summary>
        /// <param name="a">The 1st Base10BigInteger</param>
        /// <param name="b">The 2nd Base10BigInteger</param>
        /// <returns>The Base10BigInteger result of the multiplication</returns>
        public static Base10BigInteger operator *(Base10BigInteger a, Base10BigInteger b) {
            return Multiplication(a, b);
        }

        /// <summary>
        /// Incremetation by one operation of a Base10BigInteger.
        /// </summary>
        /// <param name="n">The Base10BigInteger to be incremented by one</param>
        /// <returns>The Base10BigInteger result of incrementing by one</returns>
        public static Base10BigInteger operator ++(Base10BigInteger n) {
            Base10BigInteger res = n + One;
            return res;
        }

        /// <summary>
        /// Decremetation by one operation of a Base10BigInteger.
        /// </summary>
        /// <param name="n">The Base10BigInteger to be decremented by one</param>
        /// <returns>The Base10BigInteger result of decrementing by one</returns>
        public static Base10BigInteger operator --(Base10BigInteger n) {
            Base10BigInteger res = n - One;
            return res;
        }


        #endregion


        #region Private Methods


        /// <summary>
        /// Adds two BigNumbers a and b, where a >= b, a, b non-negative.
        /// </summary>
        private static Base10BigInteger Add(Base10BigInteger a, Base10BigInteger b) {
            Base10BigInteger res = new Base10BigInteger(a);
            long trans = 0, temp;
            int i;

            for (i = 0; i < b.size; i++) {
                temp = res.digits[i] + b.digits[i] + trans;
                res.digits[i] = temp % NumberBase;
                trans = temp / NumberBase;
            }

            for (i = b.size; ((i < a.size) && (trans > 0)); i++) {
                temp = res.digits[i] + trans;
                res.digits[i] = temp % NumberBase;
                trans = temp / NumberBase;
            }

            if (trans > 0) {
                res.digits[res.size] = trans % NumberBase;
                res.size++;
                trans /= NumberBase;
            }

            return res;
        }

        /// <summary>
        /// Subtracts the Base10BigInteger b from the Base10BigInteger a, where a >= b, a, b non-negative.
        /// </summary>
        private static Base10BigInteger Subtract(Base10BigInteger a, Base10BigInteger b) {
            Base10BigInteger res = new Base10BigInteger(a);
            int i;
            long temp, trans = 0;
            bool reducible = true;

            for (i = 0; i < b.size; i++) {
                temp = res.digits[i] - b.digits[i] - trans;
                if (temp < 0) {
                    trans = 1;
                    temp += NumberBase;
                } else trans = 0;
                res.digits[i] = temp;
            }

            for (i = b.size; ((i < a.size) && (trans > 0)); i++) {
                temp = res.digits[i] - trans;
                if (temp < 0) {
                    trans = 1;
                    temp += NumberBase;
                } else trans = 0;
                res.digits[i] = temp;
            }

            while ((res.size - 1 > 0) && (reducible == true)) {
                if (res.digits[res.size - 1] == 0)
                    res.size--;
                else reducible = false;
            }

            return res;
        }

        /// <summary>
        /// Multiplies two Base10BigIntegers.
        /// </summary>
        private static Base10BigInteger Multiply(Base10BigInteger a, Base10BigInteger b) {
            int i, j;
            long temp, trans = 0;

            Base10BigInteger res = new Base10BigInteger();
            res.size = a.size + b.size - 1;
            for (i = 0; i < res.size + 1; i++)
                res.digits[i] = 0;

            for (i = 0; i < a.size; i++)
                if (a.digits[i] != 0)
                    for (j = 0; j < b.size; j++)
                        if (b.digits[j] != 0)
                            res.digits[i + j] += a.digits[i] * b.digits[j];

            for (i = 0; i < res.size; i++) {
                temp = res.digits[i] + trans;
                res.digits[i] = temp % NumberBase;
                trans = temp / NumberBase;
            }

            if (trans > 0) {
                res.digits[res.size] = trans % NumberBase;
                res.size++;
                trans /= NumberBase;
            }

            return res;
        }


        #endregion


    }


    static class PrimalityTester {

        /// <summary>
        /// Maximum number of iterations for the Miller-Rabin test.
        /// </summary>
        private const int MaxMillerRabinIterations = 3;

        // Constants
        private static readonly BigInteger Zero;
        private static readonly BigInteger One;
        private static readonly BigInteger Two;
        private static readonly BigInteger Three;

        private static readonly long[] SmallPrimes;
        private static readonly List<BigInteger> SmallPrimesList;


        static PrimalityTester() {
            Zero = new BigInteger();
            One = new BigInteger(1);
            Two = new BigInteger(2);
            Three = new BigInteger(3);

            SmallPrimes = new long[] {2, 3, 5, 7, 11, 13, 17, 19, 23, 29,
                                      31, 37, 41, 43, 47, 53, 59, 61, 67, 71,
                                      73, 79, 83, 89, 97, 101, 103, 107, 109, 113,
                                      127, 131, 137, 139, 149, 151, 157, 163, 167, 173,
                                      179, 181, 191, 193, 197, 199, 211, 223, 227, 229,
                                      233, 239, 241, 251, 257, 263, 269, 271, 277, 281,
                                      283, 293, 307, 311, 313, 317, 331, 337, 347, 349,
                                      353, 359, 367, 373, 379, 383, 389, 397, 401, 409,
                                      419, 421, 431, 433, 439, 443, 449, 457, 461, 463,
                                      467, 479, 487, 491, 499, 503, 509, 521, 523, 541,
                                      547, 557, 563, 569, 571, 577, 587, 593, 599, 601,
                                      607, 613, 617, 619, 631, 641, 643, 647, 653, 659,
                                      661, 673, 677, 683, 691, 701, 709, 719, 727, 733,
                                      739, 743, 751, 757, 761, 769, 773, 787, 797, 809,
                                      811, 821, 823, 827, 829, 839, 853, 857, 859, 863,
                                      877, 881, 883, 887, 907, 911, 919, 929, 937, 941,
                                      947, 953, 967, 971, 977, 983, 991, 997};

            SmallPrimesList = new List<BigInteger>(SmallPrimes.Length);
            for (int i = 0; i < SmallPrimes.Length; i++)
                SmallPrimesList.Add(new BigInteger(SmallPrimes[i]));
        }


        /// <summary>
        /// Tests whether the number provided is a prime.
        /// </summary>
        /// <param name="number">The number to be tested for primality</param>
        /// <returns>True, if the number given is a prime, false, otherwise</returns>
        public static bool IsPrime(BigInteger number) {
            if (IsSmallPrime(number) == true)
                return true;
            else
                return IsPrimeByTrialDivision(number) && IsPrimeMillerRabinTest(number);
        }


        private static bool IsSmallPrime(BigInteger number) {
            foreach (BigInteger aPrime in SmallPrimesList)
                if (number == aPrime)
                    return true;

            return false;
        }


        private static bool IsPrimeByTrialDivision(BigInteger number) {
            foreach (BigInteger aPrime in SmallPrimesList)
                if ((number != aPrime) && (number % aPrime == Zero))
                    return false;

            return true;
        }


        /// <summary>
        /// Determines whether the given BigInteger number is probably prime, with a probability
        /// of at least 1/(4^MaxMillerRabinIterations), using the Miller-Rabin primality test.
        /// </summary>
        private static bool IsPrimeMillerRabinTest(BigInteger number) {
            BigInteger s = new BigInteger();
            BigInteger t = number - One;
            BigInteger b = new BigInteger(2);
            BigInteger nmin1 = number - One;
            BigInteger r, j, smin1;

            if (number == One)
                return false;
            else if (number == Two)
                return true;
            else if (number == Three)
                return true;

            while (t % Two == Zero) {
                t /= Two;
                s++;
            }

            smin1 = s - One;

            for (int i = 0; i < MaxMillerRabinIterations; i++) {
                r = BigInteger.ModularExponentiation(b, t, number);

                if ((r != One) && (r != nmin1)) {
                    j = new BigInteger(One);
                    while ((j <= smin1) && (r != nmin1)) {
                        r = (r * r) % number;
                        if (r == One)
                            return false;
                        j++;
                    }
                    if (r != nmin1)
                        return false;
                }

                if (b == Two)
                    b++;
                else
                    b += Two;
            }

            return true;
        }

    }
}
