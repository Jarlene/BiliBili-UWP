using bilibili2.Class;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace bilibili2
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LoadPage : Page
    {
        public LoadPage()
        {

            this.InitializeComponent();
        }
        string parAid = "";
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter!=null&&e.Parameter.ToString().Length!=0)
            {
                parAid = e.Parameter.ToString();
            }
            // settings.SetSettingValue("BirthDay", model.birthday);
            DateTime dt = new DateTime();
            try
            {
                if (settings.SettingContains("BirthDay") && DateTime.TryParse((string)settings.GetSettingValue("BirthDay"), out dt))
                {
                    dt = Convert.ToDateTime(settings.GetSettingValue("BirthDay"));
                }
            }
            catch (Exception)
            {
            }
           
            //if (settings.SettingContains("BirthDay"))
            //{

            //    dt = Convert.ToDateTime("");
            //}
            if (dt.Day == DateTime.Now.Day && dt.Month == DateTime.Now.Month)
            {
                load_img.Source = new BitmapImage(new Uri("ms-appx:///Assets/HappyBirthDay.png"));
                switch (new Random().Next(1, 4))
                {
                    case 1:
                        txt_Load.Text = "生日就不讲段子了，生日快乐啊！";
                        break;
                    case 2:
                        txt_Load.Text = "一个人过生日吧？真可怜啊，生日快乐啊！";
                        break;
                    case 3:
                        txt_Load.Text = "Happy Birthday!你又老了一岁，哈哈";
                        break;
                    default:
                        break;
                }
            }
            else
            {
                await GetLoad();
                #region
                switch (new Random().Next(1, 39))
                {
                    case 1:
                        txt_Load.Text = "爱国、敬业、诚信、友善";
                        break;
                    case 2:
                        txt_Load.Text = "(。・`ω´・) 卖个萌";
                        break;
                    case 3:
                        txt_Load.Text = "登录后能进入会员的世界哦";
                        break;
                    case 4:
                        txt_Load.Text = "你造吗，每过59秒，1分钟就过去了";
                        break;
                    case 5:
                        txt_Load.Text = "→_→ 橙子是个帅哥";
                        break;
                    case 6:
                        txt_Load.Text = "BUG什么最讨厌了 o(￣ヘ￣*o)";
                        break;
                    case 7:
                        txt_Load.Text = "23333333";
                        break;
                    case 8:
                        txt_Load.Text = "哔哩哔哩动画 UWP";
                        break;
                    case 9:
                        txt_Load.Text = "只有帅的人才能看到这句话";
                        break;
                    case 10:
                        txt_Load.Text = "C#是世界上最好的语言！";
                        break;
                    case 11:
                        txt_Load.Text = "看到不爽的弹幕就要屏蔽掉";
                        break;
                    case 12:
                        txt_Load.Text = "书上说，看到白学家就要打死";
                        break;
                    case 13:
                        txt_Load.Text = "有妹子用这软件吗?";
                        break;
                    case 14:
                        txt_Load.Text = "看到这句话的自动为长者+1S";
                        break;
                    case 15:
                        txt_Load.Text = "年轻人不要整天习习蛤蛤的(好像打错字了";
                        break;
                    case 16:
                        txt_Load.Text = "写代码的怎么可能有女朋友...";
                        break;
                    case 17:
                        txt_Load.Text = "自由、平等、公正、法治";
                        break;
                    case 18:
                        txt_Load.Text = "注意到这句话证明你很无聊";
                        break;
                    case 19:
                        txt_Load.Text = "欢迎使用全球最大同性交友网站的UWP客户端";
                        break;
                    case 20:
                        txt_Load.Text = "好无聊...好无聊...好无聊...";
                        break;
                    case 21:
                        txt_Load.Text = "收集齐启动界面的全部句子可以找橙子拿奖励哦";
                        break;
                    case 22:
                        txt_Load.Text = "垃圾橙子，应用做得这么烂！";
                        break;
                    case 23:
                        txt_Load.Text = "～E·M·T！ E·M·T！ E·M·T！～";
                        break;
                    case 24:
                        txt_Load.Text = "看见零学家也要打死";
                        break;
                    case 25:
                        txt_Load.Text = "看漫画就用动漫之家UWP";
                        break;
                    case 26:
                        txt_Load.Text = "那啥追番UWP，追番神器！";
                        break;
                    case 27:
                        txt_Load.Text = "富强、民主、文明、和谐";
                        break;
                    case 28:
                        txt_Load.Text = "突然觉得雷姆才是真爱";
                        break;
                    case 29:
                        txt_Load.Text = "去试试我的其他应用啊";
                        break;
                    case 30:
                        txt_Load.Text = "不想当段子手的死宅不是好程序员";
                        break;
                    case 31:
                        txt_Load.Text = "想不出梗了...这里该怎么写呢...";
                        break;
                    case 32:
                        txt_Load.Text = "交流♂群530991215";
                        break;
                    case 33:
                        txt_Load.Text = "Are you OK?";
                        break;
                    case 34:
                        txt_Load.Text = "《C#从入门到精神病院》";
                        break;
                    case 35:
                        txt_Load.Text = "富强、民主、文明、和谐";
                        break;
                    case 36:
                        txt_Load.Text = "自由、平等、公正、法治";
                        break;
                    case 37:
                        txt_Load.Text = "爱国、敬业、诚信、友善";
                        break;
                    case 38:
                        txt_Load.Text = "应用埋了个福利，能找到吗？";
                        break;
                    default:
                        break;
                }
                #endregion
            }

            try
            {
                //SettingHelper strring = new SettingHelper();
                
                await RegisterBackgroundTask();
                new DownloadManage().GetDownOk();
                //if (!CheckNetworkHelper.CheckNetwork())
                //{
                //    new MessageDialog("请检查网络连接！").ShowAsync();
                //}
            }
            catch (Exception)
            {
            }
            if (goMainpage)
            {
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent(typeof(StatusBar).ToString()))
                {
                    StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                    await statusBar.ShowAsync();
                }
            }
            else
            {
                await Task.Delay(2000);

                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent(typeof(StatusBar).ToString()))
                {
                    StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                    await statusBar.ShowAsync();
                }

                if (parAid.Length != 0)
                {
                    object[] par = new object[2];
                    par[0] = LoadType.OpenAvNum;
                    par[1] = parAid;
                    this.Frame.Navigate(typeof(MainPage), par);
                }
                else
                {
                    this.Frame.Navigate(typeof(MainPage));
                }
            }
           
           
            //}

        }
        SettingHelper settings = new SettingHelper();
        private async Task GetLoad()
        {
            try
            {
                bool LoadPage = true;
               
                if (settings.SettingContains("LoadPage"))
                {
                    LoadPage = (bool)settings.GetSettingValue("LoadPage");
                }
                else
                {
                    settings.SetSettingValue("LoadPage", true);
                    LoadPage = true;
                }
                if (new UserClass().IsLogin())
                {
                    StorageFolder folder = ApplicationData.Current.LocalFolder;
                    StorageFile file = await folder.CreateFileAsync("us.bili", CreationCollisionOption.OpenIfExists);
                    ApiHelper.access_key = await FileIO.ReadTextAsync(file);
                }
               

                string device = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
                //device == "Windows.Mobile"&&this.ActualWidth<500&&
                if (LoadPage&& CheckNetworkHelper.CheckInternetConnectionType()!=InternetConnectionType.WwanConnection)
                {
                    WebClientClass wc = new WebClientClass();
                    string Result = await wc.GetResults(new Uri("http://app.bilibili.com/x/splash?plat=0&build=414000&channel=master&width=1080&height=1920"));
                    LoadModel load = JsonConvert.DeserializeObject<LoadModel>(Result);
                    List<LoadModel> ls = JsonConvert.DeserializeObject<List<LoadModel>>(load.data.ToString());
                    if (ls != null && ls.Count != 0)
                    {
                        grid_Load.DataContext = ls[ls.Count-1];
                        if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent(typeof(StatusBar).ToString()))
                        {
                            StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                            await statusBar.HideAsync();
                        }
                        await Task.Delay(3000);
                    }

                }
            }
            catch (Exception)
            {
            }
           

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //await Task.Delay(2000);
        }


        private async Task RegisterBackgroundTask()
        {
            var task = await RegisterBackgroundTask(
                typeof(BackTask.BackgroundTask),
                "BackgroundTask",
                new TimeTrigger(15, false),
                null);

            task.Progress += TaskOnProgress;
            task.Completed += TaskOnCompleted;
        }

        public static async Task<BackgroundTaskRegistration> RegisterBackgroundTask(Type taskEntryPoint,
                                                                        string taskName,
                                                                        IBackgroundTrigger trigger,
                                                                        IBackgroundCondition condition)
        {
            var status = await BackgroundExecutionManager.RequestAccessAsync();
            
            if (status == BackgroundAccessStatus.Unspecified || status == BackgroundAccessStatus.DeniedByUser)
            {
                return null;
            }

            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == taskName)
                {
                    cur.Value.Unregister(true);
                }
            }

            var builder = new BackgroundTaskBuilder
            {
                Name = taskName,
                TaskEntryPoint = taskEntryPoint.FullName
            };

            builder.SetTrigger(trigger);

            if (condition != null)
            {
                builder.AddCondition(condition);
            }

            BackgroundTaskRegistration task = builder.Register();

            Debug.WriteLine($"Task {taskName} registered successfully.");

            return task;
        }


        private void TaskOnProgress(BackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args)
        {
            Debug.WriteLine($"Background {sender.Name} TaskOnProgress.");
        }

        private void TaskOnCompleted(BackgroundTaskRegistration sender, BackgroundTaskCompletedEventArgs args)
        {
            Debug.WriteLine($"Background {sender.Name} TaskOnCompleted.");
        }
        bool goMainpage = false;
        private void grid_Load_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if ((grid_Load.DataContext as LoadModel).param != null && (grid_Load.DataContext as LoadModel).param.Length != 0)
            {
                object[] par = new object[2];
                par[0] = LoadType.OpenWeb;
                par[1] = (grid_Load.DataContext as LoadModel).param;
                goMainpage = true;
                this.Frame.Navigate(typeof(MainPage), par);
                //await Launcher.LaunchUriAsync(new Uri((grid_Load.DataContext as LoadModel).param));
            }
        }
    }

    public class LoadModel
    {
        public object data { get; set; }
        public int id { get; set; }
        public int animate { get; set; }
        public string image { get; set; }
        public string param { get; set; }
    }

}
