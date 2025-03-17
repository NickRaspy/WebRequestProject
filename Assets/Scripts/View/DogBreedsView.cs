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
        // Ссылка для показа popup с деталями породы
        [SerializeField] private DogDetailsPopup dogDetailsPopup;
        // Контейнер для кнопок пород
        [SerializeField] private Transform breedsContainer;

        // Фабрика для создания кнопок пород, внедряется через Zenject
        [Inject] private DogBreedButton.Factory dogBreedButtonFactory;

        // Событие, которое вызывается при нажатии на кнопку породы (передаёт идентификатор породы)
        // (Осталось, чтобы контроллер мог дополнительно реагировать, но показ Popup теперь происходит в View)
        public event Action<string> OnBreedSelected;

        /// <summary>
        /// Обновляет UI: очищает контейнер и создаёт кнопки для каждой породы.
        /// </summary>
        public void UpdateBreedsUI(List<DogBreed> breeds)
        {
            if (breedsContainer == null || dogBreedButtonFactory == null)
            {
                Debug.LogError("breedsContainer или dogBreedButtonFactory не назначены в DogBreedsView.");
                return;
            }

            // Очистка старых кнопок
            foreach (Transform child in breedsContainer)
            {
                Destroy(child.gameObject);
            }

            // Создание новой кнопки для каждой породы через фабрику
            foreach (var breed in breeds)
            {
                DogBreedButton button = dogBreedButtonFactory.Create();
                // Добавляем кнопку в контейнер (с опцией false для сохранения локального масштаба)
                button.transform.SetParent(breedsContainer, false);
                button.Initialize(breed);
                button.SetLoading(false);

                // Удаляем предыдущие подписки и подписываемся на событие клика кнопки.
                button.onClick -= HandleBreedButtonClicked;
                button.onClick += HandleBreedButtonClicked;
            }
        }

        /// <summary>
        /// Метод, вызываемый при нажатии на кнопку породы.
        /// Здесь popup показывается напрямую через dogDetailsPopup.Show с предварительным текстом «Загрузка...».
        /// При этом событие OnBreedSelected вызывается дополнительно (если на него подписаны).
        /// </summary>
        private void HandleBreedButtonClicked(DogBreedButton dogBreedButton)
        {
            dogDetailsPopup.Show("", "");
            dogDetailsPopup.Hide();

            OnBreedSelected?.Invoke(dogBreedButton.DogBreed.id);
        }

        /// <summary>
        /// Просит представление показать окно с деталями породы.
        /// </summary>
        public void ShowBreedDetails(string breedName, string breedDescription)
        {
            if (dogDetailsPopup != null)
            {
                dogDetailsPopup.Show(breedName, breedDescription);
            }
            else
            {
                Debug.LogError("dogDetailsPopup не назначен в DogBreedsView!");
            }
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