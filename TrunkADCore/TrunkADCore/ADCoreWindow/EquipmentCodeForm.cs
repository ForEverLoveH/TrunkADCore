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
using ADCoreDataCommon.SQLiteData;
using TrunkADCore.ADCoreSystem.ADCoreSys;

namespace TrunkADCore.ADCoreWindow
{
    public partial class EquipmentCodeForm : Form
    {
        public EquipmentCodeForm()
        {
            InitializeComponent();
        }
         
        private Dictionary<string, string> localValues = null;
        private List<Dictionary<string, string>> localInfos =  null;

        private EquipmentCodeFormSys _equipmentCodeFormSys = new EquipmentCodeFormSys();
        private void EquipmentCodeForm_Load(object sender, EventArgs e)
        {
            _equipmentCodeFormSys.InitCodeData(out localValues, out localInfos , comboBox2,comboBox3,comboBox1,comboBox4);
            
        }
        /// <summary>
        ///  保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton4_Click(object sender, EventArgs e)
        {
            string Platform = comboBox2.Text;
            string ExamID = comboBox3.Text;
            string code = comboBox1.Text;
            int uploadUn = comboBox4.SelectedIndex;
            if(_equipmentCodeFormSys.SaveEquipMentCode(Platform , ExamID ,code, uploadUn))
            {
                FrmTips.ShowTipsSuccess(this, "保存成功！！");
               this.Close();
            }
            else
            {
                FrmTips.ShowTipsError(this, "保存失败");
                return;
            }
        }
        /// <summary>
        /// 获取考试id
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            _equipmentCodeFormSys.GetExaminationCode(comboBox2,comboBox3 ,  localValues);
        }
        /// <summary>
        ///  获取机器id
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void uiButton2_Click(object sender, EventArgs e)
        {
            comboBox1 .Items.Clear();
            _equipmentCodeFormSys.GetEquipmentCode(comboBox1 ,comboBox3 ,comboBox2, localValues);
        }
         /// <summary>
         ///  退出
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
         /// <exception cref="NotImplementedException"></exception>
        private void uiButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
