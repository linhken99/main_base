// ============================================
// 🎨 MODERN UI THEME MANAGER
// ============================================
// Apply consistent modern design across the application

using System.Drawing;
using Sunny.UI;

namespace Main_Base.Themes
{
    /// <summary>
    /// Modern theme colors and styles for the application
    /// </summary>
    public static class ModernTheme
    {
        #region Color Palette
        
        // Primary Colors
        public static class Primary
        {
            public static readonly Color Blue = Color.FromArgb(33, 150, 243);      // #2196F3
            public static readonly Color DarkBlue = Color.FromArgb(25, 125, 210);  // #197DD2
            public static readonly Color LightBlue = Color.FromArgb(66, 165, 245); // #42A5F5
        }

        // Status Colors
        public static class Status
        {
            public static readonly Color Success = Color.FromArgb(76, 175, 80);    // #4CAF50
            public static readonly Color Error = Color.FromArgb(244, 67, 54);      // #F44336
            public static readonly Color Warning = Color.FromArgb(255, 193, 7);    // #FFC107
            public static readonly Color Info = Color.FromArgb(33, 150, 243);      // #2196F3
        }

        // Neutral Colors
        public static class Neutral
        {
            public static readonly Color DarkGray = Color.FromArgb(48, 48, 48);    // #303030
            public static readonly Color MediumGray = Color.FromArgb(158, 158, 158); // #9E9E9E
            public static readonly Color LightGray = Color.FromArgb(224, 224, 224); // #E0E0E0
            public static readonly Color White = Color.White;
        }

        // Background Colors
        public static class Background
        {
            public static readonly Color Primary = Color.FromArgb(249, 249, 249);  // #F9F9F9
            public static readonly Color Secondary = Color.FromArgb(245, 245, 245); // #F5F5F5
        }

        #endregion

        #region Font Styles

        /// <summary>
        /// Get styled font for different UI elements
        /// </summary>
        public static class Fonts
        {
            public static Font HeadingLarge => new Font("Segoe UI", 18F, FontStyle.Bold);
            public static Font HeadingMedium => new Font("Segoe UI", 14F, FontStyle.Bold);
            public static Font HeadingSmall => new Font("Segoe UI", 12F, FontStyle.Bold);
            
            public static Font BodyLarge => new Font("Segoe UI", 12F, FontStyle.Regular);
            public static Font BodyMedium => new Font("Segoe UI", 11F, FontStyle.Regular);
            public static Font BodySmall => new Font("Segoe UI", 10F, FontStyle.Regular);
            
            public static Font ButtonPrimary => new Font("Segoe UI", 11F, FontStyle.Bold);
            public static Font ButtonSecondary => new Font("Segoe UI", 10F, FontStyle.Regular);
            
            public static Font CaptionSmall => new Font("Segoe UI", 9F, FontStyle.Regular);
            public static Font CaptionItalic => new Font("Segoe UI", 9F, FontStyle.Italic);
        }

        #endregion

        #region Button Styles

        /// <summary>
        /// Apply modern button styling
        /// </summary>
        public static void ApplyPrimaryButtonStyle(UIButton button)
        {
            button.FillColor = Primary.Blue;
            button.FillColor2 = Primary.Blue;
            button.FillHoverColor = Primary.DarkBlue;
            button.FillPressColor = Primary.DarkBlue;
            button.ForeColor = Neutral.White;
            button.Font = Fonts.ButtonPrimary;
            button.Cursor = System.Windows.Forms.Cursors.Hand;
            button.RectColor = Primary.Blue;
        }

        public static void ApplySecondaryButtonStyle(UIButton button)
        {
            button.FillColor = Neutral.LightGray;
            button.FillColor2 = Neutral.LightGray;
            button.FillHoverColor = Neutral.MediumGray;
            button.FillPressColor = Neutral.MediumGray;
            button.ForeColor = Neutral.DarkGray;
            button.Font = Fonts.ButtonSecondary;
            button.Cursor = System.Windows.Forms.Cursors.Hand;
            button.RectColor = Neutral.LightGray;
        }

        public static void ApplyDangerButtonStyle(UIButton button)
        {
            button.FillColor = Status.Error;
            button.FillColor2 = Status.Error;
            button.FillHoverColor = Color.FromArgb(229, 57, 53); // Darker red
            button.FillPressColor = Color.FromArgb(211, 47, 47); // Even darker
            button.ForeColor = Neutral.White;
            button.Font = Fonts.ButtonPrimary;
            button.Cursor = System.Windows.Forms.Cursors.Hand;
            button.RectColor = Status.Error;
        }

        public static void ApplySuccessButtonStyle(UIButton button)
        {
            button.FillColor = Status.Success;
            button.FillColor2 = Status.Success;
            button.FillHoverColor = Color.FromArgb(67, 160, 71); // Darker green
            button.FillPressColor = Color.FromArgb(56, 142, 60); // Even darker
            button.ForeColor = Neutral.White;
            button.Font = Fonts.ButtonPrimary;
            button.Cursor = System.Windows.Forms.Cursors.Hand;
            button.RectColor = Status.Success;
        }

        #endregion

        #region Text Input Styles

        /// <summary>
        /// Apply modern textbox styling
        /// </summary>
        public static void ApplyTextBoxStyle(UITextBox textBox)
        {
            textBox.FillColor = Neutral.White;
            textBox.FillColor2 = Neutral.White;
            textBox.ForeColor = Neutral.DarkGray;
            textBox.Font = Fonts.BodyMedium;
            textBox.RectColor = Neutral.LightGray;
            textBox.RectHoverColor = Primary.Blue;
            textBox.RectFocusColor = Primary.Blue;
            textBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            textBox.MinimumSize = new System.Drawing.Size(1, 16);
            textBox.Padding = new System.Windows.Forms.Padding(5);
        }

        #endregion

        #region Panel Styles

        /// <summary>
        /// Apply modern panel styling
        /// </summary>
        public static void ApplyCardStyle(UIPanel panel)
        {
            panel.FillColor = Neutral.White;
            panel.FillColor2 = Neutral.White;
            panel.RectColor = Neutral.LightGray;
            panel.BackColor = Neutral.White;
            panel.Margin = new System.Windows.Forms.Padding(8);
            // Optional: Add shadow effect if supported
        }

        public static void ApplyLightPanelStyle(UIPanel panel)
        {
            panel.FillColor = Background.Secondary;
            panel.FillColor2 = Background.Secondary;
            panel.RectColor = Neutral.LightGray;
            panel.BackColor = Background.Secondary;
        }

        #endregion

        #region Label Styles

        /// <summary>
        /// Apply modern label styling
        /// </summary>
        public static void ApplyHeadingStyle(UILabel label)
        {
            label.ForeColor = Neutral.DarkGray;
            label.Font = Fonts.HeadingMedium;
            label.AutoSize = true;
        }

        public static void ApplyBodyTextStyle(UILabel label)
        {
            label.ForeColor = Neutral.DarkGray;
            label.Font = Fonts.BodyMedium;
            label.AutoSize = true;
        }

        public static void ApplyErrorTextStyle(UILabel label)
        {
            label.ForeColor = Status.Error;
            label.Font = Fonts.BodySmall;
            label.AutoSize = true;
        }

        public static void ApplySuccessTextStyle(UILabel label)
        {
            label.ForeColor = Status.Success;
            label.Font = Fonts.BodySmall;
            label.AutoSize = true;
        }

        #endregion

        #region Shadow & Elevation Effects

        /// <summary>
        /// Get shadow properties for elevated elements
        /// </summary>
        public static class Elevation
        {
            public static readonly int Level1 = 1;  // Subtle shadow
            public static readonly int Level2 = 4;  // Raised
            public static readonly int Level3 = 8;  // High elevation
            public static readonly int Level4 = 16; // Modal dialog
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Convert hex color to Color object
        /// </summary>
        public static Color HexToColor(string hex)
        {
            hex = hex.Replace("#", "");
            return Color.FromArgb(
                int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber),
                int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber)
            );
        }

        /// <summary>
        /// Apply overall application theme
        /// </summary>
        public static void ApplyApplicationTheme(UIForm form)
        {
            form.BackColor = Background.Primary;
            form.ForeColor = Neutral.DarkGray;
            form.Font = Fonts.BodyMedium;
        }

        #endregion

        #region Animation Constants

        public static class Animations
        {
            public const int DurationFast = 150;    // ms
            public const int DurationNormal = 300;  // ms
            public const int DurationSlow = 500;    // ms
            public const int EaseInOutCurve = 25;   // Curve type
        }

        #endregion
    }

    /// <summary>
    /// Helper for applying consistent icons and symbols
    /// </summary>
    public static class ModernIcons
    {
        // Common icon symbols (FontAwesome)
        public const int IconUser = 61447;
        public const int IconLock = 61475;
        public const int IconEyeOpen = 558391;
        public const int IconEyeClosed = 361552;
        public const int IconCheckmark = 61451;
        public const int IconX = 61453;
        public const int IconWarning = 61530;
        public const int IconInfo = 61531;
        public const int IconSettings = 61640;
        public const int IconHome = 61461;
        public const int IconBack = 61457;
        public const int IconNext = 61458;
    }
}
