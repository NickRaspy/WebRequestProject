using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum RequestType
{
    Weather,
    DogBreeds,
    DogBreedDetails
}

public class RequestTask
{
    public RequestType Type { get; }
    public Func<CancellationToken, UniTask> Execute { get; }
    public CancellationTokenSource Cts { get; }

    public RequestTask(RequestType type, Func<CancellationToken, UniTask> execute)
    {
        Type = type;
        Execute = execute;
        Cts = new CancellationTokenSource();
    }

    public void Cancel()
    {
        if (!Cts.IsCancellationRequested)
        {
            Cts.Cancel();
        }
    }
}

public class RequestQueue : MonoBehaviour
{
    private readonly Queue<RequestTask> _queue = new Queue<RequestTask>();
    private bool _isProcessing = false;
    private RequestTask _currentTask;

    public void Enqueue(RequestTask requestTask)
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

        var newQueue = new Queue<RequestTask>();
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
                await _currentTask.Execute(_currentTask.Cts.Token);
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