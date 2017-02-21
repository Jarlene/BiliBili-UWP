using BiliBili3.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using BiliBili3.Pages;
using Windows.UI;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace BiliBili3.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (SettingHelper.Get_RefreshButton()&& SettingHelper.IsPc())
            {
                b_btn_Refresh.Visibility = Visibility.Visible;
            }
            else
            {
                b_btn_Refresh.Visibility = Visibility.Collapsed;

            }
            if (e.NavigationMode == NavigationMode.New && home_GridView_Hot.ItemsSource == null)
            {
                await Task.Delay(200);
                if (SettingHelper.IsPc())
                {
                   
                    if ((await SettingHelper.Get_HomeInfo()).Length != 0)
                    {
                        SetHome(true);
                        await Task.Delay(3000);
                        SetHome(false);
                    }
                    else
                    {
                        //await Task.Delay(200);
                        SetHome(false);
                    }

                }
                else
                {
                   // b_btn_Refresh.Visibility = Visibility.Collapsed;
                    //await Task.Delay(200);
                    SetHome(false);
                }

            }
            if (time == null)
            {
                time = new DispatcherTimer();
                time.Interval = new TimeSpan(0, 0, 3);
                time.Tick += Time_Tick;
                time.Start();
            }
        }



        private void Time_Tick(object sender, object e)
        {
            if (home_flipView.SelectedIndex == -1)
            {
                return;
            }
            int i = home_flipView.SelectedIndex;
            i++;
            if (i >= home_flipView.Items.Count)
            {
                i = 0;
            }
            home_flipView.SelectedIndex = i;
        }
        DispatcherTimer time;

        private async void SetHome(bool local)
        {
            try
            {
                disaid = "";
                pr_Load.Visibility = Visibility.Visible;

                string results = "";

                if (local)
                {
                    results = await SettingHelper.Get_HomeInfo();
                }
                else
                {
                    string url = string.Format("http://app.bilibili.com/x/v2/show?access_key={0}&appkey={1}&build=429001&mobi_app=android&platform=android&ts={2}000", ApiHelper.access_key, ApiHelper._appKey_Android, ApiHelper.GetTimeSpan);
                    url += "&sign=" + ApiHelper.GetSign_Android(url);
                    results = await WebClientClass.GetResults(new Uri(url));
                    results = results.Replace("goto", "_goto");
                }
                HomeModel m = JsonConvert.DeserializeObject<HomeModel>(results);
                if (m.code == 0)
                {
                    if (!local && SettingHelper.IsPc())
                    {

                        SettingHelper.Set_HomeInfo(results);
                    }
                    //var ls = m.data.Where(x => x.type == "topic").ToList();

                    //topic_2.DataContext = ls[1].body[0];
                    //topic_3.DataContext = ls[2].body[0];
                    int i = 0;
                    foreach (var item in m.data)
                    {
                        switch (item.title)
                        {
                            case "热门焦点":
                                home_GridView_Hot.ItemsSource = item.body;
                                home_flipView.ItemsSource = item.banner.top;

                                break;
                            case "正在直播":
                                home_GridView_Live.ItemsSource = item.body;
                                break;
                            case "番剧推荐":
                                home_GridView_Bangumi.ItemsSource = item.body;
                                break;
                            case "动画区":
                                home_GridView_DH.ItemsSource = item.body;
                                break;
                            case "音乐区":
                                home_GridView_YY.ItemsSource = item.body;
                                break;
                            case "舞蹈区":
                                home_GridView_WD.ItemsSource = item.body;
                                break;
                            case "游戏区":
                                home_GridView_YX.ItemsSource = item.body;
                                break;
                            case "鬼畜区":
                                home_GridView_GC.ItemsSource = item.body;
                                break;
                            case "科技区":
                                home_GridView_KJ.ItemsSource = item.body;
                                break;
                            case "活动中心":
                                home_GridView_Activity.ItemsSource = item.body;
                                break;
                            case "生活区":
                                home_GridView_SH.ItemsSource = item.body;
                                break;
                            case "娱乐区":
                                home_GridView_YL.ItemsSource = item.body;
                                break;
                            case "时尚区":
                                home_GridView_SS.ItemsSource = item.body;
                                break;
                            case "电视剧区":
                                home_GridView_DSJ.ItemsSource = item.body;
                                break;
                            case "电影区":
                                home_GridView_DY.ItemsSource = item.body;
                                break;
                            case "广告区":
                                home_GridView_GG.ItemsSource = item.body;
                                break;
                            default:
                                {
                                    if (item.type == "topic")
                                    {
                                        switch (i)
                                        {
                                            case 0:
                                                topic_1.DataContext = item.body[0];
                                                break;
                                            case 1:
                                                topic_2.DataContext = item.body[0];
                                                break;
                                            case 2:
                                                topic_3.DataContext = item.body[0];
                                                break;
                                            default:
                                                break;
                                        }
                                        i++;
                                    }

                                }
                                break;
                        }
                    }

                    //home_GridView_Hot.ItemsSource = m.data.First(x => x.title == "热门焦点").body;
                    // home_flipView.ItemsSource = m.data.First(x => x.title == "热门焦点").banner.top;
                    //home_GridView_Live.ItemsSource = m.data.First(x => x.title == "正在直播").body;
                    //home_GridView_Bangumi.ItemsSource = m.data.First(x => x.title == "番剧推荐").body;
                    //home_GridView_Activity.ItemsSource = m.data.First(x => x.title == "活动中心").body;
                    //home_GridView_DH.ItemsSource = m.data.First(x => x.title == "动画区").body;
                    //home_GridView_DSJ.ItemsSource = m.data.First(x => x.title == "电视剧区").body;
                    //home_GridView_DY.ItemsSource = m.data.First(x => x.title == "电影区").body;
                    //home_GridView_GC.ItemsSource = m.data.First(x => x.title == "鬼畜区").body;
                    //home_GridView_GG.ItemsSource = m.data.First(x => x.title == "广告区").body;
                    //home_GridView_KJ.ItemsSource = m.data.First(x => x.title == "科技区").body;
                    //home_GridView_SH.ItemsSource = m.data.First(x => x.title == "生活区").body;
                    //home_GridView_SS.ItemsSource = m.data.First(x => x.title == "时尚区").body;
                    //home_GridView_WD.ItemsSource = m.data.First(x => x.title == "舞蹈区").body;
                    //home_GridView_YL.ItemsSource = m.data.First(x => x.title == "娱乐区").body;
                    //home_GridView_YX.ItemsSource = m.data.First(x => x.title == "游戏区").body;
                    //home_GridView_YY.ItemsSource = m.data.First(x => x.title == "音乐区").body;

                }
                else
                {
                    await new MessageDialog(m.message).ShowAsync();
                }
            }
            catch (Exception ex)
            {
                if (ex.HResult == -2147012867 || ex.HResult == -2147012889)
                {
                    messShow.Show("无法连接服务器，请检查你的网络连接", 3000);
                }
                else
                {

                    messShow.Show("更新数据失败了", 3000);
                }
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            ViewBox2_num.Width = ActualWidth / 2 - 20;
        }

        private void btn_openTopic_Click(object sender, RoutedEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(WebPage), ((sender as HyperlinkButton).DataContext as bodyModel).uri.Replace("\t", ""));
        }

        private void home_GridView_Hot_ItemClick(object sender, ItemClickEventArgs e)
        {
            var info = e.ClickedItem as bodyModel;
            if (info._goto == "av")
            {
                MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(VideoViewPage), info.param);
            }
            if (info._goto == "web")
            {
                MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(WebPage), info.param);
            }
            if (info._goto == "bangumi")
            {
                MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(BanInfoPage), info.param);
            }
            if (info._goto == "live")
            {
                MessageCenter.SendNavigateTo(NavigateMode.Play, typeof(LiveRoomPage), info.param);
            }
        }

        private void btn_Banner_Click(object sender, RoutedEventArgs e)
        {

            string ban = Regex.Match(((sender as HyperlinkButton).DataContext as topModel).uri, @"^http://bangumi.bilibili.com/anime/(.*?)$").Groups[1].Value;
            if (ban.Length != 0)
            {
                MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(BanInfoPage), ban);
                return;
            }
            //
            string aid = Regex.Match(((sender as HyperlinkButton).DataContext as topModel).uri, @"^http://www.bilibili.com/video/av(.*?)/$").Groups[1].Value;
            if (aid.Length != 0)
            {
                MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(VideoViewPage), aid);
                return;
            }

            string aid2 = Regex.Match(((sender as HyperlinkButton).DataContext as topModel).uri, @"^bilibili://video/(.*?)$").Groups[1].Value;
            if (aid2.Length != 0)
            {
                MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(VideoViewPage), aid2);
                return;
            }

            string game = Regex.Match(((sender as HyperlinkButton).DataContext as topModel).uri, @"^bilibili://game/(.*?)$").Groups[1].Value;
            if (game.Length != 0)
            {

                messShow.Show("不支持游戏链接跳转", 3000);
                return;
            }

            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(WebPage), ((sender as HyperlinkButton).DataContext as topModel).uri);
        }
        double d = 0;
        private void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

            if (sv.VerticalOffset > d)
            {
                MessageCenter.ShowOrHideBar(false);
            }
            else
            {
                MessageCenter.ShowOrHideBar(true);
            }
            d = sv.VerticalOffset;
        }

        private void btn_go_Timeline_Click(object sender, RoutedEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(TimelinePage));
        }

        private void btn_go_Tag_Click(object sender, RoutedEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(BanTagPage));
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(ActivityPage));
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(TopicPage));
        }

        private void HyperlinkButton_Click_2(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BangumiPage));
        }


        string disaid = "";


        private async void HyperlinkButton_Click_3(object sender, RoutedEventArgs e)
        {
            int rid = Convert.ToInt32((sender as HyperlinkButton).Tag.ToString());
            var data = await RefreshPartHome(rid);
            if (data != null)
            {
                switch (rid)
                {
                    case 1:
                        home_GridView_DH.ItemsSource = data;
                        break;
                    case 3:
                        home_GridView_YY.ItemsSource = data;
                        break;
                    case 4:
                        home_GridView_YX.ItemsSource = data;
                        break;
                    case 5:
                        home_GridView_YL.ItemsSource = data;
                        break;
                    case 11:
                        home_GridView_DSJ.ItemsSource = data;
                        break;
                    case 23:
                        home_GridView_DY.ItemsSource = data;
                        break;
                    case 36:
                        home_GridView_KJ.ItemsSource = data;
                        break;
                    case 119:
                        home_GridView_GC.ItemsSource = data;
                        break;
                    case 129:
                        home_GridView_WD.ItemsSource = data;
                        break;
                    case 155:
                        home_GridView_SS.ItemsSource = data;
                        break;
                    case 160:
                        home_GridView_SH.ItemsSource = data;
                        break;
                    case 165:
                        home_GridView_GG.ItemsSource = data;
                        break;
                    case 999:
                        home_GridView_Live.ItemsSource = data;
                        break;
                    default:
                        home_GridView_Hot.ItemsSource = data;

                        break;
                }
            }
            //home_GridView_DH.ItemsSource=
        }

        private async Task<List<bodyModel>> RefreshPartHome(int rid)
        {
            try
            {
                disaid = "";
                string url = "";
                switch (rid)
                {
                    case 0:
                        url = string.Format("http://app.bilibili.com/x/v2/show/change?access_key={0}&appkey={1}&build=434000&mobi_app=android&platform=android&rand=0&ts={2}000", ApiHelper.access_key, ApiHelper._appKey_Android, ApiHelper.GetTimeSpan);
                        break;
                    case 999:
                        url = string.Format("http://app.bilibili.com/x/v2/show/change/live?access_key={0}&appkey={1}&build=434000&mobi_app=android&platform=android&rand=0&ts={2}000", ApiHelper.access_key, ApiHelper._appKey_Android, ApiHelper.GetTimeSpan);
                        break;
                    default:
                        url = string.Format("http://app.bilibili.com/x/v2/show/change/region?access_key={0}&appkey={1}&build=434000&mobi_app=android&platform=android&rand=1&rid={3}&ts={2}000", ApiHelper.access_key, ApiHelper._appKey_Android, ApiHelper.GetTimeSpan, rid);
                        break;
                }
                
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                string results = await WebClientClass.GetResults(new Uri(url));
                results = results.Replace("goto", "_goto");
                HomeRefreshModel m = JsonConvert.DeserializeObject<HomeRefreshModel>(results);
                if (m.code == 0)
                {
                    return m.data;
                }
                else
                {
                    messShow.Show("刷新失败", 3000);
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void Btn_Hot_More_Click(object sender, RoutedEventArgs e)
        {
            int rid = Convert.ToInt32((sender as HyperlinkButton).Tag.ToString());

            switch (rid)
            {
                case 1:
                    MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(PartsPage), Parts.douga);
                    break;
                case 3:
                    MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(PartsPage), Parts.music);
                    break;
                case 4:
                    MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(PartsPage), Parts.game);
                    break;
                case 5:
                    MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(PartsPage), Parts.ent);
                    break;
                case 11:
                    MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(PartsPage), Parts.tv);
                    break;
                case 23:
                    MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(PartsPage), Parts.movie);
                    break;
                case 36:
                    MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(PartsPage), Parts.technology);
                    break;
                case 119:
                    MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(PartsPage), Parts.kichiku);
                    break;
                case 129:
                    MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(PartsPage), Parts.dance);
                    break;
                case 155:
                    MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(PartsPage), Parts.fashion);
                    break;
                case 160:
                    MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(PartsPage), Parts.life);
                    break;
                case 165:
                    MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(PartsPage), Parts.ad);
                    break;
                case 999:
                    this.Frame.Navigate(typeof(LivePage));
                    break;
                default:
                    MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(RankPage));

                    break;
            }
        }

        private void Border_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            // menu_Zan.Text = ((sender as Grid).DataContext as NewCommentModel).Action;
            // rpid = ((sender as Grid).DataContext as NewCommentModel).rpid.ToString();
            //  userId = ((sender as Grid).DataContext as NewCommentModel).mid;
            disaid = ((sender as Border).DataContext as bodyModel).param;
            menuFlyout.ShowAt(sender as Border, e.GetPosition(sender as Border));
        }

        private void Border_Holding(object sender, HoldingRoutedEventArgs e)
        {
            disaid = ((sender as Border).DataContext as bodyModel).param;
            menuFlyout.ShowAt(sender as Border, e.GetPosition(sender as Border));
        }

        private async void btn_dontlike_Click(object sender, RoutedEventArgs e)
        {
            if (!ApiHelper.IsLogin())
            {
                messShow.Show("你可能需要先登录- -!", 3000);
                return;
            }
            try
            {
               
                string url = string.Format("http://app.bilibili.com/x/v2/show/change/dislike?access_key={0}&appkey={1}&build=434000&goto=av&id={3}&mobi_app=android&platform=android&ts={2}000", ApiHelper.access_key, ApiHelper._appKey_Android, ApiHelper.GetTimeSpan,disaid);
     
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                string results = await WebClientClass.GetResults(new Uri(url));
                JObject obj = JObject.Parse(results);
                if ((int)obj["code"] != 0)
                {
                    messShow.Show("操作失败\r\n"+ obj["message"].ToString(), 3000);
                }
                
            }
            catch (Exception)
            {
                messShow.Show("操作失败", 3000);
            }
            finally
            {
                var data = await RefreshPartHome(0);
                home_GridView_Hot.ItemsSource = data;
            }
        }

        private void b_btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            //await Task.Delay
            SetHome(false);
        }

        private void PullToRefreshBox_RefreshInvoked(DependencyObject sender, object args)
        {
            SetHome(false);
        }
    }

}
