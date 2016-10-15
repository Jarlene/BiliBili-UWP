using bilibili2.Class;
using bilibili2.Pages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
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
using Windows.Web.Http;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace bilibili2.PartPages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DHPage : Page
    {
        public delegate void GoBackHandler();
        public event GoBackHandler BackEvent;
        public DHPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int d = Convert.ToInt32(this.ActualWidth / 400);
            bor_Width.Width = this.ActualWidth / d - 22;

            if (this.ActualWidth <= 640)
            {
                fvLeft.Visibility = Visibility.Collapsed;
                fvRight.Visibility = Visibility.Collapsed;
                grid_c_left.Width = new GridLength(0, GridUnitType.Auto);
                grid_c_right.Width = new GridLength(0, GridUnitType.Auto);
                grid_c_center.Width = new GridLength(1, GridUnitType.Star);
            }
            else
            {
                fvLeft.Visibility = Visibility.Visible;
                fvRight.Visibility = Visibility.Visible;
                grid_c_left.Width = new GridLength(1, GridUnitType.Star);
                grid_c_right.Width = new GridLength(1, GridUnitType.Star);
                grid_c_center.Width = new GridLength(0, GridUnitType.Auto);
            }



            if (this.ActualWidth <= 500)
            {
                ViewBox_num.Width = ActualWidth / 2 - 20;
                ViewBox2_num.Width = ActualWidth / 2 - 20;
            }
            else
            {
                int i = Convert.ToInt32(ActualWidth / 170);
                ViewBox_num.Width = ActualWidth / i - 15;
                ViewBox2_num.Width = ActualWidth / i - 15;
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            bg.Color = ((SolidColorBrush)this.Frame.Tag).Color;
            if (e.NavigationMode == NavigationMode.New)
            {
                GetTags();
                PageNum = 1;
                GetDYHome();
                GetDT();
            }
        }
        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
            else
            {
                BackEvent();
            }
        }

        private HttpClient hc;
        /// <summary>
        /// Banner，推荐，最新
        /// </summary>
        private async void GetDYHome()
        {
            try
            {
                pro_Bar.Visibility = Visibility.Visible;
                using (hc = new HttpClient())
                {
                    HttpResponseMessage hr = await hc.GetAsync(new Uri("http://app.bilibili.com/api/region2/1.json"));
                    hr.EnsureSuccessStatusCode();
                    var encodeResults = await hr.Content.ReadAsBufferAsync();
                    string results = Encoding.UTF8.GetString(encodeResults.ToArray(), 0, encodeResults.ToArray().Length);
                    DHModel model = JsonConvert.DeserializeObject<DHModel>(results);
                    DHModel model2 = JsonConvert.DeserializeObject<DHModel>(model.result.ToString());
                    List<DHModel> BannerModel = JsonConvert.DeserializeObject<List<DHModel>>(model2.banners.ToString());
                    home_flipView.ItemsSource = BannerModel;
                    fvLeft.ItemsSource = BannerModel;
                    fvRight.ItemsSource = BannerModel;
                    this.home_flipView.SelectedIndex = 0;

                    if (fvLeft.Visibility != Visibility.Collapsed && fvRight.Visibility != Visibility.Collapsed)
                    {
                        this.fvLeft.SelectedIndex = this.fvLeft.Items.Count - 1;
                        this.fvRight.SelectedIndex = this.home_flipView.SelectedIndex + 1;
                    }



                    //GridView_TJ.ItemsSource = RecommendsModel;
                    //GridView_New.ItemsSource = NewsModel;
                }
                pro_Bar.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message);
                await md.ShowAsync();
            }
        }
        /// <summary>
        /// 动态
        /// </summary>
        int PageNum = 1;
        private async void GetDT()
        {
            pro_Bar.Visibility = Visibility.Visible;
            try
            {
                using (hc = new HttpClient())
                {
                    if (PageNum == 1)
                    {
                        GridView_DT.Items.Clear();
                    }
                    CanLoad = false;
                    HttpResponseMessage hr = await hc.GetAsync(new Uri("http://www.bilibili.com/index/ding/1.json?rnd=" + new Random().Next(1, 9999)));
                    hr.EnsureSuccessStatusCode();

                    // var encodeResults = await hr.Content.ReadAsBufferAsync();
                    string results = await hr.Content.ReadAsStringAsync();
                    DHModel model = JsonConvert.DeserializeObject<DHModel>(results);
                    List<DHModel> DTModel = JsonConvert.DeserializeObject<List<DHModel>>(model.list.ToString());
                    foreach (DHModel item in DTModel)
                    {
                        GridView_DT.Items.Add(item);
                    }
                    CanLoad = true;
                    pro_Bar.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message);
                await md.ShowAsync();
            }
        }
        //加载综合
        int PageNum_LZ = 1;
        private async void GetLZNew(int order)
        {
            pro_Bar.Visibility = Visibility.Visible;
            try
            {
                CanLoad = false;
                using (hc = new HttpClient())
                {
                    #region
                    string uri = "";
                    //http://app.bilibili.com/x/region/show/two/old?appkey=1d8b6e7d45233436&build=422000&mobi_app=android&platform=android&tag_name=%E9%9D%99%E6%AD%A2%E7%B3%BBMAD&tid=24&ts=1469172406000&sign=487b3ce4705ae8813d0a5a6e3dc93ae4
                    switch (order)
                    {
                        case 0:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=hot&page={1}&pagesize=20&platform=android&tid=27", ApiHelper._appKey_Android, PageNum_LZ);
                            break;
                        case 1:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=damku&page={1}&pagesize=20&platform=android&tid=27", ApiHelper._appKey_Android, PageNum_LZ);
                            break;
                        case 2:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=hot&page={1}&pagesize=20&platform=android&tid=27", ApiHelper._appKey_Android, PageNum_LZ);
                            break;
                        case 3:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=review&page={1}&pagesize=20&platform=android&tid=27", ApiHelper._appKey_Android, PageNum_LZ);
                            break;
                        case 4:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=stow&page={1}&pagesize=20&platform=android&tid=27", ApiHelper._appKey_Android, PageNum_LZ);
                            break;
                        default:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=hot&page={1}&pagesize=20&platform=android&tid=27", ApiHelper._appKey_Android, PageNum_LZ);
                            break;
                    }
                    if (grid_tag.SelectedIndex != 0)
                    {
                        uri += "&tag=" + Uri.EscapeDataString((grid_tag.SelectedItem as TagsModel).tag_name);
                    }
                    uri += "&sign=" + ApiHelper.GetSign_Android(uri);
                    #endregion
                    if (PageNum_LZ == 1)
                    {
                        LZ_NewList.Items.Clear();
                    }
                    HttpResponseMessage hr = await hc.GetAsync(new Uri(uri));
                    hr.EnsureSuccessStatusCode();
                    string results = await hr.Content.ReadAsStringAsync();
                    DHModel model = JsonConvert.DeserializeObject<DHModel>(results);
                    JObject json = JObject.Parse(model.list.ToString());
                    List<DHModel> ReList = new List<DHModel>();
                    //LZ_NewList.Items.Clear();
                    for (int i = 0; i < 20; i++)
                    {
                        LZ_NewList.Items.Add(new DHModel
                        {
                            aid = (string)json[i.ToString()]["aid"],
                            title = (string)json[i.ToString()]["title"],
                            pic = (string)json[i.ToString()]["pic"],
                            author = (string)json[i.ToString()]["author"],
                            play = (string)json[i.ToString()]["play"],
                            video_review = (string)json[i.ToString()]["video_review"],
                        });
                    }
                    CanLoad = true;
                    PageNum_LZ++;
                    pro_Bar.Visibility = Visibility.Collapsed;
                    //LZ_NewList.ItemsSource = ReList;
                }

            }
            catch (Exception)
            {
                pro_Bar.Visibility = Visibility.Collapsed;
            }
        }
        //加载MAD
        int PageNum_WJ = 1;
        private async void GetWJList(int order)
        {
            pro_Bar.Visibility = Visibility.Visible;
            try
            {
                CanLoad = false;
                using (hc = new HttpClient())
                {
                    #region
                    string uri = "";
                    switch (order)
                    {
                        case 0:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=hot&page={1}&pagesize=20&platform=android&tid=24", ApiHelper._appKey_Android, PageNum_WJ);
                            break;
                        case 1:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=damku&page={1}&pagesize=20&platform=android&tid=24", ApiHelper._appKey_Android, PageNum_WJ);
                            break;
                        case 2:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=hot&page={1}&pagesize=20&platform=android&tid=24", ApiHelper._appKey_Android, PageNum_WJ);
                            break;
                        case 3:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=review&page={1}&pagesize=20&platform=android&tid=24", ApiHelper._appKey_Android, PageNum_WJ);
                            break;
                        case 4:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=stow&page={1}&pagesize=20&platform=android&tid=24", ApiHelper._appKey_Android, PageNum_WJ);
                            break;
                        default:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=hot&page={1}&pagesize=20&platform=android&tid=24", ApiHelper._appKey_Android, PageNum_WJ);
                            break;
                    }
                    if (grid_mad_tag.SelectedIndex != 0)
                    {
                        uri += "&tag=" + Uri.EscapeDataString((grid_mad_tag.SelectedItem as TagsModel).tag_name);
                    }
                    uri += "&sign=" + ApiHelper.GetSign_Android(uri);
                    #endregion
                    if (PageNum_WJ == 1)
                    {
                        WJ_NewList.Items.Clear();
                    }
                    HttpResponseMessage hr = await hc.GetAsync(new Uri(uri));
                    hr.EnsureSuccessStatusCode();
                    string results = await hr.Content.ReadAsStringAsync();
                    DHModel model = JsonConvert.DeserializeObject<DHModel>(results);
                    JObject json = JObject.Parse(model.list.ToString());
                    List<DHModel> ReList = new List<DHModel>();
                    for (int i = 0; i < 20; i++)
                    {
                        WJ_NewList.Items.Add(new DHModel
                        {
                            aid = (string)json[i.ToString()]["aid"],
                            title = (string)json[i.ToString()]["title"],
                            pic = (string)json[i.ToString()]["pic"],
                            author = (string)json[i.ToString()]["author"],
                            play = (string)json[i.ToString()]["play"],
                            video_review = (string)json[i.ToString()]["video_review"],
                        });
                    }
                    CanLoad = true;
                    PageNum_WJ++;
                    pro_Bar.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception)
            {
                pro_Bar.Visibility = Visibility.Collapsed;
            }
        }
        //加载MMD
        int PageNum_GC = 1;
        private async void GetGCList(int order)
        {
            pro_Bar.Visibility = Visibility.Visible;
            try
            {
                CanLoad = false;
                using (hc = new HttpClient())
                {
                    #region
                    string uri = "";
                    switch (order)
                    {
                        case 0:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=hot&page={1}&pagesize=20&platform=android&tid=25", ApiHelper._appKey_Android, PageNum_GC);
                            break;
                        case 1:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=damku&page={1}&pagesize=20&platform=android&tid=25", ApiHelper._appKey_Android, PageNum_GC);
                            break;
                        case 2:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=hot&page={1}&pagesize=20&platform=android&tid=25", ApiHelper._appKey_Android, PageNum_GC);
                            break;
                        case 3:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=review&page={1}&pagesize=20&platform=android&tid=25", ApiHelper._appKey_Android, PageNum_GC);
                            break;
                        case 4:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=stow&page={1}&pagesize=20&platform=android&tid=25", ApiHelper._appKey_Android, PageNum_GC);
                            break;
                        default:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=hot&page={1}&pagesize=20&platform=android&tid=25", ApiHelper._appKey_Android, PageNum_GC);
                            break;
                    }
                    if (grid_mmd_tag.SelectedIndex != 0)
                    {
                        uri += "&tag=" + Uri.EscapeDataString((grid_mmd_tag.SelectedItem as TagsModel).tag_name);
                    }
                    uri += "&sign=" + ApiHelper.GetSign_Android(uri);
                    #endregion
                    if (PageNum_GC == 1)
                    {
                        GC_NewList.Items.Clear();
                    }
                    HttpResponseMessage hr = await hc.GetAsync(new Uri(uri));
                    hr.EnsureSuccessStatusCode();
                    string results = await hr.Content.ReadAsStringAsync();
                    DHModel model = JsonConvert.DeserializeObject<DHModel>(results);
                    JObject json = JObject.Parse(model.list.ToString());
                    List<DHModel> ReList = new List<DHModel>();
                    for (int i = 0; i < 20; i++)
                    {
                        GC_NewList.Items.Add(new DHModel
                        {
                            aid = (string)json[i.ToString()]["aid"],
                            title = (string)json[i.ToString()]["title"],
                            pic = (string)json[i.ToString()]["pic"],
                            author = (string)json[i.ToString()]["author"],
                            play = (string)json[i.ToString()]["play"],
                            video_review = (string)json[i.ToString()]["video_review"],
                        });
                    }
                    CanLoad = true;
                    PageNum_GC++;
                    pro_Bar.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception)
            {
                pro_Bar.Visibility = Visibility.Collapsed;
            }
        }
        //加载短片
        int PageNum_ZX = 1;
        private async void GetZXList(int order)
        {
            pro_Bar.Visibility = Visibility.Visible;
            try
            {
                CanLoad = false;
                using (hc = new HttpClient())
                {
                    #region
                    string uri = "";
                    switch (order)
                    {
                        case 0:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=hot&page={1}&pagesize=20&platform=android&tid=47", ApiHelper._appKey_Android, PageNum_ZX);
                            break;
                        case 1:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=damku&page={1}&pagesize=20&platform=android&tid=47", ApiHelper._appKey_Android, PageNum_ZX);
                            break;
                        case 2:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=hot&page={1}&pagesize=20&platform=android&tid=47", ApiHelper._appKey_Android, PageNum_ZX);
                            break;
                        case 3:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=review&page={1}&pagesize=20&platform=android&tid=47", ApiHelper._appKey_Android, PageNum_ZX);
                            break;
                        case 4:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=stow&page={1}&pagesize=20&platform=android&tid=47", ApiHelper._appKey_Android, PageNum_ZX);
                            break;
                        default:
                            uri = string.Format("http://api.bilibili.com/list?_device=android&appkey={0}&build=422000&mobi_app=android&order=hot&page={1}&pagesize=20&platform=android&tid=47", ApiHelper._appKey_Android, PageNum_ZX);
                            break;
                    }
                    if (grid_mmd_dp.SelectedIndex != 0)
                    {
                        uri += "&tag=" + Uri.EscapeDataString((grid_mmd_dp.SelectedItem as TagsModel).tag_name);
                    }
                    uri += "&sign=" + ApiHelper.GetSign_Android(uri);
                    #endregion
                    if (PageNum_ZX == 1)
                    {
                        ZX_NewList.Items.Clear();
                    }
                    HttpResponseMessage hr = await hc.GetAsync(new Uri(uri));
                    hr.EnsureSuccessStatusCode();
                    string results = await hr.Content.ReadAsStringAsync();
                    DHModel model = JsonConvert.DeserializeObject<DHModel>(results);
                    JObject json = JObject.Parse(model.list.ToString());
                    List<DHModel> ReList = new List<DHModel>();
                    for (int i = 0; i < 20; i++)
                    {
                        ZX_NewList.Items.Add(new DHModel
                        {
                            aid = (string)json[i.ToString()]["aid"],
                            title = (string)json[i.ToString()]["title"],
                            pic = (string)json[i.ToString()]["pic"],
                            author = (string)json[i.ToString()]["author"],
                            play = (string)json[i.ToString()]["play"],
                            video_review = (string)json[i.ToString()]["video_review"],
                        });
                    }
                    CanLoad = true;
                    PageNum_ZX++;
                    pro_Bar.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception)
            {
                pro_Bar.Visibility = Visibility.Collapsed;
            }
        }


        //用于跳转视频页
        private void GridView_TJ_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(VideoInfoPage), ((DHModel)e.ClickedItem).aid);
        }
        //Banner点击
        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            var m = (DHModel)home_flipView.SelectedItem;
            if (m.aid != "0")
            {
                this.Frame.Navigate(typeof(VideoInfoPage), m.aid);
                return;
            }
            string ban = Regex.Match(m.link, @"^http://bangumi.bilibili.com/anime/(.*?)$").Groups[1].Value;
            if (ban.Length != 0)
            {
                this.Frame.Navigate(typeof(BanInfoPage), ban);
                return;
            }
            string ban2 = Regex.Match(m.link, @"^http://www.bilibili.com/bangumi/i/(.*?)$").Groups[1].Value;
            if (ban2.Length != 0)
            {
                this.Frame.Navigate(typeof(BanInfoPage), ban2);
                return;
            }
            string aid = Regex.Match(m.link, @"^http://www.bilibili.com/video/av(.*?)/$").Groups[1].Value;
            if (aid.Length != 0)
            {
                this.Frame.Navigate(typeof(VideoInfoPage), aid);
                return;
            }
            this.Frame.Navigate(typeof(WebViewPage), m.link);
        }
        //用于跳转视频页
        private void ZH_HotList_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(VideoInfoPage), ((DHModel)e.ClickedItem).aid);
        }

        //判断是不是在加载中
        bool CanLoad = true;
        private void sv_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

        }

        //用了判断是否已经加载相关信息
        bool LZ_Load = false;
        bool WJ_Load = false;
        bool GC_Load = false;
        bool ZX_Load = false;
        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (pivot.SelectedIndex == 0)
            {
                cb_Bar.Visibility = Visibility.Collapsed;
                return;
            }
            else
            {
                btn_Type.IsChecked = false;
                grid_tag.Visibility = Visibility.Collapsed;
                grid_mad_tag.Visibility = Visibility.Collapsed;
                grid_mmd_dp.Visibility = Visibility.Collapsed;
                grid_mmd_tag.Visibility = Visibility.Collapsed;

                btn_New.IsEnabled = true;
                btn_Comment.IsEnabled = true;
                btn_Danmaku.IsEnabled = true;
                btn_Play.IsEnabled = false;
                btn_Sc.IsEnabled = true;
                cb_Bar.Visibility = Visibility.Visible;
            }
            switch (pivot.SelectedIndex)
            {
                case 1:
                    if (!LZ_Load)
                    {
                        //GetLZNew(2);
                        //
                        if (grid_tag.Items.Count!=0)
                        {
                            grid_tag.ItemsSource = ZH_Tag;
                            grid_tag.SelectedIndex = 0;
                            grid_tag.Visibility = Visibility.Collapsed;
                            LZ_Load = true;
                        }
                        else
                        {
                            GetTags();
                        }
                    }
                    break;
                case 2:
                    if (!WJ_Load)
                    {
                        if (grid_mad_tag.Items.Count != 0)
                        {
                            //GetWJList(2);
                            grid_mad_tag.ItemsSource = MAD_Tag;
                            grid_mad_tag.SelectedIndex = 0;
                            grid_mad_tag.Visibility = Visibility.Collapsed;

                            WJ_Load = true;
                        }
                        else
                        {
                            GetTags();

                        }
                    }
                    break;
                case 3:
                    if (!GC_Load)
                    {
                        if (grid_mmd_tag.Items.Count != 0)
                        {
                            //GetGCList(2);
                            grid_mmd_tag.ItemsSource = MMD_Tag;
                            grid_mmd_tag.SelectedIndex = 0;
                            grid_mmd_tag.Visibility = Visibility.Collapsed;
                            GC_Load = true;
                        }
                        else
                        {
                            GetTags();
                        }
                    }
                    break;
                case 4:
                    if (!ZX_Load)
                    {
                        if (grid_mmd_dp.Items.Count != 0)
                        {
                            //GetZXList(2);
                            grid_mmd_dp.ItemsSource = DP_Tag;
                            grid_mmd_dp.SelectedIndex = 0;
                            grid_mmd_dp.Visibility = Visibility.Collapsed;
                            ZX_Load = true;
                        }
                        else
                        {
                            GetTags();
                        }
                    }
                    break;
                default:
                    break;
            }

        }

        //用来判断按什么排序
        int LZ_Order = 2;
        int WJ_Order = 2;
        int GC_Order = 2;
        int ZX_Order = 2;

        //order 0为默认,1按弹幕,2按播放,3按评论,4按收藏
        private void btn_New_Click(object sender, RoutedEventArgs e)
        {
            btn_New.IsEnabled = false;
            btn_Comment.IsEnabled = true;
            btn_Danmaku.IsEnabled = true;
            btn_Play.IsEnabled = true;
            btn_Sc.IsEnabled = true;
            switch (pivot.SelectedIndex)
            {
                case 1:
                    PageNum_LZ = 1;
                    LZ_Order = 0;
                    GetLZNew(0);
                    break;
                case 2:
                    PageNum_WJ = 1;
                    WJ_Order = 0;
                    GetWJList(0);
                    break;
                case 3:
                    GC_Order = 0;
                    PageNum_GC = 1;
                    GetGCList(0);
                    break;
                case 4:
                    ZX_Order = 0;
                    PageNum_ZX = 1;
                    GetZXList(0);
                    break;
                default:
                    break;
            }
        }
        private void btn_Danmaku_Click(object sender, RoutedEventArgs e)
        {
            btn_New.IsEnabled = true;
            btn_Comment.IsEnabled = true;
            btn_Danmaku.IsEnabled = false;
            btn_Play.IsEnabled = true;
            btn_Sc.IsEnabled = true;
            switch (pivot.SelectedIndex)
            {
                case 1:
                    PageNum_LZ = 1;
                    LZ_Order = 1;
                    GetLZNew(1);
                    break;
                case 2:
                    PageNum_WJ = 1;
                    WJ_Order = 1;
                    GetWJList(1);
                    break;
                case 3:
                    GC_Order = 1;
                    PageNum_GC = 1;
                    GetGCList(1);
                    break;
                case 4:
                    ZX_Order = 1;
                    PageNum_ZX = 1;
                    GetZXList(1);
                    break;
                default:
                    break;
            }
        }
        private void btn_Play_Click(object sender, RoutedEventArgs e)
        {
            btn_New.IsEnabled = true;
            btn_Comment.IsEnabled = true;
            btn_Danmaku.IsEnabled = true;
            btn_Play.IsEnabled = false;
            btn_Sc.IsEnabled = true;
            switch (pivot.SelectedIndex)
            {
                case 1:
                    //GridView_New.Items.Clear();
                    PageNum_LZ = 1;
                    GetLZNew(2);
                    LZ_Order = 2;
                    break;
                case 2:
                    //GridView_New.Items.Clear();
                    PageNum_WJ = 1;
                    LZ_Order = 2;
                    GetWJList(2);
                    break;
                case 3:
                    GC_Order = 2;
                    PageNum_GC = 1;
                    GetGCList(2);
                    break;
                case 4:
                    ZX_Order = 2;
                    PageNum_ZX = 1;
                    GetZXList(2);
                    break;
                default:
                    break;
            }
        }
        private void btn_Comment_Click(object sender, RoutedEventArgs e)
        {
            btn_New.IsEnabled = true;
            btn_Comment.IsEnabled = false;
            btn_Danmaku.IsEnabled = true;
            btn_Play.IsEnabled = true;
            btn_Sc.IsEnabled = true;
            switch (pivot.SelectedIndex)
            {
                case 1:
                    //GridView_New.Items.Clear();
                    PageNum_LZ = 1;
                    LZ_Order = 3;
                    GetLZNew(3);
                    break;
                case 2:
                    PageNum_WJ = 1;
                    WJ_Order = 3;
                    GetWJList(3);
                    break;
                case 3:
                    GC_Order = 3;
                    PageNum_GC = 1;
                    GetGCList(3);
                    break;
                case 4:
                    ZX_Order = 3;
                    PageNum_ZX = 1;
                    GetZXList(3);
                    break;
                default:
                    break;
            }
        }
        private void btn_Sc_Click(object sender, RoutedEventArgs e)
        {
            btn_New.IsEnabled = true;
            btn_Comment.IsEnabled = true;
            btn_Danmaku.IsEnabled = true;
            btn_Play.IsEnabled = true;
            btn_Sc.IsEnabled = false;
            switch (pivot.SelectedIndex)
            {
                case 1:
                    //GridView_New.Items.Clear();
                    PageNum_LZ = 1;
                    LZ_Order = 4;
                    GetLZNew(4);
                    break;
                case 2:
                    //GridView_New.Items.Clear();
                    PageNum_WJ = 1;
                    WJ_Order = 4;
                    GetWJList(4);
                    break;
                case 3:
                    GC_Order = 4;
                    PageNum_GC = 1;
                    GetGCList(4);
                    break;
                case 4:
                    ZX_Order = 4;
                    PageNum_ZX = 1;
                    GetZXList(4);
                    break;
                default:
                    break;
            }
        }
        //加载更多
        private void sv_LZ_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sv_LZ.VerticalOffset == sv_LZ.ScrollableHeight)
            {
                if (CanLoad)
                {
                    GetLZNew(LZ_Order);
                }
            }
        }
        private void sv_WJ_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sv_WJ.VerticalOffset == sv_WJ.ScrollableHeight)
            {
                if (CanLoad)
                {
                    GetWJList(WJ_Order);
                }
            }
        }
        private void sc_GC_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sc_GC.VerticalOffset == sc_GC.ScrollableHeight)
            {
                if (CanLoad)
                {
                    GetGCList(WJ_Order);
                }
            }
        }
        private void sv_ZX_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sv_ZX.VerticalOffset == sv_ZX.ScrollableHeight)
            {
                if (CanLoad)
                {
                    GetZXList(ZX_Order);
                }
            }
        }


        //刷新
        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            switch (pivot.SelectedIndex)
            {
                case 1:
                    PageNum_LZ = 1;
                    GetLZNew(LZ_Order);
                    break;
                case 2:
                    PageNum_WJ = 1;
                    GetWJList(WJ_Order);
                    break;
                case 3:
                    PageNum_GC = 1;
                    GetGCList(GC_Order);
                    break;
                case 4:
                    PageNum_ZX = 1;
                    GetZXList(ZX_Order);
                    break;
                default:
                    break;
            }
        }
        //刷新动态
        private void btn_Refresh_DT_Click(object sender, RoutedEventArgs e)
        {
            GetDT();
        }
        //下拉刷新
        private void PullToRefreshBox_RefreshInvoked(DependencyObject sender, object args)
        {
            PageNum = 1;
            GetDT();
            GetDYHome();
        }

        private void btn_Banner_Ban_Click(object sender, RoutedEventArgs e)
        {

        }

        private void home_flipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (home_flipView.Items.Count == 0 || fvLeft.Items.Count == 0 || fvRight.Items.Count == 0)
            {
                return;
            }
            if (fvLeft.Visibility == Visibility.Collapsed || fvRight.Visibility == Visibility.Collapsed)
            {
                return;
            }
            if (this.home_flipView.SelectedIndex == 0)
            {
                this.fvLeft.SelectedIndex = this.fvLeft.Items.Count - 1;
                this.fvRight.SelectedIndex = 1;
            }
            else if (this.home_flipView.SelectedIndex == 1)
            {
                this.fvLeft.SelectedIndex = 0;
                this.fvRight.SelectedIndex = this.fvRight.Items.Count - 1;
            }
            else if (this.home_flipView.SelectedIndex == this.home_flipView.Items.Count - 1)
            {
                this.fvLeft.SelectedIndex = this.fvLeft.Items.Count - 2;
                this.fvRight.SelectedIndex = 0;
            }
            else if ((this.home_flipView.SelectedIndex < (this.home_flipView.Items.Count - 1)) && this.home_flipView.SelectedIndex > -1)
            {
                this.fvLeft.SelectedIndex = this.home_flipView.SelectedIndex - 1;
                this.fvRight.SelectedIndex = this.home_flipView.SelectedIndex + 1;
            }
            else
            {
                return;
            }
        }
        public ObservableCollection<TagsModel> ZH_Tag;
        public ObservableCollection<TagsModel> MAD_Tag;
        public ObservableCollection<TagsModel> MMD_Tag;
        public ObservableCollection<TagsModel> DP_Tag;
        WebClientClass wc;
        private async void GetTags()
        {
            try
            {
                wc = new WebClientClass();
                string zh_result = await wc.GetResults(new Uri("http://api.bilibili.com/x/tag/hots?rid=27&type=0&jsonp=json"));
                string mad_result = await wc.GetResults(new Uri("http://api.bilibili.com/x/tag/hots?rid=24&type=0&jsonp=json"));
                string mmd_result = await wc.GetResults(new Uri("http://api.bilibili.com/x/tag/hots?rid=25&type=0&jsonp=json"));
                string dp_result = await wc.GetResults(new Uri("http://api.bilibili.com/x/tag/hots?rid=47&type=0&jsonp=json"));

                var zh = JsonConvert.DeserializeObject<TagsModel>(zh_result);
                ZH_Tag = JsonConvert.DeserializeObject<ObservableCollection<TagsModel>>(zh.data.ToString())[0].tags;
                ZH_Tag.Insert(0, new TagsModel() { tag_name = "全部" });


                var mad = JsonConvert.DeserializeObject<TagsModel>(mad_result);
                MAD_Tag = JsonConvert.DeserializeObject<ObservableCollection<TagsModel>>(mad.data.ToString())[0].tags;
                MAD_Tag.Insert(0, new TagsModel() { tag_name = "全部" });


                var mmd = JsonConvert.DeserializeObject<TagsModel>(mmd_result);
                MMD_Tag = JsonConvert.DeserializeObject<List<TagsModel>>(mmd.data.ToString())[0].tags;
                MMD_Tag.Insert(0, new TagsModel() { tag_name = "全部" });



                var dp = JsonConvert.DeserializeObject<TagsModel>(dp_result);
                DP_Tag = JsonConvert.DeserializeObject<ObservableCollection<TagsModel>>(dp.data.ToString())[0].tags;
                DP_Tag.Insert(0, new TagsModel() { tag_name = "全部" });


            }
            catch (Exception)
            {
            }
        }


        private void grid_tag_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (grid_tag.SelectedItem != null)
            {
                PageNum_LZ = 1;
                GetLZNew(LZ_Order);
            }


        }

        private void AppBarToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            switch (pivot.SelectedIndex)
            {
                case 1:
                    grid_tag.Visibility = Visibility.Visible;
                    break;
                case 2:
                    grid_mad_tag.Visibility = Visibility.Visible;
                    break;
                case 3:
                    grid_mmd_tag.Visibility = Visibility.Visible;
                    break;
                case 4:
                    grid_mmd_dp.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }

        }

        private void AppBarToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            switch (pivot.SelectedIndex)
            {
                case 1:
                    grid_tag.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    grid_mad_tag.Visibility = Visibility.Collapsed;
                    break;
                case 3:
                    grid_mmd_tag.Visibility = Visibility.Collapsed;
                    break;
                case 4:
                    grid_mmd_dp.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }

        private void grid_mad_tag_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (grid_mad_tag.SelectedItem != null)
            {
                PageNum_WJ = 1;
                GetWJList(WJ_Order);
            }
        }

        private void grid_mmd_tag_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (grid_mmd_tag.SelectedItem != null)
            {
                PageNum_GC = 1;
                GetGCList(GC_Order);
            }
        }

        private void grid_mmd_dp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (grid_mmd_dp.SelectedItem != null)
            {
                PageNum_ZX = 1;
                GetZXList(ZX_Order);
            }
        }
    }

  


}
