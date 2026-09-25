using UnityEngine;
using OverPower.Unity.Presentation.DesignSystem;

namespace OverPower.Unity.Presentation.Audio
{
    /// <summary>
    /// Central audio feedback coordinator mapping semantic UI interaction events to audio playback.
    /// Operates through a dedicated 2D AudioSource and safely ignores unassigned AudioClip slots.
    /// </summary>
    [AddComponentMenu("OverPower/UI/Audio Feedback")]
    public sealed class UIAudioFeedback : MonoBehaviour
    {
        [Header("Audio Output")]
        [SerializeField] private AudioSource _audioSource;

        [Header("Semantic Feedback Clips")]
        [SerializeField] private AudioClip _hoverClip;
        [SerializeField] private AudioClip _confirmClip;
        [SerializeField] private AudioClip _toggleClip;
        [SerializeField] private AudioClip _cancelClip;

        [Header("Debounce Settings")]
        [SerializeField] private float _hoverDebounceSeconds = 0.04f;

        private static UIAudioFeedback _instance;
        private bool _isSubscribed;
        private float _lastHoverTime = -1f;

        public static UIAudioFeedback Instance => _instance;

        public AudioSource AudioSource
        {
            get => _audioSource;
            set => _audioSource = value;
        }

        public AudioClip HoverClip
        {
            get => _hoverClip;
            set => _hoverClip = value;
        }

        public AudioClip ConfirmClip
        {
            get => _confirmClip;
            set => _confirmClip = value;
        }

        public AudioClip ToggleClip
        {
            get => _toggleClip;
            set => _toggleClip = value;
        }

        public AudioClip CancelClip
        {
            get => _cancelClip;
            set => _cancelClip = value;
        }

        public bool IsSubscribed => _isSubscribed;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            if (transform.parent == null && UnityEngine.Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
            Unsubscribe();
        }

        public void Subscribe()
        {
            if (_isSubscribed) return;

            UIButton.OnButtonFeedback += HandleButtonFeedback;
            UICancelRelay.OnCancelFeedback += HandleCancelFeedback;
            _isSubscribed = true;
        }

        public void Unsubscribe()
        {
            if (!_isSubscribed) return;

            UIButton.OnButtonFeedback -= HandleButtonFeedback;
            UICancelRelay.OnCancelFeedback -= HandleCancelFeedback;
            _isSubscribed = false;
        }

        public void PlayConfirm()
        {
            if (_audioSource != null && _confirmClip != null)
            {
                _audioSource.PlayOneShot(_confirmClip);
            }
        }

        public void PlayToggle()
        {
            var clip = _toggleClip != null ? _toggleClip : _confirmClip;
            if (_audioSource != null && clip != null)
            {
                _audioSource.PlayOneShot(clip);
            }
        }

        public void PlayHover()
        {
            if (UnityEngine.Application.isPlaying && Time.unscaledTime - _lastHoverTime < _hoverDebounceSeconds)
            {
                return;
            }

            _lastHoverTime = UnityEngine.Application.isPlaying ? Time.unscaledTime : 0f;

            if (_audioSource != null && _hoverClip != null)
            {
                _audioSource.PlayOneShot(_hoverClip);
            }
        }

        public void PlayCancel()
        {
            if (_audioSource != null && _cancelClip != null)
            {
                _audioSource.PlayOneShot(_cancelClip);
            }
        }

        private void HandleButtonFeedback(UIButton button, UIInteractionType type)
        {
            if (button != null && !button.interactable)
            {
                return;
            }

            if (button != null && button.OverrideSubmitClip != null && (type == UIInteractionType.Submit || type == UIInteractionType.Toggle))
            {
                if (_audioSource != null)
                {
                    _audioSource.PlayOneShot(button.OverrideSubmitClip);
                }
                return;
            }

            switch (type)
            {
                case UIInteractionType.Submit:
                    PlayConfirm();
                    break;

                case UIInteractionType.Toggle:
                    PlayToggle();
                    break;

                case UIInteractionType.Hover:
                case UIInteractionType.Focus:
                    PlayHover();
                    break;

                case UIInteractionType.Cancel:
                case UIInteractionType.Close:
                    PlayCancel();
                    break;

                case UIInteractionType.Press:
                    // Press is reserved for tactile visual down-state; confirmation sound fires on submit/click.
                    break;
            }
        }

        private void HandleCancelFeedback(UICancelRelay relay)
        {
            PlayCancel();
        }
    }
}
