﻿using Imagin.Core.Numerics;
using System;

namespace Imagin.Core.Colors;

/// <summary>
/// (🗸) <b>Hue (H), Saturation (S), Lightness (L)</b>
/// <para>≡ 100%</para>
/// <para><see cref="RGB"/> > <see cref="Lrgb"/> > <see cref="XYZ"/> > <see cref="Luv"/> > <see cref="LCHuv"/> > <see cref="HSLuv"/></para>
/// </summary>
/// <remarks>https://github.com/hsluv/hsluv-csharp</remarks>
[Component(360, '°', "H", "Hue"), Component(100, '%', "S", "Saturation"), Component(100, '%', "L", "Lightness")]
[Serializable]
public class HSLuv : LCHuv, IHs
{
    public HSLuv(params double[] input) : base(input) { }

    public static implicit operator HSLuv(Vector3 input) => new(input.X, input.Y, input.Z);

    /// <summary>(🗸) <see cref="HSLuv"/> > <see cref="Lrgb"/></summary>
    public override Lrgb ToLrgb(WorkingProfile profile)
    {
        double H = Value[0], S = Value[1], L = Value[2];

        if (L > 99.9999999)
            return new(100, 0, H);

        if (L < 0.00000001)
            return new(0, 0, H);

        double max = GetChroma(L, H);
        double C = max / 100 * S;

        return new LCHuv(L, C, H).ToLrgb(profile);
    }

    /// <summary>(🗸) <see cref="Lrgb"/> > <see cref="HSLuv"/></summary>
    public override void FromLrgb(Lrgb input, WorkingProfile profile)
    {
        var lch = new LCHuv();
        lch.FromLrgb(input, profile);

        double L = lch[0], C = lch[1], H = lch[2];

        if (L > 99.9999999)
        {
            Value = new(H, 0, 100);
            return;
        }

        if (L < 0.00000001)
        {
            Value = new(H, 0, 0);
            return;
        }

        double max = GetChroma(L, H);
        double S = C / max * 100;

        Value = new(H, S, L);
    }
}