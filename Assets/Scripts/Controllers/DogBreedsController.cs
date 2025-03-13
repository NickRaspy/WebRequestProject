using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;
using UnityEngine.UI;
using System.Collections.Generic;

[Serializable]
public class DogBreed
{
    public string id;
    public string name;
    // Дополнительно можно добавить другие свойства
}

public class DogBreedsController : MonoBehaviour
{
    [Inject] private RequestQueue _requestQueue;
    [Inject] private DogDetailsPopup.Factory _popupFactory;

    private bool _isActive = false;

    public GameObject loadingSpinner;
    public Text breedsText;

    private RequestTask _currentBreedDetailsRequest;

    public void Activate()
    {
        _isActive = true;
        ShowLoading(true);
        _requestQueue.Enqueue(new RequestTask(RequestType.DogBreeds, FetchDogBreeds));
    }

    public void Deactivate()
    {
        _isActive = false;
        _requestQueue.CancelRequestsOfType(RequestType.DogBreeds);
        _requestQueue.CancelRequestsOfType(RequestType.DogBreedDetails);
        ShowLoading(false);
    }

    private void ShowLoading(bool show)
    {
        if (loadingSpinner != null)
            loadingSpinner.SetActive(show);
    }

    private async UniTask FetchDogBreeds(CancellationToken token)
    {
        string url = "https://api.thedogapi.com/v2/breeds";
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
                    DogBreed[] dogBreeds = JsonHelper.FromJson<DogBreed>(uwr.downloadHandler.text);
                    List<string> breeds = new List<string>();
                    int count = Math.Min(10, dogBreeds.Length);
                    for (int i = 0; i < count; i++)
                    {
                        breeds.Add($"{i + 1} - {dogBreeds[i].name}");
                    }
                    UpdateBreedsUI(breeds);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Ошибка при разборе JSON пород: " + ex.Message);
                }
            }
        }
        ShowLoading(false);
    }

    private void UpdateBreedsUI(List<string> breeds)
    {
        if (breedsText == null) return;
        breedsText.text = string.Join("\n", breeds);
        // В реальной реализации каждому элементу списка надо добавить UI-кнопку для клика, которая вызовет OnBreedSelected.
    }

    public void OnBreedSelected(string breedId)
    {
        if (_currentBreedDetailsRequest != null)
        {
            _currentBreedDetailsRequest.Cancel();
            _currentBreedDetailsRequest = null;
        }
        ShowLoading(true);
        _currentBreedDetailsRequest = new RequestTask(RequestType.DogBreedDetails, (token) => FetchDogBreedDetails(breedId, token));
        _requestQueue.Enqueue(_currentBreedDetailsRequest);
    }

    private async UniTask FetchDogBreedDetails(string breedId, CancellationToken token)
    {
        string url = $"https://api.thedogapi.com/v2/breeds/{breedId}";
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
            }
            else
            {
                try
                {
                    DogBreed breedDetail = JsonUtility.FromJson<DogBreed>(uwr.downloadHandler.text);
                    string breedName = breedDetail.name;
                    string breedDescription = "Описание породы " + breedId; // Замените на реальное поле, если оно есть в JSON
                    var popup = _popupFactory.Create();
                    popup.Show(breedName, breedDescription);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Ошибка при разборе JSON деталей породы: " + ex.Message);
                }
            }
        }
        ShowLoading(false);
    }
}