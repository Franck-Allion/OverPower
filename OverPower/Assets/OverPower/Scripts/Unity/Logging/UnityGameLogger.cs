using UnityEngine;
using OverPower.Application.Ports.Logging;

namespace OverPower.Unity.Logging
{
    /// <summary>
    /// Concrete Unity adapter for <see cref="IGameLogger"/>, forwarding log messages to Unity's console.
    /// </summary>
    public sealed class UnityGameLogger : IGameLogger
    {
        public void Debug(string category, string message)
        {
            UnityEngine.Debug.Log($"[{category}] {message}");
        }

        public void Info(string category, string message)
        {
            UnityEngine.Debug.Log($"[{category}] {message}");
        }

        public void Warning(string category, string message)
        {
            UnityEngine.Debug.LogWarning($"[{category}] {message}");
        }

        public void Error(string category, string message)
        {
            UnityEngine.Debug.LogError($"[{category}] {message}");
        }
    }
}
