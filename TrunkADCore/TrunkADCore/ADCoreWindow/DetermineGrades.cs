using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrunkADCore.ADCoreWindow
{
    public partial class DetermineGrades : HZH_Controls.Forms.FrmWithOKCancel1
    {
        public DetermineGrades()
        {
            InitializeComponent();
        }
        public double score = -1;
        public double checkScore = 0;
        public string dangwei = "米";

        private void DetermineGrades_Load(object sender, EventArgs e)
        {
            uiLabel3.Text = $"{score.ToString("0.000")}" + dangwei;
        }

        private void uiTextBox1_TextChanged(object sender, EventArgs e)
        {
            string stl = uiTextBox1.Text.Replace("厘米", "");
            double.TryParse(stl, out checkScore);
        }
    }
}
