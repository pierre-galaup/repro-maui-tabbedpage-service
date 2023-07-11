using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;

namespace MauiApp1;

[Service(
    Exported = false,
    ForegroundServiceType = ForegroundService.TypeLocation,
    Label = "Service"
)]
public class NotificationBarService : Service
{
    public static string AppName { get; private set; }
    public static string NotificationChannelId { get; private set; }
    public static int ServiceRunningNotificationId { get; private set; }
    public static int LargeIcon { get; private set; }
    public static int SmallIcon { get; private set; }

    public static Context Context { get; private set; }
    public static NotificationBarService Current { get; private set; }

    public NotificationManager NotificationManager { get; private set; }


    public static async void Init(string appName, string notificationChannelId, int serviceRunningNotificationId, int largeIcon, int smallIcon)
    {
        AppName = appName;
        LargeIcon = largeIcon;
        SmallIcon = smallIcon;
        NotificationChannelId = notificationChannelId;
        ServiceRunningNotificationId = serviceRunningNotificationId;
    }

    public override IBinder OnBind(Intent intent)
    {
        return null;
    }

    public override void OnDestroy()
    {
        if (Current == this)
            Current = null;

        base.OnDestroy();
    }

    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    {
        if (Current != null)
            return StartCommandResult.Sticky;

        Current = this;
        Context = MainActivity.Current;
        
        var channelId = $"{PackageName}.service";
        var channel = new NotificationChannel(channelId, "Test", NotificationImportance.Default)
        {
            Description = "channelDescription",
            LockscreenVisibility = NotificationVisibility.Private,
        };

        var notificationManager = (NotificationManager)GetSystemService(NotificationService);
        notificationManager?.CreateNotificationChannel(channel);

        using var notificationBuilder = new NotificationCompat.Builder(this, channelId);
        notificationBuilder
            .SetContentTitle("Test")
            .SetSmallIcon(Resource.Drawable.notification_bg)
            .SetContentText("Description...")
            .SetOngoing(true)
            .SetCategory(Notification.CategoryService)
            .SetContentIntent(BuildIntentToShowMainActivity());

        NotificationManager = (NotificationManager)GetSystemService(NotificationService);

        var notification = notificationBuilder.Build();

        // Register this service as foreground service
        StartForeground(ServiceRunningNotificationId, notification);

        return StartCommandResult.Sticky;
    }

    private PendingIntent BuildIntentToShowMainActivity()
    {
        var notificationIntent = new Intent(this, typeof(MainActivity));
        var pendingIntent = PendingIntent.GetActivity(this, 0, notificationIntent, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Mutable);
        return pendingIntent;
    }
}

public class PushNotificationsPermission : Permissions.BasePlatformPermission
{
    public override (string androidPermission, bool isRuntime)[] RequiredPermissions => new List<(string androidPermission, bool isRuntime)>
    {
        (Android.Manifest.Permission.PostNotifications, true)
    }.ToArray();
}