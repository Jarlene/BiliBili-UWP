using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using BackTask;
using Windows.UI;
using Windows.UI.ViewManagement;
using BiliBili3.Helper;
using Windows.UI.Composition;
using Windows.UI.Xaml.Hosting;
using Microsoft.Graphics.Canvas.Effects;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace BiliBili3
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SplashPage : Page
    {
        public SplashPage()
        {
            this.InitializeComponent();
            var bg = new Color() { R = 233, G = 233, B = 233 };
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                // StatusBar.GetForCurrentView().HideAsync();
                StatusBar statusBar = StatusBar.GetForCurrentView();
                statusBar.ForegroundColor = Colors.Black;
                statusBar.BackgroundColor = bg;
                statusBar.BackgroundOpacity = 100;
            }

            var titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = bg;
            titleBar.ForegroundColor = Colors.Black;//Colors.White纯白用不了。。。
            titleBar.ButtonHoverBackgroundColor = Colors.White;
            titleBar.ButtonBackgroundColor = bg;
            titleBar.ButtonForegroundColor = Color.FromArgb(255, 254, 254, 254);
            titleBar.InactiveBackgroundColor = bg;
            titleBar.ButtonInactiveBackgroundColor = bg;
        }
        DispatcherTimer timer;
        StartModel m;
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            #region
            switch (new Random().Next(1, 20))
            {
                case 1:
                    txt_Load.Text = "爱国、敬业、诚信、友善";
                    break;
                case 2:
                    txt_Load.Text = "富强、民主、文明、和谐";
                    break;
                case 3:
                    txt_Load.Text = "自由、平等、公正、法治";
                    break;
                case 4:
                    txt_Load.Text = "新版本，新的开始";
                    break;
                case 5:
                    txt_Load.Text = "这里放广告？";
                    break;
                case 6:
                    txt_Load.Text = "橙子贼帅！";
                    break;
                case 7:
                    txt_Load.Text = "请背会社会主义核心价值观";
                    break;
                case 8:
                    txt_Load.Text = "子曰：要善于 发现";
                    break;
                case 9:
                    txt_Load.Text = "哔哩哔哩动画 UWP";
                    break;
                case 10:
                    txt_Load.Text = "只有帅的人才能看到这句话";
                    break;
                case 11:
                    txt_Load.Text = "+1S";
                    break;
                case 12:
                    txt_Load.Text = "蛤蛤蛤蛤蛤蛤蛤蛤";
                    break;
                case 13:
                    txt_Load.Text = "垃圾橙子，应用做得这么烂！";
                    break;
                case 14:
                    txt_Load.Text = "听说背景可以自定义了";
                    break;
                case 15:
                    txt_Load.Text = "听说下载位置也可以自定义了";
                    break;
                case 16:
                    txt_Load.Text = "招女友，条件：女的，会帮我写代码";
                    break;
                case 17:
                    txt_Load.Text = "偷偷放个网址，应该没人知道:nsapp.win";
                    break;
                case 18:
                    txt_Load.Text = "招女友，条件：女的，LOL王者段位";
                    break;
                case 19:
                    txt_Load.Text = "哔哩哔哩动画 UWP";
                    break;
                //case 5:
                //    txt_Load.Text = "→_→ 橙子是个帅哥";
                //    break;
                //case 6:
                //    txt_Load.Text = "BUG什么最讨厌了 o(￣ヘ￣*o)";
                //    break;
                //case 7:
                //    txt_Load.Text = "23333333";
                //    break;
                //case 8:
                //    txt_Load.Text = "哔哩哔哩动画 UWP";
                //    break;
                //case 9:
                //    txt_Load.Text = "只有帅的人才能看到这句话";
                //    break;
                //case 10:
                //    txt_Load.Text = "C#是世界上最好的语言！";
                //    break;
                //case 11:
                //    txt_Load.Text = "看到不爽的弹幕就要屏蔽掉";
                //    break;
                //case 12:
                //    txt_Load.Text = "书上说，看到白学家就要打死";
                //    break;
                //case 13:
                //    txt_Load.Text = "有妹子用这软件吗?";
                //    break;
                //case 14:
                //    txt_Load.Text = "看到这句话的自动为长者+1S";
                //    break;
                //case 15:
                //    txt_Load.Text = "年轻人不要整天习习蛤蛤的(好像打错字了";
                //    break;
                //case 16:
                //    txt_Load.Text = "写代码的怎么可能有女朋友...";
                //    break;
                //case 17:
                //    txt_Load.Text = "自由、平等、公正、法治";
                //    break;
                //case 18:
                //    txt_Load.Text = "注意到这句话证明你很无聊";
                //    break;
                //case 19:
                //    txt_Load.Text = "欢迎使用全球最大同性交友网站的UWP客户端";
                //    break;
                //case 20:
                //    txt_Load.Text = "好无聊...好无聊...好无聊...";
                //    break;
                //case 21:
                //    txt_Load.Text = "收集齐启动界面的全部句子可以找橙子拿奖励哦";
                //    break;
                //case 22:
                //    txt_Load.Text = "垃圾橙子，应用做得这么烂！";
                //    break;
                //case 23:
                //    txt_Load.Text = "～E·M·T！ E·M·T！ E·M·T！～";
                //    break;
                //case 24:
                //    txt_Load.Text = "看见零学家也要打死";
                //    break;
                //case 25:
                //    txt_Load.Text = "看漫画就用动漫之家UWP";
                //    break;
                //case 26:
                //    txt_Load.Text = "那啥追番UWP，追番神器！";
                //    break;
                //case 27:
                //    txt_Load.Text = "富强、民主、文明、和谐";
                //    break;
                //case 28:
                //    txt_Load.Text = "突然觉得雷姆才是真爱";
                //    break;
                //case 29:
                //    txt_Load.Text = "去试试我的其他应用啊";
                //    break;
                //case 30:
                //    txt_Load.Text = "不想当段子手的死宅不是好程序员";
                //    break;
                //case 31:
                //    txt_Load.Text = "想不出梗了...这里该怎么写呢...";
                //    break;
                //case 32:
                //    txt_Load.Text = "交流♂群530991215";
                //    break;
                //case 33:
                //    txt_Load.Text = "Are you OK?";
                //    break;
                //case 34:
                //    txt_Load.Text = "《C#从入门到精神病院》";
                //    break;
                //case 35:
                //    txt_Load.Text = "富强、民主、文明、和谐";
                //    break;
                //case 36:
                //    txt_Load.Text = "自由、平等、公正、法治";
                //    break;
                //case 37:
                //    txt_Load.Text = "爱国、敬业、诚信、友善";
                //    break;
                default:
                    break;
            }
            try
            {
                //SettingHelper strring = new SettingHelper();

                await RegisterBackgroundTask();
                DownloadHelper.GetDownedList();
                //if (!CheckNetworkHelper.CheckNetwork())
                //{
                //    new MessageDialog("请检查网络连接！").ShowAsync();
                //}
            }
            catch (Exception)
            {
            }
            #endregion

            //await Task.Delay(2000); 
             m = e.Parameter as StartModel;
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;
            timer.Start();
            if (m.StartType== StartTypes.None&&SettingHelper.Get_LoadSplash())
            {
                await GetResults();
              
            }
            else
            {
               // await Task.Delay(2000);
               // this.Frame.Navigate(typeof(MainPage), m);
            }

         


        }
        int i = 1;
        int maxnum = 3;
        private void Timer_Tick(object sender, object e)
        {
            if (i!= maxnum)
            {
                i++;
            }
            else
            {
                this.Frame.Navigate(typeof(MainPage), m);
              
            }
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            timer.Stop();
            timer = null;
        }
        private void InitializedFrostedGlass(UIElement glassHost)
        {
            Visual hostVisual = ElementCompositionPreview.GetElementVisual(glassHost);
            Compositor compositor = hostVisual.Compositor;

            // Create a glass effect, requires Win2D NuGet package
            var glassEffect = new GaussianBlurEffect
            {
                BlurAmount = 20.0f,
                BorderMode = EffectBorderMode.Hard,
                Source = new ArithmeticCompositeEffect
                {
                    MultiplyAmount = 0,
                    Source1Amount = 0.5f,
                    Source2Amount = 0.5f,
                    Source1 = new CompositionEffectSourceParameter("backdropBrush"),
                    Source2 = new ColorSourceEffect
                    {
                        Color = Color.FromArgb(255, 245, 245, 245)
                    }
                }
            };

            //  Create an instance of the effect and set its source to a CompositionBackdropBrush
            var effectFactory = compositor.CreateEffectFactory(glassEffect);
            var backdropBrush = compositor.CreateBackdropBrush();
            var effectBrush = effectFactory.CreateBrush();

            effectBrush.SetSourceParameter("backdropBrush", backdropBrush);

            // Create a Visual to contain the frosted glass effect
            var glassVisual = compositor.CreateSpriteVisual();
            glassVisual.Brush = effectBrush;

            // Add the blur as a child of the host in the visual tree
            ElementCompositionPreview.SetElementChildVisual(glassHost, glassVisual);

            // Make sure size of glass host and glass visual always stay in sync
            var bindSizeAnimation = compositor.CreateExpressionAnimation("hostVisual.Size");
            bindSizeAnimation.SetReferenceParameter("hostVisual", hostVisual);

            glassVisual.StartAnimation("Size", bindSizeAnimation);
        }

        private async Task GetResults()
        {
            try
            {
                string url = "http://app.bilibili.com/x/splash?plat=0&build=414000&channel=master&width=1080&height=1920";
                bool pc = SettingHelper.IsPc();
                if (pc)
                {
                   
                    img.Stretch = Stretch.Uniform;
                    url = "http://app.bilibili.com/x/splash?plat=0&build=414000&channel=master&width=1920&height=1080";
                    
                }
               
                string Result = await WebClientClass.GetResults(new Uri(url));
                LoadModel obj = JsonConvert.DeserializeObject<LoadModel>(Result);

                if (obj.code== 0)
                {
                    if (obj.data.Count!=0)
                    {
                        var buff= await WebClientClass.GetBuffer(new Uri(obj.data[0].image));
                        BitmapImage bit = new BitmapImage();
                        await bit.SetSourceAsync(buff.AsStream().AsRandomAccessStream());
                        if (!pc)
                        {
                            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                            {
                                var applicationView = ApplicationView.GetForCurrentView();
                                applicationView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);

                                // StatusBar.GetForCurrentView().HideAsync();
                                StatusBar statusBar = StatusBar.GetForCurrentView();
                                statusBar.ForegroundColor = Colors.Gray;
                                statusBar.BackgroundColor = Color.FromArgb(255, 55, 63, 76);
                                statusBar.BackgroundOpacity = 0;
                            }
                        }
                        else
                        {
                            img_bg.Source = bit;
                            InitializedFrostedGlass(GlassHost);
                        }
                        img.Source = bit;
                        _url = obj.data[0].param;
                        maxnum = 5;
                        //await Task.Delay(3000);
                        //this.Frame.Navigate(typeof(MainPage), m);
                    }
                    else
                    {
                       // await Task.Delay(2000);
                       
                    }
                }
                else
                {
                   // await Task.Delay(2000);
                    //this.Frame.Navigate(typeof(MainPage), m);
                }


            }
            catch (Exception)
            {
               // await Task.Delay(2000);
                //this.Frame.Navigate(typeof(MainPage), m);
            }
            finally
            {

               
            }

        }
        string _url;
        private void grid_Load_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //url=
            m.StartType = StartTypes.Web;
            m.Par1 = _url;
        }

        public class LoadModel
        {
            public int code { get; set; }
            public string message { get; set; }
            public List<LoadModel> data { get; set; }
            public int id { get; set; }
            public int animate { get; set; }
            public string image { get; set; }
            public string param { get; set; }
        }


        #region 后台任务注册

        private async Task RegisterBackgroundTask()
        {
            var task = await RegisterBackgroundTask(
                typeof(BackgroundTask),
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

        #endregion

      
    }
}
