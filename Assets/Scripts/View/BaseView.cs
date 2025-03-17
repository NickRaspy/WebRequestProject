using Cifkor_TA.Interfaces;
using UnityEngine;

namespace Cifkor_TA.View
{
    public class BaseView : MonoBehaviour, IView
    {
        public virtual void Hide()
        {
            gameObject.SetActive(false);
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }
    }
}
