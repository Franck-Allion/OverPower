using UnityEngine;
using UnityEngine.UI;

namespace OverPower.Unity.Presentation.DesignSystem
{
    public enum TargetVisualState { None, Valid, Invalid, Selected }

    /// <summary>Presentation-only target cue; eligibility remains in an application/use-case layer.</summary>
    public sealed class UITargetStateVisual : MonoBehaviour
    {
        [SerializeField] private UIDesignSystemConfig _config;
        [SerializeField] private Image _outline;
        [SerializeField] private GameObject _cornerCue;
        [SerializeField] private GameObject _blockedCue;
        [SerializeField] private Image _selectedGlow;
        public TargetVisualState State { get; private set; }
        private void OnEnable() => Apply();
        public void SetState(TargetVisualState state) { State = state; Apply(); }
        private void Apply()
        {
            bool valid = State == TargetVisualState.Valid, invalid = State == TargetVisualState.Invalid, selected = State == TargetVisualState.Selected;
            if (_outline != null && _config != null) _outline.color = _config.GetColor(invalid ? UISemanticColor.Danger : valid ? UISemanticColor.Success : selected ? UISemanticColor.Selected : UISemanticColor.FloatingBorder);
            if (_cornerCue != null) _cornerCue.SetActive(valid || selected);
            if (_blockedCue != null) _blockedCue.SetActive(invalid);
            if (_selectedGlow != null) _selectedGlow.gameObject.SetActive(selected);
        }
    }
}
