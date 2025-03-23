using UnityEngine;
using UnityEngine.Events;
using WRP.Interfaces;

namespace WRP.Controllers
{
    public abstract class BaseController : MonoBehaviour, IController
    {
        public abstract UnityAction OnDataLoad { get; set; }

        public abstract void Activate();

        public abstract void Deactivate();
    }
}