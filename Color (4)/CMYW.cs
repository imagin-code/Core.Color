﻿using System;

namespace Imagin.Core.Colors;

/// <summary>
/// <b>Cyan (C), Magenta (M), Yellow (Y), White (W)</b>
/// <para>A subtractive color model based on <see cref="RGBW"/> where the secondary colors are added with white.</para>
/// <para><see cref="RGB"/> > <see cref="Lrgb"/> > <see cref="CMYW"/></para>
/// </summary>
[Component(100, "C", "Cyan"), Component(100, "M", "Magenta"), Component(100, "Y", "Yellow"), Component(100, "W", "White")]
[Serializable]
public sealed class CMYW : ColorModel4
{
    public CMYW() : base() { }

    /// <summary>(🞩) <see cref="Lrgb"/> > <see cref="CMYW"/></summary>
    public override void From(Lrgb input, WorkingProfile profile) { }

    /// <summary>(🗸) <see cref="CMYW"/> > <see cref="Lrgb"/></summary>
    public override Lrgb To(WorkingProfile profile)
    {
        double r = (1 - X) / 100;
        double g = (1 - Y) / 100;
        double b = (1 - Z) / 100;
        double w = W / 100;

        r *= (1 - w);
        r += w;

        g *= (1 - w);
        g += w;

        b *= (1 - w);
        b += w;

        return Colour.New<Lrgb>(r, g, b);
    }
}