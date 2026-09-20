using TMPro;
using UnityEngine;

namespace OverPower.Unity.Presentation.DesignSystem
{
    /// <summary>
    /// Reusable UI component wrapping a TextMeshPro text component to automatically format 
    /// custom semantic inline icon tokens and inject the design system's sprite asset.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public sealed class UIRichText : MonoBehaviour
    {
        [SerializeField] private UIDesignSystemConfig _config;
        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            if (_text == null)
            {
                _text = GetComponent<TMP_Text>();
            }

            if (_config != null && _config.InlineIconSpriteAsset != null && _text != null)
            {
                _text.spriteAsset = _config.InlineIconSpriteAsset;
            }
        }

        /// <summary>
        /// Sets the raw text, formats it to replace inline icon tokens, and updates the text display.
        /// </summary>
        public void SetText(string rawText)
        {
            if (_text == null)
            {
                _text = GetComponent<TMP_Text>();
            }

            if (_config != null && _config.InlineIconSpriteAsset != null && _text != null)
            {
                _text.spriteAsset = _config.InlineIconSpriteAsset;
            }

            if (_text != null)
            {
                _text.text = UIRichTextFormatter.Format(rawText);
            }
        }
    }
}
