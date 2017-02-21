using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.System.Display;
using Windows.UI.ViewManagement;
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
    public sealed partial class PlayerPage : Page
    {
        public PlayerPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        private DisplayRequest dispRequest = null;//保持屏幕常亮
        protected  async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await Task.Delay(200);
            if (e.NavigationMode== NavigationMode.New)
            {
                object[] obj = e.Parameter as object[];
                var ls = obj[0] as List<PlayerModel>;
                var index = (int)obj[1];
                var info = ls[index];
                webplay.Visibility = Visibility.Collapsed;
                pr_Load.Visibility = Visibility.Collapsed;
                switch (info.Mode)
                {
                    case Controls.PlayMode.Bangumi:
                        
                        break;
                    case Controls.PlayMode.Movie:
                        webplay.Visibility = Visibility.Visible;
                        LoadVipMovie(info);
                        break;
                    case Controls.PlayMode.VipBangumi:
                        webplay.Visibility = Visibility.Visible;
                        LoadVipBangumi(info.episode_id);
                        break;
                    case Controls.PlayMode.Video:
                        player.LoadPlayer(ls, index);
                        break;
                    case Controls.PlayMode.QQ:
                        break;
                    case Controls.PlayMode.Sohu:
                        player.LoadPlayer(ls, index);
                        break;
                    case Controls.PlayMode.Local:
                        player.LoadPlayer(ls, index);
                        break;
                    case Controls.PlayMode.FormLocal:
                        player.LoadPlayer(ls, index);
                        break;
                    default:
                        break;
                }



               
              
            }
        }

        private async void LoadVipBangumi(string episode_id)
        {
            try
            {
                DisplayInformation.AutoRotationPreferences = (DisplayOrientations)5;
                if (dispRequest == null)
                {
                    // 用户观看视频，需要保持屏幕的点亮状态
                    dispRequest = new DisplayRequest();
                    dispRequest.RequestActive(); // 激活显示请求
                }
                web.NavigateToString("");

                pr_Load.Visibility = Visibility.Visible;
                string results = await WebClientClass.GetResults(new Uri("http://bangumi.bilibili.com/web_api/episode/" + episode_id + ".json"));
                JObject jobj = JObject.Parse(results);
                if ((int)jobj["code"] == 0)
                {
                    string playurl = "http://www.bilibili.com/html/html5player.html?cid=" + jobj["result"]["currentEpisode"]["danmaku"].ToString() + "&aid=" + jobj["result"]["currentEpisode"]["avId"].ToString() + "&lastplaytime=0&player_type=1&urlparam=module%3Dbangumi&seasonId=" + jobj["result"]["currentEpisode"]["seasonId"].ToString() + "&episodeId=" + jobj["result"]["currentEpisode"]["episodeId"].ToString() + "&p=1&crossDomain=1&as_wide=1";
                    web.Navigate(new Uri(playurl));
                }
                else
                {
                    messShow.Show(jobj["message"].ToString(), 3000);
                }
            }
            catch (Exception)
            {

                messShow.Show("读取错误", 3000);
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }
        private void LoadVipMovie(PlayerModel info)
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                DisplayInformation.AutoRotationPreferences = (DisplayOrientations)5;
                if (dispRequest == null)
                {
                    // 用户观看视频，需要保持屏幕的点亮状态
                    dispRequest = new DisplayRequest();
                    dispRequest.RequestActive(); // 激活显示请求
                }
                web.NavigateToString("");

                string playurl = "http://www.bilibili.com/html/html5player.html?cid="+info.Mid+"&aid="+ info.Aid + "&as_wide=1&player_type=2&urlparam=module%3Dmovie&p=1&crossDomain=1&as_wide=1";
                web.Navigate(new Uri(playurl));
            }
            catch (Exception)
            {

                messShow.Show("读取错误", 3000);
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }

        private void player_BackEvent()
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            player.ClosePLayer();
            if (dispRequest != null)
            {
                dispRequest = null;
            }
            ApplicationView.GetForCurrentView().ExitFullScreenMode();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
            web.NavigateToString("");

        }

        private void web_ContainsFullScreenElementChanged(WebView sender, object args)
        {
            var applicationView = ApplicationView.GetForCurrentView();

            if (sender.ContainsFullScreenElement)
            {
                applicationView.TryEnterFullScreenMode();
            }
            else if (applicationView.IsFullScreenMode)
            {
                applicationView.ExitFullScreenMode();
            }
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            player_BackEvent();
        }
        //private void player_FullEvent(bool full)
        //{
        //    if (full)
        //    {
        //        DisplayInformation.AutoRotationPreferences = (DisplayOrientations)5;

        //    }
        //    else
        //    {
        //        DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
        //    }
        //}

        //private void player_MaxWIndowsEvent(bool full)
        //{

        //}

        //private void player_PlayerEvent(int index)
        //{

        //}
    }
}
