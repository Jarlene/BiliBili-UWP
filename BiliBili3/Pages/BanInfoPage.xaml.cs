using BiliBili3.Controls;
using BiliBili3.Helper;
using BiliBili3.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.System;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace BiliBili3.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BanInfoPage : Page
    {
        public BanInfoPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
         
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }
        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = txt_Name.Text;
            request.Data.Properties.Description = txt_desc.Text + "\r\n——分享自BiliBili UWP";
            request.Data.SetWebLink(new Uri("http://bangumi.bilibili.com/anime/" + _banId + "/"));
        }

        //bool locks = false;
     



       
        string _banId = "";
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if (e.NavigationMode== NavigationMode.New || Back)
            //{
            //mediaElement.Source = null;
            btn_OrderByUp.Visibility = Visibility.Visible;
            btn_OrderByDown.Visibility = Visibility.Collapsed;
          
                pivot.SelectedIndex = 0;
                _banId = (e.Parameter as object[])[0].ToString();
                GetInfo();
            //}
        }
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
         
            comment.DisposableComment();
        }
        private async void GetInfo()
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
               string uri = string.Format("http://bangumi.bilibili.com/api/season_v3?_device=wp&access_key={2}&_ulv=10000&build=411005&platform=android&appkey=422fd9d7289a1dd9&ts={0}000&type=bangumi&season_id={1}", ApiHelper.GetTimeSpan, _banId, ApiHelper.access_key);
                uri += "&sign=" + ApiHelper.GetSign(uri);
                string results = "";
                if (SettingHelper.Get_UseCN() || SettingHelper.Get_UseHK() || SettingHelper.Get_UseTW())
                {
                    results = await WebClientClass.GetResults_Proxy(uri);
                }
                else
                {
                    results = await WebClientClass.GetResults(new Uri(uri));
                }

               // string results = await WebClientClass.GetResults(new Uri(uri));
                BangumiInfoModel model = JsonConvert.DeserializeObject<BangumiInfoModel>(results);
              
                if (model.code==0)
                {
                    int i = 0;

                    model.result.episodes.ForEach(x => { x.orderindex = i;i++; });
                    model.result.episodes = model.result.episodes.OrderByDescending(x =>  x.orderindex).ToList();
                   
                    this.DataContext = model.result;
                   
                  
                    gv_Play.ItemsSource = model.result.episodes;
                    
                    if (model.result.user_season.attention == 0)
                    {
                        btn_Like.Visibility = Visibility.Visible;
                        btn_CancelLike.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        btn_Like.Visibility = Visibility.Collapsed;
                        btn_CancelLike.Visibility = Visibility.Visible;
                    }
                    if (model.result.rank != null)
                    {
                        BangumiInfoModel rank = JsonConvert.DeserializeObject<BangumiInfoModel>(model.result.rank.ToString());
                        grid_Cb.DataContext = rank;
                        grid_Cb.Visibility = Visibility.Visible;
                        txt_NotCb.Visibility = Visibility.Collapsed;
                        GetRankInfo();
                    }
                    else
                    {
                        txt_NotCb.Visibility = Visibility.Visible;
                        grid_Cb.Visibility = Visibility.Collapsed;
                    }

                    if (model.result.seasons != null)
                    {
                        Grid_About.Visibility = Visibility.Visible;
                     
                        WrapPanel_About.Children.Clear();
                        if (model.result.seasons.Count == 1)
                        {
                            Grid_About.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            foreach (BangumiInfoModel item in model.result.seasons)
                            {

                                HyperlinkButton btn = new HyperlinkButton();
                                btn.DataContext = item;
                                btn.Margin = new Thickness(0, 0, 10, 0);
                                btn.Content = item.title;
                                btn.Foreground = App.Current.Resources["Bili-ForeColor"] as SolidColorBrush;
                                if (item.season_id == _banId)
                                {
                                    btn.IsEnabled = false;
                                }
                                btn.Click += Btn_Click; ;
                                WrapPanel_About.Children.Add(btn);

                            }
                        }

                        //Grid_About
                    }
                    else
                    {
                        Grid_About.Visibility = Visibility.Collapsed;
                    }


                    if (model.result.tags!=null)
                    {
                        WrapPanel_tag.Children.Clear();
                        foreach (var item in model.result.tags)
                        {
                            HyperlinkButton btn = new HyperlinkButton();
                            btn.DataContext = item;
                            btn.Margin = new Thickness(0, 0, 10, 0);
                            btn.Content = item.tag_name;
                            btn.Foreground = App.Current.Resources["Bili-ForeColor"] as SolidColorBrush;
                            btn.Click += Btn_Click1; ;
                            WrapPanel_tag.Children.Add(btn);
                        }
                    }

                    if (gv_Play.Items.Count!=0)
                    {
                      
                        cb_H.SelectedIndex = cb_H.Items.Count -1;
                        gv_Play.SelectedIndex = 0;
                    }
                    else
                    {
                        messShow.Show("尚未开播或不支持你所在地区", 3000);
                    }
                }
                else
                {
                    messShow.Show(model.message,3000);
                }
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
            }
            finally
            {
                Page_SizeChanged(null, null);
                btn_HideAll_Click(null, null);
               
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }
        //private async void GetPlayUrl(string cid)
        //{
        //    string url = "http://interface.bilibili.com/playurl?_device=uwp&cid=" + cid + "&otype=xml&quality=" + 2 + "&appkey=" + ApiHelper._appKey + "&access_key=" + ApiHelper.access_key + "&type=mp4&mid=" + "" + "&_buvid=" + ApiHelper._buvid + "&_hwid=" + ApiHelper._hwid + "&platform=uwp_desktop" + "&ts=" + ApiHelper.GetTimeSpan;
        //    url += "&sign=" + ApiHelper.GetSign(url);
        //    string re = await WebClientClass.GetResults_Phone(new Uri(url));
        //    re = await WebClientClass.GetResults_Phone(new Uri(url));
        //    string playUrl = Regex.Match(re, "<url>(.*?)</url>").Groups[1].Value;
        //    playUrl = playUrl.Replace("<![CDATA[", "");
        //    playUrl = playUrl.Replace("]]>", "");

        //    mediaElement.Source = new Uri(playUrl);
        //}
        private void Btn_Click1(object sender, RoutedEventArgs e)
        {
           //TO DO TAG
        }
        //bool Back = false;
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            //TO DO Seaseon
            //Back = true;
            this.Frame.Navigate(typeof(BanInfoPage), new object[] { ((sender as HyperlinkButton).DataContext as BangumiInfoModel).season_id});
        }

        private void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((sender as TextBlock).MaxLines==3)
            {
                (sender as TextBlock).MaxLines = 0;
            }
            else
            {
                (sender as TextBlock).MaxLines = 3;
            }
        }
        private void Share_Click(object sender, RoutedEventArgs e)
        {
            Windows.ApplicationModel.DataTransfer.DataPackage pack = new Windows.ApplicationModel.DataTransfer.DataPackage();
            pack.SetText(string.Format("我正在BiliBili追《{0}》,一起来看吧\r\n地址：http://bangumi.bilibili.com/anime/{1}", txt_Name.Text, _banId));
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(pack); // 保存 DataPackage 对象到剪切板
            Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
            messShow.Show("已将内容复制到剪切板", 3000);
        }

        private void btn_Share_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }
        private async void GetRankInfo()
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
              
                string url = string.Empty;
                if (cb_Cb.SelectedIndex == 0)
                {
                    url = string.Format("http://bangumi.bilibili.com/sponsor/rank/get_sponsor_week_list?access_key={0}&appkey={1}&build=418000&mobi_app=android&page=1&pagesize=25&platform=android&season_id={2}&ts={3}", ApiHelper.access_key, ApiHelper._appKey_Android, _banId, ApiHelper.GetTimeSpan);
                }
                else
                {
                    url = string.Format("http://bangumi.bilibili.com/sponsor/rank/get_sponsor_total?access_key={0}&appkey={1}&build=418000&mobi_app=android&page=1&pagesize=25&platform=android&season_id={2}&ts={3}", ApiHelper.access_key, ApiHelper._appKey_Android, _banId, ApiHelper.GetTimeSpan);
                }
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                string results = await WebClientClass.GetResults(new Uri(url));
                CBRankModel model = JsonConvert.DeserializeObject<CBRankModel>(results);
                if (model.code == 0)
                {
                    CBRankModel resultModel = JsonConvert.DeserializeObject<CBRankModel>(model.result.ToString());
                    List<CBRankModel> ls = JsonConvert.DeserializeObject<List<CBRankModel>>(resultModel.list.ToString());
                    list_Rank.ItemsSource = ls;
                }
                else
                {
                    messShow.Show("读取承包失败," + model.message, 3000);
                }
            }
            catch (Exception)
            {
                messShow.Show("读取承包失败", 3000);
                //throw;
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }
        private void btn_CB_Click(object sender, RoutedEventArgs e)
        {
            if (!UserManage.IsLogin())
            {
                messShow.Show("请先登录！", 3000);
                return;
            }
            Flyout.ShowAttachedFlyout((HyperlinkButton)sender);
            rb_5.IsChecked = true;
        }

        private void rb_5_Checked(object sender, RoutedEventArgs e)
        {
            txt_Money.Text = "5";
        }

        private void rb_10_Checked(object sender, RoutedEventArgs e)
        {
            txt_Money.Text = "10";
        }

        private void rb_50_Checked(object sender, RoutedEventArgs e)
        {
            txt_Money.Text = "50";
        }

        private void rb_450_Checked(object sender, RoutedEventArgs e)
        {
            txt_Money.Text = "450";
        }

        private void rb_ZDY_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void btn_OKCB_Click(object sender, RoutedEventArgs e)
        {
            int b = 0;
            if (int.TryParse(txt_Money.Text.ToString(), out b))
            {
                fy.Hide();
                createOrder(b);
            }
            else
            {
                messShow.Show("输入金额错误,必须为整数", 2000);
            }
        }

        private async void createOrder(int money)
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                messShow.Show("开始创建订单", 2000);
                string tokenString = await WebClientClass.GetResults(new Uri("http://bangumi.bilibili.com/web_api/get_token"));
                TokenModel tokenMess = JsonConvert.DeserializeObject<TokenModel>(tokenString);
                if (tokenMess.code == 0)
                {
                    TokenModel token = JsonConvert.DeserializeObject<TokenModel>(tokenMess.result.ToString());
                    string results = await WebClientClass.PostResults(new Uri("http://bangumi.bilibili.com/sponsor/payweb/create_order"), string.Format("pay_method=0&season_id={0}&amount={1}&token={2}", _banId, money, token.token));
                    OrderModel orderMess = JsonConvert.DeserializeObject<OrderModel>(results);
                    if (orderMess.code == 0)
                    {
                        OrderModel orderModel = JsonConvert.DeserializeObject<OrderModel>(orderMess.result.ToString());
                        this.Frame.Navigate(typeof(WebPage), new object[] { orderModel.cashier_url });
                    }
                    else
                    {
                      
                            messShow.Show("订单创建失败," + orderMess.data, 3000);
                        
                    }
                }
                else
                {
                    messShow.Show("读取Token失败," + tokenMess.message, 3000);
                }
            }
            catch (Exception)
            {
                messShow.Show("订单创建失败,请稍后重试", 3000);
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }

        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (pivot.SelectedIndex)
            {
                case 0:
                    cb_H.Visibility = Visibility.Collapsed;
                    cb_Cb.Visibility = Visibility.Collapsed;
                    com_bar.Visibility = Visibility.Visible;
                    break;
                case 1:
                    cb_H.Visibility = Visibility.Visible;
                    cb_Cb.Visibility = Visibility.Collapsed;
                    com_bar.Visibility = Visibility.Collapsed;
                    //Down_ComBar.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    cb_H.Visibility = Visibility.Collapsed;
                    cb_Cb.Visibility = Visibility.Visible;
                    com_bar.Visibility = Visibility.Collapsed;
                    //Down_ComBar.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }
        }

        private async void cb_H_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_H.SelectedItem!=null)
            {
                if ((cb_H.SelectedItem as episodesModel).episode_status==7)
                {
                    string results = await WebClientClass.GetResults(new Uri("http://bangumi.bilibili.com/web_api/episode/" + (cb_H.SelectedItem as episodesModel).episode_id + ".json"));
                    JObject jobj = JObject.Parse(results);
                    if ((int)jobj["code"] == 0)
                    {
                        comment.InitializeComment(1, 1, jobj["result"]["currentEpisode"]["avId"].ToString());
                    }


                }
                else
                {
                    comment.InitializeComment(1, 1, (cb_H.SelectedItem as episodesModel).av_id);

                }
                
            }
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void cb_Cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_Cb.Items.Count != 0 && _banId.Length != 0)
            {
                GetRankInfo();
            }
        }

        private async void btn_GoBrowser_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://bangumi.bilibili.com/anime/"+_banId+"/"));
        }

        private void gv_Play_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void comment_OpenUser(string id)
        {
            this.Frame.Navigate(typeof(UserInfoPage), new object[] { id });
        }

        private void Video_Refresh_Click(object sender, RoutedEventArgs e)
        {
            pivot.SelectedIndex = 0;
            GetInfo();
        }

        private async void btn_Like_Click(object sender, RoutedEventArgs e)
        {
            if (!ApiHelper.IsLogin())
            {
                messShow.Show("请先登录！", 3000);
                return;
            }
            try
            {
                string results = await WebClientClass.GetResults(new Uri("http://www.bilibili.com/api_proxy?app=bangumi&action=/concern_season&season_id=" + _banId));
                JObject json = JObject.Parse(results);
                if ((int)json["code"] == 0)
                {
                    btn_CancelLike.Visibility = Visibility.Visible;
                    btn_Like.Visibility = Visibility.Collapsed;
                    messShow.Show("订阅成功!", 3000);
                }
                else
                {
                    messShow.Show("订阅失败!", 3000);
                }
            }
            catch (Exception)
            {

                messShow.Show("订阅发生错误",3000);
            }
        }

        private async void btn_CancelLike_Click(object sender, RoutedEventArgs e)
        {
            if (!ApiHelper.IsLogin())
            {
                messShow.Show("请先登录！", 3000);
                return;
            }
            try
            {
                string results = await WebClientClass.GetResults(new Uri("http://www.bilibili.com/api_proxy?app=bangumi&action=/unconcern_season&season_id=" + _banId));
                JObject json = JObject.Parse(results);
                if ((int)json["code"] == 0)
                {
                    btn_CancelLike.Visibility = Visibility.Collapsed;
                    btn_Like.Visibility = Visibility.Visible;
                    messShow.Show("取消订阅成功!", 3000);
                }
                else
                {
                    messShow.Show("取消订阅失败!", 3000);
                }
            }
            catch (Exception)
            {

                messShow.Show("取消订阅发生错误", 3000);
            }
        }

        private void btn_ALL_Click(object sender, RoutedEventArgs e)
        {
            if (gv_Play.SelectedItems.Count == gv_Play.Items.Count)
            {
                gv_Play.SelectedItems.Clear();
            }
            else
            {
                gv_Play.SelectAll();
            }
        }

        private async void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            if (gv_Play.SelectedItems.Count == 0)
            {
                return;
            }
            var info = gv_Play.SelectedItem as episodesModel;

            int i = 1;
            foreach (episodesModel item in gv_Play.SelectedItems)
            {
                if (item.IsDowned == Visibility.Collapsed)
                {
                    var vitem = new PlayerModel() { Aid = _banId, Mid = item.danmaku.ToString(), Mode = PlayMode.Video, No =item.index, VideoTitle = item.index_title, Title = (this.DataContext as BangumiInfoModel).title };

                    DownloadModel m = new DownloadModel();
                    m.folderinfo = new FolderListModel()
                    {
                        id = _banId,
                        desc = txt_desc.Text,
                        title = txt_Name.Text,
                        isbangumi = true,
                        thumb = (this.DataContext as BangumiInfoModel).cover

                    };
                    m.videoinfo = new VideoListModel()
                    {
                        id = _banId,
                        mid = vitem.Mid,
                        part = Convert.ToInt32(vitem.No),
                        partTitle = vitem.No + " " + vitem.VideoTitle,
                        videoUrl = await ApiHelper.GetVideoUrl(vitem, cb_Qu.SelectedIndex + 1),
                        title = txt_Name.Text
                    };

                    DownloadHelper.StartDownload(m);
                }
               
                i++;

            }

            messShow.Show("任务已加入下载", 3000);
            gv_Play.SelectionMode = ListViewSelectionMode.None;
            gv_Play.IsItemClickEnabled = true;
            Down_ComBar.Visibility = Visibility.Collapsed;
            com_bar.Visibility = Visibility.Visible;

        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            gv_Play.SelectionMode = ListViewSelectionMode.None;
            gv_Play.IsItemClickEnabled = true;
            Down_ComBar.Visibility = Visibility.Collapsed;
            com_bar.Visibility = Visibility.Visible;
        }

        private async void btn_Download_Click(object sender, RoutedEventArgs e)
        {
            if (gv_Play.Items != null)
            {
                if (gv_Play.Items.Count == 1)
                {
                    var info = gv_Play.SelectedItem as episodesModel;

                    List<PlayerModel> ls = new List<PlayerModel>();
                   // int i = 1;
                    foreach (episodesModel item in gv_Play.Items)
                    {
                        if (item.IsDowned== Visibility.Collapsed)
                        {
                            ls.Add(new PlayerModel() { Aid = _banId, Mid = item.danmaku.ToString(), Mode = PlayMode.Video, No = item.index, VideoTitle = item.index_title, Title = (this.DataContext as BangumiInfoModel).title });
                        }
                       // i++;
                    }
                    DownloadModel m = new DownloadModel();
                    m.folderinfo = new FolderListModel()
                    {
                        id = _banId,
                        desc = txt_desc.Text,
                        title = txt_Name.Text,
                        isbangumi = true,
                        thumb = (this.DataContext as BangumiInfoModel).cover

                    };
                    m.videoinfo = new VideoListModel()
                    {
                        id = _banId,
                        mid = ls[0].Mid,
                        part = Convert.ToInt32(ls[0].No),
                        partTitle = ls[0].VideoTitle,
                        videoUrl = await ApiHelper.GetVideoUrl(ls[0], cb_Qu.SelectedIndex + 1),
                        title = txt_Name.Text
                    };

                    DownloadHelper.StartDownload(m);
                    messShow.Show("任务已加入下载", 3000);
                }
                else
                {
                    gv_Play.SelectionMode = ListViewSelectionMode.Multiple;
                    gv_Play.IsItemClickEnabled = false;
                    messShow.Show("请选中要下载的分P视频，点击确定", 3000);
                    Down_ComBar.Visibility = Visibility.Visible;
                    com_bar.Visibility = Visibility.Collapsed;
                }


                //players.LoadPlayer(ls, gv_Play.SelectedIndex);
            }


        }

        private void btn_HideAll_Click(object sender, RoutedEventArgs e)
        {
            btn_HideAll.Visibility = Visibility.Collapsed;
            btn_ShowAll.Visibility = Visibility.Visible;
            if (_rows > 3)
            {
                gv_Play.Height = 54 * 3;
            }
            else
            {
                gv_Play.Height = double.NaN;
            }
        }

        private void btn_ShowAll_Click(object sender, RoutedEventArgs e)
        {
            btn_HideAll.Visibility = Visibility.Visible;
            btn_ShowAll.Visibility = Visibility.Collapsed;
            gv_Play.Height = double.NaN;
        }

        private void btn_OrderByUp_Click(object sender, RoutedEventArgs e)
        {
            btn_OrderByUp.Visibility = Visibility.Collapsed;
            btn_OrderByDown.Visibility = Visibility.Visible;
            var ls = gv_Play.ItemsSource as List<episodesModel>;
            gv_Play.ItemsSource = ls.OrderBy(x => x.orderindex).ToList();
            
        }

        private void btn_OrderByDown_Click(object sender, RoutedEventArgs e)
        {

            btn_OrderByUp.Visibility = Visibility.Visible;
            btn_OrderByDown.Visibility = Visibility.Collapsed;
            var ls = gv_Play.ItemsSource as List<episodesModel>;
            gv_Play.ItemsSource = ls.OrderByDescending(x => x.orderindex).ToList();
        }
        int _rows = 0;
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int i = 2;
            if (this.ActualWidth>500)
            {
                 i =3;
                if (gv_Play.Items.Count % 3 != 0)
                {
                    _rows = (gv_Play.Items.Count / 3) + 1;
                }
                else
                {
                    _rows = gv_Play.Items.Count / 3;
                }

                bor_Width.Width = this.ActualWidth / i - 39;
            }
            else
            {
                if (gv_Play.Items.Count % 2!=0)
                {
                    _rows = (gv_Play.Items.Count / 2)+1;
                }
                else
                {
                    _rows = gv_Play.Items.Count / 2;
                }
                bor_Width.Width = this.ActualWidth / i - 42;
            }
               
           

        }

        private  void gv_Play_ItemClick(object sender, ItemClickEventArgs e)
        {
            var info = e.ClickedItem as episodesModel;
            

            List<PlayerModel> ls = new List<PlayerModel>();
           // int i = 1;
            foreach (episodesModel item in gv_Play.Items)
            {
                if (item.IsDowned== Visibility.Visible)
                {
                   
                    ls.Add(new PlayerModel() { Aid = item.av_id, Mid = item.danmaku.ToString(), Mode = PlayMode.Local, No = item.index, VideoTitle = item.index_title, Title = txt_Name.Text, episode_id = item.episode_id,Path=DownloadHelper.downMids[item.danmaku.ToString()] });
                }
                else
                {
                    if (item.episode_status == 7)
                    {
                        ls.Add(new PlayerModel() { Aid = item.av_id, Mid = item.danmaku.ToString(), Mode = PlayMode.VipBangumi, No = item.index, VideoTitle = item.index_title, Title = txt_Name.Text, episode_id = item.episode_id });
                    }
                    else
                    {
                        ls.Add(new PlayerModel() { Aid = item.av_id, Mid = item.danmaku.ToString(), Mode = PlayMode.Video, No = item.index, VideoTitle = item.index_title, Title = txt_Name.Text, episode_id = item.episode_id });
                    }
                }
              //  i++;
            }
            int index = (gv_Play.ItemsSource as List<episodesModel>).IndexOf(info);
            MessageCenter.SendNavigateTo(NavigateMode.Play, typeof(PlayerPage), new object[] { ls, index });
            PostHistory(ls[index].Aid,ls[index].VideoTitle);

        }

        private async void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var x = new ContentDialog();
            StackPanel st = new StackPanel();
            st.Children.Add(new Image()
            {
                Source = new BitmapImage(new Uri((this.DataContext as BangumiInfoModel).cover))
            });

            x.Content = st;
            x.PrimaryButtonText = "关闭";
            x.IsPrimaryButtonEnabled = true;
            await x.ShowAsync();
        }

        private void PostHistory(string _aid,string _title)
        {
            try
            {
                if (SqlHelper.GetComicIsOnHistory(_aid))
                {
                    SqlHelper.UpdateComicHistory(new HistoryClass()
                    {
                        _aid = _aid,
                        image ="",
                        title = txt_Name.Text + " - " + _title,
                        up = "",
                        lookTime = DateTime.Now
                    });


                }
                else
                {
                    SqlHelper.AddCommicHistory(new HistoryClass()
                    {
                        _aid = _aid,
                        image ="",
                        title = txt_Name.Text+" - "+ _title,
                        up = "",
                        lookTime = DateTime.Now
                    });
                }
            }
            catch (Exception)
            {
            }
        }

        private void btn_DownManage_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DownloadPage));
        }
    }
}
