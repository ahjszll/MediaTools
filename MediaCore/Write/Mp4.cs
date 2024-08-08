using Sdcb.FFmpeg.Codecs;
using Sdcb.FFmpeg.Formats;
using Sdcb.FFmpeg.Raw;
using Sdcb.FFmpeg.Toolboxs.Extensions;
using Sdcb.FFmpeg.Toolboxs.Generators;

namespace MediaCore.Write;

public class Mp4
{
    private FormatContext _formatContext;
    private MediaStream _videoStream;
    private IOContext _io;
    public Mp4()
    {
        _formatContext = FormatContext.AllocOutput(formatName: "mp4");
        _formatContext.VideoCodec = Codec.CommonEncoders.Libx264;
        _videoStream = _formatContext.NewStream(_formatContext.VideoCodec);
        using CodecContext vcodec = new CodecContext(_formatContext.VideoCodec)
        {
            Width = 1920,
            Height = 1080,
            PixelFormat = AVPixelFormat.Yuv420p,
            TimeBase = new AVRational(1, 25),
            Flags = AV_CODEC_FLAG.GlobalHeader
        };
        vcodec.Open(_formatContext.VideoCodec);
        _videoStream.Codecpar!.CopyFrom(vcodec);
        _videoStream.TimeBase = vcodec.TimeBase;
        
        _formatContext.DumpFormat(0,"d:\\123\\thtest.mp4",true);
        _io=IOContext.OpenWrite("d:\\123\\thtest.mp4");
        _formatContext.Pb = _io;
        
        _formatContext.WriteHeader();
    }

    public void WriteVideo(Packet video)
    {
        video.StreamIndex = 0;
        video.RescaleTimestamp(new AVRational(1, 25),_videoStream.TimeBase);
        //_formatContext.WritePacket(video);
    }

    public void Complate()
    {
         _formatContext.WriteTrailer();
        // _formatContext.Dispose();
        // _io.Dispose();
    }
}