using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WRP.Services;

namespace WRP.View
{
    public class WeatherView : BaseView
    {
        [SerializeField] private TMP_Text weatherText;
        [SerializeField] private Image weatherIcon;

        public async UniTask UpdateWeatherUI(WeatherPeriod period, WeatherService weatherService, CancellationToken token)
        {
            if (weatherText != null)
                weatherText.text = $"Today is {period.temperature}{period.temperatureUnit}";
            else
                Debug.LogWarning("weatherText isn't selected on Inspector.");

            if (weatherIcon != null && !string.IsNullOrEmpty(period.icon))
            {
                Sprite sprite = await weatherService.GetWeatherIconAsync(period.icon, token);

                if (sprite != null)
                    weatherIcon.sprite = sprite;
                else
                    Debug.LogWarning("Can't get Weather Icon.");
            }
            else
                Debug.LogWarning("weatherIcon isn't selected on Inspector or Weather Data doesn't have an icon.");
        }

        public override void Show() => base.Show();

        public override void Hide() => base.Hide();
    }
}