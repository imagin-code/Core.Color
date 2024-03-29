﻿using System;
using Imagin.Core.Numerics;
using Imagin.Core.Linq;
using static System.Math;

namespace Imagin.Core.Colors;

/// <summary>
/// <b>Red (R), Green (G), Blue (B), Black (K)</b>
/// <para>An additive model where the primary colors are added with black.</para>
/// <para><see cref="RGB"/> > <see cref="Lrgb"/> > <see cref="RGBK"/></para>
/// </summary>
[Component(255, '%', "R", "Red"), Component(255, '%', "G", "Green"), Component(255, '%', "B", "Blue"), Component(255, '%', "K", "Black")]
[Category(Class.RGB), Serializable]
[Description("An additive model where the primary colors are added with black.")]
public sealed class RGBK : ColorModel4
{
    public RGBK() : base() { }

    /// <summary>(🗸) <see cref="Lrgb"/> > <see cref="RGBK"/></summary>
    public override void From(Lrgb input, WorkingProfile profile)
    {
        var k0 = 1.0 - Max(input.X, Max(input.Y, input.Z));
        var k1 = 1.0 - k0;

        var r = (1 - input.X - k0) / k1;
        var g = (1 - input.Y - k0) / k1;
        var b = (1 - input.Z - k0) / k1;
        Value = new Vector4(1 - r.NaN(0), 1 - g.NaN(0), 1 - b.NaN(0), k0) * 255;
    }

    /// <summary>(🗸) <see cref="RGBK"/> > <see cref="Lrgb"/></summary>
    public override Lrgb To(WorkingProfile profile)
    {
        var result = Value / 255;
        var r = result[0] * (1.0 - result[3]);
        var g = result[1] * (1.0 - result[3]);
        var b = result[2] * (1.0 - result[3]);
        return Colour.New<Lrgb>(r, g, b);
    }
}