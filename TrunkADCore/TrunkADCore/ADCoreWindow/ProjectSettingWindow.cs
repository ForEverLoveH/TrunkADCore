using ADCoreDataCommon.GameModel;
using HZH_Controls.Controls;
using HZH_Controls.Forms;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrunkADCore.ADCoreSystem;
using TrunkADCore.ADCoreSystem.ADCoreSys;

namespace TrunkADCore.ADCoreWindow
{
    public partial class ProjectSettingWindow : Form
    {
        ProjectSettingWindowSys projectSettingWindowSys = new ProjectSettingWindowSys();
        public ProjectSettingWindow()
        {
            InitializeComponent();
        }
        static string projectId = "";
        string projectName = "";
        string treeGroupTxt = " ";

        private void ProjectSettingWindow_Load(object sender, EventArgs e)
        {
            UpdateListView( projectSettingWindowSys.ProjectTreeUpDate(out projectId));
        }
         /// <summary>
         /// 更新左侧tree显示效果
         /// </summary>
         /// <param name="projectModels"></param>
        private void UpdateListView(List<ProjectModel> projectModels)
        {
            uiTreeView1.Nodes.Clear();
            projectSettingWindowSys.UpdateListView(projectModels, uiTreeView1);
            
        }

        private void uiTreeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                if (e.Button == MouseButtons.Left)
                {
                    ProjectSettingBack projectSettingBack =   projectSettingWindowSys.ProjectAttributeUpdate(e.Node.Text);
                    txt_projectName.Text = projectSettingBack.Name;
                    txt_Type.SelectedIndex = projectSettingBack.type;
                    txt_RoundCount.SelectedIndex = projectSettingBack .roundCount;
                    txt_BestScoreMode.SelectedIndex = projectSettingBack .bestScoreMode;
                    txt_TestMethod.SelectedIndex = projectSettingBack .TestMethod;
                    txt_FloatType.SelectedIndex = projectSettingBack .FloatType;
                }
            }
            else  if(e.Node.Level == 1)
            {
                if (e.Button == MouseButtons.Left)
                {
                    txt_GroupName.Text = e.Node.Text;
                    projectSettingWindowSys.UpdataStudentView(e.Node.Text , ucDataGridView1 ,projectId);
                }
            }
        }

         

        private void uiTreeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
                uiContextMenuStrip1.Show(uiTreeView1, e.Location);
        }
        /// <summary>
        /// 插入项目事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 插入项目ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(projectSettingWindowSys.CreateProject())
            {
                uiTreeView1.Nodes.Clear();
                projectId = "    ";
                UpdateListView(projectSettingWindowSys.ProjectTreeUpDate(out projectId));
                FrmTips.ShowTipsSuccess(this, "插入成功");
                //projectSettingWindowSys.UpdateDataGridView();
                return;
            }
            else
            {
                FrmTips.ShowTipsSuccess(this, "插入失败");
                return;
            }

        }
        /// <summary>
        /// 删除项目事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 删除项目ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (uiTreeView1.SelectedNode.Level != 0)
            {
                FrmTips.ShowTipsInfo(this, "请选择一个项目");
            }
            else
            {
                if (FrmDialog.ShowDialog(this, $"是否确认删除{uiTreeView1.SelectedNode.Text}项目？", "删除确认", true) == System.Windows.Forms.DialogResult.OK)
                {
                     
                    if(projectSettingWindowSys.DeleteProject(uiTreeView1.SelectedNode.Text , out projectId))
                    {
                        uiTreeView1.Nodes.Clear();
                        ucDataGridView1.DataSource = null;
                        UpdateListView(projectSettingWindowSys.ProjectTreeUpDate(out projectId));
                        FrmTips.ShowTipsSuccess(this, "删除成功");

                    }
                    else
                    {
                        FrmTips.ShowTipsError(this, "插入失败");
                        return ;
                    }
                }
               
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton1_Click(object sender, EventArgs e)
        {
            var sl = uiTreeView1.SelectedNode;
            if (sl != null)
            {
                string projectName = txt_projectName.Text;
                if (!string.IsNullOrEmpty(projectName)) 
                {
                    projectSettingWindowSys.SetImportData(projectName);
                    if (ImportStuentFormSys.IsImport)
                    {
                        uiTreeView1.Nodes.Clear();
                        ucDataGridView1.DataSource = null;
                        projectId = "  ";
                        UpdateListView(projectSettingWindowSys.ProjectTreeUpDate(out projectId));

                    }
                }  
            }
            else
            { 
                FrmTips.ShowTipsInfo(this,"请先选择项目！！");
                return;
            }
        }
        /// <summary>
        /// 删除当前组
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton4_Click(object sender, EventArgs e)
        {
            int count = ucDataGridView1.SelectRows.Count;
            string st = txt_projectName.Text;
            if (!string.IsNullOrEmpty(st)) {
                if (count > 0)
                {
                    if (projectSettingWindowSys.DeleteChooseGroup(st, count, ucDataGridView1))
                    {
                        projectSettingWindowSys.UpdataStudentView(txt_GroupName.Text, ucDataGridView1, projectId);
                        UIMessageBox.ShowSuccess("删除成功");

                    }
                    else
                    {
                        UIMessageBox.ShowError("删除失败"); return;
                    }
                }

            }
            else
            {
                return;
            }

        }
        /// <summary>
        ///  删除全部租
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton5_Click(object sender, EventArgs e)
        {
            string delGroupName = txt_GroupName.Text;
            String projectNamne = txt_projectName.Text;
            if (!string.IsNullOrEmpty(projectNamne) && !string.IsNullOrEmpty(delGroupName))
            {
                if (projectSettingWindowSys.DeleteAllGroup(delGroupName, projectNamne))
                {
                    uiTreeView1.Nodes.Clear();
                    UpdateListView(projectSettingWindowSys.ProjectTreeUpDate(out projectId));
                    projectSettingWindowSys.UpdataStudentView(txt_GroupName.Text, ucDataGridView1, projectId);
                }
            }
        }
        /// <summary>
        ///  模板导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void uiButton2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveImageDialog = new SaveFileDialog();
            saveImageDialog.Filter = "xls files(*.xls)|*.xls|xlsx file(*.xlsx)|*.xlsx|All files(*.*)|*.*";
            saveImageDialog.RestoreDirectory = true;
            saveImageDialog.FileName = $"导出模板{DateTime.Now.ToString("yyyyMMddHHmmss")}.xls";
            string path = Application.StartupPath + "\\excel\\output.xlsx";
            if (saveImageDialog.ShowDialog() == DialogResult.OK)
            {
                path = saveImageDialog.FileName;
                File.Copy(@"./模板/导入名单模板.xlsx", path);
                FrmTips.ShowTipsSuccess(this, "导出成功");
            }
        }
        /// <summary>
        /// 保存项目设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void uiButton3_Click(object sender, EventArgs e)
        {
            var sl = uiTreeView1.SelectedNode;

            if(sl != null)
            {
                if (sl.Level == 0)
                {
                    try
                    {
                        string name0 = uiTreeView1.SelectedNode.Text;
                        string Name = txt_projectName.Text;
                        int Type = txt_Type.SelectedIndex;
                        int RoundCount = txt_RoundCount.SelectedIndex;
                        int BestScoreMode = txt_BestScoreMode.SelectedIndex;
                        int TestMethod = txt_TestMethod.SelectedIndex;
                        int FloatType = txt_FloatType.SelectedIndex;

                        var l = projectSettingWindowSys.SaveProjectSetting(name0, Name, Type, RoundCount, BestScoreMode, TestMethod, FloatType);
                        if (l == 1)
                        {
                            uiTreeView1.Nodes.Clear();
                            name0 = projectId;
                            UpdateListView(projectSettingWindowSys.ProjectTreeUpDate(out projectId));
                            FrmTips.ShowTips(this, "修改成功");


                        }
                        else
                        {
                            FrmTips.ShowTipsError(this, "修改失败"); return;
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerHelper.Debug(ex);
                        return;
                    }
                }
                else{
                    UIMessageBox.Show("当前选择的节点位置不是一级节点，请重新操作");
                    return;

                }
            }
        }

        private void ProjectSettingWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            projectSettingWindowSys.SetMianClose();
        }
    }
}
