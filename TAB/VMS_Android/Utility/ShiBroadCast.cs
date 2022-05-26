using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace VMS_Android.Utility
{
    [BroadcastReceiver]
    public class ShiBroadCast : BroadcastReceiver
    {
        public ShiBroadCast()
        {
            
        }
        private WifiActivity wifiActivity;
        public ShiBroadCast(WifiActivity WifiActivity)
        {
            this.wifiActivity = WifiActivity;
        }
        private MainHome mainHome;
        public ShiBroadCast(MainHome mainHome)
        {
            this.mainHome = mainHome;
        }
        public override void OnReceive(Context context, Intent intent)
        {
            if ("com.android.example.USB_PERMISSION".Equals(context))
            {
                //MainActivity.f1538b.m1258e();
                //MainActivity.f1538b.m1249c();
                Toast.MakeText(wifiActivity.ApplicationContext, "Found USB device", 0).Show();
            }
            if ("android.hardware.usb.action.USB_DEVICE_ATTACHED".Equals(context))
            {
                Toast.MakeText(this.wifiActivity.ApplicationContext, "USB device attach", 0).Show();
                return;
            }
            if ("android.hardware.usb.action.USB_DEVICE_DETACHED".Equals(context))
            {
                //MainActivity.f1538b.m1261f();
                Toast.MakeText(this.wifiActivity.ApplicationContext, "USB device removed", 0).Show();
            }
        }
    }
}