using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Text;
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
    public sealed partial class AllLivePage : Page
    {
        public delegate void GoBackHandler();
        public event GoBackHandler BackEvent;
        public AllLivePage()
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
        }
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualWidth <= 500)
            {
                ViewBox_num.Width = ActualWidth / 2 - 20;
            }
            else
            {
                int i = Convert.ToInt32(ActualWidth / 200);
                ViewBox_num.Width = ActualWidth / i-15;
                //ViewBox2_num.Width = 200;
            }

        }

        bool Hot = false;
        bool Yz = false;
        bool Sh = false;
        bool Dj = false;
        bool Wl = false;
        bool dzjz = false;
        bool Dy = false;

        int PageNum = 1;
        public async void GetZBInfo()
        {
            try
            {
                CanLoad = false;
                pr_Load.Visibility = Visibility.Visible;
                WebClientClass wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://api.bilibili.com/live/room_list?page=" + PageNum + "&pagesize=20&status=LIVE"));
                InfoModel model = new InfoModel();
                model = JsonConvert.DeserializeObject<InfoModel>(results);
                JObject json = JObject.Parse(model.list.ToString());
                List<InfoModel> ReList = new List<InfoModel>();
                for (int i = 0; i < 20; i++)
                {
                    live_HOT.Items.Add(new InfoModel
                    {
                        room_id = (string)json[i.ToString()]["room_id"],
                        title = (string)json[i.ToString()]["title"],
                        cover = (string)json[i.ToString()]["cover"],
                        uname = (string)json[i.ToString()]["uname"],
                        online = (string)json[i.ToString()]["online"],
                        face = (string)json[i.ToString()]["face"],
                    });
                }
                Hot = true;
                CanLoad = true;
                PageNum++;
                if (PageNum==2)
                {
                    GetZBInfo();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
                
            }
        }

        public async void GetYz()
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                WebClientClass wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://live.bilibili.com/area/liveList?area=otaku&order=online&page=1"));
                InfoModel data = JsonConvert.DeserializeObject<InfoModel>(results);
                //JObject json = JObject.Parse(results);
                List<InfoModel> model = JsonConvert.DeserializeObject<List<InfoModel>>(data.data.ToString());
                foreach (InfoModel item in model)
                {
                    live_Yz.Items.Add(item);
                }
                Yz = true;
            }
            catch (Exception)
            {
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }

        public async void GetSh()
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                WebClientClass wc = new WebClientClass();
                //http://live.bilibili.com/area/liveList?area=ent-life&order=online&page=1
                string results = await wc.GetResults(new Uri("http://live.bilibili.com/area/liveList?area=ent-life&order=online&page=1"));
                InfoModel data = JsonConvert.DeserializeObject<InfoModel>(results);
                //JObject json = JObject.Parse(results);
                List<InfoModel> model = JsonConvert.DeserializeObject<List<InfoModel>>(data.data.ToString());
                foreach (InfoModel item in model)
                {
                    live_Sh.Items.Add(item);
                }
                Sh = true;
            }
            catch (Exception)
            {
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }

        public async void GetDj()
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                WebClientClass wc = new WebClientClass();
                // http://live.bilibili.com/area/liveList?area=single&order=online&page=1
                string results = await wc.GetResults(new Uri("http://live.bilibili.com/area/liveList?area=single&order=online&page=1"));
                InfoModel data = JsonConvert.DeserializeObject<InfoModel>(results);
                //JObject json = JObject.Parse(results);
                List<InfoModel> model = JsonConvert.DeserializeObject<List<InfoModel>>(data.data.ToString());
                foreach (InfoModel item in model)
                {
                    live_Dj.Items.Add(item);
                }
                Dj = true;
            }
            catch (Exception)
            {
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }

        public async void GetWl()
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                WebClientClass wc = new WebClientClass();
                //http://live.bilibili.com/area/liveList?area=online&order=online&page=1
                string results = await wc.GetResults(new Uri("http://live.bilibili.com/area/liveList?area=online&order=online&page=1"));
                InfoModel data = JsonConvert.DeserializeObject<InfoModel>(results);
                //JObject json = JObject.Parse(results);
                List<InfoModel> model = JsonConvert.DeserializeObject<List<InfoModel>>(data.data.ToString());
                foreach (InfoModel item in model)
                {
                    live_Wl.Items.Add(item);
                }
                Wl = true;
            }
            catch (Exception)
            {
            }
            finally {
                pr_Load.Visibility = Visibility.Collapsed;
            }
            
        }

        public async void GetDzj()
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                WebClientClass wc = new WebClientClass();
                // http://live.bilibili.com/area/liveList?area=/e-sports&order=online&page=1
                string results = await wc.GetResults(new Uri(" http://live.bilibili.com/area/liveList?area=e-sports&order=online&page=1"));
                InfoModel data = JsonConvert.DeserializeObject<InfoModel>(results);
                //JObject json = JObject.Parse(results);
                List<InfoModel> model = JsonConvert.DeserializeObject<List<InfoModel>>(data.data.ToString());
                foreach (InfoModel item in model)
                {
                    live_Dzj.Items.Add(item);
                }
                dzjz = true;
            }
            catch (Exception)
            {
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }

        public async void GetDy()
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                WebClientClass wc = new WebClientClass();
                // http://live.bilibili.com/area/liveList?area=/movie&order=online&page=1
                string results = await wc.GetResults(new Uri("http://live.bilibili.com/area/liveList?area=movie&order=online&page=1"));
                InfoModel data = JsonConvert.DeserializeObject<InfoModel>(results);
                //JObject json = JObject.Parse(results);
                List<InfoModel> model = JsonConvert.DeserializeObject<List<InfoModel>>(data.data.ToString());
                foreach (InfoModel item in model)
                {
                    live_Dy.Items.Add(item);
                }
                Dy = true;
            }
            catch (Exception)
            {
            }
            finally {
                pr_Load.Visibility = Visibility.Collapsed;
            }

        }
        bool Sj = false;
        public async void GetSj()
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                WebClientClass wc = new WebClientClass();
                // http://live.bilibili.com/area/liveList?area=/movie&order=online&page=1
                string results = await wc.GetResults(new Uri("http://live.bilibili.com/area/liveList?area=mobile&order=online&page=1"));
                InfoModel data = JsonConvert.DeserializeObject<InfoModel>(results);
                //JObject json = JObject.Parse(results);
                List<InfoModel> model = JsonConvert.DeserializeObject<List<InfoModel>>(data.data.ToString());
                foreach (InfoModel item in model)
                {
                    live_Sj.Items.Add(item);
                }
                Sj = true;
            }
            catch (Exception)
            {
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }

        }
        bool Sy = false;
        public async void GetSy()
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                WebClientClass wc = new WebClientClass();
                // http://live.bilibili.com/area/liveList?area=/movie&order=online&page=1
                string results = await wc.GetResults(new Uri("http://live.bilibili.com/area/liveList?area=mobile-game&order=online&page=1"));
                InfoModel data = JsonConvert.DeserializeObject<InfoModel>(results);
                //JObject json = JObject.Parse(results);
                List<InfoModel> model = JsonConvert.DeserializeObject<List<InfoModel>>(data.data.ToString());
                foreach (InfoModel item in model)
                {
                    live_Sy.Items.Add(item);
                }
                Sy = true;
            }
            catch (Exception)
            {
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
            }

        }
        private void live_HOT_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(LiveInfoPage), ((InfoModel)e.ClickedItem).room_id);
            // this.Frame.Navigate(typeof(LivePlayerPage), ((InfoModel)e.ClickedItem).room_id);
            //livePlayVideo(((InfoModel)e.ClickedItem).room_id);
        }

        bool CanLoad = true;
        private void Sc_Live_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (Sc_Live.VerticalOffset == Sc_Live.ScrollableHeight)
            {
                if (CanLoad)
                {
                    GetZBInfo();
                }
            }
        }

        private void Sc_Yz_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

        }

        private async void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txt_hea_0.FontWeight = FontWeights.Normal;
            txt_hea_1.FontWeight = FontWeights.Normal;
            txt_hea_2.FontWeight = FontWeights.Normal;
            txt_hea_3.FontWeight = FontWeights.Normal;
            txt_hea_4.FontWeight = FontWeights.Normal;
            txt_hea_5.FontWeight = FontWeights.Normal;
            txt_hea_6.FontWeight = FontWeights.Normal;
            txt_hea_7.FontWeight = FontWeights.Normal;
            txt_hea_8.FontWeight = FontWeights.Normal;
            switch (pivot.SelectedIndex)
            {
                case 0:
                    txt_hea_0.FontWeight = FontWeights.Bold;
                    if (!Hot)
                    {
                        await Task.Delay(200);
                        GetZBInfo();
                    }
                    break;
                case 1:
                    txt_hea_1.FontWeight = FontWeights.Bold;
                    if (!Yz)
                    {
                        GetYz();
                    }
                    break;
                case 2:
                    txt_hea_2.FontWeight = FontWeights.Bold;
                    if (!Sh)
                    {
                        GetSh();
                    }
                    break;
                case 3:
                    txt_hea_3.FontWeight = FontWeights.Bold;
                    if (!Dj)
                    {
                        GetDj();
                    }
                    break;
                case 4:
                    txt_hea_4.FontWeight = FontWeights.Bold;
                    if (!Wl)
                    {
                        GetWl();
                    }
                    break;
                case 5:
                    txt_hea_5.FontWeight = FontWeights.Bold;
                    if (!dzjz)
                    {
                        GetDzj();
                    }
                    break;
                case 6:
                    txt_hea_6.FontWeight = FontWeights.Bold;
                    if (!Dy)
                    {
                        GetDy();
                    }
                    break;

                case 7:
                    txt_hea_7.FontWeight = FontWeights.Bold;
                    if (!Sj)
                    {
                        GetSj();
                    }
                    break;
                case 8:
                    txt_hea_8.FontWeight = FontWeights.Bold;
                    if (!Sy)
                    {
                        GetSy();
                    }
                    break;
                default:
                    break;
            }
        }

        private void btn_Go_Click(object sender, RoutedEventArgs e)
        {

            //this.Frame.Navigate(typeof(LivePlayerPage), txt_RoomID);
        }

        private void Pull_RefreshInvoked(DependencyObject sender, object args)
        {
            switch (pivot.SelectedIndex)
            {
                case 0:
                    PageNum = 1;
                    live_HOT.Items.Clear();
                    GetZBInfo();
                    break;
                case 1:
                    live_Yz.Items.Clear();
                    GetYz();
                    break;
                case 2:
                    live_Sh.Items.Clear();
                    GetSh();
                    break;
                case 3:
                    live_Dj.Items.Clear();
                    GetDj();
                    break;
                case 4:
                    live_Wl.Items.Clear();
                    GetWl();
                    break;
                case 5:
                    live_Dzj.Items.Clear();
                    GetDzj();
                    break;
                case 6:
                    live_Dy.Items.Clear();
                    GetDy();
                    break;
                case 7:
                    live_Sj.Items.Clear();
                    GetSj();
                    break;
                case 8:
                    live_Sy.Items.Clear();
                    GetSy();
                    break;
                default:
                    break;
            }
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            switch (pivot.SelectedIndex)
            {
                case 0:
                    PageNum = 1;
                    live_HOT.Items.Clear();
                    GetZBInfo();
                    break;
                case 1:
                    live_Yz.Items.Clear();
                    GetYz();
                    break;
                case 2:
                    live_Sh.Items.Clear();
                    GetSh();
                    break;
                case 3:
                    live_Dj.Items.Clear();
                    GetDj();
                    break;
                case 4:
                    live_Wl.Items.Clear();
                    GetWl();
                    break;
                case 5:
                    live_Dzj.Items.Clear();
                    GetDzj();
                    break;
                case 6:
                    live_Dy.Items.Clear();
                    GetDy();
                    break;
                case 7:
                    live_Sj.Items.Clear();
                    GetSj();
                    break;
                case 8:
                    live_Sy.Items.Clear();
                    GetSy();
                    break;
                default:
                    break;
            }
        }

        private void live_Dzj_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(LiveInfoPage), ((InfoModel)e.ClickedItem).roomid);
        }

        private void hot_LoadMore_Click(object sender, RoutedEventArgs e)
        {
            if (CanLoad)
            {
                GetZBInfo();
            }
        }
    }
}
