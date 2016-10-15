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
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;
using Windows.UI.Core;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace bilibili2.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class RankPage : Page
    {
        public delegate void GoBackHandler();
        public event GoBackHandler BackEvent;
        public RankPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Required;

        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            bg.Color = ((SolidColorBrush)this.Frame.Tag).Color;
            
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
        public async Task GetQZRank()
        {
            try
            {
                wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://www.bilibili.com/index/rank/all-03.json"));
                InfoModel model = JsonConvert.DeserializeObject<InfoModel>(results);
                InfoModel model1 = JsonConvert.DeserializeObject<InfoModel>(model.rank.ToString());
                List<InfoModel> ls = JsonConvert.DeserializeObject<List<InfoModel>>(model1.list.ToString());
                List<InfoModel> ReList = new List<InfoModel>();
                for (int i = 0; i < 30; i++)
                {
                    if (i<3)
                    {
                        ls[i].forColor = bg;
                    }
                    ls[i].num = i + 1;
                    ReList.Add(ls[i]);
                }
                QQ_Rank_QZ.ItemsSource = ReList;
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message);
                await md.ShowAsync();
            }
        }
        public async Task GetYCRank()
        {
            try
            {
                wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://www.bilibili.com/index/rank/origin-03.json"));
                InfoModel model = JsonConvert.DeserializeObject<InfoModel>(results);
                InfoModel model1 = JsonConvert.DeserializeObject<InfoModel>(model.rank.ToString());
                List<InfoModel> ls = JsonConvert.DeserializeObject<List<InfoModel>>(model1.list.ToString());
                List<InfoModel> ReList = new List<InfoModel>();
                for (int i = 0; i < 30; i++)
                {
                    if (i < 3)
                    {
                        ls[i].forColor = bg;
                    }
                    ls[i].num = i + 1;
                    ReList.Add(ls[i]);
                }
                QQ_Rank_YC.ItemsSource = ReList;

            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message);
                await md.ShowAsync();
            }
        }
        public async Task GetFJRank()
        {
            try
            {
                wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://www.bilibili.com/index/rank/all-3-33.json"));
                InfoModel model = JsonConvert.DeserializeObject<InfoModel>(results);
                InfoModel model1 = JsonConvert.DeserializeObject<InfoModel>(model.rank.ToString());
                List<InfoModel> ls = JsonConvert.DeserializeObject<List<InfoModel>>(model1.list.ToString());
                List<InfoModel> ReList = new List<InfoModel>();
                for (int i = 0; i < 15; i++)
                {
                    if (i < 3)
                    {
                        ls[i].forColor = bg;
                    }
                    ls[i].num = i + 1;
                    ReList.Add(ls[i]);
                }
                QQ_Rank_FJ.ItemsSource = ReList;

            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message);
                await md.ShowAsync();
            }
        }
        public async Task GetDHRank()
        {
            try
            {
                wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://www.bilibili.com/index/rank/all-03-1.json"));
                InfoModel model = JsonConvert.DeserializeObject<InfoModel>(results);
                InfoModel model1 = JsonConvert.DeserializeObject<InfoModel>(model.rank.ToString());
                List<InfoModel> ls = JsonConvert.DeserializeObject<List<InfoModel>>(model1.list.ToString());
                List<InfoModel> ReList = new List<InfoModel>();
                for (int i = 0; i < 30; i++)
                {
                    if (i < 3)
                    {
                        ls[i].forColor = bg;
                    }
                    ls[i].num = i + 1;
                    ReList.Add(ls[i]);
                }
                QQ_Rank_DH.ItemsSource = ReList;
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message);
                await md.ShowAsync();
            }

        }

        public async Task GetYYRank()
        {
            try
            {
                wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://www.bilibili.com/index/rank/all-03-3.json"));
                InfoModel model = JsonConvert.DeserializeObject<InfoModel>(results);
                InfoModel model1 = JsonConvert.DeserializeObject<InfoModel>(model.rank.ToString());
                List<InfoModel> ls = JsonConvert.DeserializeObject<List<InfoModel>>(model1.list.ToString());
                List<InfoModel> ReList = new List<InfoModel>();
                for (int i = 0; i < 30; i++)
                {
                    if (i < 3)
                    {
                        ls[i].forColor = bg;
                    }
                    ls[i].num = i + 1;
                    ReList.Add(ls[i]);
                }
                QQ_Rank_YY.ItemsSource = ReList;
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message);
                await md.ShowAsync();
            }

        }
        public async Task GetWDRank()
        {
            try
            {
                wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://www.bilibili.com/index/rank/all-03-129.json"));
                InfoModel model = JsonConvert.DeserializeObject<InfoModel>(results);
                InfoModel model1 = JsonConvert.DeserializeObject<InfoModel>(model.rank.ToString());
                List<InfoModel> ls = JsonConvert.DeserializeObject<List<InfoModel>>(model1.list.ToString());
                List<InfoModel> ReList = new List<InfoModel>();
                for (int i = 0; i < 30; i++)
                {
                    if (i < 3)
                    {
                        ls[i].forColor = bg;
                    }
                    ls[i].num = i + 1;
                    ReList.Add(ls[i]);
                }
               
                    QQ_Rank_WD.ItemsSource = ReList;

            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message);
                await md.ShowAsync();
            }

        }

        public async Task GetYXRank()
        {
            try
            {
                wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://www.bilibili.com/index/rank/all-03-4.json"));
                InfoModel model = JsonConvert.DeserializeObject<InfoModel>(results);
                InfoModel model1 = JsonConvert.DeserializeObject<InfoModel>(model.rank.ToString());
                List<InfoModel> ls = JsonConvert.DeserializeObject<List<InfoModel>>(model1.list.ToString());
                List<InfoModel> ReList = new List<InfoModel>();
                for (int i = 0; i < 30; i++)
                {
                    if (i < 3)
                    {
                        ls[i].forColor = bg;
                    }
                    ls[i].num = i + 1;
                    ReList.Add(ls[i]);
                }
                QQ_Rank_YX.ItemsSource = ReList;
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message);
                await md.ShowAsync();
            }

        }
        public async Task GetKJRank()
        {
            try
            {
                wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://www.bilibili.com/index/rank/all-03-36.json"));
                InfoModel model = JsonConvert.DeserializeObject<InfoModel>(results);
                InfoModel model1 = JsonConvert.DeserializeObject<InfoModel>(model.rank.ToString());
                List<InfoModel> ls = JsonConvert.DeserializeObject<List<InfoModel>>(model1.list.ToString());
                List<InfoModel> ReList = new List<InfoModel>();
                for (int i = 0; i < 30; i++)
                {
                    if (i < 3)
                    {
                        ls[i].forColor = bg;
                    }
                    ls[i].num = i + 1;
                    ReList.Add(ls[i]);
                }
                QQ_Rank_KJ.ItemsSource = ReList;


            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message);
                await md.ShowAsync();
            }

        }

        public async Task GetYLRank()
        {
            try
            {
                wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://www.bilibili.com/index/rank/all-03-5.json"));
                InfoModel model = JsonConvert.DeserializeObject<InfoModel>(results);
                InfoModel model1 = JsonConvert.DeserializeObject<InfoModel>(model.rank.ToString());
                List<InfoModel> ls = JsonConvert.DeserializeObject<List<InfoModel>>(model1.list.ToString());
                List<InfoModel> ReList = new List<InfoModel>();
                for (int i = 0; i < 30; i++)
                {
                    if (i < 3)
                    {
                        ls[i].forColor = bg;
                    }
                    ls[i].num = i + 1;
                    ReList.Add(ls[i]);
                }
                QQ_Rank_YL.ItemsSource = ReList;
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message);
                await md.ShowAsync();
            }

        }
        public async Task GetSHRank()
        {
            try
            {
                wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://www.bilibili.com/index/rank/all-03-160.json"));
                InfoModel model = JsonConvert.DeserializeObject<InfoModel>(results);
                InfoModel model1 = JsonConvert.DeserializeObject<InfoModel>(model.rank.ToString());
                List<InfoModel> ls = JsonConvert.DeserializeObject<List<InfoModel>>(model1.list.ToString());
                List<InfoModel> ReList = new List<InfoModel>();
                for (int i = 0; i < 30; i++)
                {
                    if (i < 3)
                    {
                        ls[i].forColor = bg;
                    }
                    ls[i].num = i + 1;
                    ReList.Add(ls[i]);
                }
                QQ_Rank_SH.ItemsSource = ReList;
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message);
                await md.ShowAsync();
            }

        }
        public async Task GetGCRank()
        {
            try
            {
                wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://www.bilibili.com/index/rank/all-03-119.json"));
                InfoModel model = JsonConvert.DeserializeObject<InfoModel>(results);
                InfoModel model1 = JsonConvert.DeserializeObject<InfoModel>(model.rank.ToString());
                List<InfoModel> ls = JsonConvert.DeserializeObject<List<InfoModel>>(model1.list.ToString());
                List<InfoModel> ReList = new List<InfoModel>();
                for (int i = 0; i < 30; i++)
                {
                    if (i < 3)
                    {
                        ls[i].forColor = bg;
                    }
                    ls[i].num = i + 1;
                    ReList.Add(ls[i]);
                }
                QQ_Rank_GC.ItemsSource = ReList;
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message);
                await md.ShowAsync();
            }

        }


        public async Task GetDYRank()
        {
            try
            {
                wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://www.bilibili.com/index/rank/all-03-23.json"));
                InfoModel model = JsonConvert.DeserializeObject<InfoModel>(results);
                InfoModel model1 = JsonConvert.DeserializeObject<InfoModel>(model.rank.ToString());
                List<InfoModel> ls = JsonConvert.DeserializeObject<List<InfoModel>>(model1.list.ToString());
                List<InfoModel> ReList = new List<InfoModel>();
                for (int i = 0; i < 30; i++)
                {
                    if (i < 3)
                    {
                        ls[i].forColor = bg;
                    }
                    ls[i].num = i + 1;
                    ReList.Add(ls[i]);
                }
                QQ_Rank_DY.ItemsSource = ReList;
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message);
                await md.ShowAsync();
            }

        }
        public async Task GetDSJRank()
        {
            try
            {
                wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://www.bilibili.com/index/rank/all-03-11.json"));
                InfoModel model = JsonConvert.DeserializeObject<InfoModel>(results);
                InfoModel model1 = JsonConvert.DeserializeObject<InfoModel>(model.rank.ToString());
                List<InfoModel> ls = JsonConvert.DeserializeObject<List<InfoModel>>(model1.list.ToString());
                List<InfoModel> ReList = new List<InfoModel>();
                for (int i = 0; i < 30; i++)
                {
                    if (i < 3)
                    {
                        ls[i].forColor = bg;
                    }
                    ls[i].num = i + 1;
                    ReList.Add(ls[i]);
                }
                QQ_Rank_DSJ.ItemsSource = ReList;
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message);
                await md.ShowAsync();
            }

        }
        public async Task GetSSRank()
        {
            try
            {
                wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://www.bilibili.com/index/rank/all-03-155.json"));
                InfoModel model = JsonConvert.DeserializeObject<InfoModel>(results);
                InfoModel model1 = JsonConvert.DeserializeObject<InfoModel>(model.rank.ToString());
                List<InfoModel> ls = JsonConvert.DeserializeObject<List<InfoModel>>(model1.list.ToString());
                List<InfoModel> ReList = new List<InfoModel>();
                for (int i = 0; i < 30; i++)
                {
                    if (i < 3)
                    {
                        ls[i].forColor = bg;
                    }
                    ls[i].num = i + 1;
                    ReList.Add(ls[i]);
                }
                QQ_Rank_SS.ItemsSource = ReList;
            }
            catch (Exception ex)
            {
                MessageDialog md = new MessageDialog(ex.Message);
                await md.ShowAsync();
            }

        }


        private void QQ_Rank_YC_ItemClick(object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(VideoInfoPage), ((InfoModel)e.ClickedItem).aid);
        }
        bool QZLoad = false;
        bool YCLoad = false;
        bool FJLoad = false;
        bool DHLoad = false;
        bool YYLoad = false;
        bool WDLoad = false;
        bool YXLoad = false;
        bool KJLoad = false;
        bool YLLoad = false;
        bool GCLoad = false;
        bool DYLoad = false;
        bool DSJLoad = false;
        bool SSLoad = false;
        bool SHLoad = false;
        private async void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (pivot.SelectedIndex)
            {
                case 0:
                    if (!YCLoad)
                    {
                        await Task.Delay(200);
                        pr_loading.Visibility = Visibility.Visible;
                        await GetYCRank();
                        YCLoad = true;
                        pr_loading.Visibility = Visibility.Collapsed;
                    }
                    break;
                case 1:
                    if (!QZLoad)
                    {
                        pr_loading.Visibility = Visibility.Visible;
                        await GetQZRank();
                        QZLoad = true;
                        pr_loading.Visibility = Visibility.Collapsed;
                    }
                    break;
                case 2:
                    if (!FJLoad)
                    {
                        pr_loading.Visibility = Visibility.Visible;
                        await GetFJRank();
                        FJLoad = true;
                        pr_loading.Visibility = Visibility.Collapsed;
                    }
                    break;
                case 3:
                    if (!DHLoad)
                    {
                        pr_loading.Visibility = Visibility.Visible;
                        await GetDHRank();
                        DHLoad = true;
                        pr_loading.Visibility = Visibility.Collapsed;
                    }
                    break;
                case 4:
                    if (!YYLoad)
                    {
                        pr_loading.Visibility = Visibility.Visible;
                        await GetYYRank();
                        YYLoad = true;
                        pr_loading.Visibility = Visibility.Collapsed;
                    }
                    break;
                case 5:
                    if (!WDLoad)
                    {
                        pr_loading.Visibility = Visibility.Visible;
                        await GetWDRank();
                        WDLoad = true;
                        pr_loading.Visibility = Visibility.Collapsed;
                    }
                    break;
                case 6:
                    if (!YXLoad)
                    {
                        pr_loading.Visibility = Visibility.Visible;
                        await GetYXRank();
                        YXLoad = true;
                        pr_loading.Visibility = Visibility.Collapsed;
                    }
                    break;
                case 7:
                    if (!KJLoad)
                    {
                        pr_loading.Visibility = Visibility.Visible;
                        await GetKJRank();
                        KJLoad = true;
                        pr_loading.Visibility = Visibility.Collapsed;
                    }
                    break;
                case 8:
                    if (!SHLoad)
                    {
                        pr_loading.Visibility = Visibility.Visible;
                        await GetSHRank();
                        SHLoad = true;
                        pr_loading.Visibility = Visibility.Collapsed;
                    }
                    break;
                case 9:
                    if (!YLLoad)
                    {
                        pr_loading.Visibility = Visibility.Visible;
                        await GetYLRank();
                        YLLoad = true;
                        pr_loading.Visibility = Visibility.Collapsed;
                    }
                    break;
                case 10:
                    if (!GCLoad)
                    {
                        pr_loading.Visibility = Visibility.Visible;
                        await GetGCRank();
                        GCLoad = true;
                        pr_loading.Visibility = Visibility.Collapsed;
                    }
                    break;
                case 11:
                    if (!DYLoad)
                    {
                        pr_loading.Visibility = Visibility.Visible;
                        await GetDYRank();
                        DYLoad = true;
                        pr_loading.Visibility = Visibility.Collapsed;
                    }
                    break;
                case 12:
                    if (!DSJLoad)
                    {
                        pr_loading.Visibility = Visibility.Visible;
                        await GetDSJRank();
                        DSJLoad = true;
                        pr_loading.Visibility = Visibility.Collapsed;
                    }
                    break;
                case 13:
                    if (!SSLoad)
                    {
                        pr_loading.Visibility = Visibility.Visible;
                        await GetSSRank();
                        SSLoad = true;
                        pr_loading.Visibility = Visibility.Collapsed;
                    }
                    break;
                default:
                    break;
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            int d = Convert.ToInt32(this.ActualWidth / 400);
            if (d > 3)
            {
                d = 3;
            }
            bor_Width.Width = this.ActualWidth / d - 22;
        }
    }
}
