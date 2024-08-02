using System.Runtime.InteropServices;
using System.Text;

namespace MediaCore.ZLMediaKit;

public class MkMedia
{
    
   public delegate void delegateCallBack(IntPtr user_data, int local_port, int err, string msg);
    
    [DllImport("mk_api", EntryPoint = "mk_media_create")]
    public static extern IntPtr mk_media_create(string vhost, string app, string stream, float duration, int hls_enabled,
        int mp4_enabled);


    [DllImport("mk_api", EntryPoint = "mk_media_init_video")]
    public static extern int mk_media_init_video(IntPtr ctx, int codec_id, int width, int height, float fps,
        int bit_rate);

    [DllImport("mk_api", EntryPoint = "mk_media_init_audio")]
    public static extern int mk_media_init_audio(IntPtr ctx, int codec_id, int sample_rate, int channels,
        int sample_bit);
    
    [DllImport("mk_api", EntryPoint = "mk_media_start_send_rtp")]
    public static extern void  mk_media_start_send_rtp(IntPtr ctx, string dst_url, int dst_port, string ssrc, int is_udp, delegateCallBack cb, IntPtr user_data);
    
    
    
    [DllImport("mk_api", EntryPoint = "mk_media_input_h264")]
    public static extern int  mk_media_input_h264(IntPtr ctx, IntPtr data, int len, UInt64 dts, UInt64 pts);


}