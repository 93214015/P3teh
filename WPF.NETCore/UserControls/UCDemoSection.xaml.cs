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
using System.Windows.Threading;
using WpfAnimatedGif;
using System.Linq;
using System.IO;
using System.Diagnostics;

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
        Storyboard _STBShowBackgroundProcessing;
        Storyboard _STBHideBackgroundProcessing;
        Storyboard _STBShowDemoPanelButtons;
        Storyboard _STBHideDemoPanelButtons;
        Storyboard _STBShowBackgroundThumbnail;

        DispatcherTimer _TimerBackgroundProcessing = new DispatcherTimer();

        private delegate void _DelegateProcess();
        _DelegateProcess _Processing;

        public UCDemoSection()
        {
            InitializeComponent();

            _BitmapOpenedMouth.BeginInit();
            _BitmapOpenedMouth.UriSource = new Uri("pack://application:,,,/Images/p3teOpen.png");
            _BitmapOpenedMouth.EndInit();

            _BitmapClosedMouth.BeginInit();
            _BitmapClosedMouth.UriSource = new Uri("pack://application:,,,/Images/p3teClose.png");
            _BitmapClosedMouth.EndInit();


            _STBShowResultImage = (Storyboard)this.FindResource("STBShowResultImage");
            _STBHideResultImage = (Storyboard)this.FindResource("STBHideResultImage");
            _STBShowResultImagePopIn = (Storyboard)this.FindResource("STBShowResultPopIn");
            _STBShowBackgroundProcessing = (Storyboard)this.FindResource("STBShowBackgroundProcessing");
            _STBHideBackgroundProcessing = (Storyboard)this.FindResource("STBHideBackgroundProcessing");
            _STBShowBackgroundThumbnail = (Storyboard)this.FindResource("STBShowBackgroundThumbnail");



            _TimerBackgroundProcessing.Tick += _Timer_Tick;
            _TimerBackgroundProcessing.Interval = new TimeSpan(0, 0, 10);

            var _ImageProcessingGIF = new BitmapImage();
            _ImageProcessingGIF.BeginInit();
            _ImageProcessingGIF.UriSource = new Uri("pack://application:,,,/Images/ProcessingGIF.gif");
            _ImageProcessingGIF.EndInit();
            ImageBehavior.SetAnimatedSource(ImgProcessingGIF, _ImageProcessingGIF);

        }

        public void Init(Storyboard STBShowDemoPanelButtons, Storyboard STBHideDemoPanelButtons)
        {
            _STBShowDemoPanelButtons = STBShowDemoPanelButtons;
            _STBHideDemoPanelButtons = STBHideDemoPanelButtons;
        }

        public async void PowerCamera()
        {
            if (mVideoCapture == null)
            {
                await Task.Run(StartCamera);
                _Processing += ShowResult;

                _STBShowDemoPanelButtons.Begin();
            }
            else
            {
                StopCamera();
                _STBHideDemoPanelButtons.Begin();
                _Processing -= ShowResult;
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

        public async void StartCamera()
        {
            if (mVideoCapture == null)
            {
                var task = Task.Run(() => { return new VideoCapture(0, VideoCapture.API.Any); });
                mVideoCapture = await task;
                mVideoCapture.FlipHorizontal = true;
                mVideoCapture.ImageGrabbed += MVideoCapture_ImageGrabbed;

                //_ASEImageClassifier.Train(@"C:\FinalDB\Train", false, ASE.SET_LABEL_METHOD.UseFolderNameAsLabel, ASE.EModelArchitecture.MobileNetV2);
                //_ASEImageClassifier.SaveModel(@"", "model", true);

                _ASEImageClassifier.LoadModel(@"C:\FinalDB", "P3tehFinalModel", false, @"Images\ImagePredictionSampleData", true);

                Mat _Image = new Mat();
                mVideoCapture.Retrieve(_Image);
                ASE.InMemoryImageData[] _SingleInputData = new ASE.InMemoryImageData[1];
                _SingleInputData[0] = new ASE.InMemoryImageData(_Image.GetRawData());
                var resultSingle = _ASEImageClassifier.Classify(_SingleInputData);
            }
            await Task.Run(() => mVideoCapture.Start());
        }

        private void MVideoCapture_ImageGrabbed(object sender, EventArgs e)
        {

            try
            {
                Mat _Image = new Mat();
                mVideoCapture.Retrieve(_Image);
                var _EmguImage = _Image.ToImage<Bgr, Byte>();
                var _Bitmap = _EmguImage.ToBitmap();


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

        private void MVideoCapture_BackgroundProcessing(object sender, EventArgs e)
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
                    ImgBackgroundProcessing.Source = _ImageSoruce;

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
                Width = 30,
                Height = 30,
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

            if (StackPanel_ResultImages.Children.Count > 20)
            {
                StackPanel_ResultImages.Children.RemoveAt(20);
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
                        if (_Processing != null)
                            _Processing();

                    });
                    Thread.Sleep(2000);
                }
            });

            //GIFProcessingWait.Source = new Uri(Environment.CurrentDirectory + @"/Images/ProcessingGIF2Transparent.gif");

        }

        public void StartBackgroundProcessing()
        {
            mVideoCapture.ImageGrabbed -= MVideoCapture_ImageGrabbed;
            mVideoCapture.ImageGrabbed += MVideoCapture_BackgroundProcessing;

            CTBackgroundProcessing.Visibility = Visibility.Visible;
            _STBShowBackgroundProcessing.Begin();

            //GIFProcessingWait.Play();

            _TimerBackgroundProcessing.Start();
        }

        public void StopBackgroundProcessing()
        {
            _STBHideBackgroundProcessing.Completed += _STBHideBackgroundProcessing_Completed;
            _STBHideBackgroundProcessing.Begin();
        }

        private void _Timer_Tick(object sender, EventArgs e)
        {
            _TimerBackgroundProcessing.Stop();

            StopBackgroundProcessing();
        }

        private void _STBHideBackgroundProcessing_Completed(object sender, EventArgs e)
        {
            mVideoCapture.ImageGrabbed -= MVideoCapture_BackgroundProcessing;
            mVideoCapture.ImageGrabbed += MVideoCapture_ImageGrabbed;

            //GIFProcessingWait.Stop();

            CTBackgroundProcessing.Visibility = Visibility.Collapsed;

            ShowBackgroundThumbnail();

        }

        private void ShowBackgroundThumbnail()
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
                ImgBackgroundThumbnail.Source = _ImageSoruce;

            });

            _STBShowBackgroundThumbnail.Begin();
        }

        private ASE.ASEImageClassifier _ASEImageClassifier = new ASE.ASEImageClassifier();

        private void BtnTestPrediction_Click(object sender, RoutedEventArgs e)
        {
            //{
            //    Mat _Image = new Mat();
            //    mVideoCapture.Retrieve(_Image);
            //    ASE.InMemoryImageData[] _SingleInputData = new ASE.InMemoryImageData[1];


            //    _SingleInputData[0] = new ASE.InMemoryImageData(_Image.GetRawData());

            //    var resultSingle = _ASEImageClassifier.Classify(_SingleInputData);
            //}

            //{
            //    Mat _Image = new Mat();
            //    mVideoCapture.Retrieve(_Image);
            //    Mat _ResizedImage = new Mat();
            //    CvInvoke.Resize(_Image, _ResizedImage, new System.Drawing.Size(50, 50));
            //    var _EmguBitmap = _ResizedImage.ToBitmap();
            //    ASE.InMemoryImageData[] _SingleInputData = new ASE.InMemoryImageData[1];

            //    using (var _MemoryStream = new MemoryStream())
            //    {
            //        _EmguBitmap.Save(_MemoryStream, System.Drawing.Imaging.ImageFormat.Png);
            //        _SingleInputData[0] = new ASE.InMemoryImageData(_MemoryStream.ToArray());
            //    }


            //    var resultSingle = _ASEImageClassifier.Classify(_SingleInputData).ToArray();
            //}

            //{
            //    var _Result = _ASEImageClassifier.Classify(@"D:\SourceCodes\C#\TensorFlowProjectSample\WinFormsApp1\test\1", false, ASE.SET_LABEL_METHOD.NoLabel).ToArray();
            //}

            //using (var _MemoryStream = new MemoryStream())
            //{
            //    var _EmguBitmap = _Image.ToBitmap();
            //    _EmguBitmap.Save(_MemoryStream, System.Drawing.Imaging.ImageFormat.Png);
            //}

            ASE.InMemoryImageData[] _InputData = new ASE.InMemoryImageData[40];

            //for (int i = 0; i < 40; i++)
            //{
            //    Mat _Image = new Mat();
            //    mVideoCapture.Retrieve(_Image);
            //    //byte[] _ImageData = new byte[_Image.GetRawData().Length];
            //    //_Image.GetRawData().CopyTo(_ImageData, 0);

            //    _InputData[i] = new ASE.InMemoryImageData(_Image.GetRawData());

            //}

            for (int i = 0; i < 40; i++)
            {
                Mat _Image = new Mat();
                mVideoCapture.Retrieve(_Image);
                //byte[] _ImageData = new byte[_Image.GetRawData().Length];
                //_Image.GetRawData().CopyTo(_ImageData, 0);

                using (var _MemoryStream = new MemoryStream())
                {
                    Mat _ResizedImage = new Mat();
                    CvInvoke.Resize(_Image, _ResizedImage, new System.Drawing.Size(50, 50));
                    Mat _GrayMat = new Mat();
                    CvInvoke.CvtColor(_ResizedImage, _GrayMat, Emgu.CV.CvEnum.ColorConversion.Rgb2Gray);
                    var _EmguBitmap = _GrayMat.ToBitmap();
                    _EmguBitmap.Save(_MemoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    //_EmguBitmap.Save(@"d:\sample.png", System.Drawing.Imaging.ImageFormat.Png);
                    _InputData[i] = new ASE.InMemoryImageData(_MemoryStream.ToArray());
                }

            }



            //foreach (var _ImageData in _InputData)
            //{
            //    ASE.InMemoryImageData[] _SingleInputData = new ASE.InMemoryImageData[1] { _ImageData };

            //    try
            //    {
            //        var resultSingle = _ASEImageClassifier.Classify(_SingleInputData);
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }
            //}

            //{
            //    var resultSingle = _ASEImageClassifier.Classify(_InputData);
            //}


            //Stopwatch _SW = new Stopwatch();

            //_SW.Start();
            //var result = _ASEImageClassifier.ClassifyMultiThreaded(_InputData.ToList());
            //_SW.Stop();

            //MessageBox.Show($"The ML.Net Prediction Time : {_SW.ElapsedMilliseconds}");

            //Debug.WriteLine($"The classification Time was {_SW.ElapsedMilliseconds}");

            //System.Drawing.Bitmap[] _Bitmaps = new System.Drawing.Bitmap[40];

            //for (int i = 0; i < 40; i++)
            //{
            //    Mat _Image = new Mat();
            //    mVideoCapture.Retrieve(_Image);
            //    _Bitmaps[i] = _Image.ToBitmap();
            //}


            ASETensorflow.TFImageClassifier _ASETF = new ASETensorflow.TFImageClassifier(@"D:\SourceCodes\C#\TensorFlowProjectSample\WinFormsApp1\OpModel.pb", false);

            
            long _PredictionTime;

            var _TFResults = _ASETF.Predict(@"D:\SourceCodes\C#\TensorFlowProjectSample\WinFormsApp1\test\0", "png", false, out _PredictionTime, 40 );

            MessageBox.Show($"The Prediction Time : {_PredictionTime}");
            
        }

        //private void GIFProcessingWait_MediaEnded(object sender, RoutedEventArgs e)
        //{
        //    GIFProcessingWait.Position = new TimeSpan(0, 0, 1);
        //    GIFProcessingWait.Play();
        //}

    }
}
