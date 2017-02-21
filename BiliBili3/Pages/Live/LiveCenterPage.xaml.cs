using BiliBili3.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace BiliBili3.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LiveCenterPage : Page
    {
        public LiveCenterPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode== NavigationMode.New)
            {
                LoadInfo();

            }
        }
        private async void LoadInfo()
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                
                string url = string.Format("http://live.bilibili.com/mobile/getUser?_device=android&access_key={0}&appkey={1}&build=434000&mobi_app=android&platform=android", ApiHelper.access_key, ApiHelper._appKey_Android);
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                string results = await WebClientClass.GetResults(new Uri(url));
                LiveCenterModel m = JsonConvert.DeserializeObject<LiveCenterModel>(results);
                if (m.code == 0)
                {

                    m.data.uname = ApiHelper.userInfo.name;
                    m.data.pic = ApiHelper.userInfo.face;
                  
                    if (m.data.ShowVip)
                    {
                        isvip.Visibility = Visibility.Visible;
                        novip.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        isvip.Visibility = Visibility.Collapsed;
                        novip.Visibility = Visibility.Visible;
                    }
                    this.DataContext = m.data;
                    bor_UL.Background = GetColor(m.data.user_level_color);

                    if (m.data.medal != null)
                    {
                        bor_Medal.Visibility = Visibility.Visible;
                        bor_Medal.Background = GetColor(m.data.medal.color);
                    }
                    else
                    {
                        bor_Medal.Visibility = Visibility.Collapsed;
                    }

                    if (m.data.isSign==1)
                    {
                        btn_sign.Visibility = Visibility.Collapsed;
                        signed.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        btn_sign.Visibility = Visibility.Visible;
                        signed.Visibility = Visibility.Collapsed;
                    }
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
                    messShow.Show("发生错误\r\n" + ex.Message, 3000);
                }
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
                
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
        private void btn_myroom_Click(object sender, RoutedEventArgs e)
        {
            var info = this.DataContext as LiveCenterModel;
            if (info==null)
            {
                return;
            }
            if (info.room_id=="0")
            {
                return;
            }
            MessageCenter.SendNavigateTo(NavigateMode.Play, typeof(LiveRoomPage), info.room_id);
        }

        private void btn_myfeed_Click(object sender, RoutedEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(LiveFeedPage));
        }

        private void btn_myhistory_Click(object sender, RoutedEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(LiveHistoryPage));
        }

        private async void btn_sign_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string url = string.Format("http://live.bilibili.com/sign/doSign?rnd={0}", new Random().Next(1, 9999));
                string results = await WebClientClass.GetResults(new Uri(url));
                SignModel model = JsonConvert.DeserializeObject<SignModel>(results);
                if (model.code == 0)
                {
                    SignModel data = JsonConvert.DeserializeObject<SignModel>(model.data.ToString());

                    btn_sign.Visibility = Visibility.Collapsed;
                    signed.Visibility = Visibility.Visible;

                    await new MessageDialog(data.text).ShowAsync();
                    
                }
                else
                {
                    await new MessageDialog(model.msg).ShowAsync();
                }
            }
            catch (Exception ex)
            {
                await new MessageDialog("签到时发生错误\r\n"+ ex.Message).ShowAsync();
            }

        }

        private void btn_buyVIP_Click(object sender, RoutedEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(WebPage), "http://live.bilibili.com/i#to-vip");
        }

        private void btn_Capsuletoy_Click(object sender, RoutedEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(WebPage), "http://live.bilibili.com/pages/playground/index#!/capsule-toy");
        }

        private void btn_DHH_Click(object sender, RoutedEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(WebPage), "http://live.bilibili.com/hd/guard-desc?menu=0");
        }

        private void btn_myMedal_Click(object sender, RoutedEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(LiveMyMedalPage));
        }

        private void btn_myTitle_Click(object sender, RoutedEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(LiveMyTitlePage));
        }

        private void btn_Hjjv_Click(object sender, RoutedEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info,typeof(WebPage), "http://live.bilibili.com/i/awards");
        }

        private void btn_buyGold_Click(object sender, RoutedEventArgs e)
        {
            messShow.Show("暂时没开发，请到其它平台操作",3000);
        }

        private void btn_buySlider_Click(object sender, RoutedEventArgs e)
        {
            messShow.Show("暂时没开发，请到其它平台操作", 3000);
        }
    }
}
