using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Cifkor_TA.Services
{
    #region JSON_CLASSES

    [Serializable]
    public class WeatherResponse
    {
        public WeatherProperties properties;

        [Serializable]
        public class WeatherProperties
        {
            public WeatherPeriod[] periods;
        }
    }

    [Serializable]
    public class WeatherPeriod
    {
        public int temperature;
        public string temperatureUnit;
        public string icon;
    }
    #endregion

    public class WeatherService : BaseService
    {
        private const string WeatherUrl = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";

        private readonly Dictionary<string, Sprite> _iconCache = new();

        public async UniTask<WeatherPeriod> GetCurrentWeatherAsync(CancellationToken token)
        {
            using UnityWebRequest uwr = UnityWebRequest.Get(WeatherUrl);

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

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Error while getting Weather data: " + uwr.error);
                HandleError(uwr.responseCode.ToString());
                return null;
            }
            else
            {
                try
                {
                    WeatherResponse response = JsonUtility.FromJson<WeatherResponse>(uwr.downloadHandler.text);
                    if (response != null && response.properties != null && response.properties.periods != null && response.properties.periods.Length > 0)
                    {
                        return response.properties.periods[0];
                    }
                    else
                    {
                        Debug.LogWarning("Can't get Weather data");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error while parsing Weather JSON: " + ex.Message);
                    HandleError("");
                }
            }

            return null;
        }

        public async UniTask<Sprite> GetWeatherIconAsync(string iconUrl, CancellationToken token)
        {
            if (_iconCache.TryGetValue(iconUrl, out Sprite cachedSprite))
            {
                return cachedSprite;
            }

            using UnityWebRequest iconRequest = UnityWebRequestTexture.GetTexture(iconUrl);

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

            if (iconRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error while loading Weather Icon: " + iconRequest.error);
                HandleError(iconRequest.responseCode.ToString());
                return null;
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(iconRequest);
                if (texture != null)
                {
                    Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    _iconCache[iconUrl] = sprite;
                    return sprite;
                }
                else
                {
                    Debug.LogError("Can't get Weather Icon.");
                }
            }

            return null;
        }
    }
}