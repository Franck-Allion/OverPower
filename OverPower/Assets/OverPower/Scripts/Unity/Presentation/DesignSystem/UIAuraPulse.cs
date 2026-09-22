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
        [SerializeField] private Graphic _targetGraphic;

        [Header("Alpha Breathing")]
        [SerializeField] private bool _animateAlpha = true;
        [SerializeField] private float _baseAlpha = 0.18f;
        [SerializeField] private float _peakAlpha = 0.25f;

        [Header("Scale Breathing")]
        [SerializeField] private bool _animateScale = true;
        [SerializeField] private float _baseScale = 1.00f;
        [SerializeField] private float _peakScale = 1.018f;

        [Header("Timing & Easing")]
        [SerializeField] private float _halfCycleDuration = 3.2f;
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
            ResolveReferences();
        }

        private void OnEnable()
        {
            ResolveReferences();
            if (_autoPlay && _isAnimated)
            {
                StartAnimation();
            }
            else
            {
                RestoreBaseState();
            }
        }

        private void OnDisable()
        {
            StopAnimation();
            RestoreBaseState();
        }

        private void OnDestroy()
        {
            StopAnimation();
        }

        private void ResolveReferences()
        {
            if (_targetTransform == null) _targetTransform = transform as RectTransform;
            if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
            if (_targetGraphic == null) _targetGraphic = GetComponent<Graphic>();
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
                StopAnimation();
                RestoreBaseState();
            }
            else if (isActiveAndEnabled)
            {
                StartAnimation();
            }
        }

        public void Play()
        {
            StartAnimation();
        }

        public void Stop()
        {
            StopAnimation();
            RestoreBaseState();
        }

        public void StartAnimation()
        {
            StopAnimation();

            if (!isActiveAndEnabled || !_isAnimated)
            {
                RestoreBaseState();
                return;
            }

            ResolveReferences();
            RestoreBaseState();

            _sequence = DOTween.Sequence();

            Tween alphaTween = null;
            if (_animateAlpha)
            {
                if (_canvasGroup != null)
                {
                    alphaTween = DOTween.To(() => _canvasGroup.alpha, a => _canvasGroup.alpha = a, _peakAlpha, _halfCycleDuration).SetEase(_ease);
                }
                else if (_targetGraphic != null)
                {
                    alphaTween = DOTween.To(() => _targetGraphic.color.a, a =>
                    {
                        var c = _targetGraphic.color;
                        c.a = a;
                        _targetGraphic.color = c;
                    }, _peakAlpha, _halfCycleDuration).SetEase(_ease);
                }
            }

            Tween scaleTween = null;
            if (_animateScale && _targetTransform != null)
            {
                var targetScale = Vector3.one * _peakScale;
                scaleTween = DOTween.To(() => _targetTransform.localScale, s => _targetTransform.localScale = s, targetScale, _halfCycleDuration).SetEase(_ease);
            }

            if (alphaTween != null)
            {
                _sequence.Append(alphaTween);
                if (scaleTween != null) _sequence.Join(scaleTween);
            }
            else if (scaleTween != null)
            {
                _sequence.Append(scaleTween);
            }

            _sequence
                .SetLoops(-1, LoopType.Yoyo)
                .SetUpdate(true) // Unscaled time for UI
                .SetLink(gameObject, LinkBehaviour.KillOnDisable);
        }

        public void StopAnimation()
        {
            if (_sequence != null)
            {
                _sequence.Kill();
                _sequence = null;
            }
        }

        public void RestoreBaseState()
        {
            if (_canvasGroup != null && _animateAlpha)
            {
                _canvasGroup.alpha = _baseAlpha;
            }
            else if (_targetGraphic != null && _animateAlpha)
            {
                var c = _targetGraphic.color;
                c.a = _baseAlpha;
                _targetGraphic.color = c;
            }

            if (_targetTransform != null && _animateScale)
            {
                _targetTransform.localScale = Vector3.one * _baseScale;
            }
        }
    }
}
