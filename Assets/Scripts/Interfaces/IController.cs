using UnityEngine.Events;

namespace WRP.Interfaces
{
    public interface IController
    {
        UnityAction OnDataLoad { get; set; }

        void Activate();

        void Deactivate();
    }
}