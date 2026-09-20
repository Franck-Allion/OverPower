using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;

namespace OverPower.Unity.Presentation.DesignSystem
{
    public sealed class UITooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private UIDesignSystemConfig _config;
        [SerializeField] private UITooltip _tooltip;
        [SerializeField] private LocalizedString _title = new LocalizedString();
        [SerializeField] private LocalizedString _body = new LocalizedString();
        [SerializeField] private Sprite _icon;
        private Tween _delay;
        private bool _hovered, _focused, _explicitDetails;
        private string _titleText, _bodyText;
        private Selectable _selectable;
        private Canvas _canvas;

        private void OnEnable()
        {
            _selectable = GetComponent<Selectable>();
            _canvas = GetComponentInParent<Canvas>();
            _title.StringChanged += TitleChanged;
            _body.StringChanged += BodyChanged;
        }
        private void TitleChanged(string value) { _titleText = value; RefreshOpenTooltip(); }
        private void BodyChanged(string value) { _bodyText = value; RefreshOpenTooltip(); }
        private bool WantsDetails => isActiveAndEnabled && (_hovered || _focused || _explicitDetails)
            && (_selectable == null || _selectable.IsInteractable());
        public void OnPointerEnter(PointerEventData data) { _hovered = true; Schedule(); }
        public void OnPointerExit(PointerEventData data) { _hovered = false; Schedule(); }
        public void OnSelect(BaseEventData data) { _focused = true; Schedule(); }
        public void OnDeselect(BaseEventData data) { _focused = false; _explicitDetails = false; Schedule(); }
        public void OpenDetails() { _explicitDetails = true; Schedule(); }
        public void CloseDetails() { _explicitDetails = _hovered = _focused = false; CancelDelay(); _tooltip.Hide(this); }

        private void Schedule()
        {
            CancelDelay();
            if (!WantsDetails) { _tooltip.Hide(this); return; }
            _delay = DOVirtual.DelayedCall(_config.GetDuration(UIMotion.Emphasis),
                () => { _delay = null; if (WantsDetails) Show(); }, true).SetRecyclable(false);
        }
        private void RefreshOpenTooltip() { if (_delay == null && WantsDetails) Show(); }
        private void Show()
        {
            var rect = (RectTransform)transform;
            var camera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
            var screen = RectTransformUtility.WorldToScreenPoint(camera, rect.TransformPoint(rect.rect.center));

            string formattedTitle = UIRichTextFormatter.Format(_titleText ?? string.Empty);
            string formattedBody = UIRichTextFormatter.Format(_bodyText ?? string.Empty);

            _tooltip.Show(this, formattedTitle, formattedBody, _icon, screen);
        }
        private void CancelDelay() { _delay?.Kill(); _delay = null; }
        private void OnDisable()
        {
            CancelDelay();
            _hovered = _focused = _explicitDetails = false;
            _title.StringChanged -= TitleChanged;
            _body.StringChanged -= BodyChanged;
            if (_tooltip != null) _tooltip.Hide(this);
        }
        private void OnDestroy() => CancelDelay();
    }
}
