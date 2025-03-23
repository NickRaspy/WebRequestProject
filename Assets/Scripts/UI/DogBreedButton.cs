using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WRP.Services;
using Zenject;

namespace WRP.UI
{
    [RequireComponent(typeof(Button))]
    public class DogBreedButton : MonoBehaviour
    {
        private Button button;
        [SerializeField] private TMP_Text label;
        [SerializeField] private GameObject loadingSpinner;

        public DogBreed DogBreed { get; private set; }

        public UnityAction onClick;

        private void Awake()
        {
            button = GetComponent<Button>();

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClicked);
        }

        private void OnEnable()
        {
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
            onClick?.Invoke();
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

        public class Factory : PlaceholderFactory<DogBreedButton>
        { }
    }
}