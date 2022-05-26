using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMS_Android.Utility
{
    public class PP<T>
    {
        public T data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
    public class RespondApi
    {
        public ModelQRApi data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
    public class ModelQRApi
    {
        public string QRCode { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string UseID { get; set; }
        public string UseNam { get; set; }
        public string InOut { get; set; }
        public string IsUsed { get; set; }
    }

    public class RespondApi2
    {
        public ModelQRApi data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
    }
    public class ModelQRApi2
    {
        public string UseNam { get; set; }
    }
}