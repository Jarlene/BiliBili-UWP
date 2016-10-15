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
    public sealed partial class QuanReportDialog : UserControl
    {
        public delegate void ReportHandler(ReportType type,string Content);
        public delegate void CancelHandler();
        public enum ReportType
        {
            SeQing=1,
            FangDong=2,
            BaoKong=3,
            GuangGao=4,
            other=5
        }
        public event ReportHandler Reported;
        public event CancelHandler Canceled;
        public QuanReportDialog()
        {
            this.InitializeComponent();
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Canceled();
        }

        private void btn_JuBao_Click(object sender, RoutedEventArgs e)
        {
            ReportType type= new ReportType();
            if (rb_Sq.IsChecked.Value)
            {
                type = ReportType.SeQing;
            }
            if (rb_O.IsChecked.Value)
            {
                type = ReportType.other;
            }
            if (rb_Gg.IsChecked.Value)
            {
                type = ReportType.GuangGao;
            }
            if (rb_Fd.IsChecked.Value)
            {
                type = ReportType.FangDong;
            }
            if (rb_Bk.IsChecked.Value)
            {
                type = ReportType.BaoKong;
            }

            Reported(type,txt.Text);
        }

        private void OutBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Canceled();
        }
    }
}
