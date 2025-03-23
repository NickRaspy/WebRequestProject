using WRP.Interfaces;
using WRP.UI;
using Zenject;

namespace WRP.Services
{
    public class BaseService : IService
    {
        [Inject] private ErrorMessage errorMessage;

        public void HandleError(string errorCode)
        {
            errorMessage.Show(errorCode);
        }
    }
}