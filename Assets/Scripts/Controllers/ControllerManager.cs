using Cifkor_TA.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Cifkor_TA.Controllers
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

        void DisableLoadingScreen() => loadingScreen.SetActive(false);
    }
}