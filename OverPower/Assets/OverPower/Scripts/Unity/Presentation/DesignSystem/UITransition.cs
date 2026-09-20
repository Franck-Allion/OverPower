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

        private void OnEnable()
        {
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
            _group.interactable = visible && _receivesInput;
            _group.blocksRaycasts = visible && _receivesInput;
            float destination = visible ? 1f : 0f;
            if (immediate || !isActiveAndEnabled || !UnityEngine.Application.isPlaying)
            {
                Apply(destination);
                if (!visible) Hidden?.Invoke();
                return;
            }
            _tween = DOTween.To(() => _group.alpha, Apply, destination,
                    _config.GetDuration(visible ? UIMotion.Normal : UIMotion.Fast))
                .SetEase(Ease.OutCubic).SetUpdate(true).SetRecyclable(false)
                .OnComplete(() => { _tween = null; if (!visible) Hidden?.Invoke(); });
        }

        private void Apply(float alpha)
        {
            _group.alpha = alpha;
            if (_visual != null) _visual.localScale = Vector3.one * Mathf.Lerp(_config.RevealScale, 1f, alpha);
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
