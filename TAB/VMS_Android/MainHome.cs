using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Content;
using Android.Nfc;
using Android.Nfc.Tech;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using VMS.Core.Class;
using VMS.Core.Repository;
using VMS.Core.Model;
using System.Threading.Tasks;
using Android.Preferences;
using Android.Content.PM;
using Android.Net.Wifi;
using Android.Graphics;
using ZXing;
using ZXing.Common;
using System.Timers;
using Newtonsoft.Json;
using Java.Net;
using Java.Math;
using System.Net.Http;
using VMS_Android.Utility;
using Android.Hardware.Usb;
using System.Linq;
using System.IO;
using ZXing.Mobile;
using static Android.App.ActionBar;



namespace VMS_Android
{
    [Activity(Label = "VisiBukku", MainLauncher = true, Theme = "@style/AppTheme", Icon = "@drawable/VMS_Logo")]
    public class MainHome : Activity
    {
        ISharedPreferences session;
        ISharedPreferencesEditor SessionEdit;
        VisitorRepository _visitor = new VisitorRepository();
        public const string ViewApeMimeType = "application/vnd.xamarin.VMS_Android";
        public static readonly string NfcAppRecord = "xamarin.VMS_Android";
        public static readonly string Tag = "VMS_Android";
        string ShimanoBadge = "";
        public ImageView ShimanoLogo;
        public NfcAdapter _nfcAdapter;
        public Button FindName, btnTempWifi, btnPrintWifi, btnDismissWifi, Tittle_PopUp, btnDismissSignin;
        TextView txtVersion, timeleft, txtChk1, txtChk2, txtChk3, txtChk4;
        TextView SSID_NOTE_LBL, SSID_LBL, PSWD_LBL, EXPD_LBL, USID_LBL;
        LinearLayout LinearLayoutMainContent, LinearLayoutDownload, LinearLayoutQrContent, LinearLayoutCheck1, LinearLayoutWifi;
        string _QRCode, A, B, C, D, E, F;
        RelativeLayout  Box_2;
        ImageView downloadlogo1, downloadlogo2, QrContent, QrContentWifi;
        BitMatrix bitmapMatrix = null;
        //string API_URL = "http://172.18.100.83:81/POCKETPAL-API";
        string API_URL = "https://sbm-apps.shimano.id/POCKETPAL-API";
        //
        //string API_URL = "https://sbm-apps.dev/POCKETPAL-API";
        System.Timers.Timer stopwatch1, stopwatch2, stopwatch3, stopwatchDownload;
        int seconds, seconds2, seconds3;
        ProgressDialog progress;
        private Button btnshowPopup;
        private Button btnPopupCancel;
        private Button btnSubmit;
        private Dialog popupDialog;
        private EditText InputValue;

        //Printer
        public UsbManager mUsbManager;
        public UsbDevice mDevice;
        public static UsbDeviceConnection mConnection;
        public static UsbInterface mInterface;
        public static UsbEndpoint mEndPoint;
        public static byte[] qrcode;
        public static Bitmap result;
        public PendingIntent mPermissionIntent;


        public static string ACTION_USB_PERMISSION = "com.android.example.USB_PERMISSION";
        public static bool forceCLaim = true;
        IDictionary<string, UsbDevice> mDeviceList;
        IEnumerator<UsbDevice> mDeviceIterator;
        byte[] testBytes;

        string P_SSID, P_USERNAME, P_PASSWORD, P_EXPIREON;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.Main_Home);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            _nfcAdapter = NfcAdapter.GetDefaultAdapter(this);
            LinearLayoutMainContent = FindViewById<LinearLayout>(Resource.Id.LinearLayoutMainContent);
            LinearLayoutDownload = FindViewById<LinearLayout>(Resource.Id.LinearLayoutDownload);
            LinearLayoutQrContent = FindViewById<LinearLayout>(Resource.Id.LinearLayoutQrContent);
            LinearLayoutCheck1 = FindViewById<LinearLayout>(Resource.Id.LinearLayoutCheck1);
            LinearLayoutWifi = FindViewById<LinearLayout>(Resource.Id.LinearLayoutWifi);
            txtChk1 = FindViewById<TextView>(Resource.Id.txtChk1);
            txtChk2 = FindViewById<TextView>(Resource.Id.txtChk2);
            txtChk3 = FindViewById<TextView>(Resource.Id.txtChk3);
            txtChk4 = FindViewById<TextView>(Resource.Id.txtChk4);
            SSID_NOTE_LBL = FindViewById<TextView>(Resource.Id.SSID_NOTE_LBL);
            SSID_LBL = FindViewById<TextView>(Resource.Id.SSID_LBL);
            PSWD_LBL = FindViewById<TextView>(Resource.Id.PSWD_LBL);
            EXPD_LBL = FindViewById<TextView>(Resource.Id.EXPD_LBL);
            USID_LBL = FindViewById<TextView>(Resource.Id.USID_LBL);
            QrContent = FindViewById<ImageView>(Resource.Id.QrContent);
            QrContentWifi = FindViewById<ImageView>(Resource.Id.QrContentWifi);
            timeleft = FindViewById<TextView>(Resource.Id.timeleft);
            Box_2 = FindViewById<RelativeLayout>(Resource.Id.Box_2);
            downloadlogo1 = FindViewById<ImageView>(Resource.Id.downloadlogo1);
            downloadlogo2 = FindViewById<ImageView>(Resource.Id.downloadlogo2);
            btnTempWifi = FindViewById<Button>(Resource.Id.btnTempWifi);
            btnDismissSignin = FindViewById<Button>(Resource.Id.btnDismissSignin);
            btnPrintWifi = FindViewById<Button>(Resource.Id.btnPrintWifi);
            btnDismissWifi = FindViewById<Button>(Resource.Id.btnDismissWifi);
            Tittle_PopUp = FindViewById<Button>(Resource.Id.Tittle_PopUp);
            popupDialog = new Dialog(this);
            popupDialog.SetContentView(Resource.Layout.PopUpCustom);
            popupDialog.Window.SetSoftInputMode(SoftInput.AdjustResize);
            
            popupDialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, LayoutParams.WrapContent);
            popupDialog.Window.SetBackgroundDrawableResource(Android.Resource.Color.Transparent);
            btnPopupCancel = popupDialog.FindViewById<Button>(Resource.Id.btnCancel);
            btnSubmit = popupDialog.FindViewById<Button>(Resource.Id.btnSubmit);
            InputValue = popupDialog.FindViewById<EditText>(Resource.Id.InputValue);
            btnPopupCancel.Click += BtnPopupCancel_Click;
            btnSubmit.Click += BtnSubmit_Click;
            btnPrintWifi.Click += btnPrintWifi_Click;
            btnDismissWifi.Click += btnDismissWifi_Click;
            btnDismissSignin.Click += BtnDismissSignin_Click;
            btnTempWifi.Click += btnTempWifi_Click;
            downloadlogo1.Click += Downloadlogo1_Click;
            downloadlogo2.Click += Downloadlogo2_Click;
            Box_2.Click += Box_2_Click;
            LinearLayoutMainContent.Visibility = ViewStates.Visible;
            LinearLayoutDownload.Visibility = ViewStates.Invisible;
            LinearLayoutCheck1.Visibility = ViewStates.Invisible;
            LinearLayoutWifi.Visibility = ViewStates.Invisible;
            FindViews();
            SetCustomFont();
            PrinterSetup();
        }

        private async void BtnDismissSignin_Click(object sender, EventArgs e)
        {
            Reset();
            await DeleteQrCode();
        }

        public void btnDismissWifi_Click(object sender, EventArgs e)
        {
            LinearLayoutMainContent.Visibility = ViewStates.Visible;
            LinearLayoutDownload.Visibility = ViewStates.Invisible;
            LinearLayoutCheck1.Visibility = ViewStates.Invisible;
            LinearLayoutQrContent.Visibility = ViewStates.Invisible;
            LinearLayoutWifi.Visibility = ViewStates.Invisible;
        }

        public void btnPrintWifi_Click(object sender, EventArgs e)
        {
            DisplayMessage("Printing...");
            qrcode = GetTextToPrint(P_SSID, P_USERNAME, P_PASSWORD, P_EXPIREON);
            mConnection = mUsbManager.OpenDevice(mDevice);
            Print(mConnection, mInterface);
            LinearLayoutMainContent.Visibility = ViewStates.Visible;
            LinearLayoutDownload.Visibility = ViewStates.Invisible;
            LinearLayoutCheck1.Visibility = ViewStates.Invisible;
            LinearLayoutQrContent.Visibility = ViewStates.Invisible;
            LinearLayoutWifi.Visibility = ViewStates.Invisible;
        }

        [Obsolete]
        public void LoadingShow()
        {
            progress = new ProgressDialog(this);
            progress.Indeterminate = true;
            progress.SetProgressStyle(ProgressDialogStyle.Spinner);
            progress.SetMessage("Loading...");
            progress.SetCancelable(false);
            progress.Show();
        }

        public void LoadingDissmis()
        {
            progress.Dismiss();
        }

        public void btnTempWifi_Click(object sender, EventArgs e)
        {
            try
            {
                Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                alert.SetTitle("WIFI Profiles");
                alert.SetMessage("*NOTE*\n\n Temporary:\nOnly available for download Pocket Pal from Google Play Store and Apple Store.\n\nWorking Hour:\nThis open wifi availabel for 8 working hours.");
                alert.SetPositiveButton("Working Hour", (senderAlert, args) =>
                {
                    try
                    {
                        popupDialog.Show();
                    }
                    catch (Exception ex)
                    {
                        ShowError(ex.Message.ToString());
                    }
                });
                alert.SetNegativeButton("Temporary", (senderAlert, args) =>
                {
                    TemporaryWifi();
                });
                Dialog dialog = alert.Create();
                dialog.Show();
            }
            catch(Exception ex)
            {
                ShowError(ex.Message.ToString());
                LoadingDissmis();
            }
        }

        public async void BtnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                LoadingShow();
                string url = "" + API_URL + "/VisitorTS/PP_GET_PASSWORD_RECEPTIONIST?PASSWORD=" + InputValue.Text.ToString() + "";
                HttpResponseMessage response = new HttpResponseMessage();
                HttpClient client = new HttpClient();
                response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Root_SSID Root = JsonConvert.DeserializeObject<Root_SSID>(content);
                    if (Root.Success == true)
                    {
                        LoadingDissmis();
                        popupDialog.Dismiss();
                        WorkingHourWifi();
                        InputValue.Text = "";
                    }
                }
                else
                {
                    ShowError(response.StatusCode.ToString());
                }
                LoadingDissmis();
            }
            catch(Exception ex)
            {
                ShowError(ex.Message.ToString());
                LoadingDissmis();
            }
        }

        public void BtnPopupCancel_Click(object sender, EventArgs e)
        {
            popupDialog.Hide();
        }

        public async void WorkingHourWifi()
        {
            try
            {
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var stringChars = new char[3];
                var random = new Random();
                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }
                var finalString = new String(stringChars);
                string url = "" + API_URL + "/VisitorTS/LoginArubaTemp?NAME=USER-TABLET&EMAIL=MAIL-" + finalString + "@shimano.com.sg&BUSFUNC=SYSTEM-ADMIN&TYPE=3&CUSTOMHOUR=0";
                HttpResponseMessage response = new HttpResponseMessage();
                HttpClient client = new HttpClient();
                response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Root_SSID Root = JsonConvert.DeserializeObject<Root_SSID>(content);
                    if (Root.Success == true)
                    {
                        LoadingDissmis();
                        P_SSID = "SBM-GUEST";
                        P_USERNAME = Root.Data.userdb_add.name;
                        P_PASSWORD = Root.Data.userdb_add.passwd;
                        P_EXPIREON = (Root.Data.userdb_add.expday + ", " + Root.Data.userdb_add.exptime);
                        ShowWifi(P_SSID, P_USERNAME, P_PASSWORD, P_EXPIREON);
                    }
                    else
                    {
                        LoadingDissmis();
                        ShowWifi("SBM-GUEST", "", "", "");
                    }
                }
                else
                {
                    LoadingDissmis();
                    ShowError(response.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message.ToString());
            }
        }
        public async void WorkingHourWifiwithPrint()
        {
            try
            {
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                var stringChars = new char[3];
                var random = new Random();
                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }
                var finalString = new String(stringChars);
                string url = "" + API_URL + "/VisitorTS/LoginArubaTemp?NAME=USER-TABLET&EMAIL=MAIL-" + finalString + "@shimano.com.sg&BUSFUNC=SYSTEM-ADMIN&TYPE=3&CUSTOMHOUR=0";
                HttpResponseMessage response = new HttpResponseMessage();
                HttpClient client = new HttpClient();
                response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Root_SSID Root = JsonConvert.DeserializeObject<Root_SSID>(content);
                    if (Root.Success == true)
                    {
                        LoadingDissmis();
                        P_SSID = "SBM-GUEST";
                        P_USERNAME = Root.Data.userdb_add.name;
                        P_PASSWORD = Root.Data.userdb_add.passwd;
                        P_EXPIREON = (Root.Data.userdb_add.expday + ", " + Root.Data.userdb_add.exptime);
                        ShowWifi(P_SSID, P_USERNAME, P_PASSWORD, P_EXPIREON);

                        DisplayMessage("Printing...");
                        qrcode = GetTextToPrint(P_SSID, P_USERNAME, P_PASSWORD, P_EXPIREON);
                        mConnection = mUsbManager.OpenDevice(mDevice);
                        Print(mConnection, mInterface);
                        LinearLayoutMainContent.Visibility = ViewStates.Visible;
                        LinearLayoutDownload.Visibility = ViewStates.Invisible;
                        LinearLayoutCheck1.Visibility = ViewStates.Invisible;
                        LinearLayoutQrContent.Visibility = ViewStates.Invisible;
                        LinearLayoutWifi.Visibility = ViewStates.Invisible;
                    }
                    else
                    {
                        LoadingDissmis();
                        ShowWifi("SBM-GUEST", "", "", "");
                    }
                }
                else
                {
                    LoadingDissmis();
                    ShowError(response.StatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message.ToString());
            }
        }
        public void TemporaryWifi()
        {
            ShowWifi_Open();
        }

        public async void Box_2_Click(object sender, EventArgs e)
        {
            await GetQrCode();
        }

        public async Task GetQrCode()
        {
            try
            {
                LoadingShow();
                string url = API_URL + "/VisitorTS/PP_POST_QR_VMS";
                HttpResponseMessage response = new HttpResponseMessage();
                HttpClient client = new HttpClient();
                response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    RespondApi models = JsonConvert.DeserializeObject<RespondApi>(content);
                    if (models.Success == true)
                    {
                        CheckIn(models.data.QRCode);
                    }
                    else
                    {
                        ShowError(models.Message.ToString());
                    }
                    LoadingDissmis();
                }
                else
                {
                    ShowError(response.RequestMessage.ToString());
                    LoadingDissmis();
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message.ToString());
                LoadingDissmis();
            }
        }

        public async Task DeleteQrCode()
        {
            try
            {
                LoadingShow();
                string url = API_URL + "/VisitorTS/PP_DELETE_QR_VMS";
                HttpResponseMessage response = new HttpResponseMessage();
                HttpClient client = new HttpClient();
                response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    ShowError(response.StatusCode.ToString());
                }
                LoadingDissmis();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message.ToString());
                LoadingDissmis();
            }
        }

        public async void CheckIn(string QRCODE)
        {
            try
            {
                LinearLayoutMainContent.Visibility = ViewStates.Invisible;
                LinearLayoutDownload.Visibility = ViewStates.Visible;
                LinearLayoutQrContent.Visibility = ViewStates.Visible;
                LinearLayoutCheck1.Visibility = ViewStates.Invisible;
                LinearLayoutWifi.Visibility = ViewStates.Invisible;
                int size = QrContent.Width;
                bitmapMatrix = new MultiFormatWriter().encode(QRCODE, BarcodeFormat.QR_CODE, size, size);
                var w = bitmapMatrix.Width;
                var h = bitmapMatrix.Height;
                int[] pixelsImage = new int[w * h];
                for (int i = 0; i < h; i++)
                {
                    for (int j = 0; j < w; j++)
                    {
                        if (bitmapMatrix[j, i])
                            pixelsImage[i * w + j] = (int)Convert.ToInt64(0xff000000);
                        else
                            pixelsImage[i * w + j] = (int)Convert.ToInt64(0xffffffff);
                    }
                }
                Bitmap bitmap = Bitmap.CreateBitmap(w, h, Bitmap.Config.Argb8888);
                bitmap.SetPixels(pixelsImage, 0, w, 0, 0, w, h);
                QrContent.SetImageBitmap(bitmap);
                _QRCode = QRCODE;
                QRCODE = null;
                downloadlogo1.Enabled = false;
                downloadlogo2.Enabled = false;
                stopwatch1 = new Timer();
                stopwatch2 = new Timer();
                stopwatch1.Interval = 1000;
                stopwatch2.Interval = 5000;
                seconds = 30;
                seconds2 = 30;
                stopwatch1.Elapsed += Stopwatch_Elapsed1;
                //stopwatch1.Start();
                stopwatch2.Elapsed += Stopwatch_Elapsed2;
                stopwatch2.Start();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message.ToString());
            }
        }

        public async Task GetLoginInfo()
        {
            try
            {
                //LoadingShow();
                //string url = API_URL + "/VisitorTS/PP_GET_LOGIN_INFO_VMS?QRCode=QR-VMST7G3YK9LHJCZNGNU";
                //QR-VMST7G3YK9LHJCZNGNU
                string url = API_URL + "/VisitorTS/PP_GET_LOGIN_INFO_VMS?QRCode="+ _QRCode + "";
                HttpResponseMessage response = new HttpResponseMessage();
                HttpClient client = new HttpClient();
                response = await client.GetAsync(url);
                //seconds2--;
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    RespondApi2 models = JsonConvert.DeserializeObject<RespondApi2>(content);
                    if(models.Success == true)
                    {
                        E = models.data.UseNam;
                        if (models.data.UseNam != "-")
                        {
                            stopwatch1.Stop();
                            stopwatch2.Stop();
                            seconds = 0;
                            seconds2 = 0;
                            RunOnUiThread(() =>
                            {
                                if (Convert.ToString(seconds) == "0")
                                {
                                    Reset();
                                    CheckStatus();
                                    txtChk1.Text = "Welcome,";
                                    txtChk2.Text = E;
                                    txtChk3.Text = "";
                                    txtChk4.Text = "Close in - 3s";
                                    stopwatch3 = new Timer();
                                    stopwatch3.Interval = 1000;
                                    seconds3 = 5;
                                    stopwatch3.Elapsed += Stopwatch_Elapsed3;
                                    stopwatch3.Start();
                                }
                            });
                            LoadingDissmis();
                        }
                        LoadingDissmis();
                    }
                    else
                    {
                        LoadingDissmis();
                        ShowError(models.Message.ToString());
                    }
                }
                else
                {
                    ShowError(response.StatusCode.ToString());
                    LoadingDissmis();
                }
            }
            catch(Exception ex)
            {
                //ShowError(ex.Message.ToString());
                LoadingDissmis();
            }
        }

        public void Downloadlogo1_Click(object sender, EventArgs e)
        {
            try
            {
                //LinearLayoutMainContent.Visibility = ViewStates.Invisible;
                //LinearLayoutDownload.Visibility = ViewStates.Visible;
                //LinearLayoutCheck1.Visibility = ViewStates.Invisible;
                //LinearLayoutQrContent.Visibility = ViewStates.Visible;
                //LinearLayoutWifi.Visibility = ViewStates.Invisible;
                //int size = QrContent.Width;
                string link = "https://play.google.com/store/apps/details?id=com.shimano.pocketpal";
                //bitmapMatrix = new MultiFormatWriter().encode(link, BarcodeFormat.QR_CODE, size, size);
                //var w = bitmapMatrix.Width;
                //var h = bitmapMatrix.Height;
                //int[] pixelsImage = new int[w * h];
                //for (int i = 0; i < h; i++)
                //{
                //    for (int j = 0; j < h; j++)
                //    {
                //        if (bitmapMatrix[j, i])
                //            pixelsImage[i * w + j] = (int)Convert.ToInt64(0xff000000);
                //        else
                //            pixelsImage[i * w + j] = (int)Convert.ToInt64(0xffffffff);

                //    }
                //}
                //Bitmap bitmap = Bitmap.CreateBitmap(w, h, Bitmap.Config.Argb8888);
                //bitmap.SetPixels(pixelsImage, 0, w, 0, 0, w, h);
                //QrContent.SetImageBitmap(bitmap);
                //downloadlogo1.Enabled = false;
                //downloadlogo2.Enabled = false;
                //stopwatchDownload = new System.Timers.Timer();
                //stopwatchDownload.Interval = 1000;
                //seconds = 6;
                //stopwatchDownload.Elapsed += Stopwatch_ElapsedDownload;
                //stopwatchDownload.Start();

                ShowDownload(link, true);
            }
            catch(Exception ex)
            {
                ShowError(ex.Message.ToString());
            }
        }

        public void Downloadlogo2_Click(object sender, EventArgs e)
        {
            try
            {
                //LinearLayoutMainContent.Visibility = ViewStates.Invisible;
                //LinearLayoutDownload.Visibility = ViewStates.Visible;
                //LinearLayoutCheck1.Visibility = ViewStates.Invisible;
                //LinearLayoutQrContent.Visibility = ViewStates.Visible;
                //LinearLayoutWifi.Visibility = ViewStates.Invisible;
                //int size = QrContent.Width;
                string link = "https://apps.apple.com/nz/app/pocketpal/id1538263296";
                //bitmapMatrix = new MultiFormatWriter().encode(link, BarcodeFormat.QR_CODE, size, size);
                //var w = bitmapMatrix.Width;
                //var h = bitmapMatrix.Height;
                //int[] pixelsImage = new int[w * h];

                //for (int i = 0; i < h; i++)
                //{
                //    for (int j = 0; j < h; j++)
                //    {
                //        if (bitmapMatrix[j, i])
                //            pixelsImage[i * w + j] = (int)Convert.ToInt64(0xff000000);
                //        else
                //            pixelsImage[i * w + j] = (int)Convert.ToInt64(0xffffffff);

                //    }
                //}
                //Bitmap bitmap = Bitmap.CreateBitmap(w, h, Bitmap.Config.Argb8888);
                //bitmap.SetPixels(pixelsImage, 0, w, 0, 0, w, h);
                //QrContent.SetImageBitmap(bitmap);
                //downloadlogo1.Enabled = false;
                //downloadlogo2.Enabled = false;
                //stopwatchDownload = new Timer();
                //stopwatchDownload.Interval = 1000;
                //seconds = 6;
                //stopwatchDownload.Elapsed += Stopwatch_ElapsedDownload;
                //stopwatchDownload.Start();
                ShowDownload(link, false);
            }
            catch(Exception ex)
            {
                ShowError(ex.Message.ToString());
            }
        }

        public void Reset()
        {
            LinearLayoutMainContent.Visibility = ViewStates.Visible;
            LinearLayoutDownload.Visibility = ViewStates.Invisible;
            LinearLayoutCheck1.Visibility = ViewStates.Invisible;
            LinearLayoutQrContent.Visibility = ViewStates.Invisible;
            LinearLayoutWifi.Visibility = ViewStates.Invisible;
            downloadlogo1.Enabled = true;
            downloadlogo2.Enabled = true;
        }

        public void CheckStatus()
        {
            LinearLayoutMainContent.Visibility = ViewStates.Invisible;
            LinearLayoutDownload.Visibility = ViewStates.Invisible;
            LinearLayoutCheck1.Visibility = ViewStates.Visible;
            LinearLayoutQrContent.Visibility = ViewStates.Invisible;
            LinearLayoutWifi.Visibility = ViewStates.Invisible;
            downloadlogo1.Enabled = false;
            downloadlogo2.Enabled = false;
        }

        public void Stopwatch_Elapsed1(object sender, ElapsedEventArgs e)
        {
            seconds--;
            if (seconds == 0)
            {
                stopwatch1.Stop();
                seconds = 0;
            }
            RunOnUiThread(async () =>
            {
                timeleft.Text = "session will be finished in " + Convert.ToString(seconds) + "s";
                if (seconds == 0)
                {
                    Reset();
                    await DeleteQrCode();
                }
            });
        }

        public async void Stopwatch_Elapsed2(object sender, ElapsedEventArgs e)
        {
            if (seconds == 0)
            {
                stopwatch2.Stop();
                seconds2 = 0;
            }
            else
            {
                await GetLoginInfo();
            }
        }

        public void Stopwatch_Elapsed3(object sender, ElapsedEventArgs e)
        {
            seconds3--;
            if (seconds3 == 0)
            {
                stopwatch3.Stop();
                seconds3 = 0;
            }
            RunOnUiThread(() =>
            {
                txtChk4.Text = "session will be finished in " + Convert.ToString(seconds3) + "s";
                if (seconds3 == 0)
                {
                    Reset();
                    WorkingHourWifiwithPrint();
                }
            });
        }

        public void Stopwatch_ElapsedDownload(object sender, ElapsedEventArgs e)
        {
            seconds--;
            if (seconds == 0)
            {
                stopwatchDownload.Stop();
                seconds = 0;
            }
            RunOnUiThread(() =>
            {
                timeleft.Text = "Your session will be finished in " + Convert.ToString(seconds) + "s";
                if (Convert.ToString(seconds) == "0")
                {
                    Reset();
                }
            });
        }

        public void SetCustomFont()
        {
            var tv1 = FindViewById<TextView>(Resource.Id.ShimanoWelcome);
            var tv2 = FindViewById<TextView>(Resource.Id.tit1e_1);
            var tv3 = FindViewById<TextView>(Resource.Id.subTitle_1);
            var tv4 = FindViewById<TextView>(Resource.Id.tit1e_2);
            var tv5 = FindViewById<TextView>(Resource.Id.subTitle_2);
            var tv6 = FindViewById<TextClock>(Resource.Id.digitalClock);
            var tv7 = FindViewById<Button>(Resource.Id.btnSearch);
            var tv8 = FindViewById<TextView>(Resource.Id.tit1e_3);
            var tv9 = FindViewById<TextView>(Resource.Id.subTitle_3);
            txtVersion = FindViewById<TextView>(Resource.Id.txtVer);
            tv1.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv2.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv3.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv4.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv5.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv6.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv7.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv8.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv9.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            txtVersion.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");

            var context = Application.Context;
            var VersionName = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.MetaData).VersionName;
            txtVersion.Text = "Build. " + VersionName.ToString();
        }

        protected override void OnNewIntent(Intent intent)
        {
            var tag = intent.GetParcelableExtra(NfcAdapter.ExtraTag) as Tag;
            if (tag == null)
            {
                return;
            }
            Task task = TryRead(tag);
        }

        protected override void OnResume()
        {
            base.OnResume();
            EnableReadMode();
        }

        protected override void OnPause()
        {
            base.OnPause();
            // App is paused, so no need to keep an eye out for NFC tags.
            if (_nfcAdapter != null)
                _nfcAdapter.DisableForegroundDispatch(this);
        }

        public void DisplayMessage(string message)
        {
            Toast.MakeText(this, message, ToastLength.Long).Show();
            Log.Info(Tag, message);
        }
        public void EnableReadMode()
        {
            var tagDetected = new IntentFilter(NfcAdapter.ActionTagDiscovered);
            var filters = new[] { tagDetected };
            var intent = new Intent(this, GetType()).AddFlags(ActivityFlags.SingleTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);

            if (_nfcAdapter == null)
            {
                var alert = new Android.App.AlertDialog.Builder(this).Create();
                alert.SetMessage("NFC is not supported on this device.");
                alert.SetTitle("NFC Unavailable");
                alert.SetButton("OK", delegate {
                DisplayMessage("NFC is not supported on this device.");
                });
                alert.Show();
            }
            else
                _nfcAdapter.EnableForegroundDispatch(this, pendingIntent, filters, null);
        } 

        public async Task TryRead(Tag tag)
        {
            Toast.MakeText(this, "Reading... please don't remove...", ToastLength.Short);
            List<CodLst> getAPK_Path;
            try
            {
                getAPK_Path = await _visitor.GetCodLst("GenerateTSDate");
            }
            catch (Exception ex)
            {
                DisplayMessage("" + ex.Message.ToString() + ", Can not connect to server, Please check your network connection.");
                return;
            }
            
            DateTime dateTime = Convert.ToDateTime(getAPK_Path[0].Cod.ToString());
            if (dateTime.ToShortDateString() != DateTime.Now.ToShortDateString())
            {
                DisplayMessage("WIFI for TeamShimano not yet generated \nPlease call your IT to run PowerShell.");
                return;
            }

            VisitorJoinLog visitorJoinLog = new VisitorJoinLog {
                Department="NONAME",
                Name="NONAME",
                Status= nameof(SttVisitor.PENDING),
                Temp_VisitorId= "0",
                VisitorId="0"
            };

            var ndef = Ndef.Get(tag);
            if (ndef != null)
            {
                try
                {
                    ndef.Connect();
                }
                catch (Exception ex)
                {
                    DisplayMessage(""+ ex.Message.ToString() + ", Please do not remove your card before the system has finished reading.");
                    throw;
                }
                
                if (ndef.NdefMessage != null)
                {
                    var record = ndef.NdefMessage.GetRecords()[0];
                    ShimanoBadge = Encoding.ASCII.GetString(record.GetPayload());
                    var Employee = await _visitor.GetVisitor_TSByBadge(ShimanoBadge.Replace("en", "").Trim());
                    if (Employee != null)
                    {
                        SetActivity(Employee);
                    }
                    else
                    {
                        DisplayMessage("Cannot Connect to Server, Please Try Again!");
                    }
                }
                else
                {
                    SetActivity(visitorJoinLog);
                }
            }
            var ndef_formatable = NdefFormatable.Get(tag);
            if (ndef_formatable != null)
            {
                var payload = Encoding.ASCII.GetBytes("");
                var mimeBytes = Encoding.ASCII.GetBytes(ViewApeMimeType);
                var apeRecord = new NdefRecord(NdefRecord.TnfMimeMedia, mimeBytes, new byte[0], payload);
                var ndefMessage = new NdefMessage(new[] { apeRecord });
                try
                {
                    ndef_formatable.Connect();
                    ndef_formatable.Format(ndefMessage);
                    SetActivity(visitorJoinLog);
                }
                catch (Java.IO.IOException ioex)
                {
                    var msg = "There was an error trying to format the tag.";
                    DisplayMessage(msg);
                    Log.Error(Tag, ioex, msg);
                }
            }
        }
        public void SetActivity(VisitorJoinLog Employee)
        {
            string IPAddress = "";
            try
            {
                WifiManager manager = (WifiManager)GetSystemService(Context.WifiService);
                WifiInfo info = manager.ConnectionInfo;
                int ipAddressInt = manager.DhcpInfo.Netmask;
                byte[] ipAddress = BigInteger.ValueOf(ipAddressInt).ToByteArray();
                InetAddress myaddr = InetAddress.GetByAddress(ipAddress);
                String hostaddr = myaddr.GetAddress().ToString();
                IPAddress = hostaddr;
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, ex.Message.ToString(), ToastLength.Long);
                return;
            }

            session = PreferenceManager.GetDefaultSharedPreferences(this);
            SessionEdit = session.Edit();
            SessionEdit.Clear(); 
            SessionEdit.PutString(nameof(gVarEnum.Id), Employee.VisitorId);
            SessionEdit.PutString(nameof(gVarEnum.Status), Employee.Status);
            SessionEdit.PutString(nameof(gVarEnum.Temp_VisitorId), Employee.Temp_VisitorId);
            SessionEdit.PutString(nameof(gVarEnum.Name), Employee.Name);
            SessionEdit.PutString(nameof(gVarEnum.CardId), ShimanoBadge);
            SessionEdit.PutString(nameof(gVarEnum.StayDate), Employee.DateOfEnd);
            SessionEdit.PutBoolean(nameof(gVarEnum.Stay), Employee.NeedStay);
            SessionEdit.PutString("IPAddress", IPAddress);
            SessionEdit.Apply();
            Intent intent =
                new Intent(this, Employee.Status.ToUpper() == "WIFI" ? typeof(RegisterWifi) :
                Employee.Status.ToUpper() != nameof(SttVisitor.PENDING)
                ? typeof(SignInOutActivity) :
                Employee.Temp_VisitorId != "0" || Employee.VisitorId != "0" ? typeof(LunchActivity) : //typeof(WifiActivity) : 
                typeof(TeamShimanoListActivity));
            
            intent.SetFlags(ActivityFlags.SingleTop);
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.abc_slide_in_top, Resource.Animation.abc_slide_in_bottom);

        }
        void SetActivityAdmin(string User)
        {
            Intent intent =
                new Intent(this,typeof(TeamShimanoListActivity));
            intent.SetFlags(ActivityFlags.SingleTop);
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.abc_slide_in_top, Resource.Animation.abc_slide_in_bottom);

        }
        public void FindViews()
        {
            ShimanoLogo = FindViewById<ImageView>(Resource.Id.VMSLogo);
            VisitorJoinLog visitorJoinLog = new VisitorJoinLog
            {
                Department = "NONAME",
                Name = "NONAME",
                Status = nameof(SttVisitor.PENDING),
                Temp_VisitorId = "0",
                VisitorId = "0"
            };
            FindName = FindViewById<Button>(Resource.Id.btnSearch);
            FindName.Click += (sender, e) =>
            {
                SetActivity(visitorJoinLog);
            };
            ShimanoLogo.Click += async (sender, e) =>
            {
                var context = Application.Context;
                var VersionNumber = context.PackageManager.GetPackageInfo(context.PackageName, PackageInfoFlags.MetaData).VersionName;
                List<CodLst> getAndroidVersion = await _visitor.GetCodLst("AndroidVersion");
                List<CodLst> getAPK_Path = await _visitor.GetCodLst("APK_Path");

                DisplayMessage("Database version : " + getAndroidVersion[0].Cod.ToString() + " & APK Version : " + VersionNumber);

                if (getAndroidVersion[0].Cod.ToString() != VersionNumber)
                {
                    Android.App.AlertDialog.Builder dialog = new Android.App.AlertDialog.Builder(this);
                    Android.App.AlertDialog alert = dialog.Create();
                    alert.SetTitle("Shimano");
                    alert.SetMessage("Current APK version is absolute, Database version: " + getAndroidVersion[0].Cod.ToString() + " & APK Version: " + VersionNumber + ", please update your version");
                    alert.SetButton("Download APK", (c, ev) =>
                    {
                        Intent goToMarket = new Intent(Intent.ActionView).SetData(Android.Net.Uri.Parse(getAPK_Path[0].Cod.ToString()));
                        StartActivity(goToMarket);

                        System.Environment.Exit(0);
                    });
                    alert.Show();


                    return;
                }
            };
        }
        public void ShowError(string Message)
        {
            var alert = new Android.App.AlertDialog.Builder(this).Create();
            alert.SetMessage(Message);
            alert.SetTitle("Error:");
            alert.SetButton("OK", delegate {
                DisplayMessage(Message);
            });
            alert.Show();
        }
        public void ShowWifi(string SSID, string USERNAME, string PASSWORD, string EXPIRE)
        {
            try
            {
                LoadingDissmis();
                popupDialog.Dismiss();
                InputValue.Text = "";
                LinearLayoutMainContent.Visibility = ViewStates.Invisible;
                LinearLayoutDownload.Visibility = ViewStates.Invisible;
                LinearLayoutQrContent.Visibility = ViewStates.Invisible;
                LinearLayoutCheck1.Visibility = ViewStates.Invisible;
                LinearLayoutWifi.Visibility = ViewStates.Visible;
                btnPrintWifi.Visibility = ViewStates.Visible;
                BarcodeWriter writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new ZXing.QrCode.QrCodeEncodingOptions
                    {
                        Width = 200,
                        Height = 200,
                    }
                };
                result = writer.Write("WIFI:S:SBM-GUEST;T:nopass;P:;;");
                QrContentWifi.SetImageBitmap(result); 
                SSID_NOTE_LBL.Text = "Scan QR above to join SSID or try to find manually 'SBM-GUEST' on your phone setting and then enter credentials below.";
                SSID_LBL.Text = "SSID: " + SSID;
                USID_LBL.Text = "USERNAME: " + USERNAME;
                PSWD_LBL.Text = "PASSWORD: " + PASSWORD;
                EXPD_LBL.Text = "EXPIRE ON: " + EXPIRE;
                Tittle_PopUp.Text = "S H I M A N O   S S I D";
                
            }
            catch(Exception ex)
            {
                LoadingDissmis();
                ShowError(ex.Message.ToString());
            }
        }
        public void ShowWifi_Open()
        {
            try
            {
                LinearLayoutMainContent.Visibility = ViewStates.Invisible;
                LinearLayoutDownload.Visibility = ViewStates.Invisible;
                LinearLayoutQrContent.Visibility = ViewStates.Invisible;
                LinearLayoutCheck1.Visibility = ViewStates.Invisible;
                LinearLayoutWifi.Visibility = ViewStates.Visible;
                btnPrintWifi.Visibility = ViewStates.Visible;
                BarcodeWriter writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new ZXing.QrCode.QrCodeEncodingOptions
                    {
                        Width = 200,
                        Height = 200,
                    }
                };
                result = writer.Write("WIFI:S:pocketpal;T:nopass;P:;H:true;");
                QrContentWifi.SetImageBitmap(result);
                SSID_NOTE_LBL.Text = "Scan QR above to join SSID or try to find manually 'POCKETPAL' on your phone setting then connect your phone.";
                SSID_LBL.Text = "SSID: POCKETPAL";
                USID_LBL.Text = "USERNAME : -";
                PSWD_LBL.Text = "PASSWORD : -";
                EXPD_LBL.Text = "EXPIRE ON: -";
                Tittle_PopUp.Text = "S H I M A N O   S S I D";
            }
            catch (Exception ex)
            {
                ShowError(ex.Message.ToString());
            }
        }
        public void ShowDownload(string URL, bool type)
        {
            try
            {
                LinearLayoutMainContent.Visibility = ViewStates.Invisible;
                LinearLayoutDownload.Visibility = ViewStates.Visible;
                LinearLayoutQrContent.Visibility = ViewStates.Invisible;
                LinearLayoutCheck1.Visibility = ViewStates.Invisible;
                LinearLayoutWifi.Visibility = ViewStates.Visible;
                btnPrintWifi.Visibility = ViewStates.Invisible;
                BarcodeWriter writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new ZXing.QrCode.QrCodeEncodingOptions
                    {
                        Width = 200,
                        Height = 200,
                    }
                };
                result = writer.Write(URL);
                QrContentWifi.SetImageBitmap(result);
                if(type == true)
                {
                    SSID_NOTE_LBL.Text = "";
                    SSID_LBL.Text = "OPEN GOOGLE PLAY STORE, FIND APPLICATION NAME 'POCKETPAL'";
                    USID_LBL.Text = "";
                    PSWD_LBL.Text = "";
                    EXPD_LBL.Text = "";
                    Tittle_PopUp.Text = "GOOGLE PLAY STORE";
                }
                else
                {
                    SSID_NOTE_LBL.Text = "";
                    SSID_LBL.Text = "OPEN APPLE APPSTORE, FIND APPLICATION NAME 'POCKETPAL'";
                    USID_LBL.Text = "";
                    PSWD_LBL.Text = "";
                    EXPD_LBL.Text = "";
                    Tittle_PopUp.Text = "APPLE APP STORE";
                }
                
            }
            catch (Exception ex)
            {
                ShowError(ex.Message.ToString());
            }
        }
        public void PrinterSetup()
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
        public void Print(UsbDeviceConnection connection, UsbInterface usbInterface)
        {
            try
            {
                string test = "";
                testBytes = Encoding.ASCII.GetBytes(test);
                if (usbInterface == null)
                {
                    ShowError("INTERFACE IS NULL");

                }
                else if (connection == null)
                {
                    ShowError("CONNECTION IS NULL");

                }
                else if (forceCLaim == false)
                {
                    ShowError("FORCE CLAIM IS FALSE");
                }
                else
                {
                    byte[] cut_paper = { 0x1D, 0x56, 0x41, 0x10 };
                    connection.ClaimInterface(usbInterface, forceCLaim);
                    connection.BulkTransfer(mEndPoint, qrcode, qrcode.Length, 500);
                    connection.BulkTransfer(mEndPoint, cut_paper, cut_paper.Length, 500);
                }
            }
            catch(Exception Ex)
            {
                ShowError(Ex.Message.ToString());
            }
        }
        byte[] GetTextToPrint(string ssid, string username, string password, string expireon)
        {
            try
            {
                var _header ="------------VISITOR MANAGEMENT SYSTEM-----------\n\n";
                var _information = SSID_NOTE_LBL.Text.ToUpper() + "\n\n";
                _information += "SSID      : " + P_SSID + "\n";
                _information += "USERNAME  : " + P_USERNAME + "\n";
                _information += "PASSWORD  : " + P_PASSWORD + "\n";
                _information += "EXPIREON  : " + P_EXPIREON + "\n\n";
                var _footer = "------------------------------------------------\n\n";
                _footer    += "*Please do not share this wifi account.";
                byte[] command = null;
                try
                {
                    if (result != null)
                    {
                        command = ImageHelper.decodeBitmap(result);
                    }
                    else
                    {
                        DisplayMessage("Print Photo error");
                    }
                }
                catch (Exception ex)
                {
                    DisplayMessage("Print Photo error: " + ex.Message.ToString());
                }
                var s = new MemoryStream();
                var bHeader = Encoding.ASCII.GetBytes(_header);
                var bInfo = Encoding.ASCII.GetBytes(_information);
                var bFooter = Encoding.ASCII.GetBytes(_footer);
                s.Write(bHeader, 0, bHeader.Length);
                s.Write(command, 0, command.Length);
                s.Write(bInfo,   0, bInfo.Length);
                s.Write(bFooter, 0, bFooter.Length);
                var b3 = s.ToArray();
                return b3;
            }
            catch(Exception ex)
            {
                ShowError(ex.Message.ToString());
                return null;
            }
        }
    }
}