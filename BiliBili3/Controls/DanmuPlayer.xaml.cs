using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Media;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Display;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using FFmpegInterop;
using Windows.Media.Core;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace BiliBili3.Controls
{
    public enum PlayMode
    {
        Bangumi,
        Movie,
        VipBangumi,
        Video,
        QQ,
        Sohu,
        Local,
        FormLocal
    }
    public sealed partial class DanmuPlayer : UserControl
    {
        MediaPlayer _mediaPlayer;
        SystemMediaTransportControls _systemMediaTransportControls;
        //public delegate void FullHandel(bool full);
        public delegate void BackHandel();
        //public delegate void PlayerHandel(int index);
        //public event FullHandel FullEvent;
        //public event MaxwindowsHandel MaxWIndowsEvent;
        public event BackHandel BackEvent;
        public DanmuPlayer()
        {
            this.InitializeComponent();

            CoreWindow.GetForCurrentThread().KeyDown += PlayerPage_KeyDown;
           
            _mediaPlayer = new MediaPlayer();
            _systemMediaTransportControls = _mediaPlayer.SystemMediaTransportControls;
            _mediaPlayer.CommandManager.IsEnabled = false;
            _systemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView();
            _systemMediaTransportControls.IsPlayEnabled = true;
            _systemMediaTransportControls.IsPauseEnabled = true;
            _systemMediaTransportControls.ButtonPressed += _systemMediaTransportControls_ButtonPressed;
        }
        private async void _systemMediaTransportControls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        mediaElement.Play();
                    });
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        mediaElement.Pause();
                    });
                    break;
                default:
                    break;
            }
        }

        private void PlayerPage_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            args.Handled = true;
            switch (args.VirtualKey)
            {
                case Windows.System.VirtualKey.Space:
                    if (btn_Pause.Visibility == Visibility.Visible)
                    {
                        mediaElement.Pause();
                    }
                    else
                    {
                        mediaElement.Play();
                    }
                    break;
                case Windows.System.VirtualKey.Left:
                    slider3.Value = slider.Value - 3;
                    messShow.Show(mediaElement.Position.Hours.ToString("00") + ":" + mediaElement.Position.Minutes.ToString("00") + ":" + mediaElement.Position.Seconds.ToString("00"), 3000);
                    break;
                case Windows.System.VirtualKey.Up:
                    mediaElement.Volume += 0.1;
                    //mediaElement.Balance += 0.1;
                    messShow.Show("音量:" + mediaElement.Volume.ToString("P"), 3000);
                    break;
                case Windows.System.VirtualKey.Right:
                    slider3.Value = slider.Value + 3;
                    messShow.Show(mediaElement.Position.Hours.ToString("00") + ":" + mediaElement.Position.Minutes.ToString("00") + ":" + mediaElement.Position.Seconds.ToString("00"), 3000);
                    break;
                case Windows.System.VirtualKey.Down:
                    mediaElement.Volume -= 0.1;
                    //mediaElement.Balance -= 0.1;
                    messShow.Show("音量:" + mediaElement.Volume.ToString("P"), 3000);
                    break;
                case Windows.System.VirtualKey.Escape:
                    if (btn_Full.Visibility == Visibility.Collapsed)
                    {

                        btn_Full.Visibility = Visibility.Visible;
                        btn_ExitFull.Visibility = Visibility.Collapsed;
                        ApplicationView.GetForCurrentView().ExitFullScreenMode();

                        danmu.SetJJ();
                    }
                    break;

                case Windows.System.VirtualKey.F11:
                    if (btn_ExitFull.Visibility == Visibility.Collapsed)
                    {

                        btn_Full.Visibility = Visibility.Collapsed;
                        btn_ExitFull.Visibility = Visibility.Visible;


                        ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                        danmu.SetJJ();
                    }
                    else
                    {

                        btn_Full.Visibility = Visibility.Visible;
                        btn_ExitFull.Visibility = Visibility.Collapsed;
                        ApplicationView.GetForCurrentView().ExitFullScreenMode();
                        danmu.SetJJ();
                    }
                    break;
                default:
                    break;
            }
        }

        private DisplayRequest dispRequest = null;//保持屏幕常亮
        DispatcherTimer timer;
        DispatcherTimer timer_Date;
        List<PlayerModel> playList;
        List<MyDanmaku.DanMuModel> DanMuPool = null;
        PlayerModel playNow;

        //int _index = 0;
        bool playLocal = false;
        bool LoadDanmu = true;
        int LastPost = 0;
        public void LoadPlayer(List<PlayerModel> par, int index)
        {
            UpdateSetting();
            if (_mediaPlayer==null)
            {
                _mediaPlayer = new MediaPlayer();
                _systemMediaTransportControls = _mediaPlayer.SystemMediaTransportControls;
                _mediaPlayer.CommandManager.IsEnabled = false;
                _systemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView();
                _systemMediaTransportControls.IsPlayEnabled = true;
                _systemMediaTransportControls.IsPauseEnabled = true;
                _systemMediaTransportControls.ButtonPressed += _systemMediaTransportControls_ButtonPressed;
            }
            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 1);
                timer.Tick += Timer_Tick;
                timer.Start();
            }
            if (timer_Date == null)
            {
                timer_Date = new DispatcherTimer();
                timer_Date.Interval = new TimeSpan(0, 0, 1);
                timer_Date.Tick += Timer_Date_Tick; ;
                timer_Date.Start();
            }
            if (dispRequest == null)
            {
                // 用户观看视频，需要保持屏幕的点亮状态
                dispRequest = new DisplayRequest();
                dispRequest.RequestActive(); // 激活显示请求
            }
            DisplayInformation.AutoRotationPreferences = (DisplayOrientations)5;
            if (!SettingHelper.IsPc())
            {
                btn_Full_Click(null, null);
            }

            playList = par;
            playNow = playList[index];

            //  btn_HideInfo.Visibility = Visibility.Collapsed;
            //   btn_ShowInfo.Visibility = Visibility.Collapsed;
            mediaElement.AutoPlay = true;

            gv_play.ItemsSource = playList;
            gv_play.SelectedIndex = index;


            //DisplayInformation.AutoRotationPreferences = (DisplayOrientations)5;

        }

        public void ClosePLayer()
        {
            if (dispRequest != null)
            {
                dispRequest = null;
            }

            ApplicationView.GetForCurrentView().ExitFullScreenMode();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
            if (timer != null)
            {
                timer.Stop();
                timer = null;
            }
            if (timer_Date != null)
            {
                timer_Date.Stop();
                timer_Date = null;
            }
            gv_play.ItemsSource = null;
            mediaElement.Stop();
            mediaElement.Source = null;
            danmu.ClearDanmu();
            DisplayInformation.AutoRotationPreferences = DisplayOrientations.None;
        }


        private async void PostLocalHistory()
        {
            try
            {
                string url = string.Format("http://api.bilibili.com/x/history/add?_device=wp&_ulv=10000&access_key={0}&appkey={1}&build=411005&platform=android", ApiHelper.access_key, ApiHelper._appKey);
                url += "&sign=" + ApiHelper.GetSign(url);
                string result = await WebClientClass.PostResults(new Uri(url), "aid=" + playNow.Aid);
            }
            catch (Exception)
            {
            }
        }

     
        private async void PostWatch()
        {
            try
            {
                string url = string.Format("http://bangumi.bilibili.com/api/report_watch?access_key={0}&appkey={1}&build=411005&cid={2}&mobi_app=win&platform=android&scale=xhdpi&ts={3}&episode_id={4}", ApiHelper.access_key, ApiHelper._appKey, playNow.Mid, ApiHelper.GetTimeSpan_2, playNow.episode_id);
                url += "&sign=" + ApiHelper.GetSign(url);
                string result = await WebClientClass.GetResults(new Uri(url));
            }
            catch (Exception)
            {
            }
        }

        public void UpdateSetting()
        {
            //if (!SettingHelper.IsPc())
            //{
            //    btn_ShowInfo.Visibility = Visibility.Collapsed;
            //    btn_HideInfo.Visibility = Visibility.Collapsed;
            //    // danmu.fontSize = 16;
            //}
            DanDis_Get();
            DMZZBDS = SettingHelper.Get_DMZZ();
            slider_DanmuSize.Value = SettingHelper.Get_DMSize();
            slider_Num.Value = SettingHelper.Get_DMNumber();
            slider_DanmuTran.Value = SettingHelper.Get_DMTran();
            slider_DanmuSpeed.Value = SettingHelper.Get_DMSpeed();

            cb_Font.SelectedIndex = SettingHelper.Get_DMFont();
            sw_DanmuBorder.IsOn = SettingHelper.Get_DMBorder();

            slider_V.Value = SettingHelper.Get_Volume();
            DanmuNum = SettingHelper.Get_DMNumber();
            rb_defu.IsChecked = true;
            btn_ViewPost.Visibility = Visibility.Collapsed;
            cb_Quity.SelectedIndex = SettingHelper.Get_PlayQualit() - 1;


            menu_setting_buttom.IsChecked = !SettingHelper.Get_DMVisBottom();
            menu_setting_top.IsChecked = !SettingHelper.Get_DMVisTop();
            menu_setting_gd.IsChecked = !SettingHelper.Get_DMVisRoll();


            settinging = true;
            cb_setting_playback10.IsChecked = false;
            cb_setting_playback05.IsChecked = false;
            cb_setting_playback05.IsChecked = false;
            cb_setting_playback05.IsChecked = false;
            settinging = false;
            SetPlayback();



        }

        private void SetPlayback()
        {
            switch (SettingHelper.Get_Playback())
            {
                case 0:
                    cb_setting_playback10.IsChecked = true;
                    mediaElement.PlaybackRate = 1.0;
                    break;
                case 1:
                    cb_setting_playback05.IsChecked = true;
                    mediaElement.PlaybackRate = 0.5;
                    break;
                case 2:
                    cb_setting_playback15.IsChecked = true;
                    mediaElement.PlaybackRate = 1.5;
                    break;
                case 3:
                    cb_setting_playback20.IsChecked = true;
                    mediaElement.PlaybackRate = 2.0;
                    break;
                default:
                    break;
            }
        }

        string DMZZBDS = "";
        bool settinging = false;
        int DanmuNum = 0;
        int i = 0;
        private async void Timer_Date_Tick(object sender, object e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                txt_Time.Text = DateTime.Now.ToString("HH:mm");
            });

            if (mediaElement.CurrentState == MediaElementState.Playing && LoadDanmu)
            {
                if (DanMuPool != null)
                {
                    int now_num = 0;
                    foreach (var item in DanMuPool)
                    {
                        if (!DanDis_Dis(item.DanText))
                        {
                            if (Convert.ToInt32(item.DanTime) == Convert.ToInt32(mediaElement.Position.TotalSeconds))
                            {
                                if (now_num >= DanmuNum && DanmuNum != 0)
                                {
                                    return;
                                }
                                try
                                {
                                    if (DMZZBDS.Length != 0 && Regex.IsMatch(item.source, DMZZBDS))
                                    {
                                        return;
                                    }
                                }
                                catch (Exception)
                                {
                                }

                                if (item.DanMode == "5")
                                {
                                    danmu.AddTopButtomDanmu(item, true, false);
                                }
                                else
                                {
                                    if (item.DanMode == "4")
                                    {
                                        danmu.AddTopButtomDanmu(item, false, false);
                                    }
                                    else
                                    {
                                        danmu.AddGunDanmu(item, false);
                                    }
                                }
                                now_num++;
                            }

                        }

                    }

                    if (i == 2)
                    {
                        danmu.row = 0;
                        i = 0;
                    }
                    else
                    {
                        i++;
                    }
                }


            }

        }

        private async void Timer_Tick(object sender, object e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                slider.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                slider3.Maximum = mediaElement.NaturalDuration.TimeSpan.TotalSeconds;
                slider.Value = mediaElement.Position.TotalSeconds;
                txt_Post.Text = mediaElement.Position.Hours.ToString("00") + ":" + mediaElement.Position.Minutes.ToString("00") + ":" + mediaElement.Position.Seconds.ToString("00") + "/" + mediaElement.NaturalDuration.TimeSpan.Hours.ToString("00") + ":" + mediaElement.NaturalDuration.TimeSpan.Minutes.ToString("00") + ":" + mediaElement.NaturalDuration.TimeSpan.Seconds.ToString("00");
                if (SqlHelper.GetPostIsViewPost(playNow.Mid))
                {
                    SqlHelper.UpdateViewPost(new ViewPostHelperClass() { epId = playNow.Mid, Post = Convert.ToInt32(mediaElement.Position.TotalSeconds) });
                }

                //sql.UpdateValue(Cid, Convert.ToInt32(mediaElement.Position.TotalSeconds));
            });
        }

        #region 弹幕设置
        /// <summary>
        /// 弹幕屏蔽
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Dis_Remove_Click(object sender, RoutedEventArgs e)
        {
            foreach (MyDanmaku.DanMuModel item in list_DisDanmu.SelectedItems)
            {
                DanDis_Add(item.DanID, true);
                danmu.RemoveDanmu(item);
                list_DisDanmu.Items.Remove(item);

            }


            
        }
        List<string> Guanjianzi = new List<string>();
        List<string> Yonghu = new List<string>();
        private void DanDis_Get()
        {


            string a = SettingHelper.Get_Guanjianzi();
            string b = SettingHelper.Get_Yonghu();
            if (a.Length != 0)
            {

                Guanjianzi = a.Split('|').ToList();
                Yonghu = b.Split('|').ToList();
                Guanjianzi.Remove(string.Empty);
                Yonghu.Remove(string.Empty);
            }


        }
        private bool DanDis_Dis(string text)
        {
            var a = (from sb in Guanjianzi where text.Contains(sb) select sb).ToList();
            var b = (from sb in Yonghu where text.Contains(sb) select sb).ToList();
            if (b.Count != 0 || a.Count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void DanDis_Add(string text, bool IsYonghu)
        {
            if (IsYonghu)
            {
                SettingHelper.Set_Yonghu(SettingHelper.Get_Yonghu() + "|" + text);
                Yonghu.Add(text);
            }
            else
            {
                SettingHelper.Set_Guanjianzi(SettingHelper.Get_Guanjianzi() + "|" + text);

                Guanjianzi.Add(text);
            }

        }
        #endregion


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


        private async void OpenVideo()
        {
            try
            {
                LastPost = 0;
                slider3.Value = 0;
                slider.Value = 0;
                txt_Title.Text = playNow.Title + " - " + playNow.No + " " + playNow.VideoTitle;
                send_danmu.Visibility = Visibility.Visible;
                btn_Open_CloseDanmu.Visibility = Visibility.Visible;
                btn_Select.Visibility = Visibility.Visible;
                cb_Quity.Visibility = Visibility.Visible;
                pr.Text = "正在初始化播放器...";

                txt_ffmpeg.Text = "false";
                txt_fvideo.Text = "false";
                txt_faudio.Text = "false";


                if (!playLocal)
                {
                    cb_Quity.IsEnabled = true;
                    switch (playNow.Mode)
                    {
                        case PlayMode.Bangumi:
                            break;
                        case PlayMode.Movie:
                            break;
                        case PlayMode.VipBangumi:
                            break;
                        case PlayMode.Video:
                            pr.Text = "填充弹幕中...";
                            DanMuPool = await GetDM(playNow.Mid, false, false, "");
                            pr.Text = "加载视频中...";
                            string url = await ApiHelper.GetVideoUrl(playNow, cb_Quity.SelectedIndex + 1);
                            playNow.Path = url;
                            if (SettingHelper.Get_FFmpeg())
                            {
                                mediaElement.Source = new Uri(url);
                                mediaElement_MediaFailed(null,null);
                            }
                            else
                            {
                                mediaElement.Source = new Uri(url);
                            }
                           
                            break;
                        case PlayMode.QQ:
                            break;
                        case PlayMode.Sohu:
                            pr.Text = "填充弹幕中...";
                            DanMuPool = await GetDM(playNow.Mid, false, false, "");
                            pr.Text = "加载视频中...";
                            if (SettingHelper.Get_FFmpeg())
                            {
                                mediaElement.Source = new Uri(await ApiHelper.GetVideoUrl(playNow, cb_Quity.SelectedIndex + 1));
                                mediaElement_MediaFailed(null, null);
                            }
                            else
                            {
                                mediaElement.Source = new Uri(await ApiHelper.GetVideoUrl(playNow, cb_Quity.SelectedIndex + 1));
                            }
                         
                            break;
                        case PlayMode.Local:

                            pr.Text = "加载视频中...";
                            await PlayLocal();
                            break;
                        case PlayMode.FormLocal:
                            pr.Text = "加载视频中...";
                            send_danmu.Visibility = Visibility.Collapsed;
                            btn_Open_CloseDanmu.Visibility = Visibility.Collapsed;
                            cb_Quity.Visibility = Visibility.Collapsed;
                            //btn_Select.Visibility = Visibility.Collapsed;
                            PlayFromLocal();
                            break;
                        default:
                            break;
                    }

                }
                else
                {
                    cb_Quity.IsEnabled = false;
                    StorageFile file = await StorageFile.GetFileFromPathAsync(playNow.Mid);
                    IRandomAccessStream readStream = await file.OpenAsync(FileAccessMode.Read);
                    // var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                    mediaElement.SetSource(readStream, file.ContentType);
                }

                if (SqlHelper.GetPostIsViewPost(playNow.Mid) && SqlHelper.GettViewPost(playNow.Mid).Post != 0)
                {
                    TimeSpan ts = new TimeSpan(0, 0, SqlHelper.GettViewPost(playNow.Mid).Post);
                    LastPost = SqlHelper.GettViewPost(playNow.Mid).Post;
                    btn_ViewPost.Content = "上次播放到" + ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
                    btn_ViewPost.Visibility = Visibility.Visible;
                    _lastpost_in.Begin();
                    await Task.Delay(5000);
                    _lastpost_out.Begin();
                }
                else
                {
                    if (!SqlHelper.GetPostIsViewPost(playNow.Mid))
                    {
                        SqlHelper.AddViewPost(new ViewPostHelperClass() { epId = playNow.Mid, Post = 0, viewTime = DateTime.Now.ToLocalTime() });
                    }
                }

            }
            catch (Exception)
            {
                await new MessageDialog("视频暂时无法播放哦").ShowAsync();
            }
            finally
            {
              
                PostLocalHistory();
                if (playNow.episode_id != null && playNow.episode_id.Length != 0)
                {
                    PostWatch();
                }

                await Task.Delay(3000);
                _Out.Storyboard.Begin();
            }
        }

        private async void PlayFromLocal()
        {
            var item = playNow.Parameter as StorageFile;
            //if (item .FileType== ".mp4")
            //{
            IRandomAccessStream readStream = await item.OpenAsync(FileAccessMode.Read);
            // var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            mediaElement.SetSource(readStream, item.ContentType);
            if (SettingHelper.Get_FFmpeg())
            {
              
                mediaElement_MediaFailed(null, null);
            }
            //}

        }
        private async Task PlayLocal()
        {


            StorageFolder f = await StorageFolder.GetFolderFromPathAsync(playNow.Path);
            var ls = await f.GetFilesAsync();
            foreach (var item in ls)
            {
                if (item.FileType == ".xml")
                {
                    pr.Text = "填充弹幕中...";
                    DanMuPool = await GetLocalDanmu(item);
                }
                if (item.FileType == ".mp4")
                {
                    playNow.Parameter = item;
                    IRandomAccessStream readStream = await item.OpenAsync(FileAccessMode.Read);
                    // var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                    mediaElement.SetSource(readStream, item.ContentType);
                    if (SettingHelper.Get_FFmpeg())
                    {

                        mediaElement_MediaFailed(null, null);
                    }

                }

            }
            //playNow.Path
        }
        private async Task<List<MyDanmaku.DanMuModel>> GetLocalDanmu(StorageFile danmuFile)
        {
            List<MyDanmaku.DanMuModel> ls = new List<MyDanmaku.DanMuModel>();
            try
            {
                string a = await FileIO.ReadTextAsync(danmuFile);
                XmlDocument xdoc = new XmlDocument();
                a = Regex.Replace(a, @"[\x00-\x08]|[\x0B-\x0C]|[\x0E-\x1F]", "");
                xdoc.LoadXml(a);
                XmlElement el = xdoc.DocumentElement;
                XmlNodeList xml = el.ChildNodes;
                foreach (XmlNode item in xml)
                {
                    if (item.Attributes["p"] != null)
                    {
                        try
                        {
                            string heheda = item.Attributes["p"].Value;
                            string[] haha = heheda.Split(',');
                            ls.Add(new MyDanmaku.DanMuModel
                            {
                                DanTime = decimal.Parse(haha[0]),
                                DanMode = haha[1],
                                DanSize = haha[2],
                                _DanColor = haha[3],
                                DanSendTime = haha[4],
                                DanPool = haha[5],
                                DanID = haha[6],
                                DanRowID = haha[7],
                                DanText = item.InnerText,
                                source = item.OuterXml
                            });
                        }
                        catch (Exception)
                        {
                        }

                    }
                }
                return ls;
            }
            catch (Exception)
            {
                return ls;
            }
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            //if (this.Frame.CanGoBack)
            //{
            //    mediaElement.Stop();
            //    this.Frame.GoBack();
            //}
        }

        public async Task<List<MyDanmaku.DanMuModel>> GetDM(string cid, bool IsLocal, bool IsOld, string path)
        {
            List<MyDanmaku.DanMuModel> ls = new List<MyDanmaku.DanMuModel>();
            try
            {

                string a = await WebClientClass.GetResults(new Uri("http://comment.bilibili.com/" + cid + ".xml" + "?rnd=" + new Random().Next(1, 9999)));
                XmlDocument xdoc = new XmlDocument();
                a = Regex.Replace(a, @"[\x00-\x08]|[\x0B-\x0C]|[\x0E-\x1F]", "");
                xdoc.LoadXml(a);
                XmlElement el = xdoc.DocumentElement;
                XmlNodeList xml = el.ChildNodes;
                foreach (XmlNode item in xml)
                {
                    if (item.Attributes["p"] != null)
                    {
                        try
                        {
                            string heheda = item.Attributes["p"].Value;
                            string[] haha = heheda.Split(',');
                            ls.Add(new MyDanmaku.DanMuModel
                            {
                                DanTime = decimal.Parse(haha[0]),
                                DanMode = haha[1],
                                DanSize = haha[2],
                                _DanColor = haha[3],
                                DanSendTime = haha[4],
                                DanPool = haha[5],
                                DanID = haha[6],
                                DanRowID = haha[7],
                                DanText = item.InnerText,
                                source = item.OuterXml
                            });
                        }
                        catch (Exception)
                        {
                        }

                    }
                }
                return ls;
            }
            catch (Exception)
            {

                return ls;
            }

        }


        private void btn_Play_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Play();
        }

        private void btn_Pause_Click(object sender, RoutedEventArgs e)
        {
            if (mediaElement.CanPause)
            {
                mediaElement.Pause();
            }
        }
        private FFmpegInteropMSS FFmpegMSS;
        private async void mediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            PropertySet options = new PropertySet();
            mediaElement.Stop();
            if (playNow.Mode == PlayMode.Local || playNow.Mode == PlayMode.FormLocal)
            {
               
                StorageFile file = playNow.Parameter as StorageFile;
                IRandomAccessStream readStream = await file.OpenAsync(FileAccessMode.Read);
                FFmpegMSS = FFmpegInteropMSS.CreateFFmpegInteropMSSFromStream(readStream, SettingHelper.Get_ForceAudio(), SettingHelper.Get_ForceVideo());

                if (FFmpegMSS != null)
                {
                   // messShow.Show("播放类型系统不受支持，将使用软解", 3000);
                    MediaStreamSource mss = FFmpegMSS.GetMediaStreamSource();
                    mediaElement.SetMediaStreamSource(mss);
                    mediaElement.RealTimePlayback = true;
                    txt_ffmpeg.Text = "true";
                    txt_fvideo.Text = SettingHelper.Get_ForceVideo().ToString();
                    txt_faudio.Text = SettingHelper.Get_ForceAudio().ToString();
                }
                else
                {
                    await new MessageDialog("无法播放此视频").ShowAsync();
                }
            }
            else
            {
                FFmpegMSS = FFmpegInteropMSS.CreateFFmpegInteropMSSFromUri(playNow.Path, SettingHelper.Get_ForceAudio(), SettingHelper.Get_ForceVideo());
                if (FFmpegMSS != null)
                {
                    //messShow.Show("播放地址类型系统不受支持，将使用软解", 3000);
                    MediaStreamSource mss = FFmpegMSS.GetMediaStreamSource();
                    mediaElement.SetMediaStreamSource(mss);
                    mediaElement.RealTimePlayback = true;
                    txt_ffmpeg.Text = "true";
                    txt_fvideo.Text = SettingHelper.Get_ForceVideo().ToString();
                    txt_faudio.Text = SettingHelper.Get_ForceAudio().ToString();
                }
                else
                {
                    await new MessageDialog("无法播放此视频").ShowAsync();
                }
            }




        }
        private void mediaElement_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (grid.Visibility == Visibility.Collapsed)
            {
                grid.Visibility = Visibility.Visible;
                _In.Storyboard.Begin();

            }
            else
            {
                _Out.Storyboard.Begin();
            }

            _Out.Storyboard.Completed += Storyboard_Completed;
        }
        private void Storyboard_Completed(object sender, object e)
        {
            grid.Visibility = Visibility.Collapsed;
        }

        private void mediaElement_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
         
            // if (!SettingHelper.IsPc())
            // {
            if (mediaElement.CurrentState == MediaElementState.Playing)
            {
                mediaElement.Pause();
            }
            else
            {
                mediaElement.Play();
            }
            //}
            //else
            //{
            //    if (btn_Full.Visibility == Visibility.Visible)
            //    {
            //        btn_Full_Click(sender, e);
            //    }
            //    else
            //    {
            //        btn_ExitFull_Click(sender, e);
            //    }
            //}
        }

        private void mediaElement_BufferingProgressChanged(object sender, RoutedEventArgs e)
        {
            pr.Text = mediaElement.BufferingProgress.ToString("P");
        }

        private void mediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            danmu.state = mediaElement.CurrentState;
            switch (mediaElement.CurrentState)
            {
                case MediaElementState.Closed:
                    _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Closed;
                    btn_Play.Visibility = Visibility.Visible;
                    btn_Pause.Visibility = Visibility.Collapsed;
                    break;
                case MediaElementState.Opening:
                    btn_Play.Visibility = Visibility.Visible;
                    btn_Pause.Visibility = Visibility.Collapsed;
                    progress.Visibility = Visibility.Visible;
                    danmu.IsPlaying = false;
                    break;
                case MediaElementState.Buffering:
                    btn_Play.Visibility = Visibility.Collapsed;
                    btn_Pause.Visibility = Visibility.Visible;
                    progress.Visibility = Visibility.Visible;
                    danmu.IsPlaying = false;
                    break;
                case MediaElementState.Playing:
                    _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Playing;
                    btn_Play.Visibility = Visibility.Collapsed;
                    btn_Pause.Visibility = Visibility.Visible;
                    progress.Visibility = Visibility.Collapsed;
                    danmu.IsPlaying = true;

                    if (timer != null)
                    {
                        timer.Start();
                    }
                    break;
                case MediaElementState.Paused:
                    _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Paused;
                    btn_Play.Visibility = Visibility.Visible;
                    btn_Pause.Visibility = Visibility.Collapsed;
                    progress.Visibility = Visibility.Collapsed;
                    danmu.IsPlaying = false;
                    if (timer != null)
                    {
                        timer.Stop();
                    }
                    break;
                case MediaElementState.Stopped:
                    _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
                    btn_Play.Visibility = Visibility.Visible;
                    btn_Pause.Visibility = Visibility.Collapsed;
                    progress.Visibility = Visibility.Collapsed;
                    danmu.IsPlaying = false;
                    if (timer != null)
                    {
                        timer.Stop();
                    }

                    break;
                default:
                    break;
            }
        }
        private void slider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (timer == null)
            {
                return;
            }
            if (!timer.IsEnabled)
            {
                mediaElement.Position = new TimeSpan(0, 0, Convert.ToInt32(slider.Value));

                SetPlayback();
            }

            // mediaElement.Position = new TimeSpan(0, 0, Convert.ToInt32(slider.Value));
        }
        private void slider3_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {

            TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(slider3.Value));
            txt_Post.Text = ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00") + "/" + mediaElement.NaturalDuration.TimeSpan.Hours.ToString("00") + ":" + mediaElement.NaturalDuration.TimeSpan.Minutes.ToString("00") + ":" + mediaElement.NaturalDuration.TimeSpan.Seconds.ToString("00");
            if (mediaElement.CurrentState == MediaElementState.Playing)
            {
                mediaElement.Pause();
                slider.Value = slider3.Value;
                mediaElement.Position = new TimeSpan(0, 0, Convert.ToInt32(slider.Value));
                mediaElement.Play();
            }
            else
            {
                slider.Value = slider3.Value;
                mediaElement.Position = new TimeSpan(0, 0, Convert.ToInt32(slider.Value));
            }

        }

        private void slider_V_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            //TO DO SETTING VOLUME
            SettingHelper.Set_Volume(slider_V.Value);
        }
        #region 手势操作
        private void ss_Volume_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            e.Handled = true;
            double Y = e.Delta.Translation.Y;
            if (Y > 0)
            {
                double dd = Y / ss_Volume.ActualHeight;
                double d = dd * slider_V.Maximum;
                slider_V.Value -= d;
            }
            else
            {
                double dd = Math.Abs(Y) / ss_Volume.ActualHeight;
                double d = dd * slider_V.Maximum;
                slider_V.Value += d;
            }
            messShow.Show("音量:" + mediaElement.Volume.ToString("P"), 3000);
        }

        private void ss_Volume_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            e.Handled = true;
        }
        private void Grid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            e.Handled = true;
            mediaElement.Pause();
            //progress.Visibility = Visibility.Visible;
            double X = e.Delta.Translation.X;
            if (X > 0)
            {
                double dd = X / this.ActualWidth;
                double d = dd * 90;

                slider.Value += d;
            }
            else
            {
                double dd = Math.Abs(X) / this.ActualWidth;
                double d = dd * 90;
                slider.Value -= d;
            }
            TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(slider.Value));
            txt_Post.Text = ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00") + "/" + mediaElement.NaturalDuration.TimeSpan.Hours.ToString("00") + ":" + mediaElement.NaturalDuration.TimeSpan.Minutes.ToString("00") + ":" + mediaElement.NaturalDuration.TimeSpan.Seconds.ToString("00");
            messShow.Show(mediaElement.Position.Hours.ToString("00") + ":" + mediaElement.Position.Minutes.ToString("00") + ":" + mediaElement.Position.Seconds.ToString("00"), 3000);
        }

        private void Grid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            e.Handled = true;
            mediaElement.Play();

            double X = e.Cumulative.Translation.X;
        }
        #endregion

        private void gv_play_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gv_play.SelectedIndex != -1)
            {
                playNow = gv_play.SelectedItem as PlayerModel;
                // PlayerEvent(gv_play.SelectedIndex);
                OpenVideo();

            }
        }

        private void btn_Select_Click(object sender, RoutedEventArgs e)
        {
            _Out.Storyboard.Begin();
            gv_play.Visibility = Visibility.Visible;
            grid_Setting.Visibility = Visibility.Collapsed;
            grid_DM.Visibility = Visibility.Collapsed;
            grid_Info.Visibility = Visibility.Collapsed;
            grid_PB.Visibility = Visibility.Collapsed;

            sp_View.IsPaneOpen = true;
        }

        private async void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (cb_setting_1.IsChecked.Value)
            {
                mediaElement.Play();
                danmu.ClearDanmu();
                return;
            }
            if (gv_play.SelectedIndex == gv_play.Items.Count - 1)
            {
                if (cb_setting_2.IsChecked.Value)
                {
                    gv_play.SelectedIndex = 0;
                }
                else
                {
                    messShow.Show("全部看完了", 3000);
                }

            }
            else
            {
                mediaElement.Stop();
                messShow.Show("3秒后播放下一集", 3000);
                await Task.Delay(3000);
                gv_play.SelectedIndex += 1;
                slider3.Value = 0;
                slider.Value = 0;
            }
        }

        private void cb_Quity_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gv_play.ItemsSource == null)
            {
                return;
            }
            switch (cb_Quity.SelectedIndex)
            {
                case 2:
                    SettingHelper.Set_PlayQualit(3);
                    break;
                case 1:
                    SettingHelper.Set_PlayQualit(2);
                    break;
                case 0:
                    SettingHelper.Set_PlayQualit(1);
                    break;
                default:
                    break;
            }
            OpenVideo();
        }
        private void _lastpost_out_Completed(object sender, object e)
        {
            btn_ViewPost.Visibility = Visibility.Collapsed;
        }

        private void btn_ViewPost_Click(object sender, RoutedEventArgs e)
        {
            if (LastPost != 0)
            {
                mediaElement.Position = new TimeSpan(0, 0, Convert.ToInt32(LastPost));
                btn_ViewPost.Visibility = Visibility.Collapsed;

            }
        }

        private void ss_Holding(object sender, HoldingRoutedEventArgs e)
        {
            menu.ShowAt(this);
        }

        private void btn_VideoInfo_Click(object sender, RoutedEventArgs e)
        {
            _Out.Storyboard.Begin();
            sp_View.IsPaneOpen = true;
            grid_Setting.Visibility = Visibility.Visible;
            gv_play.Visibility = Visibility.Collapsed;
            grid_DM.Visibility = Visibility.Collapsed;
            grid_Info.Visibility = Visibility.Collapsed;
            grid_PB.Visibility = Visibility.Collapsed;
            //string info = string.Format("视频高度：{0}\r\n视频宽度：{1}\r\n视频长度：{2}\r\n缓冲进度:{3}", mediaElement.NaturalVideoHeight, mediaElement.NaturalVideoWidth, mediaElement.NaturalDuration.TimeSpan.Hours.ToString("00") + ":" + mediaElement.NaturalDuration.TimeSpan.Minutes.ToString("00") + ":" + mediaElement.NaturalDuration.TimeSpan.Seconds.ToString("00"), mediaElement.DownloadProgress.ToString("P"));
            //await new MessageDialog(info, "视频信息").ShowAsync();
        }
        private void cb_setting_defu_Checked(object sender, RoutedEventArgs e)
        {
            if (grid == null || grid.ActualWidth <= 0)
            {
                return;
            }
            mediaElement.Width = grid.ActualWidth;
            mediaElement.Height = grid.ActualHeight;
            mediaElement.Stretch = Stretch.Uniform;
        }

        private void cb_setting_43_Checked(object sender, RoutedEventArgs e)
        {
            mediaElement.Stretch = Stretch.Fill;
            mediaElement.Height = grid.ActualHeight;
            mediaElement.Width = grid.ActualHeight * 4 / 3;
        }

        private void cb_setting_169_Checked(object sender, RoutedEventArgs e)
        {
            mediaElement.Stretch = Stretch.Fill;
            mediaElement.Height = grid.ActualHeight;
            mediaElement.Width = grid.ActualHeight * 16 / 9;


        }

        //private void btn_HideInfo_Click(object sender, RoutedEventArgs e)
        //{
        //    //ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
        //    //MaxWIndowsEvent(true);
        //    btn_HideInfo.Visibility = Visibility.Collapsed;
        //    btn_ShowInfo.Visibility = Visibility.Visible;
        //    danmu.SetJJ();
        //}

        //private void btn_ShowInfo_Click(object sender, RoutedEventArgs e)
        //{
        //    //MaxWIndowsEvent(false);
        //    //ApplicationView.GetForCurrentView().ExitFullScreenMode();
        //    btn_HideInfo.Visibility = Visibility.Visible;
        //    btn_ShowInfo.Visibility = Visibility.Collapsed;
        //    danmu.SetJJ();
        //}

        private void btn_Full_Click(object sender, RoutedEventArgs e)
        {
            //FullEvent(true);
            if (!SettingHelper.IsPc())
            {

            }
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();

            btn_Full.Visibility = Visibility.Collapsed;
            btn_ExitFull.Visibility = Visibility.Visible;
            danmu.SetJJ();
        }

        private void btn_ExitFull_Click(object sender, RoutedEventArgs e)
        {
            //FullEvent(false);
            ApplicationView.GetForCurrentView().ExitFullScreenMode();
            btn_Full.Visibility = Visibility.Visible;
            btn_ExitFull.Visibility = Visibility.Collapsed;
            danmu.SetJJ();
        }



        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
           
            SystemMediaTransportControlsDisplayUpdater updater = _systemMediaTransportControls.DisplayUpdater;
            updater.Type = MediaPlaybackType.Video;
            // updater.MusicProperties.AlbumArtist = info.owner.name;
            updater.VideoProperties.Subtitle = playNow.VideoTitle;
            updater.VideoProperties.Title = playNow.Title;

            // Set the album art thumbnail.
            // RandomAccessStreamReference is defined in Windows.Storage.Streams
            updater.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/Logo.png"));

            // Update the system media transport controls.
            updater.Update();
            var timelineProperties = new SystemMediaTransportControlsTimelineProperties();

            // Fill in the data, using the media elements properties 
            timelineProperties.StartTime = TimeSpan.FromSeconds(0);
            timelineProperties.MinSeekTime = TimeSpan.FromSeconds(0);
            timelineProperties.Position = mediaElement.Position;
            timelineProperties.MaxSeekTime = mediaElement.NaturalDuration.TimeSpan;
            timelineProperties.EndTime = mediaElement.NaturalDuration.TimeSpan;

            // Update the System Media transport Controls 
            _systemMediaTransportControls.UpdateTimelineProperties(timelineProperties);
        }

        private void btn_Open_CloseDanmu_Click(object sender, RoutedEventArgs e)
        {
            if (danmu.Visibility == Visibility.Collapsed)
            {
                danmu.Visibility = Visibility.Visible;
                btn_Open_CloseDanmu.Foreground = new SolidColorBrush(Colors.White);
                LoadDanmu = true;

            }
            else
            {
                danmu.Visibility = Visibility.Collapsed;
                btn_Open_CloseDanmu.Foreground = new SolidColorBrush(Colors.Gray);
                LoadDanmu = false;

            }
        }

        private void menuitem_DM_Click(object sender, RoutedEventArgs e)
        {
            _Out.Storyboard.Begin();
            sp_View.IsPaneOpen = true;
            grid_Setting.Visibility = Visibility.Collapsed;
            gv_play.Visibility = Visibility.Collapsed;
            grid_DM.Visibility = Visibility.Visible;
            grid_Info.Visibility = Visibility.Collapsed;
            grid_PB.Visibility = Visibility.Collapsed;

        }

        private void menuitem_PB_Click(object sender, RoutedEventArgs e)
        {
            _Out.Storyboard.Begin();
            mediaElement.Pause();
            sp_View.IsPaneOpen = true;
            grid_Setting.Visibility = Visibility.Collapsed;
            gv_play.Visibility = Visibility.Collapsed;
            grid_DM.Visibility = Visibility.Collapsed;
            grid_Info.Visibility = Visibility.Collapsed;
            grid_PB.Visibility = Visibility.Visible;
            list_DisDanmu.Items.Clear();
            foreach (var item in danmu.GetScreenDanmu())
            {
                list_DisDanmu.Items.Add(item);
            }
        }

        private void menuitem_Info_Click(object sender, RoutedEventArgs e)
        {
            _Out.Storyboard.Begin();
            sp_View.IsPaneOpen = true;
            if (playNow != null && DanMuPool != null)
            {
                txt_Num.Text = DanMuPool.Count.ToString();
                txt_sId.Text = playNow.Aid;
                txt_eId.Text = playNow.Mid;
            }
            grid_Setting.Visibility = Visibility.Collapsed;
            gv_play.Visibility = Visibility.Collapsed;
            grid_DM.Visibility = Visibility.Collapsed;
            grid_Info.Visibility = Visibility.Visible;
            grid_PB.Visibility = Visibility.Collapsed;
        }

        #region 设置
        private void sw_DanmuBorder_Toggled(object sender, RoutedEventArgs e)
        {
            danmu.D_Border = sw_DanmuBorder.IsOn;
            SettingHelper.Set_DMBorder(sw_DanmuBorder.IsOn);
        }

        private void slider_DanmuSize_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            danmu.fontSize = slider_DanmuSize.Value;
            danmu.SetJJ();
            SettingHelper.Set_DMSize(slider_DanmuSize.Value);
        }

        private void cb_Font_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cb_Font.SelectedIndex)
            {
                case 0:
                    danmu.fontFamily = "默认";
                    break;
                case 1:
                    danmu.fontFamily = "微软雅黑";
                    break;
                case 2:
                    danmu.fontFamily = "黑体";
                    break;
                case 3:
                    danmu.fontFamily = "楷体";
                    break;
                case 4:
                    danmu.fontFamily = "宋体";
                    break;
                case 5:
                    danmu.fontFamily = "等线";
                    break;
                default:
                    break;
            }
            SettingHelper.Set_DMFont(cb_Font.SelectedIndex);
        }

        private void slider_DanmuSpeed_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            danmu.Speed = Convert.ToInt32(slider_DanmuSpeed.Value);
            SettingHelper.Set_DMSpeed(slider_DanmuSpeed.Value);
        }

        private void slider_DanmuTran_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            danmu.Tran = slider_DanmuTran.Value / 100;
            SettingHelper.Set_DMTran(slider_DanmuTran.Value);
        }
        private void slider_Num_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            DanmuNum = Convert.ToInt32(slider_Num.Value);
            SettingHelper.Set_DMNumber(Convert.ToInt32(slider_Num.Value));
        }

        private void cb_setting_playback10_Checked(object sender, RoutedEventArgs e)
        {
            if (mediaElement == null || settinging)
            {
                return;
            }
            mediaElement.PlaybackRate = 1.0;
            SettingHelper.Set_Playback(0);
        }

        private void cb_setting_playback05_Checked(object sender, RoutedEventArgs e)
        {
            if (mediaElement == null || settinging)
            {
                return;
            }
            mediaElement.PlaybackRate = 0.5;
            SettingHelper.Set_Playback(1);
        }

        private void cb_setting_playback15_Checked(object sender, RoutedEventArgs e)
        {
            if (mediaElement == null || settinging)
            {
                return;
            }
            mediaElement.PlaybackRate = 1.5;
            SettingHelper.Set_Playback(2);
        }

        private void cb_setting_playback20_Checked(object sender, RoutedEventArgs e)
        {
            if (mediaElement == null || settinging)
            {
                return;
            }
            mediaElement.PlaybackRate = 2.0;
            SettingHelper.Set_Playback(3);
        }


        private void menu_setting_top_Click(object sender, RoutedEventArgs e)
        {

            danmu.SetDanmuVisibility(false, MyDanmaku.DanmuMode.Top);
            SettingHelper.Set_DMVisTop(false);


        }

        private void menu_setting_buttom_Click(object sender, RoutedEventArgs e)
        {
            danmu.SetDanmuVisibility(false, MyDanmaku.DanmuMode.Buttom);
            SettingHelper.Set_DMVisBottom(false);
        }

        private void menu_setting_gd_Checked(object sender, RoutedEventArgs e)
        {
            danmu.SetDanmuVisibility(false, MyDanmaku.DanmuMode.Roll);
            SettingHelper.Set_DMVisRoll(false);
        }

        private void menu_setting_gd_Unchecked(object sender, RoutedEventArgs e)
        {
            danmu.SetDanmuVisibility(true, MyDanmaku.DanmuMode.Roll);
            SettingHelper.Set_DMVisRoll(true);
        }

        private void menu_setting_top_Unchecked(object sender, RoutedEventArgs e)
        {
            danmu.SetDanmuVisibility(true, MyDanmaku.DanmuMode.Top);
            SettingHelper.Set_DMVisTop(true);
        }

        private void menu_setting_buttom_Unchecked(object sender, RoutedEventArgs e)
        {
            danmu.SetDanmuVisibility(true, MyDanmaku.DanmuMode.Buttom);
            SettingHelper.Set_DMVisBottom(true);
        }


        private void btn_OK_Click(object sender, RoutedEventArgs e)
        {
            
            DanDis_Add(txt_Dis.Text, false);
            txt_Dis.Text = "";
            var s = danmu.GetScreenDanmu();
            foreach (var item in s)
            {
                if (DanDis_Dis(item.DanText))
                {
                    danmu.RemoveDanmu(item);
                }
            }
        }






        #endregion

        private void Send_btn_Send_Click(object sender, RoutedEventArgs e)
        {
            
            SenDanmuKa();
        }
        /// <summary>
        /// 发送弹幕
        /// </summary>
        public async void SenDanmuKa()
        {
            if (Send_text_Comment.Text.Length == 0)
            {
                messShow.Show("弹幕内容不能为空!", 2000);
                return;
            }
            if (!ApiHelper.IsLogin())
            {
                messShow.Show("请先登录!", 2000);
                return;
            }
            try
            {
                Uri ReUri = new Uri("http://interface.bilibili.com/dmpost?cid=" + playNow.Mid + "&aid=" + playNow.Aid + "&pid=1");
                int modeInt = 1;
                if (Send_cb_Mode.SelectedIndex == 2)
                {
                    modeInt = 4;
                }
                if (Send_cb_Mode.SelectedIndex == 1)
                {
                    modeInt = 5;
                }
                string Canshu = "message=" + Send_text_Comment.Text + "&pool=0&playTime=" + mediaElement.Position.TotalSeconds.ToString() + "&cid=" + playNow.Mid + "&date=" + DateTime.Now.ToString() + "&fontsize=25&mode=" + modeInt + "&rnd=" + new Random().Next(100000000, 999999999) + "&color=" + ((ComboBoxItem)Send_cb_Color.SelectedItem).Tag;
                string result = await WebClientClass.PostResults(ReUri, Canshu);
                long code = long.Parse(result);

                if (code < 0)
                {
                    messShow.Show("已发送弹幕！" + result, 3000);
                }
                else
                {
                    messShow.Show("已发送弹幕！", 3000);
                    if (modeInt == 1)
                    {
                        danmu.AddGunDanmu(new MyDanmaku.DanMuModel { DanText = Send_text_Comment.Text, _DanColor = ((ComboBoxItem)Send_cb_Color.SelectedItem).Tag.ToString(), DanSize = "25" }, true);
                    }
                    if (modeInt == 4)
                    {
                        danmu.AddTopButtomDanmu(new MyDanmaku.DanMuModel { DanText = Send_text_Comment.Text, _DanColor = ((ComboBoxItem)Send_cb_Color.SelectedItem).Tag.ToString(), DanSize = "25" }, false, true);
                    }
                    if (modeInt == 5)
                    {
                        danmu.AddTopButtomDanmu(new MyDanmaku.DanMuModel { DanText = Send_text_Comment.Text, _DanColor = ((ComboBoxItem)Send_cb_Color.SelectedItem).Tag.ToString(), DanSize = "25" }, true, true);
                    }
                    Send_text_Comment.Text = string.Empty;
                }

            }
            catch (Exception ex)
            {
                messShow.Show("发送弹幕发生错误！\r\n" + ex.Message, 3000);
            }
        }


        private void btn_Back_Click_1(object sender, RoutedEventArgs e)
        {
            ClosePLayer();
            BackEvent();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            danmu.SetJJ();
        }
    }
    //进度转换
    public sealed class PostThumbToolTipValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int i = 0;
            int.TryParse(value.ToString(), out i);
            TimeSpan ts = new TimeSpan(0, 0, i);
            return ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }



}
