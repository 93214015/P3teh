using System;
using System.Collections.Generic;
using System.Text;
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
    /// Interaction logic for UCCamera.xaml
    /// </summary>
    public partial class UCCamera : UserControl
    {
        public ImageSource CameraPicture { get { return CameraPictureBox.Source; } set { CameraPictureBox.Source = value; } }

        public UCCamera(string _CameraName)
        {
            InitializeComponent();

            TxtCameraTitle.Text = _CameraName;
        }

        private void BtnCheckCamera_Click(object sender, RoutedEventArgs e)
        {
            ShowCheckResult(true);
        }

        private void ShowCheckResult(bool _Result)
        {
            Storyboard _STBShowResult = (Storyboard)FindResource("STBShowCheckResult");

            switch (_Result)
            {
                case true:
                    Storyboard.SetTarget(_STBShowResult, CameraCheckSign);
                    break;
                case false:
                    Storyboard.SetTarget(_STBShowResult, CameraFailSign);
                    break;
                default:
                    break;
            }

            _STBShowResult.Begin();
        }
    }
}
