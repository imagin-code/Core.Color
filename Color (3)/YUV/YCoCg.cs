﻿using System;

namespace Imagin.Core.Colors;

/// <summary>
/// <para><b>Luminance (Y), Co, Cg</b></para>
/// <para>A model that defines color as having luminance (Y), 'Chrominance green' (Cg), and 'Chrominance orange' (Co).</para>
/// <para><see cref="RGB"/> > <see cref="Lrgb"/> > <see cref="YCoCg"/></para>
/// 
/// <i>Alias</i>
/// <list type="bullet">
/// <item>YCgCo</item>
/// </list>
/// </summary>
/// <remarks>https://github.com/colorjs/color-space/blob/master/ycgco.js</remarks>
[Component(   1,   '%', "Y", "Luminance"), Component(-0.5, 0.5, ' ', "Co"), Component(-0.5, 0.5, ' ', "Cg")]
[Category(Class.YUV), Serializable]
[Description("A model that defines color as having luminance (Y), 'Chrominance green' (Cg), and 'Chrominance orange' (Co).")]
public class YCoCg : ColorModel3
{
    public YCoCg() : base() { }

    /// <summary>(🗸) <see cref="YCoCg"/> > <see cref="Lrgb"/></summary>
    public override Lrgb To(WorkingProfile profile)
    {
        double y = X, cg = Y, co = Z;

        var c = y - cg;
        return Colour.New<Lrgb>(c + co, y + cg, c - co);
    }

    /// <summary>(🗸) <see cref="Lrgb"/> > <see cref="YCoCg"/></summary>
    public override void From(Lrgb input, WorkingProfile profile)
    {
        double r = input.X, g = input.Y, b = input.Z;
        Value = new(0.25 * r + 0.5 * g + 0.25 * b, -0.25 * r + 0.5 * g - 0.25 * b, 0.5 * r - 0.5 * b);
    }
}