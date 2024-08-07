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