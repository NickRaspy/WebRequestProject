using WRP.Controllers;
using WRP.UI;
using WRP.Web;
using Zenject;

namespace WRP.Installers
{
    public class CoreInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<RequestQueue>()
                     .FromNewComponentOnNewGameObject()
                     .WithGameObjectName("RequestQueue")
                     .AsSingle()
                     .NonLazy();

            Container.Bind<ControllerManager>()
                     .FromComponentInHierarchy()
                     .AsSingle();

            Container.Bind<ErrorMessage>()
                .FromComponentInHierarchy()
                .AsSingle();
        }
    }
}