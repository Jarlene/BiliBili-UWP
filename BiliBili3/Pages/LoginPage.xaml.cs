using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Popups;
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
    public sealed partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            txt_User.Text = SettingHelper.Get_UserName();
            txt_Pass.Password = SettingHelper.Get_Password();
        }
        private void btn_Back_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }
        private void Login_Pass_LostFocus(object sender, RoutedEventArgs e)
        {
            KanZheNi.Visibility = Visibility.Visible;
            BuKanZheNi.Visibility = Visibility.Collapsed;
        }

        private void Login_Pass_GotFocus(object sender, RoutedEventArgs e)
        {
            KanZheNi.Visibility = Visibility.Collapsed;
            BuKanZheNi.Visibility = Visibility.Visible;
        }
        private async void Login()
        {
            if (txt_User.Text.Length == 0||txt_Pass.Password.Length==0)
            {
                MessageDialog md = new MessageDialog("账号或密码不能为空！");
                await md.ShowAsync();
            }
            else
            {
               
                sc.IsEnabled = false;
                btn_Login.Content = "正在登录";
                pr_Load.Visibility = Visibility.Visible;
                string result = await ApiHelper.LoginBilibili(txt_User.Text, txt_Pass.Password);
                if (result == "登录成功")
                {
                    SettingHelper.Set_UserName(txt_User.Text);
                    SettingHelper.Set_Password(txt_Pass.Password);
                    MessageCenter.SendLogined();
                    this.Frame.GoBack();
                }
                else
                {
                    await new MessageDialog(result).ShowAsync();
                }
                pr_Load.Visibility = Visibility.Collapsed;
                btn_Login.Content = "登录";
                sc.IsEnabled = true;
            }
        }
        private void btn_Login_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        private async void btn_SignIn_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("https://passport.bilibili.com/register/phone"));
        }

        private void txt_Pass_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                Login();
            }
         }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //https://passport.bilibili.com/resetpwd
            await Launcher.LaunchUriAsync(new Uri("https://passport.bilibili.com/resetpwd"));
        }
    }
}
