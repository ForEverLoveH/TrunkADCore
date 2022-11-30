using ADCoreDataCommon.GameModel;
using ADCoreDataCommon.SQLiteData;
using HZH_Controls.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrunkADCore.ADCoreSystem.ADCoreSys;


namespace TrunkADCore.ADCoreSystem 
{

    /// <summary>
    ///  成绩管理类
    /// </summary>
    public class GradeManager
    {
        public ADCoreSqlite ADCoreSqlite = null;
        /// <summary>
        /// 上传学生成绩(多线程方式）
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="aDCoreSqlite"></param>
        /// <param name="projectName"></param>
        public string UpLoadStudentByThreadFun(Object obj, ADCoreSqlite sQLiteHelper)
        {
            try
            {
                
                    List<Dictionary<string, string>> successList = new List<Dictionary<string, string>>();
                    List<Dictionary<string, string>> errorList = new List<Dictionary<string, string>>();
                    Dictionary<string, string> localInfos = new Dictionary<string, string>();
                    List<Dictionary<string, string>> list0 = sQLiteHelper.ExecuteReaderList("SELECT * FROM localInfos");
                    foreach (var item in list0)
                    {
                        localInfos.Add(item["key"], item["value"]);
                    }
                    int.TryParse(localInfos["UploadUnit"], out int UploadUnit);
                    string[] fusp = obj as string[];
                    ///项目名称
                    string projectName = string.Empty;
                    //组
                    string groupName = string.Empty;
                    if (fusp.Length > 0)
                        projectName = fusp[0];
                    if (fusp.Length > 1)
                        groupName = fusp[1];
                    Dictionary<string, string> SportProjectDic = sQLiteHelper.ExecuteReaderOne($"SELECT Id,Type,RoundCount,BestScoreMode,TestMethod," +
                             $"FloatType,TurnsNumber0,TurnsNumber1 FROM SportProjectInfos WHERE Name='{projectName}'");
                    string sql0 = $"SELECT Id,ProjectId,Name FROM DbGroupInfos WHERE ProjectId='{SportProjectDic["Id"]}' ";
                    ///查询本项目已考组
                    if (!string.IsNullOrEmpty(groupName))
                    {
                        sql0 += $" AND Name = '{groupName}'";
                    }
                    List<Dictionary<string, string>> sqlGroupsResults = sQLiteHelper.ExecuteReaderList(sql0);
                    UploadResultsRequestParameter urrp = new UploadResultsRequestParameter();
                    urrp.AdminUserName = localInfos["AdminUserName"];
                    urrp.TestManUserName = localInfos["TestManUserName"];
                    urrp.TestManPassword = localInfos["TestManPassword"];
                    string MachineCode = localInfos["MachineCode"];

                    if (MachineCode.IndexOf('_') != -1)
                    {
                        MachineCode = MachineCode.Substring(MachineCode.IndexOf('_') + 1);
                    }

                    StringBuilder messageSb = new StringBuilder();
                    StringBuilder logWirte = new StringBuilder();
                    ///按组上传
                    foreach (var sqlGroupsResult in sqlGroupsResults)
                    {
                        string sql = $"SELECT Id,GroupName,Name,IdNumber,SchoolName,GradeName,ClassNumber,State,Sex,BeginTime,FinalScore,uploadState FROM DbPersonInfos" +
                            $" WHERE ProjectId='{SportProjectDic["Id"]}' AND GroupName = '{sqlGroupsResult["Name"]}'";
                        List<Dictionary<string, string>> list = sQLiteHelper.ExecuteReaderList(sql);
                        //轮次
                        int turn = 0;
                        if (list.Count > 0)
                        {
                            Dictionary<string, string> stu = list[0];
                            int.TryParse(SportProjectDic["RoundCount"], out turn);
                            urrp.MachineCode = MachineCode;
                        }
                        else
                        {
                            continue;
                        }

                        List<SudentsItem> sudentsItems = new List<SudentsItem>();
                        //IdNumber 对应Id
                        Dictionary<string, string> map = new Dictionary<string, string>();

                        foreach (var stu in list)
                        {
                            //未参加考试的跳过
                            if (stu["State"] == "0" && stu["FinalScore"] == "-1") continue;

                            //已上传的跳过
                            if (stu["uploadState"] == "1" || stu["uploadState"] == "-1")
                            {
                                continue;
                            }
                            List<RoundsItem> roundsItems = new List<RoundsItem>();
                            ///成绩
                            List<Dictionary<string, string>> resultScoreList1 = sQLiteHelper.ExecuteReaderList(
                                $"SELECT Id,CreateTime,RoundId,State,uploadState,Result FROM ResultInfos WHERE PersonId='{stu["Id"]}' And IsRemoved=0 LIMIT {turn}");
                            #region 查询文件
                            //成绩根目录
                            Dictionary<string, string> dic_images = new Dictionary<string, string>();
                            Dictionary<string, string> dic_viedos = new Dictionary<string, string>();
                            Dictionary<string, string> dic_texts = new Dictionary<string, string>();
                            //string scoreRoot = Application.StartupPath + $"\\Scores\\{projectName}\\{stu["GroupName"]}\\";

                            #endregion

                            foreach (var item in resultScoreList1)
                            {
                                if (item["uploadState"] != "0") continue;
                                ///
                                DateTime.TryParse(item["CreateTime"], out DateTime dtBeginTime);
                                string dateStr = dtBeginTime.ToString("yyyyMMdd");
                                string GroupNo = $"{dateStr}_{stu["GroupName"]}_{stu["IdNumber"]}_{item["RoundId"]}";
                                //轮次成绩
                                RoundsItem rdi = new RoundsItem();
                                rdi.RoundId = Convert.ToInt32(item["RoundId"]);
                                rdi.State = ResultState.ResultState2Str(item["State"]);
                                rdi.Time = item["CreateTime"];
                                double.TryParse(item["Result"], out double score);
                                if (UploadUnit == 1)
                                {
                                    score *= 100;
                                }
                                rdi.Result = score;
                                //string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
                                rdi.GroupNo = GroupNo;
                                rdi.Text = dic_texts;
                                rdi.Images = dic_images;
                                rdi.Videos = dic_viedos;
                                roundsItems.Add(rdi);

                            }

                            if (roundsItems.Count == 0) continue;

                            SudentsItem ssi = new SudentsItem();
                            ssi.SchoolName = stu["SchoolName"];
                            ssi.GradeName = stu["GradeName"];
                            ssi.ClassNumber = stu["ClassNumber"];
                            ssi.Name = stu["Name"];
                            ssi.IdNumber = stu["IdNumber"];
                            ssi.Rounds = roundsItems;
                            sudentsItems.Add(ssi);
                            map.Add(stu["IdNumber"], stu["Id"]);

                        }
                        urrp.Sudents = sudentsItems;
                        //序列化json
                        string JsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(urrp);
                        string url = localInfos["Platform"] + RequestUrl.UploadResults;
                        var httpUpload = new HttpUpload();
                        var formDatas = new List<FormItemModel>();
                        //添加其他字段
                        formDatas.Add(new FormItemModel()
                        {
                            Key = "data",
                            Value = JsonStr
                        });

                        logWirte.AppendLine();
                        logWirte.AppendLine();
                        logWirte.AppendLine(JsonStr);

                        //上传学生成绩
                        string result = HttpUpload.PostForm(url, formDatas);
                        upload_Result upload_Result = Newtonsoft.Json.JsonConvert.DeserializeObject<upload_Result>(result);
                        string errorStr = "null";
                        List<Dictionary<string, int>> result1 = upload_Result.Result;
                        foreach (var item in sudentsItems)
                        {
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            //map
                            dic.Add("Id", map[item.IdNumber]);
                            dic.Add("IdNumber", item.IdNumber);
                            dic.Add("Name", item.Name);
                            dic.Add("uploadGroup", item.Rounds[0].GroupNo);
                            var value = 0;
                            result1.Find(a => a.TryGetValue(item.IdNumber, out value));
                            if (value == 1)
                            {
                                successList.Add(dic);
                            }
                            else if (value != 0)
                            {
                                errorStr = uploadResult.Match(value);
                                dic.Add("error", errorStr);
                                errorList.Add(dic);
                                messageSb.AppendLine($"{sqlGroupsResult["Name"]}组 考号:{item.IdNumber} 姓名:{item.Name}上传失败,错误内容:{errorStr}");
                            }
                        }
                    }
                    LoggerHelper.Monitor(logWirte.ToString());

                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine($"成功:{successList.Count},失败:{errorList.Count}");
                    sb.AppendLine("****************error***********************");

                    foreach (var item in errorList)
                    {
                        sb.AppendLine($"考号:{item["IdNumber"]} 姓名:{item["Name"]} 错误:{item["error"]}");
                    }
                    sb.AppendLine("*****************error**********************");


                    System.Data.SQLite.SQLiteTransaction sQLiteTransaction = sQLiteHelper.BeginTranaction();
                    sb.AppendLine("******************success*********************");
                    foreach (var item in successList)
                    {
                        string sql1 = $"UPDATE DbPersonInfos SET uploadState=1,uploadGroup='{item["uploadGroup"]}' WHERE Id={item["Id"]}";
                        sQLiteHelper.ExcuteNonQuery(sql1);
                        //更新成绩上传状态
                        sql1 = $"UPDATE ResultInfos SET uploadState=1 WHERE PersonId={item["Id"]}";
                        sQLiteHelper.ExcuteNonQuery(sql1);
                        sb.AppendLine($"考号:{item["IdNumber"]} 姓名:{item["Name"]}");
                    }
                    sQLiteHelper.CommitTransAction(sQLiteTransaction);
                    sb.AppendLine("*******************success********************");

                    string txtpath = Application.StartupPath + $"\\Log\\upload\\";
                    if (!Directory.Exists(txtpath))
                    {
                        Directory.CreateDirectory(txtpath);
                    }
                    if (successList.Count != 0 || errorList.Count != 0)
                    {
                        txtpath = Path.Combine(txtpath, $"upload_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt");
                        File.WriteAllText(txtpath, sb.ToString());
                    }
                    string outpitMessage = messageSb.ToString();
                    return outpitMessage;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="sQLiteHelper"></param>
        /// <param name="nowRound"></param>
        /// <returns></returns>
        public string UpLoadStudentByThreadFun(Object obj, ADCoreSqlite sQLiteHelper, int nowRound)
        {
            try
            {
                List<Dictionary<string, string>> successList = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> errorList = new List<Dictionary<string, string>>();
                Dictionary<string, string> localInfos = new Dictionary<string, string>();
                List<Dictionary<string, string>> list0 = sQLiteHelper.ExecuteReaderList("SELECT * FROM localInfos");
                foreach (var item in list0)
                {
                    localInfos.Add(item["key"], item["value"]);
                }
                int.TryParse(localInfos["UploadUnit"], out int UploadUnit);
                string[] fusp = obj as string[];
                ///项目名称
                string projectName = string.Empty;
                //组
                string groupName = string.Empty;
                if (fusp.Length > 0)
                    projectName = fusp[0];
                if (fusp.Length > 1)
                    groupName = fusp[1];
                Dictionary<string, string> SportProjectDic = sQLiteHelper.ExecuteReaderOne($"SELECT Id,Type,RoundCount,BestScoreMode,TestMethod," +
                         $"FloatType,TurnsNumber0,TurnsNumber1 FROM SportProjectInfos WHERE Name='{projectName}'");
                string sql0 = $"SELECT Id,ProjectId,Name FROM DbGroupInfos WHERE ProjectId='{SportProjectDic["Id"]}'";
                ///查询本项目已考组
                if (!string.IsNullOrEmpty(groupName))
                {
                    sql0 += $" AND Name = '{groupName}'";
                }
                List<Dictionary<string, string>> sqlGroupsResults = sQLiteHelper.ExecuteReaderList(sql0);
                UploadResultsRequestParameter urrp = new  UploadResultsRequestParameter();
                urrp.AdminUserName = localInfos["AdminUserName"];
                urrp.TestManUserName = localInfos["TestManUserName"];
                urrp.TestManPassword = localInfos["TestManPassword"];
                string MachineCode = localInfos["MachineCode"];

                if (MachineCode.IndexOf('_') != -1)
                {
                    MachineCode = MachineCode.Substring(MachineCode.IndexOf('_') + 1);
                }

                StringBuilder messageSb = new StringBuilder();
                StringBuilder logWirte = new StringBuilder();
                ///按组上传
                foreach (var sqlGroupsResult in sqlGroupsResults)
                {
                    string sql = $"SELECT Id,GroupName,Name,IdNumber,SchoolName,GradeName,ClassNumber,State,Sex,BeginTime,FinalScore,uploadState FROM DbPersonInfos" +
                        $" WHERE ProjectId='{SportProjectDic["Id"]}' AND GroupName = '{sqlGroupsResult["Name"]}'";

                    List<Dictionary<string, string>> list = sQLiteHelper.ExecuteReaderList(sql);
                    //轮次
                    urrp.MachineCode = MachineCode;
                    if (list.Count == 0)
                    {
                        continue;
                    }

                    StringBuilder resultSb = new StringBuilder();
                    List< SudentsItem> sudentsItems = new List< SudentsItem>();
                    //IdNumber 对应Id
                    Dictionary<string, string> map = new Dictionary<string, string>();

                    foreach (var stu in list)
                    {
                        //未参加考试的跳过
                        if (stu["State"] == "0" && stu["FinalScore"] == "-1") continue;

                        //已上传的跳过
                        if (stu["uploadState"] == "1" || stu["uploadState"] == "-1")
                        {
                            continue;
                        }
                        List <RoundsItem> roundsItems = new List< RoundsItem>();
                        ///成绩
                        List<Dictionary<string, string>> resultScoreList1 = sQLiteHelper.ExecuteReaderList(
                            $"SELECT Id,CreateTime,RoundId,State,uploadState,Result FROM ResultInfos WHERE PersonId='{stu["Id"]}' And IsRemoved=0 And RoundId={nowRound} LIMIT 1");
                        #region 查询文件
                        //成绩根目录
                        Dictionary<string, string> dic_images = new Dictionary<string, string>();
                        Dictionary<string, string> dic_viedos = new Dictionary<string, string>();
                        Dictionary<string, string> dic_texts = new Dictionary<string, string>();
                        //string scoreRoot = Application.StartupPath + $"\\Scores\\{projectName}\\{stu["GroupName"]}\\";

                        #endregion


                        foreach (var item in resultScoreList1)
                        {
                            if (item["uploadState"] != "0") continue;
                            ///
                            DateTime.TryParse(item["CreateTime"], out DateTime dtBeginTime);
                            string dateStr = dtBeginTime.ToString("yyyyMMdd");
                            string GroupNo = $"{dateStr}_{stu["GroupName"]}_{stu["IdNumber"]}_{item["RoundId"]}";
                            //轮次成绩
                            RoundsItem rdi = new  RoundsItem();
                            rdi.RoundId = Convert.ToInt32(item["RoundId"]);
                            rdi.State = ResultState.ResultState2Str(item["State"]);
                            rdi.Time = item["CreateTime"];
                            double.TryParse(item["Result"], out double score);
                            if (UploadUnit == 1)
                            {
                                score *= 100;
                            }
                            rdi.Result = score;
                            //string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
                            rdi.GroupNo = GroupNo;
                            rdi.Text = dic_texts;
                            rdi.Images = dic_images;
                            rdi.Videos = dic_viedos;
                            roundsItems.Add(rdi);
                        }
                        SudentsItem ssi = new SudentsItem();
                        ssi.SchoolName = stu["SchoolName"];
                        ssi.GradeName = stu["GradeName"];
                        ssi.ClassNumber = stu["ClassNumber"];
                        ssi.Name = stu["Name"];
                        ssi.IdNumber = stu["IdNumber"];
                        ssi.Rounds = roundsItems;
                        sudentsItems.Add(ssi);
                        map.Add(stu["IdNumber"], stu["Id"]);

                    }
                    urrp.Sudents = sudentsItems;

                    //序列化json
                    string JsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(urrp);
                    string url = localInfos["Platform"] +  RequestUrl.UploadResults;

                    var httpUpload = new HttpUpload();
                    var formDatas = new List< FormItemModel>();
                    //添加其他字段
                    formDatas.Add(new  FormItemModel()
                    {
                        Key = "data",
                        Value = JsonStr
                    });

                    logWirte.AppendLine();
                    logWirte.AppendLine();
                    logWirte.AppendLine(JsonStr);

                    //上传学生成绩
                    string result =  HttpUpload.PostForm(url, formDatas);
                    upload_Result upload_Result = Newtonsoft.Json.JsonConvert.DeserializeObject <upload_Result>(result);
                    string errorStr = "null";
                    List<Dictionary<string, int>> result1 = upload_Result.Result;
                    foreach (var item in sudentsItems)
                    {
                        Dictionary<string, string> dic = new Dictionary<string, string>();
                        //map
                        dic.Add("Id", map[item.IdNumber]);
                        dic.Add("IdNumber", item.IdNumber);
                        dic.Add("Name", item.Name);
                        dic.Add("uploadGroup", item.Rounds[0].GroupNo);
                        var value = 0;
                        result1.Find(a => a.TryGetValue(item.IdNumber, out value));
                        if (value == 1)
                        {
                            successList.Add(dic);
                        }
                        else if (value != 0)
                        {
                            errorStr =  uploadResult.Match(value);
                            dic.Add("error", errorStr);
                            errorList.Add(dic);
                            messageSb.AppendLine($"{sqlGroupsResult["Name"]}组 考号:{item.IdNumber} 姓名:{item.Name}上传失败,错误内容:{errorStr}");
                        }
                    }
                }
                LoggerHelper.Monitor(logWirte.ToString());

                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"成功:{successList.Count},失败:{errorList.Count}");
                sb.AppendLine("****************error***********************");

                foreach (var item in errorList)
                {
                    sb.AppendLine($"考号:{item["IdNumber"]} 姓名:{item["Name"]} 错误:{item["error"]}");
                }
                sb.AppendLine("*****************error**********************");
                System.Data.SQLite.SQLiteTransaction sQLiteTransaction = sQLiteHelper.BeginTranaction();


                sb.AppendLine("******************success*********************");
                foreach (var item in successList)
                {
                    //更新成绩上传状态
                    string sql1 = $"UPDATE ResultInfos SET uploadState=1 WHERE PersonId={item["Id"]} and RoundId={nowRound}";
                    sQLiteHelper.ExcuteNonQuery(sql1);
                    sb.AppendLine($"考号:{item["IdNumber"]} 姓名:{item["Name"]}");
                }
                sQLiteHelper.CommitTransAction(sQLiteTransaction);
                sb.AppendLine("*******************success********************");

                try
                {
                    string txtpath = Application.StartupPath + $"\\Log\\upload\\";
                    if (!Directory.Exists(txtpath))
                    {
                        Directory.CreateDirectory(txtpath);
                    }
                    if (successList.Count != 0 || errorList.Count != 0)
                    {
                        txtpath = Path.Combine(txtpath, $"upload_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt");
                        File.WriteAllText(txtpath, sb.ToString());
                    }
                }
                catch (Exception ex)
                {

                    LoggerHelper.Debug(ex);
                }

                string outpitMessage = messageSb.ToString();
                return outpitMessage;
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return ex.Message;
            }
        }

            /// <summary>
            /// 删除成绩
            /// </summary>
            /// <param name="listView1"></param>
            /// <param name="aDCoreSqlite"></param>
            /// <returns></returns>
        public int ClearGradeScore(ListView listView1,   ADCoreSqlite aDCoreSqlite)
        {
            ADCoreSqlite = aDCoreSqlite;
            try
            {
                if(listView1.SelectedItems.Count==0)
                {
                    MessageBox.Show("请选择一个考生");
                     return -1;
                }
                string Name = listView1.SelectedItems[0].SubItems[3].Text;
                string PersonIdNumber = listView1.SelectedItems[0].SubItems[4].Text;
                 DialogResult  dis = MessageBox .Show(  $"是否清除{Name}的成绩","提示",MessageBoxButtons.YesNo,MessageBoxIcon.Exclamation); 
                if(dis == DialogResult.Yes)
                {
                    string sql = $"DELETE FROM ResultInfos WHERE PersonIdNumber = '{PersonIdNumber}'";
                    int res = ADCoreSqlite .ExcuteNonQuery(sql);
                    sql = $"update DbPersonInfos SET State=0 where IdNumber='{PersonIdNumber}'";
                    int result1 = ADCoreSqlite.ExcuteNonQuery(sql);
                    return result1;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return -1;
            }
            return -1;

        }
        /// <summary>
        /// 上传成绩单人
        /// </summary>
        /// <param name="aDCoreSqlite"></param>
        /// <param name="projectName"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public  string UploadStudentByNumber(ADCoreSqlite sQLiteHelper, string projectName, int nums=100)
        {
            try
            {
                List<Dictionary<string, string>> successList = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> errorList = new List<Dictionary<string, string>>();
                Dictionary<string, string> localInfos = new Dictionary<string, string>();
                List<Dictionary<string, string>> list0 = sQLiteHelper.ExecuteReaderList("SELECT * FROM localInfos");
                foreach (var item in list0)
                {
                    localInfos.Add(item["key"], item["value"]);
                }
                //查询项目数据信息
                Dictionary<string, string> SportProjectDic = sQLiteHelper.ExecuteReaderOne($"SELECT Id,Type,RoundCount,BestScoreMode,TestMethod," +
                         $"FloatType,TurnsNumber0,TurnsNumber1 FROM SportProjectInfos WHERE Name='{projectName}'");
                UploadResultsRequestParameter urrp = new  UploadResultsRequestParameter();
                urrp.AdminUserName = localInfos["AdminUserName"];
                urrp.TestManUserName = localInfos["TestManUserName"];
                urrp.TestManPassword = localInfos["TestManPassword"];
                string MachineCode = localInfos["MachineCode"];
                if (MachineCode.IndexOf('_') != -1)
                {
                    MachineCode = MachineCode.Substring(MachineCode.IndexOf('_') + 1);
                }
                //轮次
                int turn = 0;
                if (SportProjectDic.Count > 0)
                {
                    int.TryParse(SportProjectDic["RoundCount"], out turn);
                    urrp.MachineCode = MachineCode;
                }
                else return "未找到项目";

                StringBuilder messageSb = new StringBuilder();
                StringBuilder logWirte = new StringBuilder();
                string sql0 = $"SELECT Id,GroupName,Name,IdNumber,SchoolName,GradeName,ClassNumber,State,Sex,BeginTime,FinalScore,uploadState FROM DbPersonInfos" +
                            $" WHERE ProjectId='{SportProjectDic["Id"]}' and FinalScore=1 ";
                List<Dictionary<string, string>> sqlStuResults = sQLiteHelper.ExecuteReaderList(sql0);
                //?  计次
                int numStep = 0;
                int maxNums = sqlStuResults.Count;
                List< SudentsItem> sudentsItems = new List< SudentsItem>();
                //IdNumber 对应Id
                Dictionary<string, string> map = new Dictionary<string, string>();
                //遍历学生
                foreach (var stu in sqlStuResults)
                {
                    numStep++;
                    //未参加考试的跳过
                    if (stu["State"] == "0" && stu["FinalScore"] == "-1")
                    {
                        continue;
                    }
                    /* //已上传的跳过
                     if (stu["uploadState"] == "1" || stu["uploadState"] == "-1")
                     {
                         continue;
                     }*/
                    List< RoundsItem> roundsItems = new List< RoundsItem>();
                    ///成绩
                    List<Dictionary<string, string>> resultScoreList1 = sQLiteHelper.ExecuteReaderList(
                        $"SELECT Id,CreateTime,RoundId,State,uploadState,Result FROM ResultInfos WHERE PersonId='{stu["Id"]}' And IsRemoved=0 LIMIT {turn}");
                    #region 查询文件
                    //成绩根目录
                    Dictionary<string, string> dic_images = new Dictionary<string, string>();
                    Dictionary<string, string> dic_viedos = new Dictionary<string, string>();
                    Dictionary<string, string> dic_texts = new Dictionary<string, string>();
                    //string scoreRoot = Application.StartupPath + $"\\Scores\\{projectName}\\{stu["GroupName"]}\\";
                    #endregion

                    //? 遍历成绩
                    foreach (var item in resultScoreList1)
                    {
                        if (item["uploadState"] != "0") continue;
                        ///
                        DateTime.TryParse(item["CreateTime"], out DateTime dtBeginTime);
                        string dateStr = dtBeginTime.ToString("yyyyMMdd");
                        string GroupNo = $"{stu["GroupName"]}_{stu["IdNumber"]}_{item["RoundId"]}";
                        //轮次成绩
                        RoundsItem rdi = new  RoundsItem();
                        rdi.RoundId = Convert.ToInt32(item["RoundId"]);
                        rdi.State = ResultState.ResultState2Str(item["State"]);
                        rdi.Time = item["CreateTime"];
                        double.TryParse(item["Result"], out double score);
                        rdi.Result = score;
                        //string.Format("{0:D2}:{1:D2}", ts.Minutes, ts.Seconds);
                        rdi.GroupNo = GroupNo;
                        rdi.Text = dic_texts;
                        rdi.Images = dic_images;
                        rdi.Videos = dic_viedos;
                        roundsItems.Add(rdi);
                    }
                    if (roundsItems.Count == 0) continue;

                    SudentsItem ssi = new SudentsItem();
                    ssi.SchoolName = stu["SchoolName"];
                    ssi.GradeName = stu["GradeName"];
                    ssi.ClassNumber = stu["ClassNumber"];
                    ssi.Name = stu["Name"];
                    ssi.IdNumber = stu["IdNumber"];
                    ssi.Rounds = roundsItems;
                    sudentsItems.Add(ssi);
                    map.Add(stu["IdNumber"], stu["Id"]);

                    //超过 限制数量就上传或者最后一人
                    if (sudentsItems.Count >= nums || numStep >= maxNums)
                    {
                        urrp.Sudents = sudentsItems;
                        //序列化json
                        string JsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(urrp);
                        string url = localInfos["Platform"] +  RequestUrl.UploadResults;
                        var httpUpload = new  HttpUpload();
                        var formDatas = new List< FormItemModel>();
                        //添加其他字段
                        formDatas.Add(new FormItemModel()
                        {
                            Key = "data",
                            Value = JsonStr
                        });
                        logWirte.AppendLine();
                        logWirte.AppendLine();
                        logWirte.AppendLine(JsonStr);
                        //上传学生成绩
                        string result =  HttpUpload.PostForm(url, formDatas);
                       upload_Result upload_Result = Newtonsoft.Json.JsonConvert.DeserializeObject< upload_Result>(result);
                        string errorStr = "null";
                        List<Dictionary<string, int>> result1 = upload_Result.Result;
                        foreach (var item in sudentsItems)
                        {
                            Dictionary<string, string> dic = new Dictionary<string, string>();
                            //map
                            dic.Add("Id", map[item.IdNumber]);
                            dic.Add("IdNumber", item.IdNumber);
                            dic.Add("Name", item.Name);
                            dic.Add("uploadGroup", item.Rounds[0].GroupNo);
                            var value = 0;
                            result1.Find(a => a.TryGetValue(item.IdNumber, out value));
                            if (value == 1)
                            {
                                successList.Add(dic);
                            }
                            else if (value != 0)
                            {
                                errorStr =  uploadResult.Match(value);
                                dic.Add("error", errorStr);
                                errorList.Add(dic);
                                messageSb.AppendLine($"考号:{item.IdNumber} 姓名:{item.Name}上传失败,错误内容:{errorStr}");
                            }
                        }
                        map.Clear();
                        sudentsItems.Clear();
                    }
                }
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"成功:{successList.Count},失败:{errorList.Count}");
                sb.AppendLine("****************error***********************");

                foreach (var item in errorList)
                {
                    sb.AppendLine($"考号:{item["IdNumber"]} 姓名:{item["Name"]} 错误:{item["error"]}");
                }
                sb.AppendLine("*****************error**********************");


                System.Data.SQLite.SQLiteTransaction sQLiteTransaction = sQLiteHelper. BeginTranaction();
                sb.AppendLine("******************success*********************");
                foreach (var item in successList)
                {
                    string sql1 = $"UPDATE DbPersonInfos SET uploadState=1,uploadGroup='{item["uploadGroup"]}' WHERE Id={item["Id"]}";
                    sQLiteHelper.ExcuteNonQuery(sql1);
                    //更新成绩上传状态
                    sql1 = $"UPDATE ResultInfos SET uploadState=1 WHERE PersonId={item["Id"]}";
                    sQLiteHelper.ExcuteNonQuery(sql1);
                    sb.AppendLine($"考号:{item["IdNumber"]} 姓名:{item["Name"]}");
                }
                sQLiteHelper.CommitTransAction(sQLiteTransaction);
                sb.AppendLine("*******************success********************");

                string txtpath = Application.StartupPath + $"\\Log\\upload\\";
                if (!Directory.Exists(txtpath))
                {
                    Directory.CreateDirectory(txtpath);
                }
                if (successList.Count != 0 || errorList.Count != 0)
                {
                    txtpath = Path.Combine(txtpath, $"upload_{DateTime.Now.ToString("yyyyMMddHHmmss")}.txt");
                    File.WriteAllText(txtpath, sb.ToString());
                }
                string outpitMessage = messageSb.ToString();
                return outpitMessage;
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return ex.Message;
            }
        }

        /// <summary>
        ///  
        /// </summary>
        ModifyStudentgradeSys  ModifyStudentgradeSys = new ModifyStudentgradeSys();
        /// <summary>
        /// 修改成绩
        /// </summary>
        /// <param name="listView1"></param>
        /// <param name="projectId"></param>
        /// <param name="projects"></param>
        /// <returns></returns>
        public  bool ModifyStudentgrade(ADCoreSqlite ADCoreSqlite, ListView listView1, string projectId, out string projects)
        {
            projects = null;
            try
            {

                if (listView1.SelectedItems.Count == 0)
                {
                    MessageBox.Show("请先选择一个考生！！");
                    return false;
                }
                else
                {
                    int index = listView1.SelectedItems[0].Index;
                    string projectNames = listView1.SelectedItems[0].SubItems[1].Text;
                    string groupName = listView1.SelectedItems[0].SubItems[2].Text;
                    string Name = listView1.SelectedItems[0].SubItems[3].Text;
                    string IdNumber = listView1.SelectedItems[0].SubItems[4].Text;
                    string status = listView1.SelectedItems[0].SubItems[5].Text;
                    int rountid = 0;
                    projects = groupName;
                    //查询项目数据信息
                    //查询项目数据信息
                    Dictionary<string, string> SportProjectDic = ADCoreSqlite.ExecuteReaderOne($"SELECT Id,Type,RoundCount,BestScoreMode,TestMethod," +
                             $"FloatType,TurnsNumber0,TurnsNumber1 FROM SportProjectInfos WHERE Name='{projectNames}'");
                    int FloatType = 0;
                    if (SportProjectDic.Count > 0)
                    {
                        FloatType = Convert.ToInt32(SportProjectDic["FloatType"]);
                        rountid = Convert.ToInt32(SportProjectDic["RoundCount"]);
                    }
                    ModifyStudentgradeSys.SetModifyStudentgradeData(projectNames, groupName, Name, IdNumber, status, rountid);
                    if (ModifyStudentgradeSys.ShowModifyStudentgrade())
                    {

                        ModifyDataback modifyDataback = ModifyStudentgradeSys.ModifyStudentgradeBackData();
                        if (modifyDataback != null)
                        {
                            int roundid = modifyDataback.roundid;
                            double updateScore = modifyDataback.updateScore;
                            decimal.Round(decimal.Parse(updateScore.ToString("0.0000")), FloatType).ToString();
                            string updatestatus = modifyDataback.updatestatus;
                            int Resultinfo_State = ResultState.ResultState2Int(updatestatus);
                            string perid = "";
                            var ds0 = ADCoreSqlite.ExecuteReaderOne($"SELECT Id FROM DbPersonInfos WHERE IdNumber='{IdNumber}' and ProjectId='{projectId}'");
                            if (ds0 == null || ds0.Count == 0) return false;
                            perid = ds0["Id"];
                            string sql = $"UPDATE ResultInfos SET Result={updateScore},State={Resultinfo_State} WHERE PersonId='{perid}' AND RoundId={roundid}";
                            int result = ADCoreSqlite.ExcuteNonQuery(sql);
                            if (result == 0)
                            {
                                if (string.IsNullOrEmpty(perid))
                                    return false;
                                else
                                {
                                    sql = $"INSERT INTO ResultInfos(CreateTime,SortId,IsRemoved,PersonId,SportItemType,PersonName,PersonIdNumber,RoundId,Result,State) " +
                                     $"VALUES (datetime(CURRENT_TIMESTAMP, 'localtime') ,(SELECT MAX(SortId)+1 FROM ResultInfos),0," +
                                        $"'{perid}',0,'{Name}','{IdNumber}',{rountid},{updateScore},{Resultinfo_State})";
                                    int result0 = ADCoreSqlite.ExcuteNonQuery(sql);
                                }
                            }
                            else if (result > 1)
                            {
                                return false;
                            }

                            return true;

                        }
                        else
                        {
                            return false;  
                        }
                    }
                    else
                    {
                        return false;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return false;
            }
        }
    }
}
