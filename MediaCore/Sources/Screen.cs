using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ScreenCapture.NET;
using SkiaSharp;

namespace MediaCore.Sources;

public class Screen
{
    private readonly DX11ScreenCaptureService _service;
    readonly IEnumerable<GraphicsCard> _graphicsCards;
    private readonly IEnumerable<Display> _displays;
    private readonly DX11ScreenCapture _screenCapture;

    private CaptureZone<ColorBGRA> _fullscreen;
    private CaptureZone<ColorBGRA> _topLeft;

    
    public Screen()
    {
        _service = new DX11ScreenCaptureService();
        _graphicsCards = _service.GetGraphicsCards();
        _displays   = _service.GetDisplays(_graphicsCards.First());
        _screenCapture= _service.GetScreenCapture(_displays.First());
        _fullscreen = _screenCapture.RegisterCaptureZone(0, 0, _screenCapture.Display.Width, _screenCapture.Display.Height);
        _topLeft = _screenCapture.RegisterCaptureZone(0, 0, 100, 100, downscaleLevel: 1);
    }
    
    public void Capture()
    {
        using (_fullscreen.Lock())
        {
            unsafe
            {
                RefImage<ColorBGRA> image = _fullscreen.Image;
                var intptr = Unsafe.AsPointer(ref _fullscreen.Pixels.GetPinnableReference());
                SKImage skImage = SKImage.FromPixels(new SKImageInfo(image.Width, image.Height, SKColorType.Bgra8888),
                    (IntPtr)intptr, image.RawStride);
                using FileStream fs = new FileStream(DateTime.Now.ToString("HH_mm_ss_fff") + ".jpg", FileMode.Create);
                skImage.Encode().AsStream().CopyTo(fs);
                fs.Flush();
            }
        }
    }
}