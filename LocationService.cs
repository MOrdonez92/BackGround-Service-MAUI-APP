using CommunityToolkit.Maui.Alerts;
using System.Timers;
using CommunityToolkit.Maui.Alerts;

namespace MauiApp2
{
    public class LocationService
    {
        private System.Timers.Timer _timer;

        public async Task<bool> CheckAndRequestLocationPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            return status == PermissionStatus.Granted;
        }

        public async void Start()
        {
            if (await CheckAndRequestLocationPermission())
            {
                _timer = new System.Timers.Timer(5000); // Cada 5 segundos
                _timer.Elapsed += async (sender, e) => await GetLocation();
                _timer.Start();
            }
            else
            {
                // Manejar el caso en que el permiso no es concedido
                ShowToast("Location permission not granted");
            }
        }

        public void Stop()
        {
            _timer?.Stop();
        }

        private async Task GetLocation()
        {
            try
            {
                var location = await Geolocation.Default.GetLastKnownLocationAsync();
                if (location != null)
                {
                    ShowToast($"{location.Latitude}, {location.Longitude}");
                    System.Console.WriteLine("ubicacion");
                }
            }
            catch (Exception ex)
            {
                // Manejar excepciones
            }
        }

        private void ShowToast(string message)
        {
            var toast = Toast.Make(message);
            toast.Show();
        }
    }
}
