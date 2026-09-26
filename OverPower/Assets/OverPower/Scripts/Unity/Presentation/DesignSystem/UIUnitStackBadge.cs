using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverPower.Unity.Presentation.DesignSystem
{
    /// <summary>
    /// Reusable gameplay HUD primitive representing a unit stack's primary quantity (e.g. x8)
    /// and secondary active-member HP (e.g. 3 / 10).
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu("OverPower/UI/Unit Stack Badge")]
    public sealed class UIUnitStackBadge : MonoBehaviour
    {
        [Header("Design System")]
        [SerializeField] private UIDesignSystemConfig _config;

        [Header("Visual Components")]
        [SerializeField] private Image _background;
        [SerializeField] private Image _border;
        [SerializeField] private TMP_Text _quantityLabel;
        [SerializeField] private TMP_Text _memberHpLabel;
        [SerializeField] private Image _memberHpFill;

        [Header("Runtime State")]
        [SerializeField] private int _displayedQuantity = 1;
        [SerializeField] private int _currentMemberHp = 10;
        [SerializeField] private int _hpPerMember = 10;

        public int DisplayedQuantity => _displayedQuantity;
        public int CurrentMemberHp => _currentMemberHp;
        public int HpPerMember => _hpPerMember;
        public TMP_Text QuantityLabel => _quantityLabel;
        public TMP_Text MemberHpLabel => _memberHpLabel;
        public Image MemberHpFill => _memberHpFill;

        private void Awake()
        {
            ApplyStyle();
            Refresh();
        }

        private void OnEnable()
        {
            ApplyStyle();
            Refresh();
        }

        public void SetValues(int displayedQuantity, int currentMemberHp, int hpPerMember)
        {
            _displayedQuantity = Mathf.Max(0, displayedQuantity);
            _hpPerMember = Mathf.Max(0, hpPerMember);
            _currentMemberHp = _hpPerMember > 0 ? Mathf.Clamp(currentMemberHp, 0, _hpPerMember) : 0;

            Refresh();
        }

        public void ApplyStyle()
        {
            if (_config == null) return;

            if (_background != null)
            {
                _background.color = _config.GetColor(UISemanticColor.Surface);
            }

            if (_border != null)
            {
                _border.color = _config.GetColor(UISemanticColor.FloatingBorder);
            }

            if (_quantityLabel != null)
            {
                _config.ApplyTypography(_quantityLabel, TypographyStyle.Stat);
                _quantityLabel.color = _config.GetColor(UISemanticColor.TextPrimary);
            }

            if (_memberHpLabel != null)
            {
                _config.ApplyTypography(_memberHpLabel, TypographyStyle.Caption);
                _memberHpLabel.color = _config.GetColor(UISemanticColor.TextSecondary);
            }

            if (_memberHpFill != null)
            {
                _memberHpFill.color = _config.GetColor(UISemanticColor.Health);
            }
        }

        private void Refresh()
        {
            if (_quantityLabel != null)
            {
                _quantityLabel.text = $"x{_displayedQuantity.ToString(CultureInfo.InvariantCulture)}";
            }

            if (_memberHpLabel != null)
            {
                _memberHpLabel.text = $"{_currentMemberHp.ToString(CultureInfo.InvariantCulture)} / {_hpPerMember.ToString(CultureInfo.InvariantCulture)}";
            }

            if (_memberHpFill != null)
            {
                float fill = _hpPerMember > 0 ? (float)_currentMemberHp / _hpPerMember : 0f;
                _memberHpFill.fillAmount = Mathf.Clamp01(fill);
            }
        }

        public void InitializeReferences(
            UIDesignSystemConfig config,
            Image background,
            Image border,
            TMP_Text quantityLabel,
            TMP_Text memberHpLabel,
            Image memberHpFill)
        {
            _config = config;
            _background = background;
            _border = border;
            _quantityLabel = quantityLabel;
            _memberHpLabel = memberHpLabel;
            _memberHpFill = memberHpFill;

            ApplyStyle();
            Refresh();
        }
    }
}
