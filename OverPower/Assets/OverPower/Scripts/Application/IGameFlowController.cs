using System.Threading;
using System.Threading.Tasks;

namespace OverPower.Application
{
    public interface IGameFlowController
    {
        Task StartupAsync(CancellationToken cancellationToken = default);
        Task StartNewRunAsync(CancellationToken cancellationToken = default);
        Task ReturnToMainMenuAsync(CancellationToken cancellationToken = default);
    }
}
