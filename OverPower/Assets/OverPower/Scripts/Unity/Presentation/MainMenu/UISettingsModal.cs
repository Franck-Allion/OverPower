using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using OverPower.Unity.Bootstrap;
using OverPower.Unity.Presentation.DesignSystem;

namespace OverPower.Unity.Presentation.MainMenu
{
    /// <summary>
    /// Minimal real settings modal providing language selection and clean dismissal.
    /// Reuses UITransition and Design System components with full keyboard/controller navigation.
    /// </summary>
    public sealed class UISettingsModal : MonoBehaviour, ICancelHandler
    {
        [Header("Design System")]
        [SerializeField] private UIDesignSystemConfig _config;
        [SerializeField] private UITransition _transition;
        [SerializeField] private Image _backdrop;
        [SerializeField] private CanvasGroup _background;

        [Header("Modal UI Elements")]
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private UIButton _languageButton;
        [SerializeField] private TMP_Text _languageButtonLabel;
        [SerializeField] private UIButton _closeButton;

        [Header("Events")]
        [SerializeField] private UnityEvent _closed = new UnityEvent();

        private GameObject _previousFocus;
        private bool _ownsBackground;
        private bool _previousInteractable;
        private bool _previousRaycasts;

        public bool IsOpen { get; private set; }
        public UIButton LanguageButton => _languageButton;
        public UIButton CloseButton => _closeButton;
        public CanvasGroup Background => _background;
        public UnityEvent Closed => _closed;

        public void BindBackground(CanvasGroup background)
        {
            if (background == null || transform.IsChildOf(background.transform))
                throw new ArgumentException("The modal needs a separate background group.", nameof(background));
            if (_ownsBackground)
                throw new InvalidOperationException("Close the modal before rebinding its background.");
            _background = background;
        }

        private void OnEnable()
        {
            InitializeListeners();
            UpdateLanguageButtonLabel();
        }

        public void InitializeListeners()
        {
            if (_config != null && _backdrop != null)
            {
                var color = _config.GetColor(UISemanticColor.Background);
                color.a = _config.ModalBackdropOpacity;
                _backdrop.color = color;
            }

            if (_languageButton != null)
            {
                _languageButton.onClick.RemoveListener(OnLanguageClicked);
                _languageButton.onClick.AddListener(OnLanguageClicked);
            }

            if (_closeButton != null)
            {
                _closeButton.onClick.RemoveListener(Close);
                _closeButton.onClick.AddListener(Close);
            }

            if (_transition != null)
            {
                _transition.Hidden -= OnTransitionHidden;
                _transition.Hidden += OnTransitionHidden;
            }
        }

        private void OnDisable()
        {
            if (_languageButton != null)
            {
                _languageButton.onClick.RemoveListener(OnLanguageClicked);
            }

            if (_closeButton != null)
            {
                _closeButton.onClick.RemoveListener(Close);
            }

            if (_transition != null)
            {
                _transition.Hidden -= OnTransitionHidden;
                _transition.HideImmediate();
            }

            RestoreBackground();
        }

        public void Open()
        {
            InitializeListeners();
            if (_background == null)
                throw new InvalidOperationException("Bind the modal background before opening.");

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

            UpdateLanguageButtonLabel();

            if (_transition != null)
            {
                if (UnityEngine.Application.isPlaying)
                {
                    _transition.Show();
                }
                else
                {
                    _transition.ShowImmediate();
                }
            }

            if (eventSystem != null)
            {
                var target = _languageButton != null ? _languageButton.gameObject : (_closeButton != null ? _closeButton.gameObject : null);
                if (target != null)
                {
                    eventSystem.SetSelectedGameObject(target);
                }
            }
        }

        public void Close()
        {
            if (!IsOpen) return;
            IsOpen = false;
            if (_transition != null && UnityEngine.Application.isPlaying)
            {
                _transition.Hide();
            }
            else
            {
                if (_transition != null)
                {
                    _transition.HideImmediate();
                }
                OnTransitionHidden();
            }
            _closed.Invoke();
        }

        public void CloseImmediate()
        {
            bool wasOpen = IsOpen;
            IsOpen = false;
            if (_transition != null)
            {
                _transition.HideImmediate();
            }
            OnTransitionHidden();
            if (wasOpen)
            {
                _closed.Invoke();
            }
        }

        public void OnCancel(BaseEventData eventData)
        {
            if (IsOpen)
            {
                Close();
                eventData.Use();
            }
        }

        private void OnTransitionHidden()
        {
            IsOpen = false;
            RestoreBackground();
        }

        private void RestoreBackground()
        {
            if (!_ownsBackground) return;
            _ownsBackground = false;

            if (_background != null)
            {
                _background.interactable = _previousInteractable;
                _background.blocksRaycasts = _previousRaycasts;
            }

            var eventSystem = EventSystem.current != null ? EventSystem.current : FindFirstObjectByType<EventSystem>();
            if (_previousFocus != null && _previousFocus.activeInHierarchy && eventSystem != null)
            {
                var selectable = _previousFocus.GetComponent<Selectable>();
                if (selectable == null || selectable.IsInteractable())
                {
                    eventSystem.SetSelectedGameObject(_previousFocus);
                }
            }
            _previousFocus = null;
        }

        private async void OnLanguageClicked()
        {
            var localeService = GameBootstrap.IsInitialized
                ? GameBootstrap.GetLocaleService()
                : new OverPower.Unity.Localization.UnityLocaleService();

            if (localeService != null)
            {
                string currentCode = localeService.CurrentLocale != null ? localeService.CurrentLocale.Identifier.Code : "en";
                string nextLocale = currentCode == "fr" ? "en" : "fr";
                if (_languageButton != null) _languageButton.interactable = false;

                try
                {
                    await localeService.SetLocaleAsync(nextLocale);
                    UpdateLanguageButtonLabel();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"[UISettingsModal] Error switching language: {ex.Message}");
                }
                finally
                {
                    if (_languageButton != null) _languageButton.interactable = true;
                }
            }
        }

        public void UpdateLanguageButtonLabel()
        {
            if (_languageButtonLabel == null) return;
            var localeService = GameBootstrap.IsInitialized
                ? GameBootstrap.GetLocaleService()
                : new OverPower.Unity.Localization.UnityLocaleService();
            string currentCode = localeService?.CurrentLocale != null ? localeService.CurrentLocale.Identifier.Code : "en";
            _languageButtonLabel.text = currentCode == "fr" ? "Français" : "English";
        }
    }
}
