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

// 키넥트에서 취득한 이미지의 포인터 접근용
using System.Buffers;

// AzureKinect와 System 변수 이름의 모호성을 없애는 용
using Image = Microsoft.Azure.Kinect.Sensor.Image;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using System.Diagnostics;
namespace Kinectv1
{
    public partial class Form1 : Form
    {
        // 키넥트 변수
        Device Kinect;
        // 컬러 이미지 Bitmap
        Bitmap colorBitmap;
        /*키넥트 이미지 획득 여부
        당연히 이미지는 계속 얻어야 하니까 그거 반복위한 변수*/
        bool loop = true;

        public Form1()
        {
            InitializeComponent();
            InitKinect();

            //키넥트 설정 정보에 따른 Bitmap 관련 정보 초기화
            InitBitmap();
            Task t = KinectLoop();
        }

        //비동기식 async로 데이터획득 할 것
        private async Task KinectLoop()
        {
            while(loop)
            {
                //Kinect에서 새로운 데이터를 받음
                using (Capture capture = await Task.Run(() =>
                Kinect.GetCapture()).ConfigureAwait(true))
                { //안전하지 않은 코드 사용
                    unsafe
                        {
                        //컬러 이미지를 얻음
                        Image colorImage = capture.Color;
                        //이미지의 메모리 주소를 얻음
                        using(MemoryHandle pin = colorImage.Memory.Pin()) 
                        {
                            //Bitmap 이미지 생성
                            colorBitmap = new Bitmap(
                                colorImage.WidthPixels,
                                colorImage.HeightPixels,
                                colorImage.StrideBytes,
                                PixelFormat.Format32bppArgb,
                                (IntPtr)pin.Pointer);
                        }
                    }
                    //pictureBox에 생성한 이미지를 붙여넣음
                    pictureBox1.Image = colorBitmap;
                }
                //이미지 표시를 업데이트
                this.Update();
            }
            //루프가 끝나면 Kinect도 정지
            Kinect.StopCameras();
        }

        // Bitmap 이미지에 대한 초기 설정
        private void InitBitmap()
        {
            // 컬러 이미지의 가로 폭과 세로 폭 취득
            int width = Kinect.GetCalibration().ColorCameraCalibration.ResolutionWidth;
            int height = Kinect.GetCalibration().ColorCameraCalibration.ResolutionHeight;

            // Bitmap 이미지 생성
            colorBitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        }

        private void InitKinect()
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
            // 키넥트에서 반복 멈추도록 하면 카메라 멈춤
            loop = false;
            Kinect.StopCameras();
        }
    }
}
