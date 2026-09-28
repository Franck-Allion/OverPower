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
        [SerializeField] private Image[] _accentImages;
        [SerializeField] private bool _initiallyVisible;

        private Tween _tween;

        public bool IsVisible { get; private set; }
        public TMP_Text Label => _label;
        public Image Accent => _accent;
        public Image[] AccentImages => _accentImages;

        private void Awake()
        {
            if (_group == null) _group = GetComponent<CanvasGroup>();
            if (_initiallyVisible) SetVisible(true, true);
            else HideImmediate();
        }

        public void Show(string localizedLabel, UISemanticColor tone)
        {
            if (_label != null)
            {
                _label.text = localizedLabel ?? string.Empty;
                if (_config != null)
                {
                    _config.ApplyTypography(_label, TypographyStyle.Heading, preserveSizeAndAutoSizing: true);
                }
            }

            if (_config != null)
            {
                Color toneColor = _config.GetColor(tone);
                if (_accent != null) _accent.color = toneColor;
                if (_accentImages != null)
                {
                    for (int i = 0; i < _accentImages.Length; i++)
                    {
                        var img = _accentImages[i];
                        if (img != null)
                        {
                            if (img.name == "CenterGlow")
                            {
                                img.color = new Color(toneColor.r, toneColor.g, toneColor.b, 0.14f);
                            }
                            else if (img.name == "UnderlineAccent")
                            {
                                img.color = new Color(toneColor.r, toneColor.g, toneColor.b, 0.55f);
                            }
                            else
                            {
                                img.color = toneColor;
                            }
                        }
                    }
                }
            }

            SetVisible(true, false);
        }

        public void Hide() => SetVisible(false, false);
        public void HideImmediate() => SetVisible(false, true);

        private void SetVisible(bool visible, bool immediate)
        {
            _tween?.Kill();
            _tween = null;
            IsVisible = visible;

            if (_group == null) _group = GetComponent<CanvasGroup>();
            _group.blocksRaycasts = false;
            _group.interactable = false;

            float duration = visible ? 0.22f : 0.16f;
            if (immediate || !isActiveAndEnabled || !UnityEngine.Application.isPlaying)
            {
                Apply(visible ? 1f : 0f);
                return;
            }

            Ease ease = visible ? Ease.OutCubic : Ease.InQuad;
            _tween = DOTween.To(() => _group.alpha, Apply, visible ? 1f : 0f, duration)
                .SetUpdate(true)
                .SetEase(ease)
                .OnComplete(() => _tween = null);
        }

        private void Apply(float value)
        {
            if (_group != null) _group.alpha = value;
            if (_visual != null) _visual.localScale = Vector3.one * Mathf.Lerp(0.97f, 1f, value);
        }

        private void OnDisable()
        {
            _tween?.Kill();
            _tween = null;
            IsVisible = false;
            if (_group != null)
            {
                _group.alpha = 0f;
                _group.blocksRaycasts = false;
                _group.interactable = false;
            }
        }

        private void OnDestroy()
        {
            _tween?.Kill();
        }
    }
}
