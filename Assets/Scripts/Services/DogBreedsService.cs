using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

namespace Cifkor_TA.Services
{
    [Serializable]
    public class DogBreed
    {
        public string id;
        public string name;
        public string description;
    }

    [Serializable]
    public class ResponseWrapper
    {
        public DataEntry[] data;
    }

    [Serializable]
    public class DataEntry
    {
        public string id;
        public Attributes attributes;
    }

    [Serializable]
    public class Attributes
    {
        public string name;
        public string description;
    }

    public class DogBreedsService
    {
        // Метод для получения списка пород
        public async UniTask<List<DogBreed>> GetDogBreeds(CancellationToken token)
        {
            List<DogBreed> topBreeds = new List<DogBreed>();
            string url = "https://dogapi.dog/api/v2/breeds";
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
                    Debug.LogError("Ошибка получения пород: " + uwr.error);
                }
                else
                {
                    try
                    {
                        ResponseWrapper wrapper = JsonUtility.FromJson<ResponseWrapper>(uwr.downloadHandler.text);
                        if (wrapper != null && wrapper.data != null)
                        {
                            int count = Math.Min(10, wrapper.data.Length);
                            for (int i = 0; i < count; i++)
                            {
                                DataEntry entry = wrapper.data[i];
                                DogBreed breed = new DogBreed
                                {
                                    id = entry.id,
                                    name = entry.attributes.name,
                                    description = entry.attributes.description
                                };
                                topBreeds.Add(breed);
                            }
                        }
                        else
                        {
                            Debug.LogError("Некорректный формат JSON.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Ошибка при разборе JSON пород: " + ex.ToString());
                    }
                }
            }
            return topBreeds;
        }

        // Метод для получения деталей конкретной породы
        public async UniTask<(string breedName, string breedDescription)> GetDogBreedDetails(string breedId, CancellationToken token)
        {
            string url = $"https://dogapi.dog/api/v2/breeds/{breedId}";
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
                    Debug.LogError("Ошибка получения деталей породы: " + uwr.error);
                    return (string.Empty, string.Empty);
                }
                else
                {
                    try
                    {
                        string json = uwr.downloadHandler.text;
                        if (json.Contains("\"data\":{"))
                        {
                            json = TransformDataObjectToArray(json);
                        }
                        ResponseWrapper wrapper = JsonUtility.FromJson<ResponseWrapper>(json);
                        if (wrapper != null && wrapper.data != null && wrapper.data.Length > 0)
                        {
                            DataEntry entry = wrapper.data[0];
                            string breedName = entry.attributes.name;
                            string breedDescription = !string.IsNullOrEmpty(entry.attributes.description)
                                ? entry.attributes.description
                                : "Описание породы " + breedId;
                            return (breedName, breedDescription);
                        }
                        else
                        {
                            Debug.LogError("Некорректный формат JSON для деталей породы.");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Ошибка при разборе JSON деталей породы: " + ex.ToString());
                    }
                }
            }
            return (string.Empty, string.Empty);
        }

        // Преобразование JSON, если data представлено объектом, а не массивом
        private string TransformDataObjectToArray(string json)
        {
            int dataIndex = json.IndexOf("\"data\":");
            if (dataIndex == -1)
                return json;

            int braceStart = json.IndexOf('{', dataIndex);
            if (braceStart == -1)
                return json;

            int index = braceStart;
            int braceCount = 0;
            for (; index < json.Length; index++)
            {
                if (json[index] == '{') braceCount++;
                else if (json[index] == '}') braceCount--;
                if (braceCount == 0)
                    break;
            }
            string before = json.Substring(0, braceStart);
            string dataObject = json.Substring(braceStart, index - braceStart + 1);
            string after = json.Substring(index + 1);
            return before + "[" + dataObject + "]" + after;
        }
    }
}