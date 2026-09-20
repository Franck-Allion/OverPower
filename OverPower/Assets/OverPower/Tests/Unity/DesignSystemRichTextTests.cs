using System;
using NUnit.Framework;
using OverPower.Unity.Presentation.DesignSystem;
using TMPro;
using UnityEditor;

namespace OverPower.Tests.Unity
{
    [TestFixture]
    public class DesignSystemRichTextTests
    {
        #region Formatter Tests

        [Test]
        public void Format_PlainText_IsUnchanged()
        {
            string plain = "Heal 20 to the targeted unit.";
            Assert.That(UIRichTextFormatter.Format(plain), Is.EqualTo(plain));
        }

        [Test]
        public void Format_EmptyOrNullString_IsSafe()
        {
            Assert.That(UIRichTextFormatter.Format(null!), Is.Empty);
            Assert.That(UIRichTextFormatter.Format(""), Is.Empty);
        }

        [TestCase("{health}", "<sprite name=\"health\">")]
        [TestCase("{mana}", "<sprite name=\"mana\">")]
        [TestCase("{gold}", "<sprite name=\"gold\">")]
        [TestCase("{attack}", "<sprite name=\"attack\">")]
        [TestCase("{HEALTH}", "<sprite name=\"health\">")] // Case insensitivity check
        public void Format_KnownTokens_ConvertToSpriteTags(string token, string expected)
        {
            Assert.That(UIRichTextFormatter.Format(token), Is.EqualTo(expected));
        }

        [Test]
        public void Format_MultipleAndRepeatedTokens_ConvertCorrectly()
        {
            string raw = "Costs 3 {gold}. Gain 2 {mana} and 1 {gold}.";
            string expected = "Costs 3 <sprite name=\"gold\">. Gain 2 <sprite name=\"mana\"> and 1 <sprite name=\"gold\">.";
            Assert.That(UIRichTextFormatter.Format(raw), Is.EqualTo(expected));
        }

        [TestCase("{unknown}")]
        [TestCase("{key}")]
        [TestCase("Normal { text } with braces")]
        public void Format_UnknownTokensAndBraces_RemainUnchanged(string token)
        {
            Assert.That(UIRichTextFormatter.Format(token), Is.EqualTo(token));
        }

        #endregion

        #region Sprite Asset Validation Tests

        [Test]
        public void SpriteAsset_OverPowerInlineIcons_ExistsAndHasValidSemanticMappings()
        {
            string path = "Assets/OverPower/UI/DesignSystem/Icons/OverPowerInlineIcons.asset";
            var spriteAsset = AssetDatabase.LoadAssetAtPath<TMP_SpriteAsset>(path);

            Assert.That(spriteAsset, Is.Not.Null, "OverPowerInlineIcons.asset must exist at designated location.");

            var requiredNames = new[] { "attack", "health", "gold", "mana" };
            foreach (var name in requiredNames)
            {
                // Verify entry exists in lookup/tables
                int index = spriteAsset.GetSpriteIndexFromName(name);
                Assert.That(index, Is.Not.EqualTo(-1), $"Sprite asset must contain mapping for name '{name}'.");

                var character = spriteAsset.spriteCharacterTable[index];
                Assert.That(character.name, Is.EqualTo(name));

                var glyph = spriteAsset.spriteGlyphTable[(int)character.glyphIndex];
                Assert.That(glyph.sprite, Is.Not.Null, $"Sprite '{name}' glyph must reference a valid Sprite asset.");
            }
        }

        #endregion
    }
}
