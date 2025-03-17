using Cifkor_TA.Interfaces;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Cifkor_TA.Web
{
    public class RequestQueue : MonoBehaviour
    {
        private readonly Queue<IAsyncRequest> _queue = new();
        private bool _isProcessing = false;
        private IAsyncRequest _currentTask;

        public void Enqueue(IAsyncRequest requestTask)
        {
            _queue.Enqueue(requestTask);
            if (!_isProcessing)
            {
                ProcessQueue().Forget();
            }
        }

        public void CancelRequestsOfType(RequestType type)
        {
            if (_currentTask != null && _currentTask.Type == type)
            {
                _currentTask.Cancel();
            }

            var newQueue = new Queue<IAsyncRequest>();
            while (_queue.Count > 0)
            {
                var task = _queue.Dequeue();
                if (task.Type == type)
                {
                    task.Cancel();
                }
                else
                {
                    newQueue.Enqueue(task);
                }
            }
            while (newQueue.Count > 0)
            {
                _queue.Enqueue(newQueue.Dequeue());
            }
        }

        private async UniTaskVoid ProcessQueue()
        {
            _isProcessing = true;
            while (_queue.Count > 0)
            {
                _currentTask = _queue.Dequeue();
                try
                {
                    await _currentTask.Execute(CancellationToken.None);
                }
                catch (OperationCanceledException)
                {
                    Debug.Log("Запрос отменён: " + _currentTask.Type);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Ошибка при выполнении запроса ({_currentTask.Type}): {ex.Message}");
                }
                _currentTask = null;
            }
            _isProcessing = false;
        }
    }
}