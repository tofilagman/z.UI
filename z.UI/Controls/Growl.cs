using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace z.UI.Controls
{
    public partial class Growl : Form
    {
        public Growl()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            tmrAnimation = new Timer();
            tmrAnimation.Interval = 15;
            tmrDelay = new Timer();

            this.Paint += Control_Paint;
            this.tmrAnimation.Tick += tmrAnimation_Tick;
            this.tmrDelay.Tick += tmrDelay_Tick;
        }

        private bool m_isTransparent = false;
        private Timer tmrAnimation;
        private Timer tmrDelay;

        private Color GlowColor = Color.Blue;
        private Single alphaval = 0;
        private Single incr = 0.1F;
        private bool isVisible = false;
        private SizeF textSize;
        private string msg = "";
        private StringAlignment TextAlignment;

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

        protected override void OnLoad(EventArgs e)
        {
            //var screen = Screen.FromPoint(this.Location);
            //this.Location = new Point(screen.WorkingArea.Right - (this.Width + 20), screen.WorkingArea.Bottom - (this.Height + 20));
            base.OnLoad(e);
        }

        public void Show(string Message, Color Glow, int Delay, StringAlignment TextAlignment = StringAlignment.Center, WindowLocation location = WindowLocation.BottomRight)
        {
            this.Opacity = 0.83;
            this.Show();

            this.TextAlignment = TextAlignment;
            msg = Message;

            this.IsTransparent = true;

            textSize = this.CreateGraphics().MeasureString(Message, this.Font);
            this.Height = Convert.ToInt32(25 + textSize.Height);
            this.Width = Convert.ToInt32(35 + textSize.Width);

            var screen = Screen.FromPoint(this.Location);

            //if (textSize.Width > this.Width - 100)
            //{
            //    this.Width = Parent.Width - 100;
            //    int hf = Convert.ToInt32(textSize.Width) / (Parent.Width - 100);
            //    this.Height += Convert.ToInt32(textSize.Height * hf);
            //}

            //this.Left = (Parent.Width - this.Width) / 2;
            //this.Top = (Parent.Height - this.Height) - 50;
            switch (location)
            {
                case WindowLocation.BottomRight:
                    this.Location = new Point(screen.WorkingArea.Right - (this.Width + 20), screen.WorkingArea.Bottom - (this.Height + 20));
                    break;
                case WindowLocation.BottomLeft:
                    this.Location = new Point(20, screen.WorkingArea.Bottom - (this.Height + 20));
                    break;
                case WindowLocation.BottomCenter:
                    this.Location = new Point((screen.WorkingArea.Right - this.Width) / 2, screen.WorkingArea.Bottom - (this.Height + 20));
                    break;
                case WindowLocation.TopCenter:
                    this.Location = new Point((screen.WorkingArea.Right - this.Width) / 2, screen.WorkingArea.Top + 20);
                    break;
                case WindowLocation.TopLeft:
                    this.Location = new Point(20, screen.WorkingArea.Top + 20);
                    break;
                case WindowLocation.TopRight:
                    this.Location = new Point(screen.WorkingArea.Right - (this.Width + 20), screen.WorkingArea.Top + 20);
                    break;
            }

            GlowColor = Glow;

            tmrDelay.Interval = Delay;
            tmrAnimation.Enabled = true;

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
            sf.Alignment = TextAlignment;
            sf.LineAlignment = StringAlignment.Center;
            e.DrawString(msg, this.Font, new SolidBrush(Color.FromArgb(247, 247, 247)), new Rectangle(9, 9, this.Width - 21, this.Height - 21), sf);

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
                        this.Close();
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
        public static void ShowGrowl(string Message, Color Glow, int Delay, StringAlignment TextAlignment = StringAlignment.Center, WindowLocation location = WindowLocation.BottomRight)
        {
            Growl g = new Growl();
            g.IsTransparent = true;
            g.Show(Message, Glow, Delay, TextAlignment, location);
        }
    }
}
