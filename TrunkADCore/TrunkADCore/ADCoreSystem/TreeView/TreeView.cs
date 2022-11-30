using ADCoreDataCommon.GameModel;
using ADCoreDataCommon.SQLiteData;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrunkADCore.ADCoreSystem.MyUtils;

namespace TrunkADCore.ADCoreSystem.ADCoreSys
{
    public class TreeView
    {
        public List<ProjectModel> UpdateTreeView(ADCoreSqlite ADCoreSqlite, out String projectId)
        {
            projectId = null;
            List<ProjectModel> Projects = new List<ProjectModel>();
            var ds = ADCoreSqlite.ExcuteReader($"SELECT Id,Name FROM SportProjectInfos");
            while (ds.Read())
            {
                string projrctID = ds.GetValue(0).ToString();
                projectId = projrctID;
                string projectName = ds.GetString(1);
                var sl = ADCoreSqlite.ExcuteReader($"SELECT Name,IsAllTested FROM DbGroupInfos WHERE ProjectId='{projrctID}'");
                Projects.Add(new ProjectModel { projectName = projectName, groupModels = new List<GroupModel>() });
                while (sl.Read())
                {
                    string names = sl.GetString(0);
                    int isAll = sl.GetInt32(1);
                    ProjectModel pp = Projects.FirstOrDefault(a => a.projectName == projectName);
                    if (pp != null)
                    {
                        pp.groupModels.Add(new GroupModel() { IsAllTested = isAll, name = names });
                    }
                    else
                    {
                        Projects.Add(new ProjectModel
                        {
                            groupModels = new List<GroupModel>()
                            {
                                new GroupModel  {IsAllTested = isAll, name = names}
                            },
                            projectName = projectName,
                        });
                    }

                }
            }
            return Projects;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectModels"></param>
        /// <param name="listView"></param>
        public  void ProjectTreeUpDate(List<ProjectModel> projectModels ,  Sunny.UI.UITreeView treeView)
        {
            if (projectModels != null && projectModels.Count > 0)
            {

                for (int i = 0; i < projectModels.Count; i++)
                {
                    TreeNode treeNode = new TreeNode(projectModels[i].projectName);
                    List<GroupModel> groupModels = projectModels[i].groupModels;
                    foreach (GroupModel groupModel in groupModels)
                    {
                        treeNode.Nodes.Add(groupModel.name);
                    }
                    treeView.Nodes.Add(treeNode);
                    //全部测试完成显示
                    for (int j = 0; j < groupModels.Count; j++)
                    {
                        if (groupModels[j].IsAllTested != 0)
                        {
                           treeView.Nodes[i].Nodes[j].BackColor = Color.Green;
                        }
                    }

                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="listView1"></param>
        /// <param name="groupname"></param>
        /// <param name="projectName"></param>
        /// <param name="projectId"></param>
        /// <param name="ADCoreSqlite"></param>

        public  void UpDataListview(ADCoreSqlite ADCoreSqlite,System.Windows.Forms.ListView listView1, string groupname, string projectName, string projectId = null)
        {
            listView1.Items.Clear();
            projectId = "";
            if (string.IsNullOrEmpty(groupname)) return;
            var ds = ADCoreSqlite.ExcuteReader($"SELECT b.Id,b.RoundCount,b.FloatType,b.Type " +
             $"FROM DbGroupInfos AS a,SportProjectInfos AS b WHERE a.ProjectId=b.Id AND a.Name='{groupname}' AND b.Name='{projectName}'");
            // 轮次
            int roundCount = 0;
            int FloatType = 0;
            int type0 = 0;
            while (ds.Read())
            {
                projectId = ds.GetValue(0).ToString();
                roundCount = ds.GetInt16(1); FloatType = ds.GetInt16(2);
                type0 = ds.GetInt16(3);

            }
            ds = ADCoreSqlite.ExcuteReader($"SELECT dpi.GroupName,dpi.Name,dpi.IdNumber,dpi.State,dpi.FinalScore,dpi.Id" +
             $" FROM DbPersonInfos as dpi WHERE dpi.GroupName='{groupname}' AND dpi.ProjectId='{projectId}'");
            int i = 1;
            listView1.BeginUpdate();
            //初始化标题
            InitListViewHead(listView1, roundCount);
            //listView1.Items.Clear();
            while (ds.Read())
            {
                string num = ds.GetString(2);
                int state = ds.GetInt16(3);
                string personid = ds.GetValue(5).ToString();
                System.Windows.Forms.ListViewItem item = new System.Windows.Forms.ListViewItem();
                item.UseItemStyleForSubItems = false;
                item.Text = i.ToString();
                item.SubItems.Add(projectName);
                item.SubItems.Add(ds.GetString(0));
                item.SubItems.Add(ds.GetString(1));
                item.SubItems.Add(num);
                if (state == 1)
                {
                    item.SubItems.Add("已测试");
                    item.SubItems[item.SubItems.Count - 1].BackColor = Color.MediumSpringGreen;
                }
                else
                {
                    item.SubItems.Add("未测试");
                }
                double maxscore = 1000;
                var res = ADCoreSqlite.ExecuteReaderList($"SELECT SortId,RoundId,Result,State,CreateTime,uploadState FROM ResultInfos WHERE PersonId='{personid}'");
                int k = 0;
                foreach (var dic in res)
                {
                    int.TryParse(dic["RoundId"], out int RoundId);
                    double.TryParse(dic["Result"], out double Result);
                    string restr = ResultState.ResultState2Str(dic["State"]);
                    if (restr == "已测试")
                    {
                        if (maxscore > Result)
                        {
                            maxscore = Result;
                        }
                        restr = decimal.Round(decimal.Parse(Result.ToString("0.0000")), FloatType).ToString();
                        item.SubItems.Add(restr);
                    }
                    else
                    {
                        item.SubItems.Add(restr);
                        item.SubItems[item.SubItems.Count - 1].ForeColor = Color.Red;

                    }
                    item.SubItems[item.SubItems.Count - 1].Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
                    if (dic["uploadState"] == "0")
                    {
                        item.SubItems.Add("未上传");
                        item.SubItems[item.SubItems.Count - 1].ForeColor = Color.Red;
                    }
                    else
                    {
                        item.SubItems.Add("已上传");
                        item.SubItems[item.SubItems.Count - 1].Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
                        item.SubItems[item.SubItems.Count - 1].ForeColor = Color.Green;
                    }
                    k++;
                }
                for (int j = k; j < roundCount; j++)
                {
                    item.SubItems.Add("无成绩");
                    item.SubItems.Add("未上传");
                }
                if (maxscore != 1000)
                {
                    item.SubItems.Add(decimal.Round(decimal.Parse(maxscore.ToString("0.0000")), FloatType).ToString());
                    item.SubItems[item.SubItems.Count - 1].Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
                }
                else
                {
                    item.SubItems.Add("无成绩");
                }
                listView1.Items.Insert(listView1.Items.Count, item);
                i++;
            }
            //自动列宽
            MyUntils MyUtils = new MyUntils();
            MyUtils.AutoResizeColumnWidth(listView1);
            listView1.EndUpdate();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rountCount"></param>
        /// <returns></returns>
        private  void InitListViewHead(System.Windows.Forms.ListView listView1, int rountCount)
        {
            listView1.View = System.Windows.Forms.View.Details;
            ColumnHeader[] Header = new ColumnHeader[100];
            int sp = 0;
            Header[sp] = new ColumnHeader();
            Header[sp].Text = "序号";
            Header[sp].Width = 40;
            sp++;
            Header[sp] = new ColumnHeader();
            Header[sp].Text = "项目名称";
            Header[sp].Width = 80;
            sp++;
            Header[sp] = new ColumnHeader();
            Header[sp].Text = "组别名称";
            Header[sp].Width = 40;

            sp++;
            Header[sp] = new ColumnHeader();
            Header[sp].Text = "姓名";
            Header[sp].Width = 100;
            sp++;

            Header[sp] = new ColumnHeader();
            Header[sp].Text = "准考证号";
            Header[sp].Width = 100;
            sp++;
            Header[sp] = new ColumnHeader();
            Header[sp].Text = "考试状态";
            Header[sp].Width = 40;
            sp++;
            for (int i = 1; i <= rountCount; i++)
            {
                Header[sp] = new ColumnHeader();
                Header[sp].Text = $"第{i}轮";
                Header[sp].Width = 40;
                sp++;

                Header[sp] = new ColumnHeader();
                Header[sp].Text = $"上传状态";
                Header[sp].Width = 80;
                sp++;
            }

            Header[sp] = new ColumnHeader();
            Header[sp].Text = "最好成绩";
            Header[sp].Width = 60;
            sp++;

            ColumnHeader[] Header1 = new ColumnHeader[sp];

            for (int i = 0; i < Header1.Length; i++)
            {
                Header1[i] = Header[i];
            }
            listView1.Columns.AddRange(Header1);
        }
    }
}
