using System;
using UnityEngine;

public static class JsonHelper
{
    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }

    [Serializable]
    private class WrapperData<T>
    {
        public T[] data;
    }

    public static T[] FromJson<T>(string json)
    {
        // Если JSON начинается с объекта, попробуем десериализовать как объект с полем data
        if (json.TrimStart().StartsWith("{"))
        {
            try
            {
                WrapperData<T> wrapperData = JsonUtility.FromJson<WrapperData<T>>(json);
                if (wrapperData != null && wrapperData.data != null)
                {
                    return wrapperData.data;
                }
            }
            catch (Exception) { }
        }
        // Для случаев, когда JSON – чистый массив, оборачиваем его в объект с полем Items
        string newJson = "{\"Items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.Items;
    }
}