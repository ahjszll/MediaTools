using MediaCore.Sources;
using Sdcb.FFmpeg.Codecs;
using Sdcb.FFmpeg.Formats;
using Sdcb.FFmpeg.Raw;
using Sdcb.FFmpeg.Toolboxs.Extensions;
using Sdcb.FFmpeg.Utils;
using SkiaSharp;

namespace MediaCore.Encoders;

public class H264
{
    public static void ddd()
    {
        using FormatContext fc = FormatContext.AllocOutput(formatName: "mp4");
        fc.VideoCodec = Codec.CommonEncoders.Libx264;
        MediaStream vStream = fc.NewStream(fc.VideoCodec);
        using CodecContext vcodec = new CodecContext(fc.VideoCodec)
        {
            Width = 2560,
            Height = 1440,
            PixelFormat = AVPixelFormat.Yuv420p,
            TimeBase = new AVRational(1, 25),
            Flags  = AV_CODEC_FLAG.GlobalHeader,
        };
        vcodec.Open(fc.VideoCodec);
        vStream.Codecpar!.CopyFrom(vcodec);
        vStream.TimeBase=vcodec.TimeBase;
        
        string outPath = "test.mp4";
        using IOContext io = IOContext.OpenWrite(outPath);
        fc.Pb = io;
        fc.WriteHeader();
        Screen s = new Screen();
        List<Frame> list = new List<Frame>();
        List<SKBitmap> imgList = new List<SKBitmap>();
        for (int i = 0; i < 100; i++)
        {
            var skImage = s.Capture();
            SKBitmap bitmap = SKBitmap.FromImage(skImage);
            Frame frame = new Frame();
            frame.Data[0] = bitmap.GetAddress(0,0);
            frame.Linesize[0] = skImage.Width * 4;
            frame.Format = (int)AVPixelFormat.Bgra;
            frame.Width = skImage.Width;
            frame.Height = skImage.Height;
            list.Add(frame);
            imgList.Add(bitmap);
        }
        var packs = list.ConvertFrames(vcodec).EncodeFrames(vcodec);
        foreach (var pack in packs)
        {
            fc.WritePacket(pack);
        }
        fc.WriteTrailer();
    }
}