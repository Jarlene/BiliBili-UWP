using BiliBili3.Models;
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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace BiliBili3.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MyFollowsBangumiPage : Page
    {
        public MyFollowsBangumiPage()
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
            if (e.NavigationMode== NavigationMode.New&&list.Items.Count==0)
            {
                await Task.Delay(200);
                _page = 1;
                LoadMy();
            }
        }

        int _page = 1;
        bool _loading = false;
        private async void LoadMy()
        {
            try
            {
                _loading = true;
                pr_Load.Visibility = Visibility.Visible;
                string url = string.Format("https://bangumi.bilibili.com/api/get_concerned_season?access_key={0}&appkey={1}&build=411005&mid={2}&mobi_app=android&page={4}&pagesize=20&platform=android&ts={3}000", ApiHelper.access_key, ApiHelper._appKey, ApiHelper.GetUserId(), ApiHelper.GetTimeSpan,_page);
                url += "&sign=" + ApiHelper.GetSign(url);
                string results = await WebClientClass.GetResults(new Uri(url));
                MyBangumiModel m = JsonConvert.DeserializeObject<MyBangumiModel>(results);
                if (m.code == 0)
                {
                    if (m.result.Count==0)
                    {
                        messShow.Show("加载完了...", 3000);
                        return;
                    }
                    m.result.ForEach(x=>list.Items.Add(x));
                    // list_ban_mine.ItemsSource = m.result;
                    _page++;
                }
                else
                {
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

                    messShow.Show("读取追番失败了", 3000);
                }
            }
            finally
            {
                _loading = false;
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }

        private void sv_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sv.VerticalOffset == sv.ScrollableHeight)
            {
                if (!_loading)
                {
                 
                    LoadMy();
                }
            }
        }

        private void list_ItemClick(object sender, ItemClickEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(BanInfoPage),( e.ClickedItem as MyBangumiModel).season_id.ToString());
        }

        private void b_btn_Refresh_Click(object sender, RoutedEventArgs e)
        {
            _page = 1;
            LoadMy();
        }
    }
}
