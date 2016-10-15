using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Profile;
using Windows.UI.ViewManagement;

namespace bilibili2
{
   public static class DeriveHelper
    {
        public static DeriveTypes GetDeriveType()
        {
            var deviceFamily = AnalyticsInfo.VersionInfo.DeviceFamily;
            if (deviceFamily == "Windows.Desktop")
            {
                if (UIViewSettings.GetForCurrentView().UserInteractionMode == UserInteractionMode.Mouse)
                {
                    return DeriveTypes.PC;
                }
                else
                {
                    return DeriveTypes.Pad;
                }
            }
            else
            {
                return DeriveTypes.Phone;
            }
        }

    }
}
