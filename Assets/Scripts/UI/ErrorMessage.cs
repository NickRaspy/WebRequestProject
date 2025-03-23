using DG.Tweening;
using TMPro;
using UnityEngine;

namespace WRP.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class ErrorMessage : MonoBehaviour
    {
        private TMP_Text text;

        private const string errorText = "An error occurred.";

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
        }

        public void Show(string errorCode)
        {
            gameObject.SetActive(true);

            text.text = !string.IsNullOrEmpty(errorCode) ? errorText + " Code: " + errorCode : errorText;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(text.DOFade(1f, 0.5f));
            sequence.AppendInterval(5f);
            sequence.Append(text.DOFade(0f, 0.5f));
            sequence.OnComplete(() => gameObject.SetActive(false));
        }
    }
}