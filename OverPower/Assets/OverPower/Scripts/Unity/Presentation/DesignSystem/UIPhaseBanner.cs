using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverPower.Unity.Presentation.DesignSystem
{
    /// <summary>Transient, presentation-only banner for a localized phase label.</summary>
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class UIPhaseBanner : MonoBehaviour
    {
        [SerializeField] private UIDesignSystemConfig _config;
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private RectTransform _visual;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Image _accent;
        private Tween _tween;
        public bool IsVisible { get; private set; }

        private void Awake() { if (_group == null) _group = GetComponent<CanvasGroup>(); HideImmediate(); }
        public void Show(string localizedLabel, UISemanticColor tone)
        {
            if (_label != null) { _label.text = localizedLabel ?? string.Empty; if (_config != null) _config.ApplyTypography(_label, TypographyStyle.Heading); }
            if (_accent != null && _config != null) _accent.color = _config.GetColor(tone);
            SetVisible(true, false);
        }
        public void Hide() => SetVisible(false, false);
        public void HideImmediate() => SetVisible(false, true);
        private void SetVisible(bool visible, bool immediate)
        {
            _tween?.Kill(); _tween = null; IsVisible = visible;
            if (_group == null) _group = GetComponent<CanvasGroup>();
            _group.blocksRaycasts = _group.interactable = false;
            float duration = _config != null ? _config.GetDuration(UIMotion.Fast) : .15f;
            if (immediate || !isActiveAndEnabled || !UnityEngine.Application.isPlaying) { Apply(visible ? 1f : 0f); return; }
            _tween = DOTween.To(() => _group.alpha, Apply, visible ? 1f : 0f, duration).SetUpdate(true).SetEase(Ease.OutCubic).OnComplete(() => _tween = null);
        }
        private void Apply(float value) { _group.alpha = value; if (_visual != null) _visual.localScale = Vector3.one * Mathf.Lerp(.96f, 1f, value); }
        private void OnDisable() { _tween?.Kill(); _tween = null; IsVisible = false; if (_group != null) _group.alpha = 0; }
        private void OnDestroy() { _tween?.Kill(); }
    }
}
