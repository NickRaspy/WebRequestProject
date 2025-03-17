using Cifkor_TA.Services;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Cifkor_TA.UI
{
    [RequireComponent(typeof(Button))]
    public class DogBreedButton : MonoBehaviour
    {
        private Button button;
        [SerializeField] private TMP_Text label;
        [SerializeField] private GameObject loadingSpinner;

        public DogBreed DogBreed { get; private set; }


        public event Action<DogBreedButton> onClick = delegate { };

        private void Awake()
        {
            button = GetComponent<Button>();

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnEnable()
        {
            if (button == null) button = GetComponent<Button>();

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClicked);
            button.interactable = true;
        }

        public void Initialize(DogBreed dogBreed)
        {
            DogBreed = dogBreed;
            SetLabel(DogBreed.name);
        }

        private void OnButtonClicked()
        {
            onClick?.Invoke(this);
        }

        public void SetLabel(string text)
        {
            if (label != null)
                label.text = text;
            else
                Debug.LogError("label isn't selected on Inspector");
        }

        public void SetLoading(bool show)
        {
            if (loadingSpinner != null)
            {
                loadingSpinner.SetActive(show);
            }
        }

        public class Factory : PlaceholderFactory<DogBreedButton> { }
    }
}