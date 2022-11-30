using ADCoreDataCommon.GameConst;
using ADCoreDataCommon.GameModel;
using ADCoreDataCommon.SQLiteData;
using HZH_Controls.Controls;
using HZH_Controls.Forms;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.ClientServices.Providers;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Xml.Linq;
using TrunkADCore.ADCoreSystem.ADCoreSys;
using TrunkADCore.ADCoreSystem;
using TrunkADCore.ADCoreSystem.MyUtils;
using TrunkADCore.ADCoreWindow;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TrunkADCore.ADCoreSystem.ADCoreSys
{
    public class Mainsys
    {
        public static Mainsys Instance;

        public void Awake()
        {
            Instance = this;
        }
        public void Init()
        {
            ShowMainWindow();
        }

        private void ShowMainWindow()
        {
            Application.Run(new MainWindow());
        }
        GradeManager gradeManager = new GradeManager();

        ADCoreSqlite ADCoreSqlite = new ADCoreSqlite();
        StartTestingSys startTestingSys = new StartTestingSys();
        ExportGradeIntoSys exportGradeIntoSys = new ExportGradeIntoSys();
        TreeView TreeView = new TreeView();
        ProjectSettingWindowSys ProjectSettingWindowSys = new ProjectSettingWindowSys();
        MainWindow MainWindow = null;
        public List<ProjectModel> ProjectTreeUpDate(out string projectId)
        {
            return TreeView.UpdateTreeView(ADCoreSqlite, out projectId);
        }
        public void ProjectTreeUpDate(List<ProjectModel> projectModels, UITreeView myTreeView)
        {
            TreeView.ProjectTreeUpDate(projectModels, myTreeView);
        }
        //轮次
        int rountCount = 0;

        //项目名称
        int floatType = 0;

        int type0 = 0;
        /// <summary>
        /// 设置开始测试信息数据
        /// </summary>
        public List<Dictionary<string, string>> SetStartTestingData(string[] fsp)
        {
            List<Dictionary<string, string>> data = ADCoreSqlite.ExecuteReaderList($"SELECT Id,Type,RoundCount,BestScoreMode,TestMethod," +
                   $"FloatType,TurnsNumber0,TurnsNumber1 FROM SportProjectInfos WHERE Name='{fsp[0]}'");
            if (data.Count == 1)
            {
                return data;
                 
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 更改成绩列显示
        /// </summary>
        /// <param name="listView1"></param>
        /// <param name="groupname"></param>
        /// <param name="projectName"></param>
        /// <param name="projectId"></param>

        public void UpdateDataGrid(System.Windows.Forms.ListView listView1, string groupname, string projectName, string projectId = null)
        {
            TreeView.UpDataListview(ADCoreSqlite, listView1, groupname, projectName, projectId);
        }




        /// <summary>
        /// 清除成绩
        /// </summary>
        /// <param name="listView1"></param>
        /// <returns></returns>
        public int ClearGradeScore(System.Windows.Forms.ListView listView1)
        {

            return gradeManager.ClearGradeScore(listView1, ADCoreSqlite);
        }
        /// <summary>
        ///  导入成绩
        /// </summary>  
        public void ExportGrade(string projectId, string treeGroupTxt, string projectName)
        {
            exportGradeIntoSys.SetExportGradeData(projectId, treeGroupTxt, projectName, ADCoreSqlite);
        }
        /// <summary>
        /// 展示导入成绩页面
        /// </summary>
        public void ShowExportGrade()
        {
            exportGradeIntoSys.ShowExportGrade();
        }
        /// <summary>
        ///展示测试页面
        /// </summary>
        public void ShowStartTestingWindow()
        {
             
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetProjectSettingData()
        {
            ProjectSettingWindowSys.SetProjectSettingData(ADCoreSqlite);
        }
        /// <summary>
        /// 
        /// </summary>
        public void ShowProjectSettingWindow()
        {
            ProjectSettingWindowSys.ShowProjectSettingWindow();
        }
        /// <summary>
        /// 初始话数据库
        /// </summary>
        public void InitSQLiteDataDB()
        {
            ADCoreSqlite.InitSQLiteDB();
        }
        /// <summary>
        /// 数据库备份
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void ExportSQLiteData()
        {
            ADCoreSqlite.ExportSQLiteDb();

        }
        /// <summary>
        ///  打开导入成绩模板
        /// </summary>
        public bool  OpenGradeTemplateExcel()
        {
            string path = Application.StartupPath + "\\模板\\导入成绩模板.xls";
            if (File.Exists(path))
            {
                Process.Start(path);
                return true;
            }
             
            return false;
             
        }
        /// <summary>
        ///  打开学生名单模板
        /// </summary>
        public bool OpenStudentTemplateExcel()
        {
            string path = Application.StartupPath + "\\模板\\导入名单模板.xls";
            if (File.Exists(path))
            {
                System.Diagnostics.Process.Start(path);
                return true;
            }
            return false;
        }

        EquipmentCodeFormSys equipmentCodeFormSys = new EquipmentCodeFormSys();
        public void SetEquipmentCodeFormSysData()
        {
            equipmentCodeFormSys.SetEquipmentCodeFormSysData(ADCoreSqlite);
        }

        public void ShowEquipmentCodeForm()
        {
            equipmentCodeFormSys.ShowEquipmentCodeWindow();
        }
        /// <summary>
        /// 上传 成绩
        /// </summary>
        /// <param name="fsp"></param>
        /// <param name="projectName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public string UpLoadStudentGrade(string[] fsp, string projectName)
        {
            if (!string.IsNullOrEmpty(projectName) && fsp.Length > 0)
            {
                if (fsp.Length > 1)
                {
                    return gradeManager.UpLoadStudentByThreadFun(fsp, ADCoreSqlite);
                }
                else
                {
                    return gradeManager.UploadStudentByNumber(ADCoreSqlite, projectName, 200);
                }
            }
            else
                return null;
        }
        OutPutStudentWindowSys outPutStudentWindowSys = new OutPutStudentWindowSys();
         

        public void OpenOutPutStudentWindow()
        {
            outPutStudentWindowSys.OpenOutPutStudentWindow();
        }


        /// <summary>
        /// 修改成绩
        /// </summary>
        /// <param name="listView1"></param>
        /// <param name="projectId"></param>
        /// <param name="projects"></param>
        /// <returns></returns>
        public bool ModifyStudentgrade(System.Windows.Forms.ListView listView1, string projectId, out string projects)
        {
            return gradeManager.ModifyStudentgrade(ADCoreSqlite, listView1, projectId, out projects);


        }
        /// <summary>
        /// 
        /// </summary>
        public void CloseSqlite()
        {
            ADCoreSqlite.CloseDb();
        }

        public  void SetStartTestingData(string[] fsp, Dictionary<string, string> dic)
        {
            startTestingSys.SetStartTestingdata(fsp, dic, ADCoreSqlite);
        }

        public  void SetOutPutStudentGrade(string projectName, string treeGroupTxt, string projectId)
        {
            outPutStudentWindowSys.SetOutPutStudentGradeData(projectName, projectId, treeGroupTxt, ADCoreSqlite);
        }
    }

         
}

