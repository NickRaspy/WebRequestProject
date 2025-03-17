using Cifkor_TA.Interfaces;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Cifkor_TA.Controllers
{
    public class ControllerManager : MonoBehaviour
    {
        [Inject]
        private List<IController> _controllers;

        private IController _currentController;

        [SerializeField] private GameObject startScreen;

        [SerializeField] private GameObject loadingScreen;

        public void SwitchController(IController newController)
        {
            if (newController == null)
            {
                Debug.LogWarning("Given controller is null.");
                return;
            }

            if (_currentController == newController) return;

            if (_currentController != null) _currentController.OnDataLoad -= DisableLoadingScreen;

            newController.OnDataLoad += DisableLoadingScreen;

            startScreen.SetActive(false);

            loadingScreen.SetActive(true);

            _currentController?.Deactivate();

            _currentController = newController;
            _currentController.Activate();
        }

        void DisableLoadingScreen() => loadingScreen.SetActive(false);
    }
}