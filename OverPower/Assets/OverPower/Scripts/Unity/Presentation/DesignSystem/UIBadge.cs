using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverPower.Unity.Presentation.DesignSystem
{
    public sealed class UIBadge : MonoBehaviour
    {
        [SerializeField] private UIDesignSystemConfig _config;
        [SerializeField] private UISemanticColor _tone = UISemanticColor.Secondary;
        [SerializeField] private Image _accent;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _label;
        private void OnEnable() { if (_config != null) Apply(); }
        public void SetLabel(string label) => _label.text = label;
        public void SetTone(UISemanticColor tone) { _tone = tone; Apply(); }
        public void SetIcon(Sprite icon) { _icon.sprite = icon; _icon.gameObject.SetActive(icon != null); }
        private void Apply()
        {
            _config.ApplyTypography(_label, TypographyStyle.Caption);
            _label.color = _config.GetColor(UISemanticColor.TextPrimary);
            _accent.color = _config.GetColor(_tone);
            _icon.color = _config.GetColor(_tone);
            _icon.gameObject.SetActive(_icon.sprite != null);
        }
    }
}
