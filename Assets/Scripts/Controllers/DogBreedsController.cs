using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Cifkor_TA.Services;
using Cifkor_TA.Views;
using UnityEngine.Events;

namespace Cifkor_TA.Controllers
{
    public class DogBreedsController : BaseController
    {
        [Inject] private DogBreedsService dogBreedsService;

        [SerializeField] private DogBreedsView dogBreedsView;

        private CancellationTokenSource cts;

        public override UnityAction OnDataLoad { get; set; }

        public override async void Activate()
        {
            if (dogBreedsView != null)
            {
                dogBreedsView.OnBreedSelected += OnBreedSelected;
                dogBreedsView.Show();
            }
            else
                Debug.LogError("dogBreedsView isn't selected on Inspector");

            cts?.Cancel();
            cts = new CancellationTokenSource();

            List<DogBreed> breeds = await dogBreedsService.GetDogBreeds(cts.Token);
            dogBreedsView?.UpdateBreedsUI(breeds);

            OnDataLoad.Invoke();
        }

        public override void Deactivate()
        {
            if (dogBreedsView != null)
            {
                dogBreedsView.OnBreedSelected -= OnBreedSelected;
                dogBreedsView.Hide();
            }

            cts?.Cancel();
        }

        private async void OnBreedSelected(string breedId)
        {
            cts?.Cancel();
            cts = new CancellationTokenSource();

            var (breedName, breedDescription) = await dogBreedsService.GetDogBreedDetails(breedId, cts.Token);

            dogBreedsView?.ShowBreedDetails(breedName, breedDescription);
        }
    }
}