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

using System.IO;
using System.Net;

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

            _STBShowNetworPanel = (Storyboard)FindResource("STBShowNetworkPanel");
            _STBHideNetworPanel = (Storyboard)FindResource("STBHideNetworkPanel");
            _STBHideNetworPanel.Completed += _STBHideNetworPanel_Completed;

            TxtAppVersion.Text = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
        }

        private void _STBHideNetworPanel_Completed(object sender, EventArgs e)
        {
            PanelNetwork.Visibility = Visibility.Collapsed;
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

        private void BtnShowNetworks_Click(object sender, RoutedEventArgs e)
        {
            PanelNetwork.Visibility = Visibility.Visible;

            _STBShowNetworPanel.Begin();
        }

        private void BtnCloseNetworkPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _STBHideNetworPanel.Begin();
        }

        private Storyboard _STBHideNetworPanel;
        private Storyboard _STBShowNetworPanel;

        class DowloadFileSrcDest
        {
            public Uri Url;
            public string Path;
        }

        private async void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var _ServerFiles = await gRPC.RPCClient.GetUpdateFiles();

                var _TempDir = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Temp");

                List<DowloadFileSrcDest> _DownloadList = new List<DowloadFileSrcDest>();

                Uri _BaseUri = new Uri("http://87.236.212.221/P3teProject/");

                foreach (var _File in _ServerFiles.FileList)
                {
                    FileInfo _FileInfo = new FileInfo(_File.FilePath);

                    if (!_FileInfo.Exists || _File.WriteTime.ToDateTime() > _FileInfo.LastWriteTimeUtc)
                    {
                        _DownloadList.Add(new DowloadFileSrcDest { Url = new Uri(_BaseUri, _File.FilePath), Path = System.IO.Path.Combine(_TempDir, _File.FilePath) });
                    }
                }

                if (_DownloadList.Count > 0)
                {

                    if (MessageBox.Show("مایل به بروزرسانی هستید؟", "بروزرسانی", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {

                        if (Directory.Exists(_TempDir))
                        {
                            Directory.Delete(_TempDir, true);
                        }
                        Directory.CreateDirectory(_TempDir);

                        WebClient _WebClient = new WebClient();

                        foreach (var _Download in _DownloadList)
                        {
                            FileInfo _FileInfo = new FileInfo(_Download.Path);
                            if (!_FileInfo.Directory.Exists)
                            {
                                Directory.CreateDirectory(_FileInfo.DirectoryName);
                            }
                            await _WebClient.DownloadFileTaskAsync(_Download.Url, _Download.Path);
                        }

                        var _UpdaterPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Updater.exe");

                        bool _bSucced = true;
                        try
                        {
                            var _Process = System.Diagnostics.Process.Start(_UpdaterPath, System.AppDomain.CurrentDomain.FriendlyName);

                            if (_Process == null)
                            {
                                _bSucced = false;
                            }
                        }
                        catch
                        {
                            _bSucced = false;
                        }

                        if (_bSucced)
                        {
                            Application.Current.Shutdown();
                        }

                    }


                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
