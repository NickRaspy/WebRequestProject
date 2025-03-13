using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    public RequestQueue requestQueuePrefab;
    public DogDetailsPopup dogDetailsPopupPrefab;

    public override void InstallBindings()
    {
        // Регистрируем RequestQueue как синглтон в рамках сцены (но не паттерн Singleton, а привязку Zenject)
        Container.Bind<RequestQueue>().FromComponentInNewPrefab(requestQueuePrefab).AsSingle().NonLazy();
        
        // Регистрируем контроллеры (можно назначать их на объекты UI)
        Container.Bind<WeatherController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<DogBreedsController>().FromComponentInHierarchy().AsSingle();

        // Фабрика для попапа деталей породы
        Container.BindFactory<DogDetailsPopup, DogDetailsPopup.Factory>()
            .FromComponentInNewPrefab(dogDetailsPopupPrefab)
            .UnderTransformGroup("DogDetailsPopups");
    }
}