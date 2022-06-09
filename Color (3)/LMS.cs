﻿using Imagin.Core.Numerics;
using System;

namespace Imagin.Core.Colors;

/// <summary>
/// (🗸) <b>Long (L/ρ), Medium (M/γ), Short (S/β)</b>
/// 
/// <para>A color space that represents the response of the three types of cones of the human eye, named for their responsivity (sensitivity) peaks at long, medium, and short wavelengths.</para>
/// 
/// <para>≡ 100%</para>
/// <para><see cref="RGB"/> > <see cref="Lrgb"/> > <see cref="XYZ"/> > <see cref="LMS"/></para>
/// </summary>
/// <remarks>https://github.com/tompazourek/Colourful</remarks>
[Component(1, '%', "L", "Long"), Component(1, '%', "M", "Medium"), Component(1, '%', "S", "Short")]
[Serializable]
public class LMS : XYZ, IXYZ
{    
    /// <summary>
    /// The Hunt and RLAB color appearance models use the Hunt-Pointer-Estevez transformation matrix (MHPE) for conversion from CIE XYZ to LMS. This is the transformation matrix which was originally used in conjunction with the von Kries transform method, and is therefore also called von Kries transformation matrix (MvonKries).
    /// <para>https://en.wikipedia.org/wiki/LMS_color_space#Hunt,_RLAB</para>
    /// </summary>
    /// <remarks>
    /// <para>Normalized to <see cref="Illuminant2.D65"/>.</para>
    /// <para>https://github.com/tompazourek/Colourful</para>
    /// </remarks>
    public static class Transform
    {
        /// <summary>
        /// <see cref="XYZ"/> scaling chromatic adaptation transform matrix.
        /// </summary>
        public static readonly double[,] XYZScaling = Matrix.Identity(size: 3);

        /// <summary>
        /// Von Kries chromatic adaptation transform matrix (Hunt-Pointer-Estevez for equal energy).
        /// </summary>
        public static Matrix VonKries => new double[][]
        {
            new double[] {  0.38971, 0.68898, -0.07868 },
            new double[] { -0.22981, 1.18340,  0.04641 },
            new double[] {  0.00000, 0.00000,  1.00000 }
        };

        /// <summary>
        /// Von Kries chromatic adaptation transform matrix (Hunt-Pointer-Estevez adjusted for D65).
        /// </summary>
        public static Matrix VonKriesAdjusted => new double[][]
        {
            new double[] {  0.4002, 0.7076, -0.0808 },
            new double[] { -0.2263, 1.1653,  0.0457 },
            new double[] { 0.00000, 0.00000, 0.9182 }
        };

        /// <summary>
        /// Bradford chromatic adaptation transform matrix (used in CMCCAT97).
        /// </summary>
        public static Matrix Bradford => new double[][]
        {
            new double[] {  0.8951,  0.2664,-0.1614 },
            new double[] { -0.7502,  1.7135, 0.0367 },
            new double[] {  0.0389, -0.0686, 1.0296 }
        };

        /// <summary>
        /// Spectral sharpening and the Bradford transform.
        /// </summary>
        public static Matrix BradfordSharp => new double[][]
        {
            new double[] {  1.2694, -0.0988, -0.1706 },
            new double[] { -0.8364,  1.8006,  0.0357 },
            new double[] {  0.0297, -0.0315,  1.0018 }
        };

        public static Matrix CAT97 => new double[][]
        {
            new double[] {  0.8562,  0.3372, -0.1934 },
            new double[] { -0.8360,  1.8327,  0.0033 },
            new double[] {  0.0357, -0.00469, 1.0112 }
        };

        /// <summary>
        /// CMCCAT2000 (fitted from all available color data sets).
        /// </summary>
        public static Matrix CAT00 => new double[][]
        {
            new double[] {  0.7982, 0.3389,-0.1371 },
            new double[] { -0.5918, 1.5512, 0.0406 },
            new double[] {  0.0008, 0.0239, 0.9753 }
        };

        /// <summary>
        /// CAT02 (optimized for minimizing CIELAB differences).
        /// </summary>
        public static Matrix CAT02 => new double[][]
        {
            new double[] {  0.7328, 0.4296,-0.1624 },
            new double[] { -0.7036, 1.6975, 0.0061 },
            new double[] {  0.0030, 0.0136, 0.9834 }
        };
    }

    public LMS(params double[] input) : base(input) { }

    public static implicit operator LMS(Vector3 input) => new(input.X, input.Y, input.Z);

    /// <summary>(🗸) <see cref="LMS"/> (0) > <see cref="LMS"/> (1)</summary>
    public override void Adapt(WorkingProfile source, WorkingProfile target) => Value = Adapt(this, source, target);

    /// <summary>(🗸) <see cref="LMS"/> > <see cref="XYZ"/></summary>
    public XYZ ToXYZ(WorkingProfile profile)
        => new(profile.Transform.Invert3By3().Multiply(Value));

    /// <summary>(🗸) <see cref="XYZ"/> > <see cref="LMS"/></summary>
    public void FromXYZ(XYZ input, WorkingProfile profile)
        => Value = new(profile.Transform.Multiply(input.Value));

    /// <summary>(🗸) <see cref="LMS"/> > <see cref="Lrgb"/></summary>
    public override Lrgb ToLrgb(WorkingProfile profile)
        => ToXYZ(profile).ToLrgb(profile);

    /// <summary>(🗸) <see cref="Lrgb"/> > <see cref="LMS"/></summary>
    public override void FromLrgb(Lrgb input, WorkingProfile profile)
    {
        var result = new XYZ();
        result.FromLrgb(input, profile);
        FromXYZ(result, profile);
    }
}