﻿using System.Numerics;

namespace aoc2024.Util;

public static class Math
{
    public static T GCD<T>(T a, T b) where T : IBinaryInteger<T>
    {
        while (true)
        {
            if (b == T.Zero)
            {
                return a;
            }

            var a1 = a;
            a = b;
            b = a1 % b;
        }
    }

    public static T LCM<T>(params T[] nums) where T : IBinaryInteger<T>
    {
        var num = nums.Length;
        switch (num)
        {
            case 0:
                return T.Zero;
            case 1:
                return nums[0];
        }

        var ret = lcm(nums[0], nums[1]);
        for (var i = 2; i < num; i++)
        {
            ret = lcm(nums[i], ret);
        }

        return ret;
    }

    private static T lcm<T>(T a, T b) where T : IBinaryInteger<T>
    {
        return (a * b) / GCD(a, b);
    }

    public static long Modulo(long numer, long denom)
    {
        // long q = numer / denom;
        long r = numer % denom;
        if (r < 0)
        {
            if (denom > 0)
            {
                // q = q - 1;
                r = r + denom;
            }
            else
            {
                // q = q + 1;
                r = r - denom;
            }
        }

        return r;
    }

    // taken from https://rosettacode.org/wiki/Chinese_remainder_theorem#C.23
    public static T CRT<T>(T[] n, T[] a) where T : IBinaryInteger<T>
    {
        T prod = n.Aggregate(T.One, (i, j) => i * j);
        T p;
        T sm = T.Zero;
        for (int i = 0; i < n.Length; i++)
        {
            p = prod / n[i];
            sm += a[i] * ModularMultiplicativeInverse(p, n[i]) * p;
        }

        return sm % prod;
    }

    public static T ModularMultiplicativeInverse<T>(T a, T mod) where T : IBinaryInteger<T>
    {
        T b = a % mod;
        for (T x = T.One; x < mod; x++)
        {
            if ((b * x) % mod == T.One)
            {
                return x;
            }
        }

        return T.One;
    }
}