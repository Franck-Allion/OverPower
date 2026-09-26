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
        [SerializeField] private string _labelPrefix = "";
        [SerializeField] private int _value;
        [SerializeField] private int _maximum;
        [SerializeField] private bool _hasMaximum;

        public int Value => _value;
        public int Maximum => _maximum;
        public bool HasMaximum => _hasMaximum;
        public UISemanticColor Tone => _tone;
        public string LabelPrefix { get => _labelPrefix; set { _labelPrefix = value; RefreshValue(); } }

        private void OnEnable()
        {
            if (_config == null) return;
            if (_valueLabel != null)
            {
                _config.ApplyTypography(_valueLabel, TypographyStyle.Stat);
                _valueLabel.color = _config.GetColor(UISemanticColor.TextPrimary);
            }
            if (_maximumLabel != null)
            {
                _config.ApplyTypography(_maximumLabel, TypographyStyle.BodySmall);
                _maximumLabel.color = _config.GetColor(UISemanticColor.TextSecondary);
            }
            if (_icon != null)
            {
                _icon.color = _config.GetColor(_tone);
            }
            RefreshValue();
        }

        public void SetValue(int value) { _value = value; _hasMaximum = false; RefreshValue(); }
        public void SetValue(int current, int maximum) { _value = current; _maximum = maximum; _hasMaximum = true; RefreshValue(); }
        public void SetPrefix(string prefix) { _labelPrefix = prefix; RefreshValue(); }
        public void SetIcon(Sprite sprite) { if (_icon != null) { _icon.sprite = sprite; _icon.gameObject.SetActive(sprite != null); } }
        public void SetTone(UISemanticColor tone) { _tone = tone; if (_icon != null && _config != null) _icon.color = _config.GetColor(tone); }

        private void RefreshValue()
        {
            string prefix = !string.IsNullOrEmpty(_labelPrefix) ? _labelPrefix + " " : "";
            if (_valueLabel != null)
            {
                _valueLabel.text = prefix + _value.ToString(CultureInfo.InvariantCulture);
            }
            if (_maximumLabel != null)
            {
                _maximumLabel.gameObject.SetActive(_hasMaximum);
                if (_hasMaximum) _maximumLabel.text = "/ " + _maximum.ToString(CultureInfo.InvariantCulture);
            }
        }
    }
}
