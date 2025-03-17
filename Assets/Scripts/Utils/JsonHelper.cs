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
        string newJson = "{\"Items\":" + json + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
        return wrapper.Items;
    }
}