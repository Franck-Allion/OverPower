using System.Threading;
using System.Threading.Tasks;
using OverPower.Domain.Run;

namespace OverPower.Application
{
    public interface IGameFlowController
    {
        Task StartupAsync(CancellationToken cancellationToken = default);
        Task StartNewRunAsync(RunSeed seed, CancellationToken cancellationToken = default);
        Task ReturnToMainMenuAsync(CancellationToken cancellationToken = default);
    }
}
