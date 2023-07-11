using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace MauiApp1;

[Activity(LaunchMode = LaunchMode.SingleTask, AlwaysRetainTaskState = true, Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public static MainActivity Current;

    protected override async void OnCreate(Bundle savedInstanceState)
    {
        Current = this;
        base.OnCreate(savedInstanceState);

        await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        var perm = new PushNotificationsPermission();
        await perm.RequestAsync();
        
        NotificationBarService.Init("MauiApp1", this.PackageName, 100, Resource.Mipmap.appicon, Resource.Drawable.notification_bg);
        StartNotificationBar();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    public static void StopNotificationBar()
    {
        var activity = Current;
        if (activity == null)
            return;

        activity.StopService(new Intent(activity, typeof(NotificationBarService)));
    }

    public static void StartNotificationBar()
    {
        if (NotificationBarService.Current != null)
            return;

        var activity = Current;
        if (activity == null)
            return;

        if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            activity.StartService(new Intent(activity, typeof(NotificationBarService)));
        else
            activity.StartForegroundService(new Intent(activity, typeof(NotificationBarService)));
    }

}
