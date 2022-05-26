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

namespace VMS_Android
{
    public class ReleaseLink
    {
        public static string _Api = "https://sbm-apps.shimano.com.sg/VMSAPICLIENT";
        //public static string _Api = "http://192.168.1.3/VMSAPINEW2";
        public static string Api
        {
            get { return _Api; }
            set { _Api = value; }
        }
        public static string _Web = "https://sbm-apps.shimano.com.sg/VisitorMSQA";
        //public static string _Web = "http://192.168.1.3/VisiBukku";
        public static string Web
        {
            get { return _Web; }
            set { _Web = value; }
        }
    }
    public class DebugLink
    {
        public static string _Api = "https://sbm-apps.shimano.com.sg/VMSAPICLIENT";
        //public static string _Api = "http://192.168.1.3/VMSAPINEW2";
        public static string Api
        {
            get { return _Api; }
            set { _Api = value; }
        }
        public static string _Web = "https://sbm-apps.shimano.com.sg/VisitorMSQA";
        //public static string _Web = "http://192.168.1.3/VisiBukku";
        public static string Web
        {
            get { return _Web; }
            set { _Web = value; }
        }
    }
}