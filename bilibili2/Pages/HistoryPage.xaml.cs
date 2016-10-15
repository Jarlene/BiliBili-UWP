using bilibili2.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class HistoryPage : Page
    {
        public delegate void GoBackHandler();
        public event GoBackHandler BackEvent;
        public HistoryPage()
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
        private int pageNum_His = 1;
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            bg.Color = ((SolidColorBrush)this.Frame.Tag).Color;
            await Task.Delay(200);
            if (e.NavigationMode== NavigationMode.New)
            {
                pageNum_His = 1;
                User_ListView_History.Items.Clear();
                 GetHistoryInfo();
            }
           
        }

        private void User_ListView_History_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(VideoInfoPage), ((GetHistoryModel)e.ClickedItem).aid);
        }
        bool More = true;
        private  void sv_Ho_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if ((sender as ScrollViewer).VerticalOffset == (sender as ScrollViewer).ScrollableHeight)
            {
                if (More)
                {
                    GetHistoryInfo();
                }
            }

        }

        private async void GetHistoryInfo()
        {
            try
            {
                More = false;
                pro_Load.Visibility = Visibility.Visible;
                UserClass getLogin = new UserClass();
                User_load_more_history.Content = "正在加载";
                List<GetHistoryModel> lsModel = await getLogin.GetHistory(pageNum_His);
                if (lsModel != null)
                {
                    foreach (GetHistoryModel item in lsModel)
                    {
                        User_ListView_History.Items.Add(item);
                    }
                }
                else
                {
                    User_load_more_history.Visibility = Visibility.Collapsed;
                }
                User_load_more_history.Content = "";
                pageNum_His++;
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.Message).ShowAsync();
            }
            finally
            {
                pro_Load.Visibility = Visibility.Collapsed;
                More = true;
            }
          
        }
        WebClientClass wc;
        private async void btn_ClearHistory_Click(object sender, RoutedEventArgs e)
        {
            //http://api.bilibili.com/x/v2/history/clear?_device=android&_hwid=bd2e7034b953cffe&_ulv=10000&access_key=1a8cd71c9830c73819989dade872ff55&appkey=1d8b6e7d45233436&build=421000&mobi_app=android&platform=android&sign=69e2689cd0b82f9b67aef4624360ae1b
            try
            {
                wc = new WebClientClass();
                string url = string.Format("http://api.bilibili.com/x/v2/history/clear?_device=android&access_key={0}&appkey={1}&build=421000&mobi_app=android&platform=android",ApiHelper.access_key,ApiHelper._appKey_Android);
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                string results = await wc.PostResults(new Uri(url),"");
                User_ListView_History.Items.Clear();
                pageNum_His = 1;
            }
            catch (Exception)
            {
                
            }

        }
    }
}
