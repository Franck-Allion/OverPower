using System;
using System.Collections.Generic;
using OverPower.Application;

namespace OverPower.Unity.SceneFlow
{
    public static class GameSceneCatalog
    {
        private static readonly Dictionary<GameSceneId, string> ScenePaths = new Dictionary<GameSceneId, string>
        {
            { GameSceneId.Bootstrap, "Assets/OverPower/Scenes/Bootstrap.unity" },
            { GameSceneId.MainMenu, "Assets/OverPower/Scenes/MainMenu.unity" },
            { GameSceneId.Exploration, "Assets/OverPower/Scenes/Exploration.unity" },
            { GameSceneId.Battle, "Assets/OverPower/Scenes/Battle.unity" },
            { GameSceneId.MetaProgression, "Assets/OverPower/Scenes/MetaProgression.unity" }
        };

        public static string GetScenePath(GameSceneId sceneId)
        {
            if (ScenePaths.TryGetValue(sceneId, out string path))
            {
                return path;
            }
            throw new ArgumentException($"No scene mapping found for GameSceneId: {sceneId}", nameof(sceneId));
        }

        public static bool Contains(GameSceneId sceneId)
        {
            return ScenePaths.ContainsKey(sceneId);
        }
    }
}
