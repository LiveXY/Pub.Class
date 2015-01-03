using System;

namespace Skyiv {
    static class Utility {
        public static T[] Expand<T>(T[] x, int n) {
            T[] z = new T[n]; // assume n >= x.Length
            Array.Copy(x, 0, z, n - x.Length, x.Length);
            return z;
        }

        public static void Swap<T>(ref T x, ref T y) {
            T z = x;
            x = y;
            y = z;
        }
    }
}