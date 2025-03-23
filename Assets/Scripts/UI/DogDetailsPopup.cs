using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WRP.UI
{
    public class DogDetailsPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private Button closeButton;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float animationDuration = 0.3f;

        private void Awake()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(Hide);
            else
                Debug.LogError("closeButton isn't selected on Inspector");

            if (!TryGetComponent<CanvasGroup>(out canvasGroup))
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        public void Show(string breedName, string breedDescription)
        {
            gameObject.SetActive(true);

            if (titleText != null)
                titleText.text = breedName;

            if (descriptionText != null)
                descriptionText.text = breedDescription;

            DOTween.Kill(canvasGroup);
            canvasGroup.alpha = 0f;
            canvasGroup.DOFade(1f, animationDuration);
        }

        public void Hide()
        {
            DOTween.Kill(canvasGroup);
            canvasGroup.DOFade(0f, animationDuration).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}