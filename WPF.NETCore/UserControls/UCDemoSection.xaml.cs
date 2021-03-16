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

namespace WPF.NETCore.UserControls
{
    /// <summary>
    /// Interaction logic for UCDemoSection.xaml
    /// </summary>
    public partial class UCDemoSection : UserControl
    {

        VideoCapture mVideoCapture;

        public UCDemoSection()
        {
            InitializeComponent();

        }

        public void PowerCamera()
        {
            if (mVideoCapture == null)
            {
                mVideoCapture = new VideoCapture(0, VideoCapture.API.Any);
                mVideoCapture.FlipHorizontal = true;
                mVideoCapture.ImageGrabbed += MVideoCapture_ImageGrabbed; ;
                mVideoCapture.Start();
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ImgDemo.Source = ImageSourceDemo;
        }
    }
}
