using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Windows.Forms;
using TrunkADCore.ADCoreSystem.ADCoreSys;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TrunkADCore.ADCoreWindow
{
    public partial class ModifyStudentgradeWindow : Form
    {
        public ModifyStudentgradeWindow()
        {
            InitializeComponent();
        }
        private string projectName =string.Empty;
        private string groupName = string .Empty;
        private string Name = string.Empty;
        private string IdNumber = string.Empty ;
        private string status = string.Empty;
        private double updatescore = 0;
         
        ModifyStudentgradeSys ModifyStudentgradeSys  = new ModifyStudentgradeSys();


        private void ModifyStudentgradeWindow_Load(object sender, EventArgs e)
        {
            GetModifyStudentgradeInit();
            SetInitData();
        }

        private void SetInitData()
        {
            projectnameinput.Text = projectName;
            nameinput.Text = Name;
            groupinput.Text = groupName;
            uiComboBox1.Items.Clear();
            for (int i = 0; i < rount; i++)
            {
                uiComboBox1.Items.Add($"第{i + 1}轮");
            }
            if(uiComboBox2.Items.Contains(status))
            {
                uiComboBox2.SelectedIndex =uiComboBox2.Items.IndexOf(status);
            }
            uiComboBox1.SelectedIndex = 0;
            uiComboBox2.SelectedIndex = 0;


        }
        int rount = 0;
        /// <summary>
        /// 
        /// </summary>
        private void GetModifyStudentgradeInit()
        {
            projectName = ModifyStudentgradeSys._projectNames;
            groupName = ModifyStudentgradeSys._groupName;
            Name = ModifyStudentgradeSys._name;
            IdNumber = ModifyStudentgradeSys._idNumber;
            status = ModifyStudentgradeSys._status;
            rount = ModifyStudentgradeSys._roundid;

        }
        /// <summary>
        /// 确定按钮点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(uiTextBox1.Text))
            {
                 double.TryParse(uiTextBox1.Text, out updatescore);
                 ModifyStudentgradeSys.SetModifyBackdata(updaterountId, updatescore,  status);
                 ModifyStudentgradeSys.IsSucess = true;
                this.Close();
            }
            else
            {
                MessageBox.Show("请填写好考生成绩数据！！");
                return;
            }
        }

        private void uiButton2_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = DialogResult.Cancel;
            this.Close();
        }
        private int updaterountId = 0;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (uiComboBox1.SelectedIndex != -1)
            {
                updaterountId = uiComboBox1.SelectedIndex + 1;
            }
        }
         
       
         
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            status = uiComboBox2.Text.ToString();
           
        }
    }
}
