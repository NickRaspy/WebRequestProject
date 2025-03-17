using Cifkor_TA.Interfaces;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

namespace Cifkor_TA.Web
{
    public enum RequestType
    {
        Weather,
        DogBreeds,
        DogBreedDetails
    }

    public class RequestTask : IAsyncRequest
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

        UniTask IAsyncRequest.Execute(CancellationToken token) => Execute(token);

        public void Cancel()
        {
            if (!Cts.IsCancellationRequested) Cts.Cancel();
        }
    }
}