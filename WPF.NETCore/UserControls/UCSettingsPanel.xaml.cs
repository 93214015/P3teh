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
    /// Interaction logic for UCSettingsPanel.xaml
    /// </summary>
    public partial class UCSettingsPanel : UserControl
    {
        public UCSettingsPanel()
        {
            InitializeComponent();
        }

        private void BtnSetting_MouseEnter(object sender, MouseEventArgs e)
        {
            Storyboard _STBMouseEnter = (Storyboard)FindResource("STBBtnUpdate");

            _STBMouseEnter.Begin();
        }

        private void BtnSetting_MouseLeave(object sender, MouseEventArgs e)
        {
            Storyboard _STBMouseLeave = (Storyboard)FindResource("STBBtnUpdateMouseLeave");

            _STBMouseLeave.Begin();
        }
    }
}
