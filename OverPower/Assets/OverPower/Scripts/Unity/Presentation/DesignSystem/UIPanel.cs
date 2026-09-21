using UnityEngine;
using UnityEngine.UI;

namespace OverPower.Unity.Presentation.DesignSystem
{
    public enum UIPanelStyle { Default, Elevated, Subtle, Floating, Modal }

    public sealed class UIPanel : MonoBehaviour
    {
        [SerializeField] private UIDesignSystemConfig _config;
        [SerializeField] private Image _surface;
        [SerializeField] private Image _border;
        [SerializeField] private Image _innerBorder;
        [SerializeField] private Image _accentBar;
        [SerializeField] private UIPanelStyle _style;
        private void OnEnable() { if (_config != null) Apply(); }
        public void SetStyle(UIPanelStyle style) { _style = style; Apply(); }
        public void Apply()
        {
            if (_config == null || _surface == null) return;

            // Surface background color (Subtle uses deep Background, others use Surface)
            _surface.color = _config.GetColor(_style == UIPanelStyle.Subtle ? UISemanticColor.Background : UISemanticColor.Surface);

            // Special background override for Tooltips (Floating) to make them visually darker/lighter in hierarchy
            if (_style == UIPanelStyle.Floating)
            {
                _surface.color = new Color32(10, 15, 24, 255); // Darker than standard panels
            }

            // 1. Outer Border
            if (_border != null)
            {
                UISemanticColor borderColor;
                switch (_style)
                {
                    case UIPanelStyle.Elevated:
                        borderColor = UISemanticColor.Primary;
                        break;
                    case UIPanelStyle.Floating:
                        borderColor = UISemanticColor.FloatingBorder;
                        break;
                    case UIPanelStyle.Modal:
                        borderColor = UISemanticColor.ModalBorder;
                        break;
                    default:
                        borderColor = UISemanticColor.Secondary;
                        break;
                }
                _border.color = _config.GetColor(borderColor);
                _border.enabled = _style != UIPanelStyle.Subtle;
            }

            // 2. Inner Highlight / Border
            if (_innerBorder != null)
            {
                _innerBorder.enabled = _style != UIPanelStyle.Subtle;
                if (_innerBorder.enabled)
                {
                    // Subtle highlights using transparent colors
                    if (_style == UIPanelStyle.Floating)
                    {
                        _innerBorder.color = new Color(0.89f, 0.74f, 0.47f, 0.03f); // Subtle gold tint
                    }
                    else if (_style == UIPanelStyle.Modal)
                    {
                        _innerBorder.color = new Color(0.89f, 0.74f, 0.47f, 0.05f); // Stronger gold highlight
                    }
                    else
                    {
                        _innerBorder.color = new Color(1f, 1f, 1f, 0.03f); // Faint white highlights
                    }
                }
            }

            // 3. Top Accent Bar (for Elevated and Modal panels)
            if (_accentBar != null)
            {
                _accentBar.enabled = _style == UIPanelStyle.Elevated || _style == UIPanelStyle.Modal;
                if (_accentBar.enabled)
                {
                    _accentBar.color = _config.GetColor(UISemanticColor.Primary); // Pure Gold
                }
            }
        }
    }
}
