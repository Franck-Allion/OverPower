using System.Threading;
using System.Threading.Tasks;

namespace OverPower.Application
{
    public interface ISceneNavigator
    {
        GameSceneId CurrentScene { get; }

        Task LoadSceneAsync(
            GameSceneId sceneId,
            CancellationToken cancellationToken = default);
    }
}
