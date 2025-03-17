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
        [Inject] private DogBreedsService _dogBreedsService;

        // Ссылка на представление для работы с UI пород
        [SerializeField] private DogBreedsView dogBreedsView;

        // Текущий активный процесс для получения деталей породы
        private CancellationTokenSource _cts;

        public override UnityAction OnDataLoad { get; set; }

        public override async void Activate()
        {
            // Подписываемся на событие выбора породы, если представление назначено
            if (dogBreedsView != null)
            {
                dogBreedsView.OnBreedSelected += OnBreedSelected;
                dogBreedsView.Show();
            }
            else
            {
                Debug.LogError("dogBreedsView не назначено!");
            }

            // Отменяем предыдущие запросы и создаем новый CancellationToken
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            // Запрос списка пород через сервис
            List<DogBreed> breeds = await _dogBreedsService.GetDogBreeds(_cts.Token);
            // Передача списка представлению для обновления UI
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

            _cts?.Cancel();
        }

        /// <summary>
        /// Обработка события выбора породы от представления.
        /// Запрашивает дета��и породы через сервис и просит представление показать данные.
        /// </summary>
        private async void OnBreedSelected(string breedId)
        {
            // Отменяем предыдущие запросы деталей
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            // Получаем детали породы через сервис
            var (breedName, breedDescription) = await _dogBreedsService.GetDogBreedDetails(breedId, _cts.Token);
            // Передаём детали представлению для отображения
            if (dogBreedsView != null)
            {
                dogBreedsView.ShowBreedDetails(breedName, breedDescription);
            }
        }
    }
}