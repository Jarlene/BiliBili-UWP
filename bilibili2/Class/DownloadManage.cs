﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace bilibili2.Class
{
    /**
        下载文件夹管理示例
        -视频库
        ----Bili-Down文件夹
        --------视频、番剧标题文件夹
        ------------视频信息.Json
        ------------集数文件夹
        ----------------视频.mp4
        ----------------弹幕.XML
        ----------------配置文件.Json
        **/
   public class DownloadManage:IDisposable
    {
        public DownloadManage()
        {
            settings = new SettingHelper();
            GetSetting();
            if (settings.SettingContains("DownMode"))
            {

                Mode = int.Parse(settings.GetSettingValue("DownMode").ToString());
            }
            else
            {
                settings.SetSettingValue("DownMode", 0);
                Mode = 0;
            }
        }
        int Mode = 0;
        SettingHelper settings;
        ApplicationDataContainer container = ApplicationData.Current.LocalSettings;
        WebClientClass wc;
        bool useHkIp = false;
        bool userTwIp = false;
        bool userDlIp = false;
        private void GetSetting()
        {
            //UseTW,UseHK,UseCN
            if (setting.SettingContains("UseTW"))
            {
                userTwIp = (bool)setting.GetSettingValue("UseTW");
            }
            else
            {
                userTwIp = false;
            }
            if (setting.SettingContains("UseHK"))
            {
                useHkIp = (bool)setting.GetSettingValue("UseHK");
            }
            else
            {
                useHkIp = false;
            }
            if (setting.SettingContains("UseCN"))
            {
                userDlIp = (bool)setting.GetSettingValue("UseCN");
            }
            else
            {
                userDlIp = false;
            }

        }
        public static List<string> Downloaded = new List<string>();//保存已经下载过的CID数据
        public void Dispose()
        {
            if (wc != null)
            {
                wc = null;
            }
        }

        public string ReplaceSymbol(string input)
        {
            string reg = @"\:" + @"|\;" + @"|\/" + @"|\\" + @"|\|" + @"|\," + @"|\*" + @"|\?" + @"|\""" + @"|\<" + @"|\>";//特殊字符
            Regex r = new Regex(reg);
            string strFiltered = r.Replace(input, "_");//将特殊字符替换为"_"
            return strFiltered;

        }

        public async void DownDanMu(string cid, StorageFolder folder)
        {
            try
            {
                wc = new WebClientClass();
                string results = await wc.GetResults(new Uri("http://comment.bilibili.com/" + cid + ".xml"));
                //将弹幕存在在应用文件夹
                //StorageFolder folder = ApplicationData.Current.LocalFolder;
                //StorageFolder DowFolder = await KnownFolders.VideosLibrary.CreateFolderAsync("Bili-Download", CreationCollisionOption.OpenIfExists);
                StorageFile fileWrite = await folder.CreateFileAsync(cid + ".xml", CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteTextAsync(fileWrite, results);
            }
            catch (Exception)
            {
                //return null;
            }

        }

        public async Task<string> GetVideoUri(string cid, int quality)
        {
            //http://interface.bilibili.com/playurl?platform=android&cid=5883400&quality=2&otype=json&appkey=422fd9d7289a1dd9&type=mp4
            try
            {
                wc = new WebClientClass();
                //string url = "http://interface.bilibili.com/playurl?platform=android&cid=" + cid + "&quality=" + quality + "&otype=json&appkey=422fd9d7289a1dd9&type=mp4";
                string url = "http://interface.bilibili.com/playurl?_device=uwp&cid=" + cid + "&otype=json&quality=" + quality + "&appkey=" + ApiHelper._appKey + "&access_key=" + ApiHelper.access_key + "&type=mp4&mid=" + UserClass.Uid + "&_buvid=D9EFA749-6CCA-43B3-A3D2-20225D874E672072infoc&_hwid=03005a8603001c9a&platform=uwp_desktop" + "&ts=" + ApiHelper.GetTimeSpen;
                url += "&sign=" + ApiHelper.GetSign(url);
                // url += "&sign=" + ApiHelper.GetSign(url);

                string results = "";
                VideoUriModel model = null;
                string area = "";
                if (useHkIp)
                {
                    area = "hk";
                }
                if (userTwIp)
                {
                    area = "tw";
                }
                if (userDlIp)
                {
                    area = "cn";
                }
                if (!userDlIp && !userTwIp && !useHkIp)
                {
                    results = await wc.GetResults_Phone(new Uri(url));

                    model = JsonConvert.DeserializeObject<VideoUriModel>(results);


                }
                else
                {
                    results = await wc.GetResults(new Uri("http://52uwp.com/api/BiliBili?area=" + area + "&url=" + Uri.EscapeDataString(url)));
                    MessageModel ms = JsonConvert.DeserializeObject<MessageModel>(results);


                    if (ms.code == 0)
                    {
                        model = JsonConvert.DeserializeObject<VideoUriModel>(ms.message);
                    }
                    if (ms.code == -100)
                    {
                        await new MessageDialog("远程代理失效，请联系开发者更新！").ShowAsync();
                    }
                    if (ms.code == -200)
                    {
                        await new MessageDialog("代理读取信息失败，请重试！").ShowAsync();
                    }

                }
                //-5021
                if (model.code == -5021)
                {
                    await new MessageDialog("不支持你所在地区！").ShowAsync();
                }
                List<VideoUriModel> model1 = JsonConvert.DeserializeObject<List<VideoUriModel>>(model.durl.ToString());
                return model1[0].url;
            }
            catch (Exception)
            {
                return null;
            }
        }
        SettingHelper setting = new SettingHelper();
        public async void StartDownload(DownModel downModel)
        {
            try
            {
                BackgroundDownloader downloader = new BackgroundDownloader();
                if (Mode==0)
                {
                    DownModel.group.TransferBehavior = BackgroundTransferBehavior.Serialized;
                }
                else
                {
                    DownModel.group.TransferBehavior = BackgroundTransferBehavior.Parallel;
                }
                downloader.TransferGroup = DownModel.group;

                if (setting.SettingContains("UseWifi"))
                {
                    if ((bool)setting.GetSettingValue("UseWifi"))
                    {
                        downloader.CostPolicy = BackgroundTransferCostPolicy.Default;
                    }
                    else
                    {
                        downloader.CostPolicy = BackgroundTransferCostPolicy.UnrestrictedOnly;
                    }
                }
                else
                {
                    downloader.CostPolicy = BackgroundTransferCostPolicy.UnrestrictedOnly;
                    setting.SetSettingValue("UseWifi",false);
                }
                StorageFolder DowFolder = await KnownFolders.VideosLibrary.CreateFolderAsync("Bili-Down", CreationCollisionOption.OpenIfExists);
                StorageFolder VideoFolder = await DowFolder.CreateFolderAsync(ReplaceSymbol(downModel.title), CreationCollisionOption.OpenIfExists);
                StorageFolder PartFolder = await VideoFolder.CreateFolderAsync(downModel.part, CreationCollisionOption.OpenIfExists);
                StorageFile file = await PartFolder.CreateFileAsync(downModel.mid + ".mp4", CreationCollisionOption.OpenIfExists);
                DownloadOperation downloadOp = downloader.CreateDownload(new Uri(downModel.url), file);
                downloadOp.CostPolicy = BackgroundTransferCostPolicy.UnrestrictedOnly;
                BackgroundTransferStatus downloadStatus = downloadOp.Progress.Status;
                downModel.Guid = downloadOp.Guid.ToString();
                downModel.path = downloadOp.ResultFile.Path;
                string jsonInfo = JsonConvert.SerializeObject(downModel);

                StorageFile fileWrite = await PartFolder.CreateFileAsync(downModel.Guid + ".json", CreationCollisionOption.OpenIfExists);
                await FileIO.WriteTextAsync(fileWrite, jsonInfo);

                StorageFile fileWrite2 = await DowFolder.CreateFileAsync(downModel.Guid + ".bili", CreationCollisionOption.OpenIfExists);
                await FileIO.WriteTextAsync(fileWrite2, WebUtility.UrlEncode(PartFolder.Path));
                DownDanMu(downModel.mid, PartFolder);
                downloadOp.StartAsync();
            }
            catch (Exception ex)
            {
                //WebErrorStatus error = BackgroundTransferError.GetStatus(ex.HResult);
                MessageDialog md = new MessageDialog(ex.Message);
                await md.ShowAsync();
            }
        }

        public async void GetDownOk()
        {
            try
            {
                DownloadManage.Downloaded.Clear();
                await Task.Run(async () =>
                {
                    StorageFolder DownFolder = await KnownFolders.VideosLibrary.CreateFolderAsync("Bili-Down", CreationCollisionOption.OpenIfExists);
                    //List<DownloadManage.FolderModel> list = new List<DownloadManage.FolderModel>();
                    foreach (var item in await DownFolder.GetFoldersAsync())
                    {
                        //DownloadManage.FolderModel model = new DownloadManage.FolderModel()
                        //{
                        //    title = item.Name,
                        //    count = 0,
                        //    downedCount = 0,
                        //};
                        //List<DownloadManage.DownModel> list_file = new List<DownloadManage.DownModel>();
                        foreach (var item1 in await item.GetFoldersAsync())
                        {
                            foreach (var item2 in await item1.GetFilesAsync())
                            {
                                if (item2.FileType == ".json")
                                {
                                    StorageFile files = item2;
                                    string json = await FileIO.ReadTextAsync(item2);
                                    DownloadManage.DownModel model123 = JsonConvert.DeserializeObject<DownloadManage.DownModel>(json);
                                    if (model123.downloaded == true)
                                    {
                                        ///list_file.Add(model123);
                                        //model.downedCount++;
                                        DownloadManage.Downloaded.Add(model123.mid);
                                    }
                                    //model.aid = model123.aid;
                                }
                            }
                            //model.count++;
                        }
                        //model.path = item.Path;
                        //model.downModel = list_file;
                        //list.Add(model);
                    }
                });
            }
            catch (Exception)
            {
            }

        }


        public class DownModel
        {

            public static StorageFolder DownFlie = null;//下载文件夹
            public static BackgroundTransferGroup group = BackgroundTransferGroup.CreateGroup("BILIBILI-UWP-20");//下载组，方便管理
            public string aid { get; set; }
            public string mid { get; set; }
            public string part { get; set; }//第几P
            public string path { get; set; }
            public bool isBangumi { get; set; }
            public string danmuPath { get; set; }
            public string danmuUrl { get; set; }
            public bool downloaded { get; set; }
            public int quality { get; set; }
            public string title { get; set; }
            public string Guid { get; set; }
            public string url { get; set; }
            public string partTitle { get; set; }
        }

        public class HandleModel : INotifyPropertyChanged
        {
            public DownModel downModel { get; set; }
            public CancellationTokenSource cts = new CancellationTokenSource();
            public event PropertyChangedEventHandler PropertyChanged;
            protected void thisPropertyChanged(string name)
            {
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(name));
                }
            }
            private DownloadOperation _downOp;
            public DownloadOperation downOp
            {
                get { return _downOp; }
                set
                {
                    _downOp = value;
                }
            }

            private double _progress;
            public double Progress
            {
                get { return _progress; }
                set
                {
                    _progress = value;
                    thisPropertyChanged("Progress");
                }
            }

            private string _Size;
            public string Size
            {
                get { return _Size; }
                set
                {
                    _Size = (((double)Convert.ToDouble(value) / 1024 / 1024)).ToString("0.0") + "M/" + ((Double)downOp.Progress.TotalBytesToReceive / 1024 / 1024).ToString("0.0") + "M";
                    thisPropertyChanged("Size");
                }
            }

            public string Guid { get { return downOp.Guid.ToString(); } }

            private Visibility _PauseVis;
            public Visibility PauseVis
            {
                get { return _PauseVis; }
                set { _PauseVis = value; thisPropertyChanged("PauseVis"); }
            }
            private Visibility _DownVis;
            public Visibility DownVis
            {
                get { return _DownVis; }
                set { _DownVis = value; thisPropertyChanged("DownVis"); }
            }


            public string _Status;
            public string Status
            {
                get { thisPropertyChanged("Status"); return _Status; }
                set
                {
                    switch (downOp.Progress.Status)
                    {
                        case BackgroundTransferStatus.Idle:
                            _Status = "空闲中";
                            PauseVis = Visibility.Collapsed;
                            DownVis = Visibility.Collapsed;
                            break;
                        case BackgroundTransferStatus.Running:
                            _Status = "下载中";
                            PauseVis = Visibility.Visible;
                            DownVis = Visibility.Collapsed;
                            break;
                        case BackgroundTransferStatus.PausedByApplication:
                            PauseVis = Visibility.Collapsed;
                            DownVis = Visibility.Visible;
                            _Status = "暂停中";
                            break;
                        case BackgroundTransferStatus.PausedCostedNetwork:
                            _Status = "因网络暂停";
                            PauseVis = Visibility.Collapsed;
                            DownVis = Visibility.Collapsed;
                            break;
                        case BackgroundTransferStatus.PausedNoNetwork:
                            _Status = "挂起";
                            PauseVis = Visibility.Collapsed;
                            DownVis = Visibility.Visible;
                            break;
                        case BackgroundTransferStatus.Completed:
                            _Status = "完成";
                            PauseVis = Visibility.Collapsed;
                            DownVis = Visibility.Collapsed;
                            break;
                        case BackgroundTransferStatus.Canceled:
                            _Status = "取消";
                            PauseVis = Visibility.Collapsed;
                            DownVis = Visibility.Collapsed;
                            break;
                        case BackgroundTransferStatus.Error:
                            _Status = "下载错误";
                            PauseVis = Visibility.Collapsed;
                            DownVis = Visibility.Collapsed;
                            break;
                        case BackgroundTransferStatus.PausedSystemPolicy:
                            _Status = "因系统问题暂停";
                            PauseVis = Visibility.Collapsed;
                            DownVis = Visibility.Collapsed;
                            break;
                        default:
                            _Status = "Wait...";
                            break;
                    }
                    thisPropertyChanged("Status");
                }
            }
        }

        public class FolderModel
        {
            public string aid { get; set; }
            public string sid { get; set; }
            public string title { get; set; }
            public string path { get; set; }
            public int count { get; set; }
            public List<DownModel> downModel { get; set; }
            public int downedCount { get; set; }
            public bool IsBangumi { get; set; }
        }

    }

   
    
}
