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
using Newtonsoft.Json;
using Windows.System.Profile;
using Windows.UI.ViewManagement;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace bilibili2.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AllBangumiPage : Page
    {
        public delegate void GoBackHandler();
        public event GoBackHandler BackEvent;
        public AllBangumiPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        //private void btn_back_Click(object sender, RoutedEventArgs e)
        //{
        //    if (this.Frame.CanGoBack)
        //    {
        //        this.Frame.GoBack();

        //    }
        //    else
        //    {
        //        BackEvent();
        //    }
        //}
       
        private void GetFilter()
        {

            view_DHLX.ItemsSource = new List<FilterModel>() {
                new FilterModel() {name="全部",data="1" },
                new FilterModel() {name="TV版",data="2" },
                new FilterModel() {name="OVA·OAD版",data="3" },
                new FilterModel() {name="剧场版",data="4" },
                new FilterModel() {name="其他",data="5" },
            };
            view_DHLX.SelectedIndex = 0;
            view_ZT.ItemsSource = new List<FilterModel>() {
                new FilterModel() {name="不限",data="0" },
                new FilterModel() {name="连载中",data="1" },
                new FilterModel() {name="完结",data="2" },
            };
            view_ZT.SelectedIndex = 0;
            view_DQ.ItemsSource = new List<FilterModel>() {
                new FilterModel() {name="不限",data="" },
                new FilterModel() {name="中国大陆",data="1" },
                new FilterModel() {name="美国",data="2" },
                new FilterModel() {name="英国",data="3" },
                new FilterModel() {name="加拿大",data="4" },
                new FilterModel() {name="中国香港",data="5" },
                new FilterModel() {name="中国台湾",data="6" },
                new FilterModel() {name="韩国",data="7" },
                new FilterModel() {name="法国",data="8" },
                new FilterModel() {name="泰国",data="9" },
                new FilterModel() {name="西班牙",data="10" },
                new FilterModel() {name="俄罗斯",data="11" },
                new FilterModel() {name="其他",data="12" },
            };
            view_DQ.SelectedIndex = 0;
            view_GXSJ.ItemsSource = new List<FilterModel>() {
                new FilterModel() {name="不限",data="0" },
                new FilterModel() {name="三日内",data="1" },
                new FilterModel() {name="七日内",data="2" },
                new FilterModel() {name="半月内",data="3" },
                new FilterModel() {name="一月内",data="4" },
            };
            view_GXSJ.SelectedIndex = 0;
            view_FG.ItemsSource = new List<FilterModel>() {
                new FilterModel() {name="不限",data="" },
                new FilterModel() {name="热门番剧",data="114" },
                new FilterModel() {name="轻改",data="117" },
                new FilterModel() {name="名作之壁",data="128" },
                new FilterModel() {name="神ED",data="100" },
                new FilterModel() {name="神OP",data="99" },
                new FilterModel() {name="燃烧经费",data="123" },
                new FilterModel() {name="萌萌哒",data="81" },
                new FilterModel() {name="深井冰",data="70" },
                new FilterModel() {name="燃",data="20" },
                new FilterModel() {name="催泪",data="104" },
                 new FilterModel() {name="后宫",data="5" },
                new FilterModel() {name="机战",data="105" },
                new FilterModel() {name="基腐",data="82" },
                new FilterModel() {name="恋爱",data="110" },
                new FilterModel() {name="百合",data="6" },
                new FilterModel() {name="伪娘",data="125" },
                new FilterModel() {name="科幻",data="71" },
                new FilterModel() {name="乙女向",data="115" },
                new FilterModel() {name="奇幻",data="57" },
                new FilterModel() {name="推理",data="124" },
                new FilterModel() { name = "音乐",data = "72" },
               new FilterModel() { name = "校园",data = "93" },
              new FilterModel() { name = "爱抖露",data = "121" },
                  new FilterModel() { name = "社团",data = "127" },
              new FilterModel() { name = "运动",data = "23" },
             new FilterModel() { name = "少女",data = "9" },
             new FilterModel() { name = "叶良辰",data = "94" },
               new FilterModel() { name = "头脑战",data = "103" },
         new FilterModel() { name = "战斗",data = "95" },
              new FilterModel() { name = "马猴烧酒",data = "16" },
          new FilterModel() { name = "治愈",data = "21" },
          new FilterModel() { name = "豪华声优",data = "98" },
         new FilterModel() { name = "泡面",data = "44" },
          new FilterModel() { name = "历史",data = "67" },
      new FilterModel() { name = "猎奇",data = "87" },
          new FilterModel() { name = "致郁",data = "22" },
    new FilterModel() { name = "时泪",data = "88" },
    new FilterModel() { name = "美食",data = "106" },
    new FilterModel() { name = "美国",data = "89" },
    new FilterModel() { name = "国产",data = "90" },
    new FilterModel() { name = "子供向",data = "24" },
      new FilterModel() { name = "小编私货",data = "87" },
    new FilterModel() { name = "漫改",data = "97" },
    new FilterModel() { name = "游戏改",data = "135" },
    new FilterModel() { name = "原创",data = "137" },
    new FilterModel() { name = "励志",data = "138" },
    new FilterModel() { name = "神魔",data = "139" },
    new FilterModel() { name = "职场",data = "140" },
     new FilterModel() { name = "萝莉",data = "141" }
            };
            view_FG.SelectedIndex = 0;
            var nf = new List<FilterModel>() {
                new FilterModel() {name="不限",data="" },
                new FilterModel() {name="1928",data="1928" },
                new FilterModel() {name="1940",data="1940" },
                new FilterModel() {name="1957",data="1957" },
                new FilterModel() {name="1958",data="1958" },
                new FilterModel() {name="1961",data="1961" },
                new FilterModel() {name="1962",data="1962" },
                new FilterModel() {name="1963",data="1963" },
                 new FilterModel() {name="1969",data="1969" },
            };
            for (int i = 1970; i < 2017; i++)
            {
                nf.Add(new FilterModel() { name=i.ToString(),data=i.ToString()});
            }
            view_NF.ItemsSource = nf;

            view_NF.SelectedIndex = 0;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            bg.Color = ((SolidColorBrush)this.Frame.Tag).Color;

            if (e.NavigationMode == NavigationMode.New)
            {
                await Task.Delay(200);
                GetFilter();
                GetInfo();
            }
        }



        bool canLoad = true;
        private void sc_viewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sc_viewer.VerticalOffset == sc_viewer.ScrollableHeight)
            {
                if (canLoad)
                {
                    GetInfo();
                }
            }
        }
        private void btn_LoadMore_Click(object sender, RoutedEventArgs e)
        {
            if (canLoad)
            {
                GetInfo();
            }
        }
        private void btn_CZ_Click(object sender, RoutedEventArgs e)
        {
             DHLX = "1";
             ZT = "0";
             DQ = "";
             GXSJ = "0";
             FG = "";
             NF = "";
             PageNum = 1;
            view_DHLX.SelectedIndex = 0;
            view_DQ.SelectedIndex = 0;
            view_FG.SelectedIndex = 0;
            view_GXSJ.SelectedIndex = 0;
            view_NF.SelectedIndex = 0;
            view_ZT.SelectedIndex = 0;
            items.Items.Clear();
            GetInfo();
        }

        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            DHLX = (view_DHLX.SelectedItem as FilterModel).data;
            ZT = (view_ZT.SelectedItem as FilterModel).data;
            DQ = (view_DQ.SelectedItem as FilterModel).data;
            GXSJ= (view_GXSJ.SelectedItem as FilterModel).data;
            FG = (view_FG.SelectedItem as FilterModel).data;
            NF = (view_NF.SelectedItem as FilterModel).data;
            PageNum = 1;
            items.Items.Clear();
            GetInfo();
        }
        string DHLX = "1";
        string ZT = "0";
        string DQ = "";
        string GXSJ = "0";
        string FG = "";
        string NF = "";
        int PageNum = 1;
        int index_type = 1;
        int index_sort = 0;
       // int pageCount = 30;
        WebClientClass wc;
        private async void GetInfo()
        {
            try
            {
                canLoad = false;
                pr_load.Visibility = Visibility.Visible;
                wc = new WebClientClass();
                string url = "http://bangumi.bilibili.com/web_api/season/index?page="+ PageNum + "&pagesize="+ 30;
                url += "&version" + DHLX;
                url += "&is_finish=" + ZT;
                //url += "&is_finish=" + ZT;
                url += "&update_period=" + GXSJ;
                if (DQ.Length!=0)
                {
                    url += "&area=" + DQ;
                }
                if (FG.Length!=0)
                {
                    url += "&tag_id=" + FG;
                }
                if (NF.Length != 0)
                {
                    url += "&start_year=" + NF;
                }
                url += "&index_type=" + index_type.ToString();
                //url += "&index_sort=" + index_sort.ToString();
                
                string results = await wc.GetResults(new Uri(url));
                ALLBangumiModel all = JsonConvert.DeserializeObject<ALLBangumiModel>(results);
                if (all.code==0)
                {
                    if (all.result.list.Count!=0)
                    {
                        foreach (var item in all.result.list)
                        {
                            items.Items.Add(item);
                        }
                        PageNum++;
                    }
                    else
                    {
                        messShow.Show("没有结果了",2000);
                    }
                }
                else
                {
                    messShow.Show(all.message,2000);
                }
            }
            catch (Exception ex)
            {
                messShow.Show("发生错误"+ex.Message, 2000);
                //throw;
            }
            finally
            {
                canLoad = true;
                pr_load.Visibility = Visibility.Collapsed;
            }
        }
        public DeriveTypes GetDeriveType()
        {
            var deviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
            if (deviceFamily == "Windows.Desktop")
            {
                if (UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse)
                {
                    return DeriveTypes.PC;
                }
                else
                {
                    return DeriveTypes.Pad;
                }
            }
            else
            {
                return DeriveTypes.Phone;
            }
        }
        private void tw_zf_0_Click(object sender, RoutedEventArgs e)
        {
            tw_zf_0.IsChecked = true;
            tw_zf_1.IsChecked = false;
            tw_gx_0.IsChecked = false;
            tw_gx_1.IsChecked = false;
            tw_rq_0.IsChecked = false;
            tw_rq_1.IsChecked = false;
            PageNum = 1;
            index_type = 1;
            index_sort = 0;
            items.Items.Clear();
            GetInfo();

        }

        private void tw_zf_1_Click(object sender, RoutedEventArgs e)
        {
            tw_zf_0.IsChecked = false;
            tw_zf_1.IsChecked = true;
            tw_gx_0.IsChecked = false;
            tw_gx_1.IsChecked = false;
            tw_rq_0.IsChecked = false;
            tw_rq_1.IsChecked = false;
            PageNum = 1;
            index_type = 1;
            index_sort = 1;
            items.Items.Clear();
            GetInfo();
        }

        private void tw_gx_0_Click(object sender, RoutedEventArgs e)
        {
            tw_zf_0.IsChecked = false;
            tw_zf_1.IsChecked = false;
            tw_gx_0.IsChecked = true;
            tw_gx_1.IsChecked = false;
            tw_rq_0.IsChecked = false;
            tw_rq_1.IsChecked = false;
            PageNum = 1;
            index_type = 0;
            index_sort = 0;
            items.Items.Clear();
            GetInfo();
        }

        private void tw_gx_1_Click(object sender, RoutedEventArgs e)
        {
            tw_zf_0.IsChecked = false;
            tw_zf_1.IsChecked = false;
            tw_gx_0.IsChecked = false;
            tw_gx_1.IsChecked = true;
            tw_rq_0.IsChecked = false;
            tw_rq_1.IsChecked = false;
            PageNum = 1;
            index_type = 0;
            index_sort = 1;
            items.Items.Clear();
            GetInfo();
        }

        private void tw_rq_0_Click(object sender, RoutedEventArgs e)
        {
            tw_zf_0.IsChecked = false;
            tw_zf_1.IsChecked = false;
            tw_gx_0.IsChecked = false;
            tw_gx_1.IsChecked = false;
            tw_rq_0.IsChecked = true;
            tw_rq_1.IsChecked = false;
            PageNum = 1;
            index_type = 2;
            index_sort = 0;
            items.Items.Clear();
            GetInfo();
        }

        private void tw_rq_1_Click(object sender, RoutedEventArgs e)
        {
            tw_zf_0.IsChecked = false;
            tw_zf_1.IsChecked = false;
            tw_gx_0.IsChecked = false;
            tw_gx_1.IsChecked = false;
            tw_rq_0.IsChecked = false;
            tw_rq_1.IsChecked = true;
            PageNum = 1;
            index_type = 2;
            index_sort = 1;
            items.Items.Clear();
            GetInfo();

        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int i = Convert.ToInt32(ActualWidth / 120);
            ViewBox2_num.Width = ActualWidth / i - 11;
        }

        private void items_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(BanInfoPage),(e.ClickedItem as ALLBangumiModel).season_id);
        }
    }





    public class FilterModel
    {
        public string name { get; set; }
        public string data { get; set; }
    }
    public class ALLBangumiModel
    {
        public int code { get; set; }
        public string message { get; set; }
        public ALLBangumiModel result { get; set; }
        public string count { get; set; }
        public string pages { get; set; }
        public List<ALLBangumiModel> list { get; set; }

        public string cover { get; set; }
        public int is_finish { get; set; }
        public string newest_ep_index { get; set; }
        public string season_id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        public int total_count { get; set; }
        public string week { get; set; }
    }

}
