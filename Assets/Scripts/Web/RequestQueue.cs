
using Cifkor_TA.Interfaces;
using Cifkor_TA.UI;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Zenject;

namespace Cifkor_TA.Web
{
    public class RequestQueue : MonoBehaviour
    {
        private readonly Queue<IAsyncRequest> queue = new();
        private bool isProcessing = false;
        private IAsyncRequest currentTask;

        [Inject] private ErrorMessage errorMessage;

        [SerializeField]
        private float defaultTimeout = 5f;

        private CancellationTokenSource _currentManualCts;

        public void Enqueue(IAsyncRequest requestTask)
        {
            queue.Enqueue(requestTask);
            if (!isProcessing) ProcessQueue().Forget();
        }

        public void CancelRequestsOfType(RequestType type)
        {
            if (currentTask != null && currentTask.Type == type)
            {
                currentTask.Cancel();
                _currentManualCts?.Cancel();
            }

            var newQueue = new Queue<IAsyncRequest>();

            while (queue.Count > 0)
            {
                var task = queue.Dequeue();
                if (task.Type == type)
                    task.Cancel();
                else
                    newQueue.Enqueue(task);
            }

            while (newQueue.Count > 0)
                queue.Enqueue(newQueue.Dequeue());
        }

        private async UniTaskVoid ProcessQueue()
        {
            isProcessing = true;
            while (queue.Count > 0)
            {
                currentTask = queue.Dequeue();

                using (var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(defaultTimeout)))
                using (_currentManualCts = new CancellationTokenSource())
                using (var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, _currentManualCts.Token))
                {
                    try
                    {
                        await currentTask.Execute(linkedCts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        if (_currentManualCts.IsCancellationRequested)
                        {
                            Debug.Log("Request manually cancelled: " + currentTask.Type);
                        }
                        else
                        {
                            Debug.Log("Request timed out: " + currentTask.Type);
                            errorMessage.Show("");
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log($"Request error in ({currentTask.Type}): {ex.Message}");
                        errorMessage.Show("");
                    }
                }
                currentTask = null;
                _currentManualCts = null;
            }
            isProcessing = false;
        }
    }
}
