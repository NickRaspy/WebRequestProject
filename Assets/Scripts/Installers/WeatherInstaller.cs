using Cifkor_TA.Services;
using Zenject;

namespace Cifkor_TA.Installers
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