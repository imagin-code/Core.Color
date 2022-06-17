﻿using static System.Math;

namespace Imagin.Core.Colors;

/// <summary>
/// <b>Hybrid Log-Gamma (HLG)</b>
/// <para>A hybrid transfer function developed by NHK and BBC for HDR and offering some backward compatibility on SDR displays. The lower half of the signal values use a gamma curve and the upper half of the signal values use a logarithmic curve.</para>
/// <para><b>Rec. 2100</b></para>
/// </summary>
/// <remarks>https://en.wikipedia.org/wiki/Hybrid_log%E2%80%93gamma</remarks>
[DisplayName(name), Index(2)]
public struct LogGammaCompression : ICompress
{
    const string name = "Log-Gamma";

    [Index(-1), Label(false), ReadOnly, Visible]
    public string Name => name;

    public LogGammaCompression() { }

    public double Transfer(double E)
    {
        var r = 0.5;

        var a = 0.17883277;
        var b = 1 - 4 * a;
        var c = 0.5 - a * Log(4 * a);

        if (E >= 0 && E <= 1)
            return r * Sqrt(E);

        return a * Log(E - b) + c; //1 < E
    }

    public double TransferInverse(double E)
    {
        var a = 0.17883277;
        var b = 1 - 4 * a;
        var c = 0.5 - a * Log(4 * a);

        if (E >= 0 && E <= 1 / 12)
            return Sqrt(3 * E);

        return a * Log(12 * E - b) + c; //1 / 12 < E <= 1
    }
}