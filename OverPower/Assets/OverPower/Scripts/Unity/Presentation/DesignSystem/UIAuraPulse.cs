using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace OverPower.Unity.Presentation.DesignSystem
{
    /// <summary>
    /// Subtle, ambient breathing motion for decorative UI auras and glows.
    /// Manages an unscaled DOTween sequence with safe lifecycle cleanup and reduced-motion readiness.
    /// </summary>
    public sealed class UIAuraPulse : MonoBehaviour
    {
        [Header("Target References")]
        [SerializeField] private RectTransform _targetTransform;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _targetImage;

        [Header("Alpha Breathing")]
        [SerializeField] private bool _animateAlpha = true;
        [SerializeField] private float _baseAlpha = 0.35f;
        [SerializeField] private float _peakAlpha = 0.90f;

        [Header("Scale Breathing")]
        [SerializeField] private bool _animateScale = true;
        [SerializeField] private float _baseScale = 1.00f;
        [SerializeField] private float _peakScale = 1.012f;

        [Header("Timing & Easing")]
        [SerializeField] private float _halfCycleDuration = 2.6f;
        [SerializeField] private Ease _ease = Ease.InOutSine;
        [SerializeField] private bool _autoPlay = true;

        private Sequence _sequence;
        private bool _isAnimated = true;

        public bool IsAnimated => _isAnimated;
        public float BaseAlpha => _baseAlpha;
        public float PeakAlpha => _peakAlpha;
        public float BaseScale => _baseScale;
        public float PeakScale => _peakScale;
        public float HalfCycleDuration => _halfCycleDuration;
        public Sequence ActiveSequence => _sequence;

        private void Awake()
        {
            if (_targetTransform == null) _targetTransform = transform as RectTransform;
            if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
            if (_targetImage == null) _targetImage = GetComponent<Image>();
        }

        private void OnEnable()
        {
            ApplyRestingState();
            if (_autoPlay && _isAnimated)
            {
                Play();
            }
        }

        private void OnDisable()
        {
            Stop();
            ApplyRestingState();
        }

        private void OnDestroy()
        {
            KillTween();
        }

        /// <summary>
        /// Enables or disables the animation, ensuring clean resting state restoration when disabled.
        /// Suitable for future reduced-motion preferences.
        /// </summary>
        public void SetAnimated(bool enabled)
        {
            _isAnimated = enabled;
            if (!_isAnimated)
            {
                Stop();
                ApplyRestingState();
            }
            else if (isActiveAndEnabled)
            {
                Play();
            }
        }

        public void Play()
        {
            KillTween();

            if (!isActiveAndEnabled || !_isAnimated)
            {
                ApplyRestingState();
                return;
            }

            ApplyRestingState();

            _sequence = DOTween.Sequence();
            _sequence.SetUpdate(true); // Unscaled time for UI
            _sequence.SetRecyclable(false);

            if (_animateAlpha)
            {
                if (_canvasGroup != null)
                {
                    _sequence.Join(DOTween.To(() => _canvasGroup.alpha, a => _canvasGroup.alpha = a, _peakAlpha, _halfCycleDuration).SetEase(_ease));
                }
                else if (_targetImage != null)
                {
                    _sequence.Join(DOTween.To(() => _targetImage.color.a, a =>
                    {
                        var c = _targetImage.color;
                        c.a = a;
                        _targetImage.color = c;
                    }, _peakAlpha, _halfCycleDuration).SetEase(_ease));
                }
            }

            if (_animateScale && _targetTransform != null)
            {
                var targetScale = new Vector3(_peakScale, _peakScale, 1f);
                _sequence.Join(DOTween.To(() => _targetTransform.localScale, s => _targetTransform.localScale = s, targetScale, _halfCycleDuration).SetEase(_ease));
            }

            _sequence.SetLoops(-1, LoopType.Yoyo);
        }

        public void Stop()
        {
            KillTween();
        }

        public void ApplyRestingState()
        {
            if (_canvasGroup != null && _animateAlpha)
            {
                _canvasGroup.alpha = _baseAlpha;
            }
            else if (_targetImage != null && _animateAlpha)
            {
                var c = _targetImage.color;
                c.a = _baseAlpha;
                _targetImage.color = c;
            }

            if (_targetTransform != null && _animateScale)
            {
                _targetTransform.localScale = new Vector3(_baseScale, _baseScale, 1f);
            }
        }

        private void KillTween()
        {
            if (_sequence != null)
            {
                _sequence.Kill();
                _sequence = null;
            }
        }
    }
}
