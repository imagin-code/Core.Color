﻿using System;

using static Imagin.Core.Numerics.M;
using static System.Math;

namespace Imagin.Core.Colors;

/// <summary>
/// <b>Tint (T), Saturation (S), Lightness (L)</b>
/// <para>A perceptual color space developed primarily for the purpose of face detection that defines color as tint (like hue with white added), the colorfulness of a stimulus relative to its own brightness (S), and the brightness of a stimulus relative to a stimulus that appears white in similar viewing conditions (L).</para>
/// <para><see cref="RGB"/> > <see cref="Lrgb"/> > <see cref="TSL"/></para>
/// 
/// <para>🞩 <i>The color space repeats unless <see cref="T"/> / 4. Is this expected?</i></para>
/// 
/// <i>Author</i>
/// <list type="bullet">
/// <item>Jean-Christophe Terrillon</item>
/// <item>Shigeru Akamatsu</item>
/// </list>
/// </summary>
/// <remarks>https://en.wikipedia.org/wiki/TSL_color_space#Conversion_between_RGB_and_TSL</remarks>
[Component(1, '%', "T", "Tint"), Component(1, '%', "S", "Saturation"), Component(1, '%', "L", "Lightness")]
[Category(Class.HS), Serializable]
[Description("A perceptual color space developed primarily for the purpose of face detection that defines color as tint (like hue with white added), the colorfulness of a stimulus relative to its own brightness (S), and the brightness of a stimulus relative to a stimulus that appears white in similar viewing conditions (L).")]
public class TSL : ColorModel3
{
    public double T => X;

    public double S => Y;

    public double L => Z;

    public TSL() : base() { }

    /// <summary>(🞩) <see cref="Lrgb"/> > <see cref="TSL"/></summary>
    public override void From(Lrgb input, WorkingProfile profile)
    {
        var R = input.X; var G = input.Y; var B = input.Z;

        var sum = R + G + B;
        var r = R / sum;
        var g = G / sum;

        double rP = r - (1 / 3), gP = g - (1 / 3);

        var T = gP > 0
            ? 1 / (2 * PI) * Atan(rP / gP) + (1 / 4)
            : gP < 0
            ? 1 / (2 * PI) * Atan(rP / gP) + (3 / 4)
            : 0;

        var S = Sqrt(9 / 5 * (Pow2(rP) + Pow2(gP)));
        var L = (R * 0.299) + (G * 0.587) + (B * 0.114);
        Value = new(T, S, L);
    }

    /// <summary>(🞩) <see cref="TSL"/> > <see cref="Lrgb"/></summary>
    public override Lrgb To(WorkingProfile profile)
    {
        double T = X / 4, S = Y, L = Z;

        var y = 2 * PI * T;
        var x = -(Cos(y) / Sin(y));

        var bP = 5 / (9 * (Pow(x, 2) + 1));
        var gP = T > 0.5 ? -Sqrt(bP) * S : T < 0.5 ? Sqrt(bP) * S : 0;
        var rP = T == 0 ? Sqrt(5) / 3 * S : x * gP;

        var r = rP + (1 / 3);
        var g = gP + (1 / 3);

        var k = L / (r * 0.185 + g * 0.473 + 0.114);

        var R = k * r; var G = k * g; var B = k * (1 - r - g);
        return Colour.New<Lrgb>(R, G, B);
    }
}