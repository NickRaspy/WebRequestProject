using UnityEngine;
using WRP.Interfaces;

namespace WRP.Controllers
{
    public class ControllerManager : MonoBehaviour
    {
        private IController currentController;

        [SerializeField] private GameObject startScreen;

        [SerializeField] private GameObject loadingScreen;

        public void SwitchController(IController newController)
        {
            if (newController == null)
            {
                Debug.LogWarning("Given controller is null.");
                return;
            }

            if (currentController == newController) return;

            if (currentController != null) currentController.OnDataLoad -= DisableLoadingScreen;

            newController.OnDataLoad += DisableLoadingScreen;

            startScreen.SetActive(false);

            loadingScreen.SetActive(true);

            currentController?.Deactivate();

            currentController = newController;
            currentController.Activate();
        }

        private void DisableLoadingScreen() => loadingScreen.SetActive(false);
    }
}