using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverPower.Unity.Presentation.DesignSystem
{
    public sealed class UITooltip : MonoBehaviour
    {
        [SerializeField] private UIDesignSystemConfig _config;
        [SerializeField] private UITransition _transition;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _body;
        [SerializeField] private Image _icon;
        [SerializeField] private RectTransform _bounds;
        [SerializeField] private Canvas _canvas;
        private object _owner;
        private Vector2 _screenPoint;
        private bool _positioning;
        public bool IsVisible => _transition.IsVisible;
        public RectTransform Bounds => _bounds;
        public void BindCanvas(RectTransform bounds, Canvas canvas)
        {
            if (bounds == null || canvas == null || transform.parent != bounds)
                throw new System.ArgumentException("Tooltip must be a direct child of its canvas bounds.");
            _bounds = bounds;
            _canvas = canvas;
        }

        public void Show(object owner, string title, string body, Sprite icon, Vector2 screenPoint)
        {
            if (!isActiveAndEnabled) return;
            if (_bounds == null || _canvas == null) throw new System.InvalidOperationException("Bind tooltip canvas before showing.");

            if (_config != null && _config.InlineIconSpriteAsset != null)
            {
                if (_title != null) _title.spriteAsset = _config.InlineIconSpriteAsset;
                if (_body != null) _body.spriteAsset = _config.InlineIconSpriteAsset;
            }

            _owner = owner;
            _title.text = title;
            _body.text = body;
            _icon.sprite = icon;
            _icon.gameObject.SetActive(icon != null);
            _screenPoint = screenPoint;
            PositionInsideCanvas();
            _transition.Show();
        }
        public void Hide(object owner)
        {
            if (!ReferenceEquals(owner, _owner)) return;
            _owner = null;
            _transition.Hide();
        }

        private void PositionInsideCanvas()
        {
            if (_positioning || _bounds == null || _canvas == null) return;
            _positioning = true;
            try
            {
                var rect = (RectTransform)transform;
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
                var camera = _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_bounds, _screenPoint, camera, out var point);
                var area = _bounds.rect;
                float width = rect.rect.width, height = rect.rect.height;
                point += new Vector2(UISpacing.Lg, -UISpacing.Lg);
                // The prefab uses top-left pivot and center anchors, directly under the supplied bounds.
                float x = Mathf.Clamp(point.x, area.xMin + UISpacing.Sm, Mathf.Max(area.xMin + UISpacing.Sm, area.xMax - width - UISpacing.Sm));
                float y = Mathf.Clamp(point.y, Mathf.Min(area.yMax - UISpacing.Sm, area.yMin + height + UISpacing.Sm), area.yMax - UISpacing.Sm);
                rect.anchoredPosition = new Vector2(x, y);
            }
            finally { _positioning = false; }
        }
        private void OnRectTransformDimensionsChange() { if (_owner != null) PositionInsideCanvas(); }
        private void OnDisable() { _owner = null; if (_transition != null) _transition.HideImmediate(); }
    }
}
