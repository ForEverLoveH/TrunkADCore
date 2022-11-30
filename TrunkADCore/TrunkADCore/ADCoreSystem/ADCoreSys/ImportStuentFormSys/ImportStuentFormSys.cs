using ADCoreDataCommon.GameModel;
using ADCoreDataCommon.SQLiteData;
using MiniExcelLibs;
using Newtonsoft.Json;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using TrunkADCore.ADCoreWindow;

namespace TrunkADCore.ADCoreSystem.ADCoreSys
{
    public class ImportStuentFormSys
    {
        static ADCoreSqlite ADCoreSqlite = null;
        static string projectname;
        ImportStuentForm importStuentForm = null;
        public  static bool IsImport = false;

        /// <summary>
        ///  设置页面信息
        /// </summary>
        /// <param name="aDCoreSqlite"></param>
        /// <param name="projectName"></param>
        public void SetImportData(ADCoreSqlite aDCoreSqlite, string projectName)
        {
            ADCoreSqlite = aDCoreSqlite;
            projectname = projectName;


        }
        /// <summary>
        /// 展示页面
        /// </summary>
        public void ShowImportStuentForm()
        {
            importStuentForm = new ImportStuentForm();
            importStuentForm.ShowDialog();
            
        }

        public string GetImportStuentSysdata()
        {
            return projectname;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="localInfos"></param>
        /// <param name="localValues"></param>
        public void UpDateLocalInfo(out List<Dictionary<string, string>> localInfos, out Dictionary<string, string> localValues)
        {
            localInfos = new List<Dictionary<string, string>>();
            localValues = new Dictionary<string, string>();
            localInfos = ADCoreSqlite.ExecuteReaderList("SELECT * FROM localInfos");
            if (localInfos.Count > 0)
            {
                foreach (var item in localInfos)
                {
                    localValues.Add(item["key"], item["value"]);
                }
            }
        }
        /// <summary>
        ///数据库备份 
        /// </summary>
        public void CopyDataDB()
        {
            ADCoreSqlite.ExportSQLiteDb();
        }
        /// <summary>
        /// 清空数据库
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void CleardataDB()
        {
            ADCoreSqlite.InitSQLiteDB();

        }

        public string OpenlocalXlsxExcel()
        {
            string path = string.Empty;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;      //该值确定是否可以选择多个文件
            dialog.Title = "请选择文件";     //弹窗的标题
            dialog.InitialDirectory = Application.StartupPath + "\\";    //默认打开的文件夹的位置
            dialog.Filter = "MicroSoft Excel文件(*.xlsx)|*.xlsx";       //筛选文件
            dialog.ShowHelp = true;     //是否显示“帮助”按钮
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = dialog.FileName;
            }
            return path;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="projectName"></param>
        /// <param name="proVal"></param>
        /// <param name="proMax"></param>
        /// <returns></returns>
        public bool ExcelListInput(object obj, string projectName, out int proVal, out int proMax)
        {
            bool IsResult = false;
            proMax = 0;
            proVal = 0;
            string path = obj as string;
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    if (!string.IsNullOrEmpty(projectName))
                    {

                        string projectid = ADCoreSqlite.ExecuteScalar($"select Id from SportProjectInfos where name='{projectName}'").ToString();
                        var rows = MiniExcel.Query<ExportStudentData>(path).ToList();
                        proVal = 0;
                        proMax = rows.Count;
                        HashSet<string> sets = new HashSet<string>();
                        for (int i = 0; i < rows.Count; i++)
                        {
                            sets.Add(rows[i].GroupName);
                        }
                        List<String> rolesMarketList = new List<string>();
                        rolesMarketList.AddRange(sets);
                        System.Data.SQLite.SQLiteTransaction sQLiteTransaction = ADCoreSqlite.BeginTranaction();
                        for (int i = 0; i < rolesMarketList.Count; i++)
                        {
                            string GroupName = rolesMarketList[i];
                            string countstr = ADCoreSqlite.ExecuteScalar($"SELECT COUNT(*) FROM DbGroupInfos where ProjectId='{projectid}' and Name='{GroupName}'").ToString();
                            int.TryParse(countstr, out int count);
                            if (count == 0)
                            {
                                string groupsortidstr = ADCoreSqlite.ExecuteScalar("select MAX( SortId ) + 1 from DbGroupInfos").ToString();
                                int groupsortid = 1;
                                int.TryParse(groupsortidstr, out groupsortid);
                                string insertsql = $"INSERT INTO DbGroupInfos(CreateTime,SortId,IsRemoved,ProjectId,Name,IsAllTested) " +
                                    $"VALUES(datetime(CURRENT_TIMESTAMP, 'localtime'),{groupsortid},0,'{projectid}','{GroupName}',0)";
                                //插入组
                                ADCoreSqlite.ExcuteNonQuery(insertsql);
                            }

                        }
                        for (int i = 0; i < rows.Count; i++)
                        {
                            ExportStudentData idata = rows[i];
                            string PersonIdNumber = idata.IdNumber;
                            string name = idata.Name;
                            int Sex = idata.Sex == "男" ? 0 : 1;
                            string SchoolName = idata.School;
                            string GradeName = idata.GradeName;
                            string classNumber = idata.ClassName;
                            string GroupName = idata.GroupName;
                            string countstr = ADCoreSqlite.ExecuteScalar($"SELECT COUNT(*) FROM DbPersonInfos WHERE ProjectId='{projectid}' AND IdNumber='{PersonIdNumber}'").ToString();
                            int.TryParse(countstr, out int count);
                            if (count == 0)
                            {
                                int personsortid = 1;
                                string personsortidstr = ADCoreSqlite.ExecuteScalar("select MAX( SortId ) + 1 from DbPersonInfos").ToString();
                                int.TryParse(personsortidstr, out personsortid);
                                string insertsql = $"INSERT INTO DbPersonInfos(CreateTime,SortId,IsRemoved,ProjectId,SchoolName,GradeName,ClassNumber,GroupName,Name,IdNumber,Sex,State,FinalScore,uploadState) " +
                                    $"VALUES(datetime(CURRENT_TIMESTAMP, 'localtime'),{personsortid},0,'{projectid}','{SchoolName}','{GradeName}','{classNumber}','{GroupName}'," +
                                    $"'{name}','{PersonIdNumber}',{Sex},0,-1,0)";
                                ADCoreSqlite.ExcuteNonQuery(insertsql);

                            }
                            proVal++;
                        }
                        ADCoreSqlite.CommitTransAction(sQLiteTransaction);
                        if (rows.Count == 0)
                        {
                            IsResult = false;

                        }
                        else
                        {
                            IsResult = true;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("请选择项目！！");
                    return false;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
            if (IsResult)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        EquipmentCodeFormSys EquipmentCodeFormSys = new EquipmentCodeFormSys();
        public void SetEquipMentCodedata()
        {
            EquipmentCodeFormSys.SetEquipmentCodeFormSysData(ADCoreSqlite);

        }
        public void ShowEquipmentCodeWindow()
        {
            EquipmentCodeFormSys.ShowEquipmentCodeWindow();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="localInfos"></param>
        /// <param name="localValues"></param>
        /// <exception cref="NotImplementedException"></exception>
        public GetGroupStudent ImportStudentDataFromPlatform(List<Dictionary<string, string>> localInfos, Dictionary<string, string> localValues, string groupNums)
        {
            RequestParameter RequestParameter = new RequestParameter();
            RequestParameter.AdminUserName = localValues["AdminUserName"];
            RequestParameter.TestManUserName = localValues["TestManUserName"];
            RequestParameter.TestManPassword = localValues["TestManPassword"];
            string ExamId0 = localValues["ExamId"];
            ExamId0 = ExamId0.Substring(ExamId0.IndexOf('_') + 1);
            ExamId0 = ExamId0.Substring(ExamId0.IndexOf('_') + 1);
            string MachineCode0 = localValues["MachineCode"];
            MachineCode0 = MachineCode0.Substring(MachineCode0.IndexOf('_') + 1);
            RequestParameter.ExamId = ExamId0;
            RequestParameter.MachineCode = MachineCode0;
            RequestParameter.GroupNums = groupNums + "";
            string JsonStr = JsonConvert.SerializeObject(RequestParameter);
            string url = localValues["Platform"] + RequestUrl.GetGroupStudentUrl;
            //? 下载男
            var formDatas = new List<FormItemModel>();
            //添加其他字段
            formDatas.Add(new FormItemModel()
            {
                Key = "data",
                Value = JsonStr
            });
            var httpUpload = new HttpUpload();
            string result = HttpUpload.PostForm(url, formDatas);
            GetGroupStudent upload_Result = JsonConvert.DeserializeObject<GetGroupStudent>(result);
            GetGroupStudent studentList = new GetGroupStudent();
            studentList.Results = new Results();
            studentList.Results.groups = new List<GroupsItem>();
            if (upload_Result.Results.groups.Count == 0)
            {
                UIMessageBox.Show($"男生组提交错误,错误码:[{upload_Result.Error}]");
                return null;
            }
            else
            {
                studentList.Results.groups.AddRange(upload_Result.Results.groups);

            }
            if (studentList.Results.groups.Count > 0)
            {
                return studentList;
            }
            else
            {
                return null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ls"></param>
        /// <param name="proVal"></param>
        /// <param name="proMax"></param>
        /// <returns></returns>

        public List<InputData> DownlistOutputExcel(GetGroupStudent ls, out int proVal, out int proMax)
        {
            List<GroupsItem> Groups = ls.Results.groups;
            List<InputData> doc = new List<InputData>();
            int step = 1;
            proVal = 0;
            proMax = 0;
            foreach (var group in Groups)
            {
                string groupID = group.GroupId;
                string groupName = group.GroupName;
                foreach (var studentInfo in group.StudentInfos)
                {
                    InputData data = new InputData();
                    data.Id = step;
                    data.School = studentInfo.SchoolName;
                    data.GradeName = studentInfo.GradeName;
                    data.Name = studentInfo.Name;
                    data.Sex = studentInfo.Sex;
                    data.IdNumber = studentInfo.IdNumber;
                    data.GroupName = groupID;
                    doc.Add(data);
                    step++;
                }
            }
            return doc;
        }
    }
}
