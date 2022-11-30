using ADCoreDataCommon.GameModel;
using ADCoreDataCommon.SQLiteData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrunkADCore.ADCoreWindow;

namespace TrunkADCore.ADCoreSystem.ADCoreSys 
{
    public class OutPutStudentWindowSys
    {
       static ADCoreSqlite ADCoreSqlite = null;
        public  static String _projectId = null;
        public   static string groupname = null;
       public  static  string projectname= null;
        OutPutStudentWindow outPutStudentWindow = null;
        /// <summary>
        /// 打开页面
        /// </summary>
        
        public  void OpenOutPutStudentWindow()
        {
             outPutStudentWindow = new OutPutStudentWindow();
             outPutStudentWindow.ShowDialog();
        }
        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="projectId"></param>
        /// <param name="treeGroupTxt"></param>
        /// <param name="aDCoreSqlite"></param>
        public  void SetOutPutStudentGradeData(string projectName, string projectId, string treeGroupTxt, ADCoreSqlite aDCoreSqlite)
        {
             ADCoreSqlite = aDCoreSqlite;
            _projectId = projectId;
            groupname = treeGroupTxt;
            projectname = projectName;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="_ProjectName"></param>
        /// <param name="groupName"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public  bool OutPutScore(string projectId , string _ProjectName, string groupName,bool flag =false)
        {
            bool result = false;
            try
            {
                SaveFileDialog saveImageDialog = new SaveFileDialog();
                saveImageDialog.Filter = "xlsx file(*.xlsx)|*.xlsx";
                saveImageDialog.RestoreDirectory = true;
                string path = Application.StartupPath + $"\\excel\\output{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                saveImageDialog.FileName = $"output_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                if (saveImageDialog.ShowDialog() == DialogResult.OK)
                {
                    path = saveImageDialog.FileName;
                    Dictionary<string, string> dict0 = ADCoreSqlite.ExecuteReaderOne($"SELECT RoundCount FROM SportProjectInfos WHERE Id='{projectId}' ");
                    if (dict0.Count == 0)
                    {
                        return false;
                    }
                    int.TryParse(dict0["RoundCount"], out int RoundCount); List<Dictionary<string, string>> ldic = new List<Dictionary<string, string>>();
                    //序号 项目名称    组别名称 姓名  准考证号 考试状态    第1轮 第2轮 最好成绩
                    string sql = $"SELECT BeginTime, Id, GroupName, Name, IdNumber,State,Sex FROM DbPersonInfos WHERE ProjectId='{projectId}' ";
                    if (!flag)
                    {
                        sql += $" AND GroupName = '{groupName}'";
                    }

                    List<Dictionary<string, string>> list1 = ADCoreSqlite.ExecuteReaderList(sql);
                    int step = 1;
                    foreach (Dictionary<string, string> item1 in list1)
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        dic.Add("序号", step + "");
                        dic.Add("项目名称", _ProjectName);
                        dic.Add("组别名称", item1["GroupName"]);
                        dic.Add("姓名", item1["Name"]);
                        dic.Add("准考证号", item1["IdNumber"]);
                        dic.Add("性别", item1["Sex"] == "0" ? "男" : "女");
                        string state0 = ResultState.ResultState2Str(item1["State"]);
                        dic.Add("考试状态", state0);
                        List<Dictionary<string, string>> list2 = ADCoreSqlite.ExecuteReaderList(
                            $"SELECT * FROM ResultInfos WHERE PersonId='{item1["Id"]}' And IsRemoved=0 ORDER BY RoundId ASC LIMIT {RoundCount}");
                        int step2 = 1;
                        double maxScore = 0;
                        foreach (var item2 in list2)
                        {
                            double.TryParse(item2["Result"], out double result0);
                            int.TryParse(item2["State"], out int state1);
                            if (result0 > maxScore) maxScore = result0;
                            if (state1 == 1)
                            {
                                dic.Add($"第{step2}轮", result0 + "");
                            }
                            else
                            {
                                dic.Add($"第{step2}轮", ResultState.ResultState2Str(state1));
                            }
                            step2++;
                        }

                        for (int i = step2; i <= RoundCount; i++)
                        {
                            dic.Add($"第{i}轮", "");
                        }
                        if (step2 > 1)
                        {
                            dic.Add($"最终成绩", maxScore + "");
                        }
                        else
                        {
                            dic.Add($"最终成绩", "");
                        }

                        ldic.Add(dic);
                        step++;
                    }
                    result = ExcelUtils.OutPutExcel(ldic, path);
                }
                return result;
            
            }
            catch(Exception ex)
            {
                return false;
            }
        }
    }
}
