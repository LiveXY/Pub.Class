using System;
using System.Diagnostics;

namespace Skyiv.Numeric {
    /// <summary>
    /// 提供任意精度的算术运算的静态类。使用快速傅里叶变换。
    /// 本类对字节数组进行算术运算，字节数组以 100 为基。
    /// 字节数组中第一个元素存储的数字是最高有效位。
    /// </summary>
    static class BigArithmetic {
        //= C语言数值算法程序大全(第二版)，ISBN 7-5053-2931-6 / TP 993
        //= Numerical Recipes in C, The Art of Scientific Computing, Second Edition
        //= Cambridge University Press 1988, 1992
        //= [美] W.H.Press, S.A.Teukolsky, W.T.Vetterling, B.P.Flannery 著
        //= 傅祖芸 赵梅娜 丁岩 等译，傅祖芸 校，电子工业出版社，1995年10月第一版

        static readonly byte Len = 2; // 字节数组的元素包含的十进制数字的个数
        static readonly byte Base = (byte)Math.Pow(10, Len); // 字节数组的基
        static readonly byte MaxValue = (byte)(Base - 1);    // 字节数组的元素的最大值

        //= pp.431-432, four1, 12.2 快速傅里叶变换(FFT)
        /// <summary>
        /// 复函数的快速傅里叶变换
        /// 变量 nn 是复数据点的个数，实型数组 data[1..2*nn] 的实际界长是两倍 nn，
        /// 而每个复数值占据了两个相继的存储单元。nn 必须是 2 的整数幂
        /// </summary>
        /// <param name="data">实型数组 data[1..2*nn]。注意，下标从 1 开始</param>
        /// <param name="isInverse">是否逆变换。注意: 逆变换未乘上归一化因子 1/nn</param>
        public static void ComplexFFT(double[] data, bool isInverse) {
            int n = data.Length - 1; // n 必须是 2 的正整数幂
            int nn = n >> 1;         // 变量 nn 是复数据点的个数
            for (int i = 1, j = 1; i < n; i += 2) // 这个循环实现位序颠倒
      {
                if (j > i) {
                    Utility.Swap(ref data[j], ref data[i]);
                    Utility.Swap(ref data[j + 1], ref data[i + 1]);
                }
                int m = nn;
                for (; m >= 2 && j > m; m >>= 1) j -= m;
                j += m;
            }
            for (int mmax = 2, istep = 4; n > mmax; mmax = istep) // 执行 log2(nn) 次外循环
      {
                istep = mmax << 1; // 下面是关于三角递归的初始赋值
                double theta = (isInverse ? -2 : 2) * Math.PI / mmax;
                double wtemp = Math.Sin(0.5 * theta);
                double wpr = -2 * wtemp * wtemp;
                double wpi = Math.Sin(theta);
                double wr = 1;
                double wi = 0;
                for (int m = 1; m < mmax; m += 2) {
                    for (int i = m; i <= n; i += istep) {
                        int j = i + mmax; // 下面是 Danielson-Lanczos 公式
                        double tempr = wr * data[j] - wi * data[j + 1];
                        double tempi = wr * data[j + 1] + wi * data[j];
                        data[j] = data[i] - tempr;
                        data[j + 1] = data[i + 1] - tempi;
                        data[i] += tempr;
                        data[i + 1] += tempi;
                    }
                    wr = (wtemp = wr) * wpr - wi * wpi + wr; // 三角递归
                    wi = wi * wpr + wtemp * wpi + wi;
                }
            }
        }

        //= pp.436, realft, 12.3.2 单个实函数的 FFT
        /// <summary>
        /// 单个实函数的快速傅里叶变换
        /// 计算一组 n 个实值数据点的傅里叶变换。用复傅里叶变换的正半频率替换这些数据，
        /// 它存储在数组 data[1..n] 中。复变换的第一个和最后一个分量的实数值分别返回
        /// 单元 data[1] 和 data[2] 中。n 必须是 2 的幂次。这个程序也能计算复数据数组
        /// 的逆变换，只要该数组是实值数据的变换(在这种情况下，其结果必须乘以 1/n)即可。
        /// </summary>
        /// <param name="data">实型数组 data[1..n]。注意，下标从 1 开始</param>
        /// <param name="isInverse">是否逆变换。注意: 逆变换未乘上归一化因子 1/n</param>
        public static void RealFFT(double[] data, bool isInverse) {
            int n = data.Length - 1; // n 必须是 2 的整数幂
            if (!isInverse) ComplexFFT(data, isInverse); // 此处是正向变换
            double theta = (isInverse ? -2 : 2) * Math.PI / n; // 递归的初始赋值
            double wtemp = Math.Sin(0.5 * theta);
            double wpr = -2 * wtemp * wtemp;
            double wpi = Math.Sin(theta);
            double wr = 1 + wpr;
            double wi = wpi;
            double c1 = 0.5;
            double c2 = isInverse ? 0.5 : -0.5;
            int n3 = n + 3;
            int n4 = n >> 2;
            for (int i = 2; i <= n4; i++) {
                int i1 = i + i - 1, i2 = i1 + 1, i3 = n3 - i2, i4 = i3 + 1;
                double h1r = c1 * (data[i1] + data[i3]); // 两个分离变换是
                double h1i = c1 * (data[i2] - data[i4]); // 从 data 中分离出来
                double h2r = -c2 * (data[i2] + data[i4]);
                double h2i = c2 * (data[i1] - data[i3]);
                data[i1] = h1r + wr * h2r - wi * h2i; // 此处重新组合以形成
                data[i2] = h1i + wr * h2i + wi * h2r; // 原始实型数据的真实变换
                data[i3] = h1r - wr * h2r + wi * h2i;
                data[i4] = -h1i + wr * h2i + wi * h2r;
                wr = (wtemp = wr) * wpr - wi * wpi + wr; // 递归式
                wi = wi * wpr + wtemp * wpi + wi;
            }
            double tmp = data[1];
            if (!isInverse) {
                data[1] = tmp + data[2]; // 同时挤压第一个和最后一个数据
                data[2] = tmp - data[2]; // 使它们都在原始数组中
            } else {
                data[1] = c1 * (tmp + data[2]);
                data[2] = c1 * (tmp - data[2]);
                ComplexFFT(data, isInverse); // 此处是逆变换
            }
        }

        /// <summary>
        /// 比较 x[0..n-1] 和 y[0..n-1]
        /// </summary>
        /// <param name="x">第一操作数 x[0..n-1]</param>
        /// <param name="y">第二操作数 y[0..n-1]</param>
        /// <param name="n">两个操作数 x 和 y 的精度</param>
        /// <returns>比较结果：-1:小于 1:大于 0:等于</returns>
        public static int Compare(byte[] x, byte[] y, int n) {
            Debug.Assert(x.Length >= n && y.Length >= n);
            for (int i = 0; i < n; i++)
                if (x[i] != y[i])
                    return (x[i] < y[i]) ? -1 : 1;
            return 0;
        }

        //= pp.775, mpneg, 20.6 任意精度的运算
        /// <summary>
        /// 求补码。注意，被操作数被修改。
        /// </summary>
        /// <param name="data">被操作数 data[0..n-1]</param>
        /// <param name="n">被操作数 data 的精度</param>
        /// <returns>被操作数的补码 data[0..n-1]</returns>
        public static byte[] Negative(byte[] data, int n) {
            Debug.Assert(data.Length >= n);
            for (int k = Base, i = n - 1; i >= 0; i--)
                data[i] = (byte)((k = MaxValue + k / Base - data[i]) % Base);
            return data;
        }

        //= pp.774, mpsub, 20.6 任意精度的运算
        /// <summary>
        /// 减法。从 minuend[0..n-1] 中减去 subtrahend[0..n-1]，得到 difference[0..n-1]
        /// </summary>
        /// <param name="difference">差 difference[0..n-1]</param>
        /// <param name="minuend">被减数 minuend[0..n-1]</param>
        /// <param name="subtrahend">减数 subtrahend[0..n-1]</param>
        /// <param name="n">被减数 minuend 和减数 subtrahend 的精度</param>
        /// <returns>差 difference[0..n-1]</returns>
        public static byte[] Subtract(byte[] difference, byte[] minuend, byte[] subtrahend, int n) {
            Debug.Assert(minuend.Length >= n && subtrahend.Length >= n && difference.Length >= n);
            for (int k = Base, i = n - 1; i >= 0; i--)
                difference[i] = (byte)((k = MaxValue + k / Base + minuend[i] - subtrahend[i]) % Base);
            return difference;
        }

        //= pp.774, mpadd, 20.6 任意精度的运算
        /// <summary>
        /// 加法。augend[0..n-1] 与 addend[0..n-1] 相加，得到 sum[0..n]
        /// </summary>
        /// <param name="sum">和 sum[0..n]</param>
        /// <param name="augend">被加数 augend[0..n-1]</param>
        /// <param name="addend">加数 addend[0..n-1]</param>
        /// <param name="n">被加数 augend 和加数 addend 的精度</param>
        /// <returns>和 sum[0..n]</returns>
        public static byte[] Add(byte[] sum, byte[] augend, byte[] addend, int n) {
            Debug.Assert(augend.Length >= n && addend.Length >= n && sum.Length >= n + 1);
            int k = 0;
            for (int i = n - 1; i >= 0; i--)
                sum[i + 1] = (byte)((k = k / Base + augend[i] + addend[i]) % Base);
            sum[0] += (byte)(k / Base);
            return sum;
        }

        //= pp.774, mpadd, 20.6 任意精度的运算
        /// <summary>
        /// 捷加法。augend[0..n-1] 与整数 addend 相加，得到 sum[0..n]
        /// </summary>
        /// <param name="sum">和 sum[0..n]</param>
        /// <param name="augend">被加数 augend[0..n-1]</param>
        /// <param name="n">被加数 augend 的精度</param>
        /// <param name="addend">加数 addend</param>
        /// <returns>和 sum[0..n]</returns>
        public static byte[] Add(byte[] sum, byte[] augend, int n, byte addend) {
            Debug.Assert(augend.Length >= n && sum.Length >= n + 1);
            int k = Base * addend;
            for (int i = n - 1; i >= 0; i--)
                sum[i + 1] = (byte)((k = k / Base + augend[i]) % Base);
            sum[0] += (byte)(k / Base);
            return sum;
        }

        //= pp.775, mpsdv, 20.6 任意精度的运算
        /// <summary>
        /// 捷除法。dividend[0..n-1] 除以整数 divisor，得到 quotient[0..n-1]
        /// </summary>
        /// <param name="quotient">商 quotient[0..n-1]</param>
        /// <param name="dividend">被除数 dividend[0..n-1]</param>
        /// <param name="n">被除数 dividend 的精度</param>
        /// <param name="divisor">除数 divisor</param>
        /// <returns>商 quotient[0..n-1]</returns>
        public static byte[] Divide(byte[] quotient, byte[] dividend, int n, byte divisor) {
            Debug.Assert(quotient.Length >= n && dividend.Length >= n);
            for (int r = 0, k = 0, i = 0; i < n; i++, r = k % divisor)
                quotient[i] = (byte)((k = Base * r + dividend[i]) / divisor);
            return quotient;
        }

        //= pp.776-777, mpmul, 20.6 任意精度的运算
        /// <summary>
        /// 乘法。multiplicand[0..n-1] 与 multiplier[0..m-1] 相乘，得到 product[0..n+m-1]
        /// </summary>
        /// <param name="product">积 product[0..n+m-1]</param>
        /// <param name="multiplicand">被乘数 multiplicand[0..n-1]</param>
        /// <param name="n">被乘数 multiplicand 的精度</param>
        /// <param name="multiplier">乘数 multiplier[0..m-1]</param>
        /// <param name="m">乘数 multiplier 的精度</param>
        /// <returns>积 product[0..n+m-1]</returns>
        public static byte[] Multiply(byte[] product, byte[] multiplicand, int n, byte[] multiplier, int m) {
            int mn = m + n, nn = 1;
            Debug.Assert(product.Length >= mn && multiplicand.Length >= n && multiplier.Length >= m);
            while (nn < mn) nn <<= 1; // 为变换找出最小可用的 2 的幂次
            double[] a = new double[nn + 1], b = new double[nn + 1];
            for (int i = 0; i < n; i++) a[i + 1] = multiplicand[i];
            for (int i = 0; i < m; i++) b[i + 1] = multiplier[i];
            RealFFT(a, false); // 执行卷积，首先求出二个傅里叶变换
            RealFFT(b, false);
            b[1] *= a[1]; // 复数相乘的结果(实部和虚部)
            b[2] *= a[2];
            for (int i = 3; i <= nn; i += 2) {
                double t = b[i];
                b[i] = t * a[i] - b[i + 1] * a[i + 1];
                b[i + 1] = t * a[i + 1] + b[i + 1] * a[i];
            }
            RealFFT(b, true); // 进行傅里叶逆变换
            byte[] bs = new byte[nn + 1];
            long cy = 0; // 执行最后完成所有进位的过程
            for (int i = nn, n2 = nn / 2; i >= 1; i--) {
                long t = (long)(b[i] / n2 + cy + 0.5);
                bs[i] = (byte)(t % Base); // 原书中这句使用循环，有严重的性能问题
                cy = t / Base;
            }
            if (cy >= Base) throw new OverflowException("FFT Multiply");
            bs[0] = (byte)cy;
            Array.Copy(bs, product, n + m);
            return product;
        }

        //= pp.778, mpdiv, 20.6 任意精度的运算
        /// <summary>
        /// 除法。dividend[0..n-1] 除以 divisor[0..m-1]，m ≤ n，
        /// 得到：商 quotient[0..n-m]，余数 remainder[0..m-1]。
        /// </summary>
        /// <param name="quotient">商 quotient[0..n-m]</param>
        /// <param name="remainder">余数 remainder[0..m-1]</param>
        /// <param name="dividend">被除数 dividend[0..n-1]</param>
        /// <param name="n">被除数 dividend 的精度</param>
        /// <param name="divisor">除数 divisor[0..m-1]</param>
        /// <param name="m">除数 divisor 的精度</param>
        /// <returns>商 quotient[0..n-m]</returns>
        public static byte[] DivRem(byte[] quotient, byte[] remainder, byte[] dividend, int n, byte[] divisor, int m) {
            Debug.Assert(m <= n && dividend.Length >= n && divisor.Length >= m && quotient.Length >= n - m + 1 && remainder.Length >= m);
            int MACC = 3;
            byte[] s = new byte[n + MACC], t = new byte[n - m + MACC + n];
            Inverse(s, n - m + MACC, divisor, m); // s = 1 / divisor
            Array.Copy(Multiply(t, s, n - m + MACC, dividend, n), 1, quotient, 0, n - m + 1); // quotient = dividend / divisor
            Array.Copy(Multiply(t, quotient, n - m + 1, divisor, m), 1, s, 0, n); //  s = quotient * divisor
            Subtract(s, dividend, s, n); // s = dividend - quotient * divisor
            Array.Copy(s, n - m, remainder, 0, m);
            if (Compare(remainder, divisor, m) >= 0) // 调整商和余数
      {
                Subtract(remainder, remainder, divisor, m);
                Add(s, quotient, n - m + 1, 1);
                Array.Copy(s, 1, quotient, 0, n - m + 1);
            }
            return quotient;
        }

        //= pp.777 - 778, mpinv, 20.6 任意精度的运算
        /// <summary>
        /// 求倒数。
        /// </summary>
        /// <param name="inverse">倒数 inverse[0..n-1]，在 inverse[0] 后有基数的小数点</param>
        /// <param name="n">倒数 inverse 的精度</param>
        /// <param name="data">被操作数 data[0..m-1]，data[0] > 0，在 data[0] 后有基数的小数点</param>
        /// <param name="m">被操作数 data 的精度</param>
        /// <returns>倒数 inverse[0..n-1]，在 inverse[0] 后有基数的小数点</returns>
        public static byte[] Inverse(byte[] inverse, int n, byte[] data, int m) {
            Debug.Assert(inverse.Length >= n && data.Length >= m);
            InitialValue(inverse, n, data, m, false);
            if (n == 1) return inverse;
            byte[] s = new byte[n], t = new byte[n + n];
            for (; ; ) // 牛顿迭代法: inverse = inverse * ( 2 - data * inverse )  =>  inverse = 1 / data
      {
                Array.Copy(Multiply(t, inverse, n, data, m), 1, s, 0, n); // s = data * inverse
                Negative(s, n);                                         // s = -(data * inverse)
                s[0] -= (byte)(Base - 2);                             // s = 2 - data * inverse
                Array.Copy(Multiply(t, s, n, inverse, n), 1, inverse, 0, n); // inverse = inverse * s
                int i = 1;
                for (; i < n - 1 && s[i] == 0; i++) ; // 判断 s 的小数部分是否为零
                if (i == n - 1) return inverse; // 若 s 收敛到 1 则返回 inverse = 1 / data
            }
        }

        //= pp.778-779, mpsqrt, 20.6 任意精度的运算
        /// <summary>
        /// 求平方根 sqrt，以及平方根的倒数 invSqrt。invSqrt 也可设为 sqrt，此时，invSqrt 也是平方根。
        /// </summary>
        /// <param name="sqrt">平方根 sqrt[0..n-1]，在 sqrt[0] 后有基数的小数点</param>
        /// <param name="invSqrt">平方根的倒数 invSqrt[0..n-1]，在 invSqrt[0] 后有基数的小数点</param>
        /// <param name="n">平方根的精度</param>
        /// <param name="data">被操作数 data[0..m-1]，data[0] > 0，在 data[0] 后有基数的小数点</param>
        /// <param name="m">被操作数 data 的精度</param>
        /// <returns>平方根 sqrt[0..n-1]，在 sqrt[0] 后有基数的小数点</returns>
        public static byte[] Sqrt(byte[] sqrt, byte[] invSqrt, int n, byte[] data, int m) {
            Debug.Assert(sqrt.Length >= n && invSqrt.Length >= n && data.Length >= m);
            if (n <= 1) throw new ArgumentOutOfRangeException("n", "must greater than 1");
            InitialValue(invSqrt, n, data, m, true);
            byte[] s = new byte[n], t = new byte[n + Math.Max(m, n)];
            for (; ; ) // invSqrt = invSqrt * (3 - data * invSqrt * invSqrt) / 2 => invSqrt = 1 / sqrt(data)
      {
                Array.Copy(Multiply(t, invSqrt, n, invSqrt, n), 1, s, 0, n); // s = invSqrt * invSqrt
                Array.Copy(Multiply(t, s, n, data, m), 1, s, 0, n);   // s = data * invSqrt * invSqrt
                Negative(s, n);                                     // s = -(data * invSqrt * invSqrt)
                s[0] -= (byte)(Base - 3);                         // s = 3 - data * invSqrt * invSqrt
                Divide(s, s, n, 2);                              // s = (3 - data * invSqrt * invSqrt) / 2
                Array.Copy(Multiply(t, s, n, invSqrt, n), 1, invSqrt, 0, n);   // invSqrt = invSqrt * s
                int i = 1;
                for (; i < n - 1 && s[i] == 0; i++) ; // 判断 s 的小数部分是否为零
                if (i < n - 1) continue; // 若 s 没有收敛到 1 则继续迭代
                Array.Copy(Multiply(t, invSqrt, n, data, m), 1, sqrt, 0, n); // sqrt = invSqrt * data = sqrt(data)
                return sqrt;
            }
        }

        /// <summary>
        /// 采用浮点运算以得到一个初始近似值 u[0..n-1]: u = 1 / data 或者 u = 1 / sqrt(data)
        /// </summary>
        /// <param name="u">初始近似值 u[0..n-1]</param>
        /// <param name="n">所需的精度</param>
        /// <param name="data">被操作数 data[0..m-1]</param>
        /// <param name="m">被操作数 data 的精度</param>
        /// <param name="isSqrt">是否求平方根</param>
        /// <returns>初始近似值 u[0..n-1]</returns>
        static byte[] InitialValue(byte[] u, int n, byte[] data, int m, bool isSqrt) {
            Debug.Assert(u.Length >= n && data.Length >= m);
            int scale = 16 / Len; // double 可达到 16 位有效数字
            double fu = 0;
            for (int i = Math.Min(scale, m) - 1; i >= 0; i--) fu = fu / Base + data[i];
            fu = 1 / (isSqrt ? Math.Sqrt(fu) : fu);
            for (int i = 0; i < Math.Min(scale + 1, n); i++) {
                int k = (int)fu;
                u[i] = (byte)k;
                fu = Base * (fu - k);
            }
            return u;
        }
    }
}