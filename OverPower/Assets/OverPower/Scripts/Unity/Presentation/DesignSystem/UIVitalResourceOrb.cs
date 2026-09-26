using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverPower.Unity.Presentation.DesignSystem
{
    /// <summary>
    /// Reusable circular vital resource orb representing Health or Mana with liquid wave fill,
    /// unchanged metallic frame, exact numeric values, and safe mathematical edge-case handling.
    /// </summary>
    [ExecuteAlways]
    [AddComponentMenu("OverPower/UI/Vital Resource Orb")]
    public sealed class UIVitalResourceOrb : MonoBehaviour
    {
        [Header("Design System")]
        [SerializeField] private UIDesignSystemConfig _config;
        [SerializeField] private UISemanticColor _tone = UISemanticColor.Health;

        [Header("Hierarchy References")]
        [SerializeField] private Image _liquidImage;
        [SerializeField] private Image _frameImage;
        [SerializeField] private TMP_Text _valueLabel;

        [Header("Runtime State")]
        [SerializeField] private int _current = 100;
        [SerializeField] private int _maximum = 100;

        private Material _materialInstance;
        private float _normalizedFill = 1f;

        public int Current => _current;
        public int Maximum => _maximum;
        public float NormalizedFill => _normalizedFill;
        public UISemanticColor Tone => _tone;
        public Image LiquidImage => _liquidImage;
        public Image FrameImage => _frameImage;
        public TMP_Text ValueLabel => _valueLabel;
        public Material MaterialInstance => _materialInstance;

        private void Awake()
        {
            EnsureMaterialInstance();
            ApplyTypography();
            ApplyTone();
            SetValue(_current, _maximum);
        }

        private void OnEnable()
        {
            EnsureMaterialInstance();
            ApplyTypography();
            ApplyTone();
            SetValue(_current, _maximum);
        }

        private void OnDestroy()
        {
            if (_materialInstance != null)
            {
                if (UnityEngine.Application.isPlaying)
                {
                    Destroy(_materialInstance);
                }
                else
                {
                    DestroyImmediate(_materialInstance);
                }
                _materialInstance = null;
            }
        }

        public void SetResource(int current, int maximum, UISemanticColor tone)
        {
            _tone = tone;
            ApplyTone();
            SetValue(current, maximum);
        }

        public void SetTone(UISemanticColor tone)
        {
            _tone = tone;
            ApplyTone();
        }

        public void SetValue(int current, int maximum)
        {
            _current = current;
            _maximum = maximum;

            // Safe normalized fill: handles 0 maximum, negative numbers, without NaN or division-by-zero
            if (_maximum <= 0)
            {
                _normalizedFill = 0f;
            }
            else
            {
                _normalizedFill = Mathf.Clamp01((float)_current / _maximum);
            }

            EnsureMaterialInstance();

            if (_materialInstance != null && _materialInstance.HasProperty("_Value"))
            {
                _materialInstance.SetFloat("_Value", _normalizedFill);
            }
            else if (_liquidImage != null && _liquidImage.type == Image.Type.Filled)
            {
                _liquidImage.fillAmount = _normalizedFill;
            }

            RefreshText();
        }

        private void EnsureMaterialInstance()
        {
            if (_materialInstance == null && _liquidImage != null && _liquidImage.material != null)
            {
                // Instantiate dedicated material copy to prevent Health and Mana orbs from modifying each other
                _materialInstance = new Material(_liquidImage.material);
                _materialInstance.name = $"{_liquidImage.material.name}_{_tone}_Instance";
                _liquidImage.material = _materialInstance;
            }
        }

        private void ApplyTone()
        {
            if (_liquidImage != null)
            {
                if (_tone == UISemanticColor.Health)
                {
                    // Health uses the original crimson/red texture color
                    _liquidImage.color = Color.white;
                }
                else if (_config != null)
                {
                    // Mana or other tones use the semantic palette color mapped onto liquid luminance
                    _liquidImage.color = _config.GetColor(_tone);
                }
            }

            // Invariant: The metallic frame MUST remain neutral and untinted
            if (_frameImage != null)
            {
                _frameImage.color = Color.white;
            }
        }

        private void ApplyTypography()
        {
            if (_config != null && _valueLabel != null)
            {
                _config.ApplyTypography(_valueLabel, TypographyStyle.Stat);
                _valueLabel.color = _config.GetColor(UISemanticColor.TextPrimary);
            }
        }

        private void RefreshText()
        {
            if (_valueLabel != null)
            {
                _valueLabel.text = $"{_current.ToString(CultureInfo.InvariantCulture)} / {_maximum.ToString(CultureInfo.InvariantCulture)}";
            }
        }

        public void InitializeReferences(
            UIDesignSystemConfig config,
            Image liquidImage,
            Image frameImage,
            TMP_Text valueLabel,
            UISemanticColor tone)
        {
            _config = config;
            _liquidImage = liquidImage;
            _frameImage = frameImage;
            _valueLabel = valueLabel;
            _tone = tone;

            EnsureMaterialInstance();
            ApplyTypography();
            ApplyTone();
            SetValue(_current, _maximum);
        }
    }
}
