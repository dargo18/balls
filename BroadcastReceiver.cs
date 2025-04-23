using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Android.OS;
using AndroidX.Core.App;

namespace snake
{
    [BroadcastReceiver]
    [IntentFilter(new string[] { "com.snakegame.PLAY_REMINDER" })]
    public class ReminderReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            // Create the notification
            ShowNotification(context);
        }

        private void ShowNotification(Context context)
        {
            // Create notification channel (required for Android 8.0 and above)
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                var channel = new NotificationChannel(
                    "reminder_channel",
                    "Game Reminder",
                    NotificationImportance.Default
                );
                var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
                notificationManager.CreateNotificationChannel(channel);
            }

            // Build the notification
            var builder = new NotificationCompat.Builder(context, "reminder_channel")
                .SetContentTitle("Time to Play Snake!")
                .SetContentText("Start your day with a fun game of Snake!")
                .SetAutoCancel(true)
                .SetPriority(NotificationCompat.PriorityHigh);

            // Create the pending intent that opens the Snake game activity when tapped
            var intent = new Intent(context, typeof(MainActivity)); // Change to your activity name
            var pendingIntent = PendingIntent.GetActivity(context, 0, intent, PendingIntentFlags.OneShot);

            builder.SetContentIntent(pendingIntent);

            // Get the notification manager
            var notificationManagerCompat = NotificationManagerCompat.From(context);
            notificationManagerCompat.Notify(0, builder.Build());
        }
    }
}
