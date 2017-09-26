using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace z.UI.TabControl.TabStyleProviders
{
    [System.ComponentModel.ToolboxItem(false)]
    public class TabStyleNoneProvider : TabStyleProvider
    {
        public TabStyleNoneProvider(CustomTabControl tabControl) : base(tabControl)
        {
        }

        public override void AddTabBorder(System.Drawing.Drawing2D.GraphicsPath path, System.Drawing.Rectangle tabBounds)
        {
        }
    }
}
