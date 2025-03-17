using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cifkor_TA.Interfaces;
using Zenject;
using Cifkor_TA.UI;

namespace Cifkor_TA.Services
{
    public class BaseService : IService
    {
        [Inject] private ErrorMessage errorMessage;

        public void HandleError(string errorCode)
        {
            errorMessage.Show(errorCode);
        }
    }
}
