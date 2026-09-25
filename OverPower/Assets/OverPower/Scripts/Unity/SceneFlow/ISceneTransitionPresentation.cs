using System.Threading;
using System.Threading.Tasks;

namespace OverPower.Unity.SceneFlow
{
    /// <summary>
    /// Presentation boundary for screen transition presentation (fades, overlays, loading indicators).
    /// Kept within the Unity integration/presentation boundary without leaking into pure Domain or Application layers.
    /// </summary>
    public interface ISceneTransitionPresentation
    {
        bool IsCovered { get; }
        bool IsAnimating { get; }
        Task CoverAsync(CancellationToken cancellationToken = default);
        Task RevealAsync(CancellationToken cancellationToken = default);
        void ShowLoadingIndicator(bool visible);
    }
}
