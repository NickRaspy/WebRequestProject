using Cifkor_TA.Web;
using Cysharp.Threading.Tasks;
using System.Threading;

namespace Cifkor_TA.Interfaces
{
    public interface IAsyncRequest
    {
        RequestType Type { get; }

        UniTask Execute(CancellationToken token);

        void Cancel();
    }
}