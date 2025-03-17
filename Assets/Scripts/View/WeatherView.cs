using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cifkor_TA.Services;
using Cifkor_TA.View;

namespace Cifkor_TA.Views
{
    // Интерфейс IView уже определён в базовом контроллере (или может быть добавлен здесь),
    // чтобы у представления были методы Show и Hide
    public class WeatherView : BaseView
    {
        [SerializeField] private TMP_Text weatherText;
        [SerializeField] private Image weatherIcon;

        /// <summary>
        /// Обновляет UI погоды: текст и иконку.
        /// Создание спрайта из иконки происходит здесь, так как это логика, относящаяся к UI.
        /// </summary>
        public async UniTask UpdateWeatherUI(WeatherPeriod period, WeatherService weatherService, CancellationToken token)
        {
            if (weatherText != null)
            {
                weatherText.text = $"{period.name} - {period.temperature}{period.temperatureUnit}";
            }
            else
            {
                Debug.LogWarning("weatherText не назна��ен в Inspector.");
            }

            // Если URL иконки задан, обновляем изображение.
            if (weatherIcon != null && !string.IsNullOrEmpty(period.icon))
            {
                // Вместо того чтобы непосредственно создавать спрайт в контроллере, делегируем это представлению.
                Sprite sprite = await weatherService.GetWeatherIconAsync(period.icon, token);
                if (sprite != null)
                {
                    weatherIcon.sprite = sprite;
                }
                else
                {
                    Debug.LogWarning("Не удалось получить спрайт для иконки.");
                }
            }
            else
            {
                Debug.LogWarning("weatherIcon не назначен или period.icon пустой.");
            }
        }

        public override void Show() => base.Show();

        public override void Hide() => base.Hide();
    }
}