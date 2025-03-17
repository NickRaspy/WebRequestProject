using UnityEngine.Events;

namespace Cifkor_TA.Interfaces
{
    public interface IController
    {
        UnityAction OnDataLoad { get; set; }
        void Activate();

        void Deactivate();
    }
}