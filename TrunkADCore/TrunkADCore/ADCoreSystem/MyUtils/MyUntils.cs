using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrunkADCore.ADCoreSystem.MyUtils
{
    public class MyUntils
    {
        /// <summary>
        /// 自动调整listview 的列宽
        /// 
        /// </summary>
        /// <param name="lv"></param>
        public   void AutoResizeColumnWidth(ListView lv)
        {
            int allWidth = lv.Width;
            int count = lv.Columns.Count;
            int MaxWidth = 0;
            Graphics graphics = lv.CreateGraphics();
            int width;
            lv.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            if (count == 0) return;
            for (int i = 0; i < count; i++)
            {
                string str = lv.Columns[i].Text;
                //MaxWidth = lv.Columns[i].Width;
                MaxWidth = 0;

                foreach (ListViewItem item in lv.Items)
                {
                    try
                    {
                        str = item.SubItems[i].Text;
                        width = (int)graphics.MeasureString(str, lv.Font).Width;
                        if (width > MaxWidth)
                        {
                            MaxWidth = width;
                        }
                    }
                    catch (Exception)
                    {

                        break;
                    }

                }
                lv.Columns[i].Width = MaxWidth;
                allWidth -= MaxWidth;
            }
            if (allWidth > count && count != 0)
            {
                allWidth /= count;
                for (int i = 0; i < count; i++)
                {
                    lv.Columns[i].Width += allWidth;
                }
            }
        }
    }
}
