using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace z.UI.TabControl
{
    internal sealed class ThemedColors
    {

        #region "    Variables and Constants "

        private const string NormalColor = "NormalColor";
        private const string HomeStead = "HomeStead";
        private const string Metallic = "Metallic";

        private const string NoTheme = "NoTheme";
        private static Color[] _toolBorder;
        #endregion

        #region "    Properties "

        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static ColorScheme CurrentThemeIndex
        {
            get { return ThemedColors.GetCurrentThemeIndex(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public static Color ToolBorder
        {
            get { return ThemedColors._toolBorder[Convert.ToInt32(ThemedColors.CurrentThemeIndex)]; }
        }

        #endregion

        #region "    Constructors "

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
        static ThemedColors()
        {
            ThemedColors._toolBorder = new Color[] {
            Color.FromArgb(127, 157, 185),
            Color.FromArgb(164, 185, 127),
            Color.FromArgb(165, 172, 178),
            Color.FromArgb(132, 130, 132)
        };
        }

        private ThemedColors()
        {
        }

        #endregion

        private static ColorScheme GetCurrentThemeIndex()
        {
            ColorScheme theme = ColorScheme.NoTheme;


            if (VisualStyleInformation.IsSupportedByOS && VisualStyleInformation.IsEnabledByUser && Application.RenderWithVisualStyles)
            {

                switch (VisualStyleInformation.ColorScheme)
                {
                    case NormalColor:
                        theme = ColorScheme.NormalColor;
                        break; // TODO: might not be correct. Was : Exit Select

                      
                    case HomeStead:
                        theme = ColorScheme.HomeStead;
                        break; // TODO: might not be correct. Was : Exit Select

                      
                    case Metallic:
                        theme = ColorScheme.Metallic;
                        break; // TODO: might not be correct. Was : Exit Select

                       
                    default:
                        theme = ColorScheme.NoTheme;
                        break; // TODO: might not be correct. Was : Exit Select

                       
                }
            }

            return theme;
        }

        public enum ColorScheme
        {
            NormalColor = 0,
            HomeStead = 1,
            Metallic = 2,
            NoTheme = 3
        }

    }

}
