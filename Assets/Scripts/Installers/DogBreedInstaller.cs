using Cifkor_TA.Services;
using Cifkor_TA.UI;
using UnityEngine;
using Zenject;

namespace Cifkor_TA.Installers
{
    public class DogBreedInstaller : MonoInstaller
    {
        public DogBreedButton dogBreedButtonPrefab;
        public Transform dogBreedButtonsParent;

        public override void InstallBindings()
        {
            Container.Bind<DogBreedsService>()
                .AsSingle();

            Container.BindFactory<DogBreedButton, DogBreedButton.Factory>()
             .FromComponentInNewPrefab(dogBreedButtonPrefab)
             .UnderTransformGroup(dogBreedButtonsParent != null ? dogBreedButtonsParent.name : "DogBreedButtons");
        }
    }
}