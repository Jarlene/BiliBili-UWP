using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace bilibili2.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class QuanPage : Page
    {
        public delegate void GoBackHandler();
        public event GoBackHandler BackEvent;
        public QuanPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            bg.Color = ((SolidColorBrush)this.Frame.Tag).Color;
            if (e.NavigationMode== NavigationMode.New)
            {
                messShow.Show("功能不完善，仅供测试",3000);
                grid_User.DataContext = null;
                GetMyQuanzi();
                list_DT.Items.Clear();
                Pages = 1;
               
                GetMyQuanziDT();
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

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_info_Click(object sender, RoutedEventArgs e)
        {
            pivot.SelectedIndex = Convert.ToInt32((sender as Button).Tag);
        }
        public void UpdateUI()
        {
            btn_About.Foreground = new SolidColorBrush(new Color() { A = 178, G = 255, B = 255, R = 255 });
            btn_Coment.Foreground = new SolidColorBrush(new Color() { A = 178, G = 255, B = 255, R = 255 });
            btn_info.Foreground = new SolidColorBrush(new Color() { A = 178, G = 255, B = 255, R = 255 });
            btn_About.FontWeight = FontWeights.Normal;
            btn_Coment.FontWeight = FontWeights.Normal;
            btn_info.FontWeight = FontWeights.Normal;
            switch (pivot.SelectedIndex)
            {
                case 0:
                    btn_info.Foreground = new SolidColorBrush(Colors.White);
                    btn_info.FontWeight = FontWeights.Bold;
                    break;
                case 1:
                    btn_Coment.Foreground = new SolidColorBrush(Colors.White);
                    btn_Coment.FontWeight = FontWeights.Bold;
                    if (list_TJQuan.Items.Count==0)
                    {
                        GetTJQuanzi();
                    }
                    if (list_TJ.Items.Count==0)
                    {
                        Pages_TJ = 1;
                        GetQuanziTJ();
                    }
                    break;
                case 2:
                    btn_About.Foreground = new SolidColorBrush(Colors.White);
                    btn_About.FontWeight = FontWeights.Bold;
                    if (grid_User.DataContext==null)
                    {
                        GetUserInfo();
                    }
                    Pages_His = 1;
                    list_His.Items.Clear();
                    His_Loaded = false;
                    GetQuanziHis();
                    break;
                default:
                    break;
            }
        }
        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateUI();
        }

        private void btn_refresh_Atton_Click(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(QuanInfoPage));
        }
        private async void AttenBiliQuan()
        {
            wc = new WebClientClass();
            string url_post = string.Format("http://www.im9.com/api/join.community.do?access_key={0}&actionKey=appkey&appkey={1}&build=418000&community_id={2}&mobi_app=android&platform=android&ts={3}", ApiHelper.access_key, ApiHelper._appKey_Android, 1, ApiHelper.GetTimeSpen);
            url_post += "&sign=" + ApiHelper.GetSign_Android(url_post);
            string result = await wc.PostResults(new Uri(url_post), string.Empty, "http://www.im9.com", "www.im9.com");
        }


        WebClientClass wc;
        private async void GetMyQuanzi()
        {
            try
            {

                wc = new WebClientClass();
                string url = string.Format("http://www.im9.com/api/query.my.community.list.do?access_key={0}&actionKey=appkey&appkey={1}&build=411005&data_type=2&mid={3}&mobi_app=android&page_no=1&page_size=40&platform=wp&ts={2}", ApiHelper.access_key, ApiHelper._appKey, ApiHelper.GetTimeSpen, ApiHelper.GetUserId());
                url += "&sign=" + ApiHelper.GetSign(url);

                string results = await wc.GetResults(new Uri(url));
                MyQuanziModel model = JsonConvert.DeserializeObject<MyQuanziModel>(results);
                MyQuanziModel model1 = JsonConvert.DeserializeObject<MyQuanziModel>(model.data.ToString());
                List<MyQuanziModel> list = JsonConvert.DeserializeObject<List<MyQuanziModel>>(model1.result.ToString());
                list_MyQuan.ItemsSource = list;
               
            }
            catch (Exception)
            {
                messShow.Show("我的关注加载失败", 3000);
            }
            finally
            {
                if (list_MyQuan.Items.Count == 0)
                {
                    txt_NotGz.Visibility = Visibility.Visible;
                    AttenBiliQuan();
                }
                else
                {
                    txt_NotGz.Visibility = Visibility.Collapsed;
                }
            }

        }
        private async void GetTJQuanzi()
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                wc = new WebClientClass();
                string url = string.Format("http://www.im9.com/api/query.community.list.do?access_key={0}&actionKey=appkey&appkey={1}&build=418000&data_type=1&mobi_app=android&page_no=1&page_size=30&platform=android&ts={2}", ApiHelper.access_key, ApiHelper._appKey, ApiHelper.GetTimeSpen);
                url += "&sign=" + ApiHelper.GetSign_Android(url);

                string results = await wc.GetResults(new Uri(url));
                MyQuanziModel model = JsonConvert.DeserializeObject<MyQuanziModel>(results);
                MyQuanziModel model1 = JsonConvert.DeserializeObject<MyQuanziModel>(model.data.ToString());
                List<MyQuanziModel> list = JsonConvert.DeserializeObject<List<MyQuanziModel>>(model1.result.ToString());
                list_TJQuan.ItemsSource = list;
                
            }
            catch (Exception)
            {
                messShow.Show("加载推荐信息失败", 3000);
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }

        }


        int Pages = 1;
        private async void GetMyQuanziDT()
        {
            try
            {
                LoadHome = true;
                pr_Load.Visibility = Visibility.Visible;
                wc = new WebClientClass();
                string url = string.Format("http://www.im9.com/api/query.dynamic.post.list.do?access_key={0}&actionKey=appkey&appkey={1}&build=418000&mobi_app=wp&page_no={3}&page_size=20&platform=android&ts={2}", ApiHelper.access_key,ApiHelper._appKey_Android, ApiHelper.GetTimeSpen, Pages);
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                //string url = "http://www.im9.com/api/query.dynamic.post.list.do?access_key=cac4bbdb9c54df0bc1fbe60727fbddcb&actionKey=appkey&appkey=c1b107428d337928&build=418000&mobi_app=android&page_no=1&page_size=20&platform=android&ts=1464334658000&sign=3426fe25d424a3507f09a0cd5da4dd0a";
                string results = await wc.GetResults(new Uri(url));
                QuanziListModel model = JsonConvert.DeserializeObject<QuanziListModel>(results);
                QuanziListModel model1 = JsonConvert.DeserializeObject<QuanziListModel>(model.data.ToString());
                List<QuanziListModel> list = JsonConvert.DeserializeObject<List<QuanziListModel>>(model1.result.ToString());
                //List<QuanziListModel> lists = new List<QuanziListModel>();
                foreach (var item in list)
                {
                    QuanziListModel m = JsonConvert.DeserializeObject<QuanziListModel>(item.post_info.ToString());
                    m.community_name = JsonConvert.DeserializeObject<QuanziListModel>(item.community_info.ToString()).community_name;
                    m.community_id = JsonConvert.DeserializeObject<QuanziListModel>(item.community_info.ToString()).community_id;
                    //lists.Add(m);
                    list_DT.Items.Add(m);
                }
                Pages++;
                
            }
            catch (Exception)
            {
                messShow.Show("没有数据了...",3000);
            }
            finally
            {
                LoadHome = false;
                if (list_DT.Items.Count == 0)
                {
                    txt_NotDt.Visibility = Visibility.Visible;
                    btn_LoadMore.Visibility = Visibility.Collapsed;
                }
                else
                {
                    txt_NotDt.Visibility = Visibility.Collapsed;
                    btn_LoadMore.Visibility = Visibility.Visible;
                
                }
                pr_Load.Visibility = Visibility.Collapsed;
            }
           
            //
            

        }
        int Pages_TJ = 1;
        private async void GetQuanziTJ()
        {
            try
            {
                LoadTJ = true;
                pr_Load.Visibility = Visibility.Visible;
                wc = new WebClientClass();
                string url = string.Format("http://www.im9.com/api/query.chosen.post.list.do?access_key={0}&actionKey=appkey&appkey={1}&build=418000&mobi_app=android&page_no={2}&page_size=30&platform=android&ts={3}", ApiHelper.access_key, ApiHelper._appKey_Android, Pages_TJ,ApiHelper.GetTimeSpen);
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                //string url = "http://www.im9.com/api/query.dynamic.post.list.do?access_key=cac4bbdb9c54df0bc1fbe60727fbddcb&actionKey=appkey&appkey=c1b107428d337928&build=418000&mobi_app=android&page_no=1&page_size=20&platform=android&ts=1464334658000&sign=3426fe25d424a3507f09a0cd5da4dd0a";
                string results = await wc.GetResults(new Uri(url));
                QuanziListModel model = JsonConvert.DeserializeObject<QuanziListModel>(results);
                QuanziListModel model1 = JsonConvert.DeserializeObject<QuanziListModel>(model.data.ToString());
                List<QuanziListModel> list = JsonConvert.DeserializeObject<List<QuanziListModel>>(model1.result.ToString());
                //List<QuanziListModel> lists = new List<QuanziListModel>();
                foreach (var item in list)
                {
                    QuanziListModel m = JsonConvert.DeserializeObject<QuanziListModel>(item.post_info.ToString());
                    m.community_name = JsonConvert.DeserializeObject<QuanziListModel>(item.community_info.ToString()).community_name;
                    m.community_id = JsonConvert.DeserializeObject<QuanziListModel>(item.community_info.ToString()).community_id;
                    //lists.Add(m);
                    list_TJ.Items.Add(m);
                }
                Pages_TJ++;

            }
            catch (Exception)
            {
                messShow.Show("没有数据了...", 3000);
            }
            finally
            {
                LoadTJ = false;
                pr_Load.Visibility = Visibility.Collapsed;
            }

            //


        }

        private async void GetUserInfo()
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                wc = new WebClientClass();
                string url = string.Format("http://www.im9.com/api/query.community.myinfo.do?access_key={0}&actionKey=appkey&appkey={1}&build=418000&mid={2}&mobi_app=android&platform=android&ts={3}", ApiHelper.access_key, ApiHelper._appKey_Android, ApiHelper.GetUserId(), ApiHelper.GetTimeSpen);
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                //string url = "http://www.im9.com/api/query.dynamic.post.list.do?access_key=cac4bbdb9c54df0bc1fbe60727fbddcb&actionKey=appkey&appkey=c1b107428d337928&build=418000&mobi_app=android&page_no=1&page_size=20&platform=android&ts=1464334658000&sign=3426fe25d424a3507f09a0cd5da4dd0a";
                string results = await wc.GetResults(new Uri(url));
                QuanUserModel model = JsonConvert.DeserializeObject<QuanUserModel>(results);
                QuanUserModel model1 = JsonConvert.DeserializeObject<QuanUserModel>(model.data.ToString());
                grid_User.DataContext = model1;

            }
            catch (Exception)
            {
                messShow.Show("加载个人信息失败", 2000);
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }

        int Pages_His = 1;
        int Pages_MaxHis = 1;
        bool LoadHis = false;
        bool His_Loaded = false;
        private async void GetQuanziHis()
        {
            try
            {
                LoadHis = true;
                pr_Load.Visibility = Visibility.Visible;
                wc = new WebClientClass();
                string url = string.Format("http://www.im9.com/api/notify/query.history.list.do?access_key={0}&actionKey=appkey&appkey={1}&build=418000&mobi_app=android&page_no={2}&page_size=20&platform=android&ts={3}", ApiHelper.access_key, ApiHelper._appKey_Android, Pages_His, ApiHelper.GetTimeSpen);
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                string results = await wc.GetResults(new Uri(url));

                QuanHisModel model = JsonConvert.DeserializeObject<QuanHisModel>(results);
                QuanHisModel model1 = JsonConvert.DeserializeObject<QuanHisModel>(model.data.ToString());
                Pages_MaxHis = model1.total_page;
                List< QuanHisModel > list = JsonConvert.DeserializeObject<List<QuanHisModel>>(model1.result.ToString());
                list.ForEach(x=>list_His.Items.Add(x));
                ////List<QuanziListModel> lists = new List<QuanziListModel>();
                //foreach (var item in list)
                //{
                //    QuanziListModel m = JsonConvert.DeserializeObject<QuanziListModel>(item.post_info.ToString());
                //    m.community_name = JsonConvert.DeserializeObject<QuanziListModel>(item.community_info.ToString()).community_name;
                //    m.community_id = JsonConvert.DeserializeObject<QuanziListModel>(item.community_info.ToString()).community_id;
                //    //lists.Add(m);
                //    list_TJ.Items.Add(m);
                //}
                Pages_His++;
                if (Pages_His> Pages_MaxHis)
                {
                    His_Loaded = true;
                }
                else
                {
                    His_Loaded = false;
                }
            }
            catch (Exception)
            {
                messShow.Show("没有数据了...", 3000);
            }
            finally
            {
                LoadHis = false;
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }


        bool LoadHome = false;
        private void sv_Home_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sv_Home.VerticalOffset == sv_Home.ScrollableHeight)
            {
                if (!LoadHome)
                {
                    GetMyQuanziDT();
                }
            }
        }

        private void list_DT_ItemClick(object sender, ItemClickEventArgs e)
        {
            QuanziListModel model= e.ClickedItem as QuanziListModel;
            this.Frame.Navigate(typeof(QuanInfoPage), model);
        }

        private void list_MyQuan_ItemClick(object sender, ItemClickEventArgs e)
        {
            MyQuanziModel model = e.ClickedItem as MyQuanziModel;
            this.Frame.Navigate(typeof(QuanziPage), model);
        }
        bool LoadTJ = false;
        private void sv_Find_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sv_Find.VerticalOffset == sv_Find.ScrollableHeight)
            {
                if (!LoadTJ)
                {
                    GetQuanziTJ();
                }
            }
        }

        private void btn_User_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserInfoPage));
        }

        private void list_His_ItemClick(object sender, ItemClickEventArgs e)
        {
            QuanziListModel model = new QuanziListModel()
            {
                 community_id= (e.ClickedItem as QuanHisModel).community_id,
                 community_name= (e.ClickedItem as QuanHisModel).community_name,
                 post_id = (e.ClickedItem as QuanHisModel).post_id
            };
            this.Frame.Navigate(typeof(QuanInfoPage), model);

        }

        private void sv_Myself_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (His_Loaded)
            {
                return;
            }
            if (sv_Myself.VerticalOffset == sv_Myself.ScrollableHeight)
            {
                if (!LoadHis)
                {
                    GetQuanziHis();
                }
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int i = Convert.ToInt32(this.ActualWidth / 400);
            if (i>3)
            {
                i = 3;
            }
            bor_Width.Width = this.ActualWidth / i - 22;
           
        }

        private void btn_LoadMore_Click(object sender, RoutedEventArgs e)
        {
            if (!LoadHome)
            {
                GetMyQuanziDT();
            }
        }
    }

    public class MyQuanziModel
    {
        public object data { get; set; }
        public int total_count { get; set; }
        public int total_page { get; set; }

        public object result { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string thumb { get; set; }
        public string desc { get; set; }
    }


    public class QuanziListModel
    {
        public int code { get; set; }
        public object data { get; set; }
        public int total_count { get; set; }
        public int total_page { get; set; }

        public object result { get; set; }
        public object post_info { get; set; }
        public object post_list { get; set; }
        public object community_info { get; set; }
        public int community_id { get; set; }
        public string community_name { get; set; }

        public int post_id { get; set; }
        public string post_url { get; set; }
        public string post_title { get; set; }
        public string post_summary { get; set; }
        public int reply_count { get; set; }
        public string author_mid { get; set; }
        public string author_name { get; set; }
        public string author_avatar { get; set; }
        public int is_top { get; set; }

        public long last_reply_time { get; set; }
        public string time
        {
                get {
                    DateTime dtStart = new DateTime(1970, 1, 1);
                    long lTime = long.Parse(last_reply_time + "0000");
                    //long lTime = long.Parse(textBox1.Text);
                    TimeSpan toNow = new TimeSpan(lTime);
                    DateTime dt = dtStart.Add(toNow).ToLocalTime();
                    TimeSpan span = DateTime.Now - dt;
                    if (span.TotalDays > 7)
                    {
                        return dt.ToString("MM-dd");
                    }
                    else
                    if (span.TotalDays > 1)
                    {
                        return string.Format("{0}天前", (int)Math.Floor(span.TotalDays));
                    }
                    else
                    if (span.TotalHours > 1)
                    {
                        return string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
                    }
                    else
                    if (span.TotalMinutes > 1)
                    {
                        return string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
                    }
                    else
                    if (span.TotalSeconds >= 1)
                    {
                        return string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
                    }
                    else
                    {
                        return "1秒前";
                    }
            }
        }
        public long post_time { get; set; }
        public List<ImgModel> post_image_list { get; set; }
        public List<ImgModel> Post_image_list
        {
            get
            {
                List<ImgModel> ls = new List<ImgModel>();
                if (post_image_list!=null&& post_image_list.Count>3)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        ls.Add(post_image_list[i]);
                    }
                    return ls;
                }
                else
                {
                    return post_image_list;
                }
            }
        }
        //public long last_reply_time { get; set; }
    }
    public class ImgModel
    {
        public int id { get; set; }
        public string img_suffix { get; set; }//图片格式
        public string image_id { get; set; }
        public string image_url { get; set; }
        public double width { get; set; }
        public double height { get; set; }
    }


    public class QuanUserModel
    {
        public int id { get; set; }
        public object data { get; set; }
        public string username { get; set; }
        public string user_avatar { get; set; }
        public string sign { get; set; }
        public int post_count { get; set; }
        public int reply_count { get; set; }
        public int collect_count { get; set; }
    }

    public class QuanHisModel
    {
        public int code { get; set; }
        public object data { get; set; }
        public object result { get; set; }
        public int total_page { get; set; }
        public int total_count { get; set; }

        public string community_name { get; set; }
        public string post_title { get; set; }
        public int post_id { get; set; }
        public int community_id { get; set; }
        public long visit_time { get; set; }
        public string Visit_time
        {
            get
            {
                DateTime dtStart = new DateTime(1970, 1, 1);
                long lTime = long.Parse(visit_time + "0000");
                //long lTime = long.Parse(textBox1.Text);
                TimeSpan toNow = new TimeSpan(lTime);
                DateTime dt = dtStart.Add(toNow).ToLocalTime();
                TimeSpan span = DateTime.Now - dt;
                if (span.TotalDays > 7)
                {
                    return dt.ToString("MM-dd");
                }
                else
                if (span.TotalDays > 1)
                {
                    return string.Format("{0}天前", (int)Math.Floor(span.TotalDays));
                }
                else
                if (span.TotalHours > 1)
                {
                    return string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
                }
                else
                if (span.TotalMinutes > 1)
                {
                    return string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
                }
                else
                if (span.TotalSeconds >= 1)
                {
                    return string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
                }
                else
                {
                    return "1秒前";
                }
            }
        }
        public long read_time { get; set; }
    }

}

