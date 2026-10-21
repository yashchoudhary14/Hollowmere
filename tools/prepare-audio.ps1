<#
.SYNOPSIS
  Cuts the clips listed in tools/audio-sources.json out of the Sonniss bundle into Assets/Hollowmere/Audio.

.DESCRIPTION
  For each clip: read only the needed range (or start at the Nth detected transient), downsample to 48 kHz,
  optionally mix to mono, crossfade the end into the start for loops, apply fades, normalise to -1 dBFS and
  write 16-bit PCM WAV. The bundle itself is never copied into the project. Existing outputs are skipped
  unless -Force is given.

.EXAMPLE
  powershell -NoProfile -ExecutionPolicy Bypass -File tools/prepare-audio.ps1
#>
param([switch]$Force)

$ErrorActionPreference = 'Stop'
$Root = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
$Manifest = Get-Content (Join-Path $PSScriptRoot 'audio-sources.json') -Raw | ConvertFrom-Json
$Bundle = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot $Manifest.bundle))
if (-not (Test-Path -LiteralPath $Bundle)) { throw "Sonniss bundle not found at $Bundle" }

Add-Type -TypeDefinition @'
using System;
using System.Collections.Generic;
using System.IO;

public static class WavTool
{
    public sealed class Clip { public float[][] Ch; public int Rate; }

    sealed class Header { public int Format, Channels, Rate, Bits, BlockAlign; public long DataStart, DataBytes; }

    static Header ReadHeader(BinaryReader br)
    {
        var fs = br.BaseStream;
        if (new string(br.ReadChars(4)) != "RIFF") throw new InvalidDataException("not RIFF");
        br.ReadUInt32();
        if (new string(br.ReadChars(4)) != "WAVE") throw new InvalidDataException("not WAVE");
        var h = new Header();
        while (fs.Position < fs.Length - 8)
        {
            string id = new string(br.ReadChars(4));
            long size = br.ReadUInt32();
            long next = fs.Position + size + (size % 2);
            if (id == "fmt ")
            {
                h.Format = br.ReadUInt16(); h.Channels = br.ReadUInt16(); h.Rate = (int)br.ReadUInt32();
                br.ReadUInt32(); h.BlockAlign = br.ReadUInt16(); h.Bits = br.ReadUInt16();
                if (h.Format == 0xFFFE && size >= 40) { br.ReadUInt16(); br.ReadUInt16(); br.ReadUInt32(); h.Format = br.ReadUInt16(); }
            }
            else if (id == "data") { h.DataStart = fs.Position; h.DataBytes = Math.Min(size, fs.Length - fs.Position); break; }
            fs.Position = next;
        }
        if (h.Rate == 0 || h.DataStart == 0) throw new InvalidDataException("no fmt/data chunk");
        return h;
    }

    public static double Duration(string path)
    {
        using (var br = new BinaryReader(File.OpenRead(path))) { var h = ReadHeader(br); return (double)h.DataBytes / h.BlockAlign / h.Rate; }
    }

    /// <summary>Reads [start, start + length) seconds; length below 0 reads to the end.</summary>
    public static Clip Read(string path, double start, double length)
    {
        using (var br = new BinaryReader(File.OpenRead(path)))
        {
            var h = ReadHeader(br);
            long total = h.DataBytes / h.BlockAlign;
            long first = Math.Max(0, Math.Min(total, (long)(start * h.Rate)));
            long count = length < 0 ? total - first : Math.Min(total - first, (long)(length * h.Rate));
            br.BaseStream.Position = h.DataStart + first * h.BlockAlign;
            var bytes = br.ReadBytes((int)(count * h.BlockAlign));
            count = bytes.Length / h.BlockAlign;
            var ch = new float[h.Channels][];
            for (int c = 0; c < h.Channels; c++) ch[c] = new float[count];
            int bps = h.Bits / 8;
            for (long f = 0; f < count; f++)
                for (int c = 0; c < h.Channels; c++)
                    ch[c][f] = Sample(bytes, (int)(f * h.BlockAlign + c * bps), h.Bits, h.Format);
            return new Clip { Ch = ch, Rate = h.Rate };
        }
    }

    static float Sample(byte[] b, int i, int bits, int format)
    {
        switch (bits)
        {
            case 16: return BitConverter.ToInt16(b, i) / 32768f;
            case 24: return ((b[i] | (b[i + 1] << 8) | ((sbyte)b[i + 2] << 16))) / 8388608f;
            case 32: return format == 3 ? BitConverter.ToSingle(b, i) : BitConverter.ToInt32(b, i) / 2147483648f;
            case 8: return (b[i] - 128) / 128f;
            default: throw new NotSupportedException(bits + "-bit audio");
        }
    }

    /// <summary>Start times (s) of transients in the first maxSeconds: 10 ms RMS rising past a share of the loudest window after a quiet gap.</summary>
    public static double[] Onsets(string path, double maxSeconds)
    {
        var clip = Read(path, 0, maxSeconds);
        int win = clip.Rate / 100, n = clip.Ch[0].Length / win;
        var rms = new double[n];
        double peak = 0;
        for (int w = 0; w < n; w++)
        {
            double s = 0;
            for (int i = w * win; i < (w + 1) * win; i++) foreach (var c in clip.Ch) s += c[i] * c[i];
            rms[w] = Math.Sqrt(s / (win * clip.Ch.Length));
            peak = Math.Max(peak, rms[w]);
        }
        var result = new List<double>();
        double threshold = peak * 0.3;
        int quiet = 0;
        for (int w = 0; w < n; w++)
        {
            if (rms[w] < threshold * 0.5) quiet++;
            else if (rms[w] >= threshold && (quiet >= 30 || result.Count == 0 && w == 0 || quiet >= 30))
            {
                result.Add(Math.Max(0, w * 0.01 - 0.02));
                quiet = 0;
            }
            else if (rms[w] >= threshold) quiet = 0;
        }
        return result.ToArray();
    }

    public static Clip To48k(Clip clip)
    {
        const int target = 48000;
        if (clip.Rate == target) return clip;
        var outCh = new float[clip.Ch.Length][];
        if (clip.Rate % target == 0)
        {
            int k = clip.Rate / target;
            for (int c = 0; c < clip.Ch.Length; c++)
            {
                var src = clip.Ch[c]; var dst = new float[src.Length / k];
                for (int i = 0; i < dst.Length; i++) { float s = 0; for (int j = 0; j < k; j++) s += src[i * k + j]; dst[i] = s / k; }
                outCh[c] = dst;
            }
        }
        else
        {
            double step = (double)clip.Rate / target;
            for (int c = 0; c < clip.Ch.Length; c++)
            {
                var src = clip.Ch[c]; var dst = new float[(int)(src.Length / step)];
                for (int i = 0; i < dst.Length; i++)
                {
                    double p = i * step; int a = (int)p; int b = Math.Min(a + 1, src.Length - 1); double t = p - a;
                    dst[i] = (float)(src[a] * (1 - t) + src[b] * t);
                }
                outCh[c] = dst;
            }
        }
        return new Clip { Ch = outCh, Rate = target };
    }

    public static Clip ToMono(Clip clip)
    {
        if (clip.Ch.Length == 1) return clip;
        var m = new float[clip.Ch[0].Length];
        for (int i = 0; i < m.Length; i++) { float s = 0; foreach (var c in clip.Ch) s += c[i]; m[i] = s / clip.Ch.Length; }
        return new Clip { Ch = new[] { m }, Rate = clip.Rate };
    }

    /// <summary>Keeps length seconds and blends the following crossfade seconds over the start, so the end runs into the start.</summary>
    public static Clip Loop(Clip clip, double length, double crossfade)
    {
        int len = Math.Min((int)(length * clip.Rate), clip.Ch[0].Length);
        int xf = Math.Min((int)(crossfade * clip.Rate), clip.Ch[0].Length - len);
        var outCh = new float[clip.Ch.Length][];
        for (int c = 0; c < clip.Ch.Length; c++)
        {
            var src = clip.Ch[c]; var dst = new float[len];
            Array.Copy(src, dst, len);
            for (int i = 0; i < xf; i++)
            {
                double w = Math.Sqrt((double)i / xf);
                dst[i] = (float)(src[i] * w + src[len + i] * Math.Sqrt(1 - (double)i / xf));
            }
            outCh[c] = dst;
        }
        return new Clip { Ch = outCh, Rate = clip.Rate };
    }

    public static void Fade(Clip clip, double fadeIn, double fadeOut)
    {
        int n = clip.Ch[0].Length, fi = Math.Min(n, (int)(fadeIn * clip.Rate)), fo = Math.Min(n, (int)(fadeOut * clip.Rate));
        foreach (var c in clip.Ch)
        {
            for (int i = 0; i < fi; i++) c[i] *= (float)i / fi;
            for (int i = 0; i < fo; i++) c[n - 1 - i] *= (float)i / fo;
        }
    }

    public static void Normalise(Clip clip, float peakTarget)
    {
        float peak = 0;
        foreach (var c in clip.Ch) foreach (var s in c) peak = Math.Max(peak, Math.Abs(s));
        if (peak <= 1e-6f) return;
        float g = peakTarget / peak;
        foreach (var c in clip.Ch) for (int i = 0; i < c.Length; i++) c[i] *= g;
    }

    public static void Write(string path, Clip clip)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        int chs = clip.Ch.Length, n = clip.Ch[0].Length, data = n * chs * 2;
        using (var bw = new BinaryWriter(File.Create(path)))
        {
            bw.Write("RIFF".ToCharArray()); bw.Write(36 + data); bw.Write("WAVE".ToCharArray());
            bw.Write("fmt ".ToCharArray()); bw.Write(16); bw.Write((short)1); bw.Write((short)chs);
            bw.Write(clip.Rate); bw.Write(clip.Rate * chs * 2); bw.Write((short)(chs * 2)); bw.Write((short)16);
            bw.Write("data".ToCharArray()); bw.Write(data);
            for (int i = 0; i < n; i++)
                for (int c = 0; c < chs; c++)
                    bw.Write((short)Math.Max(-32768, Math.Min(32767, (int)Math.Round(clip.Ch[c][i] * 32767f))));
        }
    }
}
'@

$made = 0
foreach ($c in $Manifest.clips) {
    $out = Join-Path $Root "Assets/Hollowmere/Audio/$($c.out)"
    if ((Test-Path $out) -and -not $Force) { continue }
    $src = Join-Path (Join-Path $Bundle $c.pack) $c.file
    if (-not (Test-Path -LiteralPath $src)) { Write-Warning "missing source: $($c.pack)/$($c.file)"; continue }

    $start = 0.0
    if ($c.onset) {
        $onsets = [WavTool]::Onsets($src, [Math]::Min(120.0, [WavTool]::Duration($src)))
        if ($onsets.Length -lt $c.onset) { Write-Warning "$($c.out): only $($onsets.Length) onsets"; $start = if ($onsets.Length) { $onsets[-1] } else { 0 } }
        else { $start = $onsets[$c.onset - 1] }
    } elseif ($c.start) { $start = [double]$c.start }

    $loop = if ($c.loop) { [double]$c.loop } else { 0.0 }
    $clip = [WavTool]::Read($src, $start, [double]$c.length + $loop)
    $clip = [WavTool]::To48k($clip)
    if ($c.mono) { $clip = [WavTool]::ToMono($clip) }
    if ($loop -gt 0) { $clip = [WavTool]::Loop($clip, [double]$c.length, $loop) }
    [WavTool]::Fade($clip, [double]($c.fadeIn), [double]($c.fadeOut))
    [WavTool]::Normalise($clip, 0.89)
    [WavTool]::Write($out, $clip)
    $made++
    '{0,-28} from {1,6:N2}s  {2,5:N1}s  {3}ch  {4,5:N1} MB' -f $c.out, $start, ($clip.Ch[0].Length / 48000.0), $clip.Ch.Length, ((Get-Item $out).Length / 1MB)
}
$total = (Get-ChildItem (Join-Path $Root 'Assets/Hollowmere/Audio') -Recurse -File -Filter *.wav | Measure-Object Length -Sum).Sum
Write-Host ("Made {0} clips; Assets/Hollowmere/Audio holds {1:N1} MB of WAV." -f $made, ($total / 1MB))
