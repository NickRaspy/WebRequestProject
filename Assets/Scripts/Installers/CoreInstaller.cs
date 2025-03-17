using Cifkor_TA.Controllers;
using Cifkor_TA.Web;
using Zenject;

namespace Cifkor_TA.Installers
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
        }
    }
}