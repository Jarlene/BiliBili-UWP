using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace bilibili2.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class TopicPage : Page
    {
        public delegate void GoBackHandler();
        public event GoBackHandler BackEvent;
        public TopicPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
           
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
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            bg.Color = ((SolidColorBrush)this.Frame.Tag).Color;
            if (e.NavigationMode== NavigationMode.New)
            {
                GetTopic();
            }
        }
        WebClientClass wc;
        int page = 1;
        private async void GetTopic()
        {
            try
            {
                IsLoading = true;
                btn_More_Video.Visibility = Visibility.Collapsed;
                wc = new WebClientClass();
                pr_Load.Visibility = Visibility.Visible;
                string url = string.Format("http://api.bilibili.com/topic/getlist?access_key={0}&appkey={1}&build=424000&mobi_app=android&page={2}&pagesize=20&platform=android&ts={3}", ApiHelper.access_key, ApiHelper._appKey_Android, page, ApiHelper.GetTimeSpen);
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                string results = await wc.GetResults(new Uri(url));
                TopicModel m = Newtonsoft.Json.JsonConvert.DeserializeObject<TopicModel>(results);

                m.list.ForEach(x =>
                {
                    if (x.link.Length!=0)
                    {
                        grid_View.Items.Add(x);
                    }
                }
                );
                page++;
                //grid_View.ItemsSource = m.list;
            }
            catch (Exception)
            {
                messShow.Show("读取失败了", 2000);
                //throw;
            }
            finally
            {
                IsLoading = false;
                btn_More_Video.Visibility = Visibility.Visible;
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }

        private void list_Topic_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (Regex.IsMatch(((TopicModel)e.ClickedItem).link, "/video/av(.*)?[/|+](.*)?"))
            {
                string a = Regex.Match(((TopicModel)e.ClickedItem).link, "/video/av(.*)?[/|+](.*)?").Groups[1].Value;
                this.Frame.Navigate(typeof(VideoInfoPage), a);
            }
            else
            {
                if (Regex.IsMatch(((TopicModel)e.ClickedItem).link, @"live.bilibili.com/(.*?)"))
                {
                    string a = Regex.Match(((TopicModel)e.ClickedItem).link + "a", "live.bilibili.com/(.*?)a").Groups[1].Value;
                    // livePlayVideo(a);
                }
                else
                {
                    this.Frame.Navigate(typeof(WebViewPage), ((TopicModel)e.ClickedItem).link);
                }
            }
        }
        bool IsLoading = true;
        private void sc_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sc.VerticalOffset == sc.ScrollableHeight)
            {
                if (!IsLoading)
                {
                    GetTopic();
                }
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int i = Convert.ToInt32(this.ActualWidth / 400);
            bor_Width.Width = this.ActualWidth / i - 12;
        }

        private void btn_More_Video_Click(object sender, RoutedEventArgs e)
        {
            if (!IsLoading)
            {
                GetTopic();
            }
        }
    }
}
