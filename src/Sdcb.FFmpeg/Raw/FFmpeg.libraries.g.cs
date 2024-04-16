using System;
using System.Runtime.InteropServices;

#pragma warning disable 169
#pragma warning disable CS0649
#pragma warning disable CS0108
namespace Sdcb.FFmpeg.Raw
{
    using System.Collections.Generic;
    
    public unsafe static partial class ffmpeg
    {
        public static Dictionary<string, int> LibraryVersionMap =  new ()
        {
            ["avcodec"] = 61,
            ["avdevice"] = 61,
            ["avfilter"] = 10,
            ["avformat"] = 61,
            ["avutil"] = 59,
            ["postproc"] = 58,
            ["swresample"] = 5,
            ["swscale"] = 8,
        };
    }
}
