using ADCoreDataCommon.GameModel;
using ADCoreDataCommon.SQLiteData;
using HZH_Controls.Controls;
using HZH_Controls.Forms;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;
using TrunkADCore.ADCoreWindow;

namespace TrunkADCore.ADCoreSystem.ADCoreSys
{
    public class ProjectSettingWindowSys
    {
        public static   ADCoreSqlite ADCoreSqlite= null;
        ProjectSettingWindow projectSettingWindow = null;
        TreeView TreeView = new TreeView();

        CreateProjectWindowSys CreateProjectWindowSys = new CreateProjectWindowSys();
        public  void SetProjectSettingData(ADCoreDataCommon.SQLiteData.ADCoreSqlite aDCoreSqlite)
        {
             ADCoreSqlite = aDCoreSqlite;   
        }

        public  void ShowProjectSettingWindow()
        {
            projectSettingWindow = new ProjectSettingWindow();
            projectSettingWindow.ShowDialog();

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public List<ProjectModel> ProjectTreeUpDate(out string projectId)
        {
            return  TreeView .UpdateTreeView(ADCoreSqlite,out projectId);
        }

        public void UpdateListView(List<ProjectModel> projectModels, Sunny.UI.UITreeView uiTreeView1)
        {
            TreeView.ProjectTreeUpDate(projectModels,uiTreeView1);  
        }
        /// <summary>
        /// 更新listview 显示
        /// </summary>
        /// <param name="listView1"></param>
        /// <param name="groupname"></param>
        /// <param name="projectName"></param>
        public  void UpdateDataGridView(System.Windows.Forms.ListView listView1, string groupname, string projectName)
        {
            TreeView.UpDataListview(ADCoreSqlite,listView1 ,groupname,projectName);
        }
        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="text"></param>
        /// <exception cref="NotImplementedException"></exception>
        public  bool  DeleteProject(string name, out string projectId)
        {
             
            projectId = null;
            var value = ADCoreSqlite.ExecuteScalar($"SELECT Id FROM SportProjectInfos WHERE Name='{name}'");
            string projectIds = value.ToString();
            projectId = projectIds;

            int result =  ADCoreSqlite.ExcuteNonQuery($"DELETE FROM SportProjectInfos WHERE Id = '{projectId}'");
            if (result == 1)
            {
                ADCoreSqlite.ExcuteNonQuery($"DELETE FROM DbGroupInfos WHERE ProjectId = '{projectId}'");
                ADCoreSqlite.ExcuteNonQuery($"DELETE FROM DbPersonInfos WHERE ProjectId = '{projectId}'");
                return true;
                 
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///   插入项目
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public  bool  CreateProject()
        {
            CreateProjectWindowSys.ShowCreateProjectWindow();
            var sl = CreateProjectWindowSys.ShowCreateProjectWindowBack();
            if (!string.IsNullOrEmpty(sl))
            {
                string NewProjectName = sl;
                string sql = $"select Id from SportProjectInfos where Name='{NewProjectName}' LIMIT 1";
                var es = ADCoreSqlite.ExecuteScalar(sql);
                int si = 1;
                var ds = ADCoreSqlite.ExecuteScalar($"SELECT MAX(SortId) + 1 FROM SportProjectInfos").ToString();
                int.TryParse(ds, out si);
                if (es != null)
                {
                    Console.WriteLine($"{NewProjectName}已存在");
                    return  false ;
                }
                else
                {
                    sql = $"INSERT INTO SportProjectInfos (CreateTime, SortId, IsRemoved, Name, Type, RoundCount, BestScoreMode, TestMethod, FloatType ) " +
                       $"VALUES(datetime(CURRENT_TIMESTAMP, 'localtime'),{si}," +
                       $"0,'{NewProjectName}',0,2,0,0,2)";
                    int result = ADCoreSqlite.ExcuteNonQuery(sql);
                    if (result == 1)
                    {
                        Console.WriteLine($"{NewProjectName}添加成功");
                        return true ;

                    }
                    else
                    {
                        return false ;
                    }
                }
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProjectName"></param>
        /// <returns></returns>
        public  ProjectSettingBack  ProjectAttributeUpdate(string ProjectName)
        {
            ProjectSettingBack ProjectSettingBack  =null;   
            var ds = ADCoreSqlite.ExcuteReader("SELECT spi.Name,spi.Type,spi.RoundCount,spi.BestScoreMode,spi.TestMethod,spi.FloatType,spi.Id " +
              $"FROM SportProjectInfos AS spi WHERE spi.Name='{ProjectName}'");
            while (ds.Read())
            {
                ProjectSettingBack = new ProjectSettingBack()
                {
                    projectID = ds.GetValue(6).ToString(),
                    Name = ds.GetString(0),
                    type = ds.GetInt16(1),
                    roundCount = ds.GetInt16(2),
                    bestScoreMode = ds.GetInt16(3),
                    TestMethod = ds.GetInt16(4),
                    FloatType = ds.GetInt16(5)
                };

            }
            return ProjectSettingBack;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupName"></param>
        public   void UpdataStudentView(string groupName, UCDataGridView view,String  projectId )
        {
            List<DataGridViewColumnEntity> lstCulumns = new List<DataGridViewColumnEntity>();
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "ID", HeadText = "序号", Width = 5, WidthType = SizeType.AutoSize });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "GroupName", HeadText = "组别名称", Width = 20, WidthType = SizeType.AutoSize });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "School", HeadText = "学校", Width = 20, WidthType = SizeType.AutoSize });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "Grade", HeadText = "年级", Width = 5, WidthType = SizeType.AutoSize });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "Class", HeadText = "班级", Width = 5, WidthType = SizeType.AutoSize });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "Name", HeadText = "姓名", Width = 20, WidthType = SizeType.AutoSize });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "Sex", HeadText = "性别", Width = 5, WidthType = SizeType.AutoSize, Format = (a) => { return ((int)a) == 0 ? "男" : "女"; } });
            lstCulumns.Add(new DataGridViewColumnEntity() { DataField = "IdNumber", HeadText = "准考证号", Width = 20, WidthType = SizeType.AutoSize });
             view.Columns =lstCulumns;
            view.IsShowCheckBox = true;
            List<object> lstSource = new List<object>();
            var ds = ADCoreSqlite.ExecuteReaderList($"SELECT d.GroupName,d.SchoolName,d.GradeName,d.ClassNumber,d.Name,d.Sex,d.IdNumber " +
              $"FROM DbPersonInfos AS d WHERE d.GroupName='{groupName}' AND d.ProjectId='{projectId}'");
            int i = 1;
            foreach (var item in ds)
            {
                DataGridViewModel model = new DataGridViewModel()
                {
                    ID = i.ToString(),
                    GroupName = item["GroupName"],
                    School = item["SchoolName"],
                    Grade = item["GradeName"],
                    Class = item["ClassNumber"] + "班",
                    Name = item["Name"],
                    Sex = Convert.ToInt32(item["Sex"]),
                    IdNumber = item["IdNumber"],

                };
                lstSource.Add(model);
                i++;
            }
            view.DataSource = lstSource;
            view.ReloadSource();
        }
        ImportStuentFormSys importStuentFormSys = new ImportStuentFormSys();
        /// <summary>
        /// 设置导入信息页面的data
        /// </summary>
        /// <param name="projectName"></param>
        /// <exception cref="NotImplementedException"></exception>
        public  void SetImportData(string projectName)
        {
            importStuentFormSys.SetImportData(ADCoreSqlite, projectName);
            importStuentFormSys.ShowImportStuentForm();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="st"></param>
        /// <param name="count"></param>
        /// <param name="ucDataGridView1"></param>
        /// <returns></returns>
        public bool DeleteChooseGroup(string st ,int count, UCDataGridView ucDataGridView1)
        {
            try
            {
                var value = ADCoreSqlite.ExecuteScalar($"SELECT Id FROM SportProjectInfos WHERE Name='{st}'");
                string projectId = value.ToString();
                if (!string.IsNullOrEmpty(projectId))
                {
                    for (int i = 0; i < count; i++)
                    {
                        DataGridViewModel osoure = (DataGridViewModel)ucDataGridView1.SelectRows[i].DataSource;
                        var vpersonId = ADCoreSqlite.ExecuteScalar($"SELECT  Id FROM DbPersonInfos WHERE ProjectId='{projectId}' and Name='{osoure.Name}' and IdNumber='{osoure.IdNumber}'");
                        //删除人
                        ADCoreSqlite.ExcuteNonQuery($"DELETE FROM DbPersonInfos WHERE Id='{vpersonId}'");
                        //删除成绩
                        ADCoreSqlite.ExcuteNonQuery($"DELETE FROM ResultInfos WHERE PersonId='{vpersonId}'");
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }

       
        public  bool DeleteAllGroup(string delGroupName, string projectNamne)
        {
            try
            {
                var value = ADCoreSqlite.ExecuteScalar($"SELECT Id FROM SportProjectInfos WHERE Name='{projectNamne}'");
                string projectId = value.ToString();
                if (!string.IsNullOrEmpty(projectId))
                {
                    ADCoreSqlite.ExcuteNonQuery($"DELETE FROM DbGroupInfos WHERE ProjectId='{projectId}' and Name='{delGroupName}'");
                    var ds = ADCoreSqlite.ExcuteReader($"SELECT Id FROM DbPersonInfos WHERE ProjectId='{projectId}' AND GroupName='{delGroupName}'");
                    while (ds.Read())
                    {
                        var vpersonId = ds.GetValue(0).ToString(); ;
                        //删除成绩
                        ADCoreSqlite.ExcuteNonQuery($"DELETE FROM ResultInfos WHERE PersonId='{vpersonId}'");
                    }
                    //删除人
                    ADCoreSqlite.ExcuteNonQuery($"DELETE FROM DbPersonInfos WHERE ProjectId='{projectId}' AND GroupName='{delGroupName}'");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        /// <summary>
        ///保存项目属性
        /// </summary>
        /// <param name="name0"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="roundCount"></param>
        /// <param name="bestScoreMode"></param>
        /// <param name="testMethod"></param>
        /// <param name="floatType"></param>
        /// <returns></returns>
        public  int  SaveProjectSetting(string name0, string name, int type, int roundCount, int bestScoreMode, int testMethod, int floatType)
        {
            try
            {
                string projectID = ADCoreSqlite.ExecuteScalar($"select Id from SportProjectInfos where Name='{name0}'").ToString();

                string sql = $"UPDATE SportProjectInfos SET Name='{name}', Type={type},RoundCount={roundCount},BestScoreMode={bestScoreMode},TestMethod={testMethod},FloatType={floatType} where Id='{projectID}'";
                int result = ADCoreSqlite.ExcuteNonQuery(sql);
                return result;
            }
            catch
            {
                return -1;
            }
        }
        public static bool isClose = false;
        public  void SetMianClose()
        {
            isClose = true;
        }
    }
}
