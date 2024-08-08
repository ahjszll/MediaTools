using MediaCore.Encoders;
using MediaCore.Write;
using MediaCore.ZLMediaKit;
using Sdcb.FFmpeg.Raw;
using Sdcb.FFmpeg.Swscales;
using SkiaSharp;
using ZLMediaKit;

namespace MediaTest.ZLMediaKit;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        mk_common.MkEnvInit2(10,0,0,"log",1,1,"ini",0,null,null);
        var port = mk_common.MkRtspServerStart(554, 0);
        var media =  mk_media.MkMediaCreate("__defaultVhost__", "live", "test", 0, 0, 0);
        mk_media.MkMediaInitVideo(media, 0, 1920, 1080, 25, 4 * 1024 * 1024);
        mk_media.MkMediaInitComplete(media);
        MediaCore.Sources.Screen s = new();
        H264 encoder = new();
        UInt64 i = 0;
        Mp4 mp4 = new();
        var sss = DateTime.Now;
        while (true)
        {
            i++;
            var bitmap = s.Capture();
            var packet = encoder.Encode(bitmap,(int)i);
            //mp4.WriteVideo(packet);

            if (packet.Data.Length > 0)
            {
                packet.RescaleTimestamp(new AVRational(1, 25),new AVRational(1, 1000));
                var result = mk_media.MkMediaInputH264(media, packet.Data.Pointer, packet.Data.Length,
                    0,0);
            }
            //bitmap.Encode(SKEncodedImageFormat.Png,100).SaveTo(new FileStream($"d:\\123\\{i}.png",FileMode.OpenOrCreate));
            Console.WriteLine($"dur:{packet.Duration}  pos:{packet.Position}  pts:{packet.Pts} timebase:{packet.TimeBase}");
            Thread.Sleep(40);
        }
        mp4.Complate();
    }

    [Test]
    public void Test2()
    {
        MediaCore.Sources.Screen s = new();
        int i = 0;
        while (true)
        {
            Thread.Sleep(100);
            s.Capture();
            i++;
            if (i > 10)
                break;
        }
    }
    
    [Test]
    public void Test3()
    {
        H264.ddd();
    }
    
    public void delegateCallBack(IntPtr user_data, int local_port, int err, string msg)
    {
        
    }
}