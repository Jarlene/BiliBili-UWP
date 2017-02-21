using BiliBili3.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace BiliBili3.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class WebPage : Page
    {
        public WebPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                webView.Navigate(new Uri((e.Parameter as object[])[0].ToString()));
            }
         
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {

            this.Frame.GoBack();
        }

        private void webview_WebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            string ban = Regex.Match(args.Uri.AbsoluteUri, @"^http://bangumi.bilibili.com/anime/(.*?)$").Groups[1].Value;
            if (ban.Length != 0)
            {
               // args.Handled = true;
                MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(BanInfoPage), ban.Replace("/", ""));
                return;
            }
            string ban2 = Regex.Match(args.Uri.AbsoluteUri, @"^http://www.bilibili.com/bangumi/i/(.*?)$").Groups[1].Value;
            if (ban2.Length != 0)
            {
                //args.Handled = true;
                MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(BanInfoPage), ban2.Replace("/", ""));
               // this.Frame.Navigate(typeof(BanInfoPage), ban2.Replace("/", ""));
                return;
            }
            //bilibili://?av=4284663
            string ban3 = Regex.Match(args.Uri.AbsoluteUri, @"^bilibili://?av=(.*?)$").Groups[1].Value;
            if (ban3.Length != 0)
            {
                //args.Handled = true;
                MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(VideoViewPage), ban3.Replace("/", ""));
                //this.Frame.Navigate(typeof(VideoViewPage), ban3.Replace("/", ""));
                return;
            }

            string live = Regex.Match(args.Uri.AbsoluteUri, @"^bilibili://live/(.*?)$").Groups[1].Value;
            if (live.Length != 0)
            {
                MessageCenter.SendNavigateTo(NavigateMode.Play, typeof(LiveRoomPage), live);

                return;
            }

            string live2 = Regex.Match(args.Uri.AbsoluteUri, @"^http://live.bilibili.com/(.*?)$").Groups[1].Value;
            if (live2.Length != 0)
            {
                MessageCenter.SendNavigateTo(NavigateMode.Play, typeof(LiveRoomPage), live2);

                return;
            }

            string minivideo = Regex.Match(args.Uri.AbsoluteUri+"/", @"vc=(.*?)/").Groups[1].Value;
            if (minivideo.Length != 0)
            {

                //MessageCenter.SendNavigateTo(NavigateMode.Play, typeof(LiveRoomPage), minivideo);
                LoadMiniVideo(minivideo);
                return;
            }


            //text .Text= args.Uri.AbsoluteUri;
            webview_progressBar.Visibility = Visibility.Visible;
            if (Regex.IsMatch(args.Uri.AbsoluteUri, "/video/av(.*)?[/|+](.*)?"))
            {
               
                string a = Regex.Match(args.Uri.AbsoluteUri, "/video/av(.*)?[/|+](.*)?").Groups[1].Value;
                //this.Frame.Navigate(typeof(VideoViewPage), a);
                MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(VideoViewPage), a);
            }


        }

        private async void LoadMiniVideo(string vid)
        {
            try
            {

                webview_progressBar.Visibility = Visibility.Visible;

                string url = string.Format("http://api.vc.bilibili.com/clip/v1/video/detail?access_key={0}&appkey={1}&build=434000&mobi_app=android&need_playurl=1&platform=android&src=master&trace_id=20170204152000022&version=4.34.0.434000&video_id={2}", ApiHelper.access_key, ApiHelper._appKey_Android,vid);
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                string results = await WebClientClass.GetResults(new Uri(url));
                LiveVideoModel m = JsonConvert.DeserializeObject<LiveVideoModel>(results.Replace("default", "_default"));
                if (m.code == 0)
                {
                    cd.DataContext = m.data;
                    await cd.ShowAsync();
                }
                else
                {
                    messShow.Show(m.message, 3000);
                }

            }
            catch (Exception ex)
            {
                if (ex.HResult == -2147012867)
                {
                    messShow.Show("检查你的网络连接！", 3000);
                }
                else
                {
                    messShow.Show("无法读取小视频发生错误\r\n" + ex.Message, 3000);
                }
            }
            finally
            {

                webview_progressBar.Visibility = Visibility.Collapsed;

            }


        }


        private void webview_WebView_FrameDOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            webview_progressBar.Visibility = Visibility.Collapsed;
        }

        private void webview_WebView_DOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            webview_progressBar.Visibility = Visibility.Collapsed;
        }


        private void webview_WebView_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            string ban = Regex.Match(args.Uri.AbsoluteUri, @"^http://bangumi.bilibili.com/anime/(.*?)$").Groups[1].Value;
            if (ban.Length != 0)
            {
                args.Handled = true;
                MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(BanInfoPage), ban.Replace("/", ""));
               // this.Frame.Navigate(typeof(BanInfoPage), ban.Replace("/", ""));
                return;
            }
            string ban2 = Regex.Match(args.Uri.AbsoluteUri, @"^http://www.bilibili.com/bangumi/i/(.*?)$").Groups[1].Value;
            if (ban2.Length != 0)
            {
                args.Handled = true;
                MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(BanInfoPage), ban2.Replace("/", ""));
                //this.Frame.Navigate(typeof(BanInfoPage), ban2.Replace("/", ""));
                return;
            }
            string ban3 = Regex.Match(args.Uri.AbsoluteUri, @"^bilibili://?av=(.*?)$").Groups[1].Value;
            if (ban3.Length != 0)
            {
                args.Handled = true;
                MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(VideoViewPage), ban3.Replace("/", ""));
                //this.Frame.Navigate(typeof(VideoViewPage), ban3.Replace("/", ""));
                return;
            }

            string live = Regex.Match(args.Uri.AbsoluteUri, @"^bilibili://live/(.*?)$").Groups[1].Value;
            if (live.Length != 0)
            {
                args.Handled = true;
                MessageCenter.SendNavigateTo(NavigateMode.Play, typeof(LiveRoomPage), live);

                return;
            }

            string minivideo = Regex.Match(args.Uri.AbsoluteUri + "/", @"vc=(.*?)/").Groups[1].Value;
            if (minivideo.Length != 0)
            {
                args.Handled = true;
                //MessageCenter.SendNavigateTo(NavigateMode.Play, typeof(LiveRoomPage), minivideo);
                LoadMiniVideo(minivideo);
                return;
            }


            //乱写一通的正则
            //正则真的真的真的不会啊- -
            if (Regex.IsMatch(args.Uri.AbsoluteUri, "/video/av(.*)?[/|+](.*)?"))
            {
                args.Handled = true;
            
                string a = Regex.Match(args.Uri.AbsoluteUri, "/video/av(.*)?[/|+](.*)?").Groups[1].Value;
                MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(VideoViewPage), a);
                //this.Frame.Navigate(typeof(VideoViewPage), a);
            }
            else
            {
                if (Regex.IsMatch(args.Uri.AbsoluteUri + "+", "/video/av(.*)[/|+]"))
                {
                    args.Handled = true;
                    string a = Regex.Match(args.Uri.AbsoluteUri + "+", "/video/av(.*)[/|+]").Groups[1].Value;
                    //this.Frame.Navigate(typeof(VideoViewPage), a);
                    MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(VideoViewPage), a);
                }
                else
                {
                    if (Regex.IsMatch(args.Uri.AbsoluteUri, @"live.bilibili.com/(.*?)"))
                    {
                        args.Handled = true;
                        string a = Regex.Match(args.Uri.AbsoluteUri + "a", "live.bilibili.com/(.*?)a").Groups[1].Value;
                        // livePlayVideo(a);
                        MessageCenter.SendNavigateTo(NavigateMode.Play, typeof(LiveRoomPage), a);
                    }
                    else
                    {
                        args.Handled = true;
                        messShow.Show("已禁止跳转：" + args.Uri.AbsoluteUri+"\r\n请点击右上角使用浏览器打开",3000);
                        //text.Text = "已禁止跳转：" + args.Uri.AbsoluteUri;
                    }
                }
            }

        }

        private void menu_copy_Click(object sender, RoutedEventArgs e)
        {
            DataPackage pack = new Windows.ApplicationModel.DataTransfer.DataPackage();
            pack.SetText(webView.Source.AbsoluteUri);
            Clipboard.SetContent(pack); // 保存 DataPackage 对象到剪切板
            Clipboard.Flush();
        }

        private async void menu_open_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(webView.Source);
        }

        private void btn_refresh_Click(object sender, RoutedEventArgs e)
        {
            webView.Refresh();
        }

        private void cd_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

            Windows.ApplicationModel.DataTransfer.DataPackage pack = new Windows.ApplicationModel.DataTransfer.DataPackage();
            pack.SetText((sender.DataContext as LiveVideoModel).item.share_url);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(pack); // 保存 DataPackage 对象到剪切板
            Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
            messShow.Show("已将内容复制到剪切板", 3000);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            cd.Hide();
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(UserInfoPage), ((sender as Button).DataContext as LiveVideoModel).user.uid);
        }

    }
}
