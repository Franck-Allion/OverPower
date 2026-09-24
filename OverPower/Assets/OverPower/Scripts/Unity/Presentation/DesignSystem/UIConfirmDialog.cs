using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OverPower.Unity.Presentation.DesignSystem
{
    public sealed class UIConfirmDialog : MonoBehaviour
    {
        [SerializeField] private UIDesignSystemConfig _config;
        [SerializeField] private UITransition _transition;
        [SerializeField] private Image _backdrop;
        [SerializeField] private CanvasGroup _background;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _body;
        [SerializeField] private UIButton _primary;
        [SerializeField] private UIButton _secondary;
        [SerializeField] private UnityEvent _confirmed = new UnityEvent();
        [SerializeField] private UnityEvent _cancelled = new UnityEvent();
        private GameObject _previousFocus;
        private bool _ownsBackground, _previousInteractable, _previousRaycasts;
        public bool IsOpen { get; private set; }
        public UIButton Primary => _primary;
        public UIButton Secondary => _secondary;
        public CanvasGroup Background => _background;
        public UnityEvent Confirmed => _confirmed;
        public UnityEvent Cancelled => _cancelled;
        public void BindBackground(CanvasGroup background)
        {
            if (background == null || transform.IsChildOf(background.transform))
                throw new System.ArgumentException("The modal needs a separate background group.", nameof(background));
            if (_ownsBackground) throw new System.InvalidOperationException("Close the modal before rebinding its background.");
            _background = background;
        }

        private void OnEnable()
        {
            if (_config == null) return;
            var color = _config.GetColor(UISemanticColor.Background);
            color.a = _config.ModalBackdropOpacity;
            _backdrop.color = color;
            _primary.onClick.AddListener(Confirm);
            _secondary.onClick.AddListener(Cancel);
            _transition.Hidden += RestoreBackground;
        }
        public void SetTitle(string text) => _title.text = text;
        public void SetBody(string text) => _body.text = text;
        public void Open()
        {
            if (!isActiveAndEnabled) return;
            if (_background == null) throw new System.InvalidOperationException("Bind the modal background before opening.");
            var eventSystem = EventSystem.current != null ? EventSystem.current : FindFirstObjectByType<EventSystem>();
            if (!_ownsBackground)
            {
                _previousFocus = eventSystem != null ? eventSystem.currentSelectedGameObject : null;
                _previousInteractable = _background.interactable;
                _previousRaycasts = _background.blocksRaycasts;
                _ownsBackground = true;
            }
            _background.interactable = false;
            _background.blocksRaycasts = false;
            IsOpen = true;
            if (UnityEngine.Application.isPlaying)
            {
                _transition.Show();
            }
            else
            {
                _transition.ShowImmediate();
            }
            if (eventSystem != null) eventSystem.SetSelectedGameObject(_primary.gameObject);
        }
        public void Close() { IsOpen = false; _transition.Hide(); }
        public void Cancel() { if (!IsOpen) return; Close(); _cancelled.Invoke(); }
        public void Confirm() { if (!IsOpen) return; Close(); _confirmed.Invoke(); }

        private void RestoreBackground()
        {
            IsOpen = false;
            if (!_ownsBackground) return;
            _ownsBackground = false;
            if (_background != null) { _background.interactable = _previousInteractable; _background.blocksRaycasts = _previousRaycasts; }
            var eventSystem = EventSystem.current != null ? EventSystem.current : FindFirstObjectByType<EventSystem>();
            if (_previousFocus != null && _previousFocus.activeInHierarchy && eventSystem != null)
            {
                var selectable = _previousFocus.GetComponent<Selectable>();
                if (selectable == null || selectable.IsInteractable()) eventSystem.SetSelectedGameObject(_previousFocus);
            }
            _previousFocus = null;
        }
        private void OnDisable()
        {
            if (_primary != null) _primary.onClick.RemoveListener(Confirm);
            if (_secondary != null) _secondary.onClick.RemoveListener(Cancel);
            if (_transition != null) { _transition.Hidden -= RestoreBackground; _transition.HideImmediate(); }
            RestoreBackground();
        }
    }
}
