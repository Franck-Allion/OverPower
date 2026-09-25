using System;
using System.Threading;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using OverPower.Unity.Presentation.DesignSystem;
using OverPower.Unity.SceneFlow;

namespace OverPower.Unity.Presentation.SceneTransition
{
    /// <summary>
    /// Persistent, fullscreen scene transition overlay providing restrained fades,
    /// raycast blocking, and optional delayed loading indicator animation.
    /// </summary>
    [AddComponentMenu("OverPower/UI/Scene Transition Overlay")]
    public sealed class UISceneTransitionOverlay : MonoBehaviour, ISceneTransitionPresentation
    {
        [Header("Hierarchy References")]
        [SerializeField] private Canvas _canvas;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private Image _overlayImage;
        [SerializeField] private RectTransform _loadingIndicator;
        [SerializeField] private CanvasGroup _loadingIndicatorGroup;
        [SerializeField] private UIDesignSystemConfig _config;

        [Header("Timing & Easing")]
        [SerializeField] private float _coverDuration = 0.25f;
        [SerializeField] private float _revealDuration = 0.30f;
        [SerializeField] private float _indicatorFadeDuration = 0.15f;
        [SerializeField] private Ease _ease = Ease.InOutSine;

        private Tween _fadeTween;
        private Tween _indicatorFadeTween;
        private Tween _indicatorRotateTween;
        private bool _isCovered;
        private bool _isAnimating;

        public bool IsCovered => _isCovered;
        public bool IsAnimating => _isAnimating;
        public CanvasGroup CanvasGroup => _canvasGroup;
        public RectTransform LoadingIndicator => _loadingIndicator;
        public float CoverDuration { get => _coverDuration; set => _coverDuration = value; }
        public float RevealDuration { get => _revealDuration; set => _revealDuration = value; }

        private void Awake()
        {
            EnsureComponents();
        }

        private void OnDestroy()
        {
            KillTweens();
        }

        private void OnDisable()
        {
            KillTweens();
        }

        public void EnsureComponents()
        {
            if (_canvas == null)
            {
                _canvas = GetComponent<Canvas>();
            }
            if (_canvas != null)
            {
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                _canvas.sortingOrder = 9999;
            }

            if (_canvasGroup == null)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
                if (_canvasGroup == null)
                {
                    _canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }

            if (_overlayImage != null && _config != null)
            {
                _overlayImage.color = _config.GetColor(UISemanticColor.Background);
            }
        }

        public void SetCoveredImmediate()
        {
            KillTweens();
            EnsureComponents();
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 1f;
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.interactable = true;
            }
            _isCovered = true;
            _isAnimating = false;
        }

        public void SetRevealedImmediate()
        {
            KillTweens();
            EnsureComponents();
            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.interactable = false;
            }
            ShowLoadingIndicatorImmediate(false);
            _isCovered = false;
            _isAnimating = false;
        }

        public Task CoverAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            EnsureComponents();

            // Already covered and not animating
            if (_isCovered && !_isAnimating && _canvasGroup != null && Mathf.Approximately(_canvasGroup.alpha, 1f))
            {
                return Task.CompletedTask;
            }

            KillTweens();
            _isAnimating = true;

            if (_canvasGroup != null)
            {
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.interactable = true;
            }

            var tcs = new TaskCompletionSource<bool>();
            CancellationTokenRegistration registration = default;

            if (cancellationToken.CanBeCanceled)
            {
                registration = cancellationToken.Register(() =>
                {
                    KillTweens();
                    _isAnimating = false;
                    tcs.TrySetCanceled(cancellationToken);
                });
            }

            float duration = _coverDuration;
            if (!UnityEngine.Application.isPlaying || duration <= 0f)
            {
                SetCoveredImmediate();
                registration.Dispose();
                return Task.CompletedTask;
            }

            _fadeTween = DOTween.To(
                () => _canvasGroup != null ? _canvasGroup.alpha : 0f,
                alpha => { if (_canvasGroup != null) _canvasGroup.alpha = alpha; },
                1f,
                duration)
                .SetEase(_ease)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    if (_canvasGroup != null) _canvasGroup.alpha = 1f;
                    _isCovered = true;
                    _isAnimating = false;
                    _fadeTween = null;
                    registration.Dispose();
                    tcs.TrySetResult(true);
                });

            return tcs.Task;
        }

        public Task RevealAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            EnsureComponents();

            // Ensure indicator is hidden when revealing
            ShowLoadingIndicator(false);

            // Already revealed and not animating
            if (!_isCovered && !_isAnimating && _canvasGroup != null && Mathf.Approximately(_canvasGroup.alpha, 0f))
            {
                return Task.CompletedTask;
            }

            KillTweens();
            _isAnimating = true;

            var tcs = new TaskCompletionSource<bool>();
            CancellationTokenRegistration registration = default;

            if (cancellationToken.CanBeCanceled)
            {
                registration = cancellationToken.Register(() =>
                {
                    KillTweens();
                    _isAnimating = false;
                    tcs.TrySetCanceled(cancellationToken);
                });
            }

            float duration = _revealDuration;
            if (!UnityEngine.Application.isPlaying || duration <= 0f)
            {
                SetRevealedImmediate();
                registration.Dispose();
                return Task.CompletedTask;
            }

            _fadeTween = DOTween.To(
                () => _canvasGroup != null ? _canvasGroup.alpha : 1f,
                alpha => { if (_canvasGroup != null) _canvasGroup.alpha = alpha; },
                0f,
                duration)
                .SetEase(_ease)
                .SetUpdate(true)
                .OnComplete(() =>
                {
                    if (_canvasGroup != null)
                    {
                        _canvasGroup.alpha = 0f;
                        _canvasGroup.blocksRaycasts = false;
                        _canvasGroup.interactable = false;
                    }
                    _isCovered = false;
                    _isAnimating = false;
                    _fadeTween = null;
                    registration.Dispose();
                    tcs.TrySetResult(true);
                });

            return tcs.Task;
        }

        public void ShowLoadingIndicator(bool visible)
        {
            if (_loadingIndicatorGroup == null || _loadingIndicator == null)
            {
                return;
            }

            _indicatorFadeTween?.Kill();

            if (visible)
            {
                StartIndicatorRotation();

                if (!UnityEngine.Application.isPlaying)
                {
                    _loadingIndicatorGroup.alpha = 1f;
                    return;
                }

                _indicatorFadeTween = DOTween.To(
                    () => _loadingIndicatorGroup.alpha,
                    val => _loadingIndicatorGroup.alpha = val,
                    1f,
                    _indicatorFadeDuration)
                    .SetUpdate(true);
            }
            else
            {
                if (!UnityEngine.Application.isPlaying)
                {
                    _loadingIndicatorGroup.alpha = 0f;
                    StopIndicatorRotation();
                    return;
                }

                _indicatorFadeTween = DOTween.To(
                    () => _loadingIndicatorGroup.alpha,
                    val => _loadingIndicatorGroup.alpha = val,
                    0f,
                    _indicatorFadeDuration)
                    .SetUpdate(true)
                    .OnComplete(StopIndicatorRotation);
            }
        }

        private void ShowLoadingIndicatorImmediate(bool visible)
        {
            _indicatorFadeTween?.Kill();
            if (_loadingIndicatorGroup != null)
            {
                _loadingIndicatorGroup.alpha = visible ? 1f : 0f;
            }
            if (visible)
            {
                StartIndicatorRotation();
            }
            else
            {
                StopIndicatorRotation();
            }
        }

        private void StartIndicatorRotation()
        {
            if (_loadingIndicator == null) return;
            if (_indicatorRotateTween != null && _indicatorRotateTween.IsActive()) return;

            _loadingIndicator.localRotation = Quaternion.identity;
            _indicatorRotateTween = _loadingIndicator.DORotate(new Vector3(0, 0, -360), 1.2f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1)
                .SetUpdate(true);
        }

        private void StopIndicatorRotation()
        {
            _indicatorRotateTween?.Kill();
            _indicatorRotateTween = null;
            if (_loadingIndicator != null)
            {
                _loadingIndicator.localRotation = Quaternion.identity;
            }
        }

        private void KillTweens()
        {
            _fadeTween?.Kill();
            _fadeTween = null;
            _indicatorFadeTween?.Kill();
            _indicatorFadeTween = null;
            StopIndicatorRotation();
        }

        /// <summary>
        /// Factory method to compose the complete persistent transition overlay hierarchy at runtime or in edit mode.
        /// </summary>
        public static UISceneTransitionOverlay Create(Transform parent, UIDesignSystemConfig config = null)
        {
            var go = new GameObject("SceneTransitionOverlay", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(CanvasGroup));
            if (parent != null)
            {
                go.transform.SetParent(parent, false);
            }

            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 9999;

            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            var cg = go.GetComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;

            // Fullscreen Background
            var bgObj = new GameObject("CoverImage", typeof(RectTransform), typeof(Image));
            bgObj.transform.SetParent(go.transform, false);
            var bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            var bgImg = bgObj.GetComponent<Image>();
            bgImg.color = config != null ? config.GetColor(UISemanticColor.Background) : new Color(0.05f, 0.08f, 0.12f, 1f);
            bgImg.raycastTarget = true;

            // Centered Loading Indicator Container
            var indicatorContainerObj = new GameObject("LoadingIndicator", typeof(RectTransform), typeof(CanvasGroup));
            indicatorContainerObj.transform.SetParent(go.transform, false);
            var indicatorContainerRect = indicatorContainerObj.GetComponent<RectTransform>();
            indicatorContainerRect.anchorMin = new Vector2(0.5f, 0.5f);
            indicatorContainerRect.anchorMax = new Vector2(0.5f, 0.5f);
            indicatorContainerRect.pivot = new Vector2(0.5f, 0.5f);
            indicatorContainerRect.sizeDelta = new Vector2(48, 48);
            indicatorContainerRect.anchoredPosition = Vector2.zero;
            var indicatorCg = indicatorContainerObj.GetComponent<CanvasGroup>();
            indicatorCg.alpha = 0f;
            indicatorCg.interactable = false;
            indicatorCg.blocksRaycasts = false;

            // Restrained visual accent: Rotating Diamond
            var visualObj = new GameObject("Visual", typeof(RectTransform), typeof(Image));
            visualObj.transform.SetParent(indicatorContainerObj.transform, false);
            var visualRect = visualObj.GetComponent<RectTransform>();
            visualRect.anchorMin = new Vector2(0.5f, 0.5f);
            visualRect.anchorMax = new Vector2(0.5f, 0.5f);
            visualRect.pivot = new Vector2(0.5f, 0.5f);
            visualRect.sizeDelta = new Vector2(24, 24);
            visualRect.anchoredPosition = Vector2.zero;
            var visualImg = visualObj.GetComponent<Image>();
            visualImg.color = config != null ? config.GetColor(UISemanticColor.Primary) : new Color(0.89f, 0.74f, 0.47f, 1f); // #E4BD78 Gold
            visualImg.raycastTarget = false;

            var overlay = go.AddComponent<UISceneTransitionOverlay>();
            overlay.InitializeReferences(canvas, cg, bgImg, visualRect, indicatorCg, config);

            return overlay;
        }

        public void InitializeReferences(
            Canvas canvas, 
            CanvasGroup canvasGroup, 
            Image overlayImage, 
            RectTransform loadingIndicator, 
            CanvasGroup loadingIndicatorGroup, 
            UIDesignSystemConfig config)
        {
            _canvas = canvas;
            _canvasGroup = canvasGroup;
            _overlayImage = overlayImage;
            _loadingIndicator = loadingIndicator;
            _loadingIndicatorGroup = loadingIndicatorGroup;
            _config = config;
        }
    }
}
