using UnityEngine;
using UnityEngine.EventSystems;

namespace OverPower.Unity.Presentation.DesignSystem.Preview
{
    /// <summary>Development scene composition only. Never included by a production screen.</summary>
    public sealed class DesignSystemPreviewView : MonoBehaviour
    {
        [SerializeField] private GameObject _foundations;
        [SerializeField] private GameObject _components;
        [SerializeField] private UIButton[] _buttonSamples;
        [SerializeField] private UIButton _componentFocus;
        [SerializeField] private UITooltip _tooltip;
        [SerializeField] private UITooltipTrigger[] _tooltipTriggers;
        [SerializeField] private UIConfirmDialog _dialog;
        [SerializeField] private UITransition _reveal;
        public UIButton[] ButtonSamples => _buttonSamples;
        public UITooltip Tooltip => _tooltip;
        public UITooltipTrigger[] TooltipTriggers => _tooltipTriggers;
        public UIConfirmDialog Dialog => _dialog;
        public UITransition Reveal => _reveal;

        public void ShowFoundations()
        {
            _components.SetActive(false);
            _foundations.SetActive(true);
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(_buttonSamples[0].gameObject);
        }
        public void ShowComponents()
        {
            _foundations.SetActive(false);
            _components.SetActive(true);
            if (EventSystem.current != null) EventSystem.current.SetSelectedGameObject(_componentFocus.gameObject);
        }
        public void ToggleReveal() { if (_reveal.IsVisible) _reveal.Hide(); else _reveal.Show(); }
    }
}
