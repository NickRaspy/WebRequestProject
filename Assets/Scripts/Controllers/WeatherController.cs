using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

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

public class WeatherController : MonoBehaviour
{
    [Inject] private RequestQueue _requestQueue;

    // Флаг активности вкладки
    private bool _isActive = false;
    // Источник отмены для периодического опроса
    private CancellationTokenSource _periodicCts;

    // Метод, вызываемый при активации вкладки "Погода"
    public void Activate()
    {
        _isActive = true;
        _periodicCts = new CancellationTokenSource();
        TaskLoop(_periodicCts.Token).Forget();
    }

    // Метод, вызываемый при деактивации вкладки "Погода"
    public void Deactivate()
    {
        _isActive = false;
        _periodicCts?.Cancel();
        _requestQueue.CancelRequestsOfType(RequestType.Weather);
    }

    // Цикл постановки запросов в очередь с интервалом 5 секунд
    private async UniTaskVoid TaskLoop(CancellationToken token)
    {
        while (_isActive && !token.IsCancellationRequested)
        {
            _requestQueue.Enqueue(new RequestTask(RequestType.Weather, FetchWeather));
            await UniTask.Delay(TimeSpan.FromSeconds(5), cancellationToken: token);
        }
    }

    // Выполнение запроса погоды с разбором JSON-ответа
    private async UniTask FetchWeather(CancellationToken token)
    {
        string url = "https://api.weather.gov/gridpoints/TOP/32,81/forecast";
        using (UnityWebRequest uwr = UnityWebRequest.Get(url))
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
            }
            else
            {
                try
                {
                    // Разбор JSON-ответа с использованием JsonUtility
                    WeatherResponse response = JsonUtility.FromJson<WeatherResponse>(uwr.downloadHandler.text);
                    if (response != null && response.properties != null && response.properties.periods != null && response.properties.periods.Length > 0)
                    {
                        // Выбираем первый период (например, текущая погода)
                        WeatherPeriod period = response.properties.periods[0];
                        Debug.Log($"Иконка: {period.icon} | {period.name} - {period.temperature}{period.temperatureUnit}");
                        // Здесь можно обновлять UI, например, устанавливая текст "Сегодня - 61F" и соответствующую иконку
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
    }
}