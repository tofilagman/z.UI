using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace z.UI.Controls
{
    /// <summary>
    /// LJ 20140911
    /// Toast for Windows
    /// </summary>
    public class Toast : ToastBase
    {
        private Timer tmrAnimation;
        private Timer tmrDelay;

        private Color GlowColor = Color.Blue;
        private Single alphaval = 0;
        private Single incr = 0.1F;
        private bool isVisible = false;
        private SizeF textSize;
        private string msg = "";
        Control prnt;

        public Toast()
        {
            tmrAnimation = new Timer();
            tmrAnimation.Interval = 15;
            tmrDelay = new Timer();

            this.Paint += Control_Paint;
            this.tmrAnimation.Tick += tmrAnimation_Tick;
            this.tmrDelay.Tick += tmrDelay_Tick;
        }

        public void Show(Control Parent, string Message, Color Glow, int Delay, WindowLocation location = WindowLocation.BottomRight)
        {
            if (!isVisible)
            {
                isVisible = true;
                prnt = Parent;
                msg = Message;

                this.IsTransparent = true;

                textSize = this.CreateGraphics().MeasureString(Message, Parent.Font);
                this.Height = Convert.ToInt32(25 + textSize.Height);
                this.Width = Convert.ToInt32(35 + textSize.Width);
                if (textSize.Width > Parent.Width - 100)
                {
                    this.Width = Parent.Width - 100;
                    int hf = Convert.ToInt32(textSize.Width) / (Parent.Width - 100);
                    this.Height += Convert.ToInt32(textSize.Height * hf);
                }

                switch (location)
                {
                    case WindowLocation.BottomCenter:
                        this.Left = (Parent.Width - this.Width) / 2;
                        this.Top = (Parent.Height - this.Height) - 20;
                        break;
                    case WindowLocation.BottomRight:
                        this.Left = (Parent.Width - this.Width) - 20;
                        this.Top = (Parent.Height - this.Height) - 20;
                        break;
                    case WindowLocation.BottomLeft:
                        this.Left = 20;
                        this.Top = (Parent.Height - this.Height) - 20;
                        break;
                    case WindowLocation.TopCenter:
                        this.Left = (Parent.Width - this.Width) / 2;
                        this.Top = 10;
                        break;
                    case WindowLocation.TopLeft:
                         this.Left = 20;
                        this.Top = 10;
                        break;
                    case WindowLocation.TopRight:
                        this.Left = (Parent.Width - this.Width) - 20;
                        this.Top = 10;
                        break;
                }

                Parent.Controls.Add(this);
                this.BringToFront();
                GlowColor = Glow;

                tmrDelay.Interval = Delay;
                tmrAnimation.Enabled = true;
            }
            else
            {
                tmrDelay.Stop();
                tmrDelay.Start();
            }
        }

        private void Control_Paint(object sender, PaintEventArgs pe)
        {
            var img = new Bitmap(this.Width, this.Height);
            Graphics e = Graphics.FromImage(img);

            e.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            Brush bru = new SolidBrush(Color.FromArgb(50, GlowColor));
            Pen pn = new Pen(bru, 6);
            GraphicsPath gp = new GraphicsPath();

            pn.LineJoin = LineJoin.Round;

            gp.AddRectangle(new Rectangle(3, 3, this.Width - 10, this.Height - 10));
            e.DrawPath(pn, gp);

            gp.Reset();
            gp.AddRectangle(new Rectangle(7, 7, this.Width - 18, this.Height - 14));

            gp.Reset();
            gp.AddRectangle(new Rectangle(7, 7, this.Width - 18, this.Height - 18));
            e.DrawPath(pn, gp);

            gp.Reset();
            gp.AddRectangle(new Rectangle(9, 9, this.Width - 22, this.Height - 22));
            e.DrawPath(pn, gp);

            gp.Reset();
            bru = new SolidBrush(Color.FromArgb(7, 7, 7));
            pn = new Pen(bru, 5);
            pn.LineJoin = LineJoin.Round;
            gp.AddRectangle(new Rectangle(8, 8, this.Width - 20, this.Height - 20));
            e.DrawPath(pn, gp);
            e.FillRectangle(bru, new Rectangle(9, 9, this.Width - 21, this.Height - 21));

            ColorMatrix cma = new ColorMatrix();
            cma.Matrix33 = alphaval;
            ImageAttributes imga = new ImageAttributes();
            imga.SetColorMatrix(cma);

            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;
            e.DrawString(msg, prnt.Font, new SolidBrush(Color.FromArgb(247, 247, 247)), new Rectangle(9, 9, this.Width - 21, this.Height - 21), sf);

            pe.Graphics.DrawImage(img, new Rectangle(0, 0, this.Width, this.Height), 0, 0, img.Width, img.Height, GraphicsUnit.Pixel, imga);

            cma = null;
            sf.Dispose();
            imga.Dispose();
            e.Dispose();
            img.Dispose();
            bru.Dispose();
            pn.Dispose();
            gp.Dispose();
        }

        void tmrDelay_Tick(object sender, EventArgs e)
        {
            incr = -0.1F;
            tmrAnimation.Enabled = true;
            tmrDelay.Enabled = false;
        }

        void tmrAnimation_Tick(object sender, EventArgs e)
        {
            if (incr > 0)
            {
                if (alphaval < 1)
                {
                    if (alphaval + incr <= 1)
                    {
                        alphaval += incr;
                        this.Refresh();
                    }
                    else
                    {
                        alphaval = 1;
                        this.Refresh();
                        tmrAnimation.Enabled = false;
                        tmrDelay.Enabled = true;
                    }
                }
            }
            else
            {
                if (alphaval > 0)
                {
                    if (alphaval + incr >= 0)
                    {
                        alphaval += incr;
                        this.Refresh();
                    }
                    else
                    {
                        alphaval = 0;
                        tmrAnimation.Enabled = false;
                        tmrAnimation.Dispose();
                        tmrDelay.Dispose();
                        incr = 0.1F;
                        isVisible = false;
                        this.Dispose();
                    }
                }
            }
        }

        public enum WindowLocation
        {
            BottomRight = 0,
            BottomCenter = 1,
            BottomLeft = 2,
            TopRight = 3,
            TopCenter = 4,
            TopLeft = 5
        }

        //static

        public static void ShowToast(Control Parent, string Message, Color Glow, int Delay = 1000, WindowLocation location = WindowLocation.BottomRight)
        {
            Toast t = new Toast();
            t.Show(Parent, Message, Glow, Delay, location);
        }

    }
}
