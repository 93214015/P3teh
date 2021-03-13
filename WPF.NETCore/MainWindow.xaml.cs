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
            Storyboard _StbStartApp = (Storyboard)this.FindResource("STBStartApp");
            _StbStartApp.Begin(this);
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
    }
}
