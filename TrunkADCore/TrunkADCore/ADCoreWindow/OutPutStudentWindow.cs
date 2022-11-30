using HZH_Controls.Forms;
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
    public partial class OutPutStudentWindow : Form
    {
        public OutPutStudentWindow()
        {
            InitializeComponent();
        }
        private string _ProjectId = "";
        private string _GroupName = "";
        private  string _ProjectName = "";

        OutPutStudentWindowSys OutPutStudentWindowSys = new OutPutStudentWindowSys();
      
        /// <summary>
        /// 导出全部成绩
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            if (OutPutStudentWindowSys.OutPutScore(_ProjectId, _ProjectName,_GroupName ,true))
            {
                FrmTips.ShowTipsSuccess(this, "导出成绩成功");
                
            }
            else
            {
                FrmTips.ShowTipsSuccess(this, "导出成绩成功");

            }
            this.Close();

        }

        private void OutPutStudentWindow_Load(object sender, EventArgs e)
        {
            _ProjectId = OutPutStudentWindowSys._projectId;
            _GroupName = OutPutStudentWindowSys.projectname;
            _ProjectName = OutPutStudentWindowSys.projectname;
            if (!String.IsNullOrEmpty(_ProjectName) && !String.IsNullOrEmpty(_GroupName))
            {
                uiLabel1.Text = $"项目:{_ProjectName},当前选择组别:{_GroupName}";
            }


        }/// <summary>
        /// 导出当前成绩
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void uiButton2_Click(object sender, EventArgs e)
        {
            if(OutPutStudentWindowSys.OutPutScore(_ProjectId, _ProjectName,_GroupName))
            {
                FrmTips.ShowTipsSuccess(this, "导出成绩成功");
            }
            else
            {
                FrmTips.ShowTipsSuccess(this, "导出成绩成功");
            }
            this.Close();
        }
    }
}
