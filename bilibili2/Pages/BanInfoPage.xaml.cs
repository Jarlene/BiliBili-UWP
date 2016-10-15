using bilibili2.Class;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;
using Windows.UI.Text;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace bilibili2.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class BanInfoPage : Page
    {
        public delegate void GoBackHandler();
        public event GoBackHandler BackEvent;
        public BanInfoPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            request.Data.Properties.Title = "【番剧】" + txt_Name.Text;
            request.Data.Properties.Description = txt_Desc.Text + "\r\n——分享自BiliBili UWP";
            request.Data.SetWebLink(new Uri("http://bangumi.bilibili.com/anime/" + banID));
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

        WebClientClass wc;
        string banID = "";
        bool Back = false;
        //bool IsBan = false;
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            bg.Color = ((SolidColorBrush)this.Frame.Tag).Color;
            GetSetting();
            if (e.NavigationMode == NavigationMode.New || Back)
            {
                await Task.Delay(200);
                font_icon.Glyph = "\uE006";
                btn_concern.Label = "订阅";
                banID = e.Parameter as string;
                //IsBan = ((KeyValuePair<string, bool>)e.Parameter).Value;
                pivot.SelectedIndex = 0;
                list_Rank.ItemsSource = null;
                cb_H.ItemsSource = null;
                //cb_H.SelectedIndex
                GetBangumiInfo(banID);
            }
            else
            {
                SetViewState();
            }
        }
        bool useHkIp = false;
        bool userTwIp = false;
        bool userDlIp = false;
        private void GetSetting()
        {
            SettingHelper setting = new SettingHelper();
            //UseTW,UseHK,UseCN
            if (setting.SettingContains("UseTW"))
            {
                userTwIp = (bool)setting.GetSettingValue("UseTW");
            }
            else
            {
                userTwIp = false;
            }
            if (setting.SettingContains("UseHK"))
            {
                useHkIp = (bool)setting.GetSettingValue("UseHK");
            }
            else
            {
                useHkIp = false;
            }
            if (setting.SettingContains("UseCN"))
            {
                userDlIp = (bool)setting.GetSettingValue("UseCN");
            }
            else
            {
                userDlIp = false;
            }
        }
        //public async void SaveHiss(string id, string title, string type)
        //{
        //    try
        //    {
        //        XmlDocument HisDoc = new XmlDocument();
        //        StorageFolder folder = ApplicationData.Current.LocalFolder;
        //        StorageFile xmlfile = await folder.CreateFileAsync("History.xml", CreationCollisionOption.OpenIfExists);
        //        string results = await FileIO.ReadTextAsync(xmlfile);
        //        if (results == "")
        //        {
        //            await FileIO.WriteTextAsync(xmlfile, @"<History></History>");
        //            results = @"<History></History>";
        //        }
        //        HisDoc.LoadXml(results);
        //        XmlElement el = HisDoc.DocumentElement;
        //        XmlElement x = HisDoc.CreateElement("info");
        //        x.SetAttribute("p", id);
        //        x.SetAttribute("type", type);
        //        x.SetAttribute("date", DateTime.Now.ToString());
        //        x.SetAttribute("title", title);
        //        el.AppendChild(x);
        //        await FileIO.WriteTextAsync(xmlfile, HisDoc.InnerXml);
        //    }
        //    catch (Exception)
        //    {
        //    }

        //}

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            wc = null;

        }
        public async void GetBangumiInfo(string banID)
        {
            //string uri = "http://bangumi.bilibili.com/api/season?_device=wp&_ulv=10000&build=411005&platform=android&appkey=422fd9d7289a1dd9&ts="+APIHelper.GetTimeSpen+ "000&type=sp&sp_id=56719";
            //string sign=  APIHelper.GetSign(uri);
            //uri += "&sign=" + sign;
            try
            {
                pr_load.Visibility = Visibility.Visible;
                wc = new WebClientClass();
                string uri = "";
                uri = string.Format("http://bangumi.bilibili.com/api/season_v3?_device=wp&access_key={2}&_ulv=10000&build=411005&platform=android&appkey=422fd9d7289a1dd9&ts={0}000&type=bangumi&season_id={1}", ApiHelper.GetTimeSpen, banID, ApiHelper.access_key);
                uri += "&sign=" + ApiHelper.GetSign(uri);
                string area = "";
                string result = "";
                if (useHkIp)
                {
                    area = "hk";
                }
                if (userTwIp)
                {
                    area = "tw";
                }
                if (userDlIp)
                {
                    area = "cn";
                }
                if (!userDlIp && !userTwIp && !useHkIp)
                {
                    result = await wc.GetResults(new Uri(uri));
                }
                else
                {
                    string re= await wc.GetResults(new Uri("http://52uwp.com/api/BiliBili?area=" + area + "&url=" + Uri.EscapeDataString(uri)));
                    MessageModel ms = JsonConvert.DeserializeObject<MessageModel>(re);
                    if (ms.code == 0)
                    {
                        result = ms.message;
                    }
                    if (ms.code == -100)
                    {
                        await new MessageDialog("远程代理失效，请联系开发者更新！").ShowAsync();
                    }
                    if (ms.code == -200)
                    {
                        await new MessageDialog("代理读取信息失败，请重试！").ShowAsync();
                        
                    }
                }
                BangumiInfoModel model = new BangumiInfoModel();
                if ((int)JObject.Parse(result)["code"] == 0)
                {

                    model = JsonConvert.DeserializeObject<BangumiInfoModel>(JObject.Parse(result)["result"].ToString());
                    grid_Info.DataContext = model;
                    BangumiInfoModel m = JsonConvert.DeserializeObject<BangumiInfoModel>(model.user_season.ToString());
                    if (m.attention == 0)
                    {
                        font_icon.Glyph = "\uE006";
                        btn_concern.Label = "订阅";
                    }
                    else
                    {
                        font_icon.Glyph = "\uE00B";
                        btn_concern.Label = "取消订阅";
                    }
                    if (model.rank != null)
                    {
                        BangumiInfoModel rank = JsonConvert.DeserializeObject<BangumiInfoModel>(model.rank.ToString());
                        grid_Cb.DataContext = rank;
                        grid_Cb.Visibility = Visibility.Visible;
                        txt_NotCb.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        txt_NotCb.Visibility = Visibility.Visible;
                        grid_Cb.Visibility = Visibility.Collapsed;
                    }

                    if (model.seasons != null)
                    {
                        Grid_About.Visibility = Visibility.Visible;
                        List<BangumiInfoModel> seasons = JsonConvert.DeserializeObject<List<BangumiInfoModel>>(model.seasons.ToString());
                        WrapPanel_About.Children.Clear();
                        if (seasons.Count==1)
                        {
                            Grid_About.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            foreach (BangumiInfoModel item in seasons)
                            {

                                HyperlinkButton btn = new HyperlinkButton();
                                btn.DataContext = item;
                                btn.Margin = new Thickness(0, 0, 10, 0);
                                btn.Content = item.title;
                                btn.Foreground = App.Current.Resources["Bili-ForeColor"] as SolidColorBrush;
                                if (item.season_id == banID)
                                {
                                    btn.IsEnabled = false;
                                }
                                btn.Click += Btn_Click1;
                                WrapPanel_About.Children.Add(btn);

                            }
                        }
                       
                        //Grid_About
                    }
                    else
                    {
                        Grid_About.Visibility = Visibility.Collapsed;
                    }

                    SqlHelper sql = new SqlHelper();
                    List<BangumiInfoModel> list = JsonConvert.DeserializeObject<List<BangumiInfoModel>>(model.episodes.ToString());
                    List<BangumiInfoModel> list2 = new List<BangumiInfoModel>();
                    for (int i = 0; i < list.Count; i++)
                    {
                        list[i].Num = i;
                        if (sql.ValuesExists(list[i].danmaku.ToString()))
                        {
                            list[i].color = new SolidColorBrush() { Color = Colors.Gray };
                        }
                        else
                        {
                            list[i].color = new SolidColorBrush() { Color = Colors.White };

                        }
                        if (DownloadManage.Downloaded.Contains(list[i].danmaku.ToString()))
                        {
                            list[i].inLocal = true;
                        }
                        list2.Add(list[i]);
                    }
                    list_E.ItemsSource = list2;
                    cb_H.ItemsSource = list2;

                    List<BangumiInfoModel> list_CV = JsonConvert.DeserializeObject<List<BangumiInfoModel>>(model.actor.ToString());
                    txt_CV.Text = "";
                    foreach (BangumiInfoModel item in list_CV)
                    {
                        txt_CV.Text += string.Format("{0}:{1}\r\n", item.role, item.actor);
                    }
                    List<BangumiInfoModel> list_Tag = JsonConvert.DeserializeObject<List<BangumiInfoModel>>(model.tags.ToString());
                    Grid_tag.Children.Clear();
                    foreach (BangumiInfoModel item in list_Tag)
                    {
                        HyperlinkButton btn = new HyperlinkButton();
                        btn.DataContext = item;
                        btn.Margin = new Thickness(0, 0, 10, 0);
                        btn.Content = item.tag_name;
                        btn.Foreground = App.Current.Resources["Bili-ForeColor"] as SolidColorBrush;
                        btn.Click += Btn_Click;
                        Grid_tag.Children.Add(btn);
                    }
                    if (list_E.Items.Count != 0)
                    {
                        GetVideoComment_Hot(list2[0].av_id);
                        cb_H.SelectedIndex = list2.Count - 1;
                    }
                }
                if ((int)JObject.Parse(result)["code"] == -3)
                {
                    messShow.Show("密钥注册失败，请联系作者", 3000);
                }
                if ((int)JObject.Parse(result)["code"] == 10)
                {
                    messShow.Show(JObject.Parse(result)["message"].ToString(), 3000);
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
                pr_load.Visibility = Visibility.Collapsed;

            }
        }

        private void Btn_Click1(object sender, RoutedEventArgs e)
        {
            Back = true;
            this.Frame.Navigate(typeof(BanInfoPage), ((sender as HyperlinkButton).DataContext as BangumiInfoModel).season_id);
        }

        private void SetViewState()
        {
            SqlHelper sql = new SqlHelper();
            foreach (var item in list_E.ItemsSource as List<BangumiInfoModel>)
            {
                if (sql.ValuesExists(item.danmaku.ToString()))
                {
                    item.color = new SolidColorBrush() { Color = Colors.Gray };
                }
                else
                {
                    item.color = new SolidColorBrush() { Color = Colors.White };

                }
            }
        }
        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            string tid = ((sender as HyperlinkButton).DataContext as BangumiInfoModel).tag_id.ToString();
            string name = ((sender as HyperlinkButton).DataContext as BangumiInfoModel).tag_name.ToString();
            if (tid != null)
            {
                this.Frame.Navigate(typeof(BanByTagPage), new string[] { tid, name });
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            bor_Width.Width = Width / 3;
        }
        private SettingHelper settings = new SettingHelper();
        private void grid_E_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (cb_IsPlay.IsChecked.Value)
            {
                BangumiInfoModel model = e.ClickedItem as BangumiInfoModel;
                if (model.inLocal)
                {
                    if ((bool)settings.GetSettingValue("PlayLocal"))
                    {
                        this.Frame.Navigate(typeof(DownloadPage), 1);
                        return;
                    }
                }
                List<VideoModel> listVideo = new List<VideoModel>();
                List<BangumiInfoModel> ls = ((List<BangumiInfoModel>)list_E.ItemsSource).OrderByDescending(s => Convert.ToDouble(s.Num)).ToList();
                foreach (BangumiInfoModel item in ls)
                {
                    listVideo.Add(new VideoModel() { aid = item.av_id, title = txt_Name.Text, cid = item.danmaku.ToString(), page = item.index, part = item.index_title ?? "" });
                }
                KeyValuePair<List<VideoModel>, int> list = new KeyValuePair<List<VideoModel>, int>(listVideo, ls.IndexOf(e.ClickedItem as BangumiInfoModel));
                PostHistory(model.av_id);
                this.Frame.Navigate(typeof(PlayerPage), list);
            }
            else
            {
                this.Frame.Navigate(typeof(VideoInfoPage), (e.ClickedItem as BangumiInfoModel).av_id);
            }
        }
        private async void PostHistory(string aid)
        {
            try
            {
                WebClientClass wc = new WebClientClass();
                string url = string.Format("http://api.bilibili.com/x/history/add?_device=wp&_ulv=10000&access_key={0}&appkey={1}&build=411005&platform=android", ApiHelper.access_key, ApiHelper._appKey);
                url += "&sign=" + ApiHelper.GetSign(url);
                string result = await wc.PostResults(new Uri(url), "aid=" + aid);
            }
            catch (Exception)
            {
            }
        }
        //private async void btn_OK_Click(object sender, RoutedEventArgs e)
        //{
        //    using (DownMangentClass wc = new DownMangentClass())
        //    {
        //        if (list_E.SelectedItems.Count != 0)
        //        {
        //            //循环读取全部选中的项目
        //            foreach (BangumiInfoModel item in list_E.SelectedItems)
        //            {
        //                int quality = cb_Qu.SelectedIndex + 1;//清晰度1-3
        //                string Downurl = await wc.GetVideoUri(item.danmaku.ToString(), quality);//取得视频URL
        //                if (Downurl != null)
        //                {
        //                    DownMangentClass.DownModel model = new DownMangentClass.DownModel()
        //                    {
        //                        mid = item.danmaku.ToString(),
        //                        title = (grid_Info.DataContext as BangumiInfoModel).title,
        //                        part = item.index,
        //                        url = Downurl,
        //                        aid = item.av_id,
        //                        danmuUrl = "http://comment.bilibili.com/" + item.danmaku + ".xml",
        //                        quality = quality,
        //                        downloaded = false,
        //                        partTitle = item.index_title ?? item.index
        //                    };
        //                    wc.StartDownload(model);
        //                    //StartDownload(model);
        //                }
        //                else
        //                {
        //                    MessageDialog md = new MessageDialog(item.title + "\t视频地址获取失败");
        //                    await md.ShowAsync();
        //                }
        //            }
        //            grid_GG.Visibility = Visibility.Visible;
        //            txt_GG.Text = "任务加入下载队列";
        //            list_E.SelectionMode = ListViewSelectionMode.None;
        //            list_E.IsItemClickEnabled = true;
        //            com_bar.Visibility = Visibility.Visible;
        //            com_bar_Down.Visibility = Visibility.Collapsed;
        //            await Task.Delay(2000);
        //            grid_GG.Visibility = Visibility.Collapsed;
        //        }
        //        else
        //        {
        //            list_E.SelectionMode = ListViewSelectionMode.None;
        //            list_E.IsItemClickEnabled = true;
        //            com_bar.Visibility = Visibility.Visible;
        //            com_bar_Down.Visibility = Visibility.Collapsed;
        //        }
        //    }
        //}

        private void btn_Down_Click(object sender, RoutedEventArgs e)
        {
            com_bar.Visibility = Visibility.Collapsed;
            com_bar_Down.Visibility = Visibility.Visible;
            list_E.SelectionMode = ListViewSelectionMode.Multiple;
            list_E.IsItemClickEnabled = false;
        }
        private void btn_ALL_Click(object sender, RoutedEventArgs e)
        {
            if (list_E.SelectedItems.Count == list_E.Items.Count)
            {
                list_E.SelectedItems.Clear();
            }
            else
            {
                list_E.SelectAll();
            }
        }
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            com_bar.Visibility = Visibility.Visible;
            com_bar_Down.Visibility = Visibility.Collapsed;
            list_E.SelectionMode = ListViewSelectionMode.None;
            list_E.IsItemClickEnabled = true;
        }

        private void btn_More_Click(object sender, RoutedEventArgs e)
        {
            if (txt_Desc.MaxLines == 3)
            {
                txt_Desc.MaxLines = 0;
                btn_More.Content = "收缩";
            }
            else
            {
                txt_Desc.MaxLines = 3;
                btn_More.Content = "展开";
            }
        }

        private void btn_More__Click(object sender, RoutedEventArgs e)
        {
            if (txt_CV.MaxLines == 3)
            {
                txt_CV.MaxLines = 0;
                btn_More.Content = "收缩";
            }
            else
            {
                txt_CV.MaxLines = 3;
                btn_More.Content = "展开";
            }
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            GetBangumiInfo(banID);
        }

        private async void btn_concern_Click(object sender, RoutedEventArgs e)
        {
            UserClass getLogin = new UserClass();
            wc = new WebClientClass();
            if (getLogin.IsLogin())
            {
                try
                {
                    if (btn_concern.Label == "订阅")
                    {
                        //http://www.bilibili.com/api_proxy?app=bangumi&action=/concern_season&season_id=779
                        string results = await wc.GetResults(new Uri("http://www.bilibili.com/api_proxy?app=bangumi&action=/concern_season&season_id=" + banID));
                        JObject json = JObject.Parse(results);
                        if ((int)json["code"] == 0)
                        {
                            font_icon.Glyph = "\uE00B";
                            btn_concern.Label = "取消订阅";
                            messShow.Show("订阅成功!", 3000);
                            //MessageDialog md = new MessageDialog("订阅成功！");
                            //  await md.ShowAsync();

                        }
                        else
                        {
                            messShow.Show("订阅失败!", 3000);
                        }
                    }
                    else
                    {
                        //http://www.bilibili.com/api_proxy?app=bangumi&action=/concern_season&season_id=779

                        string results = await wc.GetResults(new Uri("http://www.bilibili.com/api_proxy?app=bangumi&action=/unconcern_season&season_id=" + banID));
                        JObject json = JObject.Parse(results);
                        if ((int)json["code"] == 0)
                        {
                            font_icon.Glyph = "\uE006";
                            btn_concern.Label = "订阅";
                            messShow.Show("取消订阅成功!", 3000);

                        }
                        else
                        {
                            messShow.Show("取消订阅失败!", 3000);
                        }
                    }
                }
                catch (Exception)
                {
                    MessageDialog md = new MessageDialog("订阅操作失败！");

                    await md.ShowAsync();
                }
            }
            else
            {
                MessageDialog md = new MessageDialog("先登录好伐", "(´・ω・`) ");
                await md.ShowAsync();
            }
        }

        private async Task<bool> GetIsConcern(string sid)
        {
            try
            {
                string results = await wc.GetResults(new Uri("http://www.bilibili.com/api_proxy?app=bangumi&action=/user_season_status&season_id=" + sid + new Random().Next(1, 9999)));

                JObject json = JObject.Parse(results);
                if ((int)json["result"]["attention"] == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }

        private void Share_Click(object sender, RoutedEventArgs e)
        {
            Windows.ApplicationModel.DataTransfer.DataPackage pack = new Windows.ApplicationModel.DataTransfer.DataPackage();
            pack.SetText(string.Format("我正在BiliBili追{0},一起来看吧\r\n地址：http://bangumi.bilibili.com/anime/{1}", txt_Name.Text, banID));
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(pack); // 保存 DataPackage 对象到剪切板
            Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
            messShow.Show("已将内容复制到剪切板", 3000);
        }

        private void btn_Share_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private async void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            using (DownloadManage wc = new DownloadManage())
            {
                if (list_E.SelectedItems.Count != 0)
                {
                    //循环读取全部选中的项目
                    foreach (BangumiInfoModel item in list_E.SelectedItems)
                    {
                        int quality = cb_Qu.SelectedIndex + 1;//清晰度1-3
                        string Downurl = await wc.GetVideoUri(item.danmaku.ToString(), quality);//取得视频URL
                        if (Downurl != null)
                        {
                            DownloadManage.DownModel model = new DownloadManage.DownModel()
                            {
                                mid = item.danmaku.ToString(),
                                title = "【番剧】" + txt_Name.Text,
                                part = item.index,
                                url = Downurl,
                                aid = banID,
                                danmuUrl = "http://comment.bilibili.com/" + item.danmaku + ".xml",
                                quality = quality,
                                downloaded = false,
                                partTitle = item.index_title ?? "",
                                isBangumi = true
                            };
                            wc.StartDownload(model);
                            //StartDownload(model);
                        }
                        else
                        {
                            MessageDialog md = new MessageDialog(item.title + "\t视频地址获取失败");
                            await md.ShowAsync();
                        }
                    }
                    messShow.Show("任务已加入下载队列", 3000);
                    list_E.SelectionMode = ListViewSelectionMode.None;
                    list_E.IsItemClickEnabled = true;
                    com_bar_Down.Visibility = Visibility.Collapsed;
                    com_bar.Visibility = Visibility.Visible;
                }
                else
                {
                    list_E.SelectionMode = ListViewSelectionMode.None;
                    list_E.IsItemClickEnabled = true;
                    com_bar_Down.Visibility = Visibility.Collapsed;
                    com_bar.Visibility = Visibility.Visible;
                }
            }
        }

       
   
        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            txt_her_0.FontWeight = FontWeights.Normal;
            txt_her_1.FontWeight = FontWeights.Normal;
            txt_her_3.FontWeight = FontWeights.Normal;
            switch (pivot.SelectedIndex)
            {
                case 0:
                    txt_her_0.FontWeight = FontWeights.Bold;
                    break;
                case 1:
                    txt_her_1.FontWeight = FontWeights.Bold;
                  
                    break;
                case 2:
                    txt_her_3.FontWeight = FontWeights.Bold;
                    break;
               
                default:
                    break;
            }

        }

        private void btn_DownManage_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(DownloadPage));
        }

        private void btn_Play_Click(object sender, RoutedEventArgs e)
        {
            if (list_E.Items.Count <= 0)
            {
                messShow.Show("没有内容...无法播放...", 3000);
                return;
            }
            BangumiInfoModel model = (BangumiInfoModel)list_E.Items[0];
            if (model.inLocal)
            {
                if ((bool)settings.GetSettingValue("PlayLocal"))
                {
                    this.Frame.Navigate(typeof(DownloadPage), 1);
                    return;
                }
            }
            List<VideoModel> listVideo = new List<VideoModel>();
            List<BangumiInfoModel> ls = ((List<BangumiInfoModel>)list_E.ItemsSource).OrderByDescending(s => Convert.ToDouble(s.Num)).ToList();
            foreach (BangumiInfoModel item in ls)
            {
                listVideo.Add(new VideoModel() { aid = item.av_id, title = txt_Name.Text, cid = item.danmaku.ToString(), page = item.index, part = item.index_title ?? "" });
            }
            KeyValuePair<List<VideoModel>, int> list = new KeyValuePair<List<VideoModel>, int>(listVideo, 0);
            PostHistory(model.av_id);
            this.Frame.Navigate(typeof(PlayerPage), list);
        }



        private async void GetRankInfo()
        {
            try
            {
                pr_load.Visibility = Visibility.Visible;
                wc = new WebClientClass();
                string url = string.Empty;
                if (cb_Rank.SelectedIndex == 0)
                {
                    url = string.Format("http://bangumi.bilibili.com/sponsor/rank/get_sponsor_week_list?access_key={0}&appkey={1}&build=418000&mobi_app=android&page=1&pagesize=25&platform=android&season_id={2}&ts={3}", ApiHelper.access_key, ApiHelper._appKey_Android, banID, ApiHelper.GetTimeSpen);
                }
                else
                {
                    url = string.Format("http://bangumi.bilibili.com/sponsor/rank/get_sponsor_total?access_key={0}&appkey={1}&build=418000&mobi_app=android&page=1&pagesize=25&platform=android&season_id={2}&ts={3}", ApiHelper.access_key, ApiHelper._appKey_Android, banID, ApiHelper.GetTimeSpen);
                }
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                string results = await wc.GetResults(new Uri(url));
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
                pr_load.Visibility = Visibility.Collapsed;
            }
        }


        private void list_Rank_ItemClick(object sender, ItemClickEventArgs e)
        {
            CBRankModel m = e.ClickedItem as CBRankModel;
            if (m.uid.Length != 0)
            {
                this.Frame.Navigate(typeof(UserInfoPage), m.uid);
            }

        }

        private void list_Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
        private async void GetVideoComment_Hot(string aid)
        {
            try
            {
                pr_load.Visibility = Visibility.Visible;
                ListView_Comment_Hot.Items.Clear();
                WebClientClass wc = new WebClientClass();
                Random r = new Random();
                string results = await wc.GetResults(new Uri("http://api.bilibili.com/x/reply?jsonp=jsonp&type=1&sort=" + 2 + "&oid=" + aid + "&pn=" + 1 + "&nohot=1&ps=5&r=" + r.Next(1000, 99999)));
                CommentModel model = JsonConvert.DeserializeObject<CommentModel>(results);
                CommentModel model3 = JsonConvert.DeserializeObject<CommentModel>(model.data.ToString());
                //Video_Grid_Info.DataContext = model;
                List<CommentModel> ban = JsonConvert.DeserializeObject<List<CommentModel>>(model3.replies.ToString());
                foreach (CommentModel item in ban)
                {
                    CommentModel model1 = JsonConvert.DeserializeObject<CommentModel>(item.member.ToString());
                    CommentModel model2 = JsonConvert.DeserializeObject<CommentModel>(item.content.ToString());
                    CommentModel modelLV = JsonConvert.DeserializeObject<CommentModel>(model1.level_info.ToString());
                    CommentModel resultsModel = new CommentModel()
                    {
                        avatar = model1.avatar,
                        message = model2.message,
                        plat = model2.plat,
                        floor = item.floor,
                        uname = model1.uname,
                        mid = model1.mid,
                        ctime = item.ctime,
                        like = item.like,
                        rcount = item.rcount,
                        rpid = item.rpid,
                        current_level = modelLV.current_level
                    };
                    ListView_Comment_Hot.Items.Add(resultsModel);
                }
        }
            catch (Exception ex)
            {
                messShow.Show("读取热门评论失败!\r\n" + ex.Message, 3000);
            }
            finally
            {
                pr_load.Visibility = Visibility.Collapsed;
            }
        }
        private async void GetVideoComment()
        {
            try
            {
                pr_load.Visibility = Visibility.Visible;
                ListView_Comment.Items.Clear();
                WebClientClass wc = new WebClientClass();
                Random r = new Random();
                string results = await wc.GetResults(new Uri("http://api.bilibili.com/x/reply?jsonp=jsonp&type=1&sort=" + 2 + "&oid=" + ((BangumiInfoModel)cb_H.Items[cb_H.SelectedIndex]).av_id + "&pn=" + 1 + "&nohot=1&ps=20&r=" + r.Next(1000, 99999)));
                CommentModel model = JsonConvert.DeserializeObject<CommentModel>(results);
                CommentModel model3 = JsonConvert.DeserializeObject<CommentModel>(model.data.ToString());
                //Video_Grid_Info.DataContext = model;
                List<CommentModel> ban = JsonConvert.DeserializeObject<List<CommentModel>>(model3.replies.ToString());
                foreach (CommentModel item in ban)
                {
                    CommentModel model1 = JsonConvert.DeserializeObject<CommentModel>(item.member.ToString());
                    CommentModel model2 = JsonConvert.DeserializeObject<CommentModel>(item.content.ToString());
                    CommentModel modelLV = JsonConvert.DeserializeObject<CommentModel>(model1.level_info.ToString());
                    CommentModel resultsModel = new CommentModel()
                    {
                        avatar = model1.avatar,
                        message = model2.message,
                        plat = model2.plat,
                        floor = item.floor,
                        uname = model1.uname,
                        mid = model1.mid,
                        ctime = item.ctime,
                        like = item.like,
                        rcount = item.rcount,
                        rpid = item.rpid,
                        current_level = modelLV.current_level
                    };
                    ListView_Comment.Items.Add(resultsModel);
                }
            }
            catch (Exception ex)
            {
                //throw;
                messShow.Show("读取热门评论失败!\r\n" + ex.Message, 3000);
            }
            finally
            {
                pr_load.Visibility = Visibility.Collapsed;
            }
        }
        private void cb_Rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list_Rank != null)
            {
                list_Rank.ItemsSource = null;
                GetVideoComment();
            }
        }

        private void ListView_Comment_Hot_ItemClick(object sender, ItemClickEventArgs e)
        {
            object[] o = new object[] { (CommentModel)e.ClickedItem, ((BangumiInfoModel)list_E.Items[0]).av_id };
            this.Frame.Navigate(typeof(CommentPage), o);
        }

        private void ListView_Comment_ItemClick(object sender, ItemClickEventArgs e)
        {
            object[] o = new object[] { (CommentModel)e.ClickedItem, ((BangumiInfoModel)cb_H.Items[cb_H.SelectedIndex]).av_id };
            this.Frame.Navigate(typeof(CommentPage), o);
        }

        private void cb_H_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_H.ItemsSource != null)
            {
                GetVideoComment();
            }


        }

        private void btn_CB_Click(object sender, RoutedEventArgs e)
        {
            if (!new UserClass().IsLogin())
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
                pr_load.Visibility = Visibility.Visible;
                messShow.Show("开始创建订单", 2000);
                wc = new WebClientClass();
                string tokenString = await wc.GetResults(new Uri("http://bangumi.bilibili.com/web_api/get_token"));
                TokenModel tokenMess = JsonConvert.DeserializeObject<TokenModel>(tokenString);
                if (tokenMess.code == 0)
                {
                    TokenModel token = JsonConvert.DeserializeObject<TokenModel>(tokenMess.result.ToString());
                    string results = await wc.PostResults(new Uri("http://bangumi.bilibili.com/sponsor/payweb/create_order"), string.Format("pay_method=0&season_id={0}&amount={1}&token={2}", banID, money, token.token));
                    OrderModel orderMess = JsonConvert.DeserializeObject<OrderModel>(results);
                    if (orderMess.code == 0)
                    {
                        OrderModel orderModel = JsonConvert.DeserializeObject<OrderModel>(orderMess.result.ToString());
                        this.Frame.Navigate(typeof(WebViewPage), orderModel.cashier_url);
                    }
                    else
                    {
                        messShow.Show("订单创建失败," + orderMess.message, 3000);
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
                pr_load.Visibility = Visibility.Collapsed;
            }

        }


    }

    public class TokenModel
    {
        public int code { get; set; }
        public string message { get; set; }
        public object result { get; set; }
        public string token { get; set; }
    }
    public class OrderModel
    {
        public int code { get; set; }
        public string message { get; set; }
        public object result { get; set; }
        public string cashier_url { get; set; }
        public string order_id { get; set; }
        public string pay_pay_order_no { get; set; }
        public string qrcode { get; set; }
    }

    public class CBRankModel
    {
        public int code { get; set; }
        public string message { get; set; }
        public object result { get; set; }
        public object list { get; set; }
        public string face { get; set; }
        public string hidden { get; set; }//0 false,1 true
        public string rank { get; set; }
        public string uid { get; set; }
        public string uname { get; set; }
    }

}
