using ADCoreDataCommon.SQLiteData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sunny.UI;
using TrunkADCore.ADCoreWindow;
using ADCoreDataCommon.GameModel;
using Newtonsoft.Json;
using HZH_Controls.Forms;

namespace TrunkADCore.ADCoreSystem.ADCoreSys 
{
    public class EquipmentCodeFormSys
    {
        static ADCoreSqlite ADCoreSqlite = null;
        EquipmentCodeForm EquipmentCodeForm = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="aDCoreSqlite"></param>
        public void SetEquipmentCodeFormSysData(ADCoreSqlite aDCoreSqlite)
        {
            ADCoreSqlite = aDCoreSqlite;
        }
        public void ShowEquipmentCodeWindow()
        {
            EquipmentCodeForm = new EquipmentCodeForm();
            EquipmentCodeForm.ShowDialog();
        }



        /// <summary>
        /// 获取考试id
        /// </summary>
        /// <param name="comboBox1"></param>
        /// <param name="comboBox2"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void GetExaminationCode(UIComboBox comboBox1, UIComboBox comboBox2, Dictionary<string, string> localValues)
        {

            comboBox1.Items.Clear();
            string url = comboBox2.Text;
            if (string.IsNullOrEmpty(url))
            {
                return;
            }
            url += RequestUrl.GetExamListUrl;
            RequestParameter RequestParameter = new RequestParameter();
            RequestParameter.AdminUserName = localValues["AdminUserName"];
            RequestParameter.TestManUserName = localValues["TestManUserName"];
            RequestParameter.TestManPassword = localValues["TestManPassword"];
            //序列化
            string JsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(RequestParameter);
            var formDatas = new List<FormItemModel>();
            //添加其他字段
            formDatas.Add(new FormItemModel()
            {
                Key = "data",
                Value = JsonStr
            });
            var httpUpload = new HttpUpload();
            string res = HttpUpload.PostForm(url, formDatas);
            GetExamList upload_Result = JsonConvert.DeserializeObject<GetExamList>(res);
            if (upload_Result.Results.Count == 0)
            {
                UIMessageBox.Show($"提交错误,错误码:[{upload_Result.Error}]");
                return;
            }
            foreach (var item in upload_Result.Results)
            {
                string str = $"{item.title}_{item.exam_id}";
                comboBox1.Items.Add(str);
            }

        }
        /// <summary>
        ///  获取机器码
        /// </summary>
        /// <param name="comboBox3"></param>
        /// <param name="comboBox2"></param>
        /// <param name="localValues"></param>
        public void GetEquipmentCode(UIComboBox comboBox3, UIComboBox comboBox2, UIComboBox comboBox1, Dictionary<string, string> localValues)
        {
            string examid = comboBox2.Text;
            if (string.IsNullOrEmpty(examid))
            {
                UIMessageBox.ShowError("考试id为空");
                return;
            }
            if (examid.IndexOf('_') != -1)
            {
                examid = examid.Substring(examid.IndexOf('_') + 1);
            }
            string url = comboBox1.Text;
            if (string.IsNullOrEmpty(url))
            {
                UIMessageBox.ShowError("网址为空！！");
                return;
            }

            url += RequestUrl.GetMachineCodeListUrl;
            RequestParameter RequestParameter = new RequestParameter();
            RequestParameter.AdminUserName = localValues["AdminUserName"];
            RequestParameter.TestManUserName = localValues["TestManUserName"];
            RequestParameter.TestManPassword = localValues["TestManPassword"];
            RequestParameter.ExamId = examid;
            //序列化
            string JsonStr = Newtonsoft.Json.JsonConvert.SerializeObject(RequestParameter);

            var formDatas = new List<FormItemModel>();
            //添加其他字段
            formDatas.Add(new FormItemModel()
            {
                Key = "data",
                Value = JsonStr
            });
            var httpUpload = new HttpUpload();
            string result = HttpUpload.PostForm(url, formDatas);
            GetMachineCodeList upload_Result = JsonConvert.DeserializeObject<GetMachineCodeList>(result);
            if (upload_Result.Results.Count == 0)
            {
                UIMessageBox.ShowError($"提交错误,错误码:[{upload_Result.Error}]");
                return;
            }

            foreach (var item in upload_Result.Results)
            {
                string str = $"{item.title}_{item.MachineCode}";
                comboBox3.Items.Add(str);

            }
        }
        /// <summary>
        ///  保存
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="examID"></param>
        /// <param name="code"></param>
        /// <param name="uploadUn"></param>
        /// <returns></returns>
        public bool SaveEquipMentCode(string platform, string examID, string code, int uploadUn)
        {
            try
            {
                System.Data.SQLite.SQLiteTransaction sQLiteTransaction = ADCoreSqlite.BeginTranaction();
                ADCoreSqlite.ExcuteNonQuery($"UPDATE localInfos SET value = '{platform}' WHERE key = 'Platform'");
                ADCoreSqlite.ExcuteNonQuery($"UPDATE localInfos SET value = '{examID}' WHERE key = 'ExamId'");
                ADCoreSqlite.ExcuteNonQuery($"UPDATE localInfos SET value = '{code}' WHERE key = 'MachineCode'");
                ADCoreSqlite.ExcuteNonQuery($"UPDATE localInfos SET value = '{uploadUn}' WHERE key = 'UploadUnit'");
                ADCoreSqlite.CommitTransAction(sQLiteTransaction);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void InitCodeData(out Dictionary<string, string> localValues, out List<Dictionary<string, string>> localInfos, UIComboBox comboBox2, UIComboBox comboBox3, UIComboBox comboBox1, UIComboBox comboBox4)
        {
            localInfos = new List<Dictionary<string, string>>();
            localValues = new Dictionary<string, string>();
            localInfos = ADCoreSqlite.ExecuteReaderList("SELECT * FROM localInfos");
            if (localInfos.Count > 0)
            {
                string MachineCode = String.Empty;
                string ExamId = String.Empty;
                string Platform = string.Empty;
                string Platforms = String.Empty;
                int UploadUnit = 0;

                foreach (var item in localInfos)
                {
                    localValues.Add(item["key"], item["value"]);
                    switch (item["key"])
                    {
                        case "MachineCode":
                            MachineCode = item["value"];
                            break;
                        case "ExamId":
                            ExamId = item["value"];
                            break;
                        case "Platform":
                            Platform = item["value"];
                            break;
                        case "Platforms":
                            Platforms = item["value"];
                            break;
                        case "UploadUnit":
                            int.TryParse(item["value"], out UploadUnit);
                            break;
                    }
                }

                if (string.IsNullOrEmpty(MachineCode))
                {
                    Console.WriteLine("设备码为空");
                }
                else
                {
                    comboBox1.Text = MachineCode;
                }
                if (string.IsNullOrEmpty(ExamId))
                {
                    Console.WriteLine("考试id为空");
                }
                else
                {
                    comboBox3.Text = ExamId;
                }
                if (string.IsNullOrEmpty(Platforms))
                {
                    Console.WriteLine(" 平台码为空！！");
                }
                else
                {
                    string[] s = Platforms.Split(';');
                    comboBox2.Items.Clear();
                    foreach (var sl in s)
                    {
                        comboBox2.Items.Add(s);
                    }

                }

                if (string.IsNullOrEmpty(Platform))
                {
                    Console.WriteLine("平台码为空！！");
                }
                else
                {
                    comboBox2.Text = Platform;
                }
                comboBox4.SelectedIndex = UploadUnit;

            }
        }
    }
}
