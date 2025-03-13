using UnityEngine;
using Zenject;

public class CoreInstaller : MonoInstaller
{
    // Префаб для DogDetailsPopup необходимо назначить через инспектор
    public DogDetailsPopup dogDetailsPopupPrefab;

    public override void InstallBindings()
    {
        // Создаем RequestQueue на новом объекте и регистрируем его как синглтон в рамках сцены.
        Container.Bind<RequestQueue>()
                 .FromNewComponentOnNewGameObject()
                 .WithGameObjectName("RequestQueue")
                 .AsSingle()
                 .NonLazy();

        // Регистрируем WeatherController, который должен быть прикреплён к объекту в сцене.
        Container.Bind<WeatherController>()
                 .FromComponentInHierarchy()
                 .AsSingle();

        // Регистрируем DogBreedsController, который также находится в иерархии сцены.
        Container.Bind<DogBreedsController>()
                 .FromComponentInHierarchy()
                 .AsSingle();

        // Регистрируем фабрику для создания DogDetailsPopup на основании префаба.
        Container.BindFactory<DogDetailsPopup, DogDetailsPopup.Factory>()
                 .FromComponentInNewPrefab(dogDetailsPopupPrefab)
                 .UnderTransformGroup("DogDetailsPopups");
    }
}