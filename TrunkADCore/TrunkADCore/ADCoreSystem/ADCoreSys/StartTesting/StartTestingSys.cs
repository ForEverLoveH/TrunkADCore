using ADCoreDataCommon.GameModel;
using ADCoreDataCommon.GameModel.TargetPoint;
using ADCoreDataCommon.SQLiteData;
using AForge.Controls;
using AForge.Video.DirectShow;
using HZH_Controls.Forms;
using OpenCvSharp;
using Serilog.Data;
using Sunny.UI;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CustomControl;
using SpeechLib;
using TrunkADCore.ADCoreWindow;

namespace TrunkADCore.ADCoreSystem.ADCoreSys 
{
    public class StartTestingSys
    {
        StartTestingWindow StartTestingWindow = null;
        
        public   void SetStartTestingdata(string[] fsp, Dictionary<string, string> dic, ADCoreSqlite ADCoreSqlite)
        {
            
            StartTestingWindow = new StartTestingWindow();
            StartTestingWindow.SetInitdata(fsp, dic, ADCoreSqlite);
            StartTestingWindow.ShowDialog();

        }
        string groupName=string.Empty;
        /// <summary>
        ///更新组信息
        /// </summary>
        /// <param name="groupCbx"></param>
        /// <param name="_ProjectId"></param>
        /// <param name="_GroupName"></param>
        /// <param name="ADCoreSqlite"></param>
        /// <param name="groupName"></param>
        public SQLiteDataReader UpdateGroupCombox(Sunny.UI.UIComboBox groupCbx, string _ProjectId,   ADCoreSqlite ADCoreSqlite ,String groupName ="")
        { 
            try
            {
                groupCbx.Items.Clear();
                groupCbx.Text = "";
                var ds = ADCoreSqlite.ExcuteReader($"SELECT Name FROM DbGroupInfos WHERE Name LIKE'%{groupName}%' AND ProjectId='{_ProjectId}'");
                return ds;
            }
            catch(Exception ex)
            {
                LoggerHelper.Debug(ex);
                return null;
            }
        }
        /// <summary>
        /// 更新本项目轮次次数
        /// </summary>
        /// <param name="roundCountCbx"></param>
        /// <param name="roundCount"></param>
       
        public void  UpdateRoundCountCombox(UIComboBox roundCountCbx, int roundCount)
        {
            try
            {
                roundCountCbx.Items.Clear();
                for (int i = 1; i <= roundCount; i++)
                {
                    roundCountCbx.Items.Add(i.ToString());
                }
            }
            catch (Exception ex)
            {

                LoggerHelper.Debug(ex);
            }
        }
        /// <summary>
        /// 更新名单视图
        /// </summary>
        /// <param name="stuView"></param>
        /// <param name="projectId"></param>
        /// <param name="groupName"></param>
        /// <param name="roundid"></param>
        /// <param name="ADCoreSqlite"></param>
        /// <param name="updateListViewRun"></param>
        public void UpdateListView(UIDataGridView stuView, string projectId, string groupName, int roundid, ADCoreSqlite ADCoreSqlite, out bool updateListViewRun)
        {
            updateListViewRun = true;
            try
            {
                stuView.Rows.Clear();
                var ds = ADCoreSqlite.ExecuteReaderList($"SELECT Id,Name,IdNumber FROM DbPersonInfos WHERE ProjectId='{projectId}' and GroupName='{groupName}'");
                int listviewCount = 1;
                foreach (var dic in ds)
                {
                    int k = stuView.Rows.Add(new DataGridViewRow());
                    string stuId = dic["Id"];
                    string stuName = dic["Name"];
                    string idNumber = dic["IdNumber"];
                    var ds1 = ADCoreSqlite.ExecuteReaderList($"SELECT PersonName,Result,State,uploadState FROM ResultInfos WHERE PersonId='{stuId}' AND RoundId={roundid}");
                    bool flag = true;
                    stuView.Rows[k].Cells[0].Value = listviewCount.ToString();
                    stuView.Rows[k].Cells[1].Value = idNumber;
                    stuView.Rows[k].Cells[2].Value = stuName;
                    stuView.Rows[k].Cells[6].Value = stuId;

                    foreach (var item in ds1)
                    {
                        flag = false;
                        string PersonName0 = item["PersonName"];
                        double.TryParse(item["Result"], out double Result0);
                        int.TryParse(item["State"], out int State0);
                        int.TryParse(item["uploadState"], out int uploadState0);
                        string sstate = ResultState.ResultState2Str(State0);
                        if (State0 > 1)
                        {
                            //犯规异常操作

                            stuView.Rows[k].Cells[3].Value = 0;
                            stuView.Rows[k].Cells[4].Value = sstate;
                            stuView.Rows[k].Cells[4].Style.ForeColor = Color.Red;
                        }
                        else if (State0 != 0)
                        {
                            stuView.Rows[k].Cells[3].Value = Result0.ToString();
                            stuView.Rows[k].Cells[4].Value = sstate;
                            stuView.Rows[k].DefaultCellStyle.BackColor = Color.MediumSpringGreen;
                        }
                        if (uploadState0 > 0)
                        {
                            stuView.Rows[k].Cells[5].Value = "已上传";
                        }
                        else
                        {
                            stuView.Rows[k].Cells[5].Value = "未上传";
                            stuView.Rows[k].Cells[5].Style.ForeColor = Color.Red;
                        }
                        break;
                    }

                    if (flag)
                    {
                        stuView.Rows[k].Cells[3].Value = "未测试";
                        stuView.Rows[k].Cells[4].Value = "未测试";
                        stuView.Rows[k].Cells[5].Value = "未上传";
                        stuView.Rows[k].Cells[5].Style.ForeColor = Color.Red;
                    }

                    listviewCount++;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
            finally
            {
                stuView.ClearSelection();
                updateListViewRun = false;
            }
        }
        ScreenSerialReader ScreenSerialReader = null;

        /// <summary>
        /// 初始化串口
        /// </summary>
        /// <param name="portNameSearch"></param>
        /// <param name="portNamesList"></param>
        /// <param name="openSerialPortBtn"></param>
         
        public  bool  SerialInit(UITextBox portNameSearch, UIComboBox portNamesList, UIButton openSerialPortBtn, UITextBox tb_nBaudrate)
        {
            bool flag = true ;
            try
            {
                ScreenSerialReader = new ScreenSerialReader();
                ScreenSerialReader.AnalyCallback = AnalyData;
                ScreenSerialReader.ReceiveCallback = ReceiveData;
                ScreenSerialReader.SendCallback = SendData;
                SerialTool.init();
                if (RefreshComPorts(portNameSearch, portNamesList))
                {
                    ConnectPort(openSerialPortBtn ,portNamesList,tb_nBaudrate);
                }
            }
            catch(Exception ex)
            {

            }
            return flag;


            
        }

        private void ConnectPort(UIButton openSerialPortBtn, UIComboBox portNamesList, UITextBox tb_nBaudrate)
        {
            if (ScreenSerialReader.IsComOpen())
            {
                //处理串口断开连接读写器
                ScreenSerialReader.CloseCom();
                openSerialPortBtn.Text = "打开串口";
                
            }
            else
            {
                try
                {
                    string strPort = SerialTool.PortDeviceName2PortName(portNamesList.Text);
                    if (string.IsNullOrEmpty(strPort))
                    {

                        return;
                    }

                    int.TryParse(tb_nBaudrate.Text, out int nBaudrate);
                    string strException = string.Empty;
                    int nRet = ScreenSerialReader.OpenCom(strPort, nBaudrate, out strException);
                    if (nRet == -1)
                    {
                        openSerialPortBtn.Text = "打开串口";
                        
                    }
                    else
                    {
                        openSerialPortBtn.Text = "关闭串口";
                       
                    }
                }
                catch (Exception ex)
                {

                    LoggerHelper.Debug(ex);
                }

            }
        }

            /// <summary>
            /// 刷新串口
            /// </summary>
            /// <returns></returns>
        private bool RefreshComPorts(UITextBox portNameSearch, UIComboBox portNamesList)
        {
            bool flag = false;
            try
            {
                string portFind = portNameSearch.Text;
                string[] portNames = SerialTool.getPortDeviceName(portFind);
                portNamesList.Items.Clear();
                foreach (var item in portNames)
                {
                    portNamesList.Items.Add(item);
                }
                if (portNames.Length > 0)
                {
                    flag = true;
                    portNamesList.SelectedIndex = 0;
                }
            }
            catch (Exception)
            {

                flag = false;
            }

            return flag;
        }

        private void SendData(byte[] btArySendData)
        {
             
        }

        private void ReceiveData(byte[] btAryReceiveData)
        {
            
        }

        private void AnalyData(byte[] btAryAnalyData)
        {
             
        }
        #region
        FilterInfoCollection filterInfoCollection = null;
        VideoCaptureDevice rgbDeviceVideo = null;
        int fps =  0;
        int maxfps = 0;
        int _cameraSkip = 0;

        public  bool  CameraInit(out string cameraName, out int maxFps, out int fps, out int width, out int height, out int cameraSkip)
        {
            width = 1280;
            height = 720;
            return OpenCameraSetting(out cameraName, out maxFps, out fps ,out cameraSkip);
        }

        public  bool   OpenCameraSetting(out string cameraName, out int maxFps, out int Fps, out int cameraSkip)
        {
            cameraName = String.Empty;
            maxFps = 0;
            Fps = 0;
            cameraSkip = 0;
             
            filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            FrmCameraPropertySys FrmCameraPropertySys = new FrmCameraPropertySys();
            FrmCameraPropertySys.filterInfoCollection = filterInfoCollection;
            FrmCameraPropertySys.ShowFrmCameraProperty();
            if (FrmCameraPropertySys.CheackIsSetting())
            {
                var sl = FrmCameraPropertySys.GetSettingBack();
                cameraName = sl.CameraName;
                maxFps = sl.maxFps;
                maxfps = maxFps; 
                Fps = sl.Fps;
                fps = Fps;
                if (Fps == 0)
                {
                    cameraSkip = maxFps;

                }
                else
                {
                    cameraSkip = maxFps / Fps;
                }
                if (!string.IsNullOrEmpty(cameraName))
                {
                    return true;
                }
            }
            return false;
            

        }
        
        public bool  OpenCamera(UIButton opencameraBtn, string cameraName, VideoSourcePlayer rgbVideoSource, int height, int width )
        {
            bool flag = LoadCamera(rgbVideoSource, cameraName, height, width,maxfps);
            if (!flag)
            {
                opencameraBtn.Text = "打开摄像头";
                opencameraBtn.BackColor = Color.LightSkyBlue;
                return false;

            }
            else
            {
                opencameraBtn.Text = "关闭摄像头";
                opencameraBtn.BackColor = Color.Red;
                return true;
            }
        }

        private bool LoadCamera(VideoSourcePlayer rgbVideoSource, string cameraName, int height, int width, int maxFps)
        {
            bool flag = false;
            try
            {
                if (rgbVideoSource.IsRunning)
                {
                    rgbVideoSource.SignalToStop();
                }
                Boolean findInt = false;
                foreach (FilterInfo device in filterInfoCollection)
                {
                    if (device.Name == cameraName)
                    {
                        rgbDeviceVideo = new VideoCaptureDevice(device.MonikerString);
                        if (rgbDeviceVideo.VideoCapabilities.Length == 1)
                        {
                            rgbDeviceVideo.VideoResolution = rgbDeviceVideo.VideoCapabilities[0];
                            findInt = true;

                        }
                        else
                        {
                            for (int i = 0; i < rgbDeviceVideo.VideoCapabilities.Length; i++)
                            {
                                if (rgbDeviceVideo.VideoCapabilities[i].FrameSize.Width == width
                                    && rgbDeviceVideo.VideoCapabilities[i].FrameSize.Height == height
                                    && rgbDeviceVideo.VideoCapabilities[i].AverageFrameRate == maxFps)
                                {
                                    rgbDeviceVideo.VideoResolution = rgbDeviceVideo.VideoCapabilities[i];
                                    findInt = true;
                                    break;
                                }
                            }
                        }
                        break;
                    }
                }
                if (findInt)
                {
                    rgbVideoSource.VideoSource = rgbDeviceVideo;
                    rgbVideoSource.Start();
                    rgbVideoSource.SendToBack();
                    flag = true;
                }
            }
            catch (Exception ex)
            {
                flag = false;
                LoggerHelper.Debug(ex);

            }
            return flag;
        }
        public void CloseCamera(UIButton opencameraBtn, VideoSourcePlayer rgbVideoSource)
        {
            try
            {
                if (!rgbVideoSource.IsRunning) return;
                else
                {
                    if (rgbVideoSource != null && rgbVideoSource.IsRunning)
                    {
                        rgbVideoSource.SignalToStop();
                    }
                    opencameraBtn.Text = "打开摄像头";
                    opencameraBtn.BackColor = Color.LightSkyBlue;

                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }
        #endregion
        /// <summary>
        /// 展示code 页面
        /// </summary>
        /// <param name="ADCoreSqlite"></param>
        public void ShowEquipmentCodeFrom(ADCoreSqlite ADCoreSqlite)
        {
            EquipmentCodeFormSys EquipmentCodeFormSys = new EquipmentCodeFormSys();
            EquipmentCodeFormSys.SetEquipmentCodeFormSysData(ADCoreSqlite);
            EquipmentCodeFormSys.ShowEquipmentCodeWindow();
        }
        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>

        public bool OutPutScore(string projectID, string groupname ,string _ProjectName,ADCoreSqlite ADCoreSqlite ,bool flag=false)
        {
              bool result = false;
            try
            {
                string path = Application.StartupPath + $"\\data\\excel\\";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path += $"output{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
                Dictionary<string, string> dict0 = ADCoreSqlite.ExecuteReaderOne($"SELECT RoundCount FROM SportProjectInfos WHERE Id='{projectID}' ");
                if (dict0.Count == 0)
                {
                    return false;
                }
                int.TryParse(dict0["RoundCount"], out int RoundCount);
                List<Dictionary<string, string>> ldic = new List<Dictionary<string, string>>();
                //序号 项目名称    组别名称 姓名  准考证号 考试状态    第1轮 第2轮 最好成绩
                string sql = $"SELECT BeginTime, Id, GroupName, Name, IdNumber,State,Sex FROM DbPersonInfos WHERE ProjectId='{projectID}' ";
                if (flag == false)
                {
                    sql += $" AND GroupName = '{groupName}'";

                }
                List<Dictionary<string, string>> list1 = ADCoreSqlite.ExecuteReaderList(sql);
                int step = 1;
                foreach (var item1 in list1)
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
                result = ExcelUtils.MiniExcel_OutPutExcel(path, ldic);
                //result = ExcelUtils.OutPutExcel(ldic, path);
                if (result)
                {
                    System.Diagnostics.Process p = new System.Diagnostics.Process();
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                    p.StartInfo.UseShellExecute = true;
                    p.StartInfo.FileName = path;
                    p.StartInfo.Verb = "print";
                    p.Start();
                }
                return result;
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return false;
            }
        }

       /// <summary>
       /// 上传设置
       /// </summary>
       /// <param name="fusp"></param>
       /// <param name="ADCoreSqlite"></param>
       /// <param name="roundCount0"></param>
       /// <returns></returns>
        public string UploadStudentThreadFun(string[] fusp, ADCoreSqlite ADCoreSqlite, int roundCount0)
        {
            GradeManager gradeManager = new GradeManager();
            return gradeManager.UpLoadStudentByThreadFun(fusp, ADCoreSqlite, roundCount0);
        }
        /// <summary>
        /// 写下log
        /// </summary>
        /// <param name="logRichTxt"></param>
        /// <param name="strLog"></param>
        /// <param name="nType"></param>
        public void WriteLog(LogRichTextBox logRichTxt, string strLog, int nType)
        {
            try
            {
                logRichTxt.BeginInvoke(new ThreadStart((MethodInvoker)delegate ()
                {
                    if (logRichTxt.Lines.Length > 100)
                    {
                        logRichTxt.Clear();
                    }
                    if (nType == 0)
                    {
                        logRichTxt.AppendTextEx(strLog, Color.Indigo);
                    }
                    else if (nType == 2)
                    {
                        logRichTxt.AppendTextEx(strLog, Color.Blue);
                    }
                    else if (nType == 1)
                    {
                        logRichTxt.AppendTextEx(strLog, Color.Red);
                    }
                    else if (nType == 3)
                    {
                        logRichTxt.AppendTextEx(strLog, Color.DarkGreen);
                    }

                    logRichTxt.Select(logRichTxt.TextLength, 0);
                    logRichTxt.ScrollToCaret();
                }));
            }
            catch (Exception ex)
            {

                LoggerHelper.Debug(ex);
            }
        }

        /// <summary>
        ///  是否存在学生
        /// </summary>
        /// <param name="nowRaceStudentData"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public bool IsHaveStudent(SavaStudentData nowRaceStudentData, bool flag = true )
        {
            if (nowRaceStudentData == null || string.IsNullOrEmpty(nowRaceStudentData.id))
            {
                if (flag)
                    return false;
            }
            return true;
        }
        /// <summary>
        /// rgbvideo开始
        /// </summary>
        /// <param name="rgbVideoSource"></param>
        /// <param name="cameraName"></param>
        /// <param name="opencameraBtn"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public bool rgbVideoSourceStart(VideoSourcePlayer rgbVideoSource, string cameraName, UIButton opencameraBtn, int width, int height)
        {
             
            bool flag = false;
            if (rgbVideoSource.IsRunning) return true;
            if (rgbVideoSource != null && !rgbVideoSource.IsRunning)
            {
                rgbVideoSource.Start();
                //rgbVideoSource.Show();
                return true;
            }
            if (string.IsNullOrEmpty(cameraName))
            {

                return false;
            }
            flag = OpenCamera(opencameraBtn, cameraName, rgbVideoSource, height, width);
            return flag;
        }
        /// <summary>
        /// 播放播报音
        /// </summary>
        /// <param name="str"></param>
        /// <param name="rate"></param>
        public void VoiceOut0(string str,int rate = 3)
        {
            Task.Run(() =>
            {
                SpVoice voice = new SpVoice();
                ISpeechObjectTokens obj = voice.GetVoices();
                voice.Voice = obj.Item(0);
                voice.Rate = rate;
                voice.Speak(str, SpeechVoiceSpeakFlags.SVSFIsXML | SpeechVoiceSpeakFlags.SVSFDefault);
            });
        }

        /// <summary>
        /// 发送成绩到小屏幕
        /// </summary>
        /// <param name="name">姓名</param>
        /// <param name="v1">成绩</param>
        /// <param name="v2">单位</param>
        /// <exception cref="NotImplementedException"></exception>
        public bool SendScore(string name, string v1, string v2 = "米")
        {
            if (ScreenSerialReader.IsComOpen()) return false;
            try
            {

                int c1 = 0; //红
                int c2 = 1; //绿
                int c3 = 2; //蓝
                byte[] result = SerialTool.PushText_BL2(name, c1, v1, c2, v2, c3);
                ScreenSerialReader.SendMessage(result);
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return false;
            }

            return true;
        }


        public void ImgJpgSave(Image  img , string path)
        {
            if (File.Exists(path))
                File.Delete(path);
            img.Save(path);
        }
        /// <summary>
        /// 更新跳跃的长度
        /// </summary>
        /// <param name="ADCoreSqlite"></param>
        /// <param name="nowRaceStudentData"></param>
        /// <param name="roundCount0"></param>
        /// <returns></returns>
        public bool UpdateJumpLen(ADCoreSqlite ADCoreSqlite, SavaStudentData nowRaceStudentData ,int roundCount0)
        {
            System.Data.SQLite.SQLiteTransaction sQLiteTransaction = ADCoreSqlite.BeginTranaction();
            try
            {
                var sortid = ADCoreSqlite.ExecuteScalar($"SELECT MAX(SortId) + 1 FROM ResultInfos");
                string sortid0 = "1";
                if (sortid != null && sortid.ToString() != "") sortid0 = sortid.ToString();
                int state0 = nowRaceStudentData.state == 0 ? 1 : nowRaceStudentData.state;
                string sql = $"INSERT INTO ResultInfos(CreateTime,SortId,IsRemoved,PersonId,SportItemType,PersonName,PersonIdNumber,RoundId,Result,State) " +
                             $"VALUES('{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', {sortid0}, 0, '{nowRaceStudentData.id}',0,'{nowRaceStudentData.name}'," +
                             $"'{nowRaceStudentData.idNumber}',{roundCount0},{nowRaceStudentData.score},{state0})";
                ADCoreSqlite.ExcuteNonQuery(sql);
                ADCoreSqlite.ExcuteNonQuery($"UPDATE DbPersonInfos SET State = 1, FinalScore = 1 WHERE Id = '{nowRaceStudentData.id}'");


            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                return false;

            }
            ADCoreSqlite.CommitTransAction(sQLiteTransaction);
            return true;
        }
        /// <summary>
        /// 设置考试状态
        /// </summary>
        /// <param name="stuView"></param>
        /// <param name="ADCoreSqlite"></param>
        /// <param name="roundCount0"></param>
        /// <param name="state"></param>
        public void SetErrorgrade(UIDataGridView stuView, ADCoreSqlite ADCoreSqlite, int roundCount0, string state)
        {
            if (stuView.SelectedRows.Count == 0)
            {
                return;
            }

            int state0 = ResultState.ResultState2Int(state);
            string idNum = stuView.SelectedRows[0].Cells[1].Value.ToString();
            string name = stuView.SelectedRows[0].Cells[2].Value.ToString();
            string id = stuView.SelectedRows[0].Cells[6].Value.ToString();
            string sql = $"UPDATE DbPersonInfos SET State=1,FinalScore=1 WHERE Id='{id}'";
            int result = ADCoreSqlite.ExcuteNonQuery(sql);
            sql =
                $"UPDATE ResultInfos SET State={state0} WHERE PersonId='{id}' AND RoundId={roundCount0} AND IsRemoved=0";
            result = ADCoreSqlite.ExcuteNonQuery(sql);
            if (result == 0)
            {
                var sortid = ADCoreSqlite.ExecuteScalar($"SELECT MAX(SortId) + 1 FROM ResultInfos");
                string sortid0 = "1";
                if (sortid != null && sortid.ToString() != "") sortid0 = sortid.ToString();

                sql =
                    $"INSERT INTO ResultInfos(CreateTime,SortId,IsRemoved,PersonId,SportItemType,PersonName,PersonIdNumber,RoundId,Result,State) " +
                    $"VALUES('{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}', {sortid0}, 0, '{id}',0,'{name}','{idNum}',{roundCount0},{0},{state0})";
                //处理写入数据库
                ADCoreSqlite.ExcuteNonQuery(sql);
            }
        }
    }
     

}
