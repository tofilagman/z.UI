using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace z.UI.Controls.Notification
{
    [DefaultEvent("Click"), CLSCompliant(true)]
    public class PopupNotifier : Component
    {
        public event ClickHandler Click;
        public event ClickHandler Close;
        public event EventHandler MouseEnter;
        public event EventHandler MouseLeave;
        public event EventHandler OnClose;
        public event EventHandler OnShow;

        public delegate void ClickHandler(object Sender, PopUpNotifierEventArgs e);

        private PopupNotifierForm fPopup;
        private Timer tmAnimation = new Timer();
        private Timer tmWait = new Timer();

        private bool bAppearing = true;
        public bool bShouldRemainVisible = false;

        public PopupNotifier()
        {
            this.fPopup = new PopupNotifierForm(this)
            {
                TopMost = true,
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual
            };

            tmAnimation.Interval = 50;
            fPopup.CloseClick += fPopUp_CloseClick;
            fPopup.LinkClick += fPopup_LinkClick;
            tmAnimation.Tick += tmAnimation_Tick;
            tmWait.Tick += tmWait_Tick;
            fPopup.MouseEnter += fPopup_MouseEnter;
            fPopup.MouseLeave += fPopup_MouseLeave;
            
            if (ctContextMenu != null) ctContextMenu.Closed += ctContextMenu_Closed;
        }

        #region Properties

        private Color clHeader = SystemColors.ControlDark;
        [Category("Header"), DefaultValue(typeof(Color), "ControlDark")]
        public Color HeaderColor { get { return clHeader; } set { clHeader = value; } }

        private Color clBody = SystemColors.Control;
        [Category("Appearance"), DefaultValue(typeof(Color), "Control")]
        public Color BodyColor { get { return clBody; } set { clBody = value; } }

        private Color clTitle = Color.Gray;
        [Category("Title"), DefaultValue(typeof(Color), "Gray")]
        public Color TitleColor { get { return clTitle; } set { clTitle = value; } }

        private Color clBase = SystemColors.ControlText;
        [Category("Content"), DefaultValue(typeof(Color), "ControlText")]
        public Color ContentColor { get { return clBase; } set { clBase = value; } }

        private Color clBorder = SystemColors.WindowFrame;
        [Category("Appearance"), DefaultValue(typeof(Color), "WindowFrame")]
        public Color BorderColor { get { return clBorder; } set { clBorder = value; } }

        private Color clCloseBorder = SystemColors.WindowFrame;
        [Category("Buttons"), DefaultValue(typeof(Color), "WindowFrame")]
        public Color ButtonBorderColor { get { return clCloseBorder; } set { clCloseBorder = value; } }

        private Color clCloseHover = SystemColors.Highlight;
        [Category("Buttons"), DefaultValue(typeof(Color), "Highlight")]
        public Color ButtonHoverColor { get { return clCloseHover; } set { clCloseHover = value; } }

        private Color clLinkHover = SystemColors.HotTrack;
        [Category("Appearance"), DefaultValue(typeof(Color), "HotTrack")]
        public Color LinkHoverColor { get { return clLinkHover; } set { clLinkHover = value; } }

        private int iDiffGradient = 50;
        [Category("Appearance"), DefaultValue(50)]
        public int GradientPower { get { return iDiffGradient; } set { iDiffGradient = value; } }

        private Font ftBase = SystemFonts.DialogFont;
        [Category("Content")]
        public Font ContentFont { get { return ftBase; } set { ftBase = value; } }

        private Font ftTitle = SystemFonts.CaptionFont;
        [Category("Title")]
        public Font TitleFont { get { return ftTitle; } set { ftTitle = value; } }

        private Point ptImagePosition = new Point(12, 21);
        [Category("Image")]
        public Point ImagePosition { get { return ptImagePosition; } set { ptImagePosition = value; } }

        private Size szImageSize = new Size(0, 0);
        [Category("Image")]
        public Size ImageSize
        {
            get
            {
                if (szImageSize.Width == 0)
                {
                    if (Image != null) return Image.Size;
                    else return new Size(32, 32);
                }
                else return szImageSize;
            }
            set
            {
                szImageSize = value;
            }
        }

        private Image imImage = null;
        [Category("Image")]
        public Image Image { get { return imImage; } set { imImage = value; } }

        private bool bShowGrip = true;
        [Category("Header"), DefaultValue(true)]
        public bool ShowGrip { get { return bShowGrip; } set { bShowGrip = value; } }

        private string sText;
        [Category("Content")]
        public string ContentText { get { return sText; } set { sText = value; } }

        private string sTitle;
        [Category("Title")]
        public string TitleText { get { return sTitle; } set { sTitle = value; } }

        private Padding pdTextPadding = new Padding(0);
        [Category("Appearance")]
        public Padding TextPadding { get { return pdTextPadding; } set { pdTextPadding = value; } }

        private int iHeaderHeight = 9;
        [Category("Header"), DefaultValue(9)]
        public int HeaderHeight { get { return iHeaderHeight; } set { iHeaderHeight = value; } }

        private bool bCloseButtonVisible = true;
        [Category("Buttons"), DefaultValue(true)]
        public bool CloseButton { get { return bCloseButtonVisible; } set { bCloseButtonVisible = value; } }

        private bool bOptionsButtonVisible = false;
        [Category("Buttons"), DefaultValue(false)]
        public bool OptionsButton { get { return bOptionsButtonVisible; } set { bOptionsButtonVisible = value; } }

        private ContextMenuStrip ctContextMenu = null;
        [Category("Behavior")]
        public ContextMenuStrip OptionsMenu { get { return ctContextMenu; } set { ctContextMenu = value; } }

        private int iShowDelay = 3000;
        [Category("Behavior"), DefaultValue(3000)]
        public int ShowDelay { get { return iShowDelay; } set { iShowDelay = value; } }

        private Size szSize = new Size(400, 100);
        [Category("Appearance")]
        public Size Size { get { return szSize; } set { szSize = value; } }

        private int iAnimationStep = 2;
        [Category("Behavior"), DefaultValue(2)]
        public int AnimationStep { get { return iAnimationStep; } set { iAnimationStep = value; } }

        private object oTag = null;
        [Category("Content")]
        public object Tag { get { return oTag; } set { oTag = value; } }

        #endregion

        public void PopUp()
        {
            tmWait.Interval = ShowDelay;
            fPopup.Size = Size;
            fPopup.Opacity = 0;
            fPopup.Location = new Point(Screen.PrimaryScreen.WorkingArea.Right - fPopup.Size.Width - 1, Screen.PrimaryScreen.WorkingArea.Bottom);
            fPopup.Show();
            tmAnimation.Start();
        }

        private bool bMouseIsOn = false;

        private void fPopUp_CloseClick(object Sender, EventArgs e)
        {
            if (Close != null) Close(this, new PopUpNotifierEventArgs() { Tag = this.Tag });
            fPopup.Close();
        }

        private void fPopup_LinkClick(object Sender, EventArgs e)
        {
            if (Click != null) Click(this, new PopUpNotifierEventArgs() { Tag = this.Tag });
        }

        private double GetOpacityBasedOnPosition()
        {
            int iCentPourcent = fPopup.Height;
            int iCurrentlyShown = Screen.PrimaryScreen.WorkingArea.Height - fPopup.Top;
            double dPourcentOpacity = iCentPourcent / 100 * iCurrentlyShown;
            Console.WriteLine(dPourcentOpacity);
            return (dPourcentOpacity / 100) - 0.05;
        }

        private int iMaxPosition;
        private double dMaxOpacity;

        private void tmAnimation_Tick(object Sender, EventArgs e)
        {
            if (bAppearing)
            {
                fPopup.Top -= AnimationStep;
                fPopup.Opacity = GetOpacityBasedOnPosition();
                if (fPopup.Top + fPopup.Height < Screen.PrimaryScreen.WorkingArea.Bottom)
                {
                    tmAnimation.Stop();
                    bAppearing = false;
                    iMaxPosition = fPopup.Top;
                    dMaxOpacity = fPopup.Opacity;
                    tmWait.Start();
                    if (OnShow != null) OnShow(this, EventArgs.Empty);
                }
            }
            else
            {
                if (bMouseIsOn)
                {
                    fPopup.Top = iMaxPosition;
                    fPopup.Opacity = dMaxOpacity;
                    tmAnimation.Stop();
                    tmWait.Start();
                }
                else
                {
                    fPopup.Top += AnimationStep;
                    fPopup.Opacity = GetOpacityBasedOnPosition();
                    if (fPopup.Top > Screen.PrimaryScreen.WorkingArea.Bottom)
                    {
                        tmAnimation.Stop();
                        fPopup.Hide();
                        bAppearing = true;
                        if (OnClose != null) OnClose(this, EventArgs.Empty);
                    }
                }
            }
        }

        private void tmWait_Tick(object sender, EventArgs e)
        {
            tmWait.Stop();
            tmAnimation.Start();
        }

        private void fPopup_MouseEnter(object sender, EventArgs e)
        {
            bMouseIsOn = true;
            if (MouseEnter != null) MouseEnter(this, EventArgs.Empty);
        }

        private void fPopup_MouseLeave(object sender, EventArgs e)
        {
            if (!bShouldRemainVisible) bMouseIsOn = false;
            if (MouseLeave != null) MouseLeave(this, EventArgs.Empty);
        }

        private void ctContextMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            bShouldRemainVisible = false;
            bMouseIsOn = false;
            tmAnimation.Start();
        }

        public class PopUpNotifierEventArgs : EventArgs
        {
            public object Tag { get; set; }
        }

    }
}
