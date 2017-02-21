using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Text;
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
    public sealed partial class SearchPage : Page
    {
        public SearchPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        private int pageNum = 1;
        private string keyword = "";
        private int pageNum_Up = 1;
        private int pageNum_Ban = 1;
        //private string keyword = "";
        private int pageNum_Sp = 1;
        private int pageNum_Movie = 1;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                if (e.Parameter!=null)
                {
                    keyword = Uri.EscapeDataString(((object[])e.Parameter)[0].ToString());
                    autoSug_Box.Text = ((object[])e.Parameter)[0].ToString();
                }
                btn_More_Video.Visibility = Visibility.Collapsed;
                btn_More_Ban.Visibility = Visibility.Collapsed;
                btn_More_Moive.Visibility = Visibility.Collapsed;
                btn_More_SP.Visibility = Visibility.Collapsed;
                btn_More_UP.Visibility = Visibility.Collapsed;
                // text_Title.Text = "搜索 " + (string)e.Parameter;
                pr_Loading.Visibility = Visibility.Collapsed;
                pageNum = 1;
                pageNum_Up = 1;
                pageNum_Ban = 1;
                pageNum_Sp = 1;
                pageNum_Movie = 1;
                InfoLoading = false;
                BanLoading = false;
                UPLoading = false;
                SeasonLoading = false;
                MovieLoading = false;
                Seach_listview_Video.Items.Clear();
                Seach_listview_Ban.Items.Clear();
                Seach_listview_Sp.Items.Clear();
                Seach_listview_Up.Items.Clear();
                Seach_listview_Movie.Items.Clear();
                pivot.SelectedIndex = 0;
                if (!InfoLoading && Seach_listview_Video.Items.Count == 0)
                {
                    if (keyword.Length!=0)
                    {
                        SeachVideo();
                    }
                   
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                keyword = "";
                cb_OrderBy.SelectedIndex = 0;
                cb_part.SelectedIndex = 0;
            }

        }

        //搜索视频
        public async void SeachVideo()
        {
            try
            {
                btn_More_Video.Visibility = Visibility.Visible;
                InfoLoading = true;
                object a = (cb_part.SelectedItem as ComboBoxItem).Tag;
                pr_Loading.Visibility = Visibility.Visible;
                string uri = "";
                if (a == null)
                {
                    uri = "http://api.bilibili.com/search?_device=wp&appkey=422fd9d7289a1dd9&build=411005&keyword=" + keyword + "&main_ver=v3&page=" + pageNum + "&pagesize=20&platform=android&search_type=video&source_type=0" + ((cb_OrderBy.SelectedItem as ComboBoxItem).Tag == null ? "" : "&order=" + (cb_OrderBy.SelectedItem as ComboBoxItem).Tag);
                }
                else
                {
                    uri = "http://api.bilibili.com/search?_device=wp&appkey=422fd9d7289a1dd9&build=411005&keyword=" + keyword + "&main_ver=v3&page=" + pageNum + "&pagesize=20&platform=android&search_type=video&source_type=0&tids=" + a.ToString() + ((cb_OrderBy.SelectedItem as ComboBoxItem).Tag == null ? "" : "&order=" + (cb_OrderBy.SelectedItem as ComboBoxItem).Tag);
                }
                string sign = ApiHelper.GetSign(uri);
                uri += "&sign=" + sign;
                string results = await WebClientClass.GetResults(new Uri(uri));
                SVideoModel model = JsonConvert.DeserializeObject<SVideoModel>(results);
                if (model.code == 0)
                {
                    List<SVideoModel> ls = JsonConvert.DeserializeObject<List<SVideoModel>>(model.result.ToString());
                    foreach (SVideoModel item in ls)
                    {
                        Seach_listview_Video.Items.Add(item);
                    }
                    if (ls.Count == 0)
                    {
                        messShow.Show("没有内容了...", 2000);
                        btn_More_Video.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        btn_More_Video.Visibility = Visibility.Visible;
                    }
                    pageNum++;

                }
                if (model.code == -3)
                {
                    await new MessageDialog("API Sign注册失败！请联系作者！").ShowAsync();
                }

            }
            catch (Exception)
            {
                messShow.Show("搜索视频失败", 2000);
                //grid_GG.Visibility = Visibility.Visible;
                //txt_GG.Text = "搜索视频失败\r\n" + ex.Message;
                //await Task.Delay(2000);
                //grid_GG.Visibility = Visibility.Collapsed;
            }
            finally
            {
                pr_Loading.Visibility = Visibility.Collapsed;
                InfoLoading = false;
            }

        }
        //搜索番剧
        public async void SeachBangumi()
        {
            try
            {
                btn_More_Ban.Visibility = Visibility.Visible;
                BanLoading = true;
                object a = (cb_part.SelectedItem as ComboBoxItem).Tag;
                pr_Loading.Visibility = Visibility.Visible;
              
                string uri = "http://api.bilibili.com/search?_device=wp&appkey=422fd9d7289a1dd9&build=411005&keyword=" + keyword + "&main_ver=v3&page=" + pageNum_Ban + "&pagesize=20&platform=android&search_type=bangumi&source_type=0";
                string sign = ApiHelper.GetSign(uri);
                uri += "&sign=" + sign;
                string results = await WebClientClass.GetResults(new Uri(uri));
                SBanModel model = JsonConvert.DeserializeObject<SBanModel>(results);
                if (model.code == 0)
                {
                    List<SBanModel> ls = JsonConvert.DeserializeObject<List<SBanModel>>(model.result.ToString());
                    foreach (SBanModel item in ls)
                    {
                        Seach_listview_Ban.Items.Add(item);
                    }
                    if (ls.Count == 0)
                    {
                        messShow.Show("没有内容了...", 2000);
                        btn_More_Ban.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        btn_More_Ban.Visibility = Visibility.Visible;

                    }
                    pageNum_Ban++;
                }
                if (model.code == -3)
                {
                    await new MessageDialog("API Sign注册失败！请联系作者！").ShowAsync();
                }
            }
            catch (Exception)
            {
                //grid_GG.Visibility = Visibility.Visible;
                //txt_GG.Text = "搜索番剧失败\r\n" + ex.Message;
                //await Task.Delay(2000);
                //grid_GG.Visibility = Visibility.Collapsed;
                messShow.Show("搜索番剧失败", 2000);
            }
            finally
            {
                pr_Loading.Visibility = Visibility.Collapsed;
                BanLoading = false;
            }

        }
        //搜索UP
        public async void SeachUP()
        {
            try
            {
                btn_More_UP.Visibility = Visibility.Visible;
                UPLoading = true;
                object a = (cb_part.SelectedItem as ComboBoxItem).Tag;
                pr_Loading.Visibility = Visibility.Visible;
                string uri = "http://api.bilibili.com/search?_device=wp&appkey=422fd9d7289a1dd9&build=411005&keyword=" + keyword + "&main_ver=v3&page=" + pageNum_Up + "&pagesize=20&platform=android&search_type=upuser&source_type=0";
                string sign = ApiHelper.GetSign(uri);
                uri += "&sign=" + sign;
                string results = await WebClientClass.GetResults(new Uri(uri));
                SUpModel model = JsonConvert.DeserializeObject<SUpModel>(results);
                if (model.code == 0)
                {
                    List<SUpModel> ls = JsonConvert.DeserializeObject<List<SUpModel>>(model.result.ToString());
                    foreach (SUpModel item in ls)
                    {
                        Seach_listview_Up.Items.Add(item);
                    }
                    if (ls.Count == 0)
                    {
                        btn_More_UP.Visibility = Visibility.Collapsed;
                        messShow.Show("没有内容了...", 2000);
                    }
                    else
                    {
                        btn_More_UP.Visibility = Visibility.Visible;
                    }
                    pageNum_Up++;
                }
                if (model.code == -3)
                {
                    await new MessageDialog("API Sign注册失败！请联系作者！").ShowAsync();
                }
            }
            catch (Exception)
            {
                messShow.Show("搜索UP主失败", 2000);
                //grid_GG.Visibility = Visibility.Visible;
                //txt_GG.Text = "搜索UP主失败\r\n" + ex.Message;
                //await Task.Delay(2000);
                //grid_GG.Visibility = Visibility.Collapsed;
            }
            finally
            {
                pr_Loading.Visibility = Visibility.Collapsed;
                UPLoading = false;
            }

        }
        //搜索专题
        public async void SeachSp()
        {
            try
            {
                btn_More_SP.Visibility = Visibility.Visible;
                SeasonLoading = true;
                pr_Loading.Visibility = Visibility.Visible;
                string uri = "http://api.bilibili.com/search?_device=wp&appkey=422fd9d7289a1dd9&build=411005&keyword=" + keyword + "&main_ver=v3&page=" + pageNum_Sp + "&pagesize=20&platform=android&search_type=special&source_type=0";//
                string sign = ApiHelper.GetSign(uri);
                uri += "&sign=" + sign;
                string results = await WebClientClass.GetResults(new Uri(uri));
                SSpModel model = JsonConvert.DeserializeObject<SSpModel>(results);
                if (model.code == 0)
                {
                    List<SSpModel> ls = JsonConvert.DeserializeObject<List<SSpModel>>(model.result.ToString());
                    foreach (SSpModel item in ls)
                    {
                        Seach_listview_Sp.Items.Add(item);
                    }
                    if (ls.Count == 0)
                    {
                        btn_More_SP.Visibility = Visibility.Collapsed;
                        messShow.Show("没有内容了...", 2000);
                    }
                    else
                    {
                        btn_More_SP.Visibility = Visibility.Visible;
                    }
                    pageNum_Sp++;
                }
                if (model.code == -3)
                {
                    await new MessageDialog("API Sign注册失败！请联系作者！").ShowAsync();
                }
            }
            catch (Exception)
            {
                //grid_GG.Visibility = Visibility.Visible;
                //txt_GG.Text = "搜索专题失败\r\n" + ex.Message;
                //await Task.Delay(2000);
                //grid_GG.Visibility = Visibility.Collapsed;
                messShow.Show("搜索专题失败", 2000);
            }
            finally
            {
                pr_Loading.Visibility = Visibility.Collapsed;
                SeasonLoading = false;
            }

        }
        //搜索电影
        public async void SeachMovie()
        {
            try
            {
                btn_More_Moive.Visibility = Visibility.Visible;
                MovieLoading = true;
                object a = (cb_part.SelectedItem as ComboBoxItem).Tag;
                pr_Loading.Visibility = Visibility.Visible;
                string uri = "http://app.bilibili.com/x/v2/search/type?keyword=" + keyword + "&pn=" + pageNum_Movie + "&ps=20&type=3&access_key=" + ApiHelper.access_key + "&appkey=" + ApiHelper._appKey_Android2 + "&build=433000&mobi_app=android&platform=android";
                //string sign = ApiHelper.GetSign(uri);
               // uri += "&sign=" + sign;
                string results = await WebClientClass.GetResults(new Uri(uri));
                SMovieModel model = JsonConvert.DeserializeObject<SMovieModel>(results);
                if (model.code == 0)
                {
                    //List<SMovieModel> ls = JsonConvert.DeserializeObject<List<SMovieModel>>(model.result.ToString());
                    foreach (SMovieModel item in model.data.items)
                    {
                        Seach_listview_Movie.Items.Add(item);
                    }
                    if (model.data.items.Count == 0)
                    {
                        messShow.Show("没有内容了...", 2000);
                        btn_More_Moive.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        btn_More_Moive.Visibility = Visibility.Visible;

                    }
                    pageNum_Movie++;
                }
                if (model.code == -3)
                {
                    await new MessageDialog("API Sign注册失败！请联系作者！").ShowAsync();
                }
            }
            catch (Exception)
            {
                //grid_GG.Visibility = Visibility.Visible;
                //txt_GG.Text = "搜索番剧失败\r\n" + ex.Message;
                //await Task.Delay(2000);
                //grid_GG.Visibility = Visibility.Collapsed;
                messShow.Show("搜索影视失败", 2000);
            }
            finally
            {
                pr_Loading.Visibility = Visibility.Collapsed;
                MovieLoading = false;
            }

        }


        private void User_load_more_Click(object sender, RoutedEventArgs e)
        {
            SeachVideo();
        }

        private void Seach_listview_Video_ItemClick(object sender, ItemClickEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(VideoViewPage),new object[] { ((SVideoModel)e.ClickedItem).aid });
          
        }

        private void Seach_listview_Up_ItemClick(object sender, ItemClickEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(UserInfoPage), ((SUpModel)e.ClickedItem).mid);
            //this.Frame.Navigate(typeof(UserInfoPage), ((SUpModel)e.ClickedItem).mid);
        }

        private void Up_load_more_Click(object sender, RoutedEventArgs e)
        {
            SeachUP();
        }

        private void Ban_load_more_Click(object sender, RoutedEventArgs e)
        {
            SeachBangumi();
        }

        private void Seach_listview_Ban_ItemClick(object sender, ItemClickEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(BanInfoPage), new object[] { ((SBanModel)e.ClickedItem).season_id });
            //this.Frame.Navigate(typeof(BanInfoPage), ((SBanModel)e.ClickedItem).season_id);
            //this.Frame.Navigate(typeof(BanSeasonNewPage), ((SeachBanModel)e.ClickedItem).season_id);
        }

        private void Seach_listview_Sp_ItemClick(object sender, ItemClickEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(SubjectPage),new object[] { ((SSpModel)e.ClickedItem).spid });
           // this.Frame.Navigate(typeof(SPPage), ((SSpModel)e.ClickedItem).spid);
        }

        private void Sp_load_more_Click(object sender, RoutedEventArgs e)
        {
            SeachSp();
        }

        bool InfoLoading = false;
        private void sv_SP_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sv_SP.VerticalOffset == sv_SP.ScrollableHeight)
            {
                if (!InfoLoading)
                {
                    //User_load_more.IsEnabled = false;
                    //User_load_more.Content = "正在加载";
                    SeachVideo();
                }
            }
        }

        private void btn_Filter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void cb_part_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (keyword != "")
            {
                pageNum = 1;
                Seach_listview_Video.Items.Clear();
                SeachVideo();
            }

        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
         
            switch (pivot.SelectedIndex)
            {
                case 0:
                    //btn_Filter.IsEnabled = true;
                  
                    btn_Filter.Visibility = Visibility.Visible;
                    break;
                case 1:
                 
                    btn_Filter.Visibility = Visibility.Collapsed;
                    if (!UPLoading && Seach_listview_Up.Items.Count == 0)
                    {
                        SeachUP();
                    }
                    break;
                case 2:
                 
                    btn_Filter.Visibility = Visibility.Collapsed;
                    if (!BanLoading && Seach_listview_Ban.Items.Count == 0)
                    {
                        SeachBangumi();
                    }
                    break;
                case 3:

                    btn_Filter.Visibility = Visibility.Collapsed;

                    if (!SeasonLoading && Seach_listview_Sp.Items.Count == 0)
                    {
                        SeachSp();
                    }
                    break;
                case 4:

                    btn_Filter.Visibility = Visibility.Collapsed;

                    if (!MovieLoading && Seach_listview_Movie.Items.Count == 0)
                    {
                        SeachMovie();
                    }
                    break;
                default:
                    break;
            }
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }

        bool UPLoading = false;
        private void sv_UP_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sv_UP.VerticalOffset == sv_UP.ScrollableHeight)
            {
                if (!UPLoading)
                {
                    //Up_load_more.IsEnabled = false;
                    //Up_load_more.Content = "正在加载";
                    SeachUP();
                }
            }
        }
        bool BanLoading = false;
        private void sv_Ban_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sv_Ban.VerticalOffset == sv_Ban.ScrollableHeight)
            {
                if (!BanLoading)
                {
                    //Ban_load_more.IsEnabled = false;
                    //Ban_load_more.Content = "正在加载";
                    SeachBangumi();
                }
            }

        }
        bool SeasonLoading = false;
        private void sv_Season_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sv_Season.VerticalOffset == sv_Season.ScrollableHeight)
            {
                if (!SeasonLoading)
                {
                    //Sp_load_more.IsEnabled = false;
                    //Sp_load_more.Content = "正在加载";
                    SeachSp();
                }
            }
        }

        private void autoSug_Box_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            keyword = Uri.EscapeDataString(autoSug_Box.Text);
            // text_Title.Text = "搜索 " + (string)e.Parameter;
            //autoSug_Box.Text = (string)e.Parameter;
            pageNum = 1;
            pageNum_Up = 1;
            pageNum_Ban = 1;
            pageNum_Sp = 1;
            pageNum_Movie = 1;
            InfoLoading = false;
            BanLoading = false;
            UPLoading = false;
            SeasonLoading = false;
            MovieLoading = false;
            Seach_listview_Video.Items.Clear();
            Seach_listview_Ban.Items.Clear();
            Seach_listview_Sp.Items.Clear();
            Seach_listview_Up.Items.Clear();
            Seach_listview_Movie.Items.Clear();
            pivot.SelectedIndex = 0;
            if (!InfoLoading && Seach_listview_Video.Items.Count == 0)
            {
                SeachVideo();
            }

        }

        private async void autoSug_Box_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (sender.Text.Length != 0)
            {
                sender.ItemsSource = await GetSugges(sender.Text);
            }
            else
            {
                sender.ItemsSource = null;
            }
        }

        private void autoSug_Box_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            autoSug_Box.Text = args.SelectedItem as string;
        }

        public async Task<ObservableCollection<String>> GetSugges(string text)
        {
            try
            {
                string results = await WebClientClass.GetResults(new Uri("http://s.search.bilibili.com/main/suggest?suggest_type=accurate&sub_type=tag&main_ver=v1&term=" + text));
                JObject json = JObject.Parse(results);
                // json["result"]["tag"].ToString();
                List<SuggesModel> list = JsonConvert.DeserializeObject<List<SuggesModel>>(json["result"]["tag"].ToString());
                ObservableCollection<String> suggestions = new ObservableCollection<string>();
                foreach (SuggesModel item in list)
                {
                    suggestions.Add(item.value);
                }
                return suggestions;
            }
            catch (Exception)
            {
                return new ObservableCollection<string>();
            }

        }
        public class SuggesModel
        {
            public string name { get; set; }
            public string value { get; set; }
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

        private void btn_More_Video_Click(object sender, RoutedEventArgs e)
        {
            if (!InfoLoading)
            {
                SeachVideo();
            }
        }

        private void btn_More_UP_Click(object sender, RoutedEventArgs e)
        {
            if (!UPLoading)
            {
                SeachUP();
            }
        }

        private void btn_More_Ban_Click(object sender, RoutedEventArgs e)
        {
            if (!BanLoading)
            {
                SeachBangumi();
            }
        }

        private void btn_More_SP_Click(object sender, RoutedEventArgs e)
        {
            if (!SeasonLoading)
            {
                SeachSp();
            }

        }
        bool MovieLoading = false;
        private void btn_More_Moive_Click(object sender, RoutedEventArgs e)
        {
            if (!MovieLoading)
            {
                SeachMovie();
            }
        }

        private void Seach_listview_Movie_ItemClick(object sender, ItemClickEventArgs e)
        {
            MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(VideoViewPage), new object[] { ((SMovieModel)e.ClickedItem).param });
            //this.Frame.Navigate(typeof(VideoInfoPage), ((SMovieModel)e.ClickedItem).aid);
        }

        private void sv_Movie_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sv_Movie.VerticalOffset == sv_Movie.ScrollableHeight)
            {
                if (!MovieLoading)
                {
                    //Sp_load_more.IsEnabled = false;
                    //Sp_load_more.Content = "正在加载";
                    SeachMovie();
                }
            }
        }
    }

    public class SVideoModel
    {
        //第一层
        public int code { get; set; }
        public int page { get; set; }//页数
        public int numResults { get; set; }//结果数量
        public int numPages { get; set; }//结果
        public object result { get; set; }//结果
        //第二层
        public long id { get; set; }//视频AID
        public string mid { get; set; }//用户Mid
        public string author { get; set; }//作者
        public string typename { get; set; }
        public string aid { get; set; }
        public string description { get; set; }
        public string title { get; set; }//标题
        public string pic { get; set; }//封面
        public string play { get; set; }//播放
        public string video_review { get; set; }//弹幕
        public string duration { get; set; }//时长
    }
    public class SMovieModel
    {
        //第一层
        public int code { get; set; }
        public int pages { get; set; }//页数
        public int numResults { get; set; }//结果数量
        public int numPages { get; set; }//结果
        public string trackid { get; set; }
        public SMovieModel data { get; set; }//结果
        public List<SMovieModel> items { get; set; }
        //第二层
        public long id { get; set; }//视频AID
        public string mid { get; set; }//用户Mid
        public string author { get; set; }//作者
        public string type { get; set; }
        public string aid { get; set; }
        public string description { get; set; }
        public string title { get; set; }//标题
        public string cover { get; set; }//封面
        public string actors { get; set; }
        public string param { get; set; }
        public string _goto { get; set; }
        public string staff { get; set; }
        public string Staff
        {
            get
            {
                return staff.Replace("\r\n", " ").Replace("\n", " ");
            }
        }
        public string area { get; set; }
        public int status { get; set; }
        public string length { get; set; }
        public Visibility vis
        {
            get
            {
                if (status == 2)
                {
                    return Visibility.Collapsed;
                }
                else
                {
                    return Visibility.Visible;
                }
            }
        }
    }
    public class SUpModel
    {
        //第一层
        public int code { get; set; }
        public int page { get; set; }//页数
        public int numResults { get; set; }//结果数量
        public int numPages { get; set; }//结果
        public object result { get; set; }//结果
        //第二层
        public string mid { get; set; }//用户Mid
        public string uname { get; set; }//作者
        public string usign { get; set; }
        public string upic { get; set; }
    }

    public class SBanModel
    {
        //第一层
        public int code { get; set; }
        public int page { get; set; }//页数
        public int numResults { get; set; }//结果数量
        public int numPages { get; set; }//结果
        public object result { get; set; }//结果

        public string title { get; set; }
        public string season_id { get; set; }
        public string bangumi_id { get; set; }
        public string spid { get; set; }
        public string evaluate { get; set; }
        public string cover { get; set; }
        public int is_finish { get; set; }
        public string newest_ep_index { get; set; }
        public int total_count { get; set; }
    }

    public class SSpModel
    {
        //第一层
        public int code { get; set; }
        public int page { get; set; }//页数
        public int numResults { get; set; }//结果数量
        public int numPages { get; set; }
        public object result { get; set; }//结果

        //第三层
        public string id { get; set; }//id
        public string spid { get; set; }
        public string title { get; set; }
        public string description { get; set; }//标题
        public string pic { get; set; }//封面
    }
}
