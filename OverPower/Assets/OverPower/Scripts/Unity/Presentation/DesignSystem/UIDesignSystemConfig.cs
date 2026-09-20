using System;
using TMPro;
using UnityEngine;

namespace OverPower.Unity.Presentation.DesignSystem
{
    [CreateAssetMenu(menuName = "OverPower/UI/Design System")]
    public sealed class UIDesignSystemConfig : ScriptableObject
    {
        [Serializable]
        public struct TypographyDefinition
        {
            public TypographyStyle Role;
            public float Size;
            public FontStyles Style;
            public TypographyDefinition(TypographyStyle role, float size, FontStyles style = FontStyles.Normal)
            { Role = role; Size = size; Style = style; }
        }

        [Serializable]
        public struct PaletteEntry
        {
            public UISemanticColor Role;
            public Color Value;
            public PaletteEntry(UISemanticColor role, uint rgb)
            {
                Role = role;
                Value = new Color32((byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb, 255);
            }
        }

        [SerializeField] private TMP_FontAsset _font;
        [SerializeField] private TypographyDefinition[] _typography =
        {
            new TypographyDefinition(TypographyStyle.Display, 64, FontStyles.Bold),
            new TypographyDefinition(TypographyStyle.Title, 42, FontStyles.Bold),
            new TypographyDefinition(TypographyStyle.Heading, 28, FontStyles.Bold),
            new TypographyDefinition(TypographyStyle.Body, 24),
            new TypographyDefinition(TypographyStyle.BodySmall, 20),
            new TypographyDefinition(TypographyStyle.Caption, 18),
            new TypographyDefinition(TypographyStyle.Stat, 32, FontStyles.Bold),
            new TypographyDefinition(TypographyStyle.Button, 24, FontStyles.Bold)
        };
        [SerializeField] private PaletteEntry[] _palette =
        {
            new PaletteEntry(UISemanticColor.Primary, 0xE4BD78),
            new PaletteEntry(UISemanticColor.Secondary, 0x708B9B),
            new PaletteEntry(UISemanticColor.Interactive, 0xFFE0A3),
            new PaletteEntry(UISemanticColor.Selected, 0xD8EFF4),
            new PaletteEntry(UISemanticColor.Disabled, 0x53616C),
            new PaletteEntry(UISemanticColor.Danger, 0xF08585),
            new PaletteEntry(UISemanticColor.Warning, 0xF0C47F),
            new PaletteEntry(UISemanticColor.Success, 0x91D5B5),
            new PaletteEntry(UISemanticColor.Health, 0xE98991),
            new PaletteEntry(UISemanticColor.Mana, 0x88BBF0),
            new PaletteEntry(UISemanticColor.Armor, 0xBBCAD8),
            new PaletteEntry(UISemanticColor.Gold, 0xE4BD78),
            new PaletteEntry(UISemanticColor.Locked, 0x89929D),
            new PaletteEntry(UISemanticColor.Affordable, 0x91D5B5),
            new PaletteEntry(UISemanticColor.Unaffordable, 0xF08585),
            new PaletteEntry(UISemanticColor.Background, 0x0D141F),
            new PaletteEntry(UISemanticColor.Surface, 0x1B2A3A),
            new PaletteEntry(UISemanticColor.TextPrimary, 0xF5EFE3),
            new PaletteEntry(UISemanticColor.TextSecondary, 0xB5C4D0)
        };
        [SerializeField, Min(0.01f)] private float _fast = 0.12f;
        [SerializeField, Min(0.01f)] private float _normal = 0.20f;
        [SerializeField, Min(0.01f)] private float _emphasis = 0.35f;
        [SerializeField, Range(1f, 1.05f)] private float _hoverScale = 1.015f;
        [SerializeField, Range(0.9f, 1f)] private float _pressedScale = 0.975f;
        [SerializeField, Range(0.5f, 1f)] private float _disabledOpacity = 0.7f;

        public float HoverScale => _hoverScale;
        public float PressedScale => _pressedScale;
        public float DisabledOpacity => _disabledOpacity;

        public Color GetColor(UISemanticColor role)
        {
            foreach (var entry in _palette) if (entry.Role == role) return entry.Value;
            throw new ArgumentOutOfRangeException(nameof(role), role, "Missing semantic color.");
        }

        public float GetDuration(UIMotion motion)
        {
            switch (motion)
            {
                case UIMotion.Fast: return _fast;
                case UIMotion.Normal: return _normal;
                case UIMotion.Emphasis: return _emphasis;
                default: throw new ArgumentOutOfRangeException(nameof(motion));
            }
        }

        public void ApplyTypography(TMP_Text text, TypographyStyle role)
        {
            if (_font == null) throw new InvalidOperationException("Design system font is required.");
            foreach (var definition in _typography)
            {
                if (definition.Role != role) continue;
                text.font = _font;
                text.fontSize = definition.Size;
                text.fontStyle = definition.Style;
                text.enableAutoSizing = false;
                return;
            }
            throw new ArgumentOutOfRangeException(nameof(role), role, "Missing typography role.");
        }
    }
}
