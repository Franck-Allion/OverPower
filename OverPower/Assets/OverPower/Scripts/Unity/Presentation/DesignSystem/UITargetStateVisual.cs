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
        [SerializeField] private Image[] _cornerImages;
        [SerializeField] private Image[] _blockedImages;

        public TargetVisualState State { get; private set; }
        public Image Outline => _outline;
        public GameObject CornerCue => _cornerCue;
        public GameObject BlockedCue => _blockedCue;
        public Image SelectedGlow => _selectedGlow;

        private void OnEnable() => Apply();

        public void SetState(TargetVisualState state)
        {
            State = state;
            Apply();
        }

        private void Apply()
        {
            bool valid = State == TargetVisualState.Valid;
            bool invalid = State == TargetVisualState.Invalid;
            bool selected = State == TargetVisualState.Selected;
            bool isNone = State == TargetVisualState.None;

            if (_outline != null)
            {
                _outline.enabled = !isNone;
                if (_config != null)
                {
                    _outline.color = isNone
                        ? Color.clear
                        : _config.GetColor(invalid ? UISemanticColor.Danger : valid ? UISemanticColor.Success : UISemanticColor.Selected);
                }
            }

            if (_cornerCue != null)
            {
                _cornerCue.SetActive(valid || selected);
                if ((valid || selected) && _config != null)
                {
                    Color cornerColor = _config.GetColor(valid ? UISemanticColor.Success : UISemanticColor.Selected);
                    var images = _cornerImages != null && _cornerImages.Length > 0 ? _cornerImages : _cornerCue.GetComponentsInChildren<Image>(true);
                    if (images != null)
                    {
                        for (int i = 0; i < images.Length; i++)
                        {
                            if (images[i] != null) images[i].color = cornerColor;
                        }
                    }
                }
            }

            if (_blockedCue != null)
            {
                _blockedCue.SetActive(invalid);
                if (invalid && _config != null)
                {
                    Color dangerColor = _config.GetColor(UISemanticColor.Danger);
                    var images = _blockedImages != null && _blockedImages.Length > 0 ? _blockedImages : _blockedCue.GetComponentsInChildren<Image>(true);
                    if (images != null)
                    {
                        for (int i = 0; i < images.Length; i++)
                        {
                            if (images[i] != null)
                            {
                                if (images[i].name == "BlockedRim")
                                {
                                    Color rim = dangerColor;
                                    rim.a = 0.40f;
                                    images[i].color = rim;
                                }
                                else if (images[i].name.StartsWith("Cross"))
                                {
                                    images[i].color = dangerColor;
                                }
                            }
                        }
                    }
                }
            }

            if (_selectedGlow != null)
            {
                _selectedGlow.gameObject.SetActive(selected);
                if (selected && _config != null)
                {
                    Color glowColor = _config.GetColor(UISemanticColor.Selected);
                    glowColor.a = 0.28f;
                    _selectedGlow.color = glowColor;
                }
            }
        }
    }
}
