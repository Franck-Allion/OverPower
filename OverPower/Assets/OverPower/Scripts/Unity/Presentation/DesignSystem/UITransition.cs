using System;
using DG.Tweening;
using UnityEngine;

namespace OverPower.Unity.Presentation.DesignSystem
{
    [RequireComponent(typeof(CanvasGroup))]
    public sealed class UITransition : MonoBehaviour
    {
        [SerializeField] private UIDesignSystemConfig _config;
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private RectTransform _visual;
        [SerializeField] private bool _initiallyVisible;
        [SerializeField] private bool _receivesInput = true;
        private Tween _tween;
        public bool IsVisible { get; private set; }
        public bool IsAnimating => _tween != null && _tween.IsActive();
        public event Action Hidden;

        private void Awake()
        {
            if (_group == null) _group = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            if (_group == null) _group = GetComponent<CanvasGroup>();
            if (_config != null) SetVisible(_initiallyVisible, true);
        }
        public void Show() => SetVisible(true, false);
        public void Hide() => SetVisible(false, false);
        public void ShowImmediate() => SetVisible(true, true);
        public void HideImmediate() => SetVisible(false, true);

        private void SetVisible(bool visible, bool immediate)
        {
            _tween?.Kill();
            _tween = null;
            IsVisible = visible;
            if (_group == null) _group = GetComponent<CanvasGroup>();
            if (_group != null)
            {
                _group.interactable = visible && _receivesInput;
                _group.blocksRaycasts = visible && _receivesInput;
            }
            float destination = visible ? 1f : 0f;
            if (immediate || !isActiveAndEnabled || !UnityEngine.Application.isPlaying || _config == null)
            {
                Apply(destination);
                if (!visible) Hidden?.Invoke();
                return;
            }
            _tween = DOTween.To(() => _group != null ? _group.alpha : 0f, Apply, destination,
                    _config.GetDuration(visible ? UIMotion.Normal : UIMotion.Fast))
                .SetEase(Ease.OutCubic).SetUpdate(true).SetRecyclable(false)
                .OnComplete(() => { _tween = null; if (!visible) Hidden?.Invoke(); });
        }

        private void Apply(float alpha)
        {
            if (_group != null) _group.alpha = alpha;
            if (_visual != null)
            {
                float revealScale = _config != null ? _config.RevealScale : 0.98f;
                _visual.localScale = Vector3.one * Mathf.Lerp(revealScale, 1f, alpha);
            }
        }
        private void OnDisable()
        {
            _tween?.Kill();
            _tween = null;
            IsVisible = false;
            if (_group != null) { _group.alpha = 0; _group.interactable = false; _group.blocksRaycasts = false; }
            if (_visual != null) _visual.localScale = Vector3.one;
            Hidden?.Invoke();
        }
        private void OnDestroy() { _tween?.Kill(); _tween = null; }
    }
}
