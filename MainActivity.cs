using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using Java.Util;
using System.Threading.Tasks;

namespace snake
{
    [Activity(Label = "SnakeGame", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private SnakeView snakeView;
        private DatabaseService _dbService;
        private EditText _usernameEntry;
        private EditText _passwordEntry;
        private Button _loginButton;
        private Button _registerButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            // Initialize DatabaseService and UI components
            _dbService = new DatabaseService();

            _usernameEntry = FindViewById<EditText>(Resource.Id.usernameEntry);
            _passwordEntry = FindViewById<EditText>(Resource.Id.passwordEntry);
            _loginButton = FindViewById<Button>(Resource.Id.loginButton);
            _registerButton = FindViewById<Button>(Resource.Id.registerButton);

            _loginButton.Click += async (sender, e) => await OnLoginClicked();
            _registerButton.Click += async (sender, e) => await OnRegisterClicked();

            // Set up the daily reminder
            SetDailyReminder();
        }

        private async Task OnLoginClicked()
        {
            var user = await _dbService.LoginUser(_usernameEntry.Text, _passwordEntry.Text);
            if (user != null)
            {
                Toast.MakeText(this, "Login Success! Welcome " + user.Username, ToastLength.Short).Show();

                // Set the SnakeView as the main view after login
                RunOnUiThread(() => SetContentView(new SnakeView(this, user.Username)));
            }
            else
            {
                Toast.MakeText(this, "Login Failed!", ToastLength.Short).Show();
            }
        }

        private async Task OnRegisterClicked()
        {
            StartActivity(typeof(RegisterActivity));
        }

        private void StartGame(string username)
        {
            snakeView = new SnakeView(this, username);
            SetContentView(snakeView);
        }

        // Function to set up the daily reminder at 8:00 AM
        private void SetDailyReminder()
        {
            var alarmManager = (AlarmManager)GetSystemService(Context.AlarmService);
            var intent = new Intent(this, typeof(ReminderReceiver));
            intent.SetAction("com.snakegame.PLAY_REMINDER");

            // Set the time to trigger the alarm (e.g., 8:00 AM)
            var calendar = Calendar.GetInstance(Java.Util.TimeZone.Default);
            calendar.Set(CalendarField.HourOfDay, 8);
            calendar.Set(CalendarField.Minute, 0);
            calendar.Set(CalendarField.Second, 0);

            // Set the alarm to repeat daily
            var pendingIntent = PendingIntent.GetBroadcast(this, 0, intent, PendingIntentFlags.UpdateCurrent);
            alarmManager.SetRepeating(AlarmType.RtcWakeup, calendar.TimeInMillis, AlarmManager.IntervalDay, pendingIntent);

            Toast.MakeText(this, "Daily reminder set!", ToastLength.Short).Show();
        }
    }
}
