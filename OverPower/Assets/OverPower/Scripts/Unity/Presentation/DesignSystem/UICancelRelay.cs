using UnityEngine;
using UnityEngine.EventSystems;

namespace OverPower.Unity.Presentation.DesignSystem
{
    /// <summary>Routes the Input System UI Cancel event from a selected action to its dialog.</summary>
    public sealed class UICancelRelay : MonoBehaviour, ICancelHandler
    {
        [SerializeField] private UIConfirmDialog _dialog;
        public void OnCancel(BaseEventData eventData) { _dialog.Cancel(); eventData.Use(); }
    }
}
