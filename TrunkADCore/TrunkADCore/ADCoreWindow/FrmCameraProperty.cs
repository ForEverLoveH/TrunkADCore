using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Xml.Linq;
using TrunkADCore.ADCoreSystem.ADCoreSys;

namespace TrunkADCore.ADCoreWindow
{
    public partial class FrmCameraProperty : Form
    {
        FrmCameraPropertySys frmCameraPropertySys = new FrmCameraPropertySys();
        public FrmCameraProperty()
        {
            InitializeComponent();
        }
       
        public string cameraName = string.Empty;
        public int Fps = 0;
        private void FrmCameraProperty_Load(object sender, EventArgs e)
        {
            uiTitlePanel1.Text = "摄像头参数设置";
            comboBox2.SelectedIndex = 0;
            frmCameraPropertySys. LoadCameraList(CameraDrop);

        }
        
        private void CameraDrop_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CameraDrop.Text))
            {
               cameraName = CameraDrop.Text;
               frmCameraPropertySys. ChooseCamera(cameraName,comboBox1);
            }
        }
        
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(comboBox1.Text))
            {
                string cb1 = comboBox1.Text;
                string cb2 = cb1.Substring(0, cb1.IndexOf("fps"));
                int.TryParse(cb2, out Fps);
            }
        }
        private void uiButton1_Click(object sender, EventArgs e)
        {
            FrmCameraPropertySys.IsSetting = true;
            frmCameraPropertySys.SetCameraSettingBack(cameraName, Fps);
            this.Close();
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void uiTitlePanel1_Click(object sender, EventArgs e)
        {

        }
    }
}
