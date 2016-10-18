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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace bilibili2.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ActivityPage : Page
    {
        public delegate void GoBackHandler();
        public event GoBackHandler BackEvent;
        public ActivityPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }
        //private void btn_back_Click(object sender, RoutedEventArgs e)
        //{
        //    if (this.Frame.CanGoBack)
        //    {
        //        this.Frame.GoBack();
        //    }
        //    else
        //    {
        //        BackEvent();
        //    }
        //}
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            bg.Color = ((SolidColorBrush)this.Frame.Tag).Color;
            if (e.NavigationMode== NavigationMode.New)
            {
                GetInfo();
            }
        }
        WebClientClass wc;
        private async void GetInfo()
        {
            try
            {
                wc = new WebClientClass();
                pr_Load.Visibility = Visibility.Visible;
                string url = string.Format("http://api.bilibili.com/event/getlist?appkey={0}&build=422000&mobi_app=android&page=1&pagesize=20&platform=android&ts={1}",ApiHelper._appKey_Android,ApiHelper.GetTimeSpen);
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                string results = await wc.GetResults(new Uri(url));
                ActivityModel m = Newtonsoft.Json.JsonConvert.DeserializeObject<ActivityModel>(results);
                grid_View.ItemsSource = m.list;
            }
            catch (Exception)
            {
                messShow.Show("读取失败了",2000);
                //throw;
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }
        private void grid_View_ItemClick(object sender, ItemClickEventArgs e)
        {
            ActivityModel m = e.ClickedItem as ActivityModel;
            this.Frame.Navigate(typeof(WebViewPage),m.link);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int i = Convert.ToInt32(this.ActualWidth / 400);
            bor_Width.Width = this.ActualWidth / i - 12;
        }
    }
    public class ActivityModel
    {

        public int code { get; set; }
        public List<ActivityModel> list { get; set; }
        public string title { get; set; }
        public string cover { get; set; }
        public string link { get; set; }
        public int state { get; set; }
        public Visibility state_0
        {
            get
            {
                if (state==0)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }
        public Visibility state_1
        {
            get
            {
                if (state == 1)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }
    }


}
