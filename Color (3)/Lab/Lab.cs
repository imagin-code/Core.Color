﻿using System;

using static System.Math;

namespace Imagin.Core.Colors;

/// <summary>
/// <para><b>Lightness (L*), a*, b*</b></para>
/// <para>A perceptually uniform color space that measures perceptual lightness and the four unique colors of human vision (red, green, blue, and yellow) where a given numerical change corresponds to a similar perceived change in color.</para>
/// <para><see cref="RGB"/> > <see cref="Lrgb"/> > <see cref="XYZ"/> > <see cref="Lab"/></para>
/// 
/// <i>Alias</i>
/// <list type="bullet">
/// <item>CIELAB</item>
/// <item>L*a*b*</item>
/// </list>
/// 
/// <i>Author</i>
/// <list type="bullet">
/// <item><see cref="CIE"/> (1976)</item>
/// </list>
/// </summary>
/// <remarks>https://github.com/tompazourek/Colourful</remarks>
[Component(100, '%', "L*", "Lightness"), Component(-100, 100, ' ', "a*"), Component(-100, 100, ' ', "b*")]
[Category(Class.Lab), Serializable]
[Description("A perceptually uniform color space that measures perceptual lightness and the four unique colors of human vision (red, green, blue, and yellow) where a given numerical change corresponds to a similar perceived change in color.")]
public class Lab : ColorModel3<XYZ>
{
    public Lab() : base() { }

    /// <summary>(🗸) <see cref="XYZ"/> > <see cref="Lab"/></summary>
    public override void From(XYZ input, WorkingProfile profile)
    {
        double Xr = profile.White.X, Yr = profile.White.Y, Zr = profile.White.Z;
        double xr = input.X / Xr, yr = input.Y / Yr, zr = input.Z / Zr;

        static double f(double cr)
            => cr > CIE.IEpsilon
            ? Pow(cr, 1 / 3d)
            : (CIE.IKappa * cr + 16) / 116d;

        var fx = f(xr); var fy = f(yr); var fz = f(zr);

        var l = 116 * fy - 16;
        var a = 500 * (fx - fy);
        var b = 200 * (fy - fz);
        Value = new(l, a, b);
    }

    /// <summary>(🗸) <see cref="Lab"/> > <see cref="XYZ"/></summary>
    public override void To(out XYZ result, WorkingProfile profile)
    {
        double L = X, a = Y, b = Z;

        var fy = (L + 16) / 116d;
        var fx = a / 500d + fy;
        var fz = fy - b / 200d;

        var fx3 = Pow(fx, 3);
        var fz3 = Pow(fz, 3);

        var xr = fx3 > CIE.IEpsilon ? fx3 : (116 * fx - 16) / CIE.IKappa;
        var yr =   L > CIE.IKappa * CIE.IEpsilon ? Pow((L + 16) / 116d, 3) : L / CIE.IKappa;
        var zr = fz3 > CIE.IEpsilon ? fz3 : (116 * fz - 16) / CIE.IKappa;

        result = Colour.New<XYZ>(xr * profile.White.X, yr * profile.White.Y, zr * profile.White.Z);
    }
}