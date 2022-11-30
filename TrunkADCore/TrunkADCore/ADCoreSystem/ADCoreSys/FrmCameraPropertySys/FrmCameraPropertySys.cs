using AForge.Video.DirectShow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrunkADCore.ADCoreWindow;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TrunkADCore.ADCoreSystem.ADCoreSys
{
    public class FrmCameraPropertySys
    {
        public FilterInfoCollection filterInfoCollection = null;
        FrmCameraProperty frmCameraProperty = null;
        public static bool IsSetting = false;
        public int _width = 1280;
        public int _height = 720;
        public int maxFps = 0;

        List<string> FpsList = new List<string>();
        static CameraSettingBack CameraSettingBack = null;

        /// <summary>
        /// 加载相机
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void LoadCameraList(Sunny.UI.UIComboBox CameraDrop)
        {
            CameraDrop.Items.Clear();
            //设置视频来源
            try
            {
                // 枚举所有视频输入设备
                filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);

                if (filterInfoCollection.Count == 0)
                    throw new ApplicationException();   //没有找到摄像头设备
                foreach (FilterInfo device in filterInfoCollection)
                {
                    if (device.Name.Contains("Web"))
                    {
                        continue;
                    }
                    CameraDrop.Items.Add(device.Name);
                }
                if (CameraDrop.Items.Count > 0)
                {
                    CameraDrop.SelectedIndex = CameraDrop.Items.Count - 1;
                }
                else
                {

                }
            }
            catch (ApplicationException ex)
            {
                filterInfoCollection = null;
                CameraDrop.Items.Clear();
            }
        }
        
        /// <summary>
        /// 选择相机
        /// </summary>
        /// <param name="cameraName"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void ChooseCamera(string name, Sunny.UI.UIComboBox comboBox1)
        {
            FpsList.Clear();
            foreach (FilterInfo device in filterInfoCollection)
            {
                if (device.Name == name)
                {
                    VideoCaptureDevice rgbDeviceVideo = new VideoCaptureDevice(device.MonikerString);
                    for (int i = 0; i < rgbDeviceVideo.VideoCapabilities.Length; i++)
                    {
                        if (rgbDeviceVideo.VideoCapabilities[i].FrameSize.Width == _width
                            && rgbDeviceVideo.VideoCapabilities[i].FrameSize.Height == _height)
                        {
                            //rgbDeviceVideo.VideoResolution = rgbDeviceVideo.VideoCapabilities[i];
                            string fps = rgbDeviceVideo.VideoCapabilities[i].AverageFrameRate + "";
                            if (!FpsList.Contains(fps))
                                FpsList.Add(fps);
                            break;
                        }
                    }
                    break;
                }
            }
            if (FpsList.Count == 0)
            {
                foreach (FilterInfo device in filterInfoCollection)
                {
                    if (device.Name == name)
                    {
                        VideoCaptureDevice rgbDeviceVideo = new VideoCaptureDevice(device.MonikerString);
                        for (int i = 0; i < rgbDeviceVideo.VideoCapabilities.Length; i++)
                        {
                            if (rgbDeviceVideo.VideoCapabilities[i].FrameSize.Width == 1920
                                && rgbDeviceVideo.VideoCapabilities[i].FrameSize.Height == 1080)
                            {
                                //rgbDeviceVideo.VideoResolution = rgbDeviceVideo.VideoCapabilities[i];
                                string fps = rgbDeviceVideo.VideoCapabilities[i].AverageFrameRate + "";
                                if (!FpsList.Contains(fps))
                                    FpsList.Add(fps);
                                break;
                            }
                        }
                        break;
                    }
                }
            }
            comboBox1.Items.Clear();
            foreach (var item in FpsList)
            {
                int.TryParse(item, out int fps);
                maxFps = fps;
                fps /= 2;
                while (fps >= 30)
                {
                    if (fps >= 30)
                        comboBox1.Items.Add(fps + "fps");
                    fps /= 2;
                }
                comboBox1.Items.Add(maxFps + "fps");
                break;
            }
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public  void ShowFrmCameraProperty()
        {
            frmCameraProperty = new FrmCameraProperty();
            frmCameraProperty.ShowDialog();
        }
        public  bool CheackIsSetting()
        {
            return IsSetting;
        }

        public CameraSettingBack GetSettingBack()
        {
            return CameraSettingBack;
        }
        /// <summary>
        /// /
        /// </summary>
        /// <param name="cameraName"></param>
        /// <param name="fps"></param>
        public  void SetCameraSettingBack(string cameraName, int fps)
        {
            CameraSettingBack = new CameraSettingBack()
            {
                CameraName = cameraName,
                maxFps = maxFps,
                Fps = fps,
            };
        }
    }
}
