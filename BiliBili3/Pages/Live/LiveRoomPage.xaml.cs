using BiliBili3.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Windows.UI;
using Windows.UI.Xaml.Documents;
using Windows.Media.Playback;
using Windows.Media;
using Windows.Storage.Streams;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Popups;
using FFmpegInterop;
using Windows.Media.Core;
using Windows.UI.Core;
using Windows.ApplicationModel.DataTransfer;
using BiliBili3.Helper;
using static BiliBili3.Helper.BiliLiveDanmu;
using Windows.UI.ViewManagement;
using Windows.Graphics.Display;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace BiliBili3.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LiveRoomPage : Page
    {
        MediaPlayer _mediaPlayer;
        SystemMediaTransportControls _systemMediaTransportControls;
        public LiveRoomPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
           
            _mediaPlayer = new MediaPlayer();
            _systemMediaTransportControls = _mediaPlayer.SystemMediaTransportControls;
            _mediaPlayer.CommandManager.IsEnabled = false;
            _systemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView();
            _systemMediaTransportControls.IsPlayEnabled = true;
            _systemMediaTransportControls.IsPauseEnabled = true;
            _systemMediaTransportControls.ButtonPressed += _systemMediaTransportControls_ButtonPressed;
            CoreWindow.GetForCurrentThread().KeyDown += LiveRoomPage_KeyDown; ;
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }

        private void LiveRoomPage_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            args.Handled = true;
            switch (args.VirtualKey)
            {
                case Windows.System.VirtualKey.Escape:
                    btn_exitFull_Click(null, null);
                    break;
                case Windows.System.VirtualKey.Up:
                    mediaElement.Volume += 0.1;
                    //mediaElement.Balance += 0.1;
                    messShow.Show("音量:" + mediaElement.Volume.ToString("P"), 3000);
                    break;
                case Windows.System.VirtualKey.Down:
                    mediaElement.Volume -= 0.1;
                    //mediaElement.Balance -= 0.1;
                    messShow.Show("音量:" + mediaElement.Volume.ToString("P"), 3000);
                    break;


                case Windows.System.VirtualKey.F11:
                    if (btn_exitFull.Visibility == Visibility.Collapsed)
                    {

                        //btn_exitFull.Visibility = Visibility.Collapsed;
                        //btn_full.Visibility = Visibility.Visible;
                      
                        btn_full_Click(null, null);
                        // ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                        //danmu.SetJJ();
                    }
                    else
                    {
                        btn_exitFull_Click(null, null);

                        //ApplicationView.GetForCurrentView().ExitFullScreenMode();
                        //danmu.SetJJ();
                    }
                    break;
                default:
                    break;
            }
        }

        //int i = 0;
        private async void _biliLiveDanmu_HasDanmu(BiliLiveDanmu.LiveDanmuModel value)
        {
            try
            {
                switch (value.type)
                {
                    case BiliLiveDanmu.LiveDanmuTypes.Viewer:
                        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            txt_online.Text = value.viewer.ToString();
                        });
                        break;
                    case BiliLiveDanmu.LiveDanmuTypes.Danmu:
                        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                           var m=  value.value as DanmuMsgModel;
                             LoadDanmu(m);
                            if (DanmuOpen)
                            {
                                danmu.AddGunDanmu(new Controls.MyDanmaku.DanMuModel()
                                {
                                    DanText = m.text,
                                    DanSize = "25",
                                    _DanColor = "16777215"
                                }, false);
                                //danmu.AddGiftDanmu(new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Img/fff.png")));
                               // danmu.AddGiftDanmu(new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Img/233.png")));
                               // danmu.AddGiftDanmu(new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Img/666.png")));
                            }
                           
                        });

                        //if (i == danmu.maxRow)
                        //{
                        //    danmu.row = 0;
                        //    i = 0;
                        //}
                        //else
                        //{
                        //    i++;
                        //}


                        break;
                    case BiliLiveDanmu.LiveDanmuTypes.Gift:
                        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            var info = value.value as GiftMsgModel;
                            LoadGiftMsg(info);
                            if (DanmuOpen&& _LoadDMGift)
                            {
                                if (info.giftName == "FFF")
                                {
                                    danmu.AddGiftDanmu(new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Img/fff.png")));
                                }
                                if (info.giftName == "233")
                                {
                                    danmu.AddGiftDanmu(new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Img/233.png")));
                                }
                                if (info.giftName == "666")
                                {
                                    danmu.AddGiftDanmu(new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri("ms-appx:///Assets/Img/666.png")));
                                }
                            }
                        });
                        break;
                    case BiliLiveDanmu.LiveDanmuTypes.Welcome:
                        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            LoadWelcomeMsg(value.value as WelcomeMsgModel);
                        });
                        break;
                    case BiliLiveDanmu.LiveDanmuTypes.SystemMsg:
                        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            AddComment(new TextBlock() { Text = value.value.ToString().Replace("?", ""), Foreground = new SolidColorBrush(Colors.OrangeRed) }, false);
                        });
                        break;
                    default:
                        break;
                }



            }
            catch (Exception)
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    messShow.Show("加载弹幕失败",3000);
                });
             
               // throw;
            }
            finally
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    sc.ChangeView(null, sc.ExtentHeight, null);
                });
            }
           



        }

        private void LoadDanmu(DanmuMsgModel item)
        {
           

            Run r_vip = new Run() { Foreground = new SolidColorBrush(Colors.Orange) };
            Run r_lv = new Run();
            Run r_medal = new Run();
            Run r_name = new Run() { Foreground = new SolidColorBrush(Colors.Gray) };


            if (item.vip!=null)
            {
                r_vip.Text = item.vip+" ";
            }
            if (item.username!=null)
            {
                r_name.Text = item.username + ":";
            }
            if (item.ul!=null)
            {
                r_lv.Text = item.ul+" ";
                r_lv.Foreground =GetColor( item.ulColor);
            }
            if (item.medal!=null)
            {
                r_medal.Text = item.medal + " ";
                r_medal.Foreground = GetColor(item.medalColor);
            }

            //if (item.medal != null && item.medal.Length != 0)
            //{
            //    r_medal.Text = " " + item.medal[1] + item.medal[0].ToString() + " ";
            //    r_medal.Foreground = GetColor(item.medal[3].ToString());
            //    //vip += "[" + item.medal[1] + item.medal[0] + "]";
            //}
            //if (item.user_level != null && item.user_level.Length != 0)
            //{
            //    r_lv.Text = " UL" + item.user_level[0] + " ";
            //    r_lv.Foreground = GetColor(item.user_level[2].ToString());
            //}

            //r_name.Text = item.nickname + ":";

            TextBlock tx = new TextBlock();
            tx.Inlines.Add(r_vip);
            tx.Inlines.Add(r_medal);
            tx.Inlines.Add(r_lv);
            tx.Inlines.Add(r_name);
            tx.Inlines.Add(new Run() { Text = item.text });
            // tx.Text+=item.text;

            AddComment(tx, false);
        }
        private void LoadGiftMsg(GiftMsgModel item)
        {

            TextBlock tx = new TextBlock();
            tx.Inlines.Add( new Run() { Text=item.uname,Foreground=new SolidColorBrush(Colors.Gray)});
            tx.Inlines.Add(new Run() { Text = ":" + item.action + " " });
            tx.Inlines.Add(new Run() { Text = item.giftName+"x" + item.num, Foreground = new SolidColorBrush(Colors.HotPink) });
          
            // tx.Text+=item.text;

            AddComment(tx, false);
        }
        private void LoadWelcomeMsg(WelcomeMsgModel item)
        {
            Run r_u = new Run() { Foreground = new SolidColorBrush(Colors.HotPink) };

            TextBlock tx = new TextBlock();
          
            r_u.Text = item.uname;
            tx.Inlines.Add(r_u);
            tx.Inlines.Add(new Run() { Text = " 进入直播间" });
            // tx.Text+=item.text;

            AddComment(tx, false);
        }


        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            
             DataRequest request = args.Request;
            request.Data.Properties.Title = txt_title.Text;
            request.Data.Properties.Description = txt_title.Text + "\r\n——分享自哔哩哔哩UWP";
            request.Data.SetWebLink(new Uri("http://live.bilibili.com/" + _roomid));

        }


        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
        BiliLiveDanmu _biliLiveDanmu;
        DispatcherTimer time;
        private DisplayRequest dispRequest = null;//保持屏幕常亮
        string _roomid = "";
        protected override  void OnNavigatedTo(NavigationEventArgs e)
        {

            if (e.NavigationMode == NavigationMode.New)
            {
                if (_mediaPlayer == null)
                {
                    _mediaPlayer = new MediaPlayer();
                    _systemMediaTransportControls = _mediaPlayer.SystemMediaTransportControls;
                    _mediaPlayer.CommandManager.IsEnabled = false;
                    _systemMediaTransportControls = SystemMediaTransportControls.GetForCurrentView();
                    _systemMediaTransportControls.IsPlayEnabled = true;
                    _systemMediaTransportControls.IsPauseEnabled = true;
                    _systemMediaTransportControls.ButtonPressed += _systemMediaTransportControls_ButtonPressed;
                }

                if (dispRequest == null)
                {
                    // 用户观看视频，需要保持屏幕的点亮状态
                    dispRequest = new DisplayRequest();
                    dispRequest.RequestActive(); // 激活显示请求
                }
             
                cb_Source.ItemsSource = null;
                pivot.SelectedIndex = 0;
                slider_V.Value = 1;
                cd_GiftNum.Visibility = Visibility.Collapsed;
                cd_BuyGiftNum.Visibility = Visibility.Collapsed;
                list_Gift_Top.Items.Clear();
                list_Fans_Top.Items.Clear();
                list_Zhuf_Top.Items.Clear();
                mediaElement.Source = null;
                stack_Comment.Children.Clear();
                LoadSetting();

                if (_biliLiveDanmu == null)
                {
                    _biliLiveDanmu = new BiliLiveDanmu();
                    _biliLiveDanmu.HasDanmu += _biliLiveDanmu_HasDanmu;
                }


                time = new DispatcherTimer();
                time.Interval = new TimeSpan(0, 0, 0,4);

                time.Tick += Time_Tick;
                _roomid = (e.Parameter as object[])[0].ToString(); ;
                txt_room.Text = "房间" + _roomid;
                LoadRoomInfo();

                //string url = string.Format("http://live.bilibili.com/appUser/getTitle?access_key={0}&appkey={1}&build=433000&platform=wp&scale=xxhdpi", ApiHelper.access_key, ApiHelper._appKey);
                //url += "&sign=" + ApiHelper.GetSign(url);
                //string results = await WebClientClass.GetResults(new Uri(url));
            }


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
        bool ROUND = false;
        private async void LoadRoomInfo()
        {
            try
            {
                ROUND = false;
                pr_Load.Visibility = Visibility.Visible;
                cd.Visibility = Visibility.Collapsed;
                string url = string.Format("http://live.bilibili.com/AppRoom/index?_device=android&access_key={0}&appkey={1}&build=434000&buld=434000&jumpFrom=24000&mobi_app=android&platform=android&room_id={2}&scale=xxhdpi", ApiHelper.access_key, ApiHelper._appKey, _roomid);
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                string results = await WebClientClass.GetResults(new Uri(url));
                LiveInfoModel m = JsonConvert.DeserializeObject<LiveInfoModel>(results);
                if (m.code == 0)
                {
                    this.DataContext = m.data;
                    if (m.data.is_attention == 1)
                    {
                        txt_guanzhu.Text = "已关注";
                    }
                    else
                    {
                        txt_guanzhu.Text = "关注";
                    }
                    txt_ul.Foreground = GetColor(m.data.master_level_color);
                    string b = @"<head><style>p{font-family:""微软雅黑"";}</style></head>";
                    web.NavigateToString(b + m.data.meta.description);
                    grid_Error.Visibility = Visibility.Collapsed;
                    txt_online.Text = m.data.online.ToString();


                    if (m.data.status == "LIVE")
                    {
                        GetPlayUrl();
                        GetComment();
                        // time.Start();
                    }
                    else
                    {
                        if (m.data.status== "ROUND")
                        {
                            txt_room.Text += "(轮播中)";
                            ROUND = true;
                            SetRoundPlayUrl();
                            GetComment();
                            //time.Start();
                        }
                        else
                        {
                            grid_Error.Visibility = Visibility.Visible;
                            txt_ErrorInfo.Text = m.data.prepare ?? "主播正在换女装";
                            GetComment();
                        }
                     
                    }


                }
                else
                {
                    messShow.Show(m.message, 3000);
                }
            }
            catch (Exception ex)
            {
                grid_Error.Visibility = Visibility.Visible;
                messShow.Show(  "读取错误" + ex.Message,3000);
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
                GetMyGifts();
                LoadInfo();
                try
                {
                    long uid = 0;
                    if (ApiHelper.IsLogin())
                    {
                        long.TryParse(ApiHelper.GetUserId(), out uid);
                    }
                    AddComment(new TextBlock() { Text = "开始连接弹幕服务器...", Foreground = new SolidColorBrush(Colors.OrangeRed) }, false);
                    _biliLiveDanmu.Start(Convert.ToInt32(_roomid), uid);
                }
                catch (Exception ex)
                {
                    messShow.Show( ex.Message, 3000);
                }
              
                time.Start();


            }
        }

        private void Time_Tick(object sender, object e)
        {
            //GetComment();
            danmu.row = 0;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (dispRequest != null)
            {
                dispRequest = null;
            }
            if (time != null)
            {
                time.Stop();
                time = null;
            }
            if (_biliLiveDanmu!=null)
            {
                _biliLiveDanmu.Dispose();
                _biliLiveDanmu.HasDanmu -= _biliLiveDanmu_HasDanmu;
                _biliLiveDanmu = null;
            }
            stack_Comment.Children.Clear();
            danmu.row = 0;
            danmu.ClearDanmu();

        }
        private async void GetMyGifts()
        {
            string url =string.Format( "http://live.bilibili.com/AppBag/playerBag?_device=android&access_key={0}&appkey={1}&build=434000&mobi_app=android&platform=android",ApiHelper.access_key,ApiHelper._appKey_Android);
            url += "&sign=" + ApiHelper.GetSign_Android(url);
            string results =await WebClientClass.GetResults(new Uri(url));
            LiveMyGiftsModel m = JsonConvert.DeserializeObject<LiveMyGiftsModel>(results);
            if (m.code==0)
            {
                gridview_myGifts.ItemsSource = m.data;
            }
            else
            {
                messShow.Show(m.message, 3000);
            }


        }
        List<string> loaded = new List<string>();
        private async void GetComment()
        {
            try
            {
                //sc.ChangeView(null, sc.ExtentHeight, null);
                //http://live.bilibili.com/AppRoom/msg?_device=android&_hwid=68fc5d795c256cd1&appkey=c1b107428d337928&build=414000&platform=android&room_id=23058&sign=4bf8088300d9f4c90b62264c4a87585d

                string url = string.Format("http://live.bilibili.com/AppRoom/msg?_device=wp&appkey={0}&build=411005&access_key={1}&platform=android&room_id={2}&ts={3}", ApiHelper._appKey, ApiHelper.access_key, _roomid, ApiHelper.GetTimeSpan);
                url += "&sign=" + ApiHelper.GetSign(url);
                string results = await WebClientClass.GetResults(new Uri(url));
                Model models = JsonConvert.DeserializeObject<Model>(results);
                if (models.code == 0)
                {
                    Model data = JsonConvert.DeserializeObject<Model>(models.data.ToString());
                    List<Model> model = JsonConvert.DeserializeObject<List<Model>>(data.room.ToString());
                    foreach (var item in model)
                    {
                        if (!loaded.Contains(item.nickname + item.timeline + item.text))
                        {
                            Run r_vip = new Run() { Foreground = new SolidColorBrush(Colors.Orange) } ;
                          
                            Run r_lv = new Run();
                            Run r_medal = new Run();
                            Run r_name = new Run() { Foreground = new SolidColorBrush(Colors.Gray) };


                            //string vip = string.Empty;
                            if (item.vip == 1)
                            {
                                if (item.svip == 1)
                                {
                                    
                                    r_vip.Text = "年费老爷 ";
                                    //vip += "[大爷]";
                                }
                                else
                                {
                                    r_vip.Text = "老爷 ";
                                    //vip += "[爷]";
                                }
                            }

                            if (item.medal != null&& item.medal.Length!=0)
                            {
                                r_medal.Text = item.medal[1] + item.medal[0].ToString()+" ";
                                r_medal.Foreground = GetColor(item.medal[3].ToString());
                                //vip += "[" + item.medal[1] + item.medal[0] + "]";
                            }
                            if (item.user_level!=null&& item.user_level.Length != 0)
                            {
                                r_lv.Text = "UL" + item.user_level[0]+" ";
                                r_lv.Foreground = GetColor(item.user_level[2].ToString());
                            }

                            r_name.Text = item.nickname+":";

                            TextBlock tx = new TextBlock();
                            tx.Inlines.Add(r_vip);
                            tx.Inlines.Add (r_medal);
                            tx.Inlines.Add(r_lv);
                            tx.Inlines.Add(r_name);
                            tx.Inlines.Add(new Run() { Text= item.text });
                            // tx.Text+=item.text;

                            AddComment(tx, false);
                            loaded.Add(item.nickname + item.timeline + item.text);
                            //if (LoadDanmu && !DanDis_Dis(item.text))
                            //{
                            //    danmu.AddGunDanmu(new Controls.MyDanmaku.DanMuModel() { DanText = item.text, DanSize = "25", _DanColor = "16777215" }, false);
                            //}

                        }
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                sc.ChangeView(null, sc.ExtentHeight, null);
            }

        }

        public SolidColorBrush GetColor(string _color)
        {
           
                try
                {
                  _color = Convert.ToInt32(_color).ToString("X2");
                    if (_color.StartsWith("#"))
                    _color = _color.Replace("#", string.Empty);
                    int v = int.Parse(_color, System.Globalization.NumberStyles.HexNumber);
                    SolidColorBrush solid = new SolidColorBrush(new Color()
                    {
                        A = Convert.ToByte(255),
                        R = Convert.ToByte((v >> 16) & 255),
                        G = Convert.ToByte((v >> 8) & 255),
                        B = Convert.ToByte((v >> 0) & 255)
                    });
                   // color = solid;
                    return solid;
                }
                catch (Exception)
                {
                    SolidColorBrush solid = new SolidColorBrush(new Color()
                    {
                        A = 255,
                        R = 255,
                        G = 255,
                        B = 255
                    });
                   // color = solid;
                    return solid;
                }


        }
        int _countClear = 100;
        private  void AddComment(TextBlock content, bool Myself)
        {
            if (_countClear!=0)
            {
                if (stack_Comment.Children.Count > _countClear)
                {
                    stack_Comment.Children.Clear();
                }
            }
            //TextBlock tx = new TextBlock();
            //tx.Margin = new Thickness(5);
            //tx.Text = content;
            //if (Myself)
            //{
            //    tx.Foreground = new SolidColorBrush(Colors.Blue);
            //}
            content.TextWrapping = TextWrapping.Wrap;
            content.IsTextSelectionEnabled = true;
            stack_Comment.Children.Add(content);

           
        }

        public class Model
        {
            public int code { get; set; }
            public string message { get; set; }
            public object data { get; set; }
            public object room { get; set; }

            public string text { get; set; }
            public object[] medal { get; set; }
            public object[] user_level { get; set; }

            public string timeline { get; set; }
            public string nickname { get; set; }
            public string uid { get; set; }
            public int svip { get; set; }
            public int vip { get; set; }
        }
        private async void GetPlayUrl()
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                string url = string.Format("http://live.bilibili.com/api/playurl?cid={0}&appkey=422fd9d7289a1dd9&rnd={1}", _roomid, new Random().Next(1, 9999));
                url += "&sign=" + ApiHelper.GetSign(url);
                // string results = await wc.GetResults(new Uri("http://live.bilibili.com/api/playurl?platform=h5&cid=" + rommID + "&rnd=" + new Random().Next(1, 9999)));
                string results = await WebClientClass.GetResults(new Uri(url));
                //JObject json = JObject.Parse(results);
                //   mediaElement.Source = new Uri((string)json["data"]);

                //results = await WebClientClass.GetResults_Phone(new Uri(url));
                MatchCollection mc = Regex.Matches(results, "<.*?url>(.*?)</.*?url>");
                if (mc.Count != 0)
                {
                    int i = 1;

                    List<LiveUrlListModel> ls = new List<LiveUrlListModel>();
                    foreach (Match item in mc)
                    {
                        ls.Add(new LiveUrlListModel()
                        {
                            url = item.Groups[1].Value.Replace("<![CDATA[", "").Replace("]]>", ""),
                            name = "源" + i
                        });
                        i++;
                    }
                    ls.Add(new LiveUrlListModel() {  name="H5源",url=await GetH5PlayUrl() });
                    cb_Source.ItemsSource = ls;
                    if (SettingHelper.Get_UseH5())
                    {
                        cb_Source.SelectedIndex = ls.Count-1;
                    }
                    else
                    {
                        cb_Source.SelectedIndex =0;
                    }

            
                }

                // string playUrl = Regex.Match(results, "<url>(.*?)</url>").Groups[1].Value;
                // playUrl = playUrl.Replace("<![CDATA[", "");
                // playUrl = playUrl.Replace("]]>", "");



                // mediaElement.Source = new Uri(playUrl);


            }
            catch (Exception)
            {
                messShow.Show("读取地址失败", 3000);
                //throw;
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }
        public class LiveUrlListModel
        {
            public string name { get; set; }
            public string url { get; set; }
        }

        private async void SetRoundPlayUrl()
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                pr_Load.Visibility = Visibility.Visible;
                string url = string.Format("http://live.bilibili.com/live/getRoundPlayVideo?_device=android&access_key={0}&appkey={1}&build=433000&mobi_app=android&platform=android&room_id={2}", ApiHelper.access_key,ApiHelper._appKey, _roomid);
                url += "&sign=" + ApiHelper.GetSign(url);
                string results =await WebClientClass.GetResults(new Uri(url));
                JObject obj = JObject.Parse(results);
                string result2 = await WebClientClass.GetResults(new Uri(obj["data"]["play_url"].ToString()));
                JObject obj2 = JObject.Parse(result2);
                mediaElement.Source = new Uri(obj2["durl"][0]["url"].ToString());


            }
            catch (Exception)
            {
                messShow.Show("读取轮播失败",2000);
                throw;
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }


        private async Task<string> GetH5PlayUrl()
        {
            try
            {
               
                string results = await WebClientClass.GetResults(new Uri("http://live.bilibili.com/api/playurl?platform=h5&cid=" + _roomid + "&rnd=" + new Random().Next(1, 9999)));
                // string results = await wc.GetResults(new Uri(url));
                JObject json = JObject.Parse(results);
                return (string)json["data"];
                //mediaElement.Source = new Uri((string)json["data"]);
            }
            catch (Exception)
            {
                return "";
               // messShow.Show("读取地址失败", 3000);
            }
          
        }

        private void btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            GetPlayUrl();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            txt_Comment.Text += ((Button)sender).Content.ToString();
        }

        private async void btn_AttUp_Click(object sender, RoutedEventArgs e)
        {
            if (!ApiHelper.IsLogin())
            {
                messShow.Show("请先登录！",2000);
            }
            try
            {
               
                string url;
                if (txt_guanzhu.Text == "关注")
                {
                    url = string.Format("https://account.bilibili.com/api/friend/attention/add?access_key={0}&appkey={1}&build=433000&mid={2}&platform=wp", ApiHelper.access_key, ApiHelper._appKey, (Video_UP.DataContext as LiveInfoModel).mid);
                }
                else
                {
                    url = string.Format("https://account.bilibili.com/api/friend/attention/del?access_key={0}&appkey={1}&build=433000&mid={2}&platform=wp", ApiHelper.access_key, ApiHelper._appKey, (Video_UP.DataContext as LiveInfoModel).mid);
                }
               
                url += "sign="+ApiHelper.GetSign(url);
                string result = await WebClientClass.GetResults(new Uri(url));
                JObject json = JObject.Parse(result);
                if ((int)json["code"] == 0)
                {
                    if (txt_guanzhu.Text == "关注")
                    {
                        txt_guanzhu.Text = "已关注";
                    }
                    else
                    {
                        txt_guanzhu.Text = "关注";
                    }
                }
                else
                {
                    messShow.Show("关注失败" + json["msg"], 2000);
                }

            }
            catch (Exception)
            {
                messShow.Show("关注时发生错误", 2000);
            }

        }

        private  void btn_Info_Click(object sender, RoutedEventArgs e)
        {
             cd.Visibility=Visibility.Visible;
        }

        private void btn_User_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserInfoPage),new object[] { (Video_UP.DataContext as LiveInfoModel).mid});
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            cd.Visibility = Visibility.Collapsed;
        }

        private void grid_Error_Tapped(object sender, TappedRoutedEventArgs e)
        {
            LoadRoomInfo();
        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         
            switch (pivot.SelectedIndex)
            {
                case 1:
                    list_Gift_Top.Visibility = Visibility.Visible;
                    if (list_Gift_Top.Items.Count == 0)
                    {
                        GetGiftTop(_roomid);
                    }
                    break;
                case 2:
                    list_Fans_Top.Visibility = Visibility.Visible;
                    if (list_Fans_Top.Items.Count == 0)
                    {
                        GetFansTop(_roomid);
                    }
                    break;
                case 3:
                    list_Zhuf_Top.Visibility = Visibility.Visible;
                    if (list_Zhuf_Top.Items.Count == 0)
                    {
                        GetZhufTop(_roomid);
                    }
                    break;
                default:
                    break;
            }
        }

        private void mediaElement_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            switch (mediaElement.CurrentState)
            {
                case MediaElementState.Closed:
                    _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Closed;
                 
                    break;
                case MediaElementState.Opening:
                  
                    break;
                case MediaElementState.Buffering:

                    break;
                case MediaElementState.Playing:
                    btn_Pause.Visibility = Visibility.Visible;
                    btn_Play.Visibility = Visibility.Collapsed;
                    _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Playing;

                    break;
                case MediaElementState.Paused:
                    btn_Pause.Visibility = Visibility.Collapsed;
                    btn_Play.Visibility = Visibility.Visible;
                    _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Paused;

                    break;
                case MediaElementState.Stopped:
                    btn_Play.Visibility = Visibility.Visible;
                    btn_Pause.Visibility = Visibility.Collapsed;
                    _systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Stopped;


                    break;
                default:
                    break;
            }
        }

        private void mediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            SystemMediaTransportControlsDisplayUpdater updater = _systemMediaTransportControls.DisplayUpdater;
            updater.Type = MediaPlaybackType.Video;
            // updater.MusicProperties.AlbumArtist = info.owner.name;
            updater.VideoProperties.Subtitle = txt_up.Text;
            updater.VideoProperties.Title = txt_title.Text;

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

        private void cb_Source_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_Source.SelectedItem == null)
            {
                return;
            }
            string playUrl = (cb_Source.SelectedItem as LiveUrlListModel).url;
            mediaElement.Source = new Uri(playUrl);



        }
        private FFmpegInteropMSS FFmpegMSS;
        private async void mediaElement_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            messShow.Show("将使用软解,卡顿请切换H5源",2000);
            try
            {
                if (cb_Source.SelectedItem==null)
                {
                    return;
                }
                PropertySet options = new PropertySet();
                mediaElement.Stop();
                string playUrl = (cb_Source.SelectedItem as LiveUrlListModel).url;
                await Task.Run(async () =>
                {

                    FFmpegMSS = FFmpegInteropMSS.CreateFFmpegInteropMSSFromUri(playUrl, SettingHelper.Get_ForceAudio(), SettingHelper.Get_ForceVideo(), options);
                    if (FFmpegMSS != null)
                    {
                        MediaStreamSource mss = FFmpegMSS.GetMediaStreamSource();
                        //Pass MediaStreamSource to Media Element
                        await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            mediaElement.SetMediaStreamSource(mss);
                            mediaElement.RealTimePlayback = true;
                        });
                    }
                    else
                    {
                        await new MessageDialog("无法播放此源直播，请尝试更换播放源").ShowAsync();
                    }
                });
            }
            catch (Exception)
            {

                await new MessageDialog("无法播放此源直播，请尝试更换播放源").ShowAsync();
            }
            
        }



        private async void GetGiftTop(string room_id)
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                list_Gift_Top.Items.Clear();
                
                string url = string.Format("http://live.bilibili.com/AppRoom/getGiftTop?_device=wp&appkey={0}&build=411005&access_key={1}&platform=android&room_id={2}&ts={3}", ApiHelper._appKey, ApiHelper.access_key, room_id, ApiHelper.GetTimeSpan);
                url += "&sign=" + ApiHelper.GetSign(url);
                string results = await WebClientClass.GetResults(new Uri(url));
                LiveRankModel model = JsonConvert.DeserializeObject<LiveRankModel>(results);
                if (model.code == 0)
                {
                    //可以查看个人排名
                    LiveRankModel info = JsonConvert.DeserializeObject<LiveRankModel>(model.data.ToString());
                    List<LiveRankModel> rank = JsonConvert.DeserializeObject<List<LiveRankModel>>(info.list.ToString());
                    int i = 0;
                    foreach (var item in rank)
                    {
                        switch (i)
                        {
                            case 0:
                                item.PColor = new SolidColorBrush(Colors.OrangeRed);
                                break;
                            case 1:
                                item.PColor = new SolidColorBrush(Colors.LightBlue);
                                break;
                            case 2:
                                item.PColor = new SolidColorBrush(Colors.Orange);
                                break;
                            default:
                                break;
                        }
                        item.rank = i + 1;
                        list_Gift_Top.Items.Add(item);
                        i++;
                    }

                }
                else
                {
                    //grid_Error.Visibility = Visibility.Visible;
                    messShow.Show(model.message, 2000);
                }
            }
            catch (Exception ex)
            {
                messShow.Show(ex.Message, 2000);
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }

        private async void GetFansTop(string room_id)
        {
            try
            {
                list_Fans_Top.Items.Clear();
                string url = string.Format("http://live.bilibili.com/AppRoom/medalRankList?_device=wp&appkey={0}&build=411005&access_key={1}&platform=android&room_id={2}&ts={3}", ApiHelper._appKey, ApiHelper.access_key, room_id, ApiHelper.GetTimeSpan);
                url += "&sign=" + ApiHelper.GetSign(url);
                string results = await WebClientClass.GetResults(new Uri(url));
                LiveRankModel model = JsonConvert.DeserializeObject<LiveRankModel>(results);
                if (model.code == 0)
                {
                    //可以查看个人排名
                    LiveRankModel info = JsonConvert.DeserializeObject<LiveRankModel>(model.data.ToString());
                    List<LiveRankModel> rank = JsonConvert.DeserializeObject<List<LiveRankModel>>(info.list.ToString());
                    int i = 0;
                    foreach (var item in rank)
                    {
                        switch (i)
                        {
                            case 0:
                                item.PColor = new SolidColorBrush(Colors.OrangeRed);
                                break;
                            case 1:
                                item.PColor = new SolidColorBrush(Colors.LightBlue);
                                break;
                            case 2:
                                item.PColor = new SolidColorBrush(Colors.Orange);
                                break;
                            default:
                                break;
                        }
                        item.rank = i + 1;
                        list_Fans_Top.Items.Add(item);
                        i++;
                    }

                }
                else
                {
                    //grid_Error.Visibility = Visibility.Visible;
                    messShow.Show(model.message,2000);
                }
            }
            catch (Exception ex)
            {
                messShow.Show(ex.Message, 2000);
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }
        private async void GetZhufTop(string room_id)
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                list_Zhuf_Top.Items.Clear();
                string url = string.Format("http://live.bilibili.com/AppRoom/opTop?_device=android&access_key={0}&appkey={1}&build=434000&mobi_app=android&platform=android&room_id={2}&scale=xxhdpi&type=springfestival", ApiHelper.access_key, ApiHelper._appKey_Android, _roomid);
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                string results = await WebClientClass.GetResults(new Uri(url));
                LiveRankModel model = JsonConvert.DeserializeObject<LiveRankModel>(results);
                if (model.code == 0)
                {
                    //可以查看个人排名
                    LiveRankModel info = JsonConvert.DeserializeObject<LiveRankModel>(model.data.ToString());
                    List<LiveRankModel> rank = JsonConvert.DeserializeObject<List<LiveRankModel>>(info.list.ToString());
                    int i = 0;
                    foreach (var item in rank)
                    {
                        switch (i)
                        {
                            case 0:
                                item.PColor = new SolidColorBrush(Colors.OrangeRed);
                                break;
                            case 1:
                                item.PColor = new SolidColorBrush(Colors.LightBlue);
                                break;
                            case 2:
                                item.PColor = new SolidColorBrush(Colors.Orange);
                                break;
                            default:
                                break;
                        }
                        item.rank = i + 1;
                        list_Zhuf_Top.Items.Add(item);
                        i++;
                    }

                }
                else
                {
                    //grid_Error.Visibility = Visibility.Visible;
                    messShow.Show(model.message, 2000);
                }
            }
            catch (Exception ex)
            {
                messShow.Show(ex.Message, 2000);
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }
        private  void cb_rank_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void btn_SendComment_Click(object sender, RoutedEventArgs e)
        {
            SendDanmu();
        }
        public async void SendDanmu()
        {
            if (!ApiHelper.IsLogin())
            {
                messShow.Show("请先登录！",2000);
                return;
            }
            if (txt_Comment.Text.Length == 0)
            {
                messShow.Show("弹幕内容不能为空！",2000);
                return;
            }
            if (txt_Comment.Text.Length > 30)
            {
                messShow.Show("内容太长了- -!", 2000);
                return;
            }
            try
            {
                btn_SendComment.IsEnabled = false;
                DateTime timeStamp = new DateTime(1970, 1, 1); //得到1970年的时间戳
                long time = (DateTime.UtcNow.Ticks - timeStamp.Ticks) / 10000000;
                string sendText = string.Format("color=16777215&fontsize=25&mode=1&msg={0}&rnd={1}&roomid={2}", txt_Comment.Text, time, _roomid);
                string result = await WebClientClass.PostResults(new Uri("http://live.bilibili.com/msg/send"), sendText);
                JObject jb = JObject.Parse(result);
                if ((int)jb["code"] == 0)
                {
                    //AddComment(new TextBlock() { Text= "已发送：" + txt_Comment.Text }, true);
                    //if (LoadDanmu)
                    //{
                    //    danmu.AddGunDanmu(new Controls.MyDanmaku.DanMuModel() { DanText = txt_Comment.Text, DanSize = "25", _DanColor = "16777215" }, true);
                    //}
                    txt_Comment.Text = string.Empty;

                }
                else
                {
                    messShow.Show("弹幕发送失败 "+ jb["message"].ToString(), 2000);

                }

            }
            catch (Exception)
            {
                messShow.Show("弹幕发送出现错误 ", 2000);
            }
            finally
            {
                btn_SendComment.IsEnabled = true;
            }
        }

        private void btn_ShareUrl_Click(object sender, RoutedEventArgs e)
        {
            Windows.ApplicationModel.DataTransfer.DataPackage pack = new Windows.ApplicationModel.DataTransfer.DataPackage();
            pack.SetText(string.Format("哔哩哔哩直播:{0}\r\n地址：http://live.bilibili.com/{1}\r\n -分享自哔哩哔哩UWP" , (this.DataContext as LiveInfoModel).title, _roomid));
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(pack); // 保存 DataPackage 对象到剪切板
            Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
            messShow.Show("已将内容复制到剪切板", 3000);
        }

        private void btn_ShareData_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (ROUND)
            {
                SetRoundPlayUrl();
            }
            else
            {
                LoadRoomInfo();
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            grid_NotFull_SizeChanged(sender, e);
        }
        
        private void grid_NotFull_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           // double _fontSize = 25;
            if (grid_NotFull.ActualWidth >= 600)
            {
                if (slider_DanmuSize.Value != 0)
                {
                    if (slider_DanmuSize.Value > 14)
                    {
                        danmu.fontSize = slider_DanmuSize.Value;
                    }
                    else
                    {
                        danmu.fontSize = 14;
                    }
                }
                
            }
            else
            {
                double d = grid_NotFull.ActualWidth / 40;
                if (d<14)
                {
                    d = 14;
                }
                danmu.fontSize = d;
               
            }
            if (slider_DanmuSpeed.Value!=0)
            {
                danmu.Speed = Convert.ToInt32(grid_NotFull.ActualWidth / slider_DanmuSpeed.Value);
            }
           
            danmu.SetJJ();
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

        private void gridview_Gifts_ItemClick(object sender, ItemClickEventArgs e)
        {
            var info = e.ClickedItem as LiveInfoModel;
            if (info.coin_type.silver!=null&& info.coin_type.silver.Length!=0)
            {
                rb_Slider.Visibility = Visibility.Visible;
                rb_Slider.IsChecked = true;
            }
            else
            {
                rb_Slider.Visibility = Visibility.Collapsed;
                rb_Gold.IsChecked = true;
            }
            cd_BuyGiftNum.DataContext = info;
            cd_BuyGiftNum.Visibility = Visibility.Visible;
            
        }

        private  void gridview_myGifts_ItemClick(object sender, ItemClickEventArgs e)
        {
            var info = e.ClickedItem as LiveMyGiftsModel;
            maxNum = info.gift_num;
            cd_GiftNum.DataContext = info;
            cd_GiftNum.Visibility = Visibility.Visible;

           
        }
        private async void SendMyGift(string giftId,int Num,string bag_id)
        {
            try
            {
                if (Num==0)
                {
                    messShow.Show("数量不能为0",3000);
                    return;
                }
                pr_Load.Visibility = Visibility.Visible;

                string url = string.Format("http://live.bilibili.com/AppBag/send?_device=android&_hwid={0}&_ulv=10000&access_key={1}&appkey={2}&build=433000&mobi_app=android&platform=android", ApiHelper._hwid,ApiHelper.access_key, ApiHelper._appKey_Android);
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                string par = string.Format("giftId={0}&num={1}&ruid={2}&roomid={3}&timestamp={4}&bag_id={5}&rnd={6}&",giftId, Num, (Video_UP.DataContext as LiveInfoModel).mid,_roomid,ApiHelper.GetTimeSpan_2, bag_id, "5772223"+new Random().Next(10,99));
                string results = await WebClientClass.PostResults(new Uri(url), par);
                JObject m = JObject.Parse(results);
                if ((int)m["code"] == 0)
                {
                    messShow.Show("操作成功", 3000);
                    GetMyGifts();
                }
                else
                {
                    messShow.Show(m["message"].ToString(), 3000);
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
                pr_Load.Visibility = Visibility.Collapsed;
                cd_GiftNum.Visibility = Visibility.Collapsed;
            }
        }



        int maxNum = 0;
        private void txt_GiftNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            int num = 0;
            if (!int.TryParse(txt_GiftNum.Text, out num))
            {
                txt_GiftNum.Text = "1";
            }
            else
            {
                if (num> maxNum)
                {
                    txt_GiftNum.Text = maxNum.ToString();
                }
            }
            
        }


        private void btn_cnacelSend_Click(object sender, RoutedEventArgs e)
        {
            cd_GiftNum.Visibility = Visibility.Collapsed;
        }

        private void btn_SendOk_Click(object sender, RoutedEventArgs e)
        {
            int onum = 0;
            if (!int.TryParse(txt_GiftNum.Text, out onum))
            {
                messShow.Show("错误的数量", 3000);
                return;
            }
            var info = (sender as Button).DataContext as LiveMyGiftsModel;

            SendMyGift(info.gift_id.ToString(), onum, info.id);
           
        }

        private void txt_BuyGiftNum_TextChanged(object sender, TextChangedEventArgs e)
        {
            int num = 0;
            if (!int.TryParse(txt_BuyGiftNum.Text, out num))
            {
                txt_BuyGiftNum.Text = "1";
            }

        }

        private void btn_cnacelSend_Buy_Click(object sender, RoutedEventArgs e)
        {
            cd_BuyGiftNum.Visibility = Visibility.Collapsed;
        }
        private async void SendBuyGift(string giftid,int num)
        {
            try
            {
                if (num == 0)
                {
                    messShow.Show("数量不能为0", 3000);
                    return;
                }
                pr_Load.Visibility = Visibility.Visible;
                string type = "silver";
                if (rb_Gold.IsChecked.Value)
                {
                    type = "gold";
                }

                string url = string.Format("http://live.bilibili.com/mobile/sendGift?_device=android&_hwid={0}&_ulv=10000&access_key={1}&appkey={2}&build=433000&coinType={3}&giftId={4}&mobi_app=android&num={5}&platform=android&rnd={6}&roomid={7}&ruid={8}", 
                    ApiHelper._buvid,ApiHelper.access_key, ApiHelper._appKey_Android,type,giftid,num, new Random().Next(10000, 99999) + new Random().Next(10000, 99999),_roomid, (Video_UP.DataContext as LiveInfoModel).mid);
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                string results = await WebClientClass.GetResults(new Uri(url));
                JObject m = JObject.Parse(results);
                if ((int)m["code"] == 0)
                {
                    messShow.Show("操作成功", 3000);
                  
                }
                else
                {
                    messShow.Show(m["message"].ToString(), 3000);
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
                pr_Load.Visibility = Visibility.Collapsed;
                cd_BuyGiftNum.Visibility = Visibility.Collapsed;
                LoadInfo();
            }
        }



        private void btn_SendOk_Buy_Click(object sender, RoutedEventArgs e)
        {
            int onum = 0;
            if (!int.TryParse(txt_BuyGiftNum.Text, out onum))
            {
                messShow.Show("错误的数量", 3000);
                return;
            }
            var info = (sender as Button).DataContext as LiveInfoModel;
            SendBuyGift(info.id, onum);
           
        }


        private async void LoadInfo()
        {
            try
            {
                if (!ApiHelper.IsLogin())
                {
                    return;
                }
                pr_Load.Visibility = Visibility.Visible;

                string url = string.Format("http://live.bilibili.com/mobile/getUser?_device=android&access_key={0}&appkey={1}&build=434000&mobi_app=android&platform=android", ApiHelper.access_key, ApiHelper._appKey_Android);
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                string results = await WebClientClass.GetResults(new Uri(url));
                LiveCenterModel m = JsonConvert.DeserializeObject<LiveCenterModel>(results);
                if (m.code == 0)
                {
                    txt_gold.Text = m.data.gold.ToString();
                    txt_silver.Text= m.data.silver.ToString();
                  
                }
                else
                {
                    messShow.Show(m.message, 3000);
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
                    messShow.Show("读取余额发生错误\r\n" + ex.Message, 3000);
                }
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;

            }
        }



        bool DanmuOpen = true;
        private void btn_CloseDanmu_Click(object sender, RoutedEventArgs e)
        {
            if (DanmuOpen)
            {
                danmu.ClearDanmu();
                btn_CloseDanmu.Foreground = new SolidColorBrush(Colors.Gray);
                DanmuOpen = false;
            }
            else
            {
                btn_CloseDanmu.Foreground = new SolidColorBrush(Colors.White);
                DanmuOpen = true;
            }
        }

        private void sw_H5_Toggled(object sender, RoutedEventArgs e)
        {
            SettingHelper.Set_UseH5(sw_H5.IsOn);
        }

        private void cb_ClaerLiveComment_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _countClear = SettingHelper.Get_ClearLiveComment() * 100;
            SettingHelper.Set_ClearLiveComment(cb_ClaerLiveComment.SelectedIndex);
        }

        private void slider_DanmuSize_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            grid_NotFull_SizeChanged(sender, null);
            SettingHelper.Set_LDMSize(slider_DanmuSize.Value);
        }
     
        private void slider_DanmuSpeed_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            grid_NotFull_SizeChanged(sender, null);
            SettingHelper.Set_LDMSpeed(slider_DanmuSpeed.Value);
        }

        private void slider_DanmuTran_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            danmu.Tran = slider_DanmuTran.Value / 100;
            SettingHelper.Set_LDMTran(slider_DanmuTran.Value);
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
            SettingHelper.Set_LDMFont(cb_Font.SelectedIndex);
        }
        bool _LoadDMGift = true;
        private void sw_GiftDanmu_Toggled(object sender, RoutedEventArgs e)
        {
            _LoadDMGift = sw_GiftDanmu.IsOn;
            SettingHelper.Set_LDMGift(sw_GiftDanmu.IsOn);
        }

        private async void btn_Setting_Click(object sender, RoutedEventArgs e)
        {
            cd_setting.Visibility = Visibility.Visible;
            await cd_setting.ShowAsync();
        }
        private void LoadSetting()
        {
            slider_DanmuSize.Value = SettingHelper.Get_LDMSize();
            slider_DanmuTran.Value = SettingHelper.Get_LDMTran();
            slider_DanmuSpeed.Value = SettingHelper.Get_LDMSpeed();
            cb_Font.SelectedIndex = SettingHelper.Get_LDMFont();
            sw_H5.IsOn = SettingHelper.Get_UseH5();
            cb_ClaerLiveComment.SelectedIndex = SettingHelper.Get_ClearLiveComment();
            sw_GiftDanmu.IsOn = SettingHelper.Get_LDMGift();
        }

        private void btn_full_Click(object sender, RoutedEventArgs e)
        {
            btn_exitFull.Visibility = Visibility.Visible;
            btn_full.Visibility = Visibility.Collapsed;
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            DisplayInformation.AutoRotationPreferences = (DisplayOrientations)5;

            column_2.Width = new GridLength(0);
        
            row_2.Height = new GridLength(0);
            grid_top.Height = 0;


            //Grid.SetRow(grid_Info, 0);
            //Grid.SetRowSpan(grid_Info, 2);
            Grid.SetColumn(grid_NotFull, 0);
            Grid.SetColumnSpan(grid_NotFull, 2);
            grid_Info.BorderThickness = new Thickness(0,0,0,0);
           // if (this.ActualWidth>=600)
          //  {
              grid_Info.Visibility = Visibility.Collapsed;
            Video_UP.Visibility = Visibility.Collapsed;
            // }
        }

        private void btn_exitFull_Click(object sender, RoutedEventArgs e)
        {
            btn_exitFull.Visibility = Visibility.Collapsed;
            btn_full.Visibility = Visibility.Visible;
           
            DisplayInformation.AutoRotationPreferences =  DisplayOrientations.None;
            ApplicationView.GetForCurrentView().ExitFullScreenMode();

            grid_top.Height = 48;
            grid_Info.Visibility = Visibility.Visible;
            Video_UP.Visibility = Visibility.Visible;
            bool phone = false;
            if (!SettingHelper.IsPc()&&(DisplayInformation.GetForCurrentView().CurrentOrientation== DisplayOrientations.Portrait|| DisplayInformation.GetForCurrentView().CurrentOrientation == DisplayOrientations.PortraitFlipped))
            {
                phone = true;
            }


            if (this.ActualWidth>=600&&!phone)
            {
                Grid.SetColumn(grid_Info, 1);
                Grid.SetColumn(grid_NotFull, 0);
                Grid.SetColumnSpan(grid_NotFull, 1);

                column_2.Width =new GridLength(0.3, GridUnitType.Star);
             
                row_2.Height = GridLength.Auto;
                grid_Info.BorderThickness = new Thickness(1, 0, 0, 0);
            }
            else
            {
                Grid.SetColumn(grid_Info, 0);
           
                Grid.SetColumn(grid_NotFull, 0);
                Grid.SetColumnSpan(grid_NotFull, 2);

                Grid.SetRow(grid_NotFull, 0);

                Grid.SetRow(grid_Info, 2);
                
                column_2.Width = GridLength.Auto;
                
                 row_2.Height = new GridLength(0.6, GridUnitType.Star);
                 grid_Info.BorderThickness = new Thickness(0, 1, 0, 0);
            }
        }

        private void mediaElement_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (bottom.Visibility==Visibility.Visible)
            {
                bottom.Visibility = Visibility.Collapsed;
            }
            else
            {
                bottom.Visibility = Visibility.Visible;
            }
        }

        private void mediaElement_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            bottom.Visibility = Visibility.Visible;
        }

        private void mediaElement_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            bottom.Visibility = Visibility.Collapsed;
        }

        private void txt_Comment_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key== Windows.System.VirtualKey.Enter)
            {
                SendDanmu();
            }
        }
    }
    public class LiveRankModel
    {
        public int code { get; set; }
        public string message { get; set; }
        public object data { get; set; }
        public object list { get; set; }
        public string uid { get; set; }
        public string uname { get; set; }
        public string coin { get; set; }
        public int rank { get; set; }
        public string medal_name { get; set; }//前缀
        public int level { get; set; }
        public string score { get; set; }
        public string color { get; set; }
        public SolidColorBrush PColor
        { get; set; }
        //用于颜色
        public SolidColorBrush DColor
        {
            get
            {
                try
                {
                    color = Convert.ToInt32(color).ToString("X2");
                    if (color.StartsWith("#"))
                        color = color.Replace("#", string.Empty);
                    int v = int.Parse(color, System.Globalization.NumberStyles.HexNumber);
                    SolidColorBrush solid = new SolidColorBrush(new Color()
                    {
                        A = Convert.ToByte(125),
                        R = Convert.ToByte((v >> 16) & 255),
                        G = Convert.ToByte((v >> 8) & 255),
                        B = Convert.ToByte((v >> 0) & 255)
                    });
                    return solid;
                }
                catch (Exception)
                {
                    SolidColorBrush solid = new SolidColorBrush(new Color()
                    {
                        A = 125,
                        R = 255,
                        G = 255,
                        B = 255
                    });
                    return solid;
                }

            }
        }

    }





}
