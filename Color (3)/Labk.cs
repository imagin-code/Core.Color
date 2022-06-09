﻿using Imagin.Core.Numerics;
using System;

using static Imagin.Core.Numerics.M;
using static System.Math;

namespace Imagin.Core.Colors;

/// <summary>
/// (🗸) <b>Perceived lightness (L), Red/green (a), Blue/yellow (b)</b>
/// <para>≡ 100%</para>
/// <para><see cref="RGB"/> > <see cref="Lrgb"/> > <see cref="XYZ"/> > <see cref="Labk"/></para>
/// 
/// <i>Alias</i>
/// <list type="bullet">
/// <item>OKLab</item>
/// </list>
/// </summary>
/// <remarks>https://colour.readthedocs.io/en/develop/_modules/colour/models/oklab.html</remarks>
[Component(1, '°', "L", "Perceived lightness"), Component(1, '%', "a", "Red/green"), Component(1, '%', "b", "Blue/yellow")]
[Serializable]
public class Labk : XYZ, ILAb
{
    public Labk(params double[] input) : base(input) { }

    public static implicit operator Labk(Vector3 input) => new(input.X, input.Y, input.Z);

    public static Matrix XYZ_LMS => new double[][]
    {
        new[] { 0.8189330101, 0.3618667424, -0.1288597137 },
        new[] { 0.0329845436, 0.9293118715,  0.0361456387 },
        new[] { 0.0482003018, 0.2643662691,  0.6338517070 },
    };
    public static Matrix LMS_XYZ => XYZ_LMS.Invert3By3();

    public static Matrix LMS_LAB => new double[][]
    {
        new[] { 0.2104542553,  0.7936177850, -0.0040720468 },
        new[] { 1.9779984951, -2.4285922050,  0.4505937099 },
        new[] { 0.0259040371,  0.7827717662, -0.8086757660 },
    };
    public static Matrix LAB_LMS => LMS_LAB.Invert3By3();

    /// <summary>(🗸) <see cref="Labk"/> > <see cref="Lrgb"/></summary>
    public override Lrgb ToLrgb(WorkingProfile profile)
    {
        var lab = Value;

        var lms = LAB_LMS.Multiply(lab);
        var lmsPrime = new Vector(Pow(lms[0], 3), Pow(lms[1], 3), Pow(lms[2], 3));

        var xyz = LMS_XYZ.Multiply(lmsPrime);
        xyz = new(xyz * profile.White);

        return new XYZ(xyz).ToLrgb(profile);
    }

    /// <summary>(🗸) <see cref="Lrgb"/> > <see cref="Labk"/></summary>
    public override void FromLrgb(Lrgb input, WorkingProfile profile)
    {
        var xyz = new XYZ();
        xyz.FromLrgb(input, profile);

        xyz = new(xyz.Value / profile.White);

        var lms = XYZ_LMS.Multiply(xyz);
        var lmsPrime = new LMS(Cbrt(lms[0]), Cbrt(lms[1]), Cbrt(lms[2]));

        var lab = LMS_LAB.Multiply(lmsPrime);
        Value = new(lab);
    }
}