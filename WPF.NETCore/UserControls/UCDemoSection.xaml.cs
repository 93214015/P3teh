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
    /// Interaction logic for UCDemoSection.xaml
    /// </summary>
    public partial class UCDemoSection : UserControl
    {
        public UCDemoSection()
        {
            InitializeComponent();

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
