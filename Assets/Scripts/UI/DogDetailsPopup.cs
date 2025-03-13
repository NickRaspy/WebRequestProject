using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;

public class DogDetailsPopup : MonoBehaviour
{
    public Text titleText;
    public Text descriptionText;
    public Button closeButton;
    public RectTransform contentRect;

    // Параметры анимации
    public float animationDuration = 0.3f;
    public float padding = 20f; // отступ для вычисления итоговой высоты

    private float initialHeight;

    private void Awake()
    {
        if (closeButton != null)
            closeButton.onClick.AddListener(Hide);

        // Сохраняем исходную высоту, чтобы можно было её восстановить при закрытии попапа
        initialHeight = contentRect.sizeDelta.y;
        // Начальное состояние окна – скрытое
        gameObject.SetActive(false);
    }

    public void Show(string breedName, string breedDescription)
    {
        gameObject.SetActive(true);

        if (titleText != null)
            titleText.text = breedName;
        if (descriptionText != null)
            descriptionText.text = breedDescription;

        // Вычисляем предпочтительную высоту для каждого текстового элемента
        float titleHeight = titleText != null ? LayoutUtility.GetPreferredHeight(titleText.rectTransform) : 0f;
        float descriptionHeight = descriptionText != null ? LayoutUtility.GetPreferredHeight(descriptionText.rectTransform) : 0f;

        float targetHeight = titleHeight + descriptionHeight + padding;

        // Анимируем изменение высоты contentRect к рассчитанной высоте
        contentRect.DOSizeDelta(new Vector2(contentRect.sizeDelta.x, targetHeight), animationDuration);
    }

    public void Hide()
    {
        // Анимируем возвращение к исходной высоте, после чего отключаем окно
        contentRect.DOSizeDelta(new Vector2(contentRect.sizeDelta.x, initialHeight), animationDuration)
            .OnComplete(() => gameObject.SetActive(false));
    }

    public class Factory : PlaceholderFactory<DogDetailsPopup> { }
}