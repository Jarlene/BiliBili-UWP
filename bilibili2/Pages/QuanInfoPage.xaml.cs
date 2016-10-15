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
using Newtonsoft.Json;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media.Imaging;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Markup;
using Windows.UI.Popups;
using System.ComponentModel;
using Windows.UI;
using System.Xml;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace bilibili2.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class QuanInfoPage : Page
    {
        public delegate void GoBackHandler();
        public event GoBackHandler BackEvent;
        public QuanInfoPage()
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

        QuanziListModel par;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            bg.Color = ((SolidColorBrush)this.Frame.Tag).Color;
            if (e.NavigationMode == NavigationMode.New)
            {
                par = e.Parameter as QuanziListModel;
                list_Items.Items.Clear();
                pages = 1;
                total_page = 1;
                pane_Info.DataContext = null;
                btn_Zan.Foreground = new SolidColorBrush(Colors.Gray);
                btn_LookMaster.IsChecked = false;
                btn_ReverseOrder.IsChecked = false;
                Look_landlord = 1;
                reverse = 1;
                sv.ScrollToVerticalOffset(0);
                GetInfo();
            }


        }

        int Look_landlord = 1;
        int reverse = 1;
        WebClientClass wc;
        private async void GetInfo()
        {
            try
            {
                qLoading = true;
                wc = new WebClientClass();
                string url = string.Format("http://www.im9.com/api/query.detail.post.do?access_key={0}&actionKey=appkey&appkey={1}&build=418000&community_id={2}&look_landlord={5}&mobi_app=android&page_no=1&page_size=20&platform=android&post_id={3}&reply_reply_page_size=3&reverse={6}&ts={4}&version=1.0.1", ApiHelper.access_key, ApiHelper._appKey_Android, par.community_id, par.post_id, ApiHelper.GetTimeSpen, Look_landlord, reverse);
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                string results = await wc.GetResults(new Uri(url));
                QuanInfoModel info = JsonConvert.DeserializeObject<QuanInfoModel>(results);
                QuanInfoModel info2 = JsonConvert.DeserializeObject<QuanInfoModel>(info.data.ToString());
                QuanInfoModel post_info = JsonConvert.DeserializeObject<QuanInfoModel>(info2.post_info.ToString());

                pane_Info.DataContext = post_info;
              
                AddContent(post_info.post_context, post_info.post_image_list);
              
               QuanReplyModel post_reply_list = JsonConvert.DeserializeObject<QuanReplyModel>(info2.post_reply_list.ToString());

                total_page = post_reply_list.total_page;
                List<QuanReplyModel> reply_list = JsonConvert.DeserializeObject<List<QuanReplyModel>>(post_reply_list.result.ToString());
                foreach (QuanReplyModel item in reply_list)
                {
                    if (item.reply_reply_list != null)
                    {
                        QuanReplyModel hah = JsonConvert.DeserializeObject<QuanReplyModel>(item.reply_reply_list.ToString());
                        item.reList = JsonConvert.DeserializeObject<List<QuanReplyModel>>(hah.result.ToString());
                    }

                    list_Items.Items.Add(item);
                }

                pages++;

            }
            catch (Exception)
            {
                messShow.Show("加载信息失败", 3000);
            }
            finally
            {
                qLoading = false;
                pr_Load.Visibility = Visibility.Collapsed;
            }
        }
        int pages = 1;
        int total_page = 1;
        private async void AddItems()
        {
            try
            {
                pr_Load.Visibility = Visibility.Visible;
                qLoading = true;
                wc = new WebClientClass();
                string url = string.Format("http://www.im9.com/api/query.detail.post.do?access_key={0}&actionKey=appkey&appkey={1}&build=418000&community_id={2}&look_landlord={6}&mobi_app=android&page_no={5}&page_size=20&platform=android&post_id={3}&reply_reply_page_size=3&reverse={7}&ts={4}&version=1.0.1", ApiHelper.access_key, ApiHelper._appKey_Android, par.community_id, par.post_id, ApiHelper.GetTimeSpen, pages, Look_landlord, reverse);
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                string results = await wc.GetResults(new Uri(url));
                QuanInfoModel info = JsonConvert.DeserializeObject<QuanInfoModel>(results);
                QuanInfoModel info2 = JsonConvert.DeserializeObject<QuanInfoModel>(info.data.ToString());

                QuanReplyModel post_reply_list = JsonConvert.DeserializeObject<QuanReplyModel>(info2.post_reply_list.ToString());

                List<QuanReplyModel> reply_list = JsonConvert.DeserializeObject<List<QuanReplyModel>>(post_reply_list.result.ToString());
                foreach (QuanReplyModel item in reply_list)
                {
                    if (item.reply_reply_list != null)
                    {
                        QuanReplyModel hah = JsonConvert.DeserializeObject<QuanReplyModel>(item.reply_reply_list.ToString());
                        item.reList = JsonConvert.DeserializeObject<List<QuanReplyModel>>(hah.result.ToString());
                    }
                    list_Items.Items.Add(item);
                }
                pages++;
            }
            catch (Exception)
            {
                messShow.Show("加载更多失败", 3000);
            }
            finally
            {
                pr_Load.Visibility = Visibility.Collapsed;
                qLoading = false;
            }

        }

        private void AddContent(string a, List<QuanInfoModel> ls)
        {
            grid.Children.Clear();
            //rich_Content.Blocks.Clear();
            a = a.Replace("</p>", string.Empty);
            a = a.Replace("<p>", string.Empty);
            a = a.Replace("[br]", "\n");
            MatchCollection mc = Regex.Matches(a, @"\[img data-bili-img-id=""(.*?)"" src=""(.*?)""\]");
            foreach (Match item in mc)
            {
                a = a.Replace(item.Groups[0].Value, "\n" + item.Groups[0].Value);
            }
            MatchCollection mc2 = Regex.Matches(a, @"\[:(.*?)\]");
            foreach (Match item in mc2)
            {
                a = a.Replace(item.Groups[0].Value, string.Format(@"<InlineUIContainer><Image MaxWidth=""56"" Source=""ms-appx:///Assets/BQ_TV/{0}.png""></Image></InlineUIContainer>", item.Groups[1].Value));
            }
            //\[a href="(.*?)" target=".*?"\](.*?)\[/a\]
            MatchCollection mc3 = Regex.Matches(a, @"\[a href=""(.*?)"" target="".*?""\](.*?)\[/a\]");
            foreach (Match item in mc3)
            {
                a = a.Replace(item.Groups[0].Value, string.Format(@"<InlineUIContainer><HyperlinkButton  Margin=""5 -10"" Content=""{0}"" NavigateUri=""{1}""></HyperlinkButton></InlineUIContainer>", item.Groups[2].Value, item.Groups[1].Value));
            }

            string[] st = a.Split('\n');

            string xml = string.Empty;

            for (int i = 0; i < st.Length; i++)
            {
                Match mcs = Regex.Match(st[i], @"\[img data-bili-img-id=""(.*?)"" src=""(.*?)""\]");
                if (mcs.Length != 0)
                {
                    var imgitem = ls.Find(x => x.image_id == mcs.Groups[1].Value);
                    if (imgitem != null)
                    {
                        a = a.Replace(mcs.Groups[0].Value, string.Format(@"<Paragraph TextAlignment=""Center""><InlineUIContainer >  <Border Margin=""10"" BorderThickness=""1"" BorderBrush=""#66808080""><Image x:Name=""{1}""  MinWidth=""56"" MinHeight=""56"" Tapped=""Img_Tapped"" Stretch=""{2}"" HorizontalAlignment=""Center"" Source=""{0}"" MaxWidth=""500""></Image></Border></InlineUIContainer></Paragraph>", mcs.Groups[2].Value, "i_" + mcs.Groups[1].Value, (imgitem.width > 500) ? "Uniform" : "None"));
                    }
                    else
                    {
                        a = a.Replace(mcs.Groups[0].Value, string.Format(@"<Paragraph TextAlignment=""Center""><InlineUIContainer >  <Border Margin=""10"" BorderThickness=""1"" BorderBrush=""#66808080""><Image  MinWidth=""56"" MinHeight=""56"" Tapped=""Img_Tapped"" Stretch=""{1}"" HorizontalAlignment=""Center"" Source=""{0}"" MaxWidth=""500""></Image></Border></InlineUIContainer></Paragraph>", mcs.Groups[2].Value, "Uniform"));
                    }

                }
                else
                {
                    if (st[i].Length != 0)
                    {
                        a = a.Replace(st[i], string.Format(@"<Paragraph>{0}</Paragraph>", st[i]));
                    }
                    // a = a.Replace(item.Groups[0].Value, string.Format(@"<LineBreak/><InlineUIContainer><Border><Image Source=""{0}"" MaxWidth=""500"" Margin=""10"" HorizontalAlignment=""Center""/></Border></InlineUIContainer>", item.Groups[1].Value));
                }

                //Match mcs = Regex.Match(st[i], @"\[img data-bili-img-id="".*?"" src=""(.*?)""\]");
                //if (mcs.Length != 0)
                //{
                //    Paragraph p = new Paragraph();
                //    InlineUIContainer ins = new InlineUIContainer();
                //    Image img = new Image();
                //    //Regex.Match(st[i], @"\[img data-bili-img-id="".*?"" src=""(.*?)""\]").Groups[1].Value;
                //    string ab = Regex.Match(st[i], @"\[img data-bili-img-id="".*?"" src=""(.*?)""\]").Groups[1].Value;
                //    img.Source = new BitmapImage(new Uri(ab));
                //    img.MaxWidth = 500;
                //    img.Stretch = Stretch.Uniform;
                //    img.Margin = new Thickness(5);
                //    img.Tapped += Img_Tapped;
                //    p.TextAlignment = TextAlignment.Center;
                //    ins.Child = img;
                //    p.Inlines.Add(ins);
                //    rich_Content.Blocks.Add(p);
                //}
                //else
                //{
                //    Run r = new Run();
                //    r.Text = st[i];
                //    Paragraph p = new Paragraph();
                //    p.Inlines.Add(r);
                //    rich_Content.Blocks.Add(p);
                //}
                //生成xaml

            }


            RichTextBlock p;
            try
            {
                var xaml = string.Format(@"<RichTextBlock HorizontalAlignment=""Stretch"" Margin=""5"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
     xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc = ""http://schemas.openxmlformats.org/markup-compatibility/2006"" >
                                          {0}
                                      </RichTextBlock>", a);
                p = (RichTextBlock)XamlReader.Load(xaml);
            }
            catch (Exception)
            {
                var xaml = string.Format(@"<RichTextBlock HorizontalAlignment=""Stretch"" Margin=""5"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
     xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc = ""http://schemas.openxmlformats.org/markup-compatibility/2006"" >
                                          {0}
                                      </RichTextBlock>", XmlConvert.EncodeName(a));
                p = (RichTextBlock)XamlReader.Load(a);
            }

            grid.Children.Add(p);
            // List<Block>




        }

        private async void Img_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await new MessageDialog(((sender as Image).Source as BitmapImage).UriSource.AbsoluteUri).ShowAsync();
        }
        bool qLoading = false;
        private void sv_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sv.VerticalOffset == sv.ScrollableHeight)
            {
                if (!qLoading && pages <= total_page)
                {
                    AddItems();
                }
                else
                {
                    messShow.Show("加载完了...", 1500);
                }
            }
        }

        private void btn_Share_Click(object sender, RoutedEventArgs e)
        {
            Windows.ApplicationModel.DataTransfer.DataPackage pack = new Windows.ApplicationModel.DataTransfer.DataPackage();
            pack.SetText(par.post_url);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(pack); // 保存 DataPackage 对象到剪切板
            Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
            messShow.Show("已将内容复制到剪切板", 2000);
        }

        private void btn_Report_Click(object sender, RoutedEventArgs e)
        {
            c_Report.Visibility = Visibility.Visible;
        }

        private void c_Report_Canceled()
        {
            c_Report.Visibility = Visibility.Collapsed;
        }

        private void c_Report_Reported(Controls.QuanReportDialog.ReportType type, string Content)
        {

        }

        private async void btn_Zan_Click(object sender, RoutedEventArgs e)
        {


            try
            {
                string url = string.Format("http://www.im9.com/api/praise.item.do?access_key={0}&actionKey=appkey&appkey={1}&build=418000&community_id={2}&mobi_app=android&platform=android&data_type=1&post_id={3}&item_id={4}&ts={5}", ApiHelper.access_key, ApiHelper._appKey_Android, par.community_id, par.post_id, par.post_id, ApiHelper.GetTimeSpen);
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                string result = await wc.PostResults(new Uri(url), string.Empty, "http://www.im9.com", "www.im9.com");
                CodeModel code = JsonConvert.DeserializeObject<CodeModel>(result);
                if (code.code == 0)
                {
                    CodeModel codes = JsonConvert.DeserializeObject<CodeModel>(code.data.ToString());
                    if (codes.status == 1)
                    {
                        (pane_Info.DataContext as QuanInfoModel).praise_count++;
                        btn_Zan.Foreground = (SolidColorBrush)this.Frame.Tag;
                        messShow.Show("点赞成功", 2000);

                    }
                    else
                    {
                        messShow.Show("点赞失败" + result, 3000);
                    }
                }
                else
                {
                    messShow.Show("点赞失败" + result, 3000);
                }

            }
            catch (Exception)
            {
                messShow.Show("点赞发生错误", 3000);
            }
        }

        private void btn_LookMaster_Click(object sender, RoutedEventArgs e)
        {
            if (pane_Info.DataContext == null)
            {
                return;
            }
            if (btn_LookMaster.IsChecked.Value)
            {
                Look_landlord = 2;
                list_Items.Items.Clear();
                pages = 1;
                total_page = 1;
                pane_Info.DataContext = null;
                sv.ScrollToVerticalOffset(0);
                GetInfo();
            }
            else
            {
                Look_landlord = 1;
                list_Items.Items.Clear();
                pages = 1;
                total_page = 1;
                pane_Info.DataContext = null;
                sv.ScrollToVerticalOffset(0);
                GetInfo();
            }

        }

        private void btn_ReverseOrder_Click(object sender, RoutedEventArgs e)
        {
            if (pane_Info.DataContext == null)
            {
                return;
            }
            if (btn_ReverseOrder.IsChecked.Value)
            {
                reverse = 2;
                list_Items.Items.Clear();
                pages = 1;
                total_page = 1;
                pane_Info.DataContext = null;
                sv.ChangeView(null, 1, null);
                GetInfo();
            }
            else
            {
                reverse = 1;
                list_Items.Items.Clear();
                pages = 1;
                total_page = 1;
                pane_Info.DataContext = null;
                sv.ChangeView(null, 1, null);
                GetInfo();
            }
        }

        private void btn_User_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserInfoPage), (pane_Info.DataContext as QuanInfoModel).author_mid.ToString());
        }

        private void user_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UserInfoPage), ((sender as HyperlinkButton).DataContext as QuanReplyModel).mid.ToString());

        }

        private void btn_Reply_Click(object sender, RoutedEventArgs e)
        {

        }
    }



    public class QuanInfoModel : INotifyPropertyChanged
    {
        public int code { get; set; }
        public object data { get; set; }
        public int total_count { get; set; }
        public int total_page { get; set; }


        public object post_info { get; set; }
        public int community_id { get; set; }//圈子ID
        public string community_name { get; set; }//圈子名称
        public string post_context { get; set; }//回复内容
        public int has_auth_admin { get; set; }
        public int join_state { get; set; }//状态?
        public int post_id { get; set; }
        public string post_title { get; set; }
        public string author_mid { get; set; }
        public string author_name { get; set; }
        public string author_avatar { get; set; }
        public int reply_count { get; set; }//回复数
        private int _praise_count;



        public int praise_count
        {
            get { return _praise_count; }
            set { _praise_count = value; RaisePropertyChanged("praise_count"); }
        }//点赞数

        public string post_url { get; set; }
        public int is_top { get; set; }//是否置顶,0为Flase
        public int is_essence { get; set; }//是否加精,0为Flase
        public int is_praise { get; set; }//是否点赞,0为Flase
        public int is_collect { get; set; }//是否收藏,0为Flase
        public int sex { get; set; }
        public int level { get; set; }
        public string postTime
        {
            get
            {
                DateTime dtStart = new DateTime(1970, 1, 1);
                long lTime = long.Parse(post_time + "0000");
                //long lTime = long.Parse(textBox1.Text);
                TimeSpan toNow = new TimeSpan(lTime);
                DateTime dt = dtStart.Add(toNow).ToLocalTime();
                TimeSpan span = DateTime.Now - dt;
                if (span.TotalDays > 7)
                {
                    return dt.ToString("MM-dd");
                }
                else
                if (span.TotalDays > 1)
                {
                    return string.Format("{0}天前", (int)Math.Floor(span.TotalDays));
                }
                else
                if (span.TotalHours > 1)
                {
                    return string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
                }
                else
                if (span.TotalMinutes > 1)
                {
                    return string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
                }
                else
                if (span.TotalSeconds >= 1)
                {
                    return string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
                }
                else
                {
                    return "1秒前";
                }
            }
        }
        public long post_time { get; set; }


        public object post_reply_list { get; set; }



        public long reply_time { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }
        public List<QuanInfoModel> post_image_list { get; set; }
        public string id { get; set; }
        public string img_suffix { get; set; }//jpeg
        public string image_id { get; set; }
        public string image_url { get; set; }
        public double width { get; set; }
        public double height { get; set; }
    }

    public class QuanReplyModel
    {
        public int total_count { get; set; }
        public int total_page { get; set; }

        public object result { get; set; }
        public int reply_id { get; set; }
        public long reply_time { get; set; }
        public int is_landlord { get; set; }//是否楼主，0为True
        public Visibility Is_landlord
        {
            get
            {
                if (is_landlord == 0)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }
        public int reply_count { get; set; }
        public int floor_num { get; set; }
        public int praise_count { get; set; }
        public int mid { get; set; }
        public string avatar { get; set; }
        public string username { get; set; }
        public int is_force_open { get; set; }//?????
        public int sex { get; set; }
        public int level { get; set; }

        public Object reply_reply_list { get; set; }
        public int pageSize { get; set; }
        public int page_number { get; set; }

        public int reply_reply_id { get; set; }
        public string reply_context { get; set; }

        public RichTextBlock Reply_Context { get; set; }

        public string Reply_time
        {
            get
            {
                DateTime dtStart = new DateTime(1970, 1, 1);
                long lTime = long.Parse(reply_time + "0000");
                //long lTime = long.Parse(textBox1.Text);
                TimeSpan toNow = new TimeSpan(lTime);
                DateTime dt = dtStart.Add(toNow).ToLocalTime();
                TimeSpan span = DateTime.Now - dt;
                if (span.TotalDays > 7)
                {
                    return dt.ToString("MM-dd");
                }
                else
                if (span.TotalDays > 1)
                {
                    return string.Format("{0}天前", (int)Math.Floor(span.TotalDays));
                }
                else
                if (span.TotalHours > 1)
                {
                    return string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
                }
                else
                if (span.TotalMinutes > 1)
                {
                    return string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
                }
                else
                if (span.TotalSeconds >= 1)
                {
                    return string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
                }
                else
                {
                    return "1秒前";
                }
            }
        }

        public Visibility More
        {
            get
            {
                if (reply_count > 3)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
        }

        public RichTextBlock Content
        {
            get
            {
                reply_context = reply_context.Replace("</p>", string.Empty);
                reply_context = reply_context.Replace("<p>", string.Empty);
                reply_context = reply_context.Replace("[br]", "\n");
                MatchCollection mc = Regex.Matches(reply_context, @"\[img.*?data-bili-img-id="".*?"".*?src=""(.*?)"".*?\]");
                foreach (Match item in mc)
                {
                    reply_context = reply_context.Replace(item.Groups[0].Value, "\n" + item.Groups[0].Value);
                }
                MatchCollection mc2 = Regex.Matches(reply_context, @"\[:(.*?)\]");
                foreach (Match item in mc2)
                {
                    reply_context = reply_context.Replace(item.Groups[0].Value, string.Format(@"<InlineUIContainer><Image MaxWidth=""56"" Source=""ms-appx:///Assets/BQ_TV/{0}.png"" ></Image></InlineUIContainer>", item.Groups[1].Value));
                }
                MatchCollection mc3 = Regex.Matches(reply_context, @"\[a href=""(.*?)"" target="".*?""\](.*?)\[/a\]");
                foreach (Match item in mc3)
                {
                    reply_context = reply_context.Replace(item.Groups[0].Value, string.Format(@"<InlineUIContainer><HyperlinkButton Margin=""5 -10"" Content=""{0}"" NavigateUri=""{1}""></HyperlinkButton></InlineUIContainer>", item.Groups[2].Value, item.Groups[1].Value));
                }


                string[] st = reply_context.Split('\n');

                string xml = string.Empty;

                for (int i = 0; i < st.Length; i++)
                {
                    MatchCollection mcs = Regex.Matches(st[i], @"\[img data-bili-img-id=""(.*?)"" src=""(.*?)""\]");
                    if (mcs.Count != 0)
                    {
                        foreach (Match item in mcs)
                        {
                            var imgitem = reply_image_list.Find(x => x.image_id == item.Groups[1].Value);
                            if (imgitem != null)
                            {
                                reply_context = reply_context.Replace(item.Groups[0].Value, string.Format(@"<Paragraph TextAlignment=""Center""><InlineUIContainer >  <Border Margin=""10"" BorderThickness=""1"" BorderBrush=""#66808080""><Image x:Name=""{1}""  MinWidth=""56"" MinHeight=""56"" Tapped=""Img_Tapped"" Stretch=""{2}"" HorizontalAlignment=""Center"" Source=""{0}"" MaxWidth=""500""></Image></Border></InlineUIContainer></Paragraph>", item.Groups[2].Value, "i_" + item.Groups[1].Value, (imgitem.width > 500) ? "Uniform" : "None"));
                            }
                            else
                            {
                                reply_context = reply_context.Replace(item.Groups[0].Value, string.Format(@"<Paragraph TextAlignment=""Center""><InlineUIContainer >  <Border Margin=""10"" BorderThickness=""1"" BorderBrush=""#66808080""><Image  MinWidth=""56"" MinHeight=""56"" Tapped=""Img_Tapped"" Stretch=""{1}"" HorizontalAlignment=""Center"" Source=""{0}"" MaxWidth=""500""></Image></Border></InlineUIContainer></Paragraph>", item.Groups[2].Value, "Uniform"));
                            }

                            //reply_context = reply_context.Replace(item.Groups[0].Value, string.Format(@"<Paragraph TextAlignment=""Center""><InlineUIContainer >  <Border Margin=""10"" BorderThickness=""1"" BorderBrush=""#66808080""><Image  MinWidth=""56"" MinHeight=""56"" Tapped=""Img_Tapped"" HorizontalAlignment=""Center"" Stretch=""Uniform"" Source=""{0}"" MaxWidth=""500""></Image></Border></InlineUIContainer></Paragraph>", item.Groups[1].Value));
                        }
                    }
                    else
                    {
                        if (st[i].Length != 0)
                        {
                            // System.Security.
                            // XmlConvert.EncodeName(
                            //System.Security.SecurityElement.Escape(s);
                            reply_context = reply_context.Replace(st[i], string.Format(@"<Paragraph>{0}</Paragraph>", st[i]));
                        }
                        // a = a.Replace(item.Groups[0].Value, string.Format(@"<LineBreak/><InlineUIContainer><Border><Image Source=""{0}"" MaxWidth=""500"" Margin=""10"" HorizontalAlignment=""Center""/></Border></InlineUIContainer>", item.Groups[1].Value));
                    }


                }
                RichTextBlock p;
                try
                {
                    var xaml = string.Format(@"<RichTextBlock HorizontalAlignment=""Stretch"" Margin=""5"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                            xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc = ""http://schemas.openxmlformats.org/markup-compatibility/2006"" >
                                          {0}
                                      </RichTextBlock>", reply_context);

                    p = (RichTextBlock)XamlReader.Load(xaml);
                }
                catch (Exception)
                {
                    var xaml = string.Format(@"<RichTextBlock HorizontalAlignment=""Stretch"" Margin=""5"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                                            xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"" xmlns:d=""http://schemas.microsoft.com/expression/blend/2008""
    xmlns:mc = ""http://schemas.openxmlformats.org/markup-compatibility/2006"" >
                                          {0}
                                      </RichTextBlock>", XmlConvert.EncodeName(reply_context));
                    p = (RichTextBlock)XamlReader.Load(xaml);
                    
                }
                return p;
            }
        }

        public List<QuanReplyModel> reList
        {
            get; set;
        }
        public List<QuanReplyModel> reply_image_list { get; set; }
        public string id { get; set; }
        public string img_suffix { get; set; }//jpeg
        public string image_id { get; set; }
        public string image_url { get; set; }
        public double width { get; set; }
        public double height { get; set; }
    }


}
