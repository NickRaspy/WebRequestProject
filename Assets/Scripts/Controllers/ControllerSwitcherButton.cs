using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace WRP.Controllers
{
    [RequireComponent(typeof(Button))]
    public class ControllerSwitcherButton : MonoBehaviour
    {
        [SerializeField] private GameObject requiredScreen;

        [SerializeField] private BaseController targetController;

        [Inject] private ControllerManager controllerManager;

        private void Awake() => GetComponent<Button>().onClick.AddListener(OnButtonClicked);

        private void OnButtonClicked()
        {
            requiredScreen?.SetActive(true);
            controllerManager?.SwitchController(targetController);
        }
    }
}