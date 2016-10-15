using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Notifications;
using Windows.Web.Http;


namespace BackTask
{
    public sealed class BackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            //通知
            ApplicationDataContainer container = ApplicationData.Current.LocalSettings;
            bool Update = true;
            if (container.Values["UpdateCT"] != null)
            {
                Update = (bool)container.Values["UpdateCT"];
            }
            else
            {
                container.Values["UpdateCT"] = true;
            }
            if (Update)
            {
                var deferral = taskInstance.GetDeferral();
                await GetLatestNews();
                deferral.Complete();
            }
            else
            {
                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                updater.Clear();
            }
        }

        private IAsyncOperation<string> GetLatestNews()
        {
            try
            {
                return AsyncInfo.Run(token => GetNews());
            }
            catch (Exception)
            {
                // ignored
            }
            return null;
        }

        private async Task<string> GetNews()
        {
            try
            {
                var response = await GetUserAttentionUpdate();

                if (response != null)
                {
                    //var news = response.Data.Take(5).ToList();
                    UpdatePrimaryTile(response);
                    //UpdateSecondaryTile(response);
                }

            }
            catch (Exception)
            {
                // ignored
            }
            return null;
        }


        private void UpdatePrimaryTile(List<GetAttentionUpdate> news)
        {
            if (news == null || !news.Any())
            {
                return;
            }

            try
            {
                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                updater.EnableNotificationQueueForWide310x150(true);
                updater.EnableNotificationQueueForSquare150x150(true);
                updater.EnableNotificationQueueForSquare310x310(true);
                updater.EnableNotificationQueue(true);
                ApplicationDataContainer container = ApplicationData.Current.LocalSettings;
                List<string> oldList = new List<string>();
                updater.Clear();
                bool updateVideo=true;
                bool updateBnagumi = true;
                try
                {
                    if (container.Values["UpdateTSVideo"] != null)
                    {
                        updateVideo = (bool)container.Values["UpdateTSVideo"];
                    }
                    else
                    {
                        container.Values["UpdateTSVideo"] = true;
                    }

                    if (container.Values["UpdateTSBangumi"] != null)
                    {
                        updateBnagumi = (bool)container.Values["UpdateTSBangumi"];
                    }
                    else
                    {
                        container.Values["UpdateTSBangumi"] = true;
                    }


                    if (container.Values["TsDt"] != null)
                    {
                        oldList = container.Values["TsDt"].ToString().Split(',').ToList();
                        //oldList.RemoveAt(0);
                    }
                    else
                    {
                        string s1 = "";
                        foreach (var item in news)
                        {
                            s1 += item.addition.aid + ",";
                        }
                        s1 = s1.Remove(s1.Length - 1);
                        container.Values["TsDt"] = s1;
                    }
                }
                catch (Exception)
                {
                }
              

                foreach (var n in news)
                {
                    if (news.IndexOf(n)<=4)
                    {
                        var doc = new XmlDocument();
                        var xml = string.Format(TileTemplateXml, n.addition.pic, n.addition.title, n.addition.description);
                        doc.LoadXml(WebUtility.HtmlDecode(xml), new XmlLoadSettings
                        {
                            ProhibitDtd = false,
                            ValidateOnParse = false,
                            ElementContentWhiteSpace = false,
                            ResolveExternals = false
                        });
                        updater.Update(new TileNotification(doc));
                    }
                    
                    //通知
                    if (oldList.Count != 0)
                    {
                        if (!oldList.Contains(n.addition.aid))
                        {
                            ToastTemplateType toastTemplate = ToastTemplateType.ToastText01;
                            XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(toastTemplate);
                            XmlNodeList toastTextElements = toastXml.GetElementsByTagName("text");
                            IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
                            ((XmlElement)toastNode).SetAttribute("duration", "short");
                            ((XmlElement)toastNode).SetAttribute("launch", n.addition.aid);
                            if (n.type == 3)
                            {
                                if (updateBnagumi)
                                {
                                    toastTextElements[0].AppendChild(toastXml.CreateTextNode("您关注的《" + n.source.title + "》" + "更新了第" + n.content.index + "话"));
                                    ToastNotification toast = new ToastNotification(toastXml);
                                    ToastNotificationManager.CreateToastNotifier().Show(toast);
                                }
                            }
                            else
                            {
                                if (updateVideo)
                                {
                                    toastTextElements[0].AppendChild(toastXml.CreateTextNode(n.source.uname + "" + "上传了《" + n.addition.title + "》"));
                                    ToastNotification toast = new ToastNotification(toastXml);
                                    ToastNotificationManager.CreateToastNotifier().Show(toast);
                                }
                               
                            }
                            
                        }
                    }
                }
                //container.Values["Ts"] = news;
                string s = "";
                foreach (var item in news)
                {
                    s += item.addition.aid + ",";
                }
                s = s.Remove(s.Length - 1);
                container.Values["TsDt"] = s;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// 关注动态
        /// </summary>
        /// <returns></returns>
        private async Task<List<GetAttentionUpdate>> GetUserAttentionUpdate()
        {
            try
            {
                using (HttpClient hc = new HttpClient())
                {
                    StorageFolder folder = ApplicationData.Current.LocalFolder;
                    StorageFile file = await folder.CreateFileAsync("us.bili", CreationCollisionOption.OpenIfExists);
                    ApiHelper.access_key = await FileIO.ReadTextAsync(file);
                    string url = string.Format("http://api.bilibili.com/x/feed/pull?access_key={0}&actionKey=appkey&appkey={1}&platform=wp&pn={2}&ps=30&ts={3}&type=0", ApiHelper.access_key, ApiHelper._appKey, 1, ApiHelper.GetTimeSpen);
                    url += "&sign=" + ApiHelper.GetSign(url);
                    HttpResponseMessage hr = await hc.GetAsync(new Uri(url));
                    hr.EnsureSuccessStatusCode();
                    string results = await hr.Content.ReadAsStringAsync();

                    //一层
                    GetAttentionUpdate model1 = JsonConvert.DeserializeObject<GetAttentionUpdate>(results);
                    if (model1.code == 0)
                    {
                        GetAttentionUpdate model2 = JsonConvert.DeserializeObject<GetAttentionUpdate>(model1.data.ToString());
                        return model2.feeds;
                    }
                    else
                    {
                        return null;
                    }



                }

            }
            catch (Exception)
            {
                return null;
            }
        }

        private const string TileTemplateXml = @"
<tile branding='name'> 
  <visual version='3'>
    <binding template='TileMedium'>
      <image src='{0}' placement='peek'/>
      <text>{1}</text>
      <text hint-style='captionsubtle' hint-wrap='true'>{2}</text>
    </binding>
    <binding template='TileWide'>
      <image src='{0}' placement='peek'/>
      <text>{1}</text>
      <text hint-style='captionsubtle' hint-wrap='true'>{2}</text>
    </binding>
    <binding template='TileLarge'>
      <image src='{0}' placement='peek'/>
      <text>{1}</text>
      <text hint-style='captionsubtle' hint-wrap='true'>{2}</text>
    </binding>
  </visual>
</tile>";
        private class GetAttentionUpdate
        {
            //必须有登录Cookie
            //Josn：http://api.bilibili.com/x/feed/pull?jsonp=jsonp&ps=20&type=1&pn=1
            //第一层
            public int code { get; set; }//状态，0为正常
            public object data { get; set; }//数据，包含第二层

            public List<GetAttentionUpdate> feeds { get; set; }

            public GetAttentionUpdate page { get; set; }
            public int count { get; set; }
            public int num { get; set; }
            public int size { get; set; }


            public string id { get; set; }//视频ID
            public string src_id { get; set; }//作者信息，包含第四层
            public string add_id { get; set; }//视频信息，包含第四层
            public int type { get; set; }
            public string mcid { get; set; }
            public GetAttentionUpdate source { get; set; }
            public string mid { get; set; }
            public string uname { get; set; }
            public string sex { get; set; }
            public string avatar { get; set; }
            public string sign { get; set; }
            public GetAttentionUpdate new_ep { get; set; }
            public string av_id { get; set; }
            public string index { get; set; }

            public GetAttentionUpdate addition { get; set; }
            public string description { get; set; }
            public string aid { get; set; }
            public string title { get; set; }//标题
            public string typename { get; set; }//播放数
            public int typeid { get; set; }//播放数

            public string play { get; set; }//弹幕数
            public string video_review { get; set; }//上传时间
            public string pic { get; set; }//封面
            public long ctime { get; set; }

            public GetAttentionUpdate content { get; set; }


            public string Create
            {
                get
                {
                    DateTime dtStart = new DateTime(1970, 1, 1);
                    //long lTime = long.Parse(ctime + "000");
                    //long lTime = long.Parse(textBox1.Text);
                    TimeSpan toNow = TimeSpan.FromSeconds(ctime);
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

        }
    }

}
