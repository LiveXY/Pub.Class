using System;

namespace Skyiv.Numeric {
    /// <summary>
    /// 圆周率
    /// </summary>
    static class Pi {
        //= pp.780-781, mppi, 20.6 任意精度的运算
        /// <summary>
        /// 计算圆周率到小数点后 digits 位数字。
        /// 计算结果保存在返回的字节数组中，每个字节存放两个十进制数字，字节数组的第一个元素是“03”。
        /// 字节数组的长度可能大于 (digits + 1) / 2 + 1，只保证小数点后前 digits 个十进制数字是准确的。
        /// </summary>
        /// <param name="digits">小数点后的十进制数字个数</param>
        /// <returns>存放圆周率的字节数组</returns>
        public static byte[] Compute(int digits) {
            if (digits < 0) throw new ArgumentOutOfRangeException("digits", "can't less than zero");
            int n = Math.Max(5, (digits + 1) / 2 + 2);
            byte[] pi = new byte[n + 1];
            byte[] x = new byte[n + 1], y = new byte[n << 1];
            byte[] sx = new byte[n], sxi = new byte[n];
            byte[] t = new byte[n << 1], s = new byte[3 * n];
            t[0] = 2;                             // t = 2
            BigArithmetic.Sqrt(x, x, n, t, n);    // x = sqrt(2)
            BigArithmetic.Add(pi, t, x, n);       // pi = 2 + sqrt(2)
            Array.Copy(pi, 1, pi, 0, n);
            BigArithmetic.Sqrt(sx, sxi, n, x, n); // sx = sqrt(x)
            Array.Copy(sx, y, n);                 // y = sqrt(x)
            for (; ; ) {
                BigArithmetic.Add(x, sx, sxi, n);       // x = sqrt(x) + 1/sqrt(x)
                Array.Copy(x, 1, x, 0, n);
                BigArithmetic.Divide(x, x, n, 2);       // x = x / 2
                BigArithmetic.Sqrt(sx, sxi, n, x, n);   // sx = sqrt(x), sxi = 1/sqrt(x)
                BigArithmetic.Multiply(t, y, n, sx, n); // t = y * sx
                Array.Copy(t, 1, t, 0, n);
                BigArithmetic.Add(t, t, sxi, n);        // t = y * sx + sxi
                x[0]++;
                y[0]++;
                BigArithmetic.Inverse(s, n, y, n);      // s = 1 / (y + 1)
                Array.Copy(t, 1, t, 0, n);
                BigArithmetic.Multiply(y, t, n, s, n);  // y = t / (y + 1)
                Array.Copy(y, 1, y, 0, n);
                BigArithmetic.Multiply(t, x, n, s, n);  // t = (x + 1) / (y + 1)
                int mm = t[1] - 1;                      // 若 t == 1 则收敛
                int j = t[n] - mm;
                if (j > 1 || j < -1) {
                    for (j = 2; j < n; j++) {
                        if (t[j] != mm) {
                            Array.Copy(t, 1, t, 0, n);
                            BigArithmetic.Multiply(s, pi, n, t, n); // s = t * pi
                            Array.Copy(s, 1, pi, 0, n);             // pi = t * pi
                            break;
                        }
                    }
                    if (j < n) continue;
                }
                break;
            }
            return pi;
        }
    }
}