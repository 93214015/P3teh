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
        CCTV,
        Settings
    }


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
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

            mCurrentPanels = EPanels.Demo;

            Storyboard STBImagePopOut = (Storyboard)this.FindResource("STBImagePopOut");
            STBImagePopOut.Begin();


            Storyboard STBPanelsFadeIn = (Storyboard)this.FindResource("STBPanelFadeIn");
            border.Visibility = Visibility.Visible;
            STBPanelsFadeIn.Begin();

        }

        private void BtnCCTV_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private EPanels mCurrentPanels = EPanels.CCTV;

        private void BtnPower_Click(object sender, RoutedEventArgs e)
        {
            if (mCurrentPanels == EPanels.Demo)
                DemoPanel.PowerCamera();
        }

        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {
            if (mCurrentPanels == EPanels.CCTV)
                return;

            if (mCurrentPanels == EPanels.Demo)
            {
                DemoPanel.StopCamera();
            }

            mCurrentPanels = EPanels.CCTV;

            Storyboard STBImagePopIn = (Storyboard)this.FindResource("STBImagePopIn");
            STBImagePopIn.Begin();


            Storyboard STBPanelsFadeOut = (Storyboard)this.FindResource("STBPanelFadeOut");
            STBPanelsFadeOut.Begin();
        }

        private void BtnCloseWindow_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnMinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}
