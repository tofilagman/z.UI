using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace z.UI
{
    public class PopUp : IDisposable
    {

        private NotifyIcon gIcon = new NotifyIcon();
        private ToolTip gToolTip = new ToolTip();
        private ContextMenuStrip gMenu = new ContextMenuStrip();

        private string mTitle;
        private Icon mIcon;

        public delegate void MenuHandler(object sender, MouseEventArgs e);
        public delegate void ShowMsgHandler(string str, ToolTipIcon tooltip);
        public event MenuHandler MenuHover;
        public event EventHandler IconDoubleClick;


        public PopUp(string Title, Icon icon)
        {
            this.mTitle = Title;
            this.mIcon = icon;

            gMenu.Items.Clear();
            gIcon.BalloonTipText = Title;
            gIcon.BalloonTipTitle = Title;
            gIcon.Icon = icon;
            gIcon.BalloonTipIcon = ToolTipIcon.Info;
            gIcon.Visible = true;
            gIcon.Text = Title;
            gIcon.ContextMenuStrip = gMenu;
            gIcon.MouseMove += (o, e) => {
                if (MenuHover != null) MenuHover(o, e);
            };

            gIcon.DoubleClick += (o, e) => { if(IconDoubleClick != null) IconDoubleClick (o, e); };
           // gIcon.BalloonTipClosed += (o, e) => this.Dispose();
        }
        
        public void Add(string Name, Image img, EventHandler onclick){
            gMenu.Items.Add(new ToolStripMenuItem(Name, img, onclick));
        } 

        public void Add(string Name, EventHandler onclick){
            Add(Name, null, onclick);
        }

        public void Add(string Name, Image img, ToolStripItem[] items){
            gMenu.Items.Add(new ToolStripMenuItem(Name, img, items));
        }
        
        public void Add(string Name,  ToolStripItem[] items){
            Add(Name, null, items);
        }

        public void Remove(string Name)
        {
            ToolStripItem itm = gMenu.Items.Cast<ToolStripItem>().Where(x => x.GetType() == typeof(ToolStripMenuItem) && x.Text.Replace("&", "") == Name).SingleOrDefault();
            gMenu.Items.Remove(itm);
        }

        public void AddSeparator()
        {
            gMenu.Items.Add(new ToolStripSeparator());
        }

        public void Clear(){
            gMenu.Items.Clear();
        }
        
        public void ShowMsg(string str, ToolTipIcon tooltip = ToolTipIcon.Info){
            gIcon.ShowBalloonTip(3000, gIcon.BalloonTipTitle, str, tooltip);
        }

        public void ExecuteItem(string Name)
        {
            gMenu.Items.Cast<ToolStripItem>().Where(x => x.GetType() == typeof(ToolStripMenuItem) && x.Text.Replace("&", "") == Name).SingleOrDefault().PerformClick();
        }

        public string Text {
            set{
                gIcon.Text = value;
            }
        }

        public Icon Icon{
            set{
                gIcon.Icon = value;
            } 
        }
         
        public void Dispose()
        {
            gIcon.Visible = false;
            gIcon.Icon = null;
            gMenu.Close();
            gToolTip.Dispose();
            gMenu.Dispose();
            gIcon.Dispose();
            Application.DoEvents();
            GC.Collect();
        }
    }
}
