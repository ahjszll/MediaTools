using System.Runtime.InteropServices;
using System.Text;

namespace MediaCore.ZLMediaKit;

public class MkMedia
{
    [DllImport("mk_api", EntryPoint = "mk_media_create")]
    public static extern void mk_media_create(string vhost,string app,string stream,float duration,int hls_enabled,int mp4_enabled);
}