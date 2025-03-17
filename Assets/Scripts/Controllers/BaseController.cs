using Cifkor_TA.Interfaces;
using Cifkor_TA.View;
using UnityEngine;
using UnityEngine.Events;

namespace Cifkor_TA.Controllers
{
    public abstract class BaseController : MonoBehaviour, IController
    {
        public abstract UnityAction OnDataLoad { get; set; }

        public abstract void Activate();

        public abstract void Deactivate();
    }
}