using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverPower.Unity.Presentation.DesignSystem
{
    public enum CardVisualType { Unit, Spell }
    public enum CardRarityVisual { None, Common, Rare, Epic, Legendary }
    public enum CardPresentationState { Normal, Highlighted, Selected, Disabled }

    /// <summary>Presentation-only data for a reusable gameplay card frame.</summary>
    public sealed class CardPresentationData
    {
        public string Title;
        public string Description;
        public Sprite Artwork;
        public CardVisualType Type;
        public CardRarityVisual Rarity;
        public int? Cost;
        public int? Attack;
        public int? Armor;
        public int? Health;
    }

    /// <summary>Reusable visual card frame. It deliberately knows no card, cost, or targeting rules.</summary>
    public sealed class UICardFrame : MonoBehaviour
    {
        [SerializeField] private UIDesignSystemConfig _config;
        [SerializeField] private Image _surface;
        [SerializeField] private Image _accent;
        [SerializeField] private Image _artwork;
        [SerializeField] private GameObject _artworkPlaceholder;
        [SerializeField] private Image _artworkFrame;
        [SerializeField] private Image _familyIcon;
        [SerializeField] private TMP_Text _typeLabel;
        [SerializeField] private TMP_Text _costLabel;
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private UIRichText _description;
        [SerializeField] private GameObject _rarityMedallion;
        [SerializeField] private Image _rarityGem;
        [SerializeField] private TMP_Text _rarityLabel;
        [SerializeField] private GameObject _stats;
        [SerializeField] private TMP_Text _attackLabel;
        [SerializeField] private TMP_Text _armorLabel;
        [SerializeField] private TMP_Text _healthLabel;
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Image _stateOutline;
        private CardPresentationState _state;

        public CardPresentationState State => _state;
        public CardVisualType Type { get; private set; }
        public CardRarityVisual Rarity { get; private set; }
        public bool HasArtwork => _artwork != null && _artwork.sprite != null;

        private void Awake() { ApplyState(); }
        private void OnEnable() { ApplyState(); }

        public void SetPresentation(CardPresentationData data)
        {
            if (data == null) throw new System.ArgumentNullException(nameof(data));
            Type = data.Type;
            Rarity = data.Rarity;
            SetText(_titleLabel, data.Title);
            if (_description != null) _description.SetText(data.Description ?? string.Empty);
            SetOptionalText(_costLabel, data.Cost);
            SetOptionalText(_attackLabel, data.Attack);
            SetOptionalText(_armorLabel, data.Armor);
            SetOptionalText(_healthLabel, data.Health);
            if (_artwork != null)
            {
                _artwork.sprite = data.Artwork;
                _artwork.gameObject.SetActive(data.Artwork != null);
            }
            if (_artworkPlaceholder != null) _artworkPlaceholder.SetActive(data.Artwork == null);
            if (_typeLabel != null) _typeLabel.text = data.Type == CardVisualType.Unit ? "UNIT" : "SPELL";
            if (_stats != null) _stats.SetActive(data.Type == CardVisualType.Unit);
            var typeTone = data.Type == CardVisualType.Unit ? UISemanticColor.Interactive : UISemanticColor.Mana;
            if (_accent != null && _config != null) _accent.color = _config.GetColor(typeTone);
            if (_familyIcon != null && _config != null) _familyIcon.color = _config.GetColor(typeTone);
            if (_artworkFrame != null && _config != null) _artworkFrame.color = WithAlpha(_config.GetColor(typeTone), 0.78f);
            ApplyRarity(data.Rarity);
            ApplyTypography();
            ApplyState();
        }

        public void SetState(CardPresentationState state) { _state = state; ApplyState(); }

        private void ApplyTypography()
        {
            if (_config == null) return;
            Apply(_titleLabel, TypographyStyle.Heading, UISemanticColor.TextPrimary);
            Apply(_typeLabel, TypographyStyle.Caption, UISemanticColor.TextSecondary);
            Apply(_costLabel, TypographyStyle.Caption, UISemanticColor.Gold);
            Apply(_rarityLabel, TypographyStyle.Caption, UISemanticColor.TextPrimary);
            Apply(_attackLabel, TypographyStyle.Stat, UISemanticColor.Danger);
            Apply(_armorLabel, TypographyStyle.Stat, UISemanticColor.Armor);
            Apply(_healthLabel, TypographyStyle.Stat, UISemanticColor.Health);
        }

        private void ApplyState()
        {
            if (_group == null) _group = GetComponent<CanvasGroup>();
            if (_group != null) _group.alpha = _state == CardPresentationState.Disabled ? 0.45f : 1f;
            transform.localScale = _state == CardPresentationState.Highlighted ? Vector3.one * 1.025f : Vector3.one;
            if (_surface != null && _config != null)
                _surface.color = _config.GetColor(_state == CardPresentationState.Selected ? UISemanticColor.Selected : UISemanticColor.Surface);
            if (_stateOutline != null && _config != null)
            {
                _stateOutline.gameObject.SetActive(_state == CardPresentationState.Selected || _state == CardPresentationState.Highlighted);
                _stateOutline.color = _config.GetColor(_state == CardPresentationState.Selected ? UISemanticColor.Selected : UISemanticColor.Gold);
            }
        }

        private void ApplyRarity(CardRarityVisual rarity)
        {
            if (_rarityMedallion != null) _rarityMedallion.SetActive(true);
            if (_rarityLabel != null) _rarityLabel.text = RarityLabel(rarity);
            if (_rarityGem == null || _config == null) return;
            _rarityGem.color = _config.GetColor(RarityTone(rarity));
        }

        private static string RarityLabel(CardRarityVisual rarity)
        {
            switch (rarity)
            {
                case CardRarityVisual.Common: return "COMMON";
                case CardRarityVisual.Rare: return "RARE";
                case CardRarityVisual.Epic: return "EPIC";
                case CardRarityVisual.Legendary: return "LEGENDARY";
                default: return "—";
            }
        }

        private static UISemanticColor RarityTone(CardRarityVisual rarity)
        {
            switch (rarity)
            {
                case CardRarityVisual.Rare: return UISemanticColor.Mana;
                case CardRarityVisual.Epic: return UISemanticColor.Secondary;
                case CardRarityVisual.Legendary: return UISemanticColor.Gold;
                default: return UISemanticColor.TextSecondary;
            }
        }
        private void Apply(TMP_Text text, TypographyStyle style, UISemanticColor color)
        {
            if (text == null) return;
            _config.ApplyTypography(text, style); text.color = _config.GetColor(color);
        }
        private static void SetText(TMP_Text text, string value) { if (text != null) text.text = value ?? string.Empty; }
        private static void SetOptionalText(TMP_Text text, int? value)
        {
            if (text == null) return;
            text.gameObject.SetActive(value.HasValue);
            if (value.HasValue) text.text = value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        private static Color WithAlpha(Color color, float alpha) { color.a = alpha; return color; }
    }
}
