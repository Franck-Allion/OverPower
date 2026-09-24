using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace OverPower.Unity.Presentation.DesignSystem
{
    /// <summary>Routes the Input System UI Cancel event from a selected action to its dialog or handler.</summary>
    public sealed class UICancelRelay : MonoBehaviour, ICancelHandler
    {
        [SerializeField] private UIConfirmDialog _dialog;
        [SerializeField] private UnityEvent _onCancel = new UnityEvent();

        public static event System.Action<UICancelRelay> OnCancelFeedback;

        public UnityEvent OnCancelEvent => _onCancel;

        public void BindDialog(UIConfirmDialog dialog) => _dialog = dialog;

        public void OnCancel(BaseEventData eventData)
        {
            if (_dialog != null)
            {
                _dialog.Cancel();
            }
            _onCancel?.Invoke();
            OnCancelFeedback?.Invoke(this);
            eventData.Use();
        }
    }
}
