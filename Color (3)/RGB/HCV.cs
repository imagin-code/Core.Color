﻿using System;
using static System.Math;

namespace Imagin.Core.Colors;

/// <summary>
/// <b>Hue (H), Chroma (C), Gray (V)</b>
/// <para>A model that defines color as having hue (H), chroma (C), and gray (V).</para>
/// <para><see cref="RGB"/> > <see cref="Lrgb"/> > <see cref="HCV"/></para>
/// </summary>
/// <remarks>https://github.com/helixd2s/hcv-color</remarks>
[Component(360, '°', "H", "Hue"), Component(100, '%', "C", "Chroma"), Component(100, '%', "V", "Gray")]
[Category(Class.HC), Class(Class.H | Class.HC), Serializable]
[Description("A model that defines color as having hue (H), chroma (C), and gray (V).")]
public class HCV : ColorModel3
{
    public HCV() : base() { }

    /// <summary>(🗸) <see cref="HCV"/> > <see cref="Lrgb"/></summary>
    public override Lrgb To(WorkingProfile profile)
    {
        double h = Value[0] / 360, c = Value[1] / 100.0, g = Value[2] / 100.0;

        if (c == 0)
            return Colour.New<Lrgb>(g, g, g);

        var hi = (h % 1.0) * 6.0;
        var v = hi % 1.0;
        var pure = new double[3];
        var w = 1.0 - v;

        switch (Floor(hi))
        {
            case 0:
                pure[0] = 1; pure[1] = v; pure[2] = 0; break;
            case 1:
                pure[0] = w; pure[1] = 1; pure[2] = 0; break;
            case 2:
                pure[0] = 0; pure[1] = 1; pure[2] = v; break;
            case 3:
                pure[0] = 0; pure[1] = w; pure[2] = 1; break;
            case 4:
                pure[0] = v; pure[1] = 0; pure[2] = 1; break;
            default:
                pure[0] = 1; pure[1] = 0; pure[2] = w; break;
        }

        var mg = (1.0 - c) * g;
        return Colour.New<Lrgb>
        (
            c * pure[0] + mg,
            c * pure[1] + mg,
            c * pure[2] + mg
        );
    }

    /// <summary>(🗸) <see cref="Lrgb"/> > <see cref="HCV"/></summary>
    public override void From(Lrgb input, WorkingProfile profile)
    {
        double r = input.Value[0], g = input.Value[1], b = input.Value[2];

        var max = Max(Max(r, g), b);
        var min = Min(Min(r, g), b);

        var chroma = max - min;
        double grayscale = 0;
        double hue;

        if (chroma < 1)
            grayscale = min / (1.0 - chroma);

        if (chroma > 0)
        {
            if (max == r)
            {
                hue = ((g - b) / chroma) % 6;
            }
            else if (max == g)
            {
                hue = 2 + (b - r) / chroma;
            }
            else hue = 4 + (r - g) / chroma;

            hue /= 6;
            hue %= 1;
        }
        else hue = 0;

        Value = new(hue * 360, chroma * 100, grayscale * 100);
    }
}