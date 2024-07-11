using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using AndroidX.Core.App;

namespace MauiApp2.Platforms.Android
{
    [Service]
    public class LocationBackgroundService : Service
    {
        private CancellationTokenSource _cancellationTokenSource;

        public override void OnCreate()
        {
            base.OnCreate();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            StartForegroundService();

            Task.Run(() =>
            {
                GetLocationUpdates(_cancellationTokenSource.Token);
            });

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            base.OnDestroy();
        }

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        private void StartForegroundService()
        {
            const int NOTIFICATION_ID = 1001;
            const string CHANNEL_ID = "location_channel";

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationChannel channel = new NotificationChannel(CHANNEL_ID, "Location Service", NotificationImportance.Default)
                {
                    Description = "Channel for Location Service"
                };
                NotificationManager manager = (NotificationManager)GetSystemService(NotificationService);
                manager.CreateNotificationChannel(channel);
            }

            Notification notification = new NotificationCompat.Builder(this, CHANNEL_ID)
                .SetContentTitle("Location Service")
                .SetContentText("Getting location updates")
                .SetSmallIcon(Resource.Drawable.abc_btn_check_material) // Asegúrate de tener un ícono válido aquí
                .SetOngoing(true)
                .Build();

            StartForeground(NOTIFICATION_ID, notification);
        }

        private async Task GetLocationUpdates(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                var location = await Geolocation.GetLastKnownLocationAsync();
                if (location != null)
                {
                    // Mostrar un toast con las coordenadas
                    string message = $"Lat: {location.Latitude}, Lon: {location.Longitude}";
                    ShowToast(message);
                }
                else
                {
                    ShowToast("Unable to get location");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), token); // Espera 5 segundos antes de la próxima actualización
            }
        }

        private void ShowToast(string message)
        {
            // Mostrar un toast en el hilo principal
            var handler = new Handler(Looper.MainLooper);
            handler.Post(() =>
            {
                Toast.MakeText(ApplicationContext, message, ToastLength.Long).Show();
            });
        }
    }
}
