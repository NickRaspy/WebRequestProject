using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using Zenject;
using Cifkor_TA.Services;
using Cifkor_TA.Views;
using UnityEngine.Events;

namespace Cifkor_TA.Controllers
{
    public class WeatherController : BaseController
    {
        [Inject] private WeatherService weatherService;

        [SerializeField] private WeatherView weatherView;

        private bool isActive = false;

        private CancellationTokenSource periodicCts;

        public override UnityAction OnDataLoad { get; set; }

        public override void Activate()
        {
            weatherView.Show();
            isActive = true;
            periodicCts = new CancellationTokenSource();
            WeatherUpdateLoop(periodicCts.Token).Forget();
        }

        public override void Deactivate()
        {
            isActive = false;
            periodicCts?.Cancel();
            weatherView.Hide();
        }

        private async UniTaskVoid WeatherUpdateLoop(CancellationToken token)
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
                        Debug.LogError("weatherView isn't selected Inspector.");
                }
                await UniTask.Delay(TimeSpan.FromSeconds(5), cancellationToken: token);
            }
        }
    }
}