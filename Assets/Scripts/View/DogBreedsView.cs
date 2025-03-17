using System;
using System.Collections.Generic;
using UnityEngine;
using Cifkor_TA.Services;
using Cifkor_TA.UI;
using Zenject;

namespace Cifkor_TA.Views
{
    public class DogBreedsView : MonoBehaviour
    {
        [SerializeField] private DogDetailsPopup dogDetailsPopup;
        [SerializeField] private Transform breedsContainer;

        [Inject] private DogBreedButton.Factory dogBreedButtonFactory;

        public event Action<string> OnBreedSelected;

        public void UpdateBreedsUI(List<DogBreed> breeds)
        {
            if (breedsContainer == null || dogBreedButtonFactory == null)
            {
                Debug.LogError("breedsContainer and/or dogBreedButtonFactory aren't selected on Inspector");
                return;
            }

            foreach (Transform child in breedsContainer) Destroy(child.gameObject);

            foreach (var breed in breeds)
            {
                DogBreedButton button = dogBreedButtonFactory.Create();
                button.transform.SetParent(breedsContainer, false);
                button.Initialize(breed);
                button.SetLoading(false);

                button.onClick = null;

                button.onClick += () => HandleBreedButtonClicked(breed);
            }
        }

        private void HandleBreedButtonClicked(DogBreed dogBreed)
        {
            OnBreedSelected?.Invoke(dogBreed.id);
        }

        public void ShowBreedDetails(string breedName, string breedDescription)
        {
            if (dogDetailsPopup != null)
                dogDetailsPopup.Show(breedName, breedDescription);

            else
                Debug.LogError("dogDetailsPopup isn't selected on Inspector");
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}