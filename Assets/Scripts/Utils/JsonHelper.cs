using System;
using UnityEngine;

public static class JsonHelper
{
    // Оборачивает json-массив в объект, чтобы JsonUtility мог его распарсить.
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{\"Items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.Items;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}