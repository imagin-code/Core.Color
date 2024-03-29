﻿using System;

namespace Imagin.Core.Colors;

/// <summary>
/// <b>Luminance (Y), Db, Dr</b>
/// <para>The color model used in the SECAM analog terrestrial colour television broadcasting standard and PAL-N.</para>
/// <para><see cref="RGB"/> > <see cref="Lrgb"/> > <see cref="YDbDr"/></para>
/// </summary>
/// <remarks>https://github.com/colorjs/color-space/blob/master/ydbdr.js</remarks>
[Component(1, '%', "Y", "Luminance"), Component(-1.333, 1.333, ' ', "Db"), Component(-1.333, 1.333, ' ', "Dr")]
[Category(Class.YUV), Serializable]
[Description("The color model used in the SECAM analog terrestrial colour television broadcasting standard and PAL-N.")]
public class YDbDr : ColorModel3
{
    public YDbDr() : base() { }

    /// <summary>(🗸) <see cref="YDbDr"/> > <see cref="Lrgb"/></summary>
    public override Lrgb To(WorkingProfile profile)
    {
        double y = X, db = Y, dr = Z;

        var r = y + 0.000092303716148 * db - 0.525912630661865 * dr;
        var g = y - 0.129132898890509 * db + 0.267899328207599 * dr;
        var b = y + 0.664679059978955 * db - 0.000079202543533 * dr;
        return Colour.New<Lrgb>(r, g, b);
    }

    /// <summary>(🗸) <see cref="Lrgb"/> > <see cref="YDbDr"/></summary>
    public override void From(Lrgb input, WorkingProfile profile)
    {
        double r = input.X, g = input.Y, b = input.Z;
        Value = new
        (
             0.299 * r + 0.587 * g + 0.114 * b,
            -0.450 * r - 0.883 * g + 1.333 * b,
            -1.333 * r + 1.116 * g + 0.217 * b
        );
    }
}