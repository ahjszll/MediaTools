using MediaCore.Encoders;
using MediaCore.ZLMediaKit;

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
        IntPtr ptr=MkMedia.mk_media_create("", "live", "test", 0, 0, 0);
        MkMedia.mk_media_start_send_rtp(ptr, "127.0.0.1", 554, "test", 1, delegateCallBack, IntPtr.Zero);
        
        Thread.Sleep(1000 * 60);
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