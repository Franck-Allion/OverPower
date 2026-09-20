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
        [SerializeField] private UIPanelStyle _style;
        private void OnEnable() { if (_config != null) Apply(); }
        public void SetStyle(UIPanelStyle style) { _style = style; Apply(); }
        public void Apply()
        {
            _surface.color = _config.GetColor(_style == UIPanelStyle.Subtle ? UISemanticColor.Background : UISemanticColor.Surface);
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
        }
    }
}
