using BiliBili3.Controls;
using BiliBili3.Helper;
using BiliBili3.Models;
using BiliBili3.Pages;
using BiliBili3.Views;
using Microsoft.Graphics.Canvas.Effects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace BiliBili3
{
   public enum StartTypes
    {
        None,
        Video,
        Live,
        Bangumi,
        MiniVideo,
        Web,
        File
    }
    public class StartModel
    {
        public StartTypes StartType { get; set; }
        public string Par1 { get; set; }
        public string Par2 { get; set; }
        public object Par3 { get; set; }
    }


    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            DisplayInformation.GetForCurrentView().OrientationChanged += MainPage_OrientationChanged; 
        }

        private async void MainPage_OrientationChanged(DisplayInformation sender, object args)
        {
            
            if (sender.CurrentOrientation == DisplayOrientations.Landscape|| sender.CurrentOrientation == DisplayOrientations.LandscapeFlipped||sender.CurrentOrientation== (DisplayOrientations)5)
            {
                if (SettingHelper.Get_HideStatus())
                {
                    if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent(typeof(StatusBar).ToString()))
                    {
                        StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                        await statusBar.HideAsync();
                    }
                    bor_Width.Width = (this.ActualWidth / 5) - 2;
                }
               
            }
            else
            {
                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent(typeof(StatusBar).ToString()))
                {
                    StatusBar statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                    await statusBar.ShowAsync();
                    bor_Width.Width = (this.ActualWidth / 5) - 2;
                }
            }
        }

        bool IsClicks = false;
        private async void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (play_frame.CanGoBack)
            {
                e.Handled = true;
                play_frame.GoBack();
            }
            else
            {
                if (frame.CanGoBack)
                {
                    e.Handled = true;
                    frame.GoBack();
                }
                else
                {
                    if (_InBangumi)
                    {
                        e.Handled = true;
                        main_frame.GoBack();
                    }
                    else
                    {
                        if (e.Handled == false)
                        {
                            if (IsClicks)
                            {
                                Application.Current.Exit();
                            }
                            else
                            {
                                IsClicks = true;
                                e.Handled = true;
                                messShow.Show("再按一次退出应用", 1500);
                                await Task.Delay(1500);
                                IsClicks = false;
                            }
                        }
                    }

                }
            }
        }
        private async void LoadPlayApiInfo()
        {
            try
            {
                string _buid = await WebClientClass.GetResults(new Uri("http://data.bilibili.com/gv/"));
                string results = await WebClientClass.GetResults(new Uri("http://120.92.50.146/api/BiliTest?r=" + new Random().Next(1, 9999)));
                results = results.Replace("\"", "");
                if (results.Length != 0)
                {
                    string[] str = results.Split(',');
                    ApiHelper._buvid = _buid;
                    ApiHelper._hwid = str[1];
                }
            }
            catch (Exception)
            {
                //messShow.Show("读取设置失败了", 3000);
                //throw;
            }
        }
        DispatcherTimer timer;
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {

            if (SettingHelper.IsPc())
            {
                sp_View.DisplayMode = SplitViewDisplayMode.CompactOverlay;
            }
            else
            {
                sp_View.DisplayMode = SplitViewDisplayMode.Overlay;
            }
            ChangeTheme();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Start();
            timer.Tick += Timer_Tick;
            MessageCenter.ChanageThemeEvent += MessageCenter_ChanageThemeEvent;
            MessageCenter.MianNavigateToEvent += MessageCenter_MianNavigateToEvent;
            MessageCenter.InfoNavigateToEvent += MessageCenter_InfoNavigateToEvent;
            MessageCenter.PlayNavigateToEvent += MessageCenter_PlayNavigateToEvent;
            MessageCenter.HomeNavigateToEvent += MessageCenter_HomeNavigateToEvent;
            MessageCenter.BgNavigateToEvent += MessageCenter_BgNavigateToEvent; ;
            MessageCenter.Logined += MessageCenter_Logined;
            MessageCenter.ShowOrHideBarEvent += MessageCenter_ShowOrHideBarEvent;
            MessageCenter.ChangeBg += MessageCenter_ChangeBg;
            //main_frame.Navigate(typeof(HomePage));
            MessageCenter_ChangeBg();
            main_frame.Visibility = Visibility.Visible;
            menu_List.SelectedIndex = 0;
            Can_Nav = false;
            bottom.SelectedIndex = 0;
            Can_Nav = true;
            frame.Visibility = Visibility.Visible;
            frame.Navigate(typeof(BlankPage));
           
            play_frame.Visibility = Visibility.Visible;
            play_frame.Navigate(typeof(BlankPage));
           
            if (UserManage.IsLogin())
            {
                MessageCenter_Logined();
            }
            else
            {
                if (SettingHelper.Get_Password().Length!=0)
                {
                    string info = await ApiHelper.LoginBilibili(SettingHelper.Get_UserName(), SettingHelper.Get_Password());
                    if (info!="登录成功")
                    {
                        messShow.Show("过期自动登录失败",2000);
                    }
                    else
                    {
                        MessageCenter_Logined();
                    }
                }
            }
            LoadPlayApiInfo();

            if (e.Parameter!=null)
            {
                var m = e.Parameter as StartModel;
                switch (m.StartType)
                {
                    case StartTypes.None:
                        break;
                    case StartTypes.Video:
                        MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(VideoViewPage), m.Par1);
                        break;
                    case StartTypes.Live:
                        MessageCenter.SendNavigateTo(NavigateMode.Play, typeof(LiveRoomPage), m.Par1);
                        break;
                    case StartTypes.Bangumi:
                        break;
                    case StartTypes.MiniVideo:
                        MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(WebPage), "http://vc.bilibili.com/mobile/detail?vc="+ m.Par1);
                        break;
                    case StartTypes.Web:
                        MessageCenter.SendNavigateTo(NavigateMode.Info, typeof(WebPage),  m.Par1);
                        break;
                    case StartTypes.File:
                        var files = m.Par3 as IReadOnlyList<IStorageItem>;
                        List<PlayerModel> ls = new List<PlayerModel>();
                        int i = 1;
                        foreach (StorageFile file in files)
                        {
                          
                           ls.Add(new PlayerModel() { Mode = PlayMode.FormLocal, No = i.ToString(), VideoTitle = "", Title = file.DisplayName, Parameter = file, Aid = file.DisplayName, Mid = file.Path });
                            i++;
                        }
                        MessageCenter.SendNavigateTo(NavigateMode.Play, typeof(PlayerPage), new object[] { ls, 0 });
                        break;
                    default:
                        break;
                }

            }


            if (SettingHelper.Get_First())
            {
                TextBlock tx = new TextBlock() {
                    Text= @"你好，欢迎使用哔哩哔哩动画第三方UWP，使用此版本前你需要了解以下几点:
1、有些小功能尚未实现,请谅解
2、可能存在些BUG，发现请反馈至xiaoyaocz@52uwp.com
3、欢迎加入QQ群一起交流:499690038、530991215
4、哪里做得不好，请轻喷

如果你觉得好用，欢迎给我打赏瓶营养快线:
支付婊:2500655055@qq.com,**程",IsTextSelectionEnabled=true,TextWrapping= TextWrapping.Wrap};
                await new ContentDialog() { Content= tx, PrimaryButtonText="知道了"}.ShowAsync();
                SettingHelper.Set_First(false);
            }

        }

        private async void MessageCenter_ChangeBg()
        {
            if (SettingHelper.Get_CustomBG() && SettingHelper.Get_BGPath().Length != 0)
            {
                StorageFile file = await StorageFile.GetFileFromPathAsync(SettingHelper.Get_BGPath());
                if (file != null)
                {
                    img_bg.Stretch = (Stretch)SettingHelper.Get_BGStretch();
                    img_bg.HorizontalAlignment = (HorizontalAlignment)SettingHelper.Get__BGHor();
                    img_bg.VerticalAlignment = (VerticalAlignment)SettingHelper.Get_BGVer();
                    img_bg.Opacity = Convert.ToDouble(SettingHelper.Get_BGOpacity()) / 10;

                    if (SettingHelper.Get_BGMaxWidth() != 0)
                    {
                        img_bg.MaxWidth = SettingHelper.Get_BGMaxWidth();
                    }
                    else
                    {

                        img_bg.MaxWidth = double.PositiveInfinity;
                    }
                    if (SettingHelper.Get_BGMaxHeight() != 0)
                    {
                        img_bg.MaxHeight = SettingHelper.Get_BGMaxHeight();
                    }
                    else
                    {
                        img_bg.MaxHeight = double.PositiveInfinity;
                    }


                    var st = await file.OpenReadAsync();
                    BitmapImage bit = new BitmapImage();
                    await bit.SetSourceAsync(st);
                    img_bg.Source = bit;
                    if (SettingHelper.Get_FrostedGlass()!=0)
                    {
                        GlassHost.Visibility = Visibility.Visible;
                        InitializedFrostedGlass(GlassHost, SettingHelper.Get_FrostedGlass());
                    }
                    else
                    {
                        GlassHost.Visibility = Visibility.Collapsed;
                    }
                   

                }
                else
                {

                }


            }
            else
            {
                img_bg.Source = null;
            }
        }


        private void InitializedFrostedGlass(UIElement glassHost,int d)
        {
            Visual hostVisual = ElementCompositionPreview.GetElementVisual(glassHost);
            Compositor compositor = hostVisual.Compositor;

            // Create a glass effect, requires Win2D NuGet package
            var glassEffect = new GaussianBlurEffect
            {
                BlurAmount =d* 5.0f, 
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


        private void MessageCenter_BgNavigateToEvent(Type page, params object[] par)
        {
            bg_Frame.Navigate(page, par);
        }

        private void MessageCenter_ShowOrHideBarEvent(bool show)
        {
            if (show)
            {
               // row_bottom.Height = GridLength.Auto;
                //bottom.Visibility = Visibility.Visible;
                //_In.Storyboard.Begin();
                //_In.Storyboard.Completed += Storyboard_Completed;
            }
            else
            {
                //bottom.Visibility = Visibility.Visible;
                //_Out.Storyboard.Begin();
               // _Out.Storyboard.Completed += Storyboard_Completed;
            }
        }

        private void Storyboard_Completed(object sender, object e)
        {
            row_bottom.Height = new GridLength(0);
        }

        private async void MessageCenter_Logined()
        {
            btn_Login.Visibility = Visibility.Collapsed;
            btn_UserInfo.Visibility = Visibility.Visible;
            gv_User.Visibility = Visibility.Visible;
            try
            {
                UserInfoModel m = await UserManage.GetMyInfo();
                ApiHelper.userInfo = m;
                gv_user.DataContext = m;
                if (m.rank == 0 || m.rank == 5000)
                {
                    dtzz.Visibility = Visibility.Visible;
                }
                else
                {
                    dtzz.Visibility = Visibility.Collapsed;
                }
                if (m.vip.vipType != 0)
                {
                    img_VIP.Visibility = Visibility.Visible;
                }
                else
                {
                    img_VIP.Visibility = Visibility.Collapsed;
                }
                UserManage.UpdateFollowList();
            }
            catch (Exception)
            {
            }
            
        }

        private void MessageCenter_HomeNavigateToEvent(Type page, params object[] par)
        {
            main_frame.Navigate(page, par);
        }

        private void MessageCenter_PlayNavigateToEvent(Type page, params object[] par)
        {
            play_frame.Navigate(page, par);
        }

        private void MessageCenter_InfoNavigateToEvent(Type page, params object[] par)
        {
            frame.Navigate(page, par);
        }

        private void MessageCenter_MianNavigateToEvent(Type page, params object[] par)
        {
            this.Frame.Navigate(page, par);
        }

      

        private void MessageCenter_ChanageThemeEvent(object par, params object[] par1)
        {
            ChangeTheme();
        }

        private void ChangeTheme()
        {
           
            switch (SettingHelper.Get_Rigth())
            {
                case 1:
                    bg_Frame.Navigate(typeof(FastNavigatePage));
                    break;
                case 2:
                    bg_Frame.Navigate(typeof(PartPage));
                    break;
                case 3:
                    bg_Frame.Navigate(typeof(RankPage));
                    break;
                case 4:
                    bg_Frame.Navigate(typeof(TimelinePage));
                    break;
                case 5:
                    bg_Frame.Navigate(typeof(LiveAllPage));
                    break;
                default:
                    bg_Frame.Navigate(typeof(BlankPage));
                    break;
            }

           


            string ThemeName = SettingHelper.Get_Theme();
            ResourceDictionary newDictionary = new ResourceDictionary();
            switch (ThemeName)
            {
                case "Dark":
                    RequestedTheme = ElementTheme.Dark;

                    break;
                case "Red":
                 
                    newDictionary.Source = new Uri("ms-appx:///Theme/RedTheme.xaml", UriKind.RelativeOrAbsolute);
                    Application.Current.Resources.MergedDictionaries.Clear();
                    Application.Current.Resources.MergedDictionaries.Add(newDictionary);
                    RequestedTheme = ElementTheme.Dark;
                    RequestedTheme = ElementTheme.Light;
                    break;
                case "Blue":
                    
                    newDictionary.Source = new Uri("ms-appx:///Theme/BlueTheme.xaml", UriKind.RelativeOrAbsolute);
                    Application.Current.Resources.MergedDictionaries.Clear();
                    Application.Current.Resources.MergedDictionaries.Add(newDictionary);
                    RequestedTheme = ElementTheme.Dark;
                    RequestedTheme = ElementTheme.Light;
                    break;
                case "Green":
                    newDictionary.Source = new Uri("ms-appx:///Theme/GreenTheme.xaml", UriKind.RelativeOrAbsolute);
                    Application.Current.Resources.MergedDictionaries.Clear();
                    Application.Current.Resources.MergedDictionaries.Add(newDictionary);
                    RequestedTheme = ElementTheme.Dark;
                    RequestedTheme = ElementTheme.Light;
                    break;
                case "Pink":
                    newDictionary.Source = new Uri("ms-appx:///Theme/PinkTheme.xaml", UriKind.RelativeOrAbsolute);
                    Application.Current.Resources.MergedDictionaries.Clear();
                    Application.Current.Resources.MergedDictionaries.Add(newDictionary);
                    RequestedTheme = ElementTheme.Dark;
                    RequestedTheme = ElementTheme.Light;
                    break;
                case "Purple":
                    newDictionary.Source = new Uri("ms-appx:///Theme/PurpleTheme.xaml", UriKind.RelativeOrAbsolute);
                    Application.Current.Resources.MergedDictionaries.Clear();
                    Application.Current.Resources.MergedDictionaries.Add(newDictionary);
                    RequestedTheme = ElementTheme.Dark;
                    RequestedTheme = ElementTheme.Light;
                    break;
                case "Yellow":
                    newDictionary.Source = new Uri("ms-appx:///Theme/YellowTheme.xaml", UriKind.RelativeOrAbsolute);
                    Application.Current.Resources.MergedDictionaries.Clear();
                    Application.Current.Resources.MergedDictionaries.Add(newDictionary);
                    RequestedTheme = ElementTheme.Dark;
                    RequestedTheme = ElementTheme.Light;
                    break;
                case "EMT":
                    newDictionary.Source = new Uri("ms-appx:///Theme/EMTTheme.xaml", UriKind.RelativeOrAbsolute);
                   
                    Application.Current.Resources.MergedDictionaries.Clear();
                    Application.Current.Resources.MergedDictionaries.Add(newDictionary);
                   // img_Hello.Source = new BitmapImage(new Uri("ms-appx:///Assets/Logo/EMT.png"));
                    RequestedTheme = ElementTheme.Dark;
                    RequestedTheme = ElementTheme.Light;
                    break;
            }
            //tuic.To = this.ActualWidth;
            //storyboardPopOut.Begin();
            ChangeTitbarColor();
        }
        private void ChangeTitbarColor()
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var applicationView = ApplicationView.GetForCurrentView();
                applicationView.SetDesiredBoundsMode(ApplicationViewBoundsMode.UseVisible);
                // StatusBar.GetForCurrentView().HideAsync();
                StatusBar statusBar = StatusBar.GetForCurrentView();
              
                statusBar.ForegroundColor = Color.FromArgb(255, 254, 254, 254);
                statusBar.BackgroundColor = ((SolidColorBrush)grid_Top.Background).Color;
                statusBar.BackgroundOpacity = 100;
            }

            var titleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            titleBar.BackgroundColor = ((SolidColorBrush)grid_Top.Background).Color;
            titleBar.ForegroundColor = Color.FromArgb(255, 254, 254, 254);//Colors.White纯白用不了。。。
            titleBar.ButtonHoverBackgroundColor = ((SolidColorBrush)sp_View.PaneBackground).Color;
            titleBar.ButtonBackgroundColor = ((SolidColorBrush)grid_Top.Background).Color;
            titleBar.ButtonForegroundColor = Color.FromArgb(255, 254, 254, 254);
            titleBar.InactiveBackgroundColor = ((SolidColorBrush)grid_Top.Background).Color;
            titleBar.ButtonInactiveBackgroundColor = ((SolidColorBrush)grid_Top.Background).Color;
        }

        private async void Timer_Tick(object sender, object e)
        {
            //if (ApiHelper.IsLogin())
            //{
                if (await HasMessage())
                {
                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        //menu_bor_HasMessage

                        bor_TZ.Visibility = Visibility.Visible;
                    });
                }
                else
                {

                    await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        bor_TZ.Visibility = Visibility.Collapsed;
                    });
                }
            }
           
        //}

        MessageModel message = new MessageModel();
        private async Task<bool> HasMessage()
        {
            try
            {
                if (!ApiHelper.IsLogin())
                {
                    return false;
                }
                // http://message.bilibili.com/api/msg/query.room.list.do?access_key=a36a84cc8ef4ea2f92c416951c859a25&actionKey=appkey&appkey=c1b107428d337928&build=414000&page_size=100&platform=android&ts=1461404884000&sign=5e212e424761aa497a75b0fb7fbde775
                string url = string.Format("http://message.bilibili.com/api/notify/query.notify.count.do?_device=wp&_ulv=10000&access_key={0}&actionKey=appkey&appkey={1}&build=411005&platform=android&ts={2}", ApiHelper.access_key, ApiHelper._appKey, ApiHelper.GetTimeSpan);
                url += "&sign=" + ApiHelper.GetSign(url);
                string results = await WebClientClass.GetResults(new Uri(url));
                MessageModel model = JsonConvert.DeserializeObject<MessageModel>(results);

                if (model.code == 0)
                {
                    MessageModel list = JsonConvert.DeserializeObject<MessageModel>(model.data.ToString());
                    message = list;
                    if (list.reply_me != 0 || list.chat_me != 0 || list.notify_me != 0 || list.praise_me != 0 || list.at_me != 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
                //messShow.Show("读取通知失败", 3000);
            }
        }

        //侧滑来源http://www.cnblogs.com/hebeiDGL/p/4775377.html
        #region  从屏幕左侧边缘滑动屏幕时，打开 SplitView 菜单

        // SplitView 控件模板中，Pane部分的 Grid
        Grid PaneRoot;

        //  引用 SplitView 控件中， 保存从 Pane “关闭” 到“打开”的 VisualTransition
        //  也就是 <VisualTransition From="Closed" To="OpenOverlayLeft"> 这个 
        VisualTransition from_ClosedToOpenOverlayLeft_Transition;

        private void Border_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            e.Handled = true;

            // 仅当 SplitView 处于 Overlay 模式时（窗口宽度最小时）
            if (sp_View.DisplayMode == SplitViewDisplayMode.Overlay)
            {
                if (PaneRoot == null)
                {
                    // 找到 SplitView 控件中，模板的父容器
                    Grid grid = FindVisualChild<Grid>(sp_View);

                    PaneRoot = grid.FindName("PaneRoot") as Grid;

                    if (from_ClosedToOpenOverlayLeft_Transition == null)
                    {
                        // 获取 SplitView 模板中“视觉状态集合”
                        IList<VisualStateGroup> stateGroup = VisualStateManager.GetVisualStateGroups(grid);

                        //  获取 VisualTransition 对象的集合。
                        IList<VisualTransition> transitions = stateGroup[0].Transitions;

                        // 找到 SplitView.IsPaneOpen 设置为 true 时，播放的 transition
                        from_ClosedToOpenOverlayLeft_Transition = transitions?.Where(train => train.From == "Closed" && train.To == "OpenOverlayLeft").First();
                    }
                }


                // 默认为 Collapsed，所以先显示它
                PaneRoot.Visibility = Visibility.Visible;

                // 当在 Border 上向右滑动，并且滑动的总距离需要小于 Panel 的默认宽度。否则会脱离左侧窗口，继续向右拖动
                if (e.Cumulative.Translation.X >= 0 && e.Cumulative.Translation.X < sp_View.OpenPaneLength)
                {
                    CompositeTransform ct = PaneRoot.RenderTransform as CompositeTransform;
                    ct.TranslateX = (e.Cumulative.Translation.X - sp_View.OpenPaneLength);
                }
            }
        }

        private void Border_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            e.Handled = true;

            // 仅当 SplitView 处于 Overlay 模式时（窗口宽度最小时）
            if (sp_View.DisplayMode == SplitViewDisplayMode.Overlay && PaneRoot != null)
            {
                // 因为当 IsPaneOpen 为 true 时，会通过 VisualStateManager 把 PaneRoot.Visibility  设置为
                // Visibility.Visible，所以这里把它改为 Visibility.Collapsed，以回到初始状态
                PaneRoot.Visibility = Visibility.Collapsed;

                // 恢复初始状态 
                CompositeTransform ct = PaneRoot.RenderTransform as CompositeTransform;


                // 如果大于 MySplitView.OpenPaneLength 宽度的 1/2 ，则显示，否则隐藏
                if ((sp_View.OpenPaneLength + ct.TranslateX) > sp_View.OpenPaneLength / 2)
                {
                    sp_View.IsPaneOpen = true;

                    // 因为上面设置 IsPaneOpen = true 会再次播放向右滑动的动画，所以这里使用 SkipToFill()
                    // 方法，直接跳到动画结束状态
                    from_ClosedToOpenOverlayLeft_Transition?.Storyboard?.SkipToFill();

                }

                ct.TranslateX = 0;
            }
        }


        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            int count = Windows.UI.Xaml.Media.VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < count; i++)
            {
                DependencyObject child = Windows.UI.Xaml.Media.VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindVisualChild<T>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }
        #endregion
       
       
        private void play_frame_Navigated(object sender, NavigationEventArgs e)
        {
           
            if (play_frame.CanGoBack)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                if (!frame.CanGoBack)
                {
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                }
                
            }
        }
        private void frame_Navigated(object sender, NavigationEventArgs e)
        {
            if (frame.CanGoBack)
            {
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                if ( !play_frame.CanGoBack)
                {
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                }

            }

            if ((frame.Content as Page).Tag == null)
            {
                frame.Background = App.Current.Resources["Bili-Background"] as SolidColorBrush;
                return;
            }

            if ((frame.Content as Page).Tag.ToString()!= "blank")
            {
                frame.Background = App.Current.Resources["Bili-Background"] as SolidColorBrush;
            }
            else
            {
                frame.Background =null;
            }

            switch ((main_frame.Content as Page).Tag.ToString())
            {
             
                case "Cn":
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                    _InBangumi = true;
                    txt_Header.Text = "国漫";
                    break;
                case "Jp":
                    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                    _InBangumi = true;
                    txt_Header.Text = "番剧";
                    break;
                default:
                    break;
            }



            //switch ((frame.Content as Page).Tag.ToString())
            //{
            //    case "blank":c
            //        //Background="{ThemeResource Bili-Background}"

            //        break;
            //}
        }
        bool _InBangumi = false;
        private void main_frame_Navigated(object sender, NavigationEventArgs e)
        {
            //if (main_frame.CanGoBack)
            //{
            //    SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            //}
            //else
            //{
            //    if (!frame.CanGoBack && !play_frame.CanGoBack)
            //    {
            //        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            //    }

            //}

            //if (e.NavigationMode == NavigationMode.Back)
            //{
                if ((main_frame.Content as Page).Tag == null)
                {
                    return;
                }
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                Can_Nav = false;
                _InBangumi = false;
                switch ((main_frame.Content as Page).Tag.ToString())
                {
                    case "首页":
                       
                            bottom.SelectedIndex = 0;
                        
                            menu_List.SelectedIndex = 0;
                        txt_Header.Text = "首页";
                        break;
                    case "番剧":
                      
                            bottom.SelectedIndex = 2;
                       
                            menu_List.SelectedIndex = 2;
                     
                        txt_Header.Text = "番剧";
                        break;
                    case "直播":
                       
                            bottom.SelectedIndex = 1;

                            menu_List.SelectedIndex = 1;
                        //menu_List.SelectedIndex = 1;
                        //bottom.SelectedIndex =1;
                        txt_Header.Text = "直播";
                        break;
                    //case "分区":
                    //    menu_List.SelectedIndex = 3;
                    //    txt_Header.Text = "新闻资讯";
                    //    break;
                    case "动态":
                       
                            bottom.SelectedIndex = 3;
                       
                            menu_List.SelectedIndex = 3;
                       
                        //menu_List.SelectedIndex = 3;
                        //bottom.SelectedIndex = 3;
                        txt_Header.Text = "动态";
                        break;
                    case "发现":
                       
                            bottom.SelectedIndex = 4;
                        
                            menu_List.SelectedIndex = 4;
                        
                        //menu_List.SelectedIndex = 4;
                        //bottom.SelectedIndex = 4;
                        txt_Header.Text = "发现";
                        break;
                    case "设置":
                        menu_List.SelectedIndex = 6;
                        txt_Header.Text = "设置";
                        break;
                    case "Cn":
                        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                        _InBangumi = true;
                        txt_Header.Text = "国漫";
                        break;
                    case "Jp":
                        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                        _InBangumi = true;
                        txt_Header.Text = "番剧";
                        break;
                    default:
                        break;
                }
                Can_Nav = true;
           // }
        }
        bool Can_Nav = true;
        private void menu_List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!Can_Nav)
            {
                return;
            }
            switch (menu_List.SelectedIndex)
            {
                case 0:
                    main_frame.Navigate(typeof(HomePage));
                    //if (!Reg_OpenVideo)
                    //{
                    //    (main_frame.Content as HomePage).OpenVideo += MainPage_OpenVideo; 
                    //    Reg_OpenVideo = true;
                    //}
                    txt_Header.Text = "首页";
                    break;
                case 2:
                    main_frame.Navigate(typeof(BangumiPage));

                    txt_Header.Text = "番剧";
                    break;
                case 1:
                    main_frame.Navigate(typeof(LivePage));

                    txt_Header.Text = "直播";
                    break;
                //case 3:
                //    main_frame.Navigate(typeof(PartPage));

                //    txt_Header.Text = "分区";
                //    break;
                case 3:
                    main_frame.Navigate(typeof(AttentionPage));

                    txt_Header.Text = "动态";
                    break;
                case 4:
                    main_frame.Navigate(typeof(FindPage));

                    txt_Header.Text = "发现";
                    break;
                //case 4:
                //    main_frame.Navigate(typeof(SettingPage));

                //    txt_Header.Text = "设置";
                //    break;
                default:
                    break;
            }
            sp_View.IsPaneOpen = false;
        }

        private void btn_Search_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(typeof(SearchPage));
        }

        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(typeof(LoginPage));
        }

        private void btn_LogOut_Click(object sender, RoutedEventArgs e)
        {
            UserManage.Logout();
            btn_Login.Visibility = Visibility.Visible;
            btn_UserInfo.Visibility = Visibility.Collapsed;
            gv_User.Visibility = Visibility.Collapsed;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            bor_Width.Width= (this.ActualWidth / 5)-2;
        }

        private void bottom_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!Can_Nav)
            {
                return;
            }
            switch (bottom.SelectedIndex)
            {
                case 0:
                    main_frame.Navigate(typeof(HomePage));
                    //if (!Reg_OpenVideo)
                    //{
                    //    (main_frame.Content as HomePage).OpenVideo += MainPage_OpenVideo; ;
                    //    Reg_OpenVideo = true;
                    //}
                    txt_Header.Text = "首页";
                    break;
                case 2:
                    main_frame.Navigate(typeof(BangumiPage));

                    txt_Header.Text = "追番";
                    break;
                case 1:
                    main_frame.Navigate(typeof(LivePage));

                    txt_Header.Text = "直播";
                    break;
                //case 3:
                //    main_frame.Navigate(typeof(PartPage));

                //    txt_Header.Text = "分区";
                //    break;
                case 3:
                    main_frame.Navigate(typeof(AttentionPage));

                    txt_Header.Text = "动态";
                    break;
                case 4:
                    main_frame.Navigate(typeof(FindPage));

                    txt_Header.Text = "发现";
                    break;
                //case 4:
                //    main_frame.Navigate(typeof(SettingPage));

                //    txt_Header.Text = "设置";
                //    break;
                default:
                    break;
            }
            sp_View.IsPaneOpen = false;
        }

        private void btn_user_myvip_Click(object sender, RoutedEventArgs e)
        {
            //http://big.bilibili.com/site/big.html
            frame.Navigate(typeof(WebPage), new object[] { "http://big.bilibili.com/site/big.html" });
        }

        private void btn_user_mycollect_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(typeof(MyCollectPage));
        }

        private void btn_user_mychistory_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(typeof(MyHistroryPage));
        }

        private void btn_user_mywallet_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(typeof(MyWalletPage));

        }

        private void dtzz_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(typeof(WebPage), "https://account.bilibili.com/answer/base");
        }

        private void btn_UserInfo_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(typeof(UserInfoPage));
        }

        private void btn_user_myGuanzhu_Click(object sender, RoutedEventArgs e)
        {
            frame.Navigate(typeof(UserInfoPage),new object[]{ null,2});
        }

        private void btn_user_mymessage_Click(object sender, RoutedEventArgs e)
        {

            frame.Navigate(typeof(MyMessagePage));
        }

        private void btn_user_Qr_Click(object sender, RoutedEventArgs e)
        {
            //UserInfoModel m = await UserManage.GetMyInfo();
          var info=   gv_user.DataContext  as UserInfoModel;
            frame.Navigate(typeof(MyQrPage), new object[] { new MyqrModel() {
                  name=info.name,
                  photo=info.face,
                  qr=string.Format("http://qr.liantu.com/api.php?w=500&text={0}&inpt=00AAF0&logo={1}",Uri.EscapeDataString("http://space.bilibili.com/"+ApiHelper.GetUserId()),Uri.EscapeDataString(info.face)),
                  sex=info.sex
            } });
        }

        private void btn_Qr_Click(object sender, RoutedEventArgs e)
        {
            play_frame.Navigate(typeof(QRPage));
        }

        private void btn_Down_Click(object sender, RoutedEventArgs e)
        {

            frame.Navigate(typeof(DownloadPage));
        }
    }
}
