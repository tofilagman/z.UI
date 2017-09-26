using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace z.UI.TabControl.TabStyleProviders
{
    [System.ComponentModel.ToolboxItem(false)]
    public class TabStyleChromeProvider : TabStyleProvider
    {
        public TabStyleChromeProvider(CustomTabControl tabControl) : base(tabControl)
        {
            this._Overlap = 16;
            this._Radius = 16;
            this._ShowTabCloser = true;
            this._CloserColorActive = Color.White;

            //	Must set after the _Radius as this is used in the calculations of the actual padding
            this.Padding = new Point(7, 5);
        }


        public override void AddTabBorder(System.Drawing.Drawing2D.GraphicsPath path, System.Drawing.Rectangle tabBounds)
        {
            int spread = 0;
            int eigth = 0;
            int sixth = 0;
            int quarter = 0;

            if (this._TabControl.Alignment <= TabAlignment.Bottom)
            {
                spread = Convert.ToInt32(Math.Truncate(Math.Floor(Convert.ToDecimal(tabBounds.Height) * 2 / 3)));
                eigth = Convert.ToInt32(Math.Truncate(Math.Floor(Convert.ToDecimal(tabBounds.Height) * 1 / 8)));
                sixth = Convert.ToInt32(Math.Truncate(Math.Floor(Convert.ToDecimal(tabBounds.Height) * 1 / 6)));
                quarter = Convert.ToInt32(Math.Truncate(Math.Floor(Convert.ToDecimal(tabBounds.Height) * 1 / 4)));
            }
            else
            {
                spread = Convert.ToInt32(Math.Truncate(Math.Floor(Convert.ToDecimal(tabBounds.Width) * 2 / 3)));
                eigth = Convert.ToInt32(Math.Truncate(Math.Floor(Convert.ToDecimal(tabBounds.Width) * 1 / 8)));
                sixth = Convert.ToInt32(Math.Truncate(Math.Floor(Convert.ToDecimal(tabBounds.Width) * 1 / 6)));
                quarter = Convert.ToInt32(Math.Truncate(Math.Floor(Convert.ToDecimal(tabBounds.Width) * 1 / 4)));
            }

            switch (this._TabControl.Alignment)
            {
                case TabAlignment.Top:

                    path.AddCurve(new Point[] {
                    new Point(tabBounds.X, tabBounds.Bottom),
                    new Point(tabBounds.X + sixth, tabBounds.Bottom - eigth),
                    new Point(tabBounds.X + spread - quarter, tabBounds.Y + eigth),
                    new Point(tabBounds.X + spread, tabBounds.Y)
                });
                    path.AddLine(tabBounds.X + spread, tabBounds.Y, tabBounds.Right - spread, tabBounds.Y);
                    path.AddCurve(new Point[] {
                    new Point(tabBounds.Right - spread, tabBounds.Y),
                    new Point(tabBounds.Right - spread + quarter, tabBounds.Y + eigth),
                    new Point(tabBounds.Right - sixth, tabBounds.Bottom - eigth),
                    new Point(tabBounds.Right, tabBounds.Bottom)
                });
                    break; // TODO: might not be correct. Was : Exit Select

                   
                case TabAlignment.Bottom:
                    path.AddCurve(new Point[] {
                    new Point(tabBounds.Right, tabBounds.Y),
                    new Point(tabBounds.Right - sixth, tabBounds.Y + eigth),
                    new Point(tabBounds.Right - spread + quarter, tabBounds.Bottom - eigth),
                    new Point(tabBounds.Right - spread, tabBounds.Bottom)
                });
                    path.AddLine(tabBounds.Right - spread, tabBounds.Bottom, tabBounds.X + spread, tabBounds.Bottom);
                    path.AddCurve(new Point[] {
                    new Point(tabBounds.X + spread, tabBounds.Bottom),
                    new Point(tabBounds.X + spread - quarter, tabBounds.Bottom - eigth),
                    new Point(tabBounds.X + sixth, tabBounds.Y + eigth),
                    new Point(tabBounds.X, tabBounds.Y)
                });
                    break; // TODO: might not be correct. Was : Exit Select

                   
                case TabAlignment.Left:
                    path.AddCurve(new Point[] {
                    new Point(tabBounds.Right, tabBounds.Bottom),
                    new Point(tabBounds.Right - eigth, tabBounds.Bottom - sixth),
                    new Point(tabBounds.X + eigth, tabBounds.Bottom - spread + quarter),
                    new Point(tabBounds.X, tabBounds.Bottom - spread)
                });
                    path.AddLine(tabBounds.X, tabBounds.Bottom - spread, tabBounds.X, tabBounds.Y + spread);
                    path.AddCurve(new Point[] {
                    new Point(tabBounds.X, tabBounds.Y + spread),
                    new Point(tabBounds.X + eigth, tabBounds.Y + spread - quarter),
                    new Point(tabBounds.Right - eigth, tabBounds.Y + sixth),
                    new Point(tabBounds.Right, tabBounds.Y)
                });

                    break; // TODO: might not be correct. Was : Exit Select

                  
                case TabAlignment.Right:
                    path.AddCurve(new Point[] {
                    new Point(tabBounds.X, tabBounds.Y),
                    new Point(tabBounds.X + eigth, tabBounds.Y + sixth),
                    new Point(tabBounds.Right - eigth, tabBounds.Y + spread - quarter),
                    new Point(tabBounds.Right, tabBounds.Y + spread)
                });
                    path.AddLine(tabBounds.Right, tabBounds.Y + spread, tabBounds.Right, tabBounds.Bottom - spread);
                    path.AddCurve(new Point[] {
                    new Point(tabBounds.Right, tabBounds.Bottom - spread),
                    new Point(tabBounds.Right - eigth, tabBounds.Bottom - spread + quarter),
                    new Point(tabBounds.X + eigth, tabBounds.Bottom - sixth),
                    new Point(tabBounds.X, tabBounds.Bottom)
                });
                    break; // TODO: might not be correct. Was : Exit Select

                   
            }
        }

        protected override void DrawTabCloser(int index, Graphics graphics)
        {
            if (this._ShowTabCloser)
            {
                Rectangle closerRect = this._TabControl.GetTabCloserRect(index);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                if (closerRect.Contains(this._TabControl.MousePosition))
                {
                    using (GraphicsPath closerPath = GetCloserButtonPath(closerRect))
                    {
                        using (SolidBrush closerBrush = new SolidBrush(Color.FromArgb(193, 53, 53)))
                        {
                            graphics.FillPath(closerBrush, closerPath);
                        }
                    }
                    using (GraphicsPath closerPath = GetCloserPath(closerRect))
                    {
                        using (Pen closerPen = new Pen(this._CloserColorActive))
                        {
                            graphics.DrawPath(closerPen, closerPath);
                        }
                    }
                }
                else
                {
                    using (GraphicsPath closerPath = GetCloserPath(closerRect))
                    {
                        using (Pen closerPen = new Pen(this._CloserColor))
                        {
                            graphics.DrawPath(closerPen, closerPath);
                        }
                    }


                }
            }
        }

        private static GraphicsPath GetCloserButtonPath(Rectangle closerRect)
        {
            GraphicsPath closerPath = new GraphicsPath();
            closerPath.AddEllipse(new Rectangle(closerRect.X - 2, closerRect.Y - 2, closerRect.Width + 4, closerRect.Height + 4));
            closerPath.CloseFigure();
            return closerPath;
        }
    }

}
