using bilibili2.Class;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace bilibili2.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UserInfoPage : Page
    {
        public UserInfoPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;

        }


        public delegate void GoBackHandler();
        public event GoBackHandler BackEvent;
        public event GoBackHandler ExitEvent;
        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
            else
            {
                canB = false;
                BackEvent();
            }
        }
        string Uid = string.Empty;
        bool canB = false;
        //bool Back = false;
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            bg.Color = ((SolidColorBrush)this.Frame.Tag).Color;
            if (e.NavigationMode == NavigationMode.New|| canB)
            {

                Uid = "";
                pivot.SelectedIndex = 0;
                btn_Attention.Visibility = Visibility.Collapsed;
                btn_CannelAttention.Visibility = Visibility.Collapsed;
                btn_Message.Visibility = Visibility.Collapsed;
                user_GridView_FovBox.ItemsSource = null;

                getPage = 1;
                list_ASubit.Items.Clear();
                if (e.Parameter == null)
                {

                    Uid = UserClass.Uid;
                    btn_Attention.Visibility = Visibility.Collapsed;
                    btn_More.Visibility = Visibility.Visible;
                    btn_Edit.Visibility = Visibility.Visible;
                    fav.Visibility = Visibility.Visible;
                    user_GridView_FovBox.Visibility = Visibility.Visible;
                    UserClass getUser = new UserClass();
                    pr_Load.Visibility = Visibility.Visible;
                    await UserInfo();
                    //await GetSubInfo();
                    await GetDt();

                    user_GridView_FovBox.ItemsSource = await getUser.GetUserFovBox();
                    pr_Load.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Uid = e.Parameter as string;
                    btn_Attention.Visibility = Visibility.Visible;
                    btn_Message.Visibility = Visibility.Visible;
                    btn_More.Visibility = Visibility.Collapsed;
                    btn_Edit.Visibility = Visibility.Collapsed;
                    fav.Visibility = Visibility.Collapsed;
                    user_GridView_FovBox.Visibility = Visibility.Collapsed;
                    pr_Load.Visibility = Visibility.Visible;
                    await UserInfo();
                    //await GetSubInfo();
                    await GetDt();
                    if (UserClass.AttentionList.Contains(Uid))
                    {
                        btn_Attention.Visibility = Visibility.Collapsed;
                        btn_CannelAttention.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        btn_Attention.Visibility = Visibility.Visible;
                        btn_CannelAttention.Visibility = Visibility.Collapsed;
                    }
                    pr_Load.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                canB = true;

            }
        }

        //private async Task GetSubInfo()
        //{
        //    try
        //    {
        //        user_GridView_Submit.ItemsSource = null;
        //        WebClientClass wc = new WebClientClass();
        //        txt_Load.IsEnabled = false;
        //        txt_Load.Content = "加载中...";
        //        string results = await wc.GetResults(new Uri("http://space.bilibili.com/ajax/member/getSubmitVideos?mid=" + Uid + "&pagesize=20&page=1"));
        //        //一层
        //        GetUserSubmit model1 = JsonConvert.DeserializeObject<GetUserSubmit>(results);
        //        //二层
        //        GetUserSubmit model2 = JsonConvert.DeserializeObject<GetUserSubmit>(model1.data.ToString());
        //        //三层
        //        List<GetUserSubmit> lsModel = JsonConvert.DeserializeObject<List<GetUserSubmit>>(model2.vlist.ToString());
        //        user_GridView_Submit.ItemsSource = lsModel;
        //    }
        //    catch (Exception)
        //    {
        //    }
        //    finally
        //    {
        //        if (user_GridView_Submit.Items.Count == 0)
        //        {
        //            DT_SUB.Visibility =  Visibility.Visible;
        //        }
        //        else
        //        {
        //            DT_SUB.Visibility = Visibility.Collapsed;
        //        }

        //    }
        //}
        public async Task<GetLoginInfoModel> GetUserInfo()
        {
            try
            {
                wc = new WebClientClass();
                //http://account.bilibili.com/api/myinfo/v2?access_key={0}&appkey={1}&platform=ios&ts={2}&type=json
                string url = string.Format("http://account.bilibili.com/api/myinfo/v2?access_key={0}&appkey={1}&platform=ios&ts={2}&type=json", ApiHelper.access_key, ApiHelper._appKey,ApiHelper.GetTimeSpen);
                url += "&sign=" + ApiHelper.GetSign(url);
                string results = await wc.GetResults(new Uri(url));

                GetLoginInfoModel model = JsonConvert.DeserializeObject<GetLoginInfoModel>(results);
                UserClass.AttentionList = model.attentions;
                return model;
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task<GetLoginInfoModel> GetUserInfo(string uid)
        {

            try
            {
                wc = new WebClientClass();
                //
                string url = string.Format("http://app.bilibili.com/x/v2/space?access_key={0}&appkey={1}&platform=wp&ps=10&ts={2}000&vmid={3}&build=411005&mobi_app=android", ApiHelper.access_key, ApiHelper._appKey, ApiHelper.GetTimeSpen,uid);
                //string url = string.Format("http://api.bilibili.com/userinfo?access_key={0}&appkey={1}&mid={2}", ApiHelper.access_key, ApiHelper._appKey, uid);
                url += "&sign=" + ApiHelper.GetSign(url);
                string results = await wc.GetResults(new Uri(url));

                GetLoginInfoModel model = JsonConvert.DeserializeObject<GetLoginInfoModel>(results);
               
                return model.data.card;
            }
            catch (Exception)
            {
                return null;
            }
        }



        private async Task UserInfo()
        {
            try
            {
                GetLoginInfoModel model = new GetLoginInfoModel();
                if (Uid.Length != 0)
                {
                    model = await GetUserInfo(Uid);
                    // http://i0.hdslb.com/
                }
                else
                {
                    model = await GetUserInfo();
                }
                if (model.approve)
                {
                    if (model.official_verify.type != -1)
                    {
                        txt_RZ.Text =  model.official_verify.desc;
                    }
                    txt_RZ.Visibility = Visibility.Visible;
                }
                else
                {
                    txt_RZ.Visibility = Visibility.Collapsed;
                }
                grid_Info.DataContext = model;
                if (model.level_info.current_level <= 3)
                {
                    bor_Level.Background = new SolidColorBrush(new Windows.UI.Color() { R = 48, G = 161, B = 255, A = 200 });
                }
                else
                {
                    if (model.current_level <= 6)
                    {
                        bor_Level.Background = new SolidColorBrush(new Windows.UI.Color() { R = 255, G = 48, B = 48, A = 200 });
                    }
                    else
                    {
                        bor_Level.Background = new SolidColorBrush(new Windows.UI.Color() { R = 255, G = 199, B = 45, A = 200 });
                    }
                }
                if (model.vip.vipType!=0)
                {
                    bor_Vip.Visibility = Visibility.Visible;
                }
                else
                {
                    bor_Vip.Visibility = Visibility.Collapsed;
                }
                //if (model!=null&&model.toutu.Length!=0)
                //{
                //    img_bg.ImageSource = new BitmapImage(new Uri("http://i0.hdslb.com/"+model.toutu));
                //}
                SettingHelper settings = new SettingHelper();
                if (settings.SettingContains("UserWebTT"))
                {
                    if ((bool)settings.GetSettingValue("UserWebTT"))
                    {
                        GetBg();
                    }
                    else
                    {
                        img_bg.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/bg1.png"));
                    }
                }
                else
                {
                    settings.SetSettingValue("UserWebTT", false);
                    img_bg.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/bg1.png"));
                }


                page = 1;
                MaxPage = 0;
                list_AUser.Items.Clear();
                GetUserAttention(page);
            }
            catch (Exception)
            {
                messShow.Show("读取用户信息失败",2000);
               // throw;
            }
           

        }
        WebClientClass wc;
        private async void GetBg()
        {
            try
            {
                wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://space.bilibili.com/ajax/topphoto/getlist?mid="+Uid));
                BGModel bgm = JsonConvert.DeserializeObject<BGModel>(results);
                if (bgm.status)
                {
                    var fist = bgm.data.First(x => x.is_disable == 0);
                    if (fist != null && fist.l_img.Length != 0)
                    {
                        img_bg.ImageSource = new BitmapImage(new Uri("http://i0.hdslb.com/" + fist.l_img));
                    }
                }
            }
            catch (Exception)
            {
                //throw;
            }
        }
        public class BGModel
        {
            public bool status { get; set; }
            public List<BGModel> data { get; set; }
            public int id { get; set; }
            public string product_name { get; set; }
            public string l_img { get; set; }
            public int is_disable { get; set; }//0为启用

            public string s_img { get; set; }
        }

        private void ToggleMenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private async Task GetDt()
        {
            UserClass getLogin = new UserClass();
            try
            {
                DT_0.Visibility = Visibility.Collapsed;
                if (Uid.Length != 0)
                {
                    user_GridView_Bangumi.ItemsSource = await getLogin.GetUserBangumi(Uid);
                }
                else
                {
                    user_GridView_Bangumi.ItemsSource = await getLogin.GetUserBangumi();
                }

                if (user_GridView_Bangumi.Items.Count == 0)
                {
                    DT_0.Visibility = Visibility.Visible;
                }

            }
            catch (Exception)
            {

                await new MessageDialog("读取动态失败").ShowAsync();
            }
        }

        private void user_GridView_Bangumi_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(BanInfoPage), (e.ClickedItem as GetUserBangumi).season_id);
        }

        private void user_GridView_FovBox_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(FavPage), (e.ClickedItem as GetUserFovBox).fav_box);
        }

        private void btn_AttBangumi_Click(object sender, RoutedEventArgs e)
        {
            if (Uid.Length != 0)
            {
                this.Frame.Navigate(typeof(UserBangumiPage), Uid);
            }
            else
            {
                this.Frame.Navigate(typeof(UserBangumiPage), UserClass.Uid);
            }
        }

        bool IsLoading = false;
        private void sv_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sv.VerticalOffset == sv.ScrollableHeight)
            {
                if (page <= MaxPage && !IsLoading)
                {
                    GetUserAttention(page);
                }
            }


        }

        int page = 1;
        int MaxPage = 0;
        public async void GetUserAttention(int pageNum)
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                IsLoading = true;
                WebClientClass wc = new WebClientClass();
                string mid = "";
                if (Uid.Length == 0)
                {
                    mid = UserClass.Uid;
                }
                else
                {
                    mid = Uid;
                }
                string results = await wc.GetResults(new Uri("http://space.bilibili.com/ajax/friend/GetAttentionList?mid=" + mid + "&pagesize=30&page=" + pageNum));
                //一层
                GetUserFovBox model1 = JsonConvert.DeserializeObject<GetUserFovBox>(results);
                if (model1.status)
                {
                    //二层
                    GetUserAttention model2 = JsonConvert.DeserializeObject<GetUserAttention>(model1.data.ToString());
                    MaxPage = model2.pages;
                    //三层
                    List<GetUserAttention> lsModel = JsonConvert.DeserializeObject<List<GetUserAttention>>(model2.list.ToString());
                    foreach (GetUserAttention item in lsModel)
                    {
                        list_AUser.Items.Add(item);
                    }
                    page++;
                }
                else
                {
                    messShow.Show("读取关注失败！",2000);
                }
            }
            catch (Exception)
            {
                if (list_AUser.Items.Count==0)
                {
                    messShow.Show("没有关注任何人", 2000);
                }
                else
                {
                    messShow.Show("读取关注失败！", 2000);
                }
                //await new MessageDialog("读取关注失败！\r\n" + ex.Message).ShowAsync();
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
                IsLoading = false;
            }
        }

        private void list_AUser_ItemClick(object sender, ItemClickEventArgs e)
        {
            canB = true;
            this.Frame.Navigate(typeof(UserInfoPage), ((GetUserAttention)e.ClickedItem).fid);

        }

        private void Ban_btn_User_Click(object sender, RoutedEventArgs e)
        {
            //grid_AUser.Visibility = Visibility.Visible;
            //grid_ASubit.Visibility = Visibility.Collapsed;
            //grid_ACoin.Visibility = Visibility.Collapsed;
            //sp.IsPaneOpen = true;
            page = 1;
            MaxPage = 0;
            list_AUser.Items.Clear();
            GetUserAttention(page);
        }

        private async void Ban_btn_Sub_Click(object sender, RoutedEventArgs e)
        {

            getPage = 1;
            list_ASubit.Items.Clear();
            await GetSubInfo(Uid);
        }

        private int getPage = 1;
        private async Task GetSubInfo( string uid)
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                WebClientClass wc = new WebClientClass();
                btn_More_Video.Visibility = Visibility.Collapsed;
                string results = await wc.GetResults(new Uri("http://space.bilibili.com/ajax/member/getSubmitVideos?mid=" + uid + "&pagesize=30" + "&page=" + getPage));
                //一层
                GetUserSubmit model1 = JsonConvert.DeserializeObject<GetUserSubmit>(results);
                //二层
                GetUserSubmit model2 = JsonConvert.DeserializeObject<GetUserSubmit>(model1.data.ToString());
                //三层
                List<GetUserSubmit> lsModel = JsonConvert.DeserializeObject<List<GetUserSubmit>>(model2.vlist.ToString());
                foreach (GetUserSubmit item in lsModel)
                {
                    list_ASubit.Items.Add(item);
                }
                getPage++;
                if (model2.pages < getPage)
                {
                    messShow.Show("加载完了", 3000);
                }

            }
            catch (Exception)
            {
                messShow.Show("加载投稿失败", 3000);
            }
            finally
            {
                btn_More_Video.Visibility = Visibility.Visible;
                pr_Load.Visibility = Visibility.Collapsed;
                if (list_ASubit.Items.Count == 0)
                {
                    messShow.Show("没有投稿",3000);
                    btn_More_Video.Visibility = Visibility.Collapsed;
                }
               
            }
        }
        bool subLoading = false;
        private async void sv1_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sv1.VerticalOffset == sv1.ScrollableHeight)
            {
                if (!subLoading)
                {
                    subLoading = true;
                    await GetSubInfo( Uid);
                    subLoading = false;
                }
            }
        }

        private void list_ASubit_ItemClick(object sender, ItemClickEventArgs e)
        {
            canB = false;
            this.Frame.Navigate(typeof(VideoInfoPage), (e.ClickedItem as GetUserSubmit).aid);
        }

        private async void GetPutCoin()
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                WebClientClass wc = new WebClientClass();
                txt_Load_Coin.IsEnabled = false;
                txt_Load_Coin.Content = "加载中...";
                string results = await wc.GetResults(new Uri("http://space.bilibili.com/ajax/member/getCoinVideos?mid=" + Uid + "&pagesize=100" + page + "&rnd=" + new Random().Next(1, 9999)));
                //一层
                GetUserSubmit model1 = JsonConvert.DeserializeObject<GetUserSubmit>(results);
                //二层
                GetUserSubmit model2 = JsonConvert.DeserializeObject<GetUserSubmit>(model1.data.ToString());
                //三层
                List<GetUserSubmit> lsModel = JsonConvert.DeserializeObject<List<GetUserSubmit>>(model2.list.ToString());
                list_ACoin.ItemsSource = lsModel;
                txt_Load_Coin.IsEnabled = false;
                txt_Load_Coin.Content = "加载完了...";
            }
            catch (Exception)
            {
            }
            finally
            {
                if (list_ACoin.Items.Count == 0)
                {
                    txt_Load_Coin.IsEnabled = false;
                    txt_Load_Coin.Content = "没有投币...";
                }
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }

        private async void btn_Attention_Click(object sender, RoutedEventArgs e)
        {
            UserClass getUser = new UserClass();
            if (getUser.IsLogin())
            {
                try
                {
                    Uri ReUri = new Uri("http://space.bilibili.com/ajax/friend/AddAttention");
                    HttpClient hc = new HttpClient();
                    hc.DefaultRequestHeaders.Referer = new Uri("http://space.bilibili.com/");
                    var response = await hc.PostAsync(ReUri, new HttpStringContent("mid=" + Uid, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded"));
                    response.EnsureSuccessStatusCode();
                    string result = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(result);
                    if ((bool)json["status"])
                    {
                        btn_Attention.Visibility = Visibility.Collapsed;
                        btn_CannelAttention.Visibility = Visibility.Visible;
                        await getUser.GetUserInfo();
                    }
                    else
                    {
                        btn_Attention.Visibility = Visibility.Visible;
                        btn_CannelAttention.Visibility = Visibility.Collapsed;
                        MessageDialog md = new MessageDialog("关注失败！\r\n" + result);
                        await md.ShowAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageDialog md = new MessageDialog("关注发生错误！\r\n" + ex.Message);
                    await md.ShowAsync();
                }
            }
            else
            {
                MessageDialog md = new MessageDialog("请先登录！");
                await md.ShowAsync();
            }
        }

        private async void btn_CannelAttention_Click(object sender, RoutedEventArgs e)
        {
            UserClass getUser = new UserClass();
            if (getUser.IsLogin())
            {
                try
                {
                    Uri ReUri = new Uri("http://space.bilibili.com/ajax/friend/DelAttention");
                    HttpClient hc = new HttpClient();
                    hc.DefaultRequestHeaders.Referer = new Uri("http://space.bilibili.com/");
                    var response = await hc.PostAsync(ReUri, new HttpStringContent("mid=" + Uid, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded"));
                    response.EnsureSuccessStatusCode();
                    string result = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(result);
                    if ((bool)json["status"])
                    {
                        btn_Attention.Visibility = Visibility.Visible;
                        btn_CannelAttention.Visibility = Visibility.Collapsed;
                        await getUser.GetUserInfo();
                    }
                    else
                    {
                        btn_Attention.Visibility = Visibility.Collapsed;
                        btn_CannelAttention.Visibility = Visibility.Visible;
                        MessageDialog md = new MessageDialog("关注失败！\r\n" + result);
                        await md.ShowAsync();
                    }
                }
                catch (Exception ex)
                {
                    MessageDialog md = new MessageDialog("取消关注发生错误！\r\n" + ex.Message);
                    await md.ShowAsync();
                }
            }
            else
            {
                MessageDialog md = new MessageDialog("请先登录！");
                await md.ShowAsync();
            }
        }

        private async void btn_Exit_Click(object sender, RoutedEventArgs e)
        {
            List<HttpCookie> listCookies = new List<HttpCookie>();
            listCookies.Add(new HttpCookie("sid", ".bilibili.com", "/"));
            listCookies.Add(new HttpCookie("DedeUserID", ".bilibili.com", "/"));
            listCookies.Add(new HttpCookie("DedeUserID__ckMd5", ".bilibili.com", "/"));
            listCookies.Add(new HttpCookie("SESSDATA", ".bilibili.com", "/"));
            listCookies.Add(new HttpCookie("LIVE_LOGIN_DATA", ".bilibili.com", "/"));
            listCookies.Add(new HttpCookie("LIVE_LOGIN_DATA__ckMd5", ".bilibili.com", "/"));
            HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();
            foreach (HttpCookie cookie in listCookies)
            {
                filter.CookieManager.DeleteCookie(cookie);
            }
            ApplicationDataContainer container = ApplicationData.Current.LocalSettings;
            container.Values["AutoLogin"] = "false";
            container.Values["BirthDay"] = null;
            //settings.GetSettingValue("BirthDay"))
            ApiHelper.access_key = string.Empty;
            StorageFolder folder = ApplicationData.Current.LocalFolder;
            StorageFile file = await folder.CreateFileAsync("us.bili", CreationCollisionOption.OpenIfExists);
            await FileIO.WriteTextAsync(file, string.Empty);
            ExitEvent();
            BackEvent();
        }

        private void Ban_btn_Coin_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Edit_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(EditPage), grid_Info.DataContext);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int d = Convert.ToInt32(this.ActualWidth / 400);
            if (d > 3)
            {
                d = 3;
            }
            bor_Width.Width = this.ActualWidth / d - 22;
        }

        private async void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txt_her_0.FontWeight = FontWeights.Normal;
            txt_her_1.FontWeight = FontWeights.Normal;
            txt_her_2.FontWeight = FontWeights.Normal;
            txt_her_3.FontWeight = FontWeights.Normal;
            switch (pivot.SelectedIndex)
            {
                case 0:
                    txt_her_0.FontWeight = FontWeights.Bold;
                    break;
                case 1:
                    txt_her_1.FontWeight = FontWeights.Bold;
                    if (list_ASubit.Items.Count == 0)
                    {
                        getPage = 1;
                        list_ASubit.Items.Clear();
                        await GetSubInfo(Uid);
                    }
                    break;
                case 2:
                    txt_her_2.FontWeight = FontWeights.Bold;
                    break;
                case 3:
                    txt_her_3.FontWeight = FontWeights.Bold;
                    if (list_ACoin.Items.Count == 0)
                    {
                        GetPutCoin();
                    }
                    break;
                default:
                    break;
            }
        }

        private void btn_Message_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(ChatPage),new object[]{ Uid,ChatsType.New});
        }

        private void btn_load_More_Atton_Click(object sender, RoutedEventArgs e)
        {
            if (page <= MaxPage && !IsLoading)
            {
                GetUserAttention(page);
            }
            else
            {
                messShow.Show("没有更多了...",2000);
            }
        }

        private async void btn_More_Video_Click(object sender, RoutedEventArgs e)
        {
            if (!subLoading)
            {
                subLoading = true;
                await GetSubInfo(Uid);
                subLoading = false;
            }
        }
    }

    public class DataConverter1 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int r = 0;
            if (int.TryParse(value.ToString(), out r))
            {
                return (r / 10).ToString();
            }
            else
            {
                return 20;
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }


}
