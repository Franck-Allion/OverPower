using UnityEngine;

namespace OverPower.Unity.Input
{
    public interface IGameInput
    {
        Vector2 Navigate { get; }
        Vector2 Point { get; }

        bool IsClickPressed { get; }
        bool WasSubmitPressedThisFrame { get; }
        bool WasCancelPressedThisFrame { get; }
        bool WasEndTurnPressedThisFrame { get; }
        bool WasOpenDetailsPressedThisFrame { get; }

        void EnableGameplayInput();
        void EnableUIInput();
        void DisableAllInput();
    }
}
