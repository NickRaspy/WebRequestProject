using UnityEngine;
using WRP.Interfaces;

namespace WRP.View
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