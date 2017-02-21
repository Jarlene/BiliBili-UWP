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
using BiliBili3.Models;
using System.Text.RegularExpressions;
using Windows.Media.Playback;
using Windows.Media;
using Windows.Storage.Streams;
using Windows.System.Display;
using BiliBili3.Controls;
using Windows.Graphics.Display;
using Newtonsoft.Json.Linq;
using Windows.UI.Popups;
using Windows.System;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.ApplicationModel.DataTransfer;
using BiliBili3.Helper;
using Windows.UI.Xaml.Media.Imaging;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace BiliBili3.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class VideoViewPage : Page
    {
        public VideoViewPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
            //players.FullEvent += Players_FullEvent;
            //players.MaxWIndowsEvent += Players_MaxWIndowsEvent;
            //players.PlayerEvent += Players_PlayerEvent;
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }
        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = (this.DataContext as VideoInfoModels).title;
            request.Data.Properties.Description = txt_desc.Text + "\r\n——分享自BiliBili UWP";
            request.Data.SetWebLink(new Uri("http://www.bilibili.com/video/av" + _aid));
        }

      
        string _aid;
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {


            await Task.Delay(200);
            error.Visibility = Visibility.Collapsed;
            _aid = (e.Parameter as object[])[0].ToString();
            txt_Header.Text = "AV" + _aid;
            cb_Qu.SelectedIndex = SettingHelper.Get_DownQualit() - 1;
            GetFavBox();
            pivot.SelectedIndex = 0;
            LoadVideo();
            comment.InitializeComment(1, 1, _aid);
        }



        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            pivot.SelectedIndex = 0;
            comment.DisposableComment();
            this.DataContext = null;

        }
        bool isMovie = false;
        private async void LoadVideo()
        {
            try
            {
                isMovie = false;
                tag.Children.Clear();
                pr_Load.Visibility = Visibility.Visible;
                string uri = string.Format("http://app.bilibili.com/x/view?_device=wp&_ulv=10000&access_key={0}&aid={1}&appkey=422fd9d7289a1dd9&build=411005&plat=4&platform=android&ts={2}", ApiHelper.access_key, _aid, ApiHelper.GetTimeSpan);
                uri += "&sign=" + ApiHelper.GetSign(uri);
                string results = "";
                if (SettingHelper.Get_UseCN()||SettingHelper.Get_UseHK()||SettingHelper.Get_UseTW())
                {
                    results = await WebClientClass.GetResults_Proxy(uri);
                }
                else
                {
                    results = await WebClientClass.GetResults(new Uri(uri));
                }
                
                VideoInfoModels m = JsonConvert.DeserializeObject<VideoInfoModels>(results);
                if (m.code == 0)
                {
                    this.DataContext = m.data;

                    if (m.data.movie != null)
                    {
                        //isMovie = true;

                        grid_Movie.Visibility = Visibility.Visible;
                        if (m.data.movie.movie_status == 1)
                        {
                            if (m.data.movie.pay_user.status == 0)
                            {

                                movie_pay.Visibility = Visibility.Visible;
                                txt_PayMonery.Text = m.data.movie.payment.price.ToString("0.00");
                            }
                            else
                            {
                                isMovie = true;
                                movie_pay.Visibility = Visibility.Collapsed;
                            }
                        }
                        else
                        {
                            movie_pay.Visibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        grid_Movie.Visibility = Visibility.Collapsed;
                        movie_pay.Visibility = Visibility.Collapsed;
                    }

                    //m.data.pages
                    gv_Play.SelectedIndex = 0;
                    if (m.data.req_user.attention != 1)
                    {
                        btn_AttUp.Visibility = Visibility.Visible;
                        btn_CancelAttUp.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        btn_AttUp.Visibility = Visibility.Collapsed;
                        btn_CancelAttUp.Visibility = Visibility.Visible;
                    }
                    if (m.data.season != null)
                    {
                        
                        grid_season.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        grid_season.Visibility = Visibility.Collapsed;
                    }
                    if (m.data.tag != null)
                    {
                        grid_tag.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        grid_tag.Visibility = Visibility.Collapsed;
                    }

                    if (m.data.elec != null)
                    {
                        grid_elec.Visibility = Visibility.Visible;
                        txt_NotCb.Visibility = Visibility.Collapsed;
                        grid_elec.DataContext = m.data.elec;
                    }
                    else
                    {
                        grid_elec.Visibility = Visibility.Collapsed;
                        txt_NotCb.Visibility = Visibility.Visible;
                    }
                    list_About.ItemsSource = null;
                    if (m.data.relates != null)
                    {
                        list_About.ItemsSource = m.data.relates;
                    }

                    if (m.data.tag != null)
                    {
                        foreach (var item in m.data.tag)
                        {
                            HyperlinkButton hy = new HyperlinkButton();
                            hy.Content = item.tag_name;
                            hy.Margin = new Thickness(0, 0, 10, 0);
                            hy.Foreground = App.Current.Resources["Bili-ForeColor"] as SolidColorBrush;
                            hy.Click += Hy_Click; ;
                            tag.Children.Add(hy);
                        }
                    }


                }
                else
                {
                    if (m.code == -403)
                    {
                        error.Visibility = Visibility.Visible;
                        txt_error.Text = "您的权限不足或者不支持你所在地区";
                        return;

                    }
                    if (m.code == -404)
                    {
                        error.Visibility = Visibility.Visible;
                        txt_error.Text = "视频不存在或已被删除";
                        return;
                    }

                    messShow.Show(m.message, 3000);
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
                btn_HideAll_Click(null, null);
                pr_Load.Visibility = Visibility.Collapsed;

                
            }
        }

        private void Hy_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SearchPage), new object[] { (sender as HyperlinkButton).Content.ToString() });
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }

        private void txt_desc_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (txt_desc.MaxLines == 3)
            {
                txt_desc.MaxLines = 0;
            }
            else
            {
                txt_desc.MaxLines = 3;
            }
        }

        private void gv_Play_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (gv_Play.SelectedItem!=null)
            //{
            //    var info = gv_Play.SelectedItem as pagesModel;

            //    List<PlayerModel> ls = new List<PlayerModel>();
            //    int i=0;
            //    foreach (pagesModel item in gv_Play.Items)
            //    {

            //        ls.Add(new PlayerModel() { Aid = _aid, Mid = item.cid, ImageSrc = (this.DataContext as VideoInfoModels).pic,Mode= PlayMode.Video,No= i.ToString(),VideoTitle=item.View, Title= (this.DataContext as VideoInfoModels).title });
            //        i++;
            //    }

            //  //  players.LoadPlayer(ls, gv_Play.SelectedIndex);
            //}
        }

        //private async void GetPlayUrl(string cid)
        //{
        //    string url = "http://interface.bilibili.com/playurl?_device=uwp&cid=" + cid + "&otype=xml&quality=" + 2 + "&appkey=" + ApiHelper._appKey + "&access_key=" + ApiHelper.access_key + "&type=mp4&mid=" + "" + "&_buvid="+ApiHelper._buvid+"&_hwid=" + ApiHelper._hwid+"&platform=uwp_desktop" + "&ts=" + ApiHelper.GetTimeSpan;
        //    url += "&sign=" + ApiHelper.GetSign(url);
        //    string re =await WebClientClass.GetResults_Phone(new Uri(url));
        //    re = await WebClientClass.GetResults_Phone(new Uri(url));
        //    string  playUrl = Regex.Match(re, "<url>(.*?)</url>").Groups[1].Value;
        //    playUrl = playUrl.Replace("<![CDATA[", "");
        //    playUrl = playUrl.Replace("]]>", "");
        //    mediaElement.Source = new Uri(playUrl);
        //}

        //private void mediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        //{
        //    switch (mediaElement.CurrentState)
        //    {
        //        case MediaElementState.Playing:
        //            _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Playing;
        //            break;
        //        case MediaElementState.Paused:
        //            _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Paused;
        //            break;
        //        case MediaElementState.Stopped:
        //            _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
        //            break;
        //        case MediaElementState.Closed:
        //            _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Closed;
        //            break;
        //        default:
        //            break;
        //    }
        //}

        //private  void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        //{

        //    var info= this.DataContext as VideoInfoModels;
        //    // Music metadata.
        //    if (info==null)
        //    {
        //        return;
        //    }
        //    SystemMediaTransportControlsDisplayUpdater updater = _systemMediaTransportControls.DisplayUpdater;
        //    updater.Type = MediaPlaybackType.Video;
        //    // updater.MusicProperties.AlbumArtist = info.owner.name;
        //    updater.VideoProperties.Subtitle = info.owner.name;
        //    updater.VideoProperties.Title = info.title;

        //    // Set the album art thumbnail.
        //    // RandomAccessStreamReference is defined in Windows.Storage.Streams
        //    updater.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/Logo.png"));

        //    // Update the system media transport controls.
        //    updater.Update();
        //    var timelineProperties = new SystemMediaTransportControlsTimelineProperties();

        //    // Fill in the data, using the media elements properties 
        //    timelineProperties.StartTime = TimeSpan.FromSeconds(0);
        //    timelineProperties.MinSeekTime = TimeSpan.FromSeconds(0);
        //    timelineProperties.Position = mediaElement.Position;
        //    timelineProperties.MaxSeekTime = mediaElement.NaturalDuration.TimeSpan;
        //    timelineProperties.EndTime = mediaElement.NaturalDuration.TimeSpan;

        //    // Update the System Media transport Controls 
        //    _systemMediaTransportControls.UpdateTimelineProperties(timelineProperties);
        //}

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //player.Height = player.ActualWidth * (9 / 16);
        }

        private void btn_Season_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(BanInfoPage), new object[] { (this.DataContext as VideoInfoModels).season.season_id });
        }

        private void comment_OpenUser(string id)
        {
            this.Frame.Navigate(typeof(UserInfoPage), new object[] { id });
        }

        private void btn_UP_Click(object sender, RoutedEventArgs e)
        {
            VideoInfoModels info = (sender as HyperlinkButton).DataContext as VideoInfoModels;
            this.Frame.Navigate(typeof(UserInfoPage), new object[] { info.owner.mid });
        }

        private async void btn_AttUp_Click(object sender, RoutedEventArgs e)
        {

            if (ApiHelper.IsLogin())
            {
                try
                {
                    Uri ReUri = new Uri("http://space.bilibili.com/ajax/friend/AddAttention");

                    string result = await WebClientClass.PostResults(ReUri, "mid=" + (Video_UP.DataContext as VideoInfoModels).owner.mid, "http://space.bilibili.com/");
                    JObject json = JObject.Parse(result);
                    if ((bool)json["status"])
                    {
                        messShow.Show("关注成功", 3000);
                        btn_AttUp.Visibility = Visibility.Collapsed;
                        btn_CancelAttUp.Visibility = Visibility.Visible;

                        UserManage.UpdateFollowList();
                    }
                    else
                    {
                        messShow.Show("关注失败\r\n" + result, 3000);

                    }

                }
                catch (Exception ex)
                {
                    await new MessageDialog(ex.Message, "关注时发生错误").ShowAsync();
                }
            }
            else
            {
                messShow.Show("请先登录", 3000);
            }
        }

        private async void btn_CancelAttUp_Click(object sender, RoutedEventArgs e)
        {
            if (ApiHelper.IsLogin())
            {
                try
                {
                    Uri ReUri = new Uri("http://space.bilibili.com/ajax/friend/DelAttention");

                    string result = await WebClientClass.PostResults(ReUri, "mid=" + (Video_UP.DataContext as VideoInfoModels).owner.mid, "http://space.bilibili.com/");
                    JObject json = JObject.Parse(result);
                    if ((bool)json["status"])
                    {
                        messShow.Show("已取消关注", 3000);
                        btn_AttUp.Visibility = Visibility.Visible;
                        btn_CancelAttUp.Visibility = Visibility.Collapsed;
                        UserManage.UpdateFollowList();

                    }
                    else
                    {
                        messShow.Show("取消关注失败\r\n" + result, 3000);

                    }

                }
                catch (Exception ex)
                {
                    await new MessageDialog(ex.Message, "取消关注时发生错误").ShowAsync();
                }
            }
            else
            {
                messShow.Show("请先登录", 3000);
            }
        }

        private void btn_TB_1_Click(object sender, RoutedEventArgs e)
        {
            TouBi(1);
        }

        private void btn_TB_2_Click(object sender, RoutedEventArgs e)
        {
            TouBi(2);
        }

        private void btn_No_Click(object sender, RoutedEventArgs e)
        {
            grid_Tb.Hide();
        }

        public async void TouBi(int num)
        {

            if (ApiHelper.IsLogin())
            {
                try
                {
                    WebClientClass wc = new WebClientClass();
                    Uri ReUri = new Uri("http://www.bilibili.com/plus/comment.php");
                    string QuStr = "aid=" + _aid + "&rating=100&player=1&multiply=" + num;
                    string result = await WebClientClass.PostResults(ReUri, QuStr);
                    if (result == "OK")
                    {
                        messShow.Show("投币成功！", 3000);
                    }
                    else
                    {
                        messShow.Show("投币失败！" + result, 3000);
                    }
                }
                catch (Exception ex)
                {
                    messShow.Show("投币时发生错误\r\n" + ex.Message, 3000);
                }
            }
            else
            {
                messShow.Show("请先登录!", 3000);
            }
        }

        private void Video_Refresh_Click(object sender, RoutedEventArgs e)
        {
            pivot.SelectedIndex = 0;
            LoadVideo();
            comment.InitializeComment(1, 1, _aid);
        }

        private async void btn_GoBrowser_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://www.bilibili.com/video/av" + _aid));

        }

        private async void btn_SaveImage_Click(object sender, RoutedEventArgs e)
        {
            FileSavePicker save = new FileSavePicker();
            save.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            save.FileTypeChoices.Add("图片", new List<string>() { ".jpg" });
            save.SuggestedFileName = "bili_img_" + _aid;
            StorageFile file = await save.PickSaveFileAsync();
            if (file != null)
            {
                //img_Image
                IBuffer bu = await WebClientClass.GetBuffer(new Uri((this.DataContext as VideoInfoModels).pic));
                CachedFileManager.DeferUpdates(file);
                await FileIO.WriteBufferAsync(file, bu);
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                messShow.Show("保存成功", 3000);
            }
        }

        private void btn_Share_Click(object sender, RoutedEventArgs e)
        {
            Windows.ApplicationModel.DataTransfer.DataPackage pack = new Windows.ApplicationModel.DataTransfer.DataPackage();
            pack.SetText(string.Format("我正在哔哩哔哩上看《{0}》\r\n地址：http://www.bilibili.com/video/av{1}", (this.DataContext as VideoInfoModels).title, _aid));
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(pack); // 保存 DataPackage 对象到剪切板
            Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
            messShow.Show("已将内容复制到剪切板", 3000);
        }

        private void btn_ShareData_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void btn_Favbox_Click(object sender, RoutedEventArgs e)
        {
            if (ApiHelper.IsLogin())
            {
                flyout_Favbox.ShowAt(btn_Favbox);
            }
            else
            {
                messShow.Show("请先登录", 3000);
            }
        }
        private async void GetFavBox()
        {

            if (ApiHelper.IsLogin())
            {
                try
                {
                    string results = await WebClientClass.GetResults(new Uri("http://api.bilibili.com/x/favourite/folder?jsonp=jsonp&&rnd=" + new Random().Next(1, 9999)));
                    FavboxModel model = JsonConvert.DeserializeObject<FavboxModel>(results);
                    List<FavboxModel> ban = JsonConvert.DeserializeObject<List<FavboxModel>>(model.data.ToString());
                    Video_ListView_Favbox.ItemsSource = ban;
                }
                catch (Exception ex)
                {
                    FavBox_Header.Text = "获取失败！" + ex.Message;
                }
            }
            else
            {
                FavBox_Header.Text = "请先登录！";
                Video_ListView_Favbox.IsEnabled = false;
            }
        }
        private async void Video_ListView_Favbox_ItemClick(object sender, ItemClickEventArgs e)
        {

            if (ApiHelper.IsLogin())
            {
                try
                {
                    WebClientClass wc = new WebClientClass();
                    Uri ReUri = new Uri("http://api.bilibili.com/x/favourite/video/add");

                    string QuStr = "jsonp=jsonp&fid=" + ((FavboxModel)e.ClickedItem).fid + "&aid=" + _aid;

                    string result = await WebClientClass.PostResults(ReUri, QuStr);
                    JObject json = JObject.Parse(result);
                    if ((int)json["code"] == 0)
                    {
                        messShow.Show("收藏成功！", 2000);
                        GetFavBox();
                    }
                    else
                    {
                        if ((int)json["code"] == 11007)
                        {
                            messShow.Show("视频已经收藏！", 2000);
                            //MessageDialog md = new MessageDialog("视频已经收藏！");
                            //await md.ShowAsync();
                        }
                        else
                        {
                            messShow.Show("收藏失败！\r\n" + result, 2000);
                        }
                    }
                }
                catch (Exception ex)
                {
                    messShow.Show("收藏失败！" + ex.Message, 2000);
                }
            }
            else
            {
                messShow.Show("请先登录！", 2000);
            }
        }

        private void btn_CD_Click(object sender, RoutedEventArgs e)
        {
            messShow.Show("帅气的橙子说充电功能还在开发中\r\n请使用打开为UP充电！", 2000);
        }

        private void list_About_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(VideoViewPage), new object[] { (e.ClickedItem as relatesModel).aid });
        }

        private async void btn_Download_Click(object sender, RoutedEventArgs e)
        {

            if (gv_Play.Items.Count != 0)
            {
                if (gv_Play.Items.Count == 1)
                {
                    var info = gv_Play.SelectedItem as pagesModel;

                    List<PlayerModel> ls = new List<PlayerModel>();
                    int i = 1;
                    foreach (pagesModel item in gv_Play.Items)
                    {
                        if (item.IsDowned == Visibility.Collapsed)
                        {

                            switch (item.from)
                            {
                                case "sohu":
                                    ls.Add(new PlayerModel() { Aid = _aid, Mid = item.cid, rich_vid = item.rich_vid, ImageSrc = (this.DataContext as VideoInfoModels).pic, Mode = PlayMode.Sohu, No = i.ToString(), VideoTitle = item.View, Title = (this.DataContext as VideoInfoModels).title });
                                    break;
                                default:
                                    ls.Add(new PlayerModel() { Aid = _aid, Mid = item.cid, ImageSrc = (this.DataContext as VideoInfoModels).pic, Mode = PlayMode.Video, No = i.ToString(), VideoTitle = item.View, Title = (this.DataContext as VideoInfoModels).title });
                                    break;
                            }


                           
                        }
                        i++;
                    }
                    DownloadModel m = new DownloadModel();
                    m.folderinfo = new FolderListModel()
                    {
                        id = _aid,
                        desc = txt_desc.Text,
                        title = txt_title.Text,
                        isbangumi = false,
                        thumb = (this.DataContext as VideoInfoModels).pic

                    };
                    m.videoinfo = new VideoListModel()
                    {
                        id = _aid,
                        mid = ls[0].Mid,
                        part = Convert.ToInt32(ls[0].No),
                        partTitle = ls[0].VideoTitle,
                        videoUrl = await ApiHelper.GetVideoUrl(ls[0], cb_Qu.SelectedIndex + 1),
                        title = txt_title.Text
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

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            gv_Play.SelectionMode = ListViewSelectionMode.None;
            gv_Play.IsItemClickEnabled = true;
            Down_ComBar.Visibility = Visibility.Collapsed;
            com_bar.Visibility = Visibility.Visible;

        }

        private async void btn_Ok_Click(object sender, RoutedEventArgs e)
        {
            if (gv_Play.SelectedItems.Count == 0)
            {
                return;
            }
            var info = gv_Play.SelectedItem as pagesModel;

            int i = 1;
            foreach (pagesModel item in gv_Play.SelectedItems)
            {
                if (item.IsDowned == Visibility.Collapsed)
                {
                    PlayerModel vitem = null; 
                    switch (item.from)
                    {
                        case "sohu":

                            vitem = new PlayerModel() { Aid = _aid, Mid = item.cid,rich_vid=item.rich_vid, ImageSrc = (this.DataContext as VideoInfoModels).pic, Mode = PlayMode.Sohu, No = i.ToString(), VideoTitle = item.View, Title = (this.DataContext as VideoInfoModels).title };
                            break;
                        default:

                            vitem = new PlayerModel() { Aid = _aid, Mid = item.cid, ImageSrc = (this.DataContext as VideoInfoModels).pic, Mode = PlayMode.Video, No = i.ToString(), VideoTitle = item.View, Title = (this.DataContext as VideoInfoModels).title };
                            break;
                    }


                    DownloadModel m = new DownloadModel();
                    m.folderinfo = new FolderListModel()
                    {
                        id = _aid,
                        desc = txt_desc.Text,
                        title = txt_title.Text,
                        isbangumi = false,
                        thumb = (this.DataContext as VideoInfoModels).pic

                    };
                    m.videoinfo = new VideoListModel()
                    {
                        id = _aid,
                        mid = vitem.Mid,
                        part = Convert.ToInt32(vitem.No),
                        partTitle = vitem.VideoTitle,
                        videoUrl = await ApiHelper.GetVideoUrl(vitem, cb_Qu.SelectedIndex + 1),
                        title = txt_title.Text
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

        private void btn_HideAll_Click(object sender, RoutedEventArgs e)
        {
            btn_HideAll.Visibility = Visibility.Collapsed;
            btn_ShowAll.Visibility = Visibility.Visible;
            if (gv_Play.Items.Count > 3)
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

        private void gv_Play_ItemClick(object sender, ItemClickEventArgs e)
        {
            var info = e.ClickedItem as pagesModel;

            List<PlayerModel> ls = new List<PlayerModel>();
            //int i = 0;
            foreach (pagesModel item in gv_Play.Items)
            {
                if (item.IsDowned == Visibility.Collapsed)
                {
                    if (isMovie)
                    {
                        ls.Add(new PlayerModel() { Aid = _aid, Mid = item.cid, ImageSrc = (this.DataContext as VideoInfoModels).pic, Mode = PlayMode.Movie, No = "", VideoTitle = item.View, Title = (this.DataContext as VideoInfoModels).title });
                    }
                    else
                    {
                        switch (item.from)
                        {
                            case "sohu":
                                ls.Add(new PlayerModel() { Aid = _aid, Mid = item.cid, rich_vid = item.rich_vid, ImageSrc = (this.DataContext as VideoInfoModels).pic, Mode = PlayMode.Sohu, No = "", VideoTitle = item.View, Title = (this.DataContext as VideoInfoModels).title });
                                break;
                            default:
                                ls.Add(new PlayerModel() { Aid = _aid, Mid = item.cid, ImageSrc = (this.DataContext as VideoInfoModels).pic, Mode = PlayMode.Video, No = "", VideoTitle = item.View, Title = (this.DataContext as VideoInfoModels).title });
                                break;
                        }
                      
                    }
                }
                else
                {

                    ls.Add(new PlayerModel() { Aid = _aid, Mid = item.cid, ImageSrc = (this.DataContext as VideoInfoModels).pic, Mode = PlayMode.Local, No = "", VideoTitle = item.View, Title = (this.DataContext as VideoInfoModels).title, Path = DownloadHelper.downMids[item.cid] });




                }
            }

            MessageCenter.SendNavigateTo(NavigateMode.Play, typeof(PlayerPage), new object[] { ls, (gv_Play.ItemsSource as List<pagesModel>).IndexOf(info) });
            PostHistory();

        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (pivot.SelectedIndex == 0)
            {
                com_bar.Visibility = Visibility.Visible;
            }
            else
            {
                com_bar.Visibility = Visibility.Collapsed;
            }
        }

        private async void img_pic_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var x = new ContentDialog();
            StackPanel st = new StackPanel();
            st.Children.Add(new Image()
            {
                Source = new BitmapImage(new Uri((this.DataContext as VideoInfoModels).pic))
            });

            x.Content = st;
            x.PrimaryButtonText = "关闭";
            x.IsPrimaryButtonEnabled = true;
            x.SecondaryButtonText = "保存";
            x.IsSecondaryButtonEnabled = true;
            x.SecondaryButtonClick += X_SecondaryButtonClick;
            await x.ShowAsync();
        }

        private void X_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            btn_SaveImage_Click(sender, null);
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            btn_Play_Click(sender, null);
        }

        private void btn_Play_Click(object sender, RoutedEventArgs e)
        {
            if (gv_Play.Items.Count != 0)
            {
                var info = gv_Play.Items[0] as pagesModel;

                List<PlayerModel> ls = new List<PlayerModel>();
                //int i = 0;
                foreach (pagesModel item in gv_Play.Items)
                {
                    if (item.IsDowned == Visibility.Collapsed)
                    {
                       // ls.Add(new PlayerModel() { Aid = _aid, Mid = item.cid, ImageSrc = (this.DataContext as VideoInfoModels).pic, Mode = PlayMode.Video, No = "", VideoTitle = item.View, Title = (this.DataContext as VideoInfoModels).title });

                        if (isMovie)
                        {
                            ls.Add(new PlayerModel() { Aid = _aid, Mid = item.cid, ImageSrc = (this.DataContext as VideoInfoModels).pic, Mode = PlayMode.Movie, No = "", VideoTitle = item.View, Title = (this.DataContext as VideoInfoModels).title });
                        }
                        else
                        {
                            switch (item.from)
                            {
                                case "sohu":
                                    ls.Add(new PlayerModel() { Aid = _aid, Mid = item.cid, rich_vid = item.rich_vid, ImageSrc = (this.DataContext as VideoInfoModels).pic, Mode = PlayMode.Sohu, No = "", VideoTitle = item.View, Title = (this.DataContext as VideoInfoModels).title });
                                    break;
                                default:
                                    ls.Add(new PlayerModel() { Aid = _aid, Mid = item.cid, ImageSrc = (this.DataContext as VideoInfoModels).pic, Mode = PlayMode.Video, No = "", VideoTitle = item.View, Title = (this.DataContext as VideoInfoModels).title });
                                    break;
                            }

                        }
                    }
                    else
                    {
                        ls.Add(new PlayerModel() { Aid = _aid, Mid = item.cid, ImageSrc = (this.DataContext as VideoInfoModels).pic, Mode = PlayMode.Local, No = "", VideoTitle = item.View, Title = (this.DataContext as VideoInfoModels).title, Path = DownloadHelper.downMids[item.cid] });
                    }
                }

                MessageCenter.SendNavigateTo(NavigateMode.Play, typeof(PlayerPage), new object[] { ls, (gv_Play.ItemsSource as List<pagesModel>).IndexOf(info) });
                PostHistory();
            }
        }

        private void btn_movie_activity_Click(object sender, RoutedEventArgs e)
        {

        }



        private  void PostHistory()
        {
            try
            {
                if (SqlHelper.GetComicIsOnHistory(_aid))
                {
                    SqlHelper.UpdateComicHistory(new HistoryClass() {
                        _aid=_aid,
                        image= (this.DataContext as VideoInfoModels).pic,
                        title=txt_title.Text,
                        up= (Video_UP.DataContext as VideoInfoModels).owner.name,
                        lookTime=DateTime.Now
                    });


                }
                else
                {
                    SqlHelper.AddCommicHistory(new HistoryClass()
                    {
                        _aid = _aid,
                        image = (this.DataContext as VideoInfoModels).pic,
                        title = txt_title.Text,
                        up = (Video_UP.DataContext as VideoInfoModels).owner.name,
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

        private void btn_Pin_Click(object sender, RoutedEventArgs e)
        {
            //SettingHelper.PinTile(_aid, _aid, txt_title.Text, (this.DataContext as VideoInfoModels).pic);
        }
    }
}
