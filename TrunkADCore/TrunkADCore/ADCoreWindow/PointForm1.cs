using System.Runtime.InteropServices;
using System;
using System.Windows.Forms;
using System.Drawing;

namespace TrunkADCore.ADCoreWindow
{
    public partial class PointForm1_ : Form
    {
        public PointForm1_()
        {
            InitializeComponent();
        }

        //GetActiveWindow返回线程的活动窗口，而不是系统的活动窗口。如果要得到用户正在激活的窗口，应该使用 GetForegroundWindow
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll ")]
        //设置窗体置顶
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        //窗体句柄
        private IntPtr handle = IntPtr.Zero;
        public IntPtr Handle { get => handle; set => handle = value; }

        #region   窗体在最前
        [DllImport("user32")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32")]
        private static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        #endregion
        public int columSum = 5;
        PictureBox[] pics1;
        PictureBox[] pics2;

        private void PointForm_Load(object sender, System.EventArgs e)
        {
            this.TopMost = true;
            InitPicture();
        }

        private void InitPicture()
        {
            int width = flp1.Width / columSum - 10;
            int height = flp1.Height - 10;
            flp1.SuspendLayout();
            flp2.SuspendLayout();
            flp1.Controls.Clear();
            flp2.Controls.Clear();
            pics1 = new PictureBox[columSum];
            pics2 = new PictureBox[columSum];
            for (int i = 0; i < columSum; i++)
            {
                pics1[i] = new PictureBox();
                pics1[i].Size = new System.Drawing.Size(width, height);
                pics1[i].BackColor = Color.Green;
                pics2[i] = new PictureBox();
                pics2[i].Size = new System.Drawing.Size(width, height);
                pics2[i].BackColor = Color.Green;

            }

            flp1.Controls.AddRange(pics1);
            flp2.Controls.AddRange(pics2);
            flp1.ResumeLayout();
            flp2.ResumeLayout();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void updateFlp(int index)
        {
            InitPicture();

            if (index % 2 == 0)
            {
                //下标点
                int downIndex = (index - 2) / 2;
                pics2[downIndex].BackColor = Color.Red;


            }
            else
            {
                //上标点
                int downIndex = (index - 1) / 2;
                pics1[downIndex].BackColor = Color.Red;

            }
            flp1.Controls.AddRange(pics1);
            flp2.Controls.AddRange(pics2);
            flp1.ResumeLayout();
            flp2.ResumeLayout();
        }
    }

}