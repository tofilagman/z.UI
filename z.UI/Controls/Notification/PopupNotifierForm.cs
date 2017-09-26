using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Windows.Forms;

namespace z.UI.Controls.Notification
{
    [DefaultProperty("Content"), DesignTimeVisible(false)]
    public class PopupNotifierForm : System.Windows.Forms.Form
    {
        public PopupNotifierForm(PopupNotifier Parent)
        {
            this.InitializeComponent();
            this.pnParent = Parent;
            this.SetStyle(System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(System.Windows.Forms.ControlStyles.ResizeRedraw, true);
            this.SetStyle(System.Windows.Forms.ControlStyles.AllPaintingInWmPaint, true);
            this.ShowInTaskbar = false;
            this.Paint += Form_Paint;
            this.MouseUp += Form_MouseUp;
            this.MouseMove += Form_MouseMove;
            this.TopMost = true;
            this.Shown += PopupNotifierForm_Shown;
        }

        void PopupNotifierForm_Shown(object sender, EventArgs e)
        {
            this.InvokeLostFocus(this, e);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(392, 66);
            this.TopMost = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PopupNotifierForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.ResumeLayout(false);
        }

        private bool bMouseOnClose = false;
        private bool bMouseOnLink = false;
        private bool bMouseOnOptions = false;
        private int iHeightOfTitle;

        public event EventHandler LinkClick;
        public event EventHandler CloseClick;
       
        #region Properties

        private PopupNotifier pnParent;
        public new PopupNotifier Parent
        {
            get
            {
                return pnParent;
            }
            set
            {
                pnParent = value;
            }
        }

        #endregion

        #region Functions & Private properties
        private int AddValueMax255(int Input, int Add)
        {
            if (Input + Add < 256) return Input + Add;
            else return 255;
        }

        private int DedValueMin0(int Input, int Ded)
        {
            if (Input - Ded > 0) return Input - Ded;
            else return 0;
        }

        private Color GetDarkerColor(Color Color)
        {
            return Color.FromArgb(255, DedValueMin0(Convert.ToInt32(Color.R), Parent.GradientPower), DedValueMin0(Convert.ToInt32(Color.G), Parent.GradientPower), DedValueMin0(Convert.ToInt32(Color.B), Parent.GradientPower));
        }

        private Color GetLighterColor(Color Color)
        {
            return Color.FromArgb(255, AddValueMax255(Convert.ToInt32(Color.R), Parent.GradientPower), AddValueMax255(Convert.ToInt32(Color.G), Parent.GradientPower), AddValueMax255(Convert.ToInt32(Color.B), Parent.GradientPower));
        }

        private RectangleF RectText
        {
            get
            {
                if (Parent.Image != null)
                {
                    return new RectangleF(Parent.ImagePosition.X + Parent.ImageSize.Width + Parent.TextPadding.Left, Parent.TextPadding.Top + Parent.TextPadding.Top + iHeightOfTitle + Parent.HeaderHeight, this.Width - Parent.ImageSize.Width - Parent.ImagePosition.X - 16 - 5 - Parent.TextPadding.Left - Parent.TextPadding.Right, this.Height - Parent.HeaderHeight - Parent.TextPadding.Top - Parent.TextPadding.Top - Parent.TextPadding.Bottom - iHeightOfTitle - 1);
                }
                else
                {
                    return new RectangleF(Parent.TextPadding.Left, Parent.TextPadding.Top + Parent.TextPadding.Top + iHeightOfTitle + Parent.HeaderHeight, this.Width - 16 - 5 - Parent.TextPadding.Left - Parent.TextPadding.Right, this.Height - Parent.HeaderHeight - Parent.TextPadding.Top - Parent.TextPadding.Top - Parent.TextPadding.Bottom - iHeightOfTitle - 1);
                }
            }
        }

        private Rectangle RectClose
        {
            get { return new Rectangle(this.Width - 5 - 16, 12, 16, 16); }
        }

        private Rectangle RectOptions
        {
            get { return new Rectangle(this.Width - 5 - 16, 12 + 16 + 5, 16, 16); }
        }

        #endregion

        #region Events

        private void Form_MouseMove(object Sender, MouseEventArgs e)
        {
            if (Parent.CloseButton)
            {
                if (RectClose.Contains(e.X, e.Y)) bMouseOnClose = true;
                else bMouseOnClose = false;
            }

            if (Parent.OptionsButton)
            {
                if (RectOptions.Contains(e.X, e.Y)) bMouseOnOptions = true;
                else bMouseOnOptions = false;
            }

            if (RectText.Contains(e.X, e.Y)) 
                bMouseOnLink = true;
            else 
                bMouseOnLink = false;

            Invalidate();
        }


        private void Form_MouseUp(object Sender, MouseEventArgs e)
        {
            if (RectClose.Contains(e.X, e.Y)) CloseClick(this, EventArgs.Empty);
            if (RectText.Contains(e.X, e.Y)) LinkClick(this, EventArgs.Empty);
            if (RectOptions.Contains(e.X, e.Y))
            {
                if (Parent.OptionsMenu != null)
                {
                    Parent.OptionsMenu.Show(this, new Point(RectOptions.Right - Parent.OptionsMenu.Width, RectOptions.Bottom));
                    Parent.bShouldRemainVisible = true;
                }
            }
        }

        private void Form_Paint(object Sender, PaintEventArgs e)
        {
            Rectangle rcBody = new Rectangle(0, 0, this.Width, this.Height);
            Rectangle rcForm = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            LinearGradientBrush brBody = new LinearGradientBrush(rcBody, Parent.BodyColor, GetLighterColor(Parent.BodyColor), LinearGradientMode.Vertical);

            if (Parent.HeaderHeight > 0)
            {
                Rectangle rcHeader = new Rectangle(0, 0, this.Width, Parent.HeaderHeight);
                LinearGradientBrush brHeader = new LinearGradientBrush(rcHeader, Parent.HeaderColor, GetDarkerColor(Parent.HeaderColor), LinearGradientMode.Vertical);
                e.Graphics.FillRectangle(brHeader, rcHeader);
            }
            
            e.Graphics.FillRectangle(brBody, rcBody);
            
            e.Graphics.DrawRectangle(new Pen(Parent.BorderColor), rcForm);
            if (Parent.ShowGrip) e.Graphics.DrawImage(z.UI.Properties.Resources.grip, Convert.ToInt32(this.Width - z.UI.Properties.Resources.grip.Width) / 2, Convert.ToInt32(Parent.HeaderHeight - 3) / 2);
            if (Parent.CloseButton)
            {
                if (bMouseOnClose)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Parent.ButtonHoverColor), RectClose);
                    e.Graphics.DrawRectangle(new Pen(Parent.ButtonBorderColor), RectClose);
                }
                e.Graphics.DrawLine(new Pen(Parent.ContentColor, 2), RectClose.Left + 4, RectClose.Top + 4, RectClose.Right - 4, RectClose.Bottom - 4);
                e.Graphics.DrawLine(new Pen(Parent.ContentColor, 2), RectClose.Left + 4, RectClose.Bottom - 4, RectClose.Right - 4, RectClose.Top + 4);
            }
            if (Parent.OptionsButton)
            {
                if (bMouseOnOptions)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Parent.ButtonHoverColor), RectOptions);
                    e.Graphics.DrawRectangle(new Pen(Parent.ButtonBorderColor), RectOptions);
                }
                e.Graphics.FillPolygon(new SolidBrush(ForeColor), new Point[] { new Point(RectOptions.Left + 4, RectOptions.Top + 6), new Point(RectOptions.Left + 12, RectOptions.Top + 6), new Point(RectOptions.Left + 8, RectOptions.Top + 4 + 6) });
            }
            if (Parent.Image != null) e.Graphics.DrawImage(Parent.Image, Parent.ImagePosition.X, Parent.ImagePosition.Y, Parent.ImageSize.Width, Parent.ImageSize.Height);
            iHeightOfTitle = Convert.ToInt32( e.Graphics.MeasureString("A", Parent.TitleFont).Height);
            int iTitleOrigin;
            if (Parent.Image != null) iTitleOrigin = Parent.ImagePosition.X + Parent.ImageSize.Width + Parent.TextPadding.Left;
            else iTitleOrigin = Parent.TextPadding.Left;
            if (bMouseOnLink)
            {
                this.Cursor = Cursors.Hand;
                e.Graphics.DrawString(Parent.ContentText, Parent.ContentFont, new SolidBrush(Parent.LinkHoverColor), RectText);
            }
            else
            {
                this.Cursor = Cursors.Default;
                e.Graphics.DrawString(Parent.ContentText, Parent.ContentFont, new SolidBrush(Parent.ContentColor), RectText);
            }
            e.Graphics.DrawString(Parent.TitleText, Parent.TitleFont, new SolidBrush(Parent.TitleColor), iTitleOrigin, Parent.TextPadding.Top + Parent.HeaderHeight);
        }

        #endregion

    }
}
