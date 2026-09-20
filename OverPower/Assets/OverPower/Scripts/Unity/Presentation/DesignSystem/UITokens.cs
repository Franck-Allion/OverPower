namespace OverPower.Unity.Presentation.DesignSystem
{
    public enum TypographyStyle { Display, Title, Heading, Body, BodySmall, Caption, Stat, Button }
    public enum UISemanticColor
    {
        Primary, Secondary, Interactive, Selected, Disabled, Danger, Warning, Success,
        Health, Mana, Armor, Gold, Locked, Affordable, Unaffordable,
        Background, Surface, TextPrimary, TextSecondary
    }
    public enum UIMotion { Fast, Normal, Emphasis }
    public enum UIButtonFamily { Primary, Secondary, Icon }

    public static class UISpacing
    {
        public const int Xs = 4, Sm = 8, Md = 12, Lg = 16, Xl = 24,
            Xxl = 32, Section = 48, Page = 64;
    }
}
