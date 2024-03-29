﻿using System;

namespace Imagin.Core.Colors;

/// <summary>
/// <b>Luminance (Y), E-factor (E), S-factor (S)</b>
/// <para>A model that defines color as having luminance (Y), 'E-factor' (E), and 'S-factor' (S).</para>
/// <para><see cref="RGB"/> > <see cref="Lrgb"/> > <see cref="YES"/></para>
/// </summary>
/// <remarks>https://github.com/colorjs/color-space/blob/master/yes.js</remarks>
[Component(1, '%', "Y", "Luminance"), Component(1, '%', "E", "E-factor"), Component(1, '%', "S", "S-factor")]
[Category(Class.YUV), Serializable]
[Description("A model that defines color as having luminance (Y), 'E-factor' (E), and 'S-factor' (S).")]
public class YES : ColorModel3
{
    public YES() : base() { }

    /// <summary>(🗸) <see cref="YES"/> > <see cref="Lrgb"/></summary>
    public override Lrgb To(WorkingProfile profile)
    {
        double y = X, e = Y, s = Z;

        var m = new[]
        {
            1,  1.431,  0.126,
            1, -0.569,  0.126,
            1,  0.431, -1.874
        };

        double
            r = y * m[0] + e * m[1] + s * m[2],
            g = y * m[3] + e * m[4] + s * m[5],
            b = y * m[6] + e * m[7] + s * m[8];

        return Colour.New<Lrgb>(r, g, b);
    }

    /// <summary>(🗸) <see cref="Lrgb"/> > <see cref="YES"/></summary>
    public override void From(Lrgb input, WorkingProfile profile)
    {
        double r = input.X, g = input.Y, b = input.Z;

        var m = new[]
        {
            0.253,  0.684,  0.063,
            0.500, -0.500,  0,
            0.250,  0.250, -0.500
        };

        Value = new(r * m[0] + g * m[1] + b * m[2], r * m[3] + g * m[4] + b * m[5], r * m[6] + g * m[7] + b * m[8]);
    }
}