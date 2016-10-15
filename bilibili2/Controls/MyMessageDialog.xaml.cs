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

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace bilibili2.Controls
{
    public sealed partial class MyMessageDialog : UserControl
    {
        public MyMessageDialog()
        {
            this.InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void Show(string Title,string Content,bool IsUpdate)
        {
            txt_title.Text = Title;
            txt_Content.Text = Content;
            if (IsUpdate)
            {
                btn_Update.Visibility = Visibility.Visible;
            }
            else
            {
                btn_Update.Visibility = Visibility.Collapsed;

            }
            this.Visibility = Visibility.Visible;
            
        }
        public void Close()
        {
            this.Visibility = Visibility.Collapsed;
        }
        private void OutBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Close();
        }
        public event EventHandler<RoutedEventArgs> LeftClick;
        public event EventHandler<RoutedEventArgs> RightClick;
        private void btn_Update_Click(object sender, RoutedEventArgs e)
        {
            LeftClick.Invoke(this,e);
        }

        private void btn_NotShow_Click(object sender, RoutedEventArgs e)
        {
            RightClick.Invoke(this, e);
        }
    }
}
