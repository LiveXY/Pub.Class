using System;
using System.Globalization;
using System.Runtime;


[Serializable]
public struct BigInteger : IFormattable, IComparable, IComparable<BigInteger>, IEquatable<BigInteger> {
    internal uint[] _bits;

    internal int _sign;

    private const int DecimalScaleFactorMask = 16711680;

    private const int DecimalSignMask = -2147483648;

    private const int kcbitUint = 32;

    private const int kcbitUlong = 64;

    private const int knMaskHighBit = -2147483648;

    private const uint kuMaskHighBit = 2147483648;

    private readonly static BigInteger s_bnMinInt;

    private readonly static BigInteger s_bnMinusOneInt;

    private readonly static BigInteger s_bnOneInt;

    private readonly static BigInteger s_bnZeroInt;

    internal uint[] _Bits {
        get {
            return this._bits;
        }
    }

    internal int _Sign {
        get {
            return this._sign;
        }
    }

    public bool IsEven {
        get {
            if (this._bits == null) {
                return (this._sign & 1) == 0;
            } else {
                return (this._bits[0] & 1) == 0;
            }
        }
    }

    public bool IsOne {
        get {
            if (this._sign != 1) {
                return false;
            } else {
                return this._bits == null;
            }
        }
    }

    public bool IsPowerOfTwo {
        get {
            if (this._bits != null) {
                if (this._sign == 1) {
                    int num = BigInteger.Length(this._bits) - 1;
                    if ((this._bits[num] & this._bits[num] - 1) == 0) {
                        do {
                            int num1 = num - 1;
                            num = num1;
                            if (num1 >= 0) {
                                continue;
                            }
                            return true;
                        }
                        while (this._bits[num] == 0);
                        return false;
                    } else {
                        return false;
                    }
                } else {
                    return false;
                }
            } else {
                if ((this._sign & this._sign - 1) != 0) {
                    return false;
                } else {
                    return this._sign != 0;
                }
            }
        }
    }

    public bool IsZero {
        get {
            return this._sign == 0;
        }
    }

    public static BigInteger MinusOne {
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get {
            return BigInteger.s_bnMinusOneInt;
        }
    }

    public static BigInteger One {
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get {
            return BigInteger.s_bnOneInt;
        }
    }

    public int Sign {
        get {
            return (this._sign >> 31) - (-this._sign >> 31);
        }
    }

    public static BigInteger Zero {
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get {
            return BigInteger.s_bnZeroInt;
        }
    }

    static BigInteger() {
        uint[] numArray = new uint[1];
        numArray[0] = -2147483648;
        BigInteger.s_bnMinInt = new BigInteger(-1, numArray);
        BigInteger.s_bnOneInt = new BigInteger(1);
        BigInteger.s_bnZeroInt = new BigInteger(0);
        BigInteger.s_bnMinusOneInt = new BigInteger(-1);
    }

    public BigInteger(int value) {
        if (value != -2147483648) {
            this._sign = value;
            this._bits = null;
            return;
        } else {
            this = BigInteger.s_bnMinInt;
            return;
        }
    }

    [CLSCompliant(false)]
    public BigInteger(uint value) {
        if (value > 2147483647) {
            this._sign = 1;
            this._bits = new uint[1];
            this._bits[0] = value;
            return;
        } else {
            this._sign = value;
            this._bits = null;
            return;
        }
    }

    public BigInteger(long value) {
        ulong num;
        if ((long)-2147483648 > value || value > (long)2147483647) {
            if (value >= (long)0) {
                num = value;
                this._sign = 1;
            } else {
                num = -value;
                this._sign = -1;
            }
            this._bits = new uint[2];
            this._bits[0] = (uint)num;
            this._bits[1] = (uint)(num >> 32);
            return;
        } else {
            if (value != (long)-2147483648) {
                this._sign = (int)value;
                this._bits = null;
                return;
            } else {
                *(this) = BigInteger.s_bnMinInt;
                return;
            }
        }
    }

    [CLSCompliant(false)]
    public BigInteger(ulong value) {
        if (value > (long)2147483647) {
            this._sign = 1;
            this._bits = new uint[2];
            this._bits[0] = (uint)value;
            this._bits[1] = (uint)(value >> 32);
            return;
        } else {
            this._sign = (int)value;
            this._bits = null;
            return;
        }
    }

    public BigInteger(float value) {
        if (!float.IsInfinity(value)) {
            if (!float.IsNaN(value)) {
                this._sign = 0;
                this._bits = null;
                this.SetBitsFromDouble((double)value);
                return;
            } else {
                throw new OverflowException(SR.GetString("Overflow_NotANumber"));
            }
        } else {
            throw new OverflowException(SR.GetString("Overflow_BigIntInfinity"));
        }
    }

    public BigInteger(double value) {
        if (!double.IsInfinity(value)) {
            if (!double.IsNaN(value)) {
                this._sign = 0;
                this._bits = null;
                this.SetBitsFromDouble(value);
                return;
            } else {
                throw new OverflowException(SR.GetString("Overflow_NotANumber"));
            }
        } else {
            throw new OverflowException(SR.GetString("Overflow_BigIntInfinity"));
        }
    }

    public BigInteger(decimal value) {
        byte num;
        int num1;
        int[] bits = decimal.GetBits(decimal.Truncate(value));
        int num2 = 3;
        while (num2 > 0 && bits[num2 - 1] == 0) {
            num2--;
        }
        if (num2 != 0) {
            if (num2 != 1 || bits[0] <= 0) {
                this._bits = new uint[num2];
                this._bits[0] = bits[0];
                if (num2 > 1) {
                    this._bits[1] = bits[1];
                }
                if (num2 > 2) {
                    this._bits[2] = bits[2];
                }
                BigInteger bigInteger = this;
                if ((bits[3] & -2147483648) != 0) {
                    num = -1;
                } else {
                    num = 1;
                }
                bigInteger._sign = (int)num;
                return;
            } else {
                this._sign = bits[0];
                BigInteger bigInteger1 = this;
                int num3 = bigInteger1._sign;
                if ((bits[3] & -2147483648) != 0) {
                    num1 = -1;
                } else {
                    num1 = 1;
                }
                bigInteger1._sign = num3 * num1;
                this._bits = null;
                return;
            }
        } else {
            *(this) = BigInteger.s_bnZeroInt;
            return;
        }
    }

    [CLSCompliant(false)]
    public BigInteger(byte[] value) {
        bool flag;
        int num;
        int num1;
        if (value != null) {
            int length = (int)value.Length;
            if (length <= 0) {
                flag = false;
            } else {
                flag = (value[length - 1] & 128) == 128;
            }
            bool flag1 = flag;
            while (length > 0 && value[length - 1] == 0) {
                length--;
            }
            if (length != 0) {
                if (length > 4) {
                    int num2 = length % 4;
                    int num3 = length / 4;
                    if (num2 == 0) {
                        num = 0;
                    } else {
                        num = 1;
                    }
                    int num4 = num3 + num;
                    bool flag2 = true;
                    uint[] numArray = new uint[num4];
                    int j = 3;
                    int num5 = 0;
                    while (true) {
                        int num6 = num5;
                        int num7 = num4;
                        if (num2 == 0) {
                            num1 = 0;
                        } else {
                            num1 = 1;
                        }
                        if (num6 >= num7 - num1) {
                            break;
                        }
                        for (int i = 0; i < 4; i++) {
                            if (value[j] != 0) {
                                flag2 = false;
                            }
                            numArray[num5] = numArray[num5] << 8;
                            numArray[num5] = numArray[num5] | value[j];
                            j--;
                        }
                        j = j + 8;
                        num5++;
                    }
                    if (num2 != 0) {
                        if (flag1) {
                            numArray[num4 - 1] = -1;
                        }
                        for (j = length - 1; j >= length - num2; j--) {
                            if (value[j] != 0) {
                                flag2 = false;
                            }
                            numArray[num5] = numArray[num5] << 8;
                            numArray[num5] = numArray[num5] | value[j];
                        }
                    }
                    if (!flag2) {
                        if (!flag1) {
                            this._sign = 1;
                            this._bits = numArray;
                        } else {
                            NumericsHelpers.DangerousMakeTwosComplement(numArray);
                            int length1 = (int)numArray.Length;
                            while (length1 > 0 && numArray[length1 - 1] == 0) {
                                length1--;
                            }
                            if (length1 != 1 || numArray[0] <= 0) {
                                if (length1 == (int)numArray.Length) {
                                    this._sign = -1;
                                    this._bits = numArray;
                                    return;
                                } else {
                                    this._sign = -1;
                                    this._bits = new uint[length1];
                                    Array.Copy(numArray, this._bits, length1);
                                    return;
                                }
                            } else {
                                if (numArray[0] != 1) {
                                    if (numArray[0] != -2147483648) {
                                        this._sign = -1 * numArray[0];
                                        this._bits = null;
                                        return;
                                    } else {
                                        *(this) = BigInteger.s_bnMinInt;
                                        return;
                                    }
                                } else {
                                    *(this) = BigInteger.s_bnMinusOneInt;
                                    return;
                                }
                            }
                        }
                    } else {
                        *(this) = BigInteger.s_bnZeroInt;
                        return;
                    }
                } else {
                    if (!flag1) {
                        this._sign = 0;
                    } else {
                        this._sign = -1;
                    }
                    for (int k = length - 1; k >= 0; k--) {
                        BigInteger bigInteger = this;
                        bigInteger._sign = bigInteger._sign << 8;
                        BigInteger bigInteger1 = this;
                        bigInteger1._sign = bigInteger1._sign | value[k];
                    }
                    this._bits = null;
                    if (this._sign < 0 && !flag1) {
                        this._bits = new uint[1];
                        this._bits[0] = this._sign;
                        this._sign = 1;
                    }
                    if (this._sign == -2147483648) {
                        *(this) = BigInteger.s_bnMinInt;
                        return;
                    }
                }
                return;
            } else {
                this._sign = 0;
                this._bits = null;
                return;
            }
        } else {
            throw new ArgumentNullException("value");
        }
    }

    internal BigInteger(int n, uint[] rgu) {
        this._sign = n;
        this._bits = rgu;
    }

    internal BigInteger(uint[] value, bool negative) {
        byte num;
        uint num1;
        if (value != null) {
            int length = (int)value.Length;
            while (length > 0 && value[length - 1] == 0) {
                length--;
            }
            if (length != 0) {
                if (length != 1 || value[0] >= -2147483648) {
                    BigInteger bigInteger = this;
                    if (negative) {
                        num = -1;
                    } else {
                        num = 1;
                    }
                    bigInteger._sign = (int)num;
                    this._bits = new uint[length];
                    Array.Copy(value, this._bits, length);
                } else {
                    BigInteger bigInteger1 = this;
                    if (negative) {
                        num1 = -value[0];
                    } else {
                        num1 = value[0];
                    }
                    bigInteger1._sign = (int)num1;
                    this._bits = null;
                    if (this._sign == -2147483648) {
                        *(this) = BigInteger.s_bnMinInt;
                        return;
                    }
                }
                return;
            } else {
                *(this) = BigInteger.s_bnZeroInt;
                return;
            }
        } else {
            throw new ArgumentNullException("value");
        }
    }

    private BigInteger(uint[] value) {
        bool flag;
        if (value != null) {
            int length = (int)value.Length;
            if (length <= 0) {
                flag = false;
            } else {
                flag = (value[length - 1] & -2147483648) == -2147483648;
            }
            bool flag1 = flag;
            while (length > 0 && value[length - 1] == 0) {
                length--;
            }
            if (length != 0) {
                if (length != 1) {
                    if (flag1) {
                        NumericsHelpers.DangerousMakeTwosComplement(value);
                        int num = (int)value.Length;
                        while (num > 0 && value[num - 1] == 0) {
                            num--;
                        }
                        if (num != 1 || value[0] <= 0) {
                            if (num == (int)value.Length) {
                                this._sign = -1;
                                this._bits = value;
                                return;
                            } else {
                                this._sign = -1;
                                this._bits = new uint[num];
                                Array.Copy(value, this._bits, num);
                                return;
                            }
                        } else {
                            if (value[0] != 1) {
                                if (value[0] != -2147483648) {
                                    this._sign = -1 * value[0];
                                    this._bits = null;
                                    return;
                                } else {
                                    *(this) = BigInteger.s_bnMinInt;
                                    return;
                                }
                            } else {
                                *(this) = BigInteger.s_bnMinusOneInt;
                                return;
                            }
                        }
                    } else {
                        if (length == (int)value.Length) {
                            this._sign = 1;
                            this._bits = value;
                            return;
                        } else {
                            this._sign = 1;
                            this._bits = new uint[length];
                            Array.Copy(value, this._bits, length);
                            return;
                        }
                    }
                } else {
                    if (value[0] >= 0 || flag1) {
                        if (-2147483648 != value[0]) {
                            this._sign = value[0];
                            this._bits = null;
                            return;
                        } else {
                            *(this) = BigInteger.s_bnMinInt;
                            return;
                        }
                    } else {
                        this._bits = new uint[1];
                        this._bits[0] = value[0];
                        this._sign = 1;
                        return;
                    }
                }
            } else {
                *(this) = BigInteger.s_bnZeroInt;
                return;
            }
        } else {
            throw new ArgumentNullException("value");
        }
    }

    public static BigInteger Abs(BigInteger value) {
        if (value >= BigInteger.Zero) {
            return value;
        } else {
            return -value;
        }
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BigInteger Add(BigInteger left, BigInteger right) {
        return left + right;
    }

    [Conditional("DEBUG")]
    private void AssertValid() {
        if (this._bits != null) {
            BigInteger.Length(this._bits);
        }
    }

    internal static int BitLengthOfUInt(uint x) {
        int num = 0;
        while (x > 0) {
            x = x >> 1;
            num++;
        }
        return num;
    }

    public static int Compare(BigInteger left, BigInteger right) {
        return left.CompareTo(right);
    }

    public int CompareTo(long other) {
        long num;
        ulong num1;
        if (this._bits != null) {
            if (((long)this._sign ^ other) >= (long)0) {
                int num2 = BigInteger.Length(this._bits);
                int num3 = num2;
                if (num2 <= 2) {
                    if (other < (long)0) {
                        num = -other;
                    } else {
                        num = other;
                    }
                    ulong num4 = (ulong)num;
                    if (num3 == 2) {
                        num1 = NumericsHelpers.MakeUlong(this._bits[1], this._bits[0]);
                    } else {
                        num1 = (ulong)this._bits[0];
                    }
                    ulong num5 = num1;
                    return this._sign * num5.CompareTo(num4);
                }
            }
            return this._sign;
        } else {
            long num6 = (long)this._sign;
            return num6.CompareTo(other);
        }
    }

    [CLSCompliant(false)]
    public int CompareTo(ulong other) {
        ulong num;
        if (this._sign >= 0) {
            if (this._bits != null) {
                int num1 = BigInteger.Length(this._bits);
                if (num1 <= 2) {
                    if (num1 == 2) {
                        num = NumericsHelpers.MakeUlong(this._bits[1], this._bits[0]);
                    } else {
                        num = (ulong)this._bits[0];
                    }
                    ulong num2 = num;
                    return num2.CompareTo(other);
                } else {
                    return 1;
                }
            } else {
                ulong num3 = (long)this._sign;
                return num3.CompareTo(other);
            }
        } else {
            return -1;
        }
    }

    public int CompareTo(BigInteger other) {
        if ((this._sign ^ other._sign) >= 0) {
            if (this._bits != null) {
                if (other._bits != null) {
                    int num = BigInteger.Length(this._bits);
                    int num1 = num;
                    int num2 = BigInteger.Length(other._bits);
                    int num3 = num2;
                    if (num <= num2) {
                        if (num1 >= num3) {
                            int diffLength = BigInteger.GetDiffLength(this._bits, other._bits, num1);
                            if (diffLength != 0) {
                                if (this._bits[diffLength - 1] < other._bits[diffLength - 1]) {
                                    return -this._sign;
                                } else {
                                    return this._sign;
                                }
                            } else {
                                return 0;
                            }
                        } else {
                            return -this._sign;
                        }
                    }
                }
                return this._sign;
            } else {
                if (other._bits != null) {
                    return -other._sign;
                } else {
                    if (this._sign < other._sign) {
                        return -1;
                    } else {
                        if (this._sign > other._sign) {
                            return 1;
                        } else {
                            return 0;
                        }
                    }
                }
            }
        } else {
            if (this._sign < 0) {
                return -1;
            } else {
                return 1;
            }
        }
    }

    public int CompareTo(object obj) {
        if (obj != null) {
            if (obj as BigInteger != null) {
                return this.CompareTo((BigInteger)obj);
            } else {
                throw new ArgumentException(SR.GetString("Argument_MustBeBigInt"));
            }
        } else {
            return 1;
        }
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BigInteger Divide(BigInteger dividend, BigInteger divisor) {
        return dividend / divisor;
    }

    public static BigInteger DivRem(BigInteger dividend, BigInteger divisor, out BigInteger remainder) {
        int num = 1;
        int num1 = 1;
        BigIntegerBuilder bigIntegerBuilder = new BigIntegerBuilder(dividend, ref num);
        BigIntegerBuilder bigIntegerBuilder1 = new BigIntegerBuilder(divisor, ref num1);
        BigIntegerBuilder bigIntegerBuilder2 = new BigIntegerBuilder();
        bigIntegerBuilder.ModDiv(ref bigIntegerBuilder1, ref bigIntegerBuilder2);
        remainder = bigIntegerBuilder.GetInteger(num);
        return bigIntegerBuilder2.GetInteger(num * num1);
    }

    public override bool Equals(object obj) {
        if (obj as BigInteger != null) {
            return this.Equals((BigInteger)obj);
        } else {
            return false;
        }
    }

    public bool Equals(long other) {
        long num;
        if (this._bits != null) {
            if (((long)this._sign ^ other) >= (long)0) {
                int num1 = BigInteger.Length(this._bits);
                int num2 = num1;
                if (num1 <= 2) {
                    if (other < (long)0) {
                        num = -other;
                    } else {
                        num = other;
                    }
                    ulong num3 = (ulong)num;
                    if (num2 != 1) {
                        return NumericsHelpers.MakeUlong(this._bits[1], this._bits[0]) == num3;
                    } else {
                        return (ulong)this._bits[0] == num3;
                    }
                }
            }
            return false;
        } else {
            return (long)this._sign == other;
        }
    }

    [CLSCompliant(false)]
    public bool Equals(ulong other) {
        if (this._sign >= 0) {
            if (this._bits != null) {
                int num = BigInteger.Length(this._bits);
                if (num <= 2) {
                    if (num != 1) {
                        return NumericsHelpers.MakeUlong(this._bits[1], this._bits[0]) == other;
                    } else {
                        return (ulong)this._bits[0] == other;
                    }
                } else {
                    return false;
                }
            } else {
                return (long)this._sign == other;
            }
        } else {
            return false;
        }
    }

    public bool Equals(BigInteger other) {
        if (this._sign == other._sign) {
            if (this._bits != other._bits) {
                if (this._bits == null || other._bits == null) {
                    return false;
                } else {
                    int num = BigInteger.Length(this._bits);
                    if (num == BigInteger.Length(other._bits)) {
                        int diffLength = BigInteger.GetDiffLength(this._bits, other._bits, num);
                        return diffLength == 0;
                    } else {
                        return false;
                    }
                }
            } else {
                return true;
            }
        } else {
            return false;
        }
    }

    internal static int GetDiffLength(uint[] rgu1, uint[] rgu2, int cu) {
        int num = cu;
        do {
            int num1 = num - 1;
            num = num1;
            if (num1 >= 0) {
                continue;
            }
            return 0;
        }
        while (rgu1[num] == rgu2[num]);
        return num + 1;
    }

    public override int GetHashCode() {
        if (this._bits != null) {
            int num = this._sign;
            int num1 = BigInteger.Length(this._bits);
            while (true) {
                int num2 = num1 - 1;
                num1 = num2;
                if (num2 < 0) {
                    break;
                }
                num = NumericsHelpers.CombineHash(num, this._bits[num1]);
            }
            return num;
        } else {
            return this._sign;
        }
    }

    private static bool GetPartsForBitManipulation(ref BigInteger x, out uint[] xd, out int xl)
    {
        int length;
        if (x._bits != null)
        {
            xd = x._bits;
        }
        else
        {
            if (x._sign >= 0)
            {
                uint[] numArray = new uint[1];
                numArray[0] = x._sign;
                xd = numArray;
            }
            else
            {
                uint[] numArray1 = new uint[1];
                numArray1[0] = -x._sign;
                xd = numArray1;
            }
        }
        int numPointer = xl;
        if (x._bits == null)
        {
            length = 1;
        }
        else
        {
            length = (int)x._bits.Length;
        }
        *(numPointer) = length;
        return x._sign < 0;
    }

    public static BigInteger GreatestCommonDivisor(BigInteger left, BigInteger right) {
        if (left._sign != 0) {
            if (right._sign != 0) {
                BigIntegerBuilder bigIntegerBuilder = new BigIntegerBuilder(left);
                BigIntegerBuilder bigIntegerBuilder1 = new BigIntegerBuilder(right);
                BigIntegerBuilder.GCD(ref bigIntegerBuilder, ref bigIntegerBuilder1);
                return bigIntegerBuilder.GetInteger(1);
            } else {
                return BigInteger.Abs(left);
            }
        } else {
            return BigInteger.Abs(right);
        }
    }

    internal static int Length(uint[] rgu) {
        int length = (int)rgu.Length;
        if (rgu[length - 1] == 0) {
            return length - 1;
        } else {
            return length;
        }
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static double Log(BigInteger value) {
        return BigInteger.Log(value, 2.71828182845905);
    }

    public static double Log(BigInteger value, double baseValue) {
        if (value._sign < 0 || baseValue == 1) {
            return double.NaN;
        } else {
            if (baseValue != double.PositiveInfinity) {
                if (baseValue != 0 || value.IsOne) {
                    if (value._bits != null) {
                        double num = 0;
                        double num1 = 0.5;
                        int num2 = BigInteger.Length(value._bits);
                        int num3 = BigInteger.BitLengthOfUInt(value._bits[num2 - 1]);
                        int num4 = (num2 - 1) * 32 + num3;
                        uint num5 = 1 << (num3 - 1 & 31);
                        for (int i = num2 - 1; i >= 0; i--) {
                            while (num5 != 0) {
                                if ((value._bits[i] & num5) != 0) {
                                    num = num + num1;
                                }
                                num1 = num1 * 0.5;
                                num5 = num5 >> 1;
                            }
                            num5 = -2147483648;
                        }
                        return (Math.Log(num) + 0.693147180559945 * (double)num4) / Math.Log(baseValue);
                    } else {
                        return Math.Log((double)value._sign, baseValue);
                    }
                } else {
                    return double.NaN;
                }
            } else {
                if (value.IsOne) {
                    return 0;
                } else {
                    return double.NaN;
                }
            }
        }
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static double Log10(BigInteger value) {
        return BigInteger.Log(value, 10);
    }

    public static BigInteger Max(BigInteger left, BigInteger right) {
        if (left.CompareTo(right) >= 0) {
            return left;
        } else {
            return right;
        }
    }

    public static BigInteger Min(BigInteger left, BigInteger right) {
        if (left.CompareTo(right) > 0) {
            return right;
        } else {
            return left;
        }
    }

    public static BigInteger ModPow(BigInteger value, BigInteger exponent, BigInteger modulus) {
        byte num;
        if (exponent.Sign >= 0) {
            int num1 = 1;
            int num2 = 1;
            int num3 = 1;
            bool isEven = exponent.IsEven;
            BigIntegerBuilder bigIntegerBuilder = new BigIntegerBuilder(BigInteger.One, ref num1);
            BigIntegerBuilder bigIntegerBuilder1 = new BigIntegerBuilder(value, ref num2);
            BigIntegerBuilder bigIntegerBuilder2 = new BigIntegerBuilder(modulus, ref num3);
            BigIntegerBuilder bigIntegerBuilder3 = new BigIntegerBuilder(bigIntegerBuilder1.Size);
            bigIntegerBuilder.Mod(ref bigIntegerBuilder2);
            if (exponent._bits != null) {
                int num4 = BigInteger.Length(exponent._bits);
                for (int i = 0; i < num4 - 1; i++) {
                    uint num5 = exponent._bits[i];
                    BigInteger.ModPowInner32(num5, ref bigIntegerBuilder, ref bigIntegerBuilder1, ref bigIntegerBuilder2, ref bigIntegerBuilder3);
                }
                BigInteger.ModPowInner(exponent._bits[num4 - 1], ref bigIntegerBuilder, ref bigIntegerBuilder1, ref bigIntegerBuilder2, ref bigIntegerBuilder3);
            } else {
                BigInteger.ModPowInner(exponent._sign, ref bigIntegerBuilder, ref bigIntegerBuilder1, ref bigIntegerBuilder2, ref bigIntegerBuilder3);
            }
            BigIntegerBuilder & bigIntegerBuilderPointer = &bigIntegerBuilder;
            if (value._sign > 0) {
                num = 1;
            } else {
                if (isEven) {
                    num = 1;
                } else {
                    num = -1;
                }
            }
            return bigIntegerBuilderPointer.GetInteger(num);
        } else {
            throw new ArgumentOutOfRangeException("exponent", SR.GetString("ArgumentOutOfRange_MustBeNonNeg"));
        }
    }

    private static void ModPowInner(uint exp, ref BigIntegerBuilder regRes, ref BigIntegerBuilder regVal, ref BigIntegerBuilder regMod, ref BigIntegerBuilder regTmp) {
        while (exp != 0) {
            if ((exp & 1) == 1) {
                BigInteger.ModPowUpdateResult(ref regRes, ref regVal, ref regMod, ref regTmp);
            }
            if (exp != 1) {
                BigInteger.ModPowSquareModValue(ref regVal, ref regMod, ref regTmp);
                exp = exp >> 1;
            } else {
                return;
            }
        }
    }

    private static void ModPowInner32(uint exp, ref BigIntegerBuilder regRes, ref BigIntegerBuilder regVal, ref BigIntegerBuilder regMod, ref BigIntegerBuilder regTmp) {
        for (int i = 0; i < 32; i++) {
            if ((exp & 1) == 1) {
                BigInteger.ModPowUpdateResult(ref regRes, ref regVal, ref regMod, ref regTmp);
            }
            BigInteger.ModPowSquareModValue(ref regVal, ref regMod, ref regTmp);
            exp = exp >> 1;
        }
    }

    private static void ModPowSquareModValue(ref BigIntegerBuilder regVal, ref BigIntegerBuilder regMod, ref BigIntegerBuilder regTmp)
    {
        NumericsHelpers.<BigIntegerBuilder>(ref regVal, ref regTmp);
        regVal.Mul(ref regTmp, ref regTmp);
        regVal.Mod(ref regMod);
    }

    private static void ModPowUpdateResult(ref BigIntegerBuilder regRes, ref BigIntegerBuilder regVal, ref BigIntegerBuilder regMod, ref BigIntegerBuilder regTmp)
    {
        NumericsHelpers.<BigIntegerBuilder>(ref regRes, ref regTmp);
        regRes.Mul(ref regTmp, ref regVal);
        regRes.Mod(ref regMod);
    }

    private static void MulLower(ref uint uHiRes, ref int cuRes, uint uHiMul, int cuMul) {
        ulong num = (ulong)uHiRes * (ulong)uHiMul;
        uint hi = NumericsHelpers.GetHi(num);
        if (hi == 0) {
            uHiRes = NumericsHelpers.GetLo(num);
            cuRes = cuRes + cuMul - 1;
            return;
        } else {
            uHiRes = hi;
            cuRes = cuRes + cuMul;
            return;
        }
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BigInteger Multiply(BigInteger left, BigInteger right) {
        return left * right;
    }

    private static void MulUpper(ref uint uHiRes, ref int cuRes, uint uHiMul, int cuMul) {
        ulong num = (ulong)uHiRes * (ulong)uHiMul;
        uint hi = NumericsHelpers.GetHi(num);
        if (hi == 0) {
            uHiRes = NumericsHelpers.GetLo(num);
            cuRes = cuRes + cuMul - 1;
            return;
        } else {
            if (NumericsHelpers.GetLo(num) != 0) {
                int num1 = hi + 1;
                hi = (uint)num1;
                if (num1 == 0) {
                    hi = 1;
                    cuRes = cuRes + 1;
                }
            }
            uHiRes = hi;
            cuRes = cuRes + cuMul;
            return;
        }
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BigInteger Negate(BigInteger value) {
        return -value;
    }

    public static BigInteger operator +(BigInteger left, BigInteger right) {
        if (!right.IsZero) {
            if (!left.IsZero) {
                int num = 1;
                int num1 = 1;
                BigIntegerBuilder bigIntegerBuilder = new BigIntegerBuilder(left, ref num);
                BigIntegerBuilder bigIntegerBuilder1 = new BigIntegerBuilder(right, ref num1);
                if (num != num1) {
                    bigIntegerBuilder.Sub(ref num, ref bigIntegerBuilder1);
                } else {
                    bigIntegerBuilder.Add(ref bigIntegerBuilder1);
                }
                return bigIntegerBuilder.GetInteger(num);
            } else {
                return right;
            }
        } else {
            return left;
        }
    }

    public static BigInteger operator &(BigInteger left, BigInteger right) {
        byte num;
        byte num1;
        uint num2;
        uint num3;
        if (left.IsZero || right.IsZero) {
            return BigInteger.Zero;
        } else {
            uint[] uInt32Array = left.ToUInt32Array();
            uint[] numArray = right.ToUInt32Array();
            uint[] numArray1 = new uint[Math.Max((int)uInt32Array.Length, (int)numArray.Length)];
            if (left._sign < 0) {
                num = -1;
            } else {
                num = 0;
            }
            uint num4 = (uint)num;
            if (right._sign < 0) {
                num1 = -1;
            } else {
                num1 = 0;
            }
            uint num5 = (uint)num1;
            for (int i = 0; i < (int)numArray1.Length; i++) {
                if (i < (int)uInt32Array.Length) {
                    num2 = uInt32Array[i];
                } else {
                    num2 = num4;
                }
                uint num6 = num2;
                if (i < (int)numArray.Length) {
                    num3 = numArray[i];
                } else {
                    num3 = num5;
                }
                uint num7 = num3;
                numArray1[i] = num6 & num7;
            }
            return new BigInteger(numArray1);
        }
    }

    public static BigInteger operator |(BigInteger left, BigInteger right) {
        byte num;
        byte num1;
        uint num2;
        uint num3;
        if (!left.IsZero) {
            if (!right.IsZero) {
                uint[] uInt32Array = left.ToUInt32Array();
                uint[] numArray = right.ToUInt32Array();
                uint[] numArray1 = new uint[Math.Max((int)uInt32Array.Length, (int)numArray.Length)];
                if (left._sign < 0) {
                    num = -1;
                } else {
                    num = 0;
                }
                uint num4 = (uint)num;
                if (right._sign < 0) {
                    num1 = -1;
                } else {
                    num1 = 0;
                }
                uint num5 = (uint)num1;
                for (int i = 0; i < (int)numArray1.Length; i++) {
                    if (i < (int)uInt32Array.Length) {
                        num2 = uInt32Array[i];
                    } else {
                        num2 = num4;
                    }
                    uint num6 = num2;
                    if (i < (int)numArray.Length) {
                        num3 = numArray[i];
                    } else {
                        num3 = num5;
                    }
                    uint num7 = num3;
                    numArray1[i] = num6 | num7;
                }
                return new BigInteger(numArray1);
            } else {
                return left;
            }
        } else {
            return right;
        }
    }

    public static BigInteger operator --(BigInteger value) {
        return value - BigInteger.One;
    }

    public static BigInteger operator /(BigInteger dividend, BigInteger divisor) {
        int num = 1;
        BigIntegerBuilder bigIntegerBuilder = new BigIntegerBuilder(dividend, ref num);
        BigIntegerBuilder bigIntegerBuilder1 = new BigIntegerBuilder(divisor, ref num);
        bigIntegerBuilder.Div(ref bigIntegerBuilder1);
        return bigIntegerBuilder.GetInteger(num);
    }

    public static bool operator ==(BigInteger left, BigInteger right) {
        return left.Equals(right);
    }

    public static bool operator ==(BigInteger left, long right) {
        return left.Equals(right);
    }

    public static bool operator ==(long left, BigInteger right) {
        return right.Equals(left);
    }

    [CLSCompliant(false)]
    public static bool operator ==(BigInteger left, ulong right) {
        return left.Equals(right);
    }

    [CLSCompliant(false)]
    public static bool operator ==(ulong left, BigInteger right) {
        return right.Equals(left);
    }

    public static BigInteger operator ^(BigInteger left, BigInteger right) {
        byte num;
        byte num1;
        uint num2;
        uint num3;
        uint[] uInt32Array = left.ToUInt32Array();
        uint[] numArray = right.ToUInt32Array();
        uint[] numArray1 = new uint[Math.Max((int)uInt32Array.Length, (int)numArray.Length)];
        if (left._sign < 0) {
            num = -1;
        } else {
            num = 0;
        }
        uint num4 = (uint)num;
        if (right._sign < 0) {
            num1 = -1;
        } else {
            num1 = 0;
        }
        uint num5 = (uint)num1;
        for (int i = 0; i < (int)numArray1.Length; i++) {
            if (i < (int)uInt32Array.Length) {
                num2 = uInt32Array[i];
            } else {
                num2 = num4;
            }
            uint num6 = num2;
            if (i < (int)numArray.Length) {
                num3 = numArray[i];
            } else {
                num3 = num5;
            }
            uint num7 = num3;
            numArray1[i] = num6 ^ num7;
        }
        return new BigInteger(numArray1);
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

    public static explicit operator Byte(BigInteger value) {
        return (byte)((int)value);
    }

    [CLSCompliant(false)]
    public static explicit operator SByte(BigInteger value) {
        return (sbyte)((int)value);
    }

    public static explicit operator Int16(BigInteger value) {
        return (short)((int)value);
    }

    [CLSCompliant(false)]
    public static explicit operator UInt16(BigInteger value) {
        return (ushort)((int)value);
    }

    public static explicit operator Int32(BigInteger value) {
        if (value._bits != null) {
            if (BigInteger.Length(value._bits) <= 1) {
                if (value._sign <= 0) {
                    if (value._bits[0] <= -2147483648) {
                        return -value._bits[0];
                    } else {
                        throw new OverflowException(SR.GetString("Overflow_Int32"));
                    }
                } else {
                    return (void*)((int)value._bits[0]);
                }
            } else {
                throw new OverflowException(SR.GetString("Overflow_Int32"));
            }
        } else {
            return value._sign;
        }
    }

    [CLSCompliant(false)]
    public static explicit operator UInt32(BigInteger value) {
        if (value._bits != null) {
            if (BigInteger.Length(value._bits) > 1 || value._sign < 0) {
                throw new OverflowException(SR.GetString("Overflow_UInt32"));
            } else {
                return value._bits[0];
            }
        } else {
            return (uint)value._sign;
        }
    }

    public static explicit operator Int64(BigInteger value) {
        ulong num;
        ulong num1;
        if (value._bits != null) {
            int num2 = BigInteger.Length(value._bits);
            if (num2 <= 2) {
                if (num2 <= 1) {
                    num = (ulong)value._bits[0];
                } else {
                    num = NumericsHelpers.MakeUlong(value._bits[1], value._bits[0]);
                }
                if (value._sign > 0) {
                    num1 = num;
                } else {
                    num1 = -num;
                }
                long num3 = (long)num1;
                if ((num3 <= (long)0 || value._sign <= 0) && (num3 >= (long)0 || value._sign >= 0)) {
                    throw new OverflowException(SR.GetString("Overflow_Int64"));
                } else {
                    return num3;
                }
            } else {
                throw new OverflowException(SR.GetString("Overflow_Int64"));
            }
        } else {
            return (long)value._sign;
        }
    }

    [CLSCompliant(false)]
    public static explicit operator UInt64(BigInteger value) {
        if (value._bits != null) {
            int num = BigInteger.Length(value._bits);
            if (num > 2 || value._sign < 0) {
                throw new OverflowException(SR.GetString("Overflow_UInt64"));
            } else {
                if (num <= 1) {
                    return (ulong)value._bits[0];
                } else {
                    return NumericsHelpers.MakeUlong(value._bits[1], value._bits[0]);
                }
            }
        } else {
            return (ulong)value._sign;
        }
    }

    public static explicit operator Single(BigInteger value) {
        return (float)((double)((double)value));
    }

    public static explicit operator Double(BigInteger value) {
        ulong num = 0L;
        int num1 = 0;
        if (value._bits != null) {
            int num2 = 1;
            BigIntegerBuilder bigIntegerBuilder = new BigIntegerBuilder(value, ref num2);
            bigIntegerBuilder.GetApproxParts(out num1, out num);
            return NumericsHelpers.GetDoubleFromParts(num2, num1, num);
        } else {
            return (double)value._sign;
        }
    }

    public static explicit operator Decimal(BigInteger value) {
        if (value._bits != null) {
            int num = BigInteger.Length(value._bits);
            if (num <= 3) {
                int num1 = 0;
                int num2 = 0;
                int num3 = 0;
                if (num > 2) {
                    num3 = value._bits[2];
                }
                if (num > 1) {
                    num2 = value._bits[1];
                }
                if (num > 0) {
                    num1 = value._bits[0];
                }
                return new decimal(num1, num2, num3, value._sign < 0, 0);
            } else {
                throw new OverflowException(SR.GetString("Overflow_Decimal"));
            }
        } else {
            return value._sign;
        }
    }

    public static bool operator >(BigInteger left, BigInteger right) {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >(BigInteger left, long right) {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >(long left, BigInteger right) {
        return right.CompareTo(left) < 0;
    }

    [CLSCompliant(false)]
    public static bool operator >(BigInteger left, ulong right) {
        return left.CompareTo(right) > 0;
    }

    [CLSCompliant(false)]
    public static bool operator >(ulong left, BigInteger right) {
        return right.CompareTo(left) < 0;
    }

    public static bool operator >=(BigInteger left, BigInteger right) {
        return left.CompareTo(right) >= 0;
    }

    public static bool operator >=(BigInteger left, long right) {
        return left.CompareTo(right) >= 0;
    }

    public static bool operator >=(long left, BigInteger right) {
        return right.CompareTo(left) <= 0;
    }

    [CLSCompliant(false)]
    public static bool operator >=(BigInteger left, ulong right) {
        return left.CompareTo(right) >= 0;
    }

    [CLSCompliant(false)]
    public static bool operator >=(ulong left, BigInteger right) {
        return right.CompareTo(left) <= 0;
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

    public static BigInteger operator ++(BigInteger value) {
        return value + BigInteger.One;
    }

    public static bool operator !=(BigInteger left, BigInteger right) {
        return !left.Equals(right);
    }

    public static bool operator !=(BigInteger left, long right) {
        return !left.Equals(right);
    }

    public static bool operator !=(long left, BigInteger right) {
        return !right.Equals(left);
    }

    [CLSCompliant(false)]
    public static bool operator !=(BigInteger left, ulong right) {
        return !left.Equals(right);
    }

    [CLSCompliant(false)]
    public static bool operator !=(ulong left, BigInteger right) {
        return !right.Equals(left);
    }

    public static BigInteger operator <<(BigInteger value, int shift) {
        uint[] numArray = 0;
        int num = 0;
        if (shift != 0) {
            if (shift != -2147483648) {
                if (shift >= 0) {
                    int num1 = shift / 32;
                    int num2 = shift - num1 * 32;
                    bool partsForBitManipulation = BigInteger.GetPartsForBitManipulation(ref value, out numArray, out num);
                    int num3 = num + num1 + 1;
                    uint[] numArray1 = new uint[num3];
                    if (num2 != 0) {
                        int num4 = 32 - num2;
                        uint num5 = 0;
                        for (int i = 0; i < num; i++) {
                            uint num6 = numArray[i];
                            numArray1[i + num1] = num6 << (num2 & 31) | num5;
                            num5 = num6 >> (num4 & 31);
                        }
                        numArray1[i + num1] = num5;
                    } else {
                        for (int j = 0; j < num; j++) {
                            numArray1[j + num1] = numArray[j];
                        }
                    }
                    return new BigInteger(numArray1, partsForBitManipulation);
                } else {
                    return value >> -shift;
                }
            } else {
                return value >> 2147483647 >> 1;
            }
        } else {
            return value;
        }
    }

    public static bool operator <(BigInteger left, BigInteger right) {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <(BigInteger left, long right) {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <(long left, BigInteger right) {
        return right.CompareTo(left) > 0;
    }

    [CLSCompliant(false)]
    public static bool operator <(BigInteger left, ulong right) {
        return left.CompareTo(right) < 0;
    }

    [CLSCompliant(false)]
    public static bool operator <(ulong left, BigInteger right) {
        return right.CompareTo(left) > 0;
    }

    public static bool operator <=(BigInteger left, BigInteger right) {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator <=(BigInteger left, long right) {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator <=(long left, BigInteger right) {
        return right.CompareTo(left) >= 0;
    }

    [CLSCompliant(false)]
    public static bool operator <=(BigInteger left, ulong right) {
        return left.CompareTo(right) <= 0;
    }

    [CLSCompliant(false)]
    public static bool operator <=(ulong left, BigInteger right) {
        return right.CompareTo(left) >= 0;
    }

    public static BigInteger operator %(BigInteger dividend, BigInteger divisor) {
        int num = 1;
        int num1 = 1;
        BigIntegerBuilder bigIntegerBuilder = new BigIntegerBuilder(dividend, ref num);
        BigIntegerBuilder bigIntegerBuilder1 = new BigIntegerBuilder(divisor, ref num1);
        bigIntegerBuilder.Mod(ref bigIntegerBuilder1);
        return bigIntegerBuilder.GetInteger(num);
    }

    public static BigInteger operator *(BigInteger left, BigInteger right) {
        int num = 1;
        BigIntegerBuilder bigIntegerBuilder = new BigIntegerBuilder(left, ref num);
        BigIntegerBuilder bigIntegerBuilder1 = new BigIntegerBuilder(right, ref num);
        bigIntegerBuilder.Mul(ref bigIntegerBuilder1);
        return bigIntegerBuilder.GetInteger(num);
    }

    public static BigInteger operator ~(BigInteger value) {
        return -(value + BigInteger.One);
    }

    public static BigInteger operator >>(BigInteger value, int shift) {
        uint[] numArray = 0;
        int num = 0;
        if (shift != 0) {
            if (shift != -2147483648) {
                if (shift >= 0) {
                    int num1 = shift / 32;
                    int num2 = shift - num1 * 32;
                    bool partsForBitManipulation = BigInteger.GetPartsForBitManipulation(ref value, out numArray, out num);
                    if (partsForBitManipulation) {
                        if (shift < 32 * num) {
                            uint[] numArray1 = new uint[num];
                            Array.Copy(numArray, numArray1, num);
                            numArray = numArray1;
                            NumericsHelpers.DangerousMakeTwosComplement(numArray);
                        } else {
                            return BigInteger.MinusOne;
                        }
                    }
                    int num3 = num - num1;
                    if (num3 < 0) {
                        num3 = 0;
                    }
                    uint[] numArray2 = new uint[num3];
                    if (num2 != 0) {
                        int num4 = 32 - num2;
                        uint num5 = 0;
                        for (int i = num - 1; i >= num1; i--) {
                            uint num6 = numArray[i];
                            if (!partsForBitManipulation || i != num - 1) {
                                numArray2[i - num1] = num6 >> (num2 & 31) | num5;
                            } else {
                                numArray2[i - num1] = num6 >> (num2 & 31) | -1 << (num4 & 31);
                            }
                            num5 = num6 << (num4 & 31);
                        }
                    } else {
                        for (int j = num - 1; j >= num1; j--) {
                            numArray2[j - num1] = numArray[j];
                        }
                    }
                    if (partsForBitManipulation) {
                        NumericsHelpers.DangerousMakeTwosComplement(numArray2);
                    }
                    return new BigInteger(numArray2, partsForBitManipulation);
                } else {
                    return value << -shift;
                }
            } else {
                return value << 2147483647 << 1;
            }
        } else {
            return value;
        }
    }

    public static BigInteger operator -(BigInteger left, BigInteger right) {
        if (!right.IsZero) {
            if (!left.IsZero) {
                int num = 1;
                int num1 = -1;
                BigIntegerBuilder bigIntegerBuilder = new BigIntegerBuilder(left, ref num);
                BigIntegerBuilder bigIntegerBuilder1 = new BigIntegerBuilder(right, ref num1);
                if (num != num1) {
                    bigIntegerBuilder.Sub(ref num, ref bigIntegerBuilder1);
                } else {
                    bigIntegerBuilder.Add(ref bigIntegerBuilder1);
                }
                return bigIntegerBuilder.GetInteger(num);
            } else {
                return -right;
            }
        } else {
            return left;
        }
    }

    public static BigInteger operator -(BigInteger value) {
        value._sign = -value._sign;
        return value;
    }

    public static BigInteger operator +(BigInteger value) {
        return value;
    }

    public static BigInteger Parse(string value) {
        return BigNumber.ParseBigInteger(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo);
    }

    public static BigInteger Parse(string value, NumberStyles style) {
        return BigNumber.ParseBigInteger(value, style, NumberFormatInfo.CurrentInfo);
    }

    public static BigInteger Parse(string value, IFormatProvider provider) {
        return BigNumber.ParseBigInteger(value, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider));
    }

    public static BigInteger Parse(string value, NumberStyles style, IFormatProvider provider) {
        return BigNumber.ParseBigInteger(value, style, NumberFormatInfo.GetInstance(provider));
    }

    public static BigInteger Pow(BigInteger value, int exponent)
    {
        if (exponent >= 0)
        {
            if (exponent != 0)
            {
                if (exponent != 1)
                {
                    if (value._bits == null)
                    {
                        if (value._sign != 1)
                        {
                            if (value._sign != -1)
                            {
                                if (value._sign == 0)
                                {
                                    return value;
                                }
                            }
                            else
                            {
                                if ((exponent & 1) != 0)
                                {
                                    return value;
                                }
                                else
                                {
                                    return 1;
                                }
                            }
                        }
                        else
                        {
                            return value;
                        }
                    }
                    int num = 1;
                    BigIntegerBuilder bigIntegerBuilder = new BigIntegerBuilder(value, ref num);
                    int size = bigIntegerBuilder.Size;
                    int num1 = size;
                    uint high = bigIntegerBuilder.High;
                    uint num2 = high + 1;
                    if (num2 == 0)
                    {
                        num1++;
                        num2 = 1;
                    }
                    int num3 = 1;
                    int num4 = 1;
                    uint num5 = 1;
                    uint num6 = 1;
                    int num7 = exponent;
                    while (true)
                    {
                        if ((num7 & 1) != 0)
                        {
                            BigInteger.MulUpper(ref num6, ref num4, num2, num1);
                            BigInteger.MulLower(ref num5, ref num3, high, size);
                        }
                        int num8 = num7 >> 1;
                        num7 = num8;
                        if (num8 == 0)
                        {
                            break;
                        }
                        BigInteger.MulUpper(ref num2, ref num1, num2, num1);
                        BigInteger.MulLower(ref high, ref size, high, size);
                    }
                    if (num4 > 1)
                    {
                        bigIntegerBuilder.EnsureWritable(num4, 0);
                    }
                    BigIntegerBuilder bigIntegerBuilder1 = new BigIntegerBuilder(num4);
                    BigIntegerBuilder bigIntegerBuilder2 = new BigIntegerBuilder(num4);
                    bigIntegerBuilder2.Set(1);
                    if ((exponent & 1) == 0)
                    {
                        num = 1;
                    }
                    int num9 = exponent;
                    while (true)
                    {
                        if ((num9 & 1) != 0)
                        {
                            NumericsHelpers.<BigIntegerBuilder>(ref bigIntegerBuilder2, ref bigIntegerBuilder1);
                            bigIntegerBuilder2.Mul(ref bigIntegerBuilder, ref bigIntegerBuilder1);
                        }
                        int num10 = num9 >> 1;
                        num9 = num10;
                        if (num10 == 0)
                        {
                            break;
                        }
                        NumericsHelpers.<BigIntegerBuilder>(ref bigIntegerBuilder, ref bigIntegerBuilder1);
                        bigIntegerBuilder.Mul(ref bigIntegerBuilder1, ref bigIntegerBuilder1);
                    }
                    return bigIntegerBuilder2.GetInteger(num);
                }
                else
                {
                    return value;
                }
            }
            else
            {
                return BigInteger.One;
            }
        }
        else
        {
            throw new ArgumentOutOfRangeException("exponent", SR.GetString("ArgumentOutOfRange_MustBeNonNeg"));
        }
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BigInteger Remainder(BigInteger dividend, BigInteger divisor) {
        return dividend % divisor;
    }

    private void SetBitsFromDouble(double value) {
        int num = 0;
        int num1 = 0;
        ulong num2 = 0L;
        bool flag;
        NumericsHelpers.GetDoubleParts(value, out num, out num1, out num2, out flag);
        if (num2 != (long)0) {
            if (num1 > 0) {
                if (num1 > 11) {
                    num2 = num2 << 11;
                    num1 = num1 - 11;
                    int num3 = (num1 - 1) / 32 + 1;
                    int num4 = num3 * 32 - num1;
                    this._bits = new uint[num3 + 2];
                    this._bits[num3 + 1] = (uint)(num2 >> (num4 + 32 & 63));
                    this._bits[num3] = (uint)(num2 >> (num4 & 63));
                    if (num4 > 0) {
                        this._bits[num3 - 1] = (uint)num2 << (32 - num4 & 31);
                    }
                    this._sign = num;
                } else {
                    *(this) = num2 << (num1 & 63);
                    if (num < 0) {
                        this._sign = -this._sign;
                        return;
                    }
                }
            } else {
                if (num1 > -64) {
                    *(this) = num2 >> (-num1 & 63);
                    if (num < 0) {
                        this._sign = -this._sign;
                        return;
                    }
                } else {
                    *(this) = BigInteger.Zero;
                    return;
                }
            }
            return;
        } else {
            *(this) = BigInteger.Zero;
            return;
        }
    }

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static BigInteger Subtract(BigInteger left, BigInteger right) {
        return left - right;
    }

    public byte[] ToByteArray() {
        uint[] numArray;
        byte num;
        int num1;
        byte num2;
        if (this._bits != null || this._sign != 0) {
            if (this._bits != null) {
                if (this._sign != -1) {
                    numArray = this._bits;
                    num = 0;
                } else {
                    numArray = (uint[])this._bits.Clone();
                    NumericsHelpers.DangerousMakeTwosComplement(numArray);
                    num = 255;
                }
            } else {
                uint[] numArray1 = new uint[1];
                numArray1[0] = this._sign;
                numArray = numArray1;
                if (this._sign < 0) {
                    num2 = 255;
                } else {
                    num2 = 0;
                }
                num = (byte)num2;
            }
            byte[] numArray2 = new byte[4 * (int)numArray.Length];
            int num3 = 0;
            for (int i = 0; i < (int)numArray.Length; i++) {
                uint num4 = numArray[i];
                for (int j = 0; j < 4; j++) {
                    int num5 = num3;
                    num3 = num5 + 1;
                    numArray2[num5] = (byte)(num4 & 255);
                    num4 = num4 >> 8;
                }
            }
            int length = (int)numArray2.Length - 1;
            while (length > 0 && numArray2[length] == num) {
                length--;
            }
            bool flag = (numArray2[length] & 128) != (num & 128);
            int num6 = length + 1;
            if (flag) {
                num1 = 1;
            } else {
                num1 = 0;
            }
            byte[] numArray3 = new byte[num6 + num1];
            Array.Copy(numArray2, numArray3, length + 1);
            if (flag) {
                numArray3[(int)numArray3.Length - 1] = num;
            }
            return numArray3;
        } else {
            byte[] numArray4 = new byte[1];
            return numArray4;
        }
    }

    public override string ToString() {
        return BigNumber.FormatBigInteger(this, null, NumberFormatInfo.CurrentInfo);
    }

    public string ToString(IFormatProvider provider) {
        return BigNumber.FormatBigInteger(this, null, NumberFormatInfo.GetInstance(provider));
    }

    public string ToString(string format) {
        return BigNumber.FormatBigInteger(this, format, NumberFormatInfo.CurrentInfo);
    }

    public string ToString(string format, IFormatProvider provider) {
        return BigNumber.FormatBigInteger(this, format, NumberFormatInfo.GetInstance(provider));
    }

    private uint[] ToUInt32Array() {
        uint[] numArray;
        uint num;
        int num1;
        byte num2;
        if (this._bits != null || this._sign != 0) {
            if (this._bits != null) {
                if (this._sign != -1) {
                    numArray = this._bits;
                    num = 0;
                } else {
                    numArray = (uint[])this._bits.Clone();
                    NumericsHelpers.DangerousMakeTwosComplement(numArray);
                    num = -1;
                }
            } else {
                uint[] numArray1 = new uint[1];
                numArray1[0] = this._sign;
                numArray = numArray1;
                if (this._sign < 0) {
                    num2 = -1;
                } else {
                    num2 = 0;
                }
                num = (uint)num2;
            }
            int length = (int)numArray.Length - 1;
            while (length > 0 && numArray[length] == num) {
                length--;
            }
            bool flag = (numArray[length] & -2147483648) != (num & -2147483648);
            int num3 = length + 1;
            if (flag) {
                num1 = 1;
            } else {
                num1 = 0;
            }
            uint[] numArray2 = new uint[num3 + num1];
            Array.Copy(numArray, numArray2, length + 1);
            if (flag) {
                numArray2[(int)numArray2.Length - 1] = num;
            }
            return numArray2;
        } else {
            uint[] numArray3 = new uint[1];
            return numArray3;
        }
    }

    public static bool TryParse(string value, out BigInteger result) {
        return BigNumber.TryParseBigInteger(value, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out result);
    }

    public static bool TryParse(string value, NumberStyles style, IFormatProvider provider, out BigInteger result) {
        return BigNumber.TryParseBigInteger(value, style, NumberFormatInfo.GetInstance(provider), out result);
    }
}
