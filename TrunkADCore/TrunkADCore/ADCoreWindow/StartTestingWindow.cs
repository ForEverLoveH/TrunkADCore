using ADCoreDataCommon.GameModel.TargetPoint;
using ADCoreDataCommon.GameModel;
using AForge.Math.Metrics;
using AForge.Video.DirectShow;
using HZH_Controls.Forms;
using HZH_Controls;
using OpenCvSharp;
using Sunny.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using TrunkADCore.ADCoreSystem.ADCoreSys;
using TrunkADCore.ADCoreSystem;
using TrunkADCore.ADCoreWindow;
using ADCoreDataCommon.SQLiteData;
using System.Data.SQLite;

namespace TrunkADCore.ADCoreWindow
{
    public partial class StartTestingWindow : Form
    {
        public StartTestingWindow()
        {
            InitializeComponent();
        }
        StartTestingSys StartTestingSys = new StartTestingSys();
        ADCoreSqlite ADCoreSqlite = null;
        string _projectName = null;
        string _ProjectId = null;
        string _Type = null;
        int _RoundCount = 0;
        //当前测试轮次
        public int RoundCount0 = 1;
        int BestScoreMode = 0;
        int _TestMethod = 0;
        int _FloatType = 0;
        string formTitle = "";
        string _GroupName = "";

        int _BestScoreMode = 0;

        public static string strMainModule = System.AppDomain.CurrentDomain.BaseDirectory + "data\\";
        SavaStudentData nowRaceStudentData = new SavaStudentData();
        CommandData commandData = new CommandData();
        string ScoreDir = string.Empty;
        string markPointFile = "markPoint.dat";
        PointForm1_ pf1 = new PointForm1_();

        OpenCvSharp.VideoWriter VideoOutPut;
        private object bmpObj = new object();
        private Bitmap bmpSoure = new Bitmap(1, 1);
        private int _width;
        private int _height;
        string jpName = "";
        string jpDis = "";
        bool jpStatus = true;//true是上定点,false是下定点
        bool isMirrorMode = false;
        List<TargetPoint> targetPoints = new List<TargetPoint>();
        List<TargetPoint> TopListSort = new List<TargetPoint>();//? 顶部坐标列表
        List<TargetPoint> BottomListSort = new List<TargetPoint>();//? 底部坐标列表
        System.Drawing.Point cenMarkPoint;//? 铅球中心点
        System.Drawing.Point markerTopJumpY, markerBottomJumpY, markerTopJump, markerBottomJump, markerTmp, mouseMovePoint;
        List<System.Drawing.Point[]> gfencePnts = new List<System.Drawing.Point[]>();
        List<int[]> gfencePntsDisValue = new List<int[]>();
        int GCCounta = 0;
        int videoSourceRuningR0 = 0;
        int frameRecSum = 0;//计算帧数
        /// <summary>
        /// 定标参数
        /// </summary>
        public int colum = 0;
        public int initDis = 0;
        public int distance = 0;
        public int nowColum = 0;
        int formShowStatus = 0;
        private object ImageQueuesLock = new object();
        private ConcurrentQueue<ImageAndIndex> ImageQueues = new ConcurrentQueue<ImageAndIndex>();
        Bitmap backBp = null;
        Bitmap resultBp = null;
        //? 是否实心球模式
        bool isBallCheckBoxb = false;
        //? 是否展示选择
        bool isShowImgList = true;
        //? 是不是铅球
        bool isShotPut = false;
        //? 是不是坐位体前屈
        bool isSitting = false;
        public List<imgMsS> imgMs = new List<imgMsS>();
        string ReservedDigitsTxt = "0.000";
        
        FuseImg fuseImg = null;
        int skipFrameDispR0 = 0;
        int cameraSkip = 0;
        OpenCvSharp.Point[] conPoints0 = new OpenCvSharp.Point[4];

        public double MeasureLen = 0;//测量长度
        public double MeasureLenX = 0;//测量水平长度
        public double MeasureLenY = 0;//测量垂直长度


        Boolean Measure = true;//测量长度状态

        string nowFileName = "";//当前文件名
        string nowTestDir = String.Empty;//当前文件目录

        int recTimeR0 = 0;//计时时间
        int frameSum = 0;
        private Thread threadVideo2;
        string dangwei = "米";

        /// <summary>
        /// 委托和调用的异步方法有相同的签名
        /// </summary>
        /// <returns></returns>
        public delegate string AsyncMethodCaller();
        private void StartTestingWindow_Load(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            string code = "程序集版本：" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string code1 = "文件版本：" + Application.ProductVersion.ToString();
            VersionLabel.Text = code;
            AsyncMethodCaller caller = new AsyncMethodCaller(TestMethodAsync);
            var workTask = Task.Run(() => caller.Invoke());
            RunTestLoadInit();
            UpdateListView(_ProjectId, _GroupName, 1);
            ParameterizedThreadStart method = new ParameterizedThreadStart(ImagePredictLabelQueues2ThreadFun);
            threadVideo2 = new Thread(method);
            threadVideo2.IsBackground = true;
            threadVideo2.Start();
            StartTestingSys.SerialInit(portNameSearch,portNamesList,openSerialPortBtn,tb_nBaudrate);
            StartTestingSys.CameraInit(out cameraName, out maxFps, out Fps,out _width, out _height,out cameraSkip);
            Opencamera(cameraName, _width, _height);
        }

         

        string cameraName = String.Empty;
        int maxFps = 0;
        int Fps = 0;
       
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private string TestMethodAsync()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            LoadJumpInit();
            StartMeasure();
            sw.Stop();
            string useTimeStr = string.Format("耗时{0}ms.", sw.ElapsedMilliseconds.ToString());
            return useTimeStr;

        }

        #region 初始化
        void LoadJumpInit()
        {
            ScoreDir = Path.Combine(strMainModule, "Score");
            if (!Directory.Exists(ScoreDir)) Directory.CreateDirectory(ScoreDir);
            string[] strg = TxtFile.Instance.read(strMainModule + markPointFile);
            if (strg != null)
            {
                try
                {
                    if (strg.Length > 0)
                    {
                        foreach (var S in strg)
                        {
                            String[] ssl = S.Split(',');
                            if (ssl.Length == 4)
                            {
                                System.Drawing.Point p = commandData.XYString2Point(ssl[1]);
                                bool b1 = !(ssl[3] == "0");
                                targetPoints.Add(new TargetPoint()
                                {
                                    x = p.X,
                                    y = p.Y,
                                    name = ssl[0],
                                    dis = commandData.str2int(ssl[2]),//cm
                                    status = b1
                                });
                            }
                        }
                        UpdateTargetPointListView(true);
                    }

                }
                catch (Exception ex)
                {
                    LoggerHelper.Debug(ex);
                }
            }
            if (_FloatType == 0)
            {
                ReservedDigitsTxt = "0";
            }
            else
            {
                ReservedDigitsTxt = "0.";
                for (int i = 0; i < _FloatType; i++)
                {
                    ReservedDigitsTxt += "0";
                }
            }

        }
        private void RunTestLoadInit()
        {
            var ds = StartTestingSys.UpdateGroupCombox(groupCbx, _ProjectId,  ADCoreSqlite);
            UpdataGroupName(ds);
            StartTestingSys.UpdateRoundCountCombox(RoundCountCbx, _RoundCount);
            RoundCountCbx.SelectedIndex = 0;
            projectTypeCbx.SelectedIndex = Convert.ToInt32(_Type);
          
        }
        /// <summary>
        /// 更新群组信息
        /// </summary>
        /// <param name="ds"></param>
        private void UpdataGroupName(SQLiteDataReader ds)
        {
            while (ds.Read())
            {
                groupCbx.Items.Add(ds.GetString(0));
            }
            int index = -1;
            groupCbx.SelectedIndex = index;
            groupCbx.Text = "";
            if (string.IsNullOrEmpty(_GroupName) && groupCbx.Items.Count > 0)
            {
                _GroupName = groupCbx.Items[0].ToString();
                groupCbx.SelectedIndex = 0;
            }
            else
            {
                if ((index = groupCbx.Items.IndexOf(_GroupName)) >= 0)
                {
                    groupCbx.SelectedIndex = index;
                }
            }
        }




        #endregion

        #region 测距模块

        /// <summary>
        /// 开始测距
        /// </summary>
        void StartMeasure()
        {
            Measure = true;//测量长度
            ControlHelper.ThreadInvokerControl(this, () =>
            {
                button13.Text = "测量中(S)";
                button13.BackColor = Color.Red;
            });

        }
        /// <summary>
        /// 停止测距
        /// </summary>
        void StopMeasure()
        {
            Measure = false;//测量长度
            ControlHelper.ThreadInvokerControl(this, () =>
            {
                button13.Text = "测量(S)";
                button13.BackColor = Color.White;
            });
        }
        /// <summary>
        /// 
        /// </summary>
        void MeasureFun()
        {
            if (Measure)
            {
                StopMeasure();
            }
            else
            {
                StartMeasure();
            }
        }

        #endregion


        #region 页面事件
        /// <summary>
        /// 刷新按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void refreshStuViewBtn_Click(object sender, EventArgs e)
        {
            if (RoundCount0 > 0)
            {
                UpdateListView(_ProjectId, _GroupName, RoundCount0);
            }
        }
        private void StartTestingWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
             StartTestingSys.CloseCamera(opencameraBtn,rgbVideoSource);
        }
        private void StartTestingWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (uiTabControl1 .SelectedIndex != 0) return;

            if (e.KeyCode == Keys.Space)
            {
                if (recTimeR0 > 0)//停止录像
                {
                    recTimeR0 = 1;
                    return;
                }
                bool flag = true;
                if (MeasureLen != 0 || recTimeR0 != 0)
                {
                    flag = BeginTest();
                }
                if (flag)
                {
                    StartRec();
                }
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.A)
            {
                if (recTimeR0 > 0) return;
                //上一张
                DispDecPic();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.D)
            {
                //下一张
                DispIncPic();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.S)
            {
                if (recTimeR0 > 0) return;
                //测量
                MeasureFun();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.W)
            {
                WriteScore2Db();

                e.Handled = true;
            }
        }
        /// <summary>
        /// 打开摄像头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void opencameraBtn_Click(object sender, EventArgs e)
        {
            if (recTimeR0 > 0)
            {
                FrmTips.ShowTipsError(this, "考试中请勿进行此操作");
                return;
            }
            if (opencameraBtn.Text == "关闭摄像头")
            {
               StartTestingSys. CloseCamera(opencameraBtn,rgbVideoSource);
               return;
            }
            if (string.IsNullOrEmpty(cameraName))
            {
                FrmTips.ShowTipsError(this, "请选择摄像头!");
                return;
            }

            StartTestingSys.OpenCamera(  opencameraBtn,cameraName,rgbVideoSource,_width,maxFps);
        }
        /// <summary>
        ///成绩上传
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            ParameterizedThreadStart method = new ParameterizedThreadStart(UploadScoreForNowGroup);
            Thread threadRead = new Thread(method);
            threadRead.IsBackground = true;
            threadRead.Start();

        }
         /// <summary>
         ///上传成绩
         /// </summary>
         /// <param name="obj"></param>
        private void UploadScoreForNowGroup(object obj)
        {
            ControlHelper.ThreadInvokerControl(this, () =>
            {
                button2.Text = "上传中";
                button2.BackColor = Color.Red;

            });
            string[] fusp = new string[2];
            fusp[0] = _projectName;
            fusp[1] = _GroupName;
            string outMessage = StartTestingSys.UploadStudentThreadFun(fusp, ADCoreSqlite, RoundCount0);
            if (string.IsNullOrWhiteSpace(outMessage))
            {
                FrmTips.ShowTipsInfo(this, "上传结束");
            }
            else
            {
                MessageBox.Show(outMessage);
            }

            ControlHelper.ThreadInvokerControl(this, () => {
                    button2.Text = "成绩上传";
                    button2.BackColor = System.Drawing.SystemColors.Control; 
            });
            
            
        }

        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        { 
            if (StartTestingSys.OutPutScore(_ProjectId, _GroupName,_projectName, ADCoreSqlite))
            {
                FrmTips.ShowTipsSuccess(this, "打印成功 ");
                return;

            }
            else
            {
                FrmTips.ShowTipsError(this, "打印失败");
                return;
            }

        }
        /// <summary>
        /// 平台设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (recTimeR0 > 0)
            {
                FrmTips.ShowTipsError(this, "考试中请勿进行此操作");
                return;
            }
            ParameterizedThreadStart method = new ParameterizedThreadStart((obj) =>
            {
                StartTestingSys.ShowEquipmentCodeFrom(ADCoreSqlite);
            });
            Thread threadRead = new Thread(method);
            threadRead.IsBackground = true;
            threadRead.Start();
        }
        /// <summary>
        /// 筛选
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            UpdataGroupName(StartTestingSys.UpdateGroupCombox(groupCbx, _ProjectId, ADCoreSqlite, textBox1.Text));
        }
        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            _GroupName = "";
            UpdataGroupName(StartTestingSys.UpdateGroupCombox(groupCbx, _ProjectId, ADCoreSqlite));
        }
        /// <summary>
        /// 摄像头设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            if (recTimeR0 > 0)
            {
                FrmTips.ShowTipsError(this, "考试中请勿进行此操作");
                return;
            }
            StartTestingSys.OpenCameraSetting(out cameraName,out maxFps,out Fps,out cameraSkip);
        }
        private void button7_Click(object sender, EventArgs e)
        {
            if (isSitting)
            {
                int nlen = imgMs.Count;
                if (nlen > 0)
                {
                    int maxW = 0;
                    int maxH = 0;
                    int index = 0;
                    int minWidth = conPoints0[0].X;
                    int maxWidth = conPoints0[3].X;
                    int minHeigh = conPoints0[0].Y;
                    int maxHeigh = conPoints0[3].Y;
                    int anaylistLen = 10;
                    int[] anaylist = new int[anaylistLen];
                    OpenCvSharp.Point[] anaPoint = new OpenCvSharp.Point[nlen];
                    for (int k = 0; k < nlen; k++)
                    {
                        if (k == 54)
                            Console.WriteLine();
                        bool[][] isHand = imgMs[k].isHand;
                        //maxW = 0;
                        //maxH = 0;
                        int maxwT = 0;
                        int maxhT = 0;
                        if (isHand == null) continue;
                        for (int i = minWidth; i < maxWidth; i++)
                        {
                            for (int j = minHeigh; j < maxHeigh; j++)
                            {
                                if (i == 1135 && j == 143)
                                    Console.WriteLine();
                                if (isHand[i][j])
                                {
                                    int nl = i - 5;
                                    if (nl < 0) nl = 0;
                                    bool flag0 = true;
                                    for (int i1 = nl; i1 < i; i1++)
                                    {
                                        if (!isHand[i1][j])
                                        {
                                            flag0 = false;
                                            break;
                                        }
                                    }

                                    if (!flag0) continue;
                                    //当前图片最大值
                                    maxwT = i;
                                    maxhT = j;
                                    //全局最大值
                                    if (i > maxW)
                                    {
                                        maxW = i;
                                        maxH = j;
                                        index = k;
                                    }
                                }
                            }
                        }

                        anaPoint[k] = new OpenCvSharp.Point(maxwT, maxhT);
                    }


                    string Log = $"最大值:index:{index},X:{maxW}  y:{maxH}";
                    StartTestingSys.WriteLog(lrtxtLog, Log, 0);


                    if (imgMs[index] != null && imgMs[index].img != null)
                    {
                        nowFileName = index + "";
                        ControlHelper.ThreadInvokerControl(this, () =>
                        {
                            pictureBox1.Image = imgMs[index].img;
                            recImgIndex.Text = $"图片索引:{nowFileName}";
                            pictureBox1.Refresh();
                        });
                    }

                    StartMeasure();
                    DispJumpLength1(maxW, maxH);
                    pictureBox1.Refresh();
                    StopMeasure();
                }

                else
                {
                    FrmTips.ShowTipsError(this, "测量最大值失败，请检查操作事项");
                    return;
                }
            }
        }

        
        /// <summary>
        /// 上一张
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            if (recTimeR0 > 0)
            {
                FrmTips.ShowTipsError(this, "考试中请勿进行此操作");
                return;
            }
            DispDecPic();
        }

        private void DispDecPic()
        {
            if (!StartTestingSys.IsHaveStudent(nowRaceStudentData))
            {
                uiTabControl1.SelectedIndex = 0;
                return;
            }
            if (imgMs.Count == 0)
            {
                MessageBox.Show("请录像");
                return;
            }
            StartTestingSys. CloseCamera(opencameraBtn,rgbVideoSource);
            int i = commandData.str2int(nowFileName);

            if (i == 0)
            {
                MessageBox.Show("到尽头了");
                return;
            }
            i--;
            if (imgMs.Count < i)
                i = 1;


            nowFileName = i + "";
            if (null != imgMs[i])
            {
                //setHScrollBarValue(i);
                pictureBox1.Image = imgMs[i].img;
                recImgIndex.Text = $"图片索引:{nowFileName}";
            }
            rgbVideoSourceRePaint();
        }

        private void rgbVideoSourceRePaint()
        {
            pictureBox1.Refresh();
            if (!rgbVideoSource.IsRunning)
                rgbVideoSource.Refresh();
        }

        /// <summary>
        /// 下一张
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            if (recTimeR0 > 0)
            {
                FrmTips.ShowTipsError(this, "考试中请勿进行此操作");
                return;
            }
            DispIncPic();
        }

        private void DispIncPic()
        {
            if (!StartTestingSys.IsHaveStudent(nowRaceStudentData))
            {

                uiTabControl1.SelectedIndex = 0;
                return;
            }
            if (imgMs.Count == 0)
            {
                MessageBox.Show("请录像");
                return;
            }
            StartTestingSys. CloseCamera(opencameraBtn,rgbVideoSource);
            int i =commandData. str2int(nowFileName);
            i++;

            if (i >= imgMs.Count)
            {
                i = imgMs.Count - 1;
                MessageBox.Show("到尽头了");
                return;
            }
            nowFileName = i + "";
            if (null != imgMs[i])
            {
                //setHScrollBarValue(i);
                pictureBox1.Image = imgMs[i].img;
                recImgIndex.Text = $"图片索引:{nowFileName}";
            }

            rgbVideoSourceRePaint();
        }

        /// <summary>
        /// 浏览图像
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            if (recTimeR0 > 0)
            {
                FrmTips.ShowTipsError(this, "考试中请勿进行此操作");
                return;
            }
            openImgList();
        }

        private void openImgList()
        {
            if (! StartTestingSys.IsHaveStudent(nowRaceStudentData))
            {
                uiTabControl1.SelectedIndex = 0;
                return;
            }

            FindImage fi = new  FindImage();
            fi.imgMs = imgMs;
            //fi.nowTestDir = nowTestDir;
            if (fi.ShowDialog() == DialogResult.OK)
            {
                nowFileName = fi.fileName;
                //string fileName = nowTestDir + "\\" + fi.fileName + ".jpg";
                int sp = commandData.str2int(nowFileName);
                if (sp < imgMs.Count - 1)
                {
                    SetHScrollBarValue(sp);
                    pictureBox1.Image = imgMs[sp].img;
                }
            }
            else
            {
                int sp = imgMs.Count - 5;
                nowFileName = sp + "";
                if (sp < imgMs.Count)
                {
                    pictureBox1.Image = imgMs[sp].img;
                }
                else
                {
                    sp = imgMs.Count - 1;
                    nowFileName = sp + "";
                    pictureBox1.Image = imgMs[sp].img;

                }
            }
            recImgIndex.Text = $"图片索引:{nowFileName}";
            StartMeasure();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (isSitting)
            {
                int nlen = imgMs.Count;
                int maxW = 0;
                int maxH = 0;
                int index = 0;
                int minWidth = conPoints0[0].X;
                int maxWidth = conPoints0[3].X;
                int minHeigh = conPoints0[0].Y;
                int maxHeigh = conPoints0[3].Y;
                int anaylistLen = 10;
                int[] anaylist = new int[anaylistLen];
                OpenCvSharp.Point[] anaPoint = new OpenCvSharp.Point[nlen];
                for (int k = 0; k < nlen; k++)
                {
                    bool[][] isHand = imgMs[k].isHand;
                    //maxW = 0;
                    //maxH = 0;
                    int maxwT = 0;
                    int maxhT = 0;
                    if (isHand == null) continue;
                    for (int i = minWidth; i < maxWidth; i++)
                    {
                        for (int j = minHeigh; j < maxHeigh; j++)
                        {
                            if (isHand[i][j])
                            {
                                int nl = i - 5;
                                if (nl < 0) nl = 0;
                                bool flag0 = true;
                                for (int i1 = nl; i1 < i; i1++)
                                {
                                    if (!isHand[i1][j])
                                    {
                                        flag0 = false;
                                        break;
                                    }
                                }

                                if (!flag0) continue;
                                //当前图片最大值
                                maxwT = i;
                                maxhT = j;
                                //全局最大值
                                if (i > maxW)
                                {
                                    maxW = i;
                                    maxH = j;
                                    index = k;
                                }
                            }
                        }
                    }

                    anaPoint[k] = new OpenCvSharp.Point(maxwT, maxhT);
                }

                string Log = $"最大值:index:{index},X:{maxW}  y:{maxH}";
                StartTestingSys.WriteLog(lrtxtLog, Log, 0);
                int iFori = -1;
                int iFlag = 0;
                for (int i = index; i > 0; i--)
                {
                    bool isBreakForj = true;
                    int ax = anaPoint[i].X;
                    int end = (index + 1) > nlen - 1 ? nlen - 1 : index + 2;
                    int begin = (index - 10) < 0 ? 0 : index - 2;
                    for (int j = begin; j < end; j++)
                    {
                        int ipx = Math.Abs(ax - anaPoint[j].X);
                        if (ipx > 5) isBreakForj = false;
                        break;
                    }

                    if (isBreakForj)
                    {
                        iFori = i;
                        break;
                    }
                }

                if (iFori != -1)
                {
                    int maxwT = anaPoint[iFori].X;
                    int maxhT = anaPoint[iFori].Y;
                    if ((maxwT < maxWidth && maxwT > minWidth) && maxhT < maxHeigh && maxhT > minHeigh)
                    {
                        maxW = maxwT;
                        maxH = maxhT;
                        index = iFori;
                    }
                }

                Log = $"筛选值:index:{index},X:{maxW}  y:{maxH}";
                StartTestingSys.WriteLog(lrtxtLog, Log, 0);

                if (imgMs[index] != null && imgMs[index].img != null)
                {
                    nowFileName = index + "";
                    ControlHelper.ThreadInvokerControl(this, () =>
                    {
                        pictureBox1.Image = imgMs[index].img;
                        recImgIndex.Text = $"图片索引:{nowFileName}";
                        pictureBox1.Refresh();

                    });

                }

                StartMeasure();
                DispJumpLength1(maxW, maxH);
                pictureBox1.Refresh();
                StopMeasure();
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (nowRaceStudentData != null)
            {


                if (recTimeR0 > 0) //停止录像
                {
                    recTimeR0 = 1;
                    return;
                }

                bool flag = true;
                if (MeasureLen != 0 || recTimeR0 != 0)
                {
                    flag = BeginTest();
                }

                if (flag)
                {
                    StartRec();
                }
            }
            else 
            {
                FrmTips.ShowTipsInfo(this, "请先选择考生数据 ");
                return;
            }

        }

        private void StartRec()
        {
            if (!StartTestingSys.IsHaveStudent(nowRaceStudentData) )
            {

                return;
            }
            recTimeR0 = 0;
            RecordEnd = 0;
            fuseImg = new FuseImg(backBp);
            ControlHelper.ThreadInvokerControl(this, () =>
            {
                button12.Text = "录像中...";
                button12.BackColor = Color.Red;
            });

            StopMeasure();
            nowTestDir = $"\\{_projectName}\\{_GroupName}\\{nowRaceStudentData.idNumber}_{nowRaceStudentData.name}\\第{nowRaceStudentData.RoundId}轮\\";
            nowTestDir = ScoreDir + nowTestDir;
            if (!Directory.Exists(nowTestDir))
            {
                DirectoryInfo dir = new DirectoryInfo(nowTestDir);
                dir.Create();//自行判断一下是否存在。
            }
            string avipath = Path.Combine(nowTestDir,
                $"{nowRaceStudentData.idNumber}_{nowRaceStudentData.name}_第{nowRaceStudentData.RoundId}轮.mp4");
            if (File.Exists(avipath))
            {
                File.Delete(avipath);
            }
            OpenVideoOutPut(avipath);
            //recFileSp = 1;//录像文件顺序号
            frameSum = 0;
            recTimeR0 = commandData.str2int(recTimeTxt.Text) * 10;
            Task.Run(() => StartTestingSys.VoiceOut0("考生开始考试"));
           StartTestingSys. SendScore(nowRaceStudentData.name, "开始考试", "");
        }

        private void OpenVideoOutPut(string outPath)
        {
            try
            {
                if (VideoOutPut != null && VideoOutPut.IsOpened())
                {
                    VideoOutPut.Release();
                }
                VideoOutPut = new OpenCvSharp.VideoWriter(outPath, OpenCvSharp.FourCC.XVID, 60, new OpenCvSharp.Size(_width, _height));
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
                Console.WriteLine(ex.Message);
            }
        }

        private bool BeginTest()
        {
            //检查成绩要测第几次
            MeasureLen = 0;//测量长度
            bool IhvaStu =StartTestingSys. IsHaveStudent(nowRaceStudentData);
            if (!IhvaStu)
            {
                return IhvaStu;
            }
            nowRaceStudentData.state = 0;
            recTimeR0 = 0;
            imgMs.Clear();
            GC.Collect();
            try
            {
                StartTestingSys.  rgbVideoSourceStart(rgbVideoSource,cameraName,opencameraBtn,_width,_height);
                return true;
            }
            catch (Exception ex)
            {
                FrmTips.ShowTipsError(this, "摄像头未开启");
                return false;
            }
        }
        /// <summary>
        ///  测量中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            MeasureFun();
        }
        /// <summary>
        /// 定标设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button27_Click(object sender, EventArgs e)
        {
            if (recTimeR0 > 0)
            {
                FrmTips.ShowTipsError(this, "考试中请勿进行此操作");
                return;
            }
            SelectPunctuationRule sptr = new SelectPunctuationRule();
            if (sptr.ShowDialog() == DialogResult.OK)
            {
                targetPoints.Clear();
                colum = sptr.colum;
                initDis = sptr.initDis;
                distance = sptr.distance;
                pf1.columSum = colum;
                nowColum = 1;
                jpName = nowColum + "";
                jpDis = initDis + "";
                jpStatus = true;
                pf1.updateFlp(nowColum);
                formShowStatus = 1;
                pf1.Show();
            }
        }
        /// <summary>
        /// 写入成绩
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnWriteScore_Click(object sender, EventArgs e)
        {
            WriteScore2Db();
        }

        private void WriteScore2Db()
        {
            if (recTimeR0 > 0)
            {
                FrmTips.ShowTipsError(this, "考试中请勿进行此操作");
                return;
            }

            //updateRaceStudentDataListsScore();
            if (!StartTestingSys.IsHaveStudent(nowRaceStudentData))
            {
                FrmTips.ShowTipsError(this, "数据异常");
                return;
            }

            //写入成绩
            if (imgMs.Count > 0)
            {
                //弹窗显示成绩是否写入
                DetermineGrades dmg = new DetermineGrades();
                double fenmu = 1000;
                if (isSitting)
                {
                    fenmu = 10;
                }

                dmg.score = MeasureLen / fenmu;
                dmg.dangwei = dangwei;
                if (dmg.ShowDialog() == DialogResult.OK)
                {
                    ///成绩修改写入日志
                    if (dmg.checkScore != 0)
                    {
                        MeasureLen = dmg.checkScore * fenmu;
                        //测试项目
                        string projectTypeCbxtxt = projectTypeCbx.Text;
                        string txt_Grouptxt = _GroupName;
                        string txt_GNametxt = nowRaceStudentData.name;
                        double score1 = dmg.score;


                        string scoreContent = string.Format(
                            "时间:{0,-20},项目:{1,-20},组别:{2,-10},准考证号:{3,-20},姓名{4,-5},第{5}次成绩:修改成绩{6,-5}为{7,-5}, 状态:{8,-5}",
                            DateTime.Now.ToString("yyyy年MM月dd日HH:mm:ss"),
                            projectTypeCbxtxt,
                            txt_Grouptxt,
                            nowRaceStudentData.idNumber,
                            txt_GNametxt,
                            RoundCount0,
                            score1.ToString(ReservedDigitsTxt),
                            dmg.checkScore,
                            "已测试");
                        File.AppendAllText(@"./成绩日志.txt", scoreContent + "\n");
                    }

                    string score = (MeasureLen / fenmu).ToString(ReservedDigitsTxt);
                    Task.Run(() => StartTestingSys.VoiceOut0($"成绩:{score}{dangwei}", 3));
                    bool sendScoreReturn = StartTestingSys.SendScore(nowRaceStudentData.name, score);
                    //写入成绩
                   Input2Result();
                }
            }
        }

        private void Input2Result()
        {
            if (!StartTestingSys.IsHaveStudent(nowRaceStudentData))
            {
               uiTabControl1.SelectedIndex = 0;
                return;
            }
            if (imgMs.Count < 1)
            {
                FrmTips.ShowTipsError(this, "先录像");
                return;
            }
            MakeEndResult();
            SaveOkFileOut();
            StartMeasure();
            ///自动上传成绩
            //if (isAutoUpload.Checked)
            //uploadStudentScore();
            //自动下一个
            //nextMode();
          StartTestingSys.  rgbVideoSourceStart(rgbVideoSource,cameraName,opencameraBtn,_width,_height);
        }
        /// <summary>
        /// 
        /// </summary>
        private void SaveOkFileOut()
        {
            string savePath = Path.Combine(nowTestDir, $"落地_{nowRaceStudentData.idNumber}.jpg");
            if (pictureBox1.Image != null)
            {
               StartTestingSys. ImgJpgSave(pictureBox1.Image, savePath);
                Image newImage = pictureBox1.Image;
                {
                    if (null != newImage)
                    {
                        using (var graphic = Graphics.FromImage(newImage))
                        {
                            // 核心参数啊，感觉相当于PS保存时间的质量设置参数
                            Int64 qualityLevel = 80L;
                            Pen pen = new Pen(Color.MediumSpringGreen, 1);
                            Font drawFont = new Font("Arial", 14);
                            SolidBrush drawBrush = new SolidBrush(Color.MediumSpringGreen);
                            System.Drawing.Point markerTopJumpT = new System.Drawing.Point(0, 0);
                            System.Drawing.Point markerBottomJumpT = new System.Drawing.Point(0, 0);
                            List<TargetPoint> TopList = new List<TargetPoint>();
                            List<TargetPoint> BottomList = new List<TargetPoint>();
                            markerTopJumpT = markerTopJump;
                            markerBottomJumpT = markerBottomJump;
                            TopList = targetPoints.FindAll(a => a.status);
                            BottomList = targetPoints.FindAll(a => !a.status);
                            List<TargetPoint> TopListSort = TopList.OrderBy(a => a.x).ToList();
                            List<TargetPoint> BottomListSort = BottomList.OrderBy(a => a.x).ToList();
                            // 高质量
                            graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                            graphic.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            qualityLevel = 100L;
                            System.Drawing.Imaging.ImageCodecInfo codec =
                                System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders()[1];
                            System.Drawing.Imaging.EncoderParameters eParams =
                                new System.Drawing.Imaging.EncoderParameters(1);
                            eParams.Param[0] =
                                new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality,
                                    qualityLevel);
                            {
                                //框点十字
                                int left = 0;
                                //画顶标
                                foreach (var mark in TopListSort)
                                {
                                    System.Drawing.Point p = new System.Drawing.Point(mark.x, mark.y);

                                    if (left <= TopListSort.Count - 2)
                                    {
                                        System.Drawing.Point p1 = new System.Drawing.Point(TopListSort[left + 1].x,
                                            TopListSort[left + 1].y);
                                        graphic.DrawLine(pen, p, p1);
                                        left++;
                                    }

                                    commandData.drawPointCross(graphic, p, pen);
                                    commandData.drawPointText(graphic, $"({mark.name}){mark.dis}cm", p, drawFont,
                                        drawBrush, 0, 60, 0, 30);
                                }

                                left = 0;
                                //画底标
                                foreach (var mark in BottomListSort)
                                {
                                    System.Drawing.Point p = new System.Drawing.Point(mark.x, mark.y);

                                    if (left <= BottomListSort.Count - 2)
                                    {
                                        System.Drawing.Point p1 = new System.Drawing.Point(BottomListSort[left + 1].x,
                                            BottomListSort[left + 1].y);
                                        graphic.DrawLine(pen, p, p1);
                                        left++;
                                    }

                                    commandData.drawPointCross(graphic, p, pen);
                                    commandData.drawPointText(graphic, $"({mark.name}){mark.dis}m", p, drawFont,
                                        drawBrush, 0, 60, 1, 10);
                                }

                                //中间框点连线
                                int min = TopListSort.Count > BottomListSort.Count
                                    ? BottomListSort.Count
                                    : TopListSort.Count;
                                for (int i = 0; i < min; i++)
                                {
                                    TargetPoint mark = TopListSort[i];
                                    TargetPoint mark1 = BottomListSort[i];
                                    System.Drawing.Point p = new System.Drawing.Point(mark.x, mark.y);
                                    System.Drawing.Point p1 = new System.Drawing.Point(mark1.x, mark1.y);
                                    graphic.DrawLine(pen, p, p1);

                                }
                            }

                            pen.Color = Color.Red;
                            drawFont = new Font("Arial", 32);
                            graphic.DrawLine(pen, markerTopJumpT, markerBottomJumpT);
                            drawBrush = new SolidBrush(Color.Red); // Create point for upper-left corner of drawing.
                            if (isSitting)
                            {
                                graphic.DrawString((MeasureLen / 10).ToString(ReservedDigitsTxt) + dangwei, drawFont,
                                    drawBrush, markerBottomJumpT.X - 70, markerBottomJumpT.Y);

                            }
                            else
                            {
                                graphic.DrawString((MeasureLen / 1000).ToString(ReservedDigitsTxt) + dangwei, drawFont,
                                    drawBrush, markerBottomJumpT.X - 70, markerBottomJumpT.Y);

                            }

                            //时间 
                            graphic.FillRectangle(new SolidBrush(Color.White),
                                new Rectangle(new System.Drawing.Point(350, 0), new System.Drawing.Size(300, 50)));
                            graphic.DrawString($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", new Font("Arial", 20),
                                drawBrush, 360, 10);

                            //考生姓名和学号
                            graphic.FillRectangle(new SolidBrush(Color.White),
                                new Rectangle(new System.Drawing.Point(700, 0), new System.Drawing.Size(540, 50)));
                            graphic.DrawString(
                                $"组号:{nowRaceStudentData.groupName} 考号:{nowRaceStudentData.idNumber} 姓名:{nowRaceStudentData.name}",
                                new Font("Arial", 18), drawBrush, 710, 10);
                            if (isShotPut)
                            {
                                pen.DashPattern = new float[] { 5f, 10f };
                                graphic.DrawLine(pen, cenMarkPoint, mousePoint);
                            }

                            string imgOkFileName =
                                Path.Combine(nowTestDir, $"{nowRaceStudentData.idNumber}.jpg"); // = 0;//第几次考试
                            if (File.Exists(imgOkFileName))
                                File.Delete(imgOkFileName);

                            newImage.Save(imgOkFileName, codec, eParams);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void MakeEndResult()
        {
            double fenmu = 1000;
            if (isSitting)
            {
                fenmu = 10;
            }
            string score = (MeasureLen / fenmu).ToString(ReservedDigitsTxt);
            nowRaceStudentData.score = Convert.ToDouble(score);
            string projectTypeCbxtxt = projectTypeCbx.Text;
            string txt_Grouptxt = _GroupName;
            string txt_GNametxt = nowRaceStudentData.name;
            string ScoreStatus = ResultState.ResultState2Str(nowRaceStudentData.state);
            
            string scoreContent = string.Format("时间:{0,-20},项目:{1,-20},组别:{2,-10},准考证号:{3,-20},姓名{4,-10},第{5}次成绩:{6,-5}, 状态:{7,-5}",
                DateTime.Now.ToString("yyyy年MM月dd日HH: mm:ss"),
                projectTypeCbxtxt,
                txt_Grouptxt,
                nowRaceStudentData.idNumber,
                txt_GNametxt,
                RoundCount0,
                score,
                ScoreStatus);
            File.AppendAllText(@"./成绩日志.txt", scoreContent + "\n");
           StartTestingSys. UpdateJumpLen(ADCoreSqlite,nowRaceStudentData,RoundCount0) ;
        }

        
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.MediumSpringGreen, 1);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            Font drawFont = new Font("Arial", 14);
            SolidBrush drawBrush = new SolidBrush(Color.MediumSpringGreen);
            List<TargetPoint> TopList = targetPoints.FindAll(a => a.status);
            List<TargetPoint> BottomList = targetPoints.FindAll(a => !a.status);
            List<TargetPoint> TopListSort = TopList.OrderBy(a => a.x).ToList();
            List<TargetPoint> BottomListSort = BottomList.OrderBy(a => a.x).ToList();
            //框点十字
            int left = 0;
            foreach (var mark in TopListSort)
            {
                System.Drawing.Point p = new System.Drawing.Point(mark.x, mark.y);
                if (left <= TopListSort.Count - 2)
                {
                    System.Drawing.Point p1 = new System.Drawing.Point(TopListSort[left + 1].x, TopListSort[left + 1].y);
                    e.Graphics.DrawLine(pen, p, p1);
                    left++;
                }
                commandData.drawPointCross(e.Graphics, p, pen);
               commandData. drawPointText(e.Graphics, $"({mark.name}){mark.dis}cm", p, drawFont, drawBrush, 0, 20, 0, 30);
            }
            left = 0;
            foreach (var mark in BottomListSort)
            {
                System.Drawing.Point p = new System.Drawing.Point(mark.x, mark.y);

                if (left <= BottomListSort.Count - 2)
                {
                    System.Drawing.Point p1 = new System.Drawing.Point(BottomListSort[left + 1].x, BottomListSort[left + 1].y);
                    e.Graphics.DrawLine(pen, p, p1);
                    left++;
                }
                commandData.drawPointCross(e.Graphics, p, pen);
                commandData.drawPointText(e.Graphics, $"({mark.name}){mark.dis}cm", p, drawFont, drawBrush, 0, 20, 1, 10);
            }
            //中间框点连线
            int min = TopListSort.Count > BottomListSort.Count ? BottomListSort.Count : TopListSort.Count;
            for (int i = 0; i < min; i++)
            {
                if (i != 0 && i != min - 1)
                {
                    continue;
                }
                TargetPoint mark = TopListSort[i];
                TargetPoint mark1 = BottomListSort[i];
                System.Drawing.Point p = new System.Drawing.Point(mark.x, mark.y);
                System.Drawing.Point p1 = new System.Drawing.Point(mark1.x, mark1.y);
                e.Graphics.DrawLine(pen, p, p1);
            }
            //鼠标画十字
            Pen pen1 = new Pen(Color.Red, 1);
            commandData.drawPointCross(e.Graphics, mouseMovePoint, pen1);
            double fenmu = 1000;
            if (isSitting)
            {
                fenmu = 10;
            }
            string LenX = (MeasureLenX / fenmu).ToString(ReservedDigitsTxt);
            string LenY = (MeasureLenY / fenmu).ToString(ReservedDigitsTxt);
            string Len = (MeasureLen / fenmu).ToString(ReservedDigitsTxt);

            pen.Color = Color.Red;
            e.Graphics.DrawLine(pen, markerTopJump, markerBottomJump);
            commandData.drawPointCross(e.Graphics, mousePoint, pen1);
            drawFont = new Font("微软雅黑", 28, FontStyle.Bold);
            drawBrush = new SolidBrush(Color.Red);// Create point for upper-left corner of drawing.
            commandData. drawPointText(e.Graphics, $"成绩:{Len}{dangwei}", new System.Drawing.Point(10, 10), drawFont, drawBrush, 1, 10, 1, 0);
            e.Graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(new System.Drawing.Point(350, 0), new System.Drawing.Size(1000, 50)));
            //时间 
            e.Graphics.DrawString($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", new Font("微软雅黑", 14), drawBrush, 350, 15);
            //考生姓名和学号
            if (StartTestingSys.IsHaveStudent(nowRaceStudentData,false))
            {
                e.Graphics.DrawString($"组号:{nowRaceStudentData.groupName} 考号:{nowRaceStudentData.idNumber} 姓名:{nowRaceStudentData.name}", new Font("微软雅黑", 14), drawBrush, 550, 15);
            }

            e.Graphics.DrawString($"X:{LenX},Y:{LenY}", new Font("微软雅黑", 14, FontStyle.Bold), new SolidBrush(Color.Blue), 1100, 15);
            if (isShotPut)
            {
                pen.DashPattern = new float[] { 5f, 10f };
                e.Graphics.DrawLine(pen, cenMarkPoint, mousePoint);
            }
        }
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            markerTmp.X =e.X ;
            markerTmp.Y = e.Y;
            mouseMovePoint.X = e.X;
            mouseMovePoint.Y = e.Y;
            if (formShowStatus > 0)
            {
                bool flag = false;
                //添加定标
                targetPoints.Add(new TargetPoint()
                {
                    x = markerTmp.X,
                    y = markerTmp.Y,
                    name = jpName,
                    dis =commandData. str2int(jpDis),//cm
                    status = jpStatus
                });
                if (targetPoints.Count == colum * 2)
                {
                    formShowStatus = 0;
                    pf1.Hide();
                    SaveMarkSetting();
                    GC.Collect();
                    flag = true;
                }
                else
                {
                    pf1.updateFlp(++nowColum);
                    jpName = nowColum + "";
                    if (nowColum % 2 != 0)
                    {
                        initDis += distance;
                    }
                    jpDis = initDis + "";
                    jpStatus = !jpStatus;
                    System.Drawing.Point ptc = this.PointToScreen(new System.Drawing.Point(e.X, e.Y - 100));
                    pf1.Location = ptc;
                }
                UpdateTargetPointListView(flag);
            }
            DispJumpLength1(e.X, e.Y);
            rgbVideoSourceRePaint();
        }

        private void SaveMarkSetting()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var t in targetPoints)
            {
                builder.Append(t.name + ";");
                builder.Append(t.x + ",");
                builder.Append(t.y + ";");
                builder.Append(t.dis + ";");
                if (t.status)
                {
                    builder.Append("1\r\n");
                }
                else
                {
                    builder.Append("0\r\n");
                }
            }
            TxtFile.Instance.write(strMainModule + markPointFile, builder.ToString());
        }

        private void pictureBox1_MouseMove(object sender,  MouseEventArgs e)
        {  
            mouseMovePoint.X = e.X;
            mouseMovePoint.Y = e.Y;
            if (formShowStatus > 0)
            {
                Task.Run(() =>
                {
                    System.Drawing.Point ptc = this.PointToScreen(new System.Drawing.Point(e.X, e.Y - 100));
                    pf1.Location = ptc;
                });
            }
            else
            {
                DispJumpLength1(e.X, e.Y);
            }
            rgbVideoSourceRePaint();

        }
        int pBox1Width= 0;
        int pBox1Height =  0;
        
        void rgbVideoSource_SizeChanged(object sender, EventArgs e)
        {
            pBox1Width = rgbVideoSource.Width;
            pBox1Height = rgbVideoSource.Height;
        }

        private bool rgbVideoSourcePaintFlag = false;   
        void rgbVideoSource_Paint(object sender, EventArgs e)
        {
            skipFrameDispR0++;
            if (rgbVideoSourcePaintFlag) return;
            rgbVideoSourcePaintFlag = true;
            //float offsetX = pBox1Width * 1f / bmp.Width;
            //float offsetY = pBox1Height * 1f / bmp.Height; 
            try
            {
                if (skipFrameDispR0 < cameraSkip)
                {
                    return;
                }
                skipFrameDispR0 = 0;
                if (rgbVideoSource.IsRunning)
                {
                    //得到当前RGB摄像头下的图片
                    Bitmap bmp = rgbVideoSource.GetCurrentVideoFrame();
                    if (bmp == null)
                    {
                        return;
                    }
                    //处理镜像
                    if (isMirrorMode)
                    {
                        bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    }
                    //是否写入
                    if (VideoOutPut != null && VideoOutPut.IsOpened())
                    {
                        Bitmap bitmap = ImageHelper.DeepCopyBitmap(bmp);
                        OpenCvSharp.Mat mat = ImageHelper.Bitmap2Mat(bitmap);
                        VideoOutPut.Write(mat);
                    }
                    ///处理录像数据
                    backBp = ImageHelper.DeepCopyBitmap(bmp);
                    FuseBitmap fb = null;
                    Bitmap dstBitmap = null;
                    if (isSitting)
                    {
                        fb = new FuseBitmap(bmp);
                        fb.SetRect(conPoints0);
                        fb.FuseColorImg(isShowHandFlag);
                        fb.Dispose();
                        dstBitmap = fb.dstBitmap;
                    }
                    if (recTimeR0 > 0)
                    {
                        if (isBallCheckBoxb)
                        {
                            Bitmap bp = ImageHelper.DeepCopyBitmap(bmp);
                            ImageAndIndex iai = new ImageAndIndex();
                            iai.index = frameSum;
                            iai.image = bp;
                            lock (ImageQueuesLock)
                            {
                                ImageQueues.Enqueue(iai);
                            }
                        }
                        imgMsS mss = new imgMsS();
                        mss.dt = DateTime.Now;
                        mss.name = "img" + imgMs.Count;
                        //坐位体前屈
                        if (isSitting && fb != null && dstBitmap != null)
                        {
                            mss.img = dstBitmap;
                            mss.isHand = fb.isHand;
                        }
                        else
                        {
                            Bitmap bitmap = ImageHelper.DeepCopyBitmap(bmp);
                            mss.img = bitmap;
                        }
                        imgMs.Add(mss);
                        frameSum++;
                    }
                    if (isSitting && fb != null && dstBitmap != null)
                    {
                        bmp = ImageHelper.DeepCopyBitmap(dstBitmap);
                    }
                    frameRecSum++;//计算帧速用

                    //显示图片
                    pictureBox1.Image = bmp;
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
            finally
            {
                GCCounta++;
                if (GCCounta > 10)
                {
                    GCCounta = 0;
                    GC.Collect();
                }
                rgbVideoSourcePaintFlag = false;
            }
        }
        void rgbVideoSource_MouseDown(object sender, EventArgs e)
        {

        }
        void rgbVideoSource_MouseMove(object sender, EventArgs e)
        {

        }
        void RoundCountCbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RoundCountCbx.SelectedIndex != -1)
            {
                RoundCount0 = RoundCountCbx.SelectedIndex + 1;
                UpdateListView(_ProjectId, _GroupName, RoundCount0);
                label7.Text = RoundCountCbx.Text;
                nowRaceStudentData = new  SavaStudentData();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void groupCbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                ControlHelper.ThreadInvokerControl(this, () =>
                {
                    _GroupName = groupCbx.Text;
                    label13.Text = "当前测试组:" + _GroupName;
                    if (RoundCount0 > 0)
                    {
                        UpdateListView(_ProjectId, _GroupName, RoundCount0);
                        nowRaceStudentData = new SavaStudentData();
                    }

                });
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }
        void projectTypeCbx_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (projectTypeCbx.Text.Trim() == "立定跳远")
            {
                isBallCheckBoxb = false;
                isShowImgList = true;
                isShotPut = false;
                isSitting = false;
                checkBox2.Visible = false;
                button7.Visible = false;
                button11.Visible = false;
            }
            else if (projectTypeCbx.Text.Trim() == "投掷实心球")
            {
                isBallCheckBoxb = true;
                isShowImgList = false;
                isShotPut = false;
                isSitting = false;
                checkBox2.Visible = false;
                button7.Visible = false;
                button11.Visible = false;
            }
            else if (projectTypeCbx.Text.Trim() == "坐位体前屈")
            {
                isBallCheckBoxb = false;
                isShowImgList = false;
                isShotPut = false;
                isSitting = true;
                checkBox2.Visible = true;
                checkBox2.Checked = true;
                button7.Visible = true;
                button11.Visible = true;
                dangwei = "厘米";
            }
            else if (projectTypeCbx.Text.Trim() == "投掷铅球")
            {
                isBallCheckBoxb = true;
                isShowImgList = false;
                isShotPut = true;
                isSitting = false;
                checkBox2.Visible = false;
                button7.Visible = false;
                button11.Visible = false;
            }


        }
        bool isShowHandFlag = false;
        void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            isShowHandFlag = checkBox2.Checked;
        }
        void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            isMirrorMode = checkBox1.Checked;
        }
        void 弃权ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartTestingSys.SetErrorgrade(stuView,ADCoreSqlite, RoundCount0, "弃权");
        }
        void 成绩查询ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (stuView.SelectedRows.Count == 0)
                {
                    FrmTips.ShowTipsError(this, "未选择考生");
                    return;
                }
                string idNumber = stuView.SelectedRows[0].Cells[1].Value.ToString();
                string name = stuView.SelectedRows[0].Cells[2].Value.ToString();
                string nowTestDir1 = $"\\{_projectName}\\{_GroupName}\\{idNumber}_{name}\\第{RoundCount0}轮";
                nowTestDir1 = ScoreDir + nowTestDir1;
                if (Directory.Exists(nowTestDir1))
                {
                    System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo("Explorer.exe");
                    psi.Arguments = "/e,/select," + nowTestDir1;
                    System.Diagnostics.Process.Start(psi);
                }
                else
                {
                    FrmTips.ShowTipsError(this, "未找到文件夹");
                }
            }
            catch (Exception ex)
            {

                LoggerHelper.Debug(ex);
            }
        }
        private void stuView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0)
                {
                    stuView.ClearSelection();
                    stuView.Rows[e.RowIndex].Selected = true;
                    stuView.CurrentCell = stuView.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }
        private void stuView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            nowRaceStudentData = new SavaStudentData();
            if (stuView.SelectedRows.Count == 0)
            {
                return;
            }
            try
            {
                DataGridViewRow dataGridViewRow = stuView.SelectedRows[0];
                //序号 考号 姓名 成绩 考试状态 上传状态 唯一编号
                nowRaceStudentData.id = dataGridViewRow.Cells[6].Value.ToString();
                nowRaceStudentData.name = dataGridViewRow.Cells[2].Value.ToString();
                nowRaceStudentData.idNumber = dataGridViewRow.Cells[1].Value.ToString();
                String stateStr = dataGridViewRow.Cells[4].Value.ToString();
                int stateInt = ResultState.ResultState2Int(stateStr);
                nowRaceStudentData.state = stateInt;
                nowRaceStudentData.groupName = _GroupName;
                nowRaceStudentData.RoundId = RoundCount0;
                StartTestingSys.SendScore(nowRaceStudentData.name, "准备考试", "");
            }
            catch (Exception ex)
            {
                nowRaceStudentData = new SavaStudentData();
                LoggerHelper.Debug(ex);
            }
            finally
            {
                pictureBox1.Refresh();
            }
        }
        void 缺考ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartTestingSys.SetErrorgrade(stuView,ADCoreSqlite, RoundCount0, "缺考");
        }
        void 中途退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartTestingSys.SetErrorgrade(stuView, ADCoreSqlite, RoundCount0, "中退");
        }
        void 犯规ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartTestingSys.SetErrorgrade(stuView, ADCoreSqlite,RoundCount0, "犯规");
        }
        void timer1_Tick(object sender, EventArgs e)
        {

            double v = MemoryTool.GetProcessUsedMemory();
            if (v > 100)
            {
                MemoryTool.ClearMemory();
            }

            frameSpeed_txt.Text = "fps:" + frameRecSum;
            frameRecSum = 0;
        }
        void timer2_Tick(object sender, EventArgs e)
        {
            if (RecordEnd == 1)
            {
                if (remainderImgSum > 0)
                {
                    imgProgressBar.Maximum = remainderImgSum;
                    imgProgressBar.Value = remainderImgCount;
                }

            }
            else if (RecordEnd == 2)
            {
                if (remainderImgSum > 0)
                {
                    imgProgressBar.Maximum = remainderImgSum;
                    imgProgressBar.Value = remainderImgSum;
                }

                RecordEnd = 0;
            }
            if (recTimeR0 > 0)
            {
                recTimeR0--;
                if (recTimeR0 == 0)
                {
                    try
                    {

                        rgbVideoSourceStop();
                        nowRaceStudentData.state = 1;
                        ControlHelper.ThreadInvokerControl(this, () =>
                        {
                            button12.Text = "开始录像(空格)";
                            button12.BackColor = Color.White;
                        });
                        recTime.Text = "文件数:" + frameSum + "";
                        videoSourceRuningR0 = 0;//播放中的心跳包
                                                //录像结束进行操作
                                                //打开图片集
                        RecordEnd = 1;
                        ReleaseVideoOutPut();
                        if (isShowImgList)
                        {
                            openImgList();
                        }
                        else
                        {
                            if (isSitting)
                            {
                                int nlen = imgMs.Count;
                                int maxW = 0;
                                int maxH = 0;
                                int index = 0;
                                int minWidth = conPoints0[0].X;
                                int maxWidth = conPoints0[3].X;
                                int minHeigh = conPoints0[0].Y;
                                int maxHeigh = conPoints0[3].Y;
                                int anaylistLen = 10;
                                int[] anaylist = new int[anaylistLen];
                                OpenCvSharp.Point[] anaPoint = new OpenCvSharp.Point[nlen];
                                for (int k = 0; k < nlen; k++)
                                {
                                    bool[][] isHand = imgMs[k].isHand;
                                    //maxW = 0;
                                    //maxH = 0;
                                    int maxwT = 0;
                                    int maxhT = 0;
                                    if (isHand == null) continue;
                                    for (int i = minWidth; i < maxWidth; i++)
                                    {
                                        for (int j = minHeigh; j < maxHeigh; j++)
                                        {
                                            if (isHand[i][j])
                                            {
                                                int nl = i - 5;
                                                if (nl < 0) nl = 0;
                                                bool flag0 = true;
                                                for (int i1 = nl; i1 < i; i1++)
                                                {
                                                    if (!isHand[i1][j])
                                                    {
                                                        flag0 = false; break;
                                                    }
                                                }
                                                if (!flag0) continue;
                                                //当前图片最大值
                                                maxwT = i;
                                                maxhT = j;
                                                //全局最大值
                                                if (i > maxW)
                                                {
                                                    maxW = i;
                                                    maxH = j;
                                                    index = k;
                                                }
                                            }
                                        }
                                    }
                                    anaPoint[k] = new OpenCvSharp.Point(maxwT, maxhT);
                                }
                                string Log = $"最大值:index:{index},X:{maxW}  y:{maxH}";
                               StartTestingSys. WriteLog(lrtxtLog, Log, 0);
                                int iFori = -1;
                                int iFlag = 0;
                                for (int i = index; i > 0; i--)
                                {
                                    bool isBreakForj = true;
                                    int ax = anaPoint[i].X;
                                    int end = (index + 2) > nlen - 1 ? nlen - 1 : index + 2;
                                    int begin = (index - 2) < 0 ? 0 : index - 2;
                                    for (int j = begin; j < end; j++)
                                    {
                                        int ipx = Math.Abs(ax - anaPoint[j].X);
                                        if (ipx > 3) isBreakForj = false; break;
                                    }
                                    if (isBreakForj)
                                    {
                                        iFori = i; break;
                                    }
                                }
                                
                                if (iFori != -1)
                                {
                                    int maxwT = anaPoint[iFori].X;
                                    int maxhT = anaPoint[iFori].Y;
                                    if ((maxwT < maxWidth && maxwT > minWidth) && maxhT < maxHeigh && maxhT > minHeigh)
                                    {
                                        maxW = maxwT;
                                        maxH = maxhT;
                                        index = iFori;
                                    }
                                }
                                Log = $"筛选值:index:{index},X:{maxW}  y:{maxH}";
                              StartTestingSys.  WriteLog(lrtxtLog, Log, 0);

                                if (imgMs[index] != null && imgMs[index].img != null)
                                {
                                    nowFileName = index + "";
                                    ControlHelper.ThreadInvokerControl(this, () =>
                                    {
                                        pictureBox1.Image = imgMs[index].img;
                                        recImgIndex.Text = $"图片索引:{nowFileName}";
                                        pictureBox1.Refresh();

                                    });

                                }
                                StartMeasure();
                                DispJumpLength1(maxW, maxH);
                                pictureBox1.Refresh();
                                StopMeasure();

                            }
                            else
                            {
                                SetHScrollBarValue(imgMs.Count - 1);
                            }

                        }
                    }
                    catch (Exception ex)
                    {

                        LoggerHelper.Debug(ex);
                    }
                }
                else
                {
                    recTime.Text = "REC " + (recTimeR0 + 10) / 10;
                }
            }
        }
        private void ReleaseVideoOutPut()
        {
            try
            {
                if (VideoOutPut != null && VideoOutPut.IsOpened())
                {
                    VideoOutPut.Release();
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }
        }

        private void rgbVideoSourceStop()
        {
            StartTestingSys.CloseCamera(opencameraBtn,rgbVideoSource);
        }

        void SetHScrollBarValue(int value)
        {
            ControlHelper.ThreadInvokerControl(this, () =>
            {
                hScrollBar1.Value = 0;
                hScrollBar1.Maximum = imgMs.Count - 1;
                if (value > imgMs.Count - 1)
                {
                    value = imgMs.Count - 1;
                }
                if (value < 0)
                {
                    value = 0;
                }
                hScrollBar1.Value = value;
            });
        }
        #endregion

        #region  页面显示
        /// <summary>
        /// 更新标点显示
        /// </summary>
        /// <param name="flag"></param>
        private void UpdateTargetPointListView(bool flag = true)
        {
            //gfencePnts
            gfencePnts.Clear();
            gfencePntsDisValue.Clear();
            List<TargetPoint> TopList = targetPoints.FindAll(a => a.status);
            List<TargetPoint> BottomList = targetPoints.FindAll(a => !a.status);
            TopListSort.Clear();
            BottomListSort.Clear();
            TopListSort = TopList.OrderBy(a => a.x).ToList();
            BottomListSort = BottomList.OrderBy(a => a.x).ToList();

            int min = TopListSort.Count > BottomListSort.Count ? BottomListSort.Count : TopListSort.Count;
            int max = TopListSort.Count > BottomListSort.Count ? TopListSort.Count : BottomListSort.Count;
            for (int i = 0; i < min; i++)
            {
                if (i <= min - 2)
                {
                    System.Drawing.Point[] fpnts = new System.Drawing.Point[4];
                    int[] DisValue = new int[2];
                    System.Drawing.Point p = new System.Drawing.Point(TopListSort[i].x, TopListSort[i].y);
                    System.Drawing.Point p1 = new System.Drawing.Point(TopListSort[i + 1].x, TopListSort[i + 1].y);
                    System.Drawing.Point p2 = new System.Drawing.Point(BottomListSort[i].x, BottomListSort[i].y);
                    System.Drawing.Point p3 = new System.Drawing.Point(BottomListSort[i + 1].x, BottomListSort[i + 1].y);
                    int maxv = 0;
                    int minv = 0;
                    minv = Math.Min(BottomListSort[i].dis, BottomListSort[i + 1].dis);
                    maxv = Math.Max(BottomListSort[i].dis, BottomListSort[i + 1].dis);
                    DisValue[0] = minv;
                    DisValue[1] = maxv;
                    fpnts[0] = p;
                    fpnts[1] = p1;
                    fpnts[2] = p2;
                    fpnts[3] = p3;
                    gfencePnts.Add(fpnts);
                    gfencePntsDisValue.Add(DisValue);
                }
            }
            if (gfencePnts.Count > 0 && gfencePnts[0].Length > 2)
            {
                System.Drawing.Point bottomStartP = gfencePnts[0][0];
                System.Drawing.Point bottomEndP = gfencePnts[0][2];
                int x = (bottomStartP.X + bottomEndP.X) / 2;
                int y = (bottomStartP.Y + bottomEndP.Y) / 2;
                cenMarkPoint = new System.Drawing.Point(x, y);
                Console.WriteLine();
            }
            if (flag)
            {
                //gfencePnts
                int igfInde = gfencePnts.Count - 1;
                conPoints0[0] = new OpenCvSharp.Point(gfencePnts[0][0].X, gfencePnts[0][0].Y);
                conPoints0[1] = new OpenCvSharp.Point(gfencePnts[0][2].X, gfencePnts[0][2].Y);
                conPoints0[2] = new OpenCvSharp.Point(gfencePnts[igfInde][1].X, gfencePnts[igfInde][1].Y);
                conPoints0[3] = new OpenCvSharp.Point(gfencePnts[igfInde][3].X, gfencePnts[igfInde][3].Y);
                //OpenCvSharp.Rect rect = Cv2.BoundingRect(contours[i]);
                OpenCvSharp.Rect rect0 = Cv2.BoundingRect(conPoints0);
                conPoints0[0] = rect0.TopLeft;
                conPoints0[1] = new OpenCvSharp.Point(rect0.X + rect0.Width, rect0.Y);
                conPoints0[2] = new OpenCvSharp.Point(rect0.X, rect0.Y + rect0.Height);
                conPoints0[3] = new OpenCvSharp.Point(rect0.X + rect0.Width, rect0.Y + rect0.Height);
            }
        }
       
        
        /// <summary>
        /// 更新名单视图
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="groupName"></param>
        /// <param name="v"></param>
        bool UpdateListViewRun = false;
        private void UpdateListView(string projectId, string groupName, int roundid)
        {
            if (UpdateListViewRun) return;
            StartTestingSys.UpdateListView(stuView, projectId, groupName, roundid, ADCoreSqlite, out UpdateListViewRun);
            
        }
        int sleepCount = 0;
        int RecordEnd = 0;
        
        //录像结束剩余图片总数
        int remainderImgSum = 0;
        //录像结束 未处理图片数
        int remainderImgCount = 0;

        

         
        void ImagePredictLabelQueues2ThreadFun(object obj)
        {
            while (true)
            {
                if (ImageQueues.Count == 0)
                {
                    Thread.Sleep(10);
                    sleepCount++;
                    if (sleepCount > 10)
                    {
                        Thread.Sleep(100);
                    }
                    if (RecordEnd == 1)
                    {
                        //剩余图片处理结束
                        RecordEnd = 2;
                        fuseImg.Dispose();
                        Bitmap dstcopy0 = FuseImg.DeepCopyBitmap(fuseImg.dstBitmap);
                        imgMsS mss = new imgMsS();
                        mss.img = dstcopy0;
                        mss.dt = DateTime.Now;
                        mss.name = "img" + imgMs.Count;
                        imgMs.Add(mss);
                        SetHScrollBarValue(imgMs.Count - 1);
                    }
                }
                else
                {
                    sleepCount = 0;
                    //处理剩余图片
                    if (RecordEnd == 1)
                    {
                        remainderImgSum = ImageQueues.Count;
                        remainderImgCount = 0;
                        bool flag = false;
                        ImageAndIndex iplr = new ImageAndIndex();
                        lock (ImageQueuesLock)
                        {
                            flag = ImageQueues.TryDequeue(out iplr);
                        }
                        while (flag)
                        {
                            remainderImgCount++;
                            fuseImg.FuseColorImg1(iplr.image);
                            lock (ImageQueuesLock)
                            {
                                flag = ImageQueues.TryDequeue(out iplr);
                            }
                        }
                        //剩余图片处理结束
                        RecordEnd = 2;
                        fuseImg.Dispose();
                        Bitmap dstcopy0 = FuseImg.DeepCopyBitmap(fuseImg.dstBitmap);

                        imgMsS mss = new imgMsS();
                        mss.img = dstcopy0;
                        mss.dt = DateTime.Now;
                        mss.name = "img" + imgMs.Count;
                        imgMs.Add(mss);
                        SetHScrollBarValue(imgMs.Count - 1);
                        StartMeasure();
                    }
                    else
                    {
                        bool flag = false;
                        ImageAndIndex iplr = new ImageAndIndex();
                        lock (ImageQueuesLock)
                        {
                            flag = ImageQueues.TryDequeue(out iplr);
                        }
                        if (flag)
                        {
                            fuseImg.FuseColorImg1(iplr.image);
                        }
                    }

                }
            }
        }
        #endregion
        /// <summary>
        /// 设置初始信息
        /// </summary>
        /// <param name="fsp"></param>
        /// <param name="dic"></param>
        /// <param name="aDCoreSqlite"></param>
         
        public void SetInitdata(string[] fsp, Dictionary<string, string> dic, ADCoreSqlite aDCoreSqlite)
        {
            ADCoreSqlite = aDCoreSqlite;
            _projectName = fsp[0];
            if (fsp.Length >1)
            {
                _GroupName = fsp[1];
            }
            _ProjectId = dic["Id"];
            _Type = dic["Type"];
            _RoundCount = Convert.ToInt32(dic["RoundCount"]);
            _BestScoreMode = Convert.ToInt32(dic["BestScoreMode"]);
            _TestMethod = Convert.ToInt32(dic["TestMethod"]);
            _FloatType = Convert.ToInt32(dic["FloatType"]);
            formTitle = string.Format("考试项目:{0}", fsp[0]);
        }
        private void Opencamera(string cameraName, int width, int height)
        {
            if (string.IsNullOrEmpty(cameraName))
            {
                FrmTips.ShowTipsError(this, "打开相机失败！！");
                return;
            }
            else
            {
                StartTestingSys.OpenCamera(opencameraBtn, cameraName, rgbVideoSource, _height, _width);
            }

        }
        System.Drawing.Point mousePoint;
        private void DispJumpLength1(int x3, int y3)
        {
             if (Measure == false) return;//不测量
            if (recTimeR0 > 0) return;
            mousePoint.X = x3;
            mousePoint.Y = y3;
            for (int i = 0; i < gfencePnts.Count; i++)
            {
                int angle2_x1 = commandData.judgeSide(gfencePnts[i][0], gfencePnts[i][2], mousePoint);
                int angle2_x2 = commandData.judgeSide(gfencePnts[i][1], gfencePnts[i][3], mousePoint);
                //在左边界右,在右边界左
                if (angle2_x1 > 0 && angle2_x2 < 0)
                {
                    double pixLength1 = (gfencePntsDisValue[i][1] - gfencePntsDisValue[i][0]) * 10;
                    double baseLen = Convert.ToDouble(gfencePntsDisValue[i][0]);
                    commandData.DrawMousePointLine1(mousePoint, gfencePnts[i][0], gfencePnts[i][1], gfencePnts[i][2], gfencePnts[i][3],
                        ref markerTopJump, ref markerBottomJump, baseLen, pixLength1, ref MeasureLenX);
                    break;
                }
            }
            try
            {
                commandData.VerticalDistance(gfencePnts, mousePoint, ref markerTopJumpY, ref markerBottomJumpY, ref MeasureLenY, gfencePntsDisValue);

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }

            try
            {
                MeasureLen = MeasureLenX;
                //是否铅球
                if (isShotPut)
                {
                    double distancey = 0;
                    double distancex = MeasureLenX;
                    int personY = cenMarkPoint.Y;
                    if (mousePoint.Y <= personY)
                    {
                        //distancey = (str2double("0.4") / 2) * 1000 - MeasureLenY;
                        distancey = 2000 - MeasureLenY;
                    }
                    else
                    {
                        //distancey = MeasureLenY - (str2double("0.4") / 2) * 1000;
                        distancey = MeasureLenY - 2000;
                    }
                    MeasureLen = Math.Sqrt((distancey * distancey) + (distancex * distancex));
                }

            }
            catch (Exception ex)
            {
                LoggerHelper.Debug(ex);
            }

            return;
        }


         
    }
} 
