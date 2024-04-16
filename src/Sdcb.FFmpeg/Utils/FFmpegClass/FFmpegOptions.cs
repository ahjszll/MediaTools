﻿using System;
using System.Collections.Generic;
using Sdcb.FFmpeg.Raw;
using static Sdcb.FFmpeg.Raw.ffmpeg;
using System.Linq;
using Sdcb.FFmpeg.Common;

namespace Sdcb.FFmpeg.Utils;

public unsafe class FFmpegOptions
{
    private readonly void* _obj;

    public FFmpegOptions(void* ptr)
    {
        if (ptr == null)
        {
            throw new ArgumentNullException(nameof(ptr));
        }
        _obj = ptr;
    }

    public FFmpegOptions(IntPtr ptr)
    {
        if (ptr == IntPtr.Zero)
        {
            throw new ArgumentNullException(nameof(ptr));
        }
        _obj = (void*)ptr;
    }

    /// <summary>
    /// <see cref="av_opt_find(void*, string, string, int, int)"/>
    /// </summary>
    /// <returns></returns>
    public FFmpegOption? Find(string name, string unit, AV_OPT_FLAG AV_OPT_FLAG = default, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children)
    {
        AVOption* val = av_opt_find(_obj, name, unit, (int)AV_OPT_FLAG, (int)searchFlags);
        if (val == null) return null;
        return new FFmpegOption(val);
    }

    /// <summary>
    /// <para>Look for an option in an object. Consider only options which have all the specified flags set.</para>
    /// <see cref="av_opt_find2(void*, string, string, int, int, void**)"/>
    /// </summary>
    public (FFmpegOption? option, IntPtr @object) Find2(string name, string unit, AV_OPT_FLAG AV_OPT_FLAG = default, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children)
    {
        void* obj;
        AVOption* val = av_opt_find2(_obj, name, unit, (int)AV_OPT_FLAG, (int)searchFlags, &obj);
        if (val == null) return (null, IntPtr.Zero);
        return (new FFmpegOption(val), (IntPtr)obj);
    }

    public void SetDefaults(AV_OPT_FLAG mask = default, AV_OPT_FLAG flags = default) => av_opt_set_defaults2(_obj, (int)mask, (int)flags);

    /// <summary>
    /// <see cref="av_opt_set(void*, string, string, int)"/>
    /// </summary>
    public void Set(string name, string? value, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children) => 
        av_opt_set(_obj, name, value, (int)searchFlags).ThrowIfError();

    /// <summary>
    /// <see cref="av_opt_set_int(void*, string, long, int)"/>
    /// </summary>
    public void Set(string name, long value, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children) => 
        av_opt_set_int(_obj, name, value, (int)searchFlags).ThrowIfError();

    /// <summary>
    /// <see cref="av_opt_set_double(void*, string, double, int)"/>
    /// </summary>
    public void Set(string name, double value, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children) => 
        av_opt_set_double(_obj, name, value, (int)searchFlags).ThrowIfError();

    /// <summary>
    /// <see cref="av_opt_set_q(void*, string, AVRational, int)"/>
    /// </summary>
    public void Set(string name, AVRational value, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children) => 
        av_opt_set_q(_obj, name, value, (int)searchFlags).ThrowIfError();

    /// <summary>
    /// <see cref="av_opt_set_bin(void*, string, byte*, int, int)"/>
    /// </summary>
    public void Set(string name, DataPointer value, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children) => 
        av_opt_set_bin(_obj, name, (byte*)value.Pointer, value.Length, (int)searchFlags).ThrowIfError();

    /// <summary>
    /// <see cref="av_opt_set_bin(void*, string, byte*, int, int)"/>
    /// </summary>
    public void Set(string name, int[] value, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children)
    {
        fixed (int* ptr = value)
        {
            av_opt_set_bin(_obj, name, (byte*)ptr, value.Length * 4, (int)searchFlags).ThrowIfError();
        }
    }

    /// <summary>
    /// <see cref="av_opt_set_chlayout"/>
    /// </summary>
    public void Set(string name, AVChannelLayout value, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children)
    {
        av_opt_set_chlayout(_obj, name, &value, (int)searchFlags).ThrowIfError();
    }

    /// <summary>
    /// <see cref="av_opt_set_bin(void*, string, byte*, int, int)"/>
    /// </summary>
    public void Set(string name, ulong[] value, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children)
    {
        fixed (ulong* ptr = value)
        {
            av_opt_set_bin(_obj, name, (byte*)ptr, value.Length * 8, (int)searchFlags).ThrowIfError();
        }
    }

    /// <summary>
    /// <see cref="av_opt_set_image_size(void*, string, int, int, int)"/>
    /// </summary>
    public void Set(string name, int width, int height, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children) => 
        av_opt_set_image_size(_obj, name, width, height, (int)searchFlags).ThrowIfError();

    /// <summary>
    /// <see cref="av_opt_set_pixel_fmt(void*, string, AVPixelFormat, int)"/>
    /// </summary>
    public void Set(string name, AVPixelFormat value, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children) => 
        av_opt_set_pixel_fmt(_obj, name, value, (int)searchFlags).ThrowIfError();

    /// <summary>
    /// <see cref="av_opt_set_sample_fmt(void*, string, AVSampleFormat, int)"/>
    /// </summary>
    public void Set(string name, AVSampleFormat value, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children) => 
        av_opt_set_sample_fmt(_obj, name, (AVSampleFormat)value, (int)searchFlags).ThrowIfError();

    /// <summary>
    /// <see cref="av_opt_set_video_rate(void*, string, AVRational, int)"/>
    /// </summary>
    public void SetVideoRate(string name, AVRational value, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children) => 
        av_opt_set_video_rate(_obj, name, value, (int)searchFlags).ThrowIfError();

    /// <summary>
    /// <see cref="av_opt_set_dict_val(void*, string, AVDictionary*, int)"/>
    /// </summary>
    public void Set(string name, MediaDictionary value, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children) => 
        av_opt_set_dict_val(_obj, name, value, (int)searchFlags).ThrowIfError();

    /// <summary>
    /// <see cref="av_opt_get(void*, string, int, byte**)"/>
    /// </summary>
    public DisposableNativeString GetData(string name, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children)
    {
        byte* outVal;
        av_opt_get(_obj, name, (int)searchFlags, &outVal).ThrowIfError($"name: {name}");
        return new DisposableNativeString(outVal);
    }

    /// <summary>
    /// <see cref="av_opt_get_int(void*, string, int, long*)"/>
    /// </summary>
    public long GetInt64(string name, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children)
    {
        long val;
        av_opt_get_int(_obj, name, (int)searchFlags, &val).ThrowIfError();
        return val;
    }

    /// <summary>
    /// <see cref="av_opt_get_double(void*, string, int, double*)"/>
    /// </summary>
    public double GetDouble(string name, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children)
    {
        double val;
        av_opt_get_double(_obj, name, (int)searchFlags, &val).ThrowIfError();
        return val;
    }

    /// <summary>
    /// <see cref="av_opt_get_q(void*, string, int, AVRational*)"/>
    /// </summary>
    public AVRational GetRational(string name, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children)
    {
        AVRational rational;
        av_opt_get_q(_obj, name, (int)searchFlags, &rational).ThrowIfError();
        return rational;
    }

    /// <summary>
    /// <see cref="av_opt_get_image_size(void*, string, int, int*, int*)"/>
    /// </summary>
    public (int width, int height) GetImageSize(string name, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children)
    {
        int width, height;
        av_opt_get_image_size(_obj, name, (int)searchFlags, &width, &height).ThrowIfError();
        return (width, height);
    }

    /// <summary>
    /// <see cref="av_opt_get_pixel_fmt(void*, string, int, AVPixelFormat*)"/>
    /// </summary>
    public AVPixelFormat GetPixelFormat(string name, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children)
    {
        AVPixelFormat pixelFormat;
        av_opt_get_pixel_fmt(_obj, name, (int)searchFlags, &pixelFormat);
        return pixelFormat;
    }

    /// <summary>
    /// <see cref="av_opt_get_video_rate(void*, string, int, AVRational*)"/>
    /// </summary>
    public AVRational GetVideoRate(string name, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children)
    {
        AVRational rational;
        av_opt_get_video_rate(_obj, name, (int)searchFlags, &rational).ThrowIfError();
        return rational;
    }

    /// <summary>
    /// <see cref="av_opt_get_dict_val(void*, string, int, AVDictionary**)"/>
    /// </summary>
    public MediaDictionary GetDictionary(string name, AV_OPT_SEARCH searchFlags = AV_OPT_SEARCH.Children)
    {
        AVDictionary* dict;
        av_opt_get_dict_val(_obj, name, (int)searchFlags, &dict);
        return MediaDictionary.FromNative(dict, isOwner: true);
    }

    public IEnumerable<FFmpegOption> ConstValues => GetKnownOptions()
        .Where(x => x.Type == AVOptionType.Const);

    public IEnumerable<FFmpegOption> Options => GetKnownOptions()
        .Where(x => x.Type != AVOptionType.Const);

    public Dictionary<string, string> Values => GetKnownOptions()
        .Where(x => x.Type != AVOptionType.Const)
        .ToDictionary(k => k.Name, v =>
    {
        using DisposableNativeString data = GetData(v.Name);
        return data.ToString()!;
    });

    public IEnumerable<FFmpegOptions> Children => GetChildren();

    /// <summary>
    /// <see cref="av_opt_next(void*, AVOption*)"/>
    /// </summary>
    public IEnumerable<FFmpegOption> GetKnownOptions()
    {
        IntPtr prev = IntPtr.Zero;
        while (true)
        {
            prev = av_opt_next_safe(prev);
            if (prev == IntPtr.Zero) break;
            yield return new FFmpegOption(prev);
        }

        IntPtr av_opt_next_safe(IntPtr prev) => (IntPtr)av_opt_next(_obj, (AVOption*)prev);
    }

    private IEnumerable<FFmpegOptions> GetChildren()
    {
        IntPtr prev = IntPtr.Zero;
        while (true)
        {
            prev = av_opt_child_next_safe(prev);
            if (prev == IntPtr.Zero) break;
            yield return new FFmpegOptions(prev);
        }

        IntPtr av_opt_child_next_safe(IntPtr prev) => (IntPtr)av_opt_child_next(_obj, (AVOption*)prev);
    }
}
