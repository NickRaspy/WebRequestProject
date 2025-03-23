using WRP.Services;
using Zenject;

namespace WRP.Installers
{
    public class WeatherInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<WeatherService>()
                .AsSingle();
        }
    }
}