using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;

namespace z.UI.Controls
{
    /// <summary>
    /// LJ 20140911
    /// ToastBase
    /// </summary>
    public class ToastBase : Control
    {

        public ToastBase()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        private bool m_isTransparent = false;

        public bool IsTransparent
        {
            get
            {
                return this.m_isTransparent;
            }
            set
            {
                m_isTransparent = value;
                if (value) this.BackColor = Color.Transparent;
                Invalidate();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);

            if (IsTransparent)
            {
                if (Parent != null)
                {
                    int myIndex = Parent.Controls.GetChildIndex(this);
                    for (int i = Parent.Controls.Count - 1; i < myIndex + 1; i--)
                    {
                        Control ctrl = Parent.Controls[i];
                        if (ctrl.Bounds.IntersectsWith(Bounds))
                        {
                            if (ctrl.Visible)
                            {
                                Bitmap bmp = new Bitmap(ctrl.Width, ctrl.Height);
                                ctrl.DrawToBitmap(bmp, ctrl.ClientRectangle);
                                pevent.Graphics.TranslateTransform(ctrl.Left - Left, ctrl.Top - Top);
                                pevent.Graphics.DrawImage(bmp, Point.Empty);
                                pevent.Graphics.TranslateTransform(Left - ctrl.Left, Top - ctrl.Top);
                                bmp.Dispose();
                            }
                        }
                    }
                }
            }
        }

    }
}
