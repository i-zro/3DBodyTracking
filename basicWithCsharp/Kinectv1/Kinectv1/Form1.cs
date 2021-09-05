using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Azure.Kinect.Sensor;

namespace Kinectv1
{
    public partial class Form1 : Form
    {
        //키넥트 변수
        Device Kinect;
        public Form1()
        {
            InitializeComponent();
            initKinect();
        }

        private void initKinect()
        {
            Kinect = Device.Open(0);

            //키넥트 모드 설정
            Kinect.StartCameras(new
                DeviceConfiguration
            {
                ColorFormat = ImageFormat.ColorBGRA32,
                ColorResolution = ColorResolution.R720p,
                DepthMode = DepthMode.NFOV_2x2Binned,
                CameraFPS = FPS.FPS30
            });
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Kinect.StopCameras();
        }
    }
}
