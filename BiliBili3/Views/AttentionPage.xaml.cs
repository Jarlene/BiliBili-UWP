using BiliBili3.Helper;
using BiliBili3.Models;
using BiliBili3.Pages;
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
using Windows.Data.Xml.Dom;
using System.Net;
using Windows.UI.Notifications;
// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace BiliBili3.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AttentionPage : Page
    {
        public AttentionPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (SettingHelper.Get_RefreshButton() && SettingHelper.IsPc())
            {
                b_btn_Refresh.Visibility = Visibility.Visible;
            }
            else
            {
                b_btn_Refresh.Visibility = Visibility.Collapsed;

            }
            if (e.NavigationMode == NavigationMode.New)
            {
                await Task.Delay(200);
                MessageCenter.Logined += MessageCenter_Logined;
                if (!UserManage.IsLogin())
                {
                    DT_noLogin.Visibility = Visibility.Visible;
                    DT_Info.Visibility = Visibility.Collapsed;
                    b_btn_Refresh.Visibility = Visibility.Collapsed;
                }
                else
                {
                  
                    DT_noLogin.Visibility = Visibility.Collapsed;
                    DT_Info.Visibility = Visibility.Visible;
                    //GetBangumi();
                    if (User_ListView_Attention.Items.Count==0)
                    {
                        _PageNum = 1;
                        GetAttention();
                    }
                  
                }
            }
            
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            MessageCenter.Logined -= MessageCenter_Logined;
        }
        private void MessageCenter_Logined()
        {
            if (UserManage.IsLogin())
            {
                DT_noLogin.Visibility = Visibility.Collapsed;
                DT_Info.Visibility = Visibility.Visible;
                _PageNum = 1;
                //GetBangumi();
                GetAttention();
                messShow.Show(@"我的追番移至""追番""页里啦", 3000);
            }
        }

        //private async void GetBangumi()
        //{
        //    try
        //    {
        //        string results = await WebClientClass.GetResults(new Uri("http://space.bilibili.com/ajax/Bangumi/getList?mid=" + UserManage.Uid + "&pagesize=20"));
        //        //一层
        //        UserBangumiModel model1 = JsonConvert.DeserializeObject<UserBangumiModel>(results);
        //        if (model1.status)
        //        {
        //            //二层
        //            UserBangumiModel model2 = JsonConvert.DeserializeObject<UserBangumiModel>(model1.data.ToString());
        //            //三层
        //            List<UserBangumiModel> lsModel = JsonConvert.DeserializeObject<List<UserBangumiModel>>(model2.result.ToString());
        //            user_GridView_Bangumi.ItemsSource = lsModel ;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.HResult == -2147012867 || ex.HResult == -2147012889)
        //        {
        //            messShow.Show("无法连接服务器，请检查你的网络连接", 3000);
        //        }
        //        else
        //        {

        //            messShow.Show("读取追番错误", 3000);
        //        }
        //    }
        //    finally
        //    {
        //        if (user_GridView_Bangumi.Items.Count==0)
        //        {
        //            DT_0.Visibility = Visibility.Visible;
        //        }
        //        else
        //        {
        //            DT_0.Visibility = Visibility.Collapsed;
        //        }
        //    }


        //}
        private const string TileTemplateXml = @"
<tile branding='name'> 
  <visual version='3'>
    <binding template='TileMedium'>
      <image src='{0}' placement='peek'/>
      <text>{1}</text>
      <text hint-style='captionsubtle' hint-wrap='true'>{2}</text>
    </binding>
    <binding template='TileWide'>
      <image src='{0}' placement='peek'/>
      <text>{1}</text>
      <text hint-style='captionsubtle' hint-wrap='true'>{2}</text>
    </binding>
    <binding template='TileLarge'>
      <image src='{0}' placement='peek'/>
      <text>{1}</text>
      <text hint-style='captionsubtle' hint-wrap='true'>{2}</text>
    </binding>
  </visual>
</tile>";


        int _PageNum = 1;
        private async void GetAttention()
        {
            try
            {
                _Loading = true;
                if (_PageNum==1)
                {
                    User_ListView_Attention.Items.Clear();
                }
                User_load_more.IsEnabled = false;
                User_load_more.Content = "加载中...";
                _Loading = true;
                pr_Load_DT.Visibility = Visibility.Visible;
                string url = string.Format("http://api.bilibili.com/x/feed/pull?access_key={0}&actionKey=appkey&appkey={1}&platform=wp&pn={2}&ps=30&ts={3}&type=0", ApiHelper.access_key, ApiHelper._appKey, _PageNum, ApiHelper.GetTimeSpan);
                url += "&sign=" + ApiHelper.GetSign(url);
                string results = await WebClientClass.GetResults(new Uri(url));
                //一层
                AttentionModel model1 = JsonConvert.DeserializeObject<AttentionModel>(results);
                if (model1.code == 0)
                {
                    AttentionModel model2 = JsonConvert.DeserializeObject<AttentionModel>(model1.data.ToString());
                    if (model2.feeds.Count == 0)
                    {
                        messShow.Show("加载完了", 3000);
                    }
                    else
                    {
                        try
                        {
                            if (_PageNum == 1)
                            {
                                //var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                                //updater.EnableNotificationQueueForWide310x150(true);
                                //updater.EnableNotificationQueueForSquare150x150(true);
                                //updater.EnableNotificationQueueForSquare310x310(true);
                                //updater.EnableNotificationQueue(true);

                                List<string> oldList = new List<string>();
                                //updater.Clear();
                                string s1 = "";
                                foreach (var item in model2.feeds)
                                {
                                    //if (SettingHelper.Get_DTCT())
                                    //{
                                    //    if (model2.feeds.IndexOf(item) <= 4)
                                    //    {
                                    //        var doc = new XmlDocument();
                                    //        var xml = string.Format(TileTemplateXml, item.addition.pic, item.addition.title, item.addition.title);
                                    //        doc.LoadXml(WebUtility.HtmlDecode(xml), new XmlLoadSettings
                                    //        {
                                    //            ProhibitDtd = false,
                                    //            ValidateOnParse = false,
                                    //            ElementContentWhiteSpace = false,
                                    //            ResolveExternals = false
                                    //        });
                                    //        updater.Update(new TileNotification(doc));
                                    //    }
                                    //}
                                   

                                    s1 += item.addition.aid + ",";
                                }
                              
                                s1 = s1.Remove(s1.Length - 1);
                                SettingHelper.Set_TsDt(s1);
                                //container.Values["TsDt"] = s1;
                            }
                        }
                        catch (Exception)
                        {
                        }
                       
                        model2.feeds.ForEach(x => User_ListView_Attention.Items.Add(x));
                        _PageNum++;
                       


                       
                    }
                }
                else
                {

                    messShow.Show(model1.message, 3000);
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

                    messShow.Show("动态加载错误", 3000);
                }
            }
            finally
            {
                _Loading = false;
                User_load_more.IsEnabled = true;
                User_load_more.Content = "加载更多";
                pr_Load_DT.Visibility = Visibility.Collapsed;
                if (User_ListView_Attention.Items.Count == 0)
                {
                    DT_1.Visibility = Visibility.Visible;
                    User_load_more.Visibility = Visibility.Collapsed;
                }
                else
                {
                    DT_1.Visibility = Visibility.Collapsed;
                    User_load_more.Visibility = Visibility.Visible;
                }
            }
        }

        private void Ban_btn_MyBan_Click(object sender, RoutedEventArgs e)
        {
            if (UserManage.IsLogin())
            {
                //infoFrame.Navigate(typeof(UserBangumiPage), UserClass.Uid);
            }
            else
            {
                messShow.Show("请先登录", 3000);
            }
        }

        //刷新动态
        private void btn_refresh_Atton_Click(object sender, RoutedEventArgs e)
        {
            _PageNum = 1;
            //GetBangumi();
            GetAttention();
        }
        private void HyperlinkButton_Click_2(object sender, RoutedEventArgs e)
        {
            if (UserManage.IsLogin())
            {
                //infoFrame.Navigate(typeof(UserBangumiPage), UserClass.Uid);
            }
            else
            {
                messShow.Show("请先登录", 3000);
            }
        }
        private void sc_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int i = Convert.ToInt32(sc.ActualWidth / 400);
            if (i > 3)
            {
                i = 3;
            }
            bor_Width.Width = sc.ActualWidth / i - 32;
        }

        private void User_Login_Click(object sender, RoutedEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(LoginPage));
        }
        bool _Loading = false;
        private void User_load_more_Click(object sender, RoutedEventArgs e)
        {
            GetAttention();
        }

        private void sc_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sc.VerticalOffset == sc.ScrollableHeight)
            {
                if (!_Loading)
                {
                    GetAttention();
                }
            }
        }

        private void User_ListView_Attention_ItemClick(object sender, ItemClickEventArgs e)
        {
            var model = e.ClickedItem as AttentionModel;
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(VideoViewPage), model.addition.aid);
        }

        private void user_GridView_Bangumi_ItemClick(object sender, ItemClickEventArgs e)
        {
            var info = e.ClickedItem as UserBangumiModel;
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(BanInfoPage), info.season_id);
        }

        private void b_btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            _PageNum = 1;
            GetAttention();
        }

        private void PullToRefreshBox_RefreshInvoked(DependencyObject sender, object args)
        {
            _PageNum = 1;
            GetAttention();
        }
    }
    public class FeedItemDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if ((item as AttentionModel).type == 3)
            {
                return App.Current.Resources["Feed2"] as DataTemplate;
            }
            else
            {
                return App.Current.Resources["Feed1"] as DataTemplate;
            }
        }
    }
}
