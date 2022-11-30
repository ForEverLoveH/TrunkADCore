using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrunkADCore.ADCoreSystem.ADCoreSys;

namespace TrunkADCore.ADCoreWindow
{
    public partial class CreateProjectWindow : Form
    {
        public CreateProjectWindow()
        {
            InitializeComponent();
        }
        
        CreateProjectWindowSys CreateProjectWindowSys = new  CreateProjectWindowSys();
        private void uiButton1_Click(object sender, EventArgs e)
        {
            NewProject();

        }

        private void NewProject()
        {
            string sl = uiTextBox1.Text;
            if (string.IsNullOrEmpty(sl))
            {
                return;
            }
            else
            {
                CreateProjectWindowSys.projectname = sl;
                CreateProjectWindowSys.IsSucess = true;
                this.Close();
            }
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

         

        private void uiTitlePanel1_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                NewProject();
            }
        }

        private void CreateProjectWindow_Load(object sender, EventArgs e)
        {

        }
    }
}
