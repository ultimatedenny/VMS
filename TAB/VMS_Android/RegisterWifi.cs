using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using VMS.Core.Class;
using VMS.Core.Repository;

namespace VMS_Android
{
    [Activity(Label = "RegisterWifi")]
    public class RegisterWifi : Activity
    {
        VisitorRepository _visitor = new VisitorRepository();
        ISharedPreferences session;
        ISharedPreferencesEditor SessionEdit;
        Button btnSubmit, btnBack;
        EditText txtHost, txtVisitor, txtExp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.register_wifi);

            getView();

            btnSubmit.Click += BtnSubmit_Click;

            btnBack.Click += (sender, e) =>
            {
                Finish();
            };

            SetCustomFont();
        }

        void SetCustomFont()
        {
            var tv1 = FindViewById<TextView>(Resource.Id.tit1e_1);
            var tv2 = FindViewById<TextView>(Resource.Id.visName);
            var tv3 = FindViewById<Button>(Resource.Id.btnGen);
            var tv4 = FindViewById<TextView>(Resource.Id.txtInfo);
            var tv5 = FindViewById<TextView>(Resource.Id.footer_1);
            var tv6 = FindViewById<TextView>(Resource.Id.lblDuration);
            var tv7 = FindViewById<EditText>(Resource.Id.txtHostName);
            var tv8 = FindViewById<EditText>(Resource.Id.txtVisitorName);
            var tv9 = FindViewById<EditText>(Resource.Id.txtExp);

            tv1.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv2.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv3.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv4.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv5.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Bold.ttf");
            tv6.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv7.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv8.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv9.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");

        }


        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHost.Text))
            {
                Toast.MakeText(this, "Host name can't empty", ToastLength.Long).Show();
                return;
            }
            else if (string.IsNullOrWhiteSpace(txtVisitor.Text))
            {
                Toast.MakeText(this, "Visitor name can't empty", ToastLength.Long).Show();
                return;
            }
            //else if(tpick.Hour < DateTime.Now.Hour)
            //{
            //    Toast.MakeText(this, "Invalid selection time", ToastLength.Long).Show();
            //    return;
            //}
            else
            {

                Intent intent =
                new Intent(this, typeof(WifiActivity));
                session = PreferenceManager.GetDefaultSharedPreferences(this);
                SessionEdit = session.Edit();
                SessionEdit.PutString("WIFI_Host", txtHost.Text);
                SessionEdit.PutString("WIFI_Visitor", txtVisitor.Text);
                SessionEdit.PutString("WIFI_Time", txtExp.Text);
                SessionEdit.PutString(nameof(gVarEnum.Name), "GUEST");
                SessionEdit.Apply();
                intent.SetFlags(ActivityFlags.SingleTop);
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.abc_slide_in_top, Resource.Animation.abc_slide_in_bottom);
                //txtHost.Text = "";
                //txtVisitor.Text = "";

            }
        }

        void getView()
        {
            session = PreferenceManager.GetDefaultSharedPreferences(this);
            btnBack = FindViewById<Button>(Resource.Id.btnNext);
            btnSubmit = FindViewById<Button>(Resource.Id.btnGen);
            txtHost = FindViewById<EditText>(Resource.Id.txtHostName);
            txtHost.Text = session.GetString(nameof(gVarEnum.Name), "");
            txtVisitor = FindViewById<EditText>(Resource.Id.txtVisitorName);
            txtVisitor.Text = "GUEST";
            txtExp = FindViewById<EditText>(Resource.Id.txtExp);
            txtExp.Text = DateTime.Now.AddHours(2).ToString(@"HH\:mm");
            txtExp.FocusChange += (sender, e) =>
            {
                if (e.HasFocus)
                {
                    ShowTime();
                }
            };
            txtExp.Click += (sender, e) =>
            {
                ShowTime();
            };

        }

        void ShowTime()
        {

            var imm = (InputMethodManager)GetSystemService(InputMethodService);
            imm.HideSoftInputFromWindow(CurrentFocus.WindowToken, 0);


            TimePickerFragment frag = TimePickerFragment.NewInstance(
            delegate (DateTime time)
            {
                txtExp.Text = time.ToString(@"HH\:mm");
            });

            frag.Show(FragmentManager, TimePickerFragment.TAG);
        }
    }
}