
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

namespace Cifkor_TA.Services
{
    #region JSON_CLASSES
    [Serializable]
    public class DogBreed
    {
        public string id;
        public string name;
        public string description;
    }

    [Serializable]
    public class DogBreedResponse
    {
        public string id;
        public string type;
        public DogBreedAttributes attributes;

        [Serializable]
        public class DogBreedAttributes
        {
            public string name;
            public string description;
        }
    }
    #endregion

    public class DogBreedsService : BaseService
    {
        public async UniTask<List<DogBreed>> GetDogBreeds(CancellationToken token)
        {
            List<DogBreed> topBreeds = new();

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

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.Log("Error while getting DogBreed data: " + uwr.error);
                    HandleError(uwr.responseCode.ToString());
                }
                else
                {
                    try
                    {
                        string json = uwr.downloadHandler.text;
                        JObject jsonObj = JObject.Parse(json);
                        JToken dataToken = jsonObj["data"];
                        List<DogBreedResponse> breedResponses = null;

                        if (dataToken != null)
                        {
                            if (dataToken.Type == JTokenType.Array)
                            {
                                breedResponses = dataToken.ToObject<List<DogBreedResponse>>();
                            }
                            else if (dataToken.Type == JTokenType.Object)
                            {
                                DogBreedResponse singleBreed = dataToken.ToObject<DogBreedResponse>();
                                breedResponses = new List<DogBreedResponse>() { singleBreed };
                            }
                        }
                        else
                        {
                            Debug.LogError("JSON does not contain 'data' property.");
                            HandleError("");
                        }


                        if (breedResponses != null)
                        {
                            int count = Math.Min(10, breedResponses.Count);
                            for (int i = 0; i < count; i++)
                            {
                                DogBreedResponse response = breedResponses[i];
                                if (response.attributes != null)
                                {
                                    DogBreed breed = new DogBreed
                                    {
                                        id = response.id,
                                        name = response.attributes.name,
                                        description = response.attributes.description
                                    };
                                    topBreeds.Add(breed);
                                }
                                else
                                    Debug.LogWarning($"No attributes found for breed with id {response.id}");
                            }
                        }
                        else
                        {
                            Debug.LogError("Incorrect JSON format: 'data' property not found or invalid.");
                            HandleError("");
                        }

                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Exception during deserialization: " + ex);
                        HandleError("");
                    }
                }
            }
            return topBreeds;
        }

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

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Error while getting DogBreed details: " + uwr.error);
                    HandleError(uwr.responseCode.ToString());
                    return (string.Empty, string.Empty);
                }
                else
                {
                    try
                    {
                        string json = uwr.downloadHandler.text;
                        JObject jsonObj = JObject.Parse(json);
                        JToken dataToken = jsonObj["data"];
                        List<DogBreedResponse> breedResponses = null;

                        if (dataToken != null)
                        {
                            if (dataToken.Type == JTokenType.Array)
                            {
                                breedResponses = dataToken.ToObject<List<DogBreedResponse>>();
                            }
                            else if (dataToken.Type == JTokenType.Object)
                            {
                                DogBreedResponse singleBreed = dataToken.ToObject<DogBreedResponse>();
                                breedResponses = new List<DogBreedResponse>() { singleBreed };
                            }
                        }
                        else
                        {
                            Debug.LogError("JSON does not contain 'data' property for DogBreed details.");
                            HandleError("");
                        }

                        if (breedResponses != null && breedResponses.Count > 0)
                        {
                            DogBreedResponse response = breedResponses[0];
                            if (response.attributes != null)
                            {
                                string breedName = response.attributes.name;
                                string breedDescription = !string.IsNullOrEmpty(response.attributes.description) ? response.attributes.description : string.Empty;
                                return (breedName, breedDescription);
                            }
                        }
                        else
                        {
                            Debug.LogError("Incorrect JSON format for DogBreed details.");
                            HandleError("");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("Exception during deserialization of DogBreed details: " + ex);
                        HandleError("");
                    }
                }
            }
            return (string.Empty, string.Empty);
        }
    }
}
