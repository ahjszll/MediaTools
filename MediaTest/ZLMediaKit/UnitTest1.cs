using MediaCore.Encoders;
using MediaCore.ZLMediaKit;
using Sdcb.FFmpeg.Swscales;
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
        var config = new MkConfig();
        mk_common.MkEnvInit(config);
        var port = mk_common.MkRtspServerStart(554, 0);
        var media =  mk_media.MkMediaCreate("__defaultVhost__", "live", "test", 0, 0, 0);
        mk_media.MkMediaInitVideo(media, 0, 1920, 1080, 25, 4 * 1024 * 1024);
        mk_media.MkMediaInitComplete(media);
        MediaCore.Sources.Screen s = new();
        H264 encoder = new();
        UInt64 i = 0;
        while (true)
        {
            i+=40;
            var bitmap = s.Capture();
            var packet = encoder.Encode(bitmap,(int)i);
            if (packet.Data.Length > 0)
            {
                var result = mk_media.MkMediaInputH264(media, packet.Data.Pointer, packet.Data.Length,
                    (UInt32)packet.Dts, (UInt32)packet.Dts);
            }
            Thread.Sleep(40);
        }
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