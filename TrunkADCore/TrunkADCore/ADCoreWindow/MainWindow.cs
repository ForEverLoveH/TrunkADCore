using ADCoreDataCommon.GameModel;
using HZH_Controls.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrunkADCore.ADCoreSystem.ADCoreSys;
using TrunkADCore.ADCoreSystem.MyUtils;

namespace TrunkADCore.ADCoreWindow
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        Mainsys mainsys = new Mainsys();
        string projectName = "";
        string treeGroupTxt = "";
        static string projectId = string.Empty;
        
        private void MainWindow_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;

            string code = "程序集版本：" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string code1 = "文件版本：" + Application.ProductVersion.ToString();
            toolStripStatusLabel1.Text = code;
            ProjectTreeUpDate(mainsys.ProjectTreeUpDate( out projectId));
        }
        /// <summary>
        /// 项目树结构更新
        /// </summary>
        /// <param name="projectModels"></param>
        private void ProjectTreeUpDate(List<ProjectModel> projectModels)
        {
            mainsys.ProjectTreeUpDate(projectModels, MyTreeView);
        }

        private void ucNavigationMenu1_ClickItemed(object sender, EventArgs e)
        {
            string stxt = ucNavigationMenu1.SelectItem.Text;
            if (stxt == "启动测试")
            {
                var ls = MyTreeView.SelectedNode;

                if (ls != null)
                {
                    if (ls.Level == 0) 
                    { 
                        string Fullpath = MyTreeView.SelectedNode.Text;
                        if (!string.IsNullOrEmpty(Fullpath))
                        {
                            string[] fsp = Fullpath.Split('\\');
                            if (fsp.Length > 0)
                            {
                                var li = mainsys.SetStartTestingData(fsp);
                                if (li.Count == 1)
                                {

                                    Dictionary<string, string> dic = li[0];
                                    int.TryParse(dic["Type"], out int state);
                                    this.Hide();
                                    mainsys.SetStartTestingData(fsp, dic);

                                    if (!string.IsNullOrEmpty(projectName))
                                    {
                                        HZH_Controls.ControlHelper.ThreadInvokerControl(this, () =>
                                        {
                                            UpdateDataGridView(treeGroupTxt);
                                        });
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        FrmTips.ShowTipsInfo(this, "请选择项目数据，不是组数据");
                        return;
                    }
                }
                else
                {
                    MessageBox.Show("请先选择考试信息！！！");return;
                }
            }
            if(stxt == "修改成绩")
            {
                string projects = null;
                if (mainsys.ModifyStudentgrade(listView1,projectId,out projects))
                {
                    if (!string.IsNullOrEmpty(projects))
                    {
                        HZH_Controls.ControlHelper.ThreadInvokerControl(this, () =>
                        {
                            UpdateDataGridView(treeGroupTxt);
                        });
                    }
                    FrmTips.ShowTipsSuccess(this, "修改成绩成功");
                }
                else
                {
                    FrmTips.ShowTipsError(this, "修改成绩失败");
                }
            }
             
            else if (stxt == "导入成绩")
            {
                mainsys.ExportGrade(projectId, treeGroupTxt, projectName);
                mainsys.ShowExportGrade();
            }
            else if (stxt == "删除成绩")
            {
                var sl = mainsys.ClearGradeScore(listView1);
                if (sl == 1 && sl > 0)
                {
                    UpdateDataGridView(treeGroupTxt);
                    FrmTips.ShowTipsSuccess(this, "清除成功！！");
                }
                else
                {
                    FrmTips.ShowTips(this, "清除异常！！");
                }
            }

            else if (stxt == "导出成绩")
            {
                mainsys.SetOutPutStudentGrade(projectName, treeGroupTxt, projectId);
                mainsys.OpenOutPutStudentWindow();
            }
             
            else if (stxt == "项目设置")
            {
                mainsys.SetProjectSettingData();
                mainsys.ShowProjectSettingWindow();
                if (ProjectSettingWindowSys.isClose)
                {
                    MyTreeView.Nodes.Clear();
                    ProjectTreeUpDate(mainsys.ProjectTreeUpDate(out projectId));

                }

            }
            else if( stxt == "系统参数设置")
            {

            }
            else if (stxt == "初始化数据库")
            {
                mainsys.InitSQLiteDataDB();
                
                FrmTips.ShowTipsSuccess(this, "初始化数据库成功");
                HZH_Controls.ControlHelper.ThreadInvokerControl(this, () =>
                {
                    MyTreeView.Nodes.Clear();
                    ProjectTreeUpDate(mainsys.ProjectTreeUpDate( out projectId ));
                    treeGroupTxt = "";
                    mainsys.UpdateDataGrid(listView1, treeGroupTxt, projectName);   
                });
            }
            else  if(stxt == "数据库备份")
            {
                mainsys.ExportSQLiteData();
                FrmTips.ShowTipsSuccess(this, "数据库备份成功");

            }
            else if(stxt == "导入成绩模板")
            {
                if (mainsys.OpenGradeTemplateExcel())
                {
                    FrmTips.ShowTipsSuccess(this, "打开导入成绩模板成功！！！");
                    return;
                }
                else
                {
                    FrmTips.ShowTipsError(this, "打开导入成绩模板失败！！！");
                    return;
                }
            }
            else if (stxt == "导入名单模板")
            {
                if(mainsys.OpenStudentTemplateExcel())
                {
                    FrmTips.ShowTipsSuccess(this, "打开导入名单模板成功！！！");
                    return;
                }
                else
                {
                    FrmTips.ShowTipsError(this, "打开导名单模板失败！！！");
                    return;
                }
            }
            else if (stxt == "平台设备码")
            {
                mainsys.SetEquipmentCodeFormSysData();
                mainsys.ShowEquipmentCodeForm();
            }
            else if(stxt == "退出")
            {
                this.Close();
            }

            else if (stxt == "上传成绩")
            {
                ParameterizedThreadStart method = new ParameterizedThreadStart(UpLoadGrade);
                Thread thread = new Thread(method);
                thread.IsBackground = true;
                thread.Start();

            }
        }
        /// <summary>
        ///  上传成绩
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void UpLoadGrade(object obj)
        {
            var sl = MyTreeView.SelectedNode;
            if(sl == null)
            {
                return;
            }
            else
            {
                string path = MyTreeView.SelectedNode.FullPath;
                if (!string.IsNullOrEmpty(path))
                {
                    string[] fsp = path.Split('\\');
                    string projectName = string.Empty;
                    if (fsp.Length > 0)
                        projectName = fsp[0];
                    if (string.IsNullOrEmpty(projectName))
                    {
                        FrmTips.ShowTipsError(this, "请选择上传的成绩项目");
                        return;

                    }
                    string  outMessage = mainsys.UpLoadStudentGrade(fsp, projectName);
                    if (string.IsNullOrEmpty(outMessage))
                    {
                        FrmTips.ShowTipsInfo(this, "上传结束");
                    }
                    else
                    {
                        MessageBox.Show(outMessage);
                    }
                    if (!string.IsNullOrEmpty(projectName))
                    {
                        HZH_Controls.ControlHelper.ThreadInvokerControl(this, () =>
                        {
                            UpdateDataGridView(treeGroupTxt);
                        });
                        FrmTips.ShowTipsSuccess(this ,"上传成功！！");
                    }
                }
                    
            }
        }

        /// <summary>
        /// 更新表格数据
        /// </summary>
        /// <param name="Groupname"></param>
        private void UpdateDataGridView(string  Groupname)
        {
            listView1.Items.Clear();
            if (string.IsNullOrEmpty(Groupname)) return;
             mainsys.UpdateDataGrid(listView1,Groupname, projectName);
            

        }
           /// <summary>
           /// 节点点击事件
           /// </summary>
           /// <param name="sender"></param>
           /// <param name="e"></param>
        private void MyTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Level == 1)
            {
                if (e.Button == MouseButtons.Left)
                {
                    string fullPath = e.Node.FullPath; ;
                    if (!string.IsNullOrEmpty(fullPath))
                    {
                        string[] fsp = fullPath.Split('\\');
                        if (fsp.Length > 0)
                        {
                            projectName = fsp[0];
                        }
                        treeGroupTxt = e.Node.Text;
                        UpdateDataGridView(e.Node.Text);
                    }
                }
            }
            else if(e.Node.Level==0) {

                if (e.Button == MouseButtons.Left)
                {
                    projectName = e.Node.Text;
                     listView1 .Items .Clear();
                }
            }
        }

        private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            mainsys.CloseSqlite();
        }
    }
}
