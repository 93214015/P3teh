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

        BitmapImage _BitmapOpenedMouth = new BitmapImage();
        BitmapImage _BitmapClosedMouth = new BitmapImage();

        List<Image> _ListResultImages = new List<Image>();

        Storyboard _STBShowResultImage;
        Storyboard _STBHideResultImage;
        Storyboard _STBShowResultImagePopIn;

        public UCDemoSection()
        {
            InitializeComponent();


            _BitmapOpenedMouth.BeginInit();
            _BitmapOpenedMouth.UriSource = new Uri("pack://application:,,,/Images/DemoImage.png");
            _BitmapOpenedMouth.EndInit();

            _BitmapClosedMouth.BeginInit();
            _BitmapClosedMouth.UriSource = new Uri("pack://application:,,,/Images/DemoImage2.png");
            _BitmapClosedMouth.EndInit();


            _STBShowResultImage = (Storyboard)this.FindResource("STBShowResultImage");
            _STBHideResultImage = (Storyboard)this.FindResource("STBHideResultImage");
            _STBShowResultImagePopIn = (Storyboard)this.FindResource("STBShowResultPopIn");
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

        int _rand = 0;
        int closedMouth = 0;
        int openedMouth = 1;

        public void ShowResult()
        {
            var transformGroup = new TransformGroup();
            transformGroup.Children.Add(new ScaleTransform(0, 0));

            Image _Image = new Image()
            {
                Width = 50,
                Height = 50,
                Opacity = 0,
                RenderTransform = transformGroup,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            foreach (var anim in _STBShowResultImagePopIn.Children)
            {
                Storyboard.SetTarget(anim, _Image);
            }

            _rand = (_rand + 1) % 2;

            if (_rand == 0)
            {
                _Image.Source = _BitmapOpenedMouth;
            }

            if (_rand == 1)
            {
                _Image.Source = _BitmapClosedMouth;
            }

            StackPanel_ResultImages.Children.Insert(0, _Image);

            _STBShowResultImagePopIn.Begin();

            if (StackPanel_ResultImages.Children.Count > 10)
            {
                StackPanel_ResultImages.Children.RemoveAt(10);
            }


            TxtTotalCount.Text = (int.Parse(TxtTotalCount.Text) + 1).ToString();
            TxtOpenedMouthCount.Text = (int.Parse(TxtOpenedMouthCount.Text) + openedMouth).ToString();
            TxtClosedMouthCount.Text = (int.Parse(TxtClosedMouthCount.Text) + closedMouth).ToString();

            closedMouth = (closedMouth + 1) % 2;
            openedMouth = (openedMouth + 1) % 2;

        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //ImgDemo.Source = ImageSourceDemo;

            Task.Run(() =>
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
