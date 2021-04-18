using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace WPF.NETCore
{

    enum EPanels
    {
        Demo,
        Home,
        Settings,
        CCTV
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Storyboard _STBShowDemoPanelButtons;
        Storyboard _STBHideDemoPanelButtons;

        private EPanels mCurrentPanels = EPanels.Home;

        private Border _CurrentUserControlContainer = null;

        private Storyboard _STBPanelFadeOut = null;

        Storyboard _STBPanelFadeIn = null;

        public MainWindow()
        {
            InitializeComponent();


            _STBShowDemoPanelButtons = (Storyboard)FindResource("STBShowDemoPanelButtons");
            _STBHideDemoPanelButtons = (Storyboard)FindResource("STBHideDemoPanelButtons");

            DemoPanel.Init(_STBShowDemoPanelButtons, _STBHideDemoPanelButtons);

            _CurrentUserControlContainer = PanelContainer_Main;

            _STBPanelFadeOut = (Storyboard)this.FindResource("STBPanelFadeOut");
            _STBPanelFadeOut.Completed += STBPanelsFadeOut_Completed;

            _STBPanelFadeIn = (Storyboard)this.FindResource("STBPanelFadeIn");

            gRPC.RPCClient.Init();
        }

        private void ChangeUserControl(Border _NextUserControl)
        {

            Storyboard.SetTarget(_STBPanelFadeOut, _CurrentUserControlContainer);
            _STBPanelFadeOut.Begin();

            Storyboard.SetTarget(_STBPanelFadeIn, _NextUserControl);

            _NextUserControl.Visibility = Visibility.Visible;
            _STBPanelFadeIn.Begin();

            _CurrentUserControlContainer = _NextUserControl;
        }

        private void STBPanelsFadeOut_Completed(object sender, EventArgs e)
        {
            var _b = (Border)Storyboard.GetTarget(_STBPanelFadeOut);
            _b.Visibility = Visibility.Collapsed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Threading.DispatcherTimer _Timer = new System.Windows.Threading.DispatcherTimer();
            _Timer.Tick += _Timer_Tick;
            _Timer.Interval = TimeSpan.FromSeconds(1);
            _Timer.Start();

            Storyboard _StbStartApp = (Storyboard)this.FindResource("STBStartApp");
            _StbStartApp.Begin(this);

            //var uriSource = new Uri(@"/WPF.NETCore;component/Images/DemoImage.png", UriKind.Relative);
            //ImgDemo.ImageSourceDemo = new BitmapImage(uriSource);
        }

        private void _Timer_Tick(object sender, EventArgs e)
        {
            var _Time = DateTime.Now;
            TxtBlock_Time.Text = $"{_Time.Hour.ToString("D2")} : {_Time.Minute.ToString("D2")} : {_Time.Second.ToString("D2")}";
        }

        private void BtnMenuLeft_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Storyboard STBLeftMenu = (Storyboard)this.FindResource("STBLeftMenu");
            STBLeftMenu.Begin();
        }

        private void BtnLeftMenuClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Storyboard STBLeftMenuReverse = (Storyboard)this.FindResource("STBLeftMenuReverse");
            STBLeftMenuReverse.Begin();
        }

        private void BtnDemo_Click(object sender, RoutedEventArgs e)
        {
            if (mCurrentPanels == EPanels.Demo)
                return;


            ChangeUserControl(PanelContainer_Demo);

            mCurrentPanels = EPanels.Demo;
        }

        private void BtnCCTV_Click(object sender, RoutedEventArgs e)
        {
            if (mCurrentPanels == EPanels.CCTV)
                return;

            ChangeUserControl(PanelContainer_CCTV);

            mCurrentPanels = EPanels.CCTV;
        }


        private void BtnPower_Click(object sender, RoutedEventArgs e)
        {
            switch (mCurrentPanels)
            {
                case EPanels.Demo:
                    DemoPanel.PowerCamera();
                    break;

                case EPanels.Home:
                    if (!UCMainPanel.IsStarted)
                    {
                        UCMainPanel.StartOperations();
                    }
                    else
                    {
                        UCMainPanel.StopOperations();
                    }
                    break;
            }
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            if (mCurrentPanels == EPanels.Home)
                return;

            if (mCurrentPanels == EPanels.Demo)
            {
                DemoPanel.PowerCamera();
            }

            ChangeUserControl(PanelContainer_Main);

            mCurrentPanels = EPanels.Home;
        }

        private void BtnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnMinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void BtnStartBackgroundProcessing_Click(object sender, RoutedEventArgs e)
        {
            DemoPanel.StartBackgroundProcessing();
        }

        private void BtnSetting_Click(object sender, RoutedEventArgs e)
        {
            if (mCurrentPanels == EPanels.Settings)
                return;

            ChangeUserControl(PanelContainer_Settings);

            //Storyboard STBImagePopOut = (Storyboard)this.FindResource("STBImagePopOut");
            //STBImagePopOut.Begin();

            //Storyboard STBPanelsFadeIn = (Storyboard)this.FindResource("STBPanelFadeIn");

            //foreach (var anim in STBPanelsFadeIn.Children)
            //{
            //    Storyboard.SetTarget(anim, PanelContainer_Settings);
            //}

            //PanelContainer_Settings.Visibility = Visibility.Visible;
            //STBPanelsFadeIn.Begin();

            mCurrentPanels = EPanels.Settings;
        }
    }
}
