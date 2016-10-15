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

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace bilibili2.Pages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class QuanziPage : Page
    {
        public delegate void GoBackHandler();
        public event GoBackHandler BackEvent;
        public QuanziPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            bg.Color = ((SolidColorBrush)this.Frame.Tag).Color;
            if (e.NavigationMode==  NavigationMode.New)
            {
                par = e.Parameter as MyQuanziModel;
                sv.ScrollToVerticalOffset(0);
                Pages = 1;
                list_Top.Items.Clear();
                list_Content.Items.Clear();
                GetQuanziInfo();
                GetQuanziDT();
            }
        }

        private void btn_Back_Click(object sender, RoutedEventArgs e)
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

        MyQuanziModel par;
        WebClientClass wc;
        private async void GetQuanziInfo()
        {
            wc = new WebClientClass();
            string url = string.Format("http://www.im9.com/api/query.detail.community.do?community_id={0}&access_key={1}&actionKey=appkey&appkey={2}&build=418000&mobi_app=android&platform=android&ts={3}",par.id,ApiHelper.access_key,ApiHelper._appKey_Android,ApiHelper.GetTimeSpen);
            url += "&sign="+ApiHelper.GetSign_Android(url);
            string results = await wc.GetResults(new Uri(url));
            QuanziInfoModel model = JsonConvert.DeserializeObject<QuanziInfoModel>(results);
            QuanziInfoModel info = JsonConvert.DeserializeObject<QuanziInfoModel>(model.data.ToString());
            grid_Info.DataContext = info;
            if (info.join_state==1)
            {
                btn_Atten.Label = "关注";
                FontIcon font = new FontIcon();
                font.FontFamily = new FontFamily("Segoe MDL2 Assets");
                font.Glyph = "\uE006";
                btn_Atten.Icon = font;
            }
            else
            {
                btn_Atten.Label = "取消关注";
                FontIcon font = new FontIcon();
                font.FontFamily = new FontFamily("Segoe MDL2 Assets");
                font.Glyph = "\uE00B";
                btn_Atten.Icon = font;
            }
        }
        int Pages = 1;
        private async void GetQuanziDT()
        {
            try
            {
                qLoading = true;
                pr_Load.Visibility = Visibility.Visible;
                wc = new WebClientClass();
                string url = string.Format("http://www.im9.com/api/query.community.post.list.do?page_no={4}&community_id={0}&sort_type=1&access_key={1}&actionKey=appkey&appkey={2}&build=418000&mobi_app=android&platform=android&ts={3}",par.id, ApiHelper.access_key, ApiHelper._appKey_Android, ApiHelper.GetTimeSpen, Pages);
                url += "&sign=" + ApiHelper.GetSign_Android(url);
                string results = await wc.GetResults(new Uri(url));
                QuanziListModel model = JsonConvert.DeserializeObject<QuanziListModel>(results);
                QuanziListModel model1 = JsonConvert.DeserializeObject<QuanziListModel>(model.data.ToString());
                QuanziListModel model2 = JsonConvert.DeserializeObject<QuanziListModel>(model1.post_list.ToString());
                List<QuanziListModel> list = JsonConvert.DeserializeObject<List<QuanziListModel>>(model2.result.ToString());
                //List<QuanziListModel> lists = new List<QuanziListModel>();
                foreach (var item in list)
                {
                    item.community_id = par.id;
                    if (item.is_top==1)
                    {
                        list_Top.Items.Add(item);
                    }
                    else
                    {
                        list_Content.Items.Add(item);
                    }
                }
                Pages++;

            }
            catch (Exception)
            {
                messShow.Show("没有数据了...", 3000);
            }
            finally
            {
                qLoading = false;
                pr_Load.Visibility = Visibility.Collapsed;
            }

            //

        }
       
        private void list_Top_ItemClick(object sender, ItemClickEventArgs e)
        {
            QuanziListModel model = e.ClickedItem as QuanziListModel;
            this.Frame.Navigate(typeof(QuanInfoPage), model);
        }

        private async void btn_Atten_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                wc = new WebClientClass();
                if (btn_Atten.Label == "关注")
                {
                    string url = string.Format("http://www.im9.com/api/join.community.do?access_key={0}&actionKey=appkey&appkey={1}&build=418000&community_id={2}&mobi_app=android&platform=android&ts={3}",ApiHelper.access_key,ApiHelper._appKey_Android,par.id,ApiHelper.GetTimeSpen);
                    url += "&sign=" + ApiHelper.GetSign_Android(url);
                    string result = await wc.PostResults(new Uri(url), string.Empty, "http://www.im9.com", "www.im9.com");
                    CodeModel code = JsonConvert.DeserializeObject<CodeModel>(result);
                    if (code.code==0)
                    {
                        CodeModel codes = JsonConvert.DeserializeObject<CodeModel>(code.data.ToString());
                        if (codes.status==1)
                        {
                            messShow.Show("关注成功", 2000);
                            btn_Atten.Label = "取消关注";
                            FontIcon font = new FontIcon();
                            font.FontFamily = new FontFamily("Segoe MDL2 Assets");
                            font.Glyph = "\uE00B";
                            btn_Atten.Icon = font;
                        }
                        else
                        {
                            messShow.Show("关注失败" + result, 3000);
                        }
                    }
                    else
                    {
                        messShow.Show("关注失败" + result, 3000);
                    }
                }
                else
                {
                    string url = string.Format("http://www.im9.com/api/cancel.join.community.do?access_key={0}&actionKey=appkey&appkey={1}&build=418000&community_id={2}&mobi_app=android&platform=android&ts={3}", ApiHelper.access_key, ApiHelper._appKey_Android, par.id, ApiHelper.GetTimeSpen);
                    url += "&sign=" + ApiHelper.GetSign_Android(url);
                    string result = await wc.PostResults(new Uri(url),string.Empty, "http://www.im9.com", "www.im9.com");
                    CodeModel code = JsonConvert.DeserializeObject<CodeModel>(result);
                    if (code.code == 0)
                    {
                        CodeModel codes = JsonConvert.DeserializeObject<CodeModel>(code.data.ToString());
                        if (codes.status == 1)
                        {
                            messShow.Show("取消关注成功", 2000);
                            btn_Atten.Label = "关注";
                            FontIcon font = new FontIcon();
                            font.FontFamily = new FontFamily("Segoe MDL2 Assets");
                            font.Glyph = "\uE006";
                            btn_Atten.Icon = font;
                        }
                        else
                        {
                            messShow.Show("取消关注失败" + result, 3000);
                        }
                    }
                    else
                    {
                        messShow.Show("取消关注失败" + result, 3000);
                    }
                }
            }
            catch (Exception)
            {
                messShow.Show("发生错误",3000);

            }
           

        }
        bool qLoading = false;
        private void sv_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sv.VerticalOffset == sv.ScrollableHeight)
            {
                if (!qLoading)
                {
                    GetQuanziDT();
                }
            }
        }

        private void list_Content_ItemClick(object sender, ItemClickEventArgs e)
        {
            
        }

        private void btn_Share_Click(object sender, RoutedEventArgs e)
        {
            Windows.ApplicationModel.DataTransfer.DataPackage pack = new Windows.ApplicationModel.DataTransfer.DataPackage();
            pack.SetText("http://www.im9.com/community.html?community_id="+par.id);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(pack); // 保存 DataPackage 对象到剪切板
            Windows.ApplicationModel.DataTransfer.Clipboard.Flush();
            messShow.Show("已将链接复制到剪切板", 2000);
        }
    }

    public class QuanziInfoModel
    {
        public int code { get; set; }
        public object data { get; set; }
        public string avatar { get; set; }
        public string desc { get; set; }
        public int post_count { get; set; }
        public int member_count { get; set; }
        public int join_state { get; set; }//2为已经加入
        public string community_url { get; set; }
        public string name { get; set; }
        public string community_bg_url { get; set; }
        public string bg_url
        {
            get
            {
                if (community_bg_url==null||community_bg_url.Length==0)
                {
                    return "ms-appx:///Assets/ic_group_header_bg.png";
                }
                else
                {
                    return community_bg_url;
                }
            }
        }
        public int user_status { get; set; }
        public int certification { get; set; }
        public int member_update { get; set; }//新会员
        public string post_nickname { get; set; }
        public string member_nickname { get; set; }
        public int role_id { get; set; }//4为普通会员
    }

    public class CodeModel
    {
        public int code { get; set; }
        public object data { get; set; }
        public int status { get; set; }


    }


}
