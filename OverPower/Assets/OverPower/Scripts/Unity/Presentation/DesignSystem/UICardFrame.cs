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
        public string TypeLabel;
        public string RarityLabel;
        public string AttackStatLabel;
        public string ArmorStatLabel;
        public string HealthStatLabel;
        public string UnitCountStatLabel;
        public Sprite Artwork;
        public CardVisualType Type;
        public CardRarityVisual Rarity;
        public int? Cost;
        public int? Attack;
        public int? Armor;
        public int? Health;
        public int? UnitCount;
    }

    /// <summary>Reusable visual card frame. It deliberately knows no card, cost, or targeting rules.</summary>
    public sealed class UICardFrame : MonoBehaviour
    {
        [SerializeField] private UIDesignSystemConfig _config;
        [SerializeField] private Image _surface;
        [SerializeField] private Image _accent;
        [SerializeField] private Image _artwork;
        [SerializeField] private AspectRatioFitter _artworkAspect;
        [SerializeField] private GameObject _artworkPlaceholder;
        [SerializeField] private Image _artworkFrame;
        [SerializeField] private Image _familyIcon;
        [SerializeField] private GameObject _unitBanner;
        [SerializeField] private GameObject _spellBanner;
        [SerializeField] private GameObject _costBadge;
        [SerializeField] private TMP_Text _typeLabel;
        [SerializeField] private TMP_Text _costLabel;
        [SerializeField] private TMP_Text _titleLabel;
        [SerializeField] private UIRichText _description;
        [SerializeField] private GameObject _rarityMedallion;
        [SerializeField] private Image _rarityGem;
        [SerializeField] private TMP_Text _rarityLabel;
        [SerializeField] private Sprite _commonRaritySprite;
        [SerializeField] private Sprite _rareRaritySprite;
        [SerializeField] private Sprite _epicRaritySprite;
        [SerializeField] private Sprite _legendaryRaritySprite;
        [SerializeField] private GameObject _stats;
        [SerializeField] private GameObject _spellFooter;
        [SerializeField] private GameObject _attackSlot;
        [SerializeField] private GameObject _armorSlot;
        [SerializeField] private GameObject _healthSlot;
        [SerializeField] private GameObject _unitCountSlot;
        [SerializeField] private TMP_Text _attackLabel;
        [SerializeField] private TMP_Text _armorLabel;
        [SerializeField] private TMP_Text _healthLabel;
        [SerializeField] private TMP_Text _unitCountLabel;
        [SerializeField] private TMP_Text _attackCaption;
        [SerializeField] private TMP_Text _armorCaption;
        [SerializeField] private TMP_Text _healthCaption;
        [SerializeField] private TMP_Text _unitCountCaption;
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private Image _stateOutline;
        private CardPresentationState _state;

        public CardPresentationState State => _state;
        public CardVisualType Type { get; private set; }
        public CardRarityVisual Rarity { get; private set; }
        public int? UnitCount { get; private set; }
        public bool HasArtwork => _artwork != null && _artwork.sprite != null;

        private void Awake() { ApplyState(); }
        private void OnEnable() { ApplyState(); }

        public void SetPresentation(CardPresentationData data)
        {
            if (data == null) throw new System.ArgumentNullException(nameof(data));
            Type = data.Type;
            Rarity = data.Rarity;
            UnitCount = data.UnitCount;
            SetText(_titleLabel, data.Title);
            SetText(_attackCaption, data.AttackStatLabel);
            SetText(_armorCaption, data.ArmorStatLabel);
            SetText(_healthCaption, data.HealthStatLabel);
            SetText(_unitCountCaption, data.UnitCountStatLabel);
            if (_description != null) _description.SetText(data.Description ?? string.Empty);
            SetOptionalSlot(_costBadge, _costLabel, data.Cost);
            SetOptionalSlot(_attackSlot, _attackLabel, data.Attack);
            SetOptionalSlot(_armorSlot, _armorLabel, data.Armor);
            SetOptionalSlot(_healthSlot, _healthLabel, data.Health);
            SetOptionalSlot(_unitCountSlot, _unitCountLabel, data.UnitCount, "x");
            if (_artwork != null)
            {
                _artwork.sprite = data.Artwork;
                _artwork.gameObject.SetActive(data.Artwork != null);
                if (_artworkAspect != null && data.Artwork != null && data.Artwork.rect.height > 0f)
                    _artworkAspect.aspectRatio = data.Artwork.rect.width / data.Artwork.rect.height;
            }
            if (_artworkPlaceholder != null) _artworkPlaceholder.SetActive(data.Artwork == null);
            SetText(_typeLabel, data.TypeLabel);
            bool isUnit = data.Type == CardVisualType.Unit;
            if (_unitBanner != null) _unitBanner.SetActive(isUnit);
            if (_spellBanner != null) _spellBanner.SetActive(!isUnit);
            if (_stats != null) _stats.SetActive(isUnit && (data.Attack.HasValue || data.Armor.HasValue || data.Health.HasValue || data.UnitCount.HasValue));
            if (_spellFooter != null) _spellFooter.SetActive(!isUnit);
            var typeTone = data.Type == CardVisualType.Unit ? UISemanticColor.Interactive : UISemanticColor.Mana;
            if (_accent != null && _config != null) _accent.color = _config.GetColor(typeTone);
            if (_familyIcon != null && _config != null) _familyIcon.color = _config.GetColor(typeTone);
            if (_artworkFrame != null && _config != null) _artworkFrame.color = WithAlpha(_config.GetColor(typeTone), 0.78f);
            ApplyRarity(data.Rarity, data.RarityLabel);
            ApplyTypography();
            ApplyState();
        }

        public void SetState(CardPresentationState state) { _state = state; ApplyState(); }

        private void ApplyTypography()
        {
            if (_config == null) return;
            Apply(_titleLabel, TypographyStyle.Heading, UISemanticColor.TextPrimary);
            if (_titleLabel != null) { _titleLabel.enableAutoSizing = true; _titleLabel.fontSizeMin = 22; _titleLabel.fontSizeMax = 38; }
            Apply(_typeLabel, TypographyStyle.Caption, UISemanticColor.TextSecondary);
            Apply(_costLabel, TypographyStyle.Caption, UISemanticColor.Gold);
            Apply(_rarityLabel, TypographyStyle.Caption, UISemanticColor.TextPrimary);
            Apply(_attackLabel, TypographyStyle.Stat, UISemanticColor.Danger);
            Apply(_armorLabel, TypographyStyle.Stat, UISemanticColor.Armor);
            Apply(_healthLabel, TypographyStyle.Stat, UISemanticColor.Health);
            Apply(_unitCountLabel, TypographyStyle.Stat, UISemanticColor.Interactive);
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
                _stateOutline.color = WithAlpha(_config.GetColor(_state == CardPresentationState.Selected ? UISemanticColor.Selected : UISemanticColor.Gold), .20f);
            }
        }

        private void ApplyRarity(CardRarityVisual rarity, string label)
        {
            if (_rarityMedallion != null) _rarityMedallion.SetActive(true);
            SetText(_rarityLabel, label);
            if (_rarityGem == null || _config == null) return;
            _rarityGem.sprite = RaritySprite(rarity);
            _rarityGem.gameObject.SetActive(_rarityGem.sprite != null);
            _rarityGem.color = rarity == CardRarityVisual.Common ? Color.white : _config.GetColor(RarityTone(rarity));
        }

        private Sprite RaritySprite(CardRarityVisual rarity)
        {
            switch (rarity)
            {
                case CardRarityVisual.Rare: return _rareRaritySprite != null ? _rareRaritySprite : _commonRaritySprite;
                case CardRarityVisual.Epic: return _epicRaritySprite != null ? _epicRaritySprite : _commonRaritySprite;
                case CardRarityVisual.Legendary: return _legendaryRaritySprite != null ? _legendaryRaritySprite : _commonRaritySprite;
                case CardRarityVisual.Common: return _commonRaritySprite;
                default: return null;
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
        private static void SetOptionalSlot(GameObject slot, TMP_Text text, int? value, string prefix = "")
        {
            if (slot != null) slot.SetActive(value.HasValue);
            if (text == null) return;
            if (slot == null) text.gameObject.SetActive(value.HasValue);
            if (value.HasValue) text.text = prefix + value.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
        private static Color WithAlpha(Color color, float alpha) { color.a = alpha; return color; }
    }
}
