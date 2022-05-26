using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Hardware.Usb;
using Android.OS;
using Android.Preferences;
using Android.Views;
using Android.Widget;
using VMS.Core.Class;
using VMS.Core.Constants;
using VMS.Core.Repository;
using VMS_Android.Utility;
using ZXing;
using VMS.Core.Model;
using ZXing.Mobile;

namespace VMS_Android
{
    [Activity(Label = "QR Code")]
    public class WifiActivity : Activity
    {

        VisitorRepository _visitor = new VisitorRepository();
        ISharedPreferences session;
        ISharedPreferencesEditor SessionEdit;
        Button btnNext, btnPrint;
        TextView txtNet, txtNetPassword;
        System.Timers.Timer timer = new System.Timers.Timer();
        ImageView QRCode;
        string _Id, _Status;
        string _Name, _IPAddress;


        int counting = 0;
        TextView txtSession;

        //Printer
        private UsbManager mUsbManager;
        private UsbDevice mDevice;
        private static UsbDeviceConnection mConnection;
        private static UsbInterface mInterface;
        private static UsbEndpoint mEndPoint;
        private static byte[] qrcode;
        private static Android.Graphics.Bitmap result;
        private PendingIntent mPermissionIntent;

        private static string ACTION_USB_PERMISSION = "com.android.example.USB_PERMISSION";
        private static bool forceCLaim = true;
        IDictionary<string, UsbDevice> mDeviceList;
        IEnumerator<UsbDevice> mDeviceIterator;
        byte[] testBytes;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.Wifi_Scan);
            // Create your application here

            session = PreferenceManager.GetDefaultSharedPreferences(this);
            Task t2 = PostVisitorCheckin();
            Task T  = BindData();
            PrinterSetup();

            SetCustomFont();

            CountDown();
        }

        void SetCustomFont()
        {
            var tv1 = FindViewById<TextView>(Resource.Id.txtTitle);
            var tv2 = FindViewById<TextView>(Resource.Id.txtSubTitle);
            var tv3 = FindViewById<TextView>(Resource.Id.txtNetwork);
            var tv4 = FindViewById<TextView>(Resource.Id.txtNetPassword);
            var tv5 = FindViewById<TextView>(Resource.Id.footer_1);
            var tv6 = FindViewById<TextView>(Resource.Id.txtSession);

            tv1.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Bold.ttf");
            tv2.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv3.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv4.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv5.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Bold.ttf");
            tv6.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Bold.ttf");

        }

        void PrinterSetup()
        {
            try
            {
                mUsbManager = (UsbManager)GetSystemService(Context.UsbService);
                mDeviceList = mUsbManager.DeviceList;
                if (mDeviceList.Count() > 0)
                {
                    mDeviceIterator = mDeviceList.Values.GetEnumerator();
                    while (mDeviceIterator.MoveNext())
                    {
                        UsbDevice usbDevice1 = mDeviceIterator.Current;

                        if (usbDevice1.VendorId == 8864)
                        {
                            mDevice = usbDevice1;
                        }
                    }


                    ShiBroadCast mUsbReceiver = new ShiBroadCast(this);
                    mPermissionIntent = PendingIntent.GetBroadcast(this, 0, new Intent(ACTION_USB_PERMISSION), 0);
                    IntentFilter filter = new IntentFilter(ACTION_USB_PERMISSION);
                    RegisterReceiver(mUsbReceiver, filter);
                    mUsbManager.RequestPermission(mDevice, mPermissionIntent);
                    if (mDevice != null)
                    {
                        mInterface = mDevice.GetInterface(1);
                        mEndPoint = mInterface.GetEndpoint(0);// 0 IN and  1 OUT to printer.

                    }
                }
            }
            catch (Exception Ex)
            {

                throw Ex;
            }

        }

        async Task BindData()
        {
            _Id = session.GetString(nameof(gVarEnum.Id), "");
            _Status = session.GetString(nameof(gVarEnum.Status), "");
            _Name = session.GetString(nameof(gVarEnum.Name), "");
            _IPAddress = session.GetString("IPAddress", "");
            btnNext = FindViewById<Button>(Resource.Id.btnNext);
            btnPrint = FindViewById<Button>(Resource.Id.btnPrint);
            QRCode = FindViewById<ImageView>(Resource.Id.QRCode);
            txtNet = FindViewById<TextView>(Resource.Id.txtNetwork);
            txtNetPassword = FindViewById<TextView>(Resource.Id.txtNetPassword);
            txtSession = FindViewById<TextView>(Resource.Id.txtSession);
            btnNext.Click += (sender, e) =>
            {
                moveActivity();

            };
            btnPrint.Click += (sender, e) =>
            {
                mConnection = mUsbManager.OpenDevice(mDevice);
                Print(mConnection, mInterface);
            };
            var Setup = await _visitor.GetWifiAccount(_Name, _Status, _IPAddress);


            txtNet.Text = "Network: " + Setup.WifiName;
            txtNetPassword.Text = "";
            string WIFI_Host = session.GetString("WIFI_Host", "");
            string WIFI_Visitor = session.GetString("WIFI_Visitor", "");

            string sb = "";
            if (_Status == "WIFI")
            {
                sb = "WIFI:S:" + Setup.WifiName + @";;";
                var Message = await _visitor.PostNewWIFI(WIFI_Host, WIFI_Visitor, Setup.Username, Setup.Password, session.GetString(nameof(gVarEnum.Name), ""));
                txtNetPassword.Text = "Username: " + Setup.Username + "\nPassword:" + Setup.Password;
            }
            else
            {
                txtNetPassword.Text = "Password:" + Setup.Password;
                sb = "WIFI:S:" + Setup.WifiName + @";T:WEP;P:" + Setup.Password + ";;";
            }

            BarcodeWriter writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new ZXing.QrCode.QrCodeEncodingOptions
                {
                    Width = 200,
                    Height = 200
                }
            };

            result = writer.Write(sb);
            QRCode.SetImageBitmap(result);
            qrcode = GetTextToPrint(Setup.WifiName, _Name, Setup.Username, Setup.Password);
        }

        void moveActivity()
        {
            if (_Status == "WIFI")
            {
                Finish();
            }
            else
            {

                Intent intent = new Intent(this, typeof(SignInOutActivity));
                intent.SetFlags(ActivityFlags.SingleTop);
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.abc_slide_in_top, Resource.Animation.abc_slide_in_bottom);
                Finish();
            }

        }

        async Task CountDown()
        {
            List<CodLst> CodLs = await _visitor.GetCodLst("WifiRemainingTime");
            Int16 cnt =Convert.ToInt16(CodLs[0].Cod.ToString());
            if (cnt == 0) cnt = 60;

            counting = cnt;
            timer.Interval = 1000;
            timer.Enabled = true;
            timer.Elapsed += (sender, e) =>
            {
                RunOnUiThread(() =>
                {
                    txtSession.Text = "Your Session Will Be Finish In - " + counting.ToString() +"s";
                    if (counting == 0)
                    {
                        timer.Stop();

                        if (_Status == "WIFI")
                        {
                            Finish();
                        }
                        else
                        {

                            Intent intent = new Intent(this, typeof(SignInOutActivity));
                            intent.SetFlags(ActivityFlags.SingleTop);
                            StartActivity(intent);
                            OverridePendingTransition(Resource.Animation.abc_slide_in_top, Resource.Animation.abc_slide_in_bottom);
                            Finish();
                        }
                    }
                    counting--;

                });
            };
            timer.Start();

        }

        protected async Task PostVisitorCheckin()
        {
            var ShimanoBadge = session.GetString(nameof(gVarEnum.CardId), "");
            var Temp_Id = session.GetString(nameof(gVarEnum.Temp_VisitorId), "");
            var NeedLunch = session.GetBoolean(nameof(gVarEnum.Lunch), true);
            var NeedStay = session.GetBoolean(nameof(gVarEnum.Stay), true);
            var PhotoName = session.GetString(nameof(gVarEnum.Photo), "");
            var StayDate = session.GetString(nameof(gVarEnum.StayDate), "");
            if (NeedStay == false)
            {
                StayDate = DateTime.Now.ToString("yyyyMMdd");
            }
            var postData = new ShimanoBadgeModel
            {
                NeedLunch = NeedLunch,
                ShimanoBadge = ShimanoBadge,
                Temp_Visitor = Temp_Id,
                PhotoName = PhotoName,
                NeedStay = NeedStay,
                StayDate = StayDate

            };
            var Message = await _visitor.PostVisitorCheckin(postData);

        }

        private void Print(UsbDeviceConnection connection, UsbInterface usbInterface)
        {
            string test = "";
            testBytes = Encoding.ASCII.GetBytes(test);

            if (usbInterface == null)
            {
                DisplayMessage("INTERFACE IS NULL");

            }
            else if (connection == null)
            {
                DisplayMessage("CONNECTION IS NULL");

            }
            else if (forceCLaim == null)
            {
                DisplayMessage("FORCE CLAIM IS NULL");

            }
            else
            {

                byte[] cut_paper = { 0x1D, 0x56, 0x41, 0x10 };
                Thread thread = new Thread(() =>
                {
                    connection.ClaimInterface(usbInterface, forceCLaim);
                    connection.BulkTransfer(mEndPoint, qrcode, qrcode.Length, 500);
                    connection.BulkTransfer(mEndPoint, cut_paper, cut_paper.Length, 500);
                });

                thread.Start();
            }
        }
        private void DisplayMessage(string message)
        {
            Toast.MakeText(this, message, ToastLength.Long).Show();
        }

        byte[] GetTextToPrint(string ssid, string name, string username, string password)
        {

            var _header = "Welcome To Shimano Batam Manufacture, \n" + name + " !\n";
            _header += "-------------------------------------------\n\n";
            //================================================================

            var _information = "Use this Account Detail \nto Login our Wi-fi\n";
            _information += "Wi-fi Name : " + ssid + "\n";
            if (_Status == "WIFI")
            {
                _information += "Username : " + username + "\n";
            }
            _information += "Password : " + password + "\n\n";
            var _footer = "-------------------------------------------\n\n";
            _footer += "Your Account valid for one day only\n\n";
            _footer += " Please Do not share your Wi-fi \nAccount to others.";

            //==============================
            byte[] command = null;
            try
            {
                Android.Graphics.Bitmap bmp = result;
                if (bmp != null)
                {
                    command = ImageHelper.decodeBitmap(bmp);
                }
                else
                {
                    DisplayMessage("Print Photo error");
                }
            }
            catch (Exception e)
            {
                DisplayMessage("Print Photo error");
            }
            //===================
            var s = new MemoryStream();
            var bHeader = Encoding.ASCII.GetBytes(_header);
            var bInfo = Encoding.ASCII.GetBytes(_information);
            var bFooter = Encoding.ASCII.GetBytes(_footer);
            s.Write(bHeader, 0, bHeader.Length);
            s.Write(bInfo, 0, bInfo.Length);
            //s.Write(PrinterCommand.ESC_ALIGN_CENTER,0, PrinterCommand.ESC_ALIGN_CENTER.Length);

            s.Write(command, 0, command.Length);
            s.Write(bFooter, 0, bFooter.Length);
            var b3 = s.ToArray();

            return b3;
        }

    }
}