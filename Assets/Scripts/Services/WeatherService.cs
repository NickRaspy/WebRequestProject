using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Cifkor_TA.Services
{
    [Serializable]
    public class WeatherResponse
    {
        public WeatherProperties properties;
    }

    [Serializable]
    public class WeatherProperties
    {
        public WeatherPeriod[] periods;
    }

    [Serializable]
    public class WeatherPeriod
    {
        public string name;
        public int temperature;
        public string temperatureUnit;
        public string icon;
    }

    public class WeatherService
    {
        private const string WeatherUrl = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";

        // Кэш для сохранения иконок, чтобы избежать повторного запроса
        private readonly Dictionary<string, Sprite> _iconCache = new();

        // Получает текущий период погоды (например, первый из масси��а)
        public async UniTask<WeatherPeriod> GetCurrentWeatherAsync(CancellationToken token)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(WeatherUrl))
            {
                var asyncOp = uwr.SendWebRequest();
                while (!asyncOp.isDone)
                {
                    if (token.IsCancellationRequested)
                    {
                        uwr.Abort();
                        token.ThrowIfCancellationRequested();
                    }
                    await UniTask.Yield();
                }
#if UNITY_2020_1_OR_NEWER
                if (uwr.result != UnityWebRequest.Result.Success)
#else
                if (uwr.isNetworkError || uwr.isHttpError)
#endif
                {
                    Debug.LogError("Ошибка получения погоды: " + uwr.error);
                    return null;
                }
                else
                {
                    try
                    {
                        WeatherResponse response = JsonUtility.FromJson<WeatherResponse>(uwr.downloadHandler.text);
                        if (response != null && response.properties != null && response.properties.periods != null && response.properties.periods.Length > 0)
                        {
                            // Возвращаем первый период (например, текущая погода)
                            return response.properties.periods[0];
                        }
                        else
                        {
                            Debug.LogWarning("Не удалось разобрать данные погоды");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Ошибка при разборе JSON погоды: " + ex.Message);
                    }
                }
            }
            return null;
        }

        // Загружает иконку погоды по URL и возвращает спрайт с использованием кэша
        public async UniTask<Sprite> GetWeatherIconAsync(string iconUrl, CancellationToken token)
        {
            // Если иконка уже загружена, возвращаем её из кэша
            if (_iconCache.TryGetValue(iconUrl, out Sprite cachedSprite))
            {
                return cachedSprite;
            }

            using (UnityWebRequest iconRequest = UnityWebRequestTexture.GetTexture(iconUrl))
            {
                var asyncOp = iconRequest.SendWebRequest();
                while (!asyncOp.isDone)
                {
                    if (token.IsCancellationRequested)
                    {
                        iconRequest.Abort();
                        token.ThrowIfCancellationRequested();
                    }
                    await UniTask.Yield();
                }
#if UNITY_2020_1_OR_NEWER
                if (iconRequest.result != UnityWebRequest.Result.Success)
#else
                if (iconRequest.isNetworkError || iconRequest.isHttpError)
#endif
                {
                    Debug.LogError("Ошибка загрузки иконки: " + iconRequest.error);
                    return null;
                }
                else
                {
                    Texture2D texture = DownloadHandlerTexture.GetContent(iconRequest);
                    if (texture != null)
                    {
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                        // Сохраняем загруженную иконку в кэше для последующего использования
                        _iconCache[iconUrl] = sprite;
                        return sprite;
                    }
                    else
                    {
                        Debug.LogError("Не удалось получить текстуру из загруженной иконки.");
                    }
                }
            }
            return null;
        }
    }
}