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

namespace WPF.NETCore.UserControls
{
    /// <summary>
    /// Interaction logic for UCCameraPanel.xaml
    /// </summary>
    public partial class UCCameraPanel : UserControl
    {
        public UCCameraPanel()
        {
            InitializeComponent();
        }

        int _CameraNum = 1;

        private void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            UCCamera _UCCamera = new UCCamera("Doorbin 1" + _CameraNum.ToString());
            StackPanelCameraController.Children.Add(_UCCamera);
        }
    }
}
