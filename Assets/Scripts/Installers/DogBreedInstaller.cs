using UnityEngine;
using WRP.Services;
using WRP.UI;
using Zenject;

namespace WRP.Installers
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