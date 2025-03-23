using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using WRP.Services;
using WRP.View;
using Zenject;

namespace WRP.Controllers
{
    public class WeatherController : BaseController
    {
        [Inject] private WeatherService weatherService;

        [SerializeField] private WeatherView weatherView;

        [SerializeField] private float updateTimeout = 5f;

        private bool isActive = false;

        private CancellationTokenSource periodicCts;

        public override UnityAction OnDataLoad { get; set; }

        public override async void Activate()
        {
            weatherView.Show();
            isActive = true;
            periodicCts = new CancellationTokenSource();
            await WeatherUpdateLoop(periodicCts.Token);
        }

        public override void Deactivate()
        {
            isActive = false;
            periodicCts?.Cancel();
            weatherView.Hide();
        }

        private async UniTask WeatherUpdateLoop(CancellationToken token)
        {
            while (isActive && !token.IsCancellationRequested)
            {
                WeatherPeriod period = await weatherService.GetCurrentWeatherAsync(token);
                if (period != null)
                {
                    if (weatherView != null)
                    {
                        await weatherView.UpdateWeatherUI(period, weatherService, token);
                        OnDataLoad.Invoke();
                    }
                    else
                        Debug.LogError("weatherView isn't selected on Inspector.");
                }
                await UniTask.Delay(TimeSpan.FromSeconds(updateTimeout), cancellationToken: token);
            }
        }
    }
}