using Cysharp.Threading.Tasks;
using System.Threading;
using WRP.Web;

namespace WRP.Interfaces
{
    public interface IAsyncRequest
    {
        RequestType Type { get; }

        UniTask Execute(CancellationToken token);

        void Cancel();
    }
}