using bilibili2.Class;
using NotificationsExtensions.TileContent;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Display;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace bilibili2
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingPage : Page
    {
        public delegate void GoBackHandler();
        public event GoBackHandler BackEvent;
        public event GoBackHandler ChangeTheme;
        public event GoBackHandler ChangeDrak;
        public SettingPage()
        {
            this.InitializeComponent();
            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        private SettingHelper settings = new SettingHelper();
        private DisplayRequest dispRequest = null;
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

            GetSetting();
        }
        bool Geting = true;
        private async void GetSetting()
        {

            try
            {
                txt_Ver.Text = settings.GetVersion();
                string device = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
                if (!settings.SettingContains("Theme"))
                {
                    settings.SetSettingValue("Theme", "Pink");
                }
                switch (settings.GetSettingValue("Theme").ToString())
                {
                    case "Red":
                        cb_Theme.SelectedIndex = 1;
                        break;
                    case "Blue":
                        cb_Theme.SelectedIndex = 4;
                        break;
                    case "Green":
                        cb_Theme.SelectedIndex = 3;
                        break;
                    case "Pink":
                        cb_Theme.SelectedIndex = 0;
                        break;
                    case "Purple":
                        cb_Theme.SelectedIndex = 5;
                        break;
                    case "Yellow":
                        cb_Theme.SelectedIndex = 2;
                        break;
                    case "EMT":
                        cb_Theme.SelectedIndex = 6;
                        break;

                    default:
                        break;
                }
                Geting = false;
                if (settings.SettingContains("UpdateCT"))
                {
                    tw_CT.IsOn = (bool)settings.GetSettingValue("UpdateCT");
                }
                else
                {
                    tw_CT.IsOn = true;
                }

                //UpdateTSVideo,UpdateTSBangumi
                if (settings.SettingContains("UpdateTSVideo"))
                {
                    tw_TS_Video.IsOn = (bool)settings.GetSettingValue("UpdateTSVideo");
                }
                else
                {
                    tw_TS_Video.IsOn = true;
                }
                if (settings.SettingContains("UpdateTSBangumi"))
                {
                    tw_TS_Bangumi.IsOn = (bool)settings.GetSettingValue("UpdateTSBangumi");
                }
                else
                {
                    tw_TS_Bangumi.IsOn = true;
                }

                if (settings.SettingContains("Drak"))
                {
                    tw_Drak.IsOn = (bool)settings.GetSettingValue("Drak");
                }
                else
                {
                    tw_Drak.IsOn = false;
                }


                //settings.SetSettingValue("DMBorder", tw_DMBorder.IsOn);
                if (settings.SettingContains("DMBorder"))
                {
                    tw_DMBorder.IsOn = (bool)settings.GetSettingValue("DMBorder");
                }
                else
                {
                    tw_DMBorder.IsOn = true;
                }
                //settings.SetSettingValue("LoadPage", tw_LoadPage.IsOn);
                if (settings.SettingContains("LoadPage"))
                {
                    tw_LoadPage.IsOn = (bool)settings.GetSettingValue("LoadPage");
                }
                else
                {
                    tw_LoadPage.IsOn = true;
                }

                // settings.SetSettingValue("UserWebTT", tw_TT.IsOn);
                if (settings.SettingContains("UserWebTT"))
                {
                    tw_TT.IsOn = (bool)settings.GetSettingValue("UserWebTT");
                }
                else
                {
                    tw_TT.IsOn = false;
                }


                //settings.SetSettingValue("CloseADS", tw_JZ.IsOn);
                if (settings.SettingContains("CloseADS"))
                {
                    tw_JZ.IsOn = (bool)settings.GetSettingValue("CloseADS");
                }
                else
                {
                    tw_JZ.IsOn = false;
                }
               

                if (settings.SettingContains("HideTitleBar"))
                {
                    tw_HideStatusBar.IsOn = (bool)settings.GetSettingValue("HideTitleBar");
                }
                else
                {
                    tw_HideStatusBar.IsOn = true;
                }

                if (settings.SettingContains("PlayLocal"))
                {
                    tw_PlayLocal.IsOn = (bool)settings.GetSettingValue("PlayLocal");
                }
                else
                {
                    tw_PlayLocal.IsOn = true;
                }

                if (settings.SettingContains("CloseMenu"))
                {
                    tw_CloseMenu.IsOn = (bool)settings.GetSettingValue("CloseMenu");
                }
                else
                {
                    tw_CloseMenu.IsOn = false;
                }

                if (settings.SettingContains("UseWifi"))
                {
                    sw_UseWifi.IsOn = (bool)settings.GetSettingValue("UseWifi");
                }
                else
                {
                    sw_UseWifi.IsOn = false;
                }

                //ForceVideo
                if (settings.SettingContains("ForceVideo"))
                {
                    tw_forceVideoDecode.IsOn = (bool)settings.GetSettingValue("ForceVideo");
                }
                else
                {
                    tw_forceVideoDecode.IsOn = false;
                }
                //ForceVideo
                if (settings.SettingContains("ForceAudio"))
                {
                    tw_forceAudioDecode.IsOn = (bool)settings.GetSettingValue("ForceAudio");
                }
                else
                {
                    tw_forceAudioDecode.IsOn = false;
                }

                if (settings.SettingContains("HoldLight"))
                {
                    sw_Light.IsOn = (bool)settings.GetSettingValue("HoldLight");
                }
                else
                {
                    sw_Light.IsOn = false;
                }

                //UseTW,UseHK,UseCN
                if (settings.SettingContains("UseTW"))
                {
                    tw_MNTW.IsOn = (bool)settings.GetSettingValue("UseTW");
                }
                else
                {
                    tw_MNTW.IsOn = false;
                }
                if (settings.SettingContains("UseHK"))
                {
                    tw_MNGA.IsOn = (bool)settings.GetSettingValue("UseHK");
                }
                else
                {
                    tw_MNGA.IsOn = false;
                }
                if (settings.SettingContains("UseCN"))
                {
                    tw_MNDL.IsOn = (bool)settings.GetSettingValue("UseCN");
                }
                else
                {
                    tw_MNDL.IsOn = false;
                }
                //
                if (settings.SettingContains("Quality"))
                {
                    cb_Quality.SelectedIndex = int.Parse(settings.GetSettingValue("Quality").ToString());
                }
                else
                {
                    cb_Quality.SelectedIndex = 1;
                }

                //settings.SetSettingValue("DownQuality", cb_DownQuality.SelectedIndex);
                if (settings.SettingContains("DownQuality"))
                {
                    cb_DownQuality.SelectedIndex = int.Parse(settings.GetSettingValue("DownQuality").ToString());
                }
                else
                {
                    cb_DownQuality.SelectedIndex = 1;
                }
                //DownMode
                if (settings.SettingContains("DownMode"))
                {
                    cb_DownMode.SelectedIndex = int.Parse(settings.GetSettingValue("DownMode").ToString());
                }
                else
                {
                    cb_DownMode.SelectedIndex = 0;
                }

                if (settings.SettingContains("Format"))
                {
                    cb_Format.SelectedIndex = int.Parse(settings.GetSettingValue("Format").ToString());
                }
                else
                {
                    cb_Format.SelectedIndex = 0;
                }

                if (settings.SettingContains("PartOrderBy"))
                {
                    cb_orderBy.SelectedIndex = int.Parse(settings.GetSettingValue("PartOrderBy").ToString());
                }
                else
                {
                    cb_orderBy.SelectedIndex = 0;
                }


                if (settings.SettingContains("AutoPlay"))
                {
                    tw_AutoPlay.IsOn = (bool)settings.GetSettingValue("AutoPlay");
                }
                else
                {
                    settings.SetSettingValue("AutoPlay", false);
                    tw_AutoPlay.IsOn = false;
                }

                //弹幕字体
                if (settings.SettingContains("FontFamily"))
                {
                    switch ((string)settings.GetSettingValue("FontFamily"))
                    {
                        case "默认":
                            cb_Font.SelectedIndex = 0;
                            break;
                        case "雅黑":
                            cb_Font.SelectedIndex = 1;
                            break;
                        case "黑体":
                            cb_Font.SelectedIndex = 2;
                            break;
                        case "楷体":
                            cb_Font.SelectedIndex = 3;
                            break;
                        case "宋体":
                            cb_Font.SelectedIndex = 4;
                            break;
                        case "等线":
                            cb_Font.SelectedIndex = 5;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    settings.SetSettingValue("FontFamily", "默认");
                    cb_Font.SelectedIndex = 0;
                }

                if (settings.SettingContains("Full"))
                {
                    tw_AutoFull.IsOn = bool.Parse(settings.GetSettingValue("Full").ToString());
                }
                else
                {
                    if (device == "Windows.Mobile")
                    {
                        //settings.SetSettingValue("Full", true);
                        tw_AutoFull.IsOn = true;
                    }
                    else
                    {
                        //settings.SetSettingValue("Full", false);
                        tw_AutoFull.IsOn = false;
                    }
                }

                if (settings.SettingContains("DanmuJianju"))
                {
                    //slider_DanmuJianju.Value =double.Parse( settings.GetSettingValue("DanmuJianju").ToString());
                    settings.SetSettingValue("DanmuJianju", 0);
                }
                else
                {
                    settings.SetSettingValue("DanmuJianju", 0);
                    //slider_DanmuJianju.Value = 0;
                }

                if (settings.SettingContains("DanmuTran"))
                {
                    slider_DanmuTran.Value = double.Parse(settings.GetSettingValue("DanmuTran").ToString());
                }
                else
                {
                    slider_DanmuTran.Value = 100;
                }

                //DanmuNum

                if (settings.SettingContains("DanmuNum"))
                {
                    slider_Num.Value = double.Parse(settings.GetSettingValue("DanmuNum").ToString());
                }
                else
                {
                    slider_Num.Value = 0;
                }

                if (settings.SettingContains("DanmuSpeed"))
                {
                    slider_DanmuSpeed.Value = double.Parse(settings.GetSettingValue("DanmuSpeed").ToString());
                }
                else
                {
                    slider_DanmuSpeed.Value = 12;
                }
                if (settings.SettingContains("DanmuSize"))
                {
                    slider_DanmuSize.Value = double.Parse(settings.GetSettingValue("DanmuSize").ToString());
                }
                else
                {
                    if (device == "Windows.Mobile")
                    {
                        slider_DanmuSize.Value = 16;
                    }
                    else
                    {
                        slider_DanmuSize.Value = 22;
                    }
                }
            }
            catch (Exception)
            {
                await new MessageDialog("读取设置失败了，建议你重新安装此应用").ShowAsync();
                throw;
            }

        }

        private void cb_Theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            txt_bgbq.Visibility = Visibility.Collapsed;
            if (cb_Theme.SelectedItem!=null&&!Geting)
            {
                switch (cb_Theme.SelectedIndex)
                {
                    case 0:
                        settings.SetSettingValue("Theme", "Pink");
                        break;
                    case 1:
                        settings.SetSettingValue("Theme", "Red");
                        break;
                    case 2:
                        settings.SetSettingValue("Theme", "Yellow");
                        break;
                    case 3:
                        settings.SetSettingValue("Theme", "Green");
                        break;
                    case 4:
                        settings.SetSettingValue("Theme", "Blue");
                        break;
                    case 5:
                        settings.SetSettingValue("Theme", "Purple");
                        break;
                    case 6:
                        settings.SetSettingValue("Theme", "EMT");
                        txt_bgbq.Visibility = Visibility.Visible;
                        break;
                    default:
                        break;
                }
                ChangeTheme();
                bg.Color = ((SolidColorBrush)this.Frame.Tag).Color;
            }

        }

        private void tw_CT_Toggled(object sender, RoutedEventArgs e)
        {
            if (tw_CT.IsOn)
            {
                settings.SetSettingValue("UpdateCT",true);
            }
            else
            {
                settings.SetSettingValue("UpdateCT", false);
                var updater = TileUpdateManager.CreateTileUpdaterForApplication();
                updater.Clear();
            }

        }

        private void tw_HideStatusBar_Toggled(object sender, RoutedEventArgs e)
        {
            if (tw_HideStatusBar.IsOn)
            {
                settings.SetSettingValue("HideTitleBar", true);
            }
            else
            {
                settings.SetSettingValue("HideTitleBar", false);
            }
        }

        private void cb_Quality_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            settings.SetSettingValue("Quality", cb_Quality.SelectedIndex);
            
        }

        private void tw_AutoFull_Toggled(object sender, RoutedEventArgs e)
        {
            settings.SetSettingValue("Full", tw_AutoFull.IsOn);
        }

        private void tw_AutoPlay_Toggled(object sender, RoutedEventArgs e)
        {
            settings.SetSettingValue("AutoPlay", tw_AutoPlay.IsOn);
        }

        private void slider_DanmuSize_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            settings.SetSettingValue("DanmuSize", slider_DanmuSize.Value);
        }

        private void slider_DanmuSpeed_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            settings.SetSettingValue("DanmuSpeed", slider_DanmuSpeed.Value);
        }

        private void slider_DanmuJianju_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            settings.SetSettingValue("DanmuJianju", slider_DanmuJianju.Value);
        }

        private void slider_DanmuTran_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            settings.SetSettingValue("DanmuTran", slider_DanmuTran.Value);
        }

        private void slider_DanmuFont_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string a = (cb_Font.SelectedItem as ComboBoxItem).Content.ToString();
            settings.SetSettingValue("FontFamily", a);

        }

        private void tw_Drak_Toggled(object sender, RoutedEventArgs e)
        {
            if (tw_Drak.IsOn)
            {
                settings.SetSettingValue("Drak", true);
            }
            else
            {
                settings.SetSettingValue("Drak", false);
            }
            ChangeDrak();
            bg.Color = ((SolidColorBrush)this.Frame.Tag).Color;
        }

        private void btn_DeleteGuanjianzi_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in list_Guanjianzi.SelectedItems)
            {
                string b = (string)settings.GetSettingValue("Guanjianzi");
                list_Guanjianzi.Items.Remove(item);
                settings.SetSettingValue("Guanjianzi", b.Replace("|" + item, string.Empty));
            }
        }

        private void btn_GetGuanjianzi_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_OpenGuanjianzi_Click(object sender, RoutedEventArgs e)
        {
            dan_Sp.IsPaneOpen = true;
            grid_Guanjianzi.Visibility = Visibility.Visible;
            grid_Yonghu.Visibility = Visibility.Collapsed;
            if (settings.SettingContains("Guanjianzi") )
            {
                string a = (string)settings.GetSettingValue("Guanjianzi");
                if (a.Length != 0)
                {
                    list_Guanjianzi.Items.Clear();
                    foreach (var item in a.Split('|').ToList())
                    {
                        list_Guanjianzi.Items.Add(item);
                    }
                    list_Guanjianzi.Items.Remove(string.Empty);
                }
            }
            else
            {
                settings.SetSettingValue("Guanjianzi", string.Empty);
            }
        }

        private void btn_OpenYonghu_Click(object sender, RoutedEventArgs e)
        {
            dan_Sp.IsPaneOpen = true;
            grid_Guanjianzi.Visibility = Visibility.Collapsed;
            grid_Yonghu.Visibility = Visibility.Visible;
            if (settings.SettingContains("Yonghu"))
            {
                string a = (string)settings.GetSettingValue("Yonghu");
                if (a.Length != 0)
                {
                    list_Yonghu.Items.Clear();
                    foreach (var item in a.Split('|').ToList())
                    {
                        list_Yonghu.Items.Add(item);
                    }
                    list_Yonghu.Items.Remove(string.Empty);
                }
            }
            else
            {
                settings.SetSettingValue("Yonghu", string.Empty);
            }
        }

        private void btn_DeleteYonghu_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in list_Yonghu.SelectedItems)
            {
                string b = (string)settings.GetSettingValue("Yonghu") ;
                list_Yonghu.Items.Remove(item);
                settings.SetSettingValue("Yonghu", b.Replace("|" + item, string.Empty));
            }
        }

        private void btn_GetYonghu_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_AddYonghu_Click(object sender, RoutedEventArgs e)
        { 
              string b = (string)settings.GetSettingValue("Yonghu") + "|" + txt_Yonghu.Text;
               settings.SetSettingValue("Yonghu", b);
               list_Yonghu.Items.Add(txt_Yonghu.Text);
               txt_Yonghu.Text = string.Empty;
        }

        private void btn_AddGuanjianzi_Click(object sender, RoutedEventArgs e)
        {
            string b = (string)settings.GetSettingValue("Guanjianzi") + "|" + txt_Guanjianzi.Text;
            settings.SetSettingValue("Guanjianzi", b);
            list_Yonghu.Items.Add(txt_Guanjianzi.Text);
            txt_Guanjianzi.Text = string.Empty;
        }


        private void tw_Drak_Toggled_1(object sender, RoutedEventArgs e)
        {

        }

        private void sw_Light_Toggled(object sender, RoutedEventArgs e)
        {
            if (sw_Light.IsOn)
            {
                if (dispRequest == null)
                {
                    dispRequest = new DisplayRequest();
                    dispRequest.RequestActive(); 
                }
            }
            else
            {
                if (dispRequest != null)
                {
                    dispRequest = null;
                }
            }
            settings.SetSettingValue("HoldLight", sw_Light.IsOn);
        }

        private void sw_UseWifi_Toggled(object sender, RoutedEventArgs e)
        {
            settings.SetSettingValue("UseWifi", sw_UseWifi.IsOn);
        }

        private void tw_PlayLocal_Toggled(object sender, RoutedEventArgs e)
        {
            settings.SetSettingValue("PlayLocal", tw_PlayLocal.IsOn);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualWidth<=500)
            {
                dan_Sp.OpenPaneLength = this.ActualWidth;
            }
            else
            {
                dan_Sp.OpenPaneLength = 350;

            }
        }

        private void cb_Format_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            settings.SetSettingValue("Format", cb_Format.SelectedIndex);
        }

        private void tw_forceAudioDecode_Toggled(object sender, RoutedEventArgs e)
        {
            settings.SetSettingValue("ForceAudio", tw_forceAudioDecode.IsOn);
        }

        private void tw_forceVideoDecode_Toggled(object sender, RoutedEventArgs e)
        {
            settings.SetSettingValue("ForceVideo", tw_forceVideoDecode.IsOn);
        }
        
        private void btn_feedback_Click(object sender, RoutedEventArgs e)
        {

        }

        public const string appbarTileId = "BilibiliUWP1";
        private async void cb_CT_Click(object sender, RoutedEventArgs e)
        {
            Color co = new Color();
            switch (cb_CTColor.SelectedIndex)
            {
                case 0:
                    co = new Color() {R=223,G=133,B=160,A=255 };
                    break;
                case 1:
                    co = new Color() { R = 247, G = 13, B = 13, A = 255 };
                    break;
                case 2:
                    co = new Color() { R = 249, G = 239, B = 35, A = 255 };
                    break;
                case 3:
                    co = new Color() { R = 113, G = 249, B = 35, A = 255 };
                    break;
                case 4:
                    co = new Color() { R = 24, G = 189, B = 251, A = 255 };
                    break;
                case 5:
                    co = new Color() { R = 185, G = 44, B = 191, A = 255 };
                    break;
                default:
                    break;
            }
            string displayName = "BiliBili UWP";
            string tileActivationArguments = "CSCT";
            Uri square150x150Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png");
            TileSize newTileDesiredSize = TileSize.Square150x150;

            SecondaryTile secondaryTile = new SecondaryTile(appbarTileId,
                                                displayName,
                                                tileActivationArguments,
                                                square150x150Logo,
                                                newTileDesiredSize);

            secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
            secondaryTile.VisualElements.ShowNameOnSquare310x310Logo = false;
            secondaryTile.VisualElements.ForegroundText = ForegroundText.Light;
            secondaryTile.VisualElements.Square30x30Logo = new Uri("ms-appx:///Assets/Square150x150Logo.scale-200.png");
            secondaryTile.VisualElements.Wide310x150Logo= new Uri("ms-appx:///Assets/Wide310x150Logo.scale-200.png");
            secondaryTile.VisualElements.BackgroundColor = co;
            //Windows.Foundation.Rect rect =(FrameworkElement)sender;
            //Windows.UI.Popups.Placement placement = Windows.UI.Popups.Placement.Above;

            await secondaryTile.RequestCreateAsync();
            

        }

        private void tw_JZ_Toggled(object sender, RoutedEventArgs e)
        {
            settings.SetSettingValue("CloseADS", tw_JZ.IsOn);
        }

        private void tw_CloseMenu_Toggled(object sender, RoutedEventArgs e)
        {
            settings.SetSettingValue("CloseMenu", tw_CloseMenu.IsOn);
        }

        private void cb_orderBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            settings.SetSettingValue("PartOrderBy", cb_orderBy.SelectedIndex);
        }

        private void tw_DMBorder_Toggled(object sender, RoutedEventArgs e)
        {
            //tw_DMBorder
            settings.SetSettingValue("DMBorder", tw_DMBorder.IsOn);
        }

        private void tw_LoadPage_Toggled(object sender, RoutedEventArgs e)
        {
            //tw_LoadPage
            settings.SetSettingValue("LoadPage", tw_LoadPage.IsOn);
        }

        private void cb_DownQuality_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            settings.SetSettingValue("DownQuality", cb_DownQuality.SelectedIndex);
        }

        private void slider_Num_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            settings.SetSettingValue("DanmuNum", slider_Num.Value);
        }

        private void tw_TT_Toggled(object sender, RoutedEventArgs e)
        {
            settings.SetSettingValue("UserWebTT", tw_TT.IsOn);

        }

        private void cb_DownMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            settings.SetSettingValue("DownMode", cb_DownMode.SelectedIndex);
        }

        private void tw_TS_Video_Toggled(object sender, RoutedEventArgs e)
        {
            settings.SetSettingValue("UpdateTSVideo", tw_TS_Video.IsOn);
           
        }

        private void tw_TS_Bangumi_Toggled(object sender, RoutedEventArgs e)
        {
            settings.SetSettingValue("UpdateTSBangumi", tw_TS_Bangumi.IsOn);
        }

     

        private void tw_MNGA_Toggled(object sender, RoutedEventArgs e)
        {
            settings.SetSettingValue("UseHK", tw_MNGA.IsOn);
            if (tw_MNGA.IsOn)
            {
                tw_MNTW.IsOn = false;
                tw_MNDL.IsOn = false;
            }
        }

        private void tw_MNTW_Toggled(object sender, RoutedEventArgs e)
        {
            settings.SetSettingValue("UseTW", tw_MNTW.IsOn);
            if (tw_MNTW.IsOn)
            {
                tw_MNGA.IsOn = false;
                tw_MNDL.IsOn = false;
            }
        }

        private void tw_MNDL_Toggled(object sender, RoutedEventArgs e)
        {
            settings.SetSettingValue("UseCN", tw_MNDL.IsOn);
            if (tw_MNDL.IsOn)
            {
                tw_MNGA.IsOn = false;
                tw_MNTW.IsOn = false;
            }
        }
    }
}
