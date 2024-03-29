﻿using System;
using System.Collections.Generic;
using static System.Double;
using static System.Math;

namespace Imagin.Core.Colors;

[Category(Class.LCHuv), Serializable]
public abstract class HLuv : ColorModel3<LCHuv>
{
    protected static double[][] M = new double[][]
   {
        new double[] {  3.240969941904521, -1.537383177570093, -0.498610760293    },
        new double[] { -0.96924363628087,   1.87596750150772,   0.041555057407175 },
        new double[] {  0.055630079696993, -0.20397695888897,   1.056971514242878 },
   };

    ///

    public HLuv() : base() { }

    ///

    protected static IList<double[]> GetBounds(double L)
    {
        var result = new List<double[]>();

        double sub1 = Pow(L + 16, 3) / 1560896;
        double sub2 = sub1 > CIE.IEpsilon ? sub1 : L / CIE.IKappa;

        for (int c = 0; c < 3; ++c)
        {
            var m1 = M[c][0];
            var m2 = M[c][1];
            var m3 = M[c][2];

            for (int t = 0; t < 2; ++t)
            {
                var top1 = (284517 * m1 - 94839 * m3) * sub2;
                var top2 = (838422 * m3 + 769860 * m2 + 731718 * m1) * L * sub2 - 769860 * t * L;
                var bottom = (632260 * m3 - 126452 * m2) * sub2 + 126452 * t;

                result.Add(new double[] { top1 / bottom, top2 / bottom });
            }
        }

        return result;
    }

    protected static double GetChroma(double L)
    {
        var bounds = GetBounds(L);
        double min = MaxValue;

        for (int i = 0; i < 2; ++i)
        {
            var m1 = bounds[i][0];
            var b1 = bounds[i][1];
            var line = new double[] { m1, b1 };

            double x = GetIntersection(line, new double[] { -1 / m1, 0 });
            double length = GetDistance(new double[] { x, b1 + x * m1 });

            min = Min(min, length);
        }

        return min;
    }

    protected static double GetChroma(double L, double H)
    {
        double hrad = H / 360 * PI * 2;

        var bounds = GetBounds(L);
        double min = MaxValue;
        foreach (var bound in bounds)
        {
            if (GetRayLength(hrad, bound, out double length))
                min = Min(min, length);
        }

        return min;
    }

    protected static double GetDistance(IList<double> point)
        => Sqrt(Pow(point[0], 2) + Pow(point[1], 2));

    protected static double GetIntersection(IList<double> lineA, IList<double> lineB)
        => (lineA[1] - lineB[1]) / (lineB[0] - lineA[0]);

    protected static bool GetRayLength(double theta, IList<double> line, out double length)
    {
        length = line[1] / (Sin(theta) - line[0] * Cos(theta));
        return length >= 0;
    }
}