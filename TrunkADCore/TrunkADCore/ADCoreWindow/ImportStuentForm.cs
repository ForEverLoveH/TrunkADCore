using ADCoreDataCommon.GameModel;
using HZH_Controls.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.XPath;
using TrunkADCore.ADCoreSystem.ADCoreSys;

namespace TrunkADCore.ADCoreWindow
{
    public partial class ImportStuentForm : Form
    {
        public ImportStuentForm()
        {
            InitializeComponent();
        }
        static  List<Dictionary<string, string>> localInfos = null; 
        ImportStuentFormSys ImportStuentFormSys = new ImportStuentFormSys();
        string projectName = null   ;
        static Dictionary<string, string> localValues  = null;
        int proVal = 0;
        int proMax = 0;

        private void ImportStuentForm_Load(object sender, EventArgs e)
        {
            projectName = ImportStuentFormSys.GetImportStuentSysdata();
            ImportStuentFormSys.UpDateLocalInfo(out localInfos, out localValues);
        }
        /// <summary>
        /// 数据库备份
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            ImportStuentFormSys.CopyDataDB();
            FrmTips.ShowTipsSuccess(this, "备份成功 ");
        }
        /// <summary>
        /// 清空数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton2_Click(object sender, EventArgs e)
        {
            ImportStuentFormSys.CleardataDB();
                FrmTips.ShowTipsSuccess(this, "初始化成功 ");
        }
        /// <summary>
        /// 模板导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton3_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveImageDialog = new SaveFileDialog();
            saveImageDialog.Filter = "xls files(*.xls)|*.xls|xlsx file(*.xlsx)|*.xlsx|All files(*.*)|*.*";
            saveImageDialog.RestoreDirectory = true;
            saveImageDialog.FileName = $"导出模板{DateTime.Now.ToString("yyyyMMddHHmmss")}.xls";
            string path = Application.StartupPath + "\\excel\\output.xlsx";

            if (saveImageDialog.ShowDialog() == DialogResult.OK)
            {
                path = saveImageDialog.FileName;
                File.Copy(@"./模板/导入名单模板1.xlsx", path);
                FrmTips.ShowTipsSuccess(this, "导出成功");
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton4_Click(object sender, EventArgs e)
        {
            string path = ImportStuentFormSys.OpenlocalXlsxExcel();
            if(ImportStuentFormSys.ExcelListInput(path,projectName , out proVal,out proMax))
            {
                FrmTips.ShowTipsSuccess(this, "导入成功");
                ImportStuentFormSys.IsImport = true;
            }
            else
            {
                FrmTips.ShowTipsError(this, "导入失败");
            }

        }
        /// <summary>
        /// 计时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (proMax != 0)
            {
                uiProcessBar1.Maximum = proMax;
                if (proVal > proMax)
                {
                    proVal = proMax;
                    timer1.Stop();
                }
                uiProcessBar1.Value = proVal;
            }
        }
         /// <summary>
         /// 
         /// </summary>
         /// <param name="sender"></param>
         /// <param name="e"></param>
        private void uiButton5_Click(object sender, EventArgs e)
        {
            ImportStuentFormSys.SetEquipMentCodedata();
            ImportStuentFormSys.ShowEquipmentCodeWindow();
            ImportStuentFormSys.UpDateLocalInfo(out localInfos, out localValues);
        }
         

        /// <summary>
        /// 平台名单导入
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton6_Click(object sender, EventArgs e)
        {
            string nums = uiTextBox1.Text;
            if (!string.IsNullOrEmpty(nums))
            {
                var ls = ImportStuentFormSys.ImportStudentDataFromPlatform(localInfos, localValues, nums);
                var sls  =ImportStuentFormSys.DownlistOutputExcel(ls, out proVal, out proMax);
                string path = Application.StartupPath + $"\\模板\\下载名单\\downList{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                ExcelUtils.MiniExcel_OutPutExcel(path, sls);
                ImportStuentFormSys.ExcelListInput(path ,projectName,out proVal  , out proMax); 
                    
            }
            else
                return;

        }
    }
}
