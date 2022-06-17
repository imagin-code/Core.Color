﻿using Imagin.Core.Numerics;
using System;

namespace Imagin.Core.Colors;

/// <summary>
/// Specifies an abstract mathematical model that describes the way colors can be represented as tuples of numbers.
/// <para>https://en.wikipedia.org/wiki/Color_model</para>
/// </summary>
[Serializable]
public abstract class ColorModel : IEquatable<ColorModel>, IColorModel, IConvert<Lrgb>, IConvert<RGB>
{
    #region Properties

    Vector _value;
    public Vector Value
    {
        get => _value;
        internal set
        {
            _value = value;
            if (this is ColorModel2)
            {
                if (value.Length != 2)
                    throw new NotSupportedException();
            }
            else if (this is ColorModel3)
            {
                if (value.Length != 3)
                    throw new NotSupportedException();
            }
            else if(this is ColorModel4)
            {
                if (value.Length != 4)
                    throw new NotSupportedException();
            }
        }
    }

    public double this[int i]
    {
        get => Value[i];
        set => Value = new(i == 0 ? value : Value[0], i == 1 ? value : Value[1], i == 2 ? value : Value[2]);
    }

    #endregion

    #region ColorModel

    protected ColorModel() : base() { }

    public static implicit operator Vector(ColorModel input) => input.Value;

    public static explicit operator double[](ColorModel input) => input.Value;

    #endregion

    #region Methods

    /// <summary>(🗸) <see cref="LMS"/> (0) > <see cref="LMS"/> (1)</summary>
    protected LMS Adapt(LMS color, LMS sW, LMS dW)
    {
        var result = Matrix.Diagonal(dW[0] / sW[0], dW[1] / sW[1], dW[2] / sW[2]).Multiply(color.Value);
        return Colour.New<LMS>(result[0], result[1], result[2]);
    }

    /// <summary>(🗸) <see cref="LMS"/> (0) > <see cref="LMS"/> (1)</summary>
    protected LMS Adapt(LMS input, WorkingProfile source, WorkingProfile target)
    {
        //XYZ (0) > LMS (0)
        var a = new LMS();
        a.From((XYZ)(xyY)(xy)source.Chromacity, source);

        //XYZ (1) > LMS (1)
        var b = new LMS();
        b.From((XYZ)(xyY)(xy)target.Chromacity, source);

        //LMS (0) > LMS (1)
        return Adapt(input, a, b);
    }

    /// <summary>(🗸) <see cref="RGB"/> (0) > <see cref="XYZ"/> (0) > <see cref="LMS"/> (0) > <see cref="LMS"/> (1) > <see cref="XYZ"/> (1) > <see cref="RGB"/> (1)</summary>
    protected RGB Adapt(RGB input, WorkingProfile source, WorkingProfile target)
    {
        //RGB (0) > XYZ (0)
        var xyz = new XYZ();
        xyz.From(input, source);

        //XYZ (0) > LMS (0) > LMS (1) > XYZ (1)
        xyz = Adapt(xyz, source, target);

        //XYZ (1) > RGB (1)
        xyz.To(out RGB result, target);
        return result;
    }

    /// <summary>(🗸) <see cref="XYZ"/> (0) > <see cref="LMS"/> (0) > <see cref="LMS"/> (1) > <see cref="XYZ"/> (1)</summary>
    protected XYZ Adapt(XYZ input, WorkingProfile source, WorkingProfile target)
    {
        //XYZ (0) > LMS (0)
        var lms = new LMS();
        lms.From(input, source);

        //LMS (0) > LMS (1)
        lms = Adapt(lms, source, target);

        //LMS (1) > XYZ (1)
        lms.To(out XYZ result, target);
        return result;
    }

    /// <summary>(🗸) <see cref="ColorModel">this</see> (0) > <see cref="RGB"/> (0) > <see cref="XYZ"/> (0) > <see cref="LMS"/> (0) > <see cref="LMS"/> (1) > <see cref="XYZ"/> (1) > <see cref="RGB"/> (1) > <see cref="ColorModel">this</see> (1)</summary>
    public virtual void Adapt(WorkingProfile source, WorkingProfile target)
    {
        To(out RGB result, source);
        Value = Adapt(result, source, target);
    }

    //...

    /// <summary>(🗸) <see cref="RGB"/> > <see cref="Lrgb"/> > this</summary>
    public virtual void From(RGB input, WorkingProfile profile)
    {
        var result = input.To(profile);
        From(result, profile);
    }

    /// <summary>(🗸) <see cref="Lrgb"/> > this</summary>
    public abstract void From(Lrgb input, WorkingProfile profile);

    /// <summary>(🗸) this > <see cref="Lrgb"/></summary>
    public abstract Lrgb To(WorkingProfile profile);

    /// <summary>(🗸) this > <see cref="Lrgb"/></summary>
    public void To(out Lrgb result, WorkingProfile profile) => result = To(profile);

    /// <summary>(🗸) this > <see cref="Lrgb"/> > <see cref="RGB"/></summary>
    public virtual void To(out RGB result, WorkingProfile profile)
    {
        var a = To(profile);

        var b = new RGB();
        b.From(a, profile);
        result = b;
    }

    /// <summary>(🗸) this > <see cref="Lrgb"/> > <see cref="To{T}(WorkingProfile)">T</see></summary>
    public T To<T>(WorkingProfile profile = default) where T : ColorModel, new()
    {
        //this > RGB
        To(out RGB rgb, profile);

        //RGB > T
        var result = new T();
        result.From(rgb, profile);
        return result;
    }

    //...

    public override string ToString() => Value.ToString();

    #endregion

    #region ==

    public static bool operator ==(ColorModel left, ColorModel right) => Equals(left, right);

    public static bool operator !=(ColorModel left, ColorModel right) => !Equals(left, right);

    public bool Equals(ColorModel i) => Value.Equals(i.Value);

    public override bool Equals(object i) => i is ColorModel j && Equals(j);

    public override int GetHashCode() => Value.GetHashCode();

    #endregion
}