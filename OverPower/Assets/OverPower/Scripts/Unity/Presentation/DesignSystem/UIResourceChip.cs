using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverPower.Unity.Presentation.DesignSystem
{
    [ExecuteAlways]
    public sealed class UIResourceChip : MonoBehaviour
    {
        [SerializeField] private UIDesignSystemConfig _config;
        [SerializeField] private UISemanticColor _tone = UISemanticColor.Primary;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _valueLabel;
        [SerializeField] private TMP_Text _maximumLabel;
        [SerializeField] private int _value;
        [SerializeField] private int _maximum;
        [SerializeField] private bool _hasMaximum;
        private void OnEnable()
        {
            if (_config == null) return;
            _config.ApplyTypography(_valueLabel, TypographyStyle.Stat);
            _config.ApplyTypography(_maximumLabel, TypographyStyle.BodySmall);
            _valueLabel.color = _config.GetColor(UISemanticColor.TextPrimary);
            _maximumLabel.color = _config.GetColor(UISemanticColor.TextSecondary);
            _icon.color = _config.GetColor(_tone);
            RefreshValue();
        }
        public void SetValue(int value) { _value = value; _hasMaximum = false; RefreshValue(); }
        public void SetValue(int current, int maximum) { _value = current; _maximum = maximum; _hasMaximum = true; RefreshValue(); }
        public void SetIcon(Sprite sprite) { _icon.sprite = sprite; _icon.gameObject.SetActive(sprite != null); }
        public void SetTone(UISemanticColor tone) { _tone = tone; _icon.color = _config.GetColor(tone); }
        private void RefreshValue()
        {
            _valueLabel.text = _value.ToString(CultureInfo.InvariantCulture);
            _maximumLabel.gameObject.SetActive(_hasMaximum);
            if (_hasMaximum) _maximumLabel.text = "/ " + _maximum.ToString(CultureInfo.InvariantCulture);
        }
    }
}
