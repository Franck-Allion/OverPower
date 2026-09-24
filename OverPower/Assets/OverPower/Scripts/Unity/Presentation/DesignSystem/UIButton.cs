using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace OverPower.Unity.Presentation.DesignSystem
{
    public enum UIInteractionType
    {
        Hover,
        Focus,
        Press,
        Submit,
        Cancel,
        Close
    }

    /// <summary>Shared uGUI interaction for the three prefab families. Screen composition owns actions and labels.</summary>
    [AddComponentMenu("OverPower/UI/Button")]
    public sealed class UIButton : Button
    {
        [SerializeField] private UIDesignSystemConfig _config;
        [SerializeField] private UIButtonFamily _family;
        [SerializeField] private RectTransform _visual;
        [SerializeField] private Image _surface;
        [SerializeField] private Image _border;
        [SerializeField] private GameObject _focusMark;
        [SerializeField] private GameObject _disabledMark;
        [SerializeField] private CanvasGroup _visualOpacity;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Image _icon;
        [SerializeField] private LocalizedString _accessibleLabel = new LocalizedString();

        [Header("Audio Feedback Hooks")]
        [SerializeField] private UnityEvent _onHoverFeedback = new UnityEvent();
        [SerializeField] private UnityEvent _onPressFeedback = new UnityEvent();
        [SerializeField] private UnityEvent _onSubmitFeedback = new UnityEvent();
        [SerializeField] private UIInteractionType _submitInteractionType = UIInteractionType.Submit;

        public static event Action<UIButton, UIInteractionType> OnButtonFeedback;

        private Tween _scaleTween;

        public LocalizedString AccessibleLabel => _accessibleLabel;
        public Image Icon => _icon;
        public UIButtonFamily Family => _family;
        public UnityEvent OnHoverFeedback => _onHoverFeedback;
        public UnityEvent OnPressFeedback => _onPressFeedback;
        public UnityEvent OnSubmitFeedback => _onSubmitFeedback;
        public UIInteractionType SubmitInteractionType
        {
            get => _submitInteractionType;
            set => _submitInteractionType = value;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (_config == null) return; // AddComponent/editor composition has not wired serialized references yet.
            var timings = colors;
            timings.fadeDuration = _config.GetDuration(UIMotion.Fast); // uGUI's submit hold uses ColorBlock timing.
            colors = timings;
            if (_label != null) _config.ApplyTypography(_label, TypographyStyle.Button);
            DoStateTransition(currentSelectionState, true);
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (_config == null || _visual == null) return;
            _scaleTween?.Kill();
            _scaleTween = null;
            bool disabled = state == SelectionState.Disabled;
            bool isSelected = state == SelectionState.Selected;
            bool isHovered = state == SelectionState.Highlighted;
            bool focused = isSelected || isHovered;
            bool pressed = state == SelectionState.Pressed;
            bool primary = _family == UIButtonFamily.Primary;

            if (!instant && !disabled)
            {
                if (state == SelectionState.Highlighted)
                {
                    _onHoverFeedback?.Invoke();
                    OnButtonFeedback?.Invoke(this, UIInteractionType.Hover);
                }
                else if (state == SelectionState.Selected)
                {
                    _onHoverFeedback?.Invoke();
                    OnButtonFeedback?.Invoke(this, UIInteractionType.Focus);
                }
                else if (state == SelectionState.Pressed)
                {
                    _onPressFeedback?.Invoke();
                    OnButtonFeedback?.Invoke(this, UIInteractionType.Press);
                }
            }

            var accent = disabled ? UISemanticColor.Disabled : focused || pressed
                ? UISemanticColor.Interactive : primary ? UISemanticColor.Primary : UISemanticColor.Secondary;
            _border.color = _config.GetColor(accent);
            _surface.color = _config.GetColor(primary && !disabled ? accent : UISemanticColor.Surface);
            var ink = _config.GetColor(primary && !disabled ? UISemanticColor.Background : UISemanticColor.TextPrimary);
            if (_label != null) _label.color = ink;
            if (_icon != null) _icon.color = ink;
            _focusMark.SetActive(isSelected && !disabled);
            _disabledMark.SetActive(disabled);
            _visualOpacity.alpha = disabled ? _config.DisabledOpacity : 1f;
            var scale = Vector3.one * (pressed ? _config.PressedScale : focused ? _config.HoverScale : 1f);
            if (instant || disabled || !UnityEngine.Application.isPlaying || !isActiveAndEnabled)
                _visual.localScale = scale;
            else
                _scaleTween = DOTween.To(() => _visual.localScale, value => _visual.localScale = value,
                    scale, _config.GetDuration(pressed ? UIMotion.Fast : UIMotion.Normal))
                    .SetEase(Ease.OutCubic).SetUpdate(true).SetRecyclable(false);
        }

        public override void OnSubmit(BaseEventData eventData)
        {
            base.OnSubmit(eventData);
            if (IsInteractable())
            {
                _onSubmitFeedback?.Invoke();
                OnButtonFeedback?.Invoke(this, _submitInteractionType);
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            if (IsInteractable())
            {
                _onSubmitFeedback?.Invoke();
                OnButtonFeedback?.Invoke(this, _submitInteractionType);
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ResetMotion();
        }

        protected override void OnDestroy()
        {
            ResetMotion();
            base.OnDestroy();
        }

        private void ResetMotion()
        {
            _scaleTween?.Kill();
            _scaleTween = null;
            if (_visual != null) _visual.localScale = Vector3.one;
        }
    }
}
