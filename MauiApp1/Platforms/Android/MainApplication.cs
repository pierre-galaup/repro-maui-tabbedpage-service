using Android.App;
using Android.Runtime;

namespace MauiApp1;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }

}
