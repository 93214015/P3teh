using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF.NETCore.UserControls
{
    /// <summary>
    /// Interaction logic for UCMainPanel.xaml
    /// </summary>
    public partial class UCMainPanel : UserControl
    {


        Storyboard _STBStartMainImageFrameRotation = null;
        Storyboard _STBHideMainImageFrame = null;
        Storyboard _STBShowMainImageFrame = null;
        Storyboard _STBShowInfoCards = null;

        bool IsShowInfoCards = false;

        delegate void UpateTickDelegate();
        UpateTickDelegate _UpdateTick;

        public UCMainPanel()
        {
            InitializeComponent();

            var ellipse = (EllipseGeometry)PathMainImageFrame.Data;
            var strokeLength = 2 * Math.PI * ellipse.RadiusX / PathMainImageFrame.StrokeThickness;

            var numSegments = 8;
            var relativeSegmentLength = 0.75;

            var segmentLength = strokeLength / numSegments * relativeSegmentLength;
            var gapLength = strokeLength / numSegments * (1 - relativeSegmentLength);

            PathMainImageFrame.StrokeDashArray = new DoubleCollection { segmentLength, gapLength };
            PathMainImageFrame.StrokeDashOffset = -gapLength / 2;

            _STBStartMainImageFrameRotation = (Storyboard)FindResource("STBRotateMainImageFrame");
            _STBStartMainImageFrameRotation.RepeatBehavior = RepeatBehavior.Forever;

            _STBHideMainImageFrame = (Storyboard)FindResource("STBHideMainImageFrame");
            _STBShowMainImageFrame = (Storyboard)FindResource("STBShowMainImageFrame");
            _STBShowInfoCards = (Storyboard)FindResource("STBShowInfoCards4");
                       

            Task.Run(() =>
            {
                while (true)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (_UpdateTick != null)
                            _UpdateTick();
                    });
                    Thread.Sleep(2000);
                }
            });
        }

        public bool IsChecked { get; set; } = false;
        public bool IsStarted { get; set; } = false;


        public async Task<bool> StartOperations()
        {
            bool _Result = true;

            if (!IsChecked)
            {
                _Result = await StartChecking();

            }

            if (!IsShowInfoCards)
            { 
                _STBShowInfoCards.Begin();
                IsShowInfoCards = true;
            }

            _STBShowMainImageFrame.Begin();
            _STBStartMainImageFrameRotation.Begin();

            IsStarted = true;

            _UpdateTick += UpdateInfo;

            return _Result;
        }
        public void StopOperations()
        {
            _STBStartMainImageFrameRotation.Stop();
            _STBHideMainImageFrame.Begin();

            IsStarted = false;

            _UpdateTick -= UpdateInfo;
        }


        private void BtnStartChecking_Click(object sender, RoutedEventArgs e)
        {
            StartChecking();
        }

        private int _InfoNum = 0;
        private void UpdateInfo()
        {
            
            TxtTotalCount.Text = _InfoNum.ToString();
            TxtOpenedCount.Text = _InfoNum.ToString();
            TxtClosedCount.Text = _InfoNum.ToString();

            _InfoNum++;
        }

        private async Task<bool> StartChecking()
        {
            _StackPanelCheckingItemList.Children.Clear();

            UCCheckListItem _CheckListItem = new UCCheckListItem("بررسی دوربین ها");
            _CheckListItem.HorizontalAlignment = HorizontalAlignment.Stretch;
            _CheckListItem.Height = 40.0;
            _CheckListItem.Margin = new Thickness(0.0, 5.0, 0.0, 5.0);
            _CheckListItem.Opacity = 0;

            TranslateTransform _Translation = new TranslateTransform();
            TransformGroup _TransformGroup = new TransformGroup();
            _TransformGroup.Children.Add(_Translation);

            _CheckListItem.RenderTransform = _TransformGroup;


            Storyboard _STB = (Storyboard)FindResource("STBShowCheckingListItem");
            Storyboard.SetTarget(_STB, _CheckListItem);

            _StackPanelCheckingItemList.Children.Add(_CheckListItem);
            _STB.Begin();

            _CheckListItem.ShowSign(await CheckCameras());

            UCCheckListItem _CheckListItemModels = new UCCheckListItem("بارگذاری مدل ها");
            _CheckListItemModels.HorizontalAlignment = HorizontalAlignment.Stretch;
            _CheckListItemModels.Height = 40.0;
            _CheckListItemModels.Margin = new Thickness(0.0, 5.0, 0.0, 5.0);
            _CheckListItemModels.Opacity = 0;

            TranslateTransform _Translation2 = new TranslateTransform();
            TransformGroup _TransformGroup2 = new TransformGroup();
            _TransformGroup2.Children.Add(_Translation2);

            _CheckListItemModels.RenderTransform = _TransformGroup2;

            Storyboard.SetTarget(_STB, _CheckListItemModels);

            _StackPanelCheckingItemList.Children.Add(_CheckListItemModels);
            _STB.Begin();

            _CheckListItemModels.ShowSign(await CheckLoadingModels());


            IsChecked = true;

            return true;
        }
        private async Task<bool> CheckCameras()
        {
            await Task.Delay(1000);
            return true;
        }
        private async Task<bool> CheckLoadingModels()
        {
            await Task.Delay(1000);
            return true;
        }


    }
}
