using BiliBili3.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using System.Threading.Tasks;
using BiliBili3.Helper;
using Newtonsoft.Json.Linq;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace BiliBili3.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class UserInfoPage : Page
    {
        public UserInfoPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;
        }
        string Uid = string.Empty;
        bool canB = false;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New || canB)
            {

                Uid = "";
                pivot.SelectedIndex = 0;

                page = 1;
                MaxPage = 0;
                list_AUser.Items.Clear();
                list_ASubit.Items.Clear();
                if (e.Parameter == null|| (e.Parameter as object[]).Length == 0)
                {
                    btn_EditProfile.Visibility = Visibility.Visible;
                    btn_Chat.Visibility = Visibility.Collapsed;
                    btn_AddFollow.Visibility = Visibility.Collapsed;
                    btn_CancelFollow.Visibility = Visibility.Collapsed;
                    favbox.Visibility = Visibility.Visible;
                    Uid = ApiHelper.GetUserId();

                }
                else
                {
                    if ((e.Parameter as object[]).Length!=1)
                    {
                        btn_EditProfile.Visibility = Visibility.Visible;
                        favbox.Visibility = Visibility.Visible;
                        btn_Chat.Visibility = Visibility.Collapsed;
                        btn_AddFollow.Visibility = Visibility.Collapsed;
                        btn_CancelFollow.Visibility = Visibility.Collapsed;
                        Uid = ApiHelper.GetUserId();
                        pivot.SelectedIndex = (int)(e.Parameter as object[])[1];
                    }
                    else
                    {
                        btn_EditProfile.Visibility = Visibility.Collapsed;
                        favbox.Visibility = Visibility.Collapsed;
                        btn_Chat.Visibility = Visibility.Visible;
                        btn_AddFollow.Visibility = Visibility.Visible;
                        btn_CancelFollow.Visibility = Visibility.Visible;
                        Uid = (e.Parameter as object[])[0].ToString();
                        if (ApiHelper.followList==null)
                        {
                            UserManage.UpdateFollowList();
                        }
                        else
                        {
                            if (ApiHelper.IsLogin() && ApiHelper.followList.Contains(Uid))
                            {
                                btn_AddFollow.Visibility = Visibility.Collapsed;
                                btn_CancelFollow.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                btn_AddFollow.Visibility = Visibility.Visible;
                                btn_CancelFollow.Visibility = Visibility.Collapsed;
                            }
                        }
                    }
                }

                GetUserInfo();
            }
            else
            {
                pivot.SelectedIndex = 0;

                canB = true;

            }
        }
        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
            else
            {
                canB = false;
            }

        }

        private async void GetUserInfo()
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                string results = await WebClientClass.PostResults(new Uri("http://space.bilibili.com/ajax/member/GetInfo"), "mid=" + Uid + "&_=" + ApiHelper.GetTimeSpan_2, "http://space.bilibili.com/" + Uid + "/");
                UserModel um = JsonConvert.DeserializeObject<UserModel>(results);

               


                if (um.status)
                {
                    info.DataContext = um.data;
                    grid_UserInfo.DataContext = um.data;

                    if (um.data.approve)
                    {
                        if (um.data.official_verify.type != -1)
                        {
                            txt_RZ.Text = um.data.official_verify.desc;
                        }
                        txt_RZ.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        txt_RZ.Visibility = Visibility.Collapsed;
                    }

                    if (um.data.level_info.current_level <= 3)
                    {
                        bor_Level.Background = new SolidColorBrush(new Windows.UI.Color() { R = 48, G = 161, B = 255, A = 200 });
                    }
                    else
                    {
                        if (um.data.current_level <= 6)
                        {
                            bor_Level.Background = new SolidColorBrush(new Windows.UI.Color() { R = 255, G = 48, B = 48, A = 200 });
                        }
                        else
                        {
                            bor_Level.Background = new SolidColorBrush(new Windows.UI.Color() { R = 255, G = 199, B = 45, A = 200 });
                        }
                    }


                }
                else
                {
                    messShow.Show("读取用户信息失败",3000);
                }

                string url = string.Format("http://app.bilibili.com/x/v2/space?access_key={0}&appkey={1}&platform=wp&ps=10&ts={2}000&vmid={3}&build=411005&mobi_app=android", ApiHelper.access_key, ApiHelper._appKey, ApiHelper.GetTimeSpan, Uid);

                url += "&sign=" + ApiHelper.GetSign(url);
                string results1 = await WebClientClass.GetResults(new Uri(url));
                UserInfoModel m = JsonConvert.DeserializeObject<UserInfoModel>(results1);
                if (m.code==0)
                {
                    if (m.data.season != null&& m.data.season.item != null)
                    {
                        DT_0.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        DT_0.Visibility = Visibility.Visible;
                    }

                    if (m.data.card.vip.vipType != 0)
                    {
                        bor_Vip.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        bor_Vip.Visibility = Visibility.Collapsed;
                    }
                if (m.data.coin_archive!=null&& m.data.coin_archive.item != null)
                {
                    list_ACoin.ItemsSource = m.data.coin_archive.item;

                }
                 
                    data.DataContext = m.data;
                }
               

        }
            catch (Exception)
            {
                messShow.Show("读取用户信息失败", 3000);
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }

        int page = 1;
        int MaxPage = 0;
        bool IsLoading = true;
        public async void GetUserAttention(int pageNum)
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                IsLoading = true;
              
               
                string results = await WebClientClass.GetResults(new Uri("http://space.bilibili.com/ajax/friend/GetAttentionList?mid=" + Uid + "&pagesize=30&page=" + pageNum));
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
                    // messShow.Show("读取关注失败！",2000);
                }
            }
            catch (Exception)
            {
                if (list_AUser.Items.Count == 0)
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



        private void user_GridView_FovBox_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(MyCollectPage),(e.ClickedItem as UserInfoModel).fid);
        }

        private void user_GridView_Bangumi_ItemClick(object sender, ItemClickEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(BanInfoPage), (e.ClickedItem as UserInfoModel).param);
            //this.Frame.Navigate(typeof(MyCollectPage), );
        }

        private void btn_AttBangumi_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MyFollowsBangumiPage));
        }

        private void list_ASubit_ItemClick(object sender, ItemClickEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(VideoViewPage), (e.ClickedItem as UserInfoModel).param);
        }

        private void btn_load_More_Atton_Click(object sender, RoutedEventArgs e)
        {
            if (page <= MaxPage && !IsLoading)
            {
                GetUserAttention(page);
            }
        }

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

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int d = Convert.ToInt32(this.ActualWidth / 400);
            if (d > 3)
            {
                d = 3;
            }
            bor_Width.Width = this.ActualWidth / d - 22;
        }

        private void list_AUser_ItemClick(object sender, ItemClickEventArgs e)
        {
            canB = true;
            this.Frame.Navigate(typeof(UserInfoPage), new object[] { ((GetUserAttention)e.ClickedItem).fid });
        }

        private async void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (pivot.SelectedIndex)
            {
                case 1:

                    if (list_ASubit.Items.Count == 0)
                    {
                        getPage = 1;
                     
                        await GetSubInfo(Uid);
                    }
                    break;
                case 2:
                    if (list_AUser.Items.Count==0)
                    {
                        GetUserAttention(page);
                    }
                    break;
                default:
                    break;
            }
        }

        private void btn_More_Video_Click(object sender, RoutedEventArgs e)
        {

            if (page <= MaxPage && !IsLoading)
            {
                GetUserAttention(page);
            }
            else
            {
                messShow.Show("没有更多了...", 2000);
            }
        }

        private int getPage = 1;
        private async Task GetSubInfo(string uid)
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                btn_More_Video.Visibility = Visibility.Collapsed;
                string results = await WebClientClass.GetResults(new Uri("http://space.bilibili.com/ajax/member/getSubmitVideos?mid=" + uid + "&pagesize=30" + "&page=" + getPage));
                //一层
                GetUserSubmit model1 = JsonConvert.DeserializeObject<GetUserSubmit>(results);
                //二层
                GetUserSubmit model2 = JsonConvert.DeserializeObject<GetUserSubmit>(model1.data.ToString());
                //三层
                List<GetUserSubmit> lsModel = JsonConvert.DeserializeObject<List<GetUserSubmit>>(model2.vlist.ToString());
                if (lsModel.Count!=0)
                {
                    foreach (GetUserSubmit item in lsModel)
                    {
                        list_ASubit.Items.Add(item);
                    }
                    getPage++;
                }
                else
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
                    messShow.Show("没有投稿", 3000);
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
                    await GetSubInfo(Uid);
                    subLoading = false;
                }
            }

        }

        private void list_ASubit_ItemClick_1(object sender, ItemClickEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(VideoViewPage), (e.ClickedItem as GetUserSubmit).aid);
        }

        private async void btn_AddFollow_Click(object sender, RoutedEventArgs e)
        {
            if (ApiHelper.IsLogin())
            {
                try
                {
                    Uri ReUri = new Uri("http://space.bilibili.com/ajax/friend/AddAttention");

                    string result = await WebClientClass.PostResults(ReUri, "mid=" + Uid, "http://space.bilibili.com/");
                    JObject json = JObject.Parse(result);
                    if ((bool)json["status"])
                    {
                        messShow.Show("关注成功", 3000);
                        btn_AddFollow.Visibility = Visibility.Collapsed;
                        btn_CancelFollow.Visibility = Visibility.Visible;
                      UserManage.UpdateFollowList();
                        MessageCenter.SendLogined();
                    }
                    else
                    {
                        messShow.Show("关注失败\r\n" + result, 3000);

                    }

                }
                catch (Exception ex)
                {
                    messShow.Show("关注时发生错误\r\n"+ex.Message, 3000);
                }
            }
            else
            {
                messShow.Show("请先登录", 3000);
            }
        }

        private async void btn_CancelFollow_Click(object sender, RoutedEventArgs e)
        {
            if (ApiHelper.IsLogin())
            {
                try
                {
                    Uri ReUri = new Uri("http://space.bilibili.com/ajax/friend/DelAttention");

                    string result = await WebClientClass.PostResults(ReUri, "mid=" + Uid, "http://space.bilibili.com/");
                    JObject json = JObject.Parse(result);
                    if ((bool)json["status"])
                    {
                        messShow.Show("已取消关注", 3000);
                        btn_AddFollow.Visibility = Visibility.Visible;
                        btn_CancelFollow.Visibility = Visibility.Collapsed;

                        UserManage.UpdateFollowList();
                        MessageCenter.SendLogined();
                    }
                    else
                    {
                        messShow.Show("取消关注失败\r\n" + result, 3000);

                    }

                }
                catch (Exception ex)
                {
                    messShow.Show("关注时发生错误\r\n" + ex.Message, 3000);
                }
            }
            else
            {
                messShow.Show("请先登录", 3000);
            }
        }

        private void btn_Chat_Click(object sender, RoutedEventArgs e)
        {

            this.Frame.Navigate(typeof(ChatPage), new object[] { Uid, ChatType.New });
        }

        private void btn_EditProfile_Click(object sender, RoutedEventArgs e)
        {
            canB = true;
            this.Frame.Navigate(typeof(EditProfilePage));
        }
    }
}
