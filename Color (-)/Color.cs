﻿using Imagin.Core.Linq;
using Imagin.Core.Numerics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Imagin.Core.Colors;

public static partial class Colour
{
    #region Properties

    /// <summary>Gets the index (based on the type) corresponding to each <see cref="ColorModel"/> in the relevant shader file.</summary>
    internal static readonly Dictionary<Type, int> Index = new();

    /// <summary>Gets the type (based on the index) corresponding to each <see cref="ColorModel"/> in the relevant shader file.</summary>
    internal static readonly Dictionary<int, Type> Type = new();

    ///

    public static readonly Dictionary<Type, Vector<Component>> Components = new();

    /// <summary>Gets all (shader-supported) color models as an <see cref="ObservableCollection{T}"/>.</summary>
    public static ObservableCollection<Type> Types => new(Type.Select(i => i.Value));

    #endregion

    #region Color (static)

    /// <remarks>Rearrange lines to match indices in relevant shader file.</remarks>
    static Colour()
    {
        #region i<T>()

        var index = 0;
        void i<T>()
        {
            var type = typeof(T);
            Type.Add(index, type); Index.Add(typeof(T), index);

            var components = new Vector<Component>(type.GetAttributes<ComponentAttribute>()?.Select(i => i.Info).ToArray());
            Components.Add(type, components);

            index++;
        }

        #endregion

        //(3)
        i<RCA>(); i<RGB>(); i<RGV>(); i<RYB>();
        i<CMY>();
        i<HCV>(); i<HCY>();
        i<HPLuv>();
        i<HSB>(); 
        i<HSL>(); i<HSLuv>();
        i<HSM>();
        i<HSP>();
        i<HWB>();
        i<IPT>();
        i<JCh>(); i<JMh>(); i<Jsh>();
        i<JPEG>();
        i<Lab>(); i<Labh>(); /*i<Labi>();*/ i<Labj>(); i<Labk>(); i<Labksl>(); i<Labksb>(); i<Labkwb>();
        i<LCHab>(); i<LCHabh>(); i<LCHabj>(); i<LCHrg>(); i<LCHuv>(); i<LCHxy>();
        i<LMS>(); i<Luv>();
        i<QCh>(); i<QMh>(); i<Qsh>();
        i<rgG>();
        i<TSL>();
        i<UCS>();
        i<UVW>();
        i<xvYCC>(); 
        i<xyY>(); i<xyYC>(); 
        i<XYZ>();
        i<YCbCr>(); i<YCoCg>(); i<YDbDr>(); i<YES>(); i<YIQ>(); i<YPbPr>(); i<YUV>();

        //(4)
        i<CMYK>(); i<CMYW>(); i<RGBK>(); i<RGBW>();
    }

    #endregion

    #region Get(Index/Type)

    /// <summary>Gets the index (based on the type) corresponding to each <see cref="ColorModel"/> in the relevant shader file.</summary>
    public static int GetIndex<T>() => GetIndex(typeof(T));

    /// <summary>Gets the index (based on the type) corresponding to each <see cref="ColorModel"/> in the relevant shader file.</summary>
    public static int GetIndex(Type input) => Index[input];

    /// <summary>Gets the type (based on the index) corresponding to each <see cref="ColorModel"/> in the relevant shader file.</summary>
    public static Type GetType(int input) => Type[input];

    #endregion

    #region Maximum

    public static Vector Maximum(Type i) => new(Components[i].Transform((x, y) => y.Maximum));

    public static Vector Maximum<T>() where T : ColorModel => Maximum(typeof(T));

    #endregion

    #region Minimum

    public static Vector Minimum(Type i) => new(Components[i].Transform((x, y) => y.Minimum));

    public static Vector Minimum<T>() where T : ColorModel => Minimum(typeof(T));

    #endregion

    #region New

    public static ColorModel New(Type i) => i.Create<ColorModel>();

    public static ColorModel New<T>() where T : ColorModel => New(typeof(T));

    ///

    public static ColorModel New(Type i, RGB input, WorkingProfile profile)
    {
        var result = New(i);
        result.From(input, profile);
        return result;
    }

    public static ColorModel New<T>(RGB input, WorkingProfile profile) where T : ColorModel => New(typeof(T), input, profile);

    ///

    public static ColorModel New(Type i, double all)
    {
        var result = New(i);
        if (result is ColorModel2 a)
            a.Value = new(all, all);

        else if(result is ColorModel3 b)
            b.Value = new(all, all, all);

        else if (result is ColorModel4 c)
            c.Value = new(all, all, all, all);

        else throw new NotSupportedException();

        return result;
    }

    public static ColorModel New(Type i, double x, double y)
    {
        var result = New(i);
        if (result is ColorModel2 j)
            j.Value = new(x, y);

        else throw new NotSupportedException();

        return result;
    }

    public static ColorModel New(Type i, double x, double y, double z)
    {
        var result = New(i);
        if (result is ColorModel3 j)
            j.Value = new(x, y, z);

        else throw new NotSupportedException();

        return result;
    }

    public static ColorModel New(Type i, double x, double y, double z, double w)
    {
        var result = New(i);
        if (result is ColorModel4 j)
            j.Value = new(x, y, z, w);
        
        else throw new NotSupportedException();

        return result;
    }

    public static ColorModel New(Type i, Vector input)
    {
        var result = New(i);

        if (result is ColorModel4 c && input.Length >= 4)
        {
            c.X = input[0]; c.Y = input[1]; c.Z = input[2]; c.W = input[3];
        }

        else if(result is ColorModel3 b && input.Length >= 3)
        {
            b.X = input[0]; b.Y = input[1]; b.Z = input[2];
        }

        else if(result is ColorModel2 a && input.Length >= 2)
        {
            a.X = input[0]; a.Y = input[1];
        }

        else throw new NotSupportedException();
        return result;
    }

    public static ColorModel New(Type i, Vector2 input)
    {
        var result = New(i);
        if (result is ColorModel2 model)
        {
            model.X = input.X; model.Y = input.Y;
        }
        return result;
    }

    public static ColorModel New(Type i, Vector3 input)
    {
        var result = New(i);
        if (result is ColorModel3 model)
        {
            model.X = input.X; model.Y = input.Y; model.Z = input.Z;
        }
        return result;
    }

    public static ColorModel New(Type i, Vector4 input)
    {
        var result = New(i);
        if (result is ColorModel4 model)
        {
            model.X = input.X; model.Y = input.Y; model.Z = input.Z; model.W = input.W;
        }
        return result;
    }

    ///

    public static T New<T>(double all) where T : ColorModel => (T)New(typeof(T), all);

    public static T New<T>(double x, double y) where T : ColorModel2 => (T)New(typeof(T), x, y);

    public static T New<T>(double x, double y, double z) where T : ColorModel3 => (T)New(typeof(T), x, y, z);

    public static T New<T>(double x, double y, double z, double w) where T : ColorModel4 => (T)New(typeof(T), x, y, z, w);

    public static T New<T>(Vector input)  where T : ColorModel => (T)New(typeof(T), input);

    public static T New<T>(Vector2 input) where T : ColorModel2 => (T)New(typeof(T), input);

    public static T New<T>(Vector3 input) where T : ColorModel3 => (T)New(typeof(T), input);

    public static T New<T>(Vector4 input) where T : ColorModel4 => (T)New(typeof(T), input);

    #endregion

    #region Range

    public static Range<Vector> Range(Type i) => new(Minimum(i), Maximum(i));

    public static Range<Vector> Range<T>() where T : ColorModel => Range(typeof(T));

    #endregion

    public static string FindName(ByteVector4 input)
    {
        List<Type> types = new()
        {
            typeof(BasicColors), typeof(CSSColors),
            typeof(Colors1), typeof(Colors2), typeof(Colors3), typeof(Colors4), typeof(Colors5)
        };

        foreach (var i in types)
        {
            var fields = i.GetFields();
            if (fields?.Length > 0)
            {
                foreach (var j in fields)
                {
                    if (j.GetValue(null).To<string>().ToLower() == input.ToString(false).ToLower())
                        return j.Name.SplitCamel();
                }
            }
        }

        return null;
    }
}