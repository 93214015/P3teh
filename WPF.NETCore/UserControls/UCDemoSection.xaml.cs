using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Media.Animation;
using System.Threading.Tasks;

namespace WPF.NETCore.UserControls
{
    /// <summary>
    /// Interaction logic for UCDemoSection.xaml
    /// </summary>
    public partial class UCDemoSection : UserControl
    {

        VideoCapture mVideoCapture;

        Image _CurrentImage;
        Image _NextImage;

        Storyboard _STBShowResultImage;
        Storyboard _STBHideResultImage;

        public UCDemoSection()
        {
            InitializeComponent();

            _CurrentImage = ImgDemo;
            _NextImage = ImgDemo2;

            _STBShowResultImage = (Storyboard)this.FindResource("STBShowResultImage");
            _STBHideResultImage = (Storyboard)this.FindResource("STBHideResultImage");
        }

        public async void PowerCamera()
        {
            if (mVideoCapture == null)
            {
                var task = Task.Run(() => { return new VideoCapture(0, VideoCapture.API.Any); });
                mVideoCapture = await task;
                mVideoCapture.FlipHorizontal = true;
                mVideoCapture.ImageGrabbed += MVideoCapture_ImageGrabbed; ;
                await Task.Run(() => mVideoCapture.Start());
            }
            else
            {
                StopCamera();
            }

        }

        public void StopCamera()
        {
            if (mVideoCapture != null)
            {
                mVideoCapture.ImageGrabbed -= MVideoCapture_ImageGrabbed;
                mVideoCapture.Stop();
                mVideoCapture = null;
            }
        }

        private void MVideoCapture_ImageGrabbed(object sender, EventArgs e)
        {

            try
            {
                Mat _Image = new Mat();
                mVideoCapture.Retrieve(_Image);
                var _Bitmap = _Image.ToImage<Bgr, Byte>().ToBitmap();

                this.Dispatcher.Invoke(() =>
                {
                    var _ImageSoruce = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                       _Bitmap.GetHbitmap(),
                                       IntPtr.Zero,
                                       System.Windows.Int32Rect.Empty,
                                       BitmapSizeOptions.FromWidthAndHeight(_Bitmap.Width, _Bitmap.Height));
                    CameraPicture.Source = _ImageSoruce;

                });

                Thread.Sleep(16);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }

        public static readonly DependencyProperty ImageSourceDemoProperty = DependencyProperty.Register("ImageSourceDemo", typeof(ImageSource), typeof(UCDemoSection));

        public ImageSource ImageSourceDemo
        {
            get { return (ImageSource)GetValue(ImageSourceDemoProperty); }
            set { SetValue(ImageSourceDemoProperty, value); }
        }


        int openedMouth = 1;
        int closedMouth = 0;

        public void ShowResult()
        {
            
            foreach (var anim in _STBHideResultImage.Children)
            {
                Storyboard.SetTarget(anim, _CurrentImage);
            }

            foreach (var anim in _STBShowResultImage.Children)
            {
                Storyboard.SetTarget(anim, _NextImage);
            }

            _STBHideResultImage.Begin();
            _STBShowResultImage.Begin();

            Image _Temp = _CurrentImage;
            _CurrentImage = _NextImage;
            _NextImage = _Temp;


            TxtTotalCount.Text = (int.Parse(TxtTotalCount.Text) + 1).ToString();
            TxtOpenedMouthCount.Text = (int.Parse(TxtOpenedMouthCount.Text) + openedMouth).ToString();
            TxtClosedMouthCount.Text = (int.Parse(TxtClosedMouthCount.Text) + closedMouth).ToString();

            closedMouth = (closedMouth + 1) % 2;
            openedMouth = (openedMouth + 1) % 2;
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ImgDemo.Source = ImageSourceDemo;

            Task.Run(()=> 
            { 
                while (true)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        ShowResult();

                    });  
                    Thread.Sleep(2000); 
                } 
            });
        }

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            ShowResult();
        }
    }
}
