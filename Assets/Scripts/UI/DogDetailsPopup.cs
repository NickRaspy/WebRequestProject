using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Cifkor_TA.UI
{
    public class DogDetailsPopup : MonoBehaviour
    {
        public TMP_Text titleText;
        public TMP_Text descriptionText;
        public Button closeButton;
        public CanvasGroup canvasGroup;
        public float animationDuration = 0.3f;

        private void Awake()
        {
            if (closeButton != null)
                closeButton.onClick.AddListener(Hide);

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }

        public void Show(string breedName, string breedDescription)
        {
            transform.SetAsLastSibling();
            gameObject.SetActive(true);
            Canvas.ForceUpdateCanvases();

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