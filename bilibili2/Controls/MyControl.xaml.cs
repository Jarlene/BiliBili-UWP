using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace bilibili2
{
    public sealed partial class MyControl : UserControl
    {
        public delegate void PlayHandler(string aid);
        public event PlayHandler PlayEvent;
        public event PlayHandler ErrorEvent;
        public event PlayHandler PartEvent;
        public MyControl()
        {
            this.InitializeComponent();
        }


        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualWidth<1400)
            {
                pagesize = 8;
            }
            if (this.ActualWidth <= 500)
            {
                ViewBox_num.Width = ActualWidth / 2 - 15;
                ViewBox2_num.Width = ActualWidth / 2 - 15;
                double d = ((ViewBox2_num.Width + 12) / 1.08) * 2;
                home_GridView_FJ.Height = d;
                home_GridView_DH.Height = d;
                home_GridView_YYWD.Height = d;
                home_GridView_WD.Height = d;
                home_GridView_YX.Height = d;
                home_GridView_KJ.Height = d;
                home_GridView_GC.Height = d;
                home_GridView_YL.Height = d;
                home_GridView_DY.Height = d;
                home_GridView_DSJ.Height = d;
                home_GridView_SS.Height = d;
                home_GridView_SH.Height = d;
                home_GridView_Hot.Height = d;
                //PageCount = 4;
            }
            else
            {
                if (this.ActualWidth <= 800)
                {
                    ViewBox_num.Width = ActualWidth / 3 - 13;
                    ViewBox2_num.Width = ActualWidth / 3 - 13;
                    double d = ((ViewBox2_num.Width + 12) / 1.08) * 2;
                    home_GridView_FJ.Height = d;
                    home_GridView_DH.Height = d;
                    home_GridView_YYWD.Height = d;
                    home_GridView_WD.Height = d;
                    home_GridView_YX.Height = d;
                    home_GridView_KJ.Height = d;
                    home_GridView_GC.Height = d;
                    home_GridView_YL.Height = d;
                    home_GridView_DY.Height = d;
                    home_GridView_DSJ.Height = d;
                    home_GridView_SS.Height = d;
                    home_GridView_SH.Height = d;
                    home_GridView_Hot.Height = d;
                }
                else
                {
                    int i = Convert.ToInt32(ActualWidth / 200);
                    ViewBox_num.Width = ActualWidth / i - 13;
                    ViewBox2_num.Width = ActualWidth / i - 13;
                    double d = ((ViewBox2_num.Width + 12) / 1.08);
                    home_GridView_FJ.Height = d;
                    home_GridView_DH.Height = d;
                    home_GridView_YYWD.Height = d;
                    home_GridView_WD.Height = d;
                    home_GridView_YX.Height = d;
                    home_GridView_KJ.Height = d;
                    home_GridView_GC.Height = d;
                    home_GridView_YL.Height = d;
                    home_GridView_DY.Height = d;
                    home_GridView_DSJ.Height = d;
                    home_GridView_SS.Height = d;
                    home_GridView_SH.Height = d;
                    home_GridView_Hot.Height = d;
                }
            }
        }
        public async void SetListView(string results, GridView ls)
        {
            try
            {
                InfoModel model = JsonConvert.DeserializeObject<InfoModel>(results);
                List<InfoModel> ban = JsonConvert.DeserializeObject<List<InfoModel>>(model.list.ToString());
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { ls.ItemsSource = ban; });
            
              
            }
            catch (Exception)
            {
            }
        }

        public async void SetListView(string results, GridView ls,bool isHot)
        {
            try
            {
                List<InfoModel> ban = new List<InfoModel>();
                    HomeHotModel model = JsonConvert.DeserializeObject<HomeHotModel>(results);
                    foreach (var item in model.data[0].body)
                    {
                        ban.Add(new InfoModel()
                        {
                            aid = item.param,
                            pic = item.cover,
                            play = item.play,
                            video_review = item.danmaku,
                            title = item.title,
                        });
                    }
                    if (pagesize == 16)
                    {
                        try
                        {
                            wc = new WebClientClass();
                            // http://app.bilibili.com/x/v2/show/change?access_key=bb9e0f8a509a1d8eb53dc744432f1e8d&actionKey=appkey&appkey=5fd5a7d8bfd9b0e6&build=10180&channel=appstore&device=pad&mobi_app=ipad&plat=2&platform=ios&rand=0&sign=344b13c26840254d674cd4634be2d621&ts=1475599666
                            string url = string.Format("http://app.bilibili.com/x/v2/show?access_key={0}&actionKey=appkey&appkey={1}&platform=wp&ts={2}&plat=2&device=pad&mobi_app=ipad&build=10180&warm=1", ApiHelper.access_key, ApiHelper._appKey, ApiHelper.GetTimeSpen);
                            url += "&sign=" + ApiHelper.GetSign(url);
                            string re = await wc.GetResults(new Uri(url));
                            HomeHotModel mo = JsonConvert.DeserializeObject<HomeHotModel>(re);
                            foreach (var item in mo.data[0].body)
                            {
                                ban.Add(new InfoModel()
                                {
                                    aid = item.param,
                                    pic = item.cover,
                                    play = item.play,
                                    video_review = item.danmaku,
                                    title = item.title,
                                });
                            }
                        }
                        catch (Exception)
                        {
                        }

                    }

                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    ls.ItemsSource = ban;
                });
                //for (int i = 0; i <12; i++)
                //{
                //    ls.Items.Add(ban[i]);
                //}

            }
            catch (Exception)
            {
                //ErrorEvent(ex.Message);
            }
        }

        int FJ = 1;
        int DH = 1;
        int YY = 1;
        int WD = 1;
        int YX = 1;
        int YL = 1;
        int KJ = 1;
        int GC = 1;
        int DY = 1;
        int DSJ = 1;
        int SS = 1;
        int SH = 1;
        int pagesize = 16;
        WebClientClass wc = new WebClientClass();
        public async void SetHomeInfo()
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                await Task.Run(async () =>
                {
                    string url = string.Format("http://app.bilibili.com/x/v2/show?access_key={0}&actionKey=appkey&appkey={1}&platform=wp&ts={2}&plat=2&device=pad&mobi_app=ipad&build=10180&warm=1", ApiHelper.access_key, ApiHelper._appKey, ApiHelper.GetTimeSpen);
                    url += "&sign=" + ApiHelper.GetSign(url);
                    string HOT = await wc.GetResults(new Uri(url));
                    SetListView(HOT, home_GridView_Hot, true);

                    string fj = await wc.GetResults(new Uri("http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=13&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999)));
                    SetListView(fj, home_GridView_FJ);
                    // string banner = await wc.GetResults(new Uri("http://www.bilibili.com/index/slideshow.json"));
                    string dh = await wc.GetResults(new Uri("http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=1&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999)));
                    SetListView(dh, home_GridView_DH);

                    string yy = await wc.GetResults(new Uri("http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=3&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999)));
                    SetListView(yy, home_GridView_YYWD);
                    string wd = await wc.GetResults(new Uri("http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=20&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999)));
                    SetListView(wd, home_GridView_WD);
                    string yx = await wc.GetResults(new Uri("http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=4&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999)));
                    SetListView(yx, home_GridView_YX);
                    string kj = await wc.GetResults(new Uri("http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=36&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999)));
                    SetListView(kj, home_GridView_KJ);
                    string SH = await wc.GetResults(new Uri("http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=5&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999)));
                    SetListView(SH, home_GridView_SH);
                    string YL = await wc.GetResults(new Uri("http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=160&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999)));
                    SetListView(YL, home_GridView_YL);
                    string GC = await wc.GetResults(new Uri("http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=119&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999)));
                    SetListView(GC, home_GridView_GC);
                    string DY = await wc.GetResults(new Uri("http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=23&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999)));
                    SetListView(DY, home_GridView_DY);
                    string DSJ = await wc.GetResults(new Uri("http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=11&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999)));
                    SetListView(DSJ, home_GridView_DSJ);
                    string SS = await wc.GetResults(new Uri("http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=155&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999)));
                    SetListView(SS, home_GridView_SS);

                });
               

            }
            catch (Exception ex)
            {
                ErrorEvent(ex.Message);

            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }

        private void items_listview_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame frame = Window.Current.Content as Frame;
            frame.Navigate(typeof(VideoInfoPage));
        }
        private void home_GridView_FJ_ItemClick(object sender, ItemClickEventArgs e)
        {
            PlayEvent((e.ClickedItem as InfoModel).aid);
        }

        private async void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GridView gridview = null;
                string uri = string.Empty;
                switch ((sender as HyperlinkButton).Tag.ToString())
                {
                    case "FJ":
                        gridview = home_GridView_FJ;
                        switch (FJ)
                        {
                            case 0:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=13&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999);
                                FJ++;
                                break;
                            case 1:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=13&page=1&pagesize=" + pagesize + "&order=review&ver=2&rnd=" + new Random().Next(1000, 9999);
                                FJ++;
                                break;
                            case 2:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=13&page=1&pagesize=" + pagesize + "&order=default&ver=2&rnd=" + new Random().Next(1000, 9999);
                                FJ = 0;
                                break;
                            default:
                                break;
                        }
                        break;
                    case "DH":
                        gridview = home_GridView_DH;
                        switch (DH)
                        {
                            case 0:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=1&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999);
                                DH++;
                                break;
                            case 1:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=1&page=1&pagesize=" + pagesize + "&order=review&ver=2&rnd=" + new Random().Next(1000, 9999);
                                DH++;
                                break;
                            case 2:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=1&page=1&pagesize=" + pagesize + "&order=default&ver=2&rnd=" + new Random().Next(1000, 9999);
                                DH = 0;
                                break;
                            default:
                                break;
                        }
                        break;
                    case "YY":
                        gridview = home_GridView_YYWD;
                        switch (YY)
                        {
                            case 0:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=3&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999);
                                YY++;
                                break;
                            case 1:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=3&page=1&pagesize=" + pagesize + "&order=review&ver=2&rnd=" + new Random().Next(1000, 9999);
                                YY++;
                                break;
                            case 2:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=3&page=1&pagesize=" + pagesize + "&order=default&ver=2&rnd=" + new Random().Next(1000, 9999);
                                YY = 0;
                                break;
                            default:
                                break;
                        }
                        break;
                    case "WD":
                        gridview = home_GridView_WD;
                        switch (WD)
                        {
                            case 0:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=20&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999);
                                WD++;
                                break;
                            case 1:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=20&page=1&pagesize=" + pagesize + "&order=review&ver=2&rnd=" + new Random().Next(1000, 9999);
                                WD++;
                                break;
                            case 2:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=20&page=1&pagesize=" + pagesize + "&order=default&ver=2&rnd=" + new Random().Next(1000, 9999);
                                WD = 0;
                                break;
                            default:
                                break;
                        }
                        break;
                    case "YX":
                        gridview = home_GridView_YX;
                        switch (YX)
                        {
                            case 0:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=4&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999);
                                YX++;
                                break;
                            case 1:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=4&page=1&pagesize=" + pagesize + "&order=review&ver=2&rnd=" + new Random().Next(1000, 9999);
                                YX++;
                                break;
                            case 2:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=4&page=1&pagesize=" + pagesize + "&order=default&ver=2&rnd=" + new Random().Next(1000, 9999);
                                YX = 0;
                                break;
                            default:
                                break;
                        }
                        break;
                    case "KJ":
                        gridview = home_GridView_KJ;
                        switch (KJ)
                        {
                            case 0:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=36&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999);
                                KJ++;
                                break;
                            case 1:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=36&page=1&pagesize=" + pagesize + "&order=review&ver=2&rnd=" + new Random().Next(1000, 9999);
                                KJ++;
                                break;
                            case 2:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=36&page=1&pagesize=" + pagesize + "&order=default&ver=2&rnd=" + new Random().Next(1000, 9999);
                                KJ = 0;
                                break;
                            default:
                                break;
                        }
                        break;
                    case "YL":
                        gridview = home_GridView_YL;
                        switch (YL)
                        {
                            case 0:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=5&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999);
                                YL++;
                                break;
                            case 1:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=5&page=1&pagesize=" + pagesize + "&order=review&ver=2&rnd=" + new Random().Next(1000, 9999);
                                YL++;
                                break;
                            case 2:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=5&page=1&pagesize=" + pagesize + "&order=default&ver=2&rnd=" + new Random().Next(1000, 9999);
                                YL = 0;
                                break;
                            default:
                                break;
                        }
                        break;
                    case "GC":
                        gridview = home_GridView_GC;
                        switch (GC)
                        {
                            case 0:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=119&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999);
                                GC++;
                                break;
                            case 1:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=119&page=1&pagesize=" + pagesize + "&order=review&ver=2&rnd=" + new Random().Next(1000, 9999);
                                GC++;
                                break;
                            case 2:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=119&page=1&pagesize=" + pagesize + "&order=default&ver=2&rnd=" + new Random().Next(1000, 9999);
                                GC = 0;
                                break;
                            default:
                                break;
                        }
                        break;
                    case "DY":
                        gridview = home_GridView_DY;
                        switch (DY)
                        {
                            case 0:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=23&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999);
                                DY++;
                                break;
                            case 1:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=23&page=1&pagesize=" + pagesize + "&order=review&ver=2&rnd=" + new Random().Next(1000, 9999);
                                DY++;
                                break;
                            case 2:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=23&page=1&pagesize=" + pagesize + "&order=default&ver=2&rnd=" + new Random().Next(1000, 9999);
                                DY = 0;
                                break;
                            default:
                                break;
                        }
                        break;
                    case "DSJ":
                        gridview = home_GridView_DSJ;
                        switch (DSJ)
                        {
                            case 0:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=11&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999);
                                DSJ++;
                                break;
                            case 1:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=11&page=1&pagesize=" + pagesize + "&order=review&ver=2&rnd=" + new Random().Next(1000, 9999);
                                DSJ++;
                                break;
                            case 2:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=11&page=1&pagesize=" + pagesize + "&order=default&ver=2&rnd=" + new Random().Next(1000, 9999);
                                DSJ = 0;
                                break;
                            default:
                                break;
                        }

                        break;
                    case "SS":
                        gridview = home_GridView_SS;
                        switch (SS)
                        {
                            case 0:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=155&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999);
                                SS++;
                                break;
                            case 1:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=155&page=1&pagesize=" + pagesize + "&order=review&ver=2&rnd=" + new Random().Next(1000, 9999);
                                SS++;
                                break;
                            case 2:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=155&page=1&pagesize=" + pagesize + "&order=default&ver=2&rnd=" + new Random().Next(1000, 9999);
                                SS = 0;
                                break;
                            default:
                                break;
                        }
                        //uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=155&page=1&pagesize=" + 20 + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999);
                        break;
                    case "SH":
                        gridview = home_GridView_SH;
                        switch (SH)
                        {
                            case 0:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=160&page=1&pagesize=" + pagesize + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999);
                                SS++;
                                break;
                            case 1:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=160&page=1&pagesize=" + pagesize + "&order=review&ver=2&rnd=" + new Random().Next(1000, 9999);
                                SS++;
                                break;
                            case 2:
                                uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=160&page=1&pagesize=" + pagesize + "&order=default&ver=2&rnd=" + new Random().Next(1000, 9999);
                                SS = 0;
                                break;
                            default:
                                break;
                        }
                        //uri = "http://api.bilibili.com/list?type=json&appkey=422fd9d7289a1dd9&tid=155&page=1&pagesize=" + 20 + "&order=hot&ver=2&rnd=" + new Random().Next(1000, 9999);
                        break;
                    case "HOT":
                        string url = string.Format("http://app.bilibili.com/x/v2/show?access_key={0}&actionKey=appkey&appkey={1}&platform=wp&ts={2}&plat=2&device=pad&mobi_app=ipad&build=10180&warm=1", ApiHelper.access_key, ApiHelper._appKey, ApiHelper.GetTimeSpen);
                        url += "&sign=" + ApiHelper.GetSign(url);
                        string HOT = await wc.GetResults(new Uri(url));
                        SetListView(HOT, home_GridView_Hot, true);
                        break;
                    default:
                        break;
                }
                string fj = await wc.GetResults(new Uri(uri));
                SetListView(fj, gridview);
            }
            catch (Exception)
            {
            }
            
            //if (gridview.Items==null|| gridview.Items.Count==0)
            //{
               
            //}
            //else
            //{
            //    var lists = (from a in gridview.Items select a).ToArray();
            //    gridview.Items.Clear();
            //    for (int i = lists.Length - 1; i >= 0; i--)
            //    {
            //        gridview.Items.Add(lists[i]);
            //    }
            //    lists = null;
            //}
           
        }

        private void Btn_FJ_More_Click(object sender, RoutedEventArgs e)
        {
            PartEvent((sender as HyperlinkButton).Tag.ToString());
        }
    }
}
