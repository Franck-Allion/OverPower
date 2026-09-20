using UnityEngine;
using UnityEngine.UI;

namespace OverPower.Unity.Presentation.DesignSystem
{
    public enum UIPanelStyle { Default, Elevated, Subtle }

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
                _border.color = _config.GetColor(_style == UIPanelStyle.Elevated ? UISemanticColor.Primary : UISemanticColor.Secondary);
                _border.enabled = _style != UIPanelStyle.Subtle;
            }
        }
    }
}
