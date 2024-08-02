using MediaCore.Encoders;
using MediaCore.ZLMediaKit;
using Sdcb.FFmpeg.Swscales;

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
        var config = new mk_config();
        MkCommon.mk_env_init(ref config);
        var port = MkCommon.mk_rtsp_server_start(554, 0);
        IntPtr ptr = MkMedia.mk_media_create("__defaultVhost__", "live", "test", 0, 0, 0);
        MkMedia.mk_media_init_video(ptr, 0, 1920, 1080, 25, 4 * 1024 * 1024);
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
               var result = MkMedia.mk_media_input_h264(ptr, packet.Data.Pointer,packet.Data.Length, (UInt32)packet.Dts, (UInt32)packet.Dts);
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