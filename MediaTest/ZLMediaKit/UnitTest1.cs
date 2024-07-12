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
        Thread.Sleep(1000 * 60);
    }

    [Test]
    public void Test2()
    {
        MediaCore.Sources.Screen s = new();
        int i = 0;
        while (true)
        {
            Thread.Sleep(1000);
            s.Capture();
            i++;
            if (i > 10)
                break;
        }
    }
}