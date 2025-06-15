using Microsoft.UI.Xaml;
using TimeCafeWinUI3.Contracts.Services;

namespace TimeCafeWinUI3.Services
{
    public class ThemeColorService : IThemeColorService
    {
        private readonly Dictionary<string, string> _borderStyleKeys = new()
        {
            { "Red", "RedGradientBorderStyle" },
            { "Orange", "OrangeGradientBorderStyle" },
            { "Amber", "AmberGradientBorderStyle" },
            { "Yellow", "YellowGradientBorderStyle" },
            { "Lime", "LimeGradientBorderStyle" },
            { "Green", "GreenGradientBorderStyle" },
            { "Emerald", "EmeraldGradientBorderStyle" },
            { "Teal", "TealGradientBorderStyle" },
            { "Cyan", "CyanGradientBorderStyle" },
            { "Sky", "SkyGradientBorderStyle" },
            { "Blue", "BlueGradientBorderStyle" },
            { "Indigo", "IndigoGradientBorderStyle" },
            { "Violet", "VioletGradientBorderStyle" },
            { "Purple", "PurpleGradientBorderStyle" },
            { "Fuchsia", "FuchsiaGradientBorderStyle" },
            { "Pink", "PinkGradientBorderStyle" },
            { "Rose", "RoseGradientBorderStyle" },
            { "Slate", "SlateGradientBorderStyle" },
            { "Gray", "GrayGradientBorderStyle" },
            { "Zinc", "ZincGradientBorderStyle" },
            { "Neutral", "NeutralGradientBorderStyle" },
            { "Stone", "StoneGradientBorderStyle" }
        };

        public Style GetThemeBorderStyle(string technicalName)
        {
            if (string.IsNullOrEmpty(technicalName) || !_borderStyleKeys.ContainsKey(technicalName))
            {
                return null;
            }

            var styleKey = _borderStyleKeys[technicalName];
            return Application.Current.Resources[styleKey] as Style;
        }
    }
} 