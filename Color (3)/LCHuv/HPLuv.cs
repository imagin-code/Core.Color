﻿using System;

namespace Imagin.Core.Colors;

/// <summary>
/// <b>Hue (H), Saturation (P), Lightness (L)</b>
/// <para>A model derived from 'LCHuv' that defines color as having hue (H), saturation (P), and lightness (L).</para>
/// <para><see cref="RGB"/> > <see cref="Lrgb"/> > <see cref="XYZ"/> > <see cref="Luv"/> > <see cref="LCHuv"/> > <see cref="HPLuv"/></para>
/// </summary>
/// <remarks>https://github.com/hsluv/hsluv-csharp</remarks>
[Component(360, '°', "H", "Hue"), Component(100, '%', "P", "Saturation"), Component(100, '%', "L", "Lightness")]
[Category(Class.LCHuv), Serializable]
[Description("A model derived from 'LCHuv' that defines color as having hue (H), saturation (P), and lightness (L).")]
public class HPLuv : HLuv
{
    public HPLuv() : base() { }

    /// <summary>(🗸) <see cref="LCHuv"/> > <see cref="HPLuv"/></summary>
    public override void From(LCHuv input, WorkingProfile profile)
    {
        double L = input.X, C = input.Y, H = input.Z;

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

        double max = GetChroma(L);
        double S = C / max * 100;

        Value = new(H, S, L);
    }

    /// <summary>(🗸) <see cref="HPLuv"/> > <see cref="LCHuv"/></summary>
    public override void To(out LCHuv result, WorkingProfile profile)
    {
        double H = Value[0], S = Value[1], L = Value[2];

        if (L > 99.9999999)
        {
            result = Colour.New<LCHuv>(100, 0, H);
            return;
        }

        if (L < 0.00000001)
        {
            result = Colour.New<LCHuv>(0, 0, H);
            return;
        }

        double max = GetChroma(L);
        double C = max / 100 * S;

        result = Colour.New<LCHuv>(L, C, H);
    }
}