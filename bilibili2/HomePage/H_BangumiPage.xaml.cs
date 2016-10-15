using bilibili2.Class;
using bilibili2.Pages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace bilibili2.HomePage
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class H_BangumiPage : Page
    {
        public delegate void NavigatedToHandel(Type t,object par);
        public event NavigatedToHandel NavigatedTo;

        public H_BangumiPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            pr_Laod.Visibility = Visibility.Visible;
            Task.Delay(200);
            SetWeekInfo();
            if (e.NavigationMode== NavigationMode.New)
            {
                GetBanBanner();
                GetBanUpdate();
                GetBanTJ();
            }
            //pr_Laod.Visibility = Visibility.Collapsed;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            
            int w = Convert.ToInt32(this.sp_Bangumi.OpenPaneLength / 90);

           
            if (this.ActualWidth<=500)
            {
                sp_Bangumi.OpenPaneLength = this.ActualWidth;
                bor_Tag_Width.Width = this.sp_Bangumi.OpenPaneLength / w - 12;
            }
            else
            {
                sp_Bangumi.OpenPaneLength =500;
                bor_Tag_Width.Width = this.sp_Bangumi.OpenPaneLength / w-12;
            }

            if (this.ActualWidth < 640)
            {
                //double i = (double)test.ActualWidth;
                test.Width = double.NaN;
            }
            else
            {
                int i = Convert.ToInt32(this.ActualWidth / 500);
                test.Width = this.ActualWidth / i - 12;
            }
            if (this.ActualWidth <= 640)
            {
                fvLeft_Ban.Visibility = Visibility.Collapsed;
                fvRight_Ban.Visibility = Visibility.Collapsed;
                grid_c_left_Ban.Width = new GridLength(0, GridUnitType.Auto);
                grid_c_right_Ban.Width = new GridLength(0, GridUnitType.Auto);
                grid_c_center_Ban.Width = new GridLength(1, GridUnitType.Star);
                
            }
            else
            {
                fvLeft_Ban.Visibility = Visibility.Visible;
                fvRight_Ban.Visibility = Visibility.Visible;
                grid_c_left_Ban.Width = new GridLength(1, GridUnitType.Star);
                grid_c_right_Ban.Width = new GridLength(1, GridUnitType.Star);
                grid_c_center_Ban.Width = new GridLength(0, GridUnitType.Auto);
            }

        }

        private void list_0_ItemClick(object sender, ItemClickEventArgs e)
        {
            sp_Bangumi.IsPaneOpen = false;
            NavigatedTo(typeof(BanInfoPage), (e.ClickedItem as BangumiTimeLineModel).season_id);

        }

        private void home_flipView_Ban_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (home_flipView_Ban.Items.Count == 0 || fvLeft_Ban.Items.Count == 0 || fvRight_Ban.Items.Count == 0)
            {
                return;
            }
            if (fvLeft_Ban.Visibility == Visibility.Collapsed || fvRight_Ban.Visibility == Visibility.Collapsed)
            {
                return;
            }
            if (this.home_flipView_Ban.SelectedIndex == 0)
            {
                this.fvLeft_Ban.SelectedIndex = this.fvLeft_Ban.Items.Count - 1;
                this.fvRight_Ban.SelectedIndex = 1;
            }
            else if (this.home_flipView_Ban.SelectedIndex == 1)
            {
                this.fvLeft_Ban.SelectedIndex = 0;
                this.fvRight_Ban.SelectedIndex = this.fvRight_Ban.Items.Count - 1;
            }
            else if (this.home_flipView_Ban.SelectedIndex == this.home_flipView_Ban.Items.Count - 1)
            {
                this.fvLeft_Ban.SelectedIndex = this.fvLeft_Ban.Items.Count - 2;
                this.fvRight_Ban.SelectedIndex = 0;
            }
            else if ((this.home_flipView_Ban.SelectedIndex < (this.home_flipView_Ban.Items.Count - 1)) && this.home_flipView_Ban.SelectedIndex > -1)
            {
                this.fvLeft_Ban.SelectedIndex = this.home_flipView_Ban.SelectedIndex - 1;
                this.fvRight_Ban.SelectedIndex = this.home_flipView_Ban.SelectedIndex + 1;
            }
            else
            {
                return;
            }
        }

        private void btn_Banner_Ban_Click(object sender, RoutedEventArgs e)
        {

            string tag = Regex.Match((home_flipView_Ban.SelectedItem as BanBannerModel).link, @"^http://bangumi.bilibili.com/anime/category/(.*?)$").Groups[1].Value;
            if (tag.Length != 0)
            {
                NavigatedTo(typeof(BanByTagPage), new string[] { tag, (home_flipView_Ban.SelectedItem as BanBannerModel).title });
                return;
            }
            string ban = Regex.Match((home_flipView_Ban.SelectedItem as BanBannerModel).link, @"^http://bangumi.bilibili.com/anime/(.*?)$").Groups[1].Value;
            if (ban.Length != 0)
            {
                NavigatedTo(typeof(BanInfoPage), ban);
                return;
            }
            string video = Regex.Match((home_flipView_Ban.SelectedItem as BanBannerModel).link, @"^http://www.bilibili.com/video/av(.*?)/$").Groups[1].Value;
            if (video.Length != 0)
            {
                NavigatedTo(typeof(VideoInfoPage), video);
                return;
            }
            NavigatedTo(typeof(WebViewPage), (home_flipView_Ban.SelectedItem as BanBannerModel).link);
        }
        WebClientClass wc;
        //读取番剧Banner
        private async void GetBanBanner()
        {
            try
            {
                pr_Laod.Visibility = Visibility.Visible;
                wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://bangumi.bilibili.com/api/app_index_page_v2"));
                BanBannerModel model = JsonConvert.DeserializeObject<BanBannerModel>(results);
                if (model.code == 0)
                {
                    JObject jo = JObject.Parse(results);
                    List<BanBannerModel> list = JsonConvert.DeserializeObject<List<BanBannerModel>>(jo["result"]["banners"].ToString());
                    home_flipView_Ban.ItemsSource = list;
                    fvLeft_Ban.ItemsSource = list;
                    fvRight_Ban.ItemsSource = list;
                    this.home_flipView_Ban.SelectedIndex = 0;
                    if (fvLeft_Ban.Visibility != Visibility.Collapsed || fvRight_Ban.Visibility != Visibility.Collapsed)
                    {
                        this.fvLeft_Ban.SelectedIndex = this.fvLeft_Ban.Items.Count - 1;
                        this.fvRight_Ban.SelectedIndex = this.home_flipView_Ban.SelectedIndex + 1;
                    }
                }
                else
                {
                    messShow.Show("读取番剧Banner失败！" + model.message, 2000);
                }
            }
            catch (Exception ex)
            {
                pr_Laod.Visibility = Visibility.Collapsed;
                messShow.Show("读取番剧Banner失败！" + ex.Message, 2000);
                //throw;
            }

        }

        private void gridview_List_ItemClick(object sender, ItemClickEventArgs e)
        {
            sp_Bangumi.IsPaneOpen = false;
            NavigatedTo(typeof(BanByTagPage), new string[] { (e.ClickedItem as TagModel).tag_id.ToString(), (e.ClickedItem as TagModel).tag_name });
        }

        private void Ban_btn_MyBan_Click(object sender, RoutedEventArgs e)
        {
            if (new UserClass().IsLogin())
            {
                NavigatedTo(typeof(UserBangumiPage), UserClass.Uid);
            }
            else
            {
                messShow.Show("请先登录", 3000);
            }
        }

        private void Ban_btn_Tag_Click(object sender, RoutedEventArgs e)
        {
            txt_Title.Text = "番剧索引";
            B_Timeline.Visibility = Visibility.Collapsed;
            gridview_List.Visibility = Visibility.Visible;
            sp_Bangumi.IsPaneOpen = true;
            GetTagInfo();
        }
        //索引
        public async void GetTagInfo()
        {
            try
            {
                wc = new WebClientClass();
                string uri = "http://bangumi.bilibili.com/api/tags?_device=wp&_ulv=10000&appkey=422fd9d7289a1dd9&build=411005&page=" + 1 + "&pagesize=60&platform=android&ts=" + ApiHelper.GetTimeSpen + "000";
                string sign = ApiHelper.GetSign(uri);
                uri += "&sign=" + sign;
                string results = await wc.GetResults(new Uri(uri));
                JObject jo = JObject.Parse(results);
                List<TagModel> list = JsonConvert.DeserializeObject<List<TagModel>>(jo["result"].ToString());
                gridview_List.ItemsSource = list;
            }
            catch (Exception ex)
            {
                if (ex.HResult == -2147012867)
                {
                    messShow.Show("检查你的网络连接！", 3000);
                }
                else
                {
                    messShow.Show("发生错误\r\n" + ex.Message, 3000);
                }
                //await new MessageDialog("读取索引信息失败！\r\n" + ex.Message).ShowAsync();
            }
            finally
            {
                 pr_Load_Ban.Visibility = Visibility.Collapsed;
            }
        }
        private void Ban_btn_All_Click(object sender, RoutedEventArgs e)
        {
            NavigatedTo(typeof(AllBangumiPage),null);
        }

        private void Ban_btn_Timeline_Click(object sender, RoutedEventArgs e)
        {
            txt_Title.Text = "放送时间表";
            B_Timeline.Visibility = Visibility.Visible;
            gridview_List.Visibility = Visibility.Collapsed;
            sp_Bangumi.IsPaneOpen = true;
            GetBangumiTimeLine();
        }


        private async void GetBanUpdate()
        {
            try
            {
                wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://bangumi.bilibili.com/api/app_index_page"));
                BannumiIndexModel model = JsonConvert.DeserializeObject<BannumiIndexModel>(results);
                JObject json = JObject.Parse(model.result.ToString());
                List<BannumiIndexModel> ban = JsonConvert.DeserializeObject<List<BannumiIndexModel>>(json["latestUpdate"]["list"].ToString());
                GridView_Bangumi_NewUpdate.ItemsSource = ban;
            }
            catch (Exception ex)
            {
                if (ex.HResult == -2147012867)
                {
                    messShow.Show("检查你的网络连接！", 3000);
                }
                else
                {
                    messShow.Show("读取番剧最近更新失败\r\n" + ex.Message, 3000);
                }
                //messShow.Show("读取番剧最近更新失败\r\n" + ex.Message, 2000);
            }
        }
        private void GridView_Bangumi_NewUpdate_ItemClick(object sender, ItemClickEventArgs e)
        {
            NavigatedTo(typeof(BanInfoPage), (e.ClickedItem as BannumiIndexModel).season_id);
        }

        private void list_Ban_TJ_ItemClick(object sender, ItemClickEventArgs e)
        {
            //妈蛋，B站就一定要返回个链接么,就不能返回个类型加参数吗
            string tag = Regex.Match((e.ClickedItem as BanTJModel).link, @"^http://bangumi.bilibili.com/anime/category/(.*?)$").Groups[1].Value;
            if (tag.Length != 0)
            {
                NavigatedTo(typeof(BanByTagPage), new string[] { tag, (e.ClickedItem as BanTJModel).title });
                return;
            }
            string ban = Regex.Match((e.ClickedItem as BanTJModel).link, @"^http://bangumi.bilibili.com/anime/(.*?)$").Groups[1].Value;
            if (ban.Length != 0)
            {
                NavigatedTo(typeof(BanInfoPage), ban);
                return;
            }
            //
            string aid = Regex.Match((e.ClickedItem as BanTJModel).link, @"^http://www.bilibili.com/video/av(.*?)/$").Groups[1].Value;
            if (aid.Length != 0)
            {
                NavigatedTo(typeof(VideoInfoPage), aid);
                return;
            }
            NavigatedTo(typeof(WebViewPage), (e.ClickedItem as BanTJModel).link);
        }
        //读取番剧推荐
        string Page_BanTJ = "-1";
        private async void GetBanTJ()
        {
            try
            {
                LoadBaning = true;
                Ban_TJ_more.Text = "正在加载...";
                wc = new WebClientClass();
                string uri = "http://bangumi.bilibili.com/api/bangumi_recommend?_device=wp&appkey=422fd9d7289a1dd9&build=411005&cursor=" + Page_BanTJ + "&pagesize=10&platform=android&ts=" + ApiHelper.GetTimeSpen;
                uri += "&sign=" + ApiHelper.GetSign(uri);
                string results = await wc.GetResults(new Uri(uri));
                BanTJModel model = JsonConvert.DeserializeObject<BanTJModel>(results);
                if (model.code == 0)
                {
                    JObject jo = JObject.Parse(results);
                    List<BanTJModel> list = JsonConvert.DeserializeObject<List<BanTJModel>>(model.result.ToString());
                    foreach (BanTJModel item in list)
                    {
                        list_Ban_TJ.Items.Add(item);
                    }
                    if (list.Count != 0)
                    {
                        Page_BanTJ = (list[list.Count - 1] as BanTJModel).cursor;
                        Ban_TJ_more.Text = "加载更多";
                    }
                    else
                    {
                        Ban_TJ_more.Text = "没有更多了...";
                    }
                }
                else
                {
                    messShow.Show("读取番剧推荐失败！" + model.message, 3000);
                }

            }
            catch (Exception ex)
            {
               
                messShow.Show("读取番剧推荐失败！" + ex.Message, 3000);
              
            }
            finally
            {
                LoadBaning = false;
                pr_Laod.Visibility = Visibility.Collapsed;
            }

        }
        bool LoadBaning = false;
        private  void sc_Ban_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sc_Ban.VerticalOffset == sc_Ban.ScrollableHeight)
            {
                if (!LoadBaning && Ban_TJ_more.Text != "没有更多了...")
                {
                    GetBanTJ();
                }
            }
        }
      
        private void PullToRefreshBox_RefreshInvoked(DependencyObject sender, object args)
        {
            pr_Laod.Visibility = Visibility.Visible;
            GetBanBanner();
            GetBanUpdate();
            GetBanTJ();
        }


        int taday = 0;
        public void SetWeekInfo()
        {
            date_2.Text = DateTime.Now.AddDays(-2).Date.Month + "月" + DateTime.Now.AddDays(-2).Date.Day + "日";
            date_3.Text = DateTime.Now.AddDays(-3).Date.Month + "月" + DateTime.Now.AddDays(-3).Date.Day + "日";
            date_4.Text = DateTime.Now.AddDays(-4).Date.Month + "月" + DateTime.Now.AddDays(-4).Date.Day + "日";
            date_5.Text = DateTime.Now.AddDays(-5).Date.Month + "月" + DateTime.Now.AddDays(-5).Date.Day + "日";
            date_6.Text = DateTime.Now.AddDays(-6).Date.Month + "月" + DateTime.Now.AddDays(-6).Date.Day + "日";

            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    week_0.Text = "周一";
                    week_1.Text = "周日";
                    week_2.Text = "周六";
                    week_3.Text = "周五";
                    week_4.Text = "周四";
                    week_5.Text = "周三";
                    week_5.Text = "周二";
                    taday = 1;
                    break;
                case DayOfWeek.Tuesday:
                    week_1.Text = "周一";
                    week_2.Text = "周日";
                    week_3.Text = "周六";
                    week_4.Text = "周五";
                    week_5.Text = "周四";
                    week_6.Text = "周三";
                    week_0.Text = "周二";
                    taday = 2;
                    break;
                case DayOfWeek.Wednesday:
                    week_2.Text = "周一";
                    week_3.Text = "周日";
                    week_4.Text = "周六";
                    week_5.Text = "周五";
                    week_6.Text = "周四";
                    week_0.Text = "周三";
                    week_1.Text = "周二";
                    taday = 3;
                    break;
                case DayOfWeek.Thursday:
                    week_3.Text = "周一";
                    week_4.Text = "周日";
                    week_5.Text = "周六";
                    week_6.Text = "周五";
                    week_0.Text = "周四";
                    week_1.Text = "周三";
                    week_2.Text = "周二";
                    taday = 4;
                    break;
                case DayOfWeek.Friday:
                    week_4.Text = "周一";
                    week_5.Text = "周日";
                    week_6.Text = "周六";
                    week_0.Text = "周五";
                    week_1.Text = "周四";
                    week_2.Text = "周三";
                    week_3.Text = "周二";
                    taday = 5;
                    break;
                case DayOfWeek.Saturday:
                    week_5.Text = "周一";
                    week_6.Text = "周日";
                    week_0.Text = "周六";
                    week_1.Text = "周五";
                    week_2.Text = "周四";
                    week_3.Text = "周三";
                    week_4.Text = "周二";
                    taday = 6;
                    break;
                case DayOfWeek.Sunday:
                    week_6.Text = "周一";
                    week_0.Text = "周日";
                    week_1.Text = "周六";
                    week_2.Text = "周五";
                    week_3.Text = "周四";
                    week_4.Text = "周三";
                    week_5.Text = "周二";
                    taday = 0;
                    break;
                default:
                    break;
            }
        }
        //时间表
        public async void GetBangumiTimeLine()
        {
            try
            {
                pr_Load_Ban.Visibility = Visibility.Visible;
                wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://app.bilibili.com/bangumi/timeline_v2"));
                BangumiTimeLineModel model = new BangumiTimeLineModel();
                model = JsonConvert.DeserializeObject<BangumiTimeLineModel>(results);
                List<BangumiTimeLineModel> ban = JsonConvert.DeserializeObject<List<BangumiTimeLineModel>>(model.list.ToString());
                list_0.Items.Clear();
                list_1.Items.Clear();
                list_2.Items.Clear();
                list_3.Items.Clear();
                list_4.Items.Clear();
                list_5.Items.Clear();
                list_6.Items.Clear();
                list_7.Items.Clear();
                foreach (BangumiTimeLineModel item in ban)
                {
                    switch (item.weekday)
                    {
                        case -1:
                            list_7.Items.Add(item);
                            break;
                        case 0:
                            switch (taday)
                            {
                                case 0:
                                    list_0.Items.Add(item);
                                    break;
                                case 1:
                                    list_1.Items.Add(item);
                                    break;
                                case 2:
                                    list_2.Items.Add(item);
                                    break;
                                case 3:
                                    list_3.Items.Add(item);
                                    break;
                                case 4:
                                    list_4.Items.Add(item);
                                    break;
                                case 5:
                                    list_5.Items.Add(item);
                                    break;
                                case 6:
                                    list_6.Items.Add(item);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 1:
                            switch (taday)
                            {
                                case 1:
                                    list_0.Items.Add(item);
                                    break;
                                case 2:
                                    list_1.Items.Add(item);
                                    break;
                                case 3:
                                    list_2.Items.Add(item);
                                    break;
                                case 4:
                                    list_3.Items.Add(item);
                                    break;
                                case 5:
                                    list_4.Items.Add(item);
                                    break;
                                case 6:
                                    list_5.Items.Add(item);
                                    break;
                                case 0:
                                    list_6.Items.Add(item);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 2:
                            switch (taday)
                            {
                                case 2:
                                    list_0.Items.Add(item);
                                    break;
                                case 3:
                                    list_1.Items.Add(item);
                                    break;
                                case 4:
                                    list_2.Items.Add(item);
                                    break;
                                case 5:
                                    list_3.Items.Add(item);
                                    break;
                                case 6:
                                    list_4.Items.Add(item);
                                    break;
                                case 0:
                                    list_5.Items.Add(item);
                                    break;
                                case 1:
                                    list_6.Items.Add(item);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 3:
                            switch (taday)
                            {
                                case 3:
                                    list_0.Items.Add(item);
                                    break;
                                case 4:
                                    list_1.Items.Add(item);
                                    break;
                                case 5:
                                    list_2.Items.Add(item);
                                    break;
                                case 6:
                                    list_3.Items.Add(item);
                                    break;
                                case 0:
                                    list_4.Items.Add(item);
                                    break;
                                case 1:
                                    list_5.Items.Add(item);
                                    break;
                                case 2:
                                    list_6.Items.Add(item);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 4:
                            switch (taday)
                            {
                                case 4:
                                    list_0.Items.Add(item);
                                    break;
                                case 5:
                                    list_1.Items.Add(item);
                                    break;
                                case 6:
                                    list_2.Items.Add(item);
                                    break;
                                case 0:
                                    list_3.Items.Add(item);
                                    break;
                                case 1:
                                    list_4.Items.Add(item);
                                    break;
                                case 2:
                                    list_5.Items.Add(item);
                                    break;
                                case 3:
                                    list_6.Items.Add(item);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 5:
                            switch (taday)
                            {
                                case 5:
                                    list_0.Items.Add(item);
                                    break;
                                case 6:
                                    list_1.Items.Add(item);
                                    break;
                                case 0:
                                    list_2.Items.Add(item);
                                    break;
                                case 1:
                                    list_3.Items.Add(item);
                                    break;
                                case 2:
                                    list_4.Items.Add(item);
                                    break;
                                case 3:
                                    list_5.Items.Add(item);
                                    break;
                                case 4:
                                    list_6.Items.Add(item);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case 6:
                            switch (taday)
                            {
                                case 6:
                                    list_0.Items.Add(item);
                                    break;
                                case 0:
                                    list_1.Items.Add(item);
                                    break;
                                case 1:
                                    list_2.Items.Add(item);
                                    break;
                                case 2:
                                    list_3.Items.Add(item);
                                    break;
                                case 3:
                                    list_4.Items.Add(item);
                                    break;
                                case 4:
                                    list_5.Items.Add(item);
                                    break;
                                case 5:
                                    list_6.Items.Add(item);
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }

                }
                pr_Load_Ban.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                if (ex.HResult == -2147012867)
                {
                    messShow.Show("检查你的网络连接！", 3000);
                }
                else
                {
                    messShow.Show("读取番剧更新失败\r\n" + ex.Message, 3000);
                }

            }
        }
        //索引

    }
}
