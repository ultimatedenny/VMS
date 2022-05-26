using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Nfc;
using Android.Nfc.Tech;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.IO;
using VMS.Core.Class;
using VMS.Core.Model;
using VMS.Core.Repository;

namespace VMS_Android
{
    [Activity(Label = "RegisterChangeCardActivity")]
    public class RegisterChangeCardActivity : Activity
    {
        VisitorRepository _visitor = new VisitorRepository();
        ISharedPreferences session;
        ISharedPreferencesEditor SessionEdit;
        string ShimanoBadge;
        private string _Id = "";
        private string _Name = "";
        private string _Temp_VisitorId = "";
        public const string ViewApeMimeType = "application/vnd.xamarin.VMS_Android";
        public static readonly string NfcAppRecord = "xamarin.VMS_Android";
        public static readonly string Tag = "VMS_Android";

        private NfcAdapter _nfcAdapter;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.Register_Card);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            // Create your application here
            BindData();

        }
        void BindData()
        {
            session = PreferenceManager.GetDefaultSharedPreferences(this);
            _nfcAdapter = NfcAdapter.GetDefaultAdapter(this);
            _Id = session.GetString(nameof(gVarEnum.Temp_VisitorId), "");
            _Name = session.GetString(nameof(gVarEnum.Name), "");
            _Temp_VisitorId = session.GetString(nameof(gVarEnum.Temp_VisitorId), "");
            DisplayMessage("Name: " + _Name);
        }

        protected override void OnNewIntent(Intent intent)
        {
            Tag tag = intent.GetParcelableExtra(NfcAdapter.ExtraTag) as Tag;

            if (tag == null)
            {
                return;
            }
            // These next few lines will create a payload (consisting of a string)
            // and a mimetype. NFC record are arrays of bytes. 
            TryRead(tag);
        }

        protected async void TryRead(Tag tag)
        {
            ShimanoBadge = DateTime.Now.ToString("yyMMddHHmm") + '_' + _Id;
            var payload = Encoding.ASCII.GetBytes(ShimanoBadge);
            var mimeBytes = Encoding.ASCII.GetBytes(ViewApeMimeType);
            var apeRecord = new NdefRecord(NdefRecord.TnfMimeMedia, mimeBytes, new byte[0], payload);
            var ndefMessage = new NdefMessage(new[] { apeRecord });

            if (!await TryAndWriteToTag(tag, ndefMessage))
            {
                // Maybe the write couldn't happen because the tag wasn't formatted?

            }
            else
            {
                var postData = new ShimanoBadgeModel
                {
                    ShimanoBadge = ShimanoBadge,
                    Temp_Visitor = _Id,

                };
                var Message = await _visitor.PostChangeShimanoBadge(postData);
                if (Message.isSuccess)
                {
                    GotoHomeActivity();
                }
                else
                {
                    DisplayMessage("Cannot Change This Card");
                }
                
            }
        }


        protected void GotoHomeActivity()
        {

            SessionEdit = session.Edit();
            SessionEdit.Clear();
            SessionEdit.Commit();
            Finish();

        }
        protected override void OnResume()
        {
            base.OnResume();
            EnableWriteMode();
        }
        protected override void OnPause()
        {
            base.OnPause();
            // App is paused, so no need to keep an eye out for NFC tags.
            if (_nfcAdapter != null)
                _nfcAdapter.DisableForegroundDispatch(this);
        }

        private void EnableWriteMode()
        {
            var tagDetected = new IntentFilter(NfcAdapter.ActionTagDiscovered);
            var filters = new[] { tagDetected };

            var intent = new Intent(this, GetType()).AddFlags(ActivityFlags.SingleTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, 0);
            if (_nfcAdapter == null)
            {
                var alert = new AlertDialog.Builder(this).Create();
                alert.SetMessage("NFC is not supported on this device.");
                alert.SetTitle("NFC Unavailable");
                alert.SetButton("OK", delegate
                {
                    DisplayMessage("NFC is not supported on this device.");
                });
                alert.Show();
            }
            else
                _nfcAdapter.EnableForegroundDispatch(this, pendingIntent, filters, null);
        }

        private void DisplayMessage(string message)
        {
            Toast.MakeText(this, message, ToastLength.Long).Show();
            Log.Info(Tag, message);
        }

        private bool TryAndFormatTagWithMessage(Tag tag, NdefMessage ndefMessage)
        {
            var format = NdefFormatable.Get(tag);
            if (format == null)
            {
                DisplayMessage("Tag does not appear to support NDEF format.");
            }
            else
            {
                try
                {
                    format.Connect();
                    format.Format(ndefMessage);

                    DisplayMessage("Tag successfully written.");
                    return true;
                }
                catch (IOException ioex)
                {
                    var msg = "There was an error trying to format the tag.";
                    DisplayMessage(msg);
                    Log.Error(Tag, ioex, msg);
                }
            }
            return false;
        }
        private async Task<bool> TryAndWriteToTag(Tag tag, NdefMessage ndefMessage)
        {

            // This object is used to get information about the NFC tag as 
            // well as perform operations on it.
            var ndef = Ndef.Get(tag);
            if (ndef != null)
            {
                ndef.Connect();

                // Once written to, a tag can be marked as read-only - check for this.
                if (!ndef.IsWritable)
                {
                    DisplayMessage("Tag is read-only.");
                }
                if (ndef.NdefMessage != null)
                {
                    var record = ndef.NdefMessage.GetRecords()[0];
                    var data = Encoding.ASCII.GetString(record.GetPayload());
                    var Employee = await _visitor.GetVisitorTSForCheck(data);
                    if (Employee == null)
                    {
                        DisplayMessage("Tag Cannot be wrote, The connection is error");
                        return false;
                    }
                    if (Employee.Name != "NONAME")
                    {
                        DisplayMessage("Tag Cannot be wrote, this tag has been registered for " + Employee.Name);
                        return false;
                    }

                }
                // NFC tags can only store a small amount of data, this depends on the type of tag its.
                var size = ndefMessage.ToByteArray().Length;
                if (ndef.MaxSize < size)
                {
                    DisplayMessage("Tag doesn't have enough space.");
                }
                ndef.WriteNdefMessage(ndefMessage);
                DisplayMessage("Succesfully wrote tag.");
                return true;
            }

            return false;
        }
    }
}