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
    /// Interaction logic for UCCheckListItem.xaml
    /// </summary>
    public partial class UCCheckListItem : UserControl
    {

        public UCCheckListItem(string _CheckName)
        {
            InitializeComponent();

            TxtCheckName.Text = _CheckName;
        }

        public string CheckName { get{ return TxtCheckName.Text; } set{ TxtCheckName.Text = value; } }

        public void ShowSign(bool _Result)
        {
            Storyboard _Storybaord = (Storyboard)FindResource("STBShowResult");

            switch (_Result)
            {
                case true:
                    Storyboard.SetTarget(_Storybaord, _SignChecked);
                    break;
                case false:
                    Storyboard.SetTarget(_Storybaord, _SignFailed);
                    break;
                default:
                    break;
            }

            _Storybaord.Begin();
        }
    }
}
