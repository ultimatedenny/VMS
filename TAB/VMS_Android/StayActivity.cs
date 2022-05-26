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
using Android.Widget;
using VMS.Core.Class;

namespace VMS_Android
{
    [Activity(Label = "Stay Confirmation")]
    public class StayActivity : Activity
    {
        Button btnNext;
        ImageView ImageLunch;
        EditText etDateTo;
        TextView txtTillDate;
        bool isStay = false;
        ISharedPreferences session;
        ISharedPreferencesEditor SessionEdit;
        DateTime MainDate;


        protected override void OnCreate(Bundle savedInstanceState)
        {
           
            base.OnCreate(savedInstanceState);
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.Stay_Page);

            // Create your application here
            BindData();

            SetCustomFont();
        }

        void SetCustomFont()
        {
            var tv1 = FindViewById<TextView>(Resource.Id.txtTitle);
            var tv2 = FindViewById<TextView>(Resource.Id.txtSubTitle);
            var tv3 = FindViewById<TextView>(Resource.Id.footer_1);
            var tv4 = FindViewById<EditText>(Resource.Id.txtExp);
            var tv5 = FindViewById<TextView>(Resource.Id.txtTillDate);

            tv1.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Bold.ttf");
            tv2.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Bold.ttf");
            tv3.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Bold.ttf");
            tv4.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Bold.ttf");
            tv5.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Bold.ttf");


        }

        void BindData()
        {
            session = PreferenceManager.GetDefaultSharedPreferences(this);
            btnNext = FindViewById<Button>(Resource.Id.btnNext);
            etDateTo = FindViewById<EditText>(Resource.Id.txtExp);
            txtTillDate = FindViewById<TextView>(Resource.Id.txtTillDate);

            etDateTo.FocusChange += (sender, e) =>
            {
                if (e.HasFocus)
                {
                    ShowDatePickerDialog(true);
                }
            };
            etDateTo.Click += (sender, e) =>
            {
                ShowDatePickerDialog(true);
            };

            ImageLunch = FindViewById<ImageView>(Resource.Id.ImageLunch);
            ImageLunch.Click += (sender, e) =>
            {
                isStay = !isStay;
                ImageLunch.SetImageResource(isStay ? Resource.Drawable.box_btn_yes : Resource.Drawable.box_btn_no);
                ShowDatePickerDialog(isStay);
            };
            btnNext.Click += (sender, e) =>
            {
                moveActivity();
            };
        }

        void ShowDatePickerDialog(bool isStayDia)
        {
            MainDate = DateTime.Now;
            etDateTo.Text = MainDate.ToString("dd-MMM-yy");
            if (isStayDia)
            {
                txtTillDate.Visibility = ViewStates.Visible;
                etDateTo.Visibility = ViewStates.Visible;

                DatePickerFragment frag = DatePickerFragment.NewInstance(
                delegate (DateTime date)
                {
                    MainDate = date;
                    etDateTo.Text = MainDate.ToString("dd-MMM-yy");
                });

                frag.Show(FragmentManager, DatePickerFragment.TAG);
            }
            else
            {
                etDateTo.Visibility = ViewStates.Invisible;
                txtTillDate.Visibility = ViewStates.Invisible;
                etDateTo.Text = "";
            }
        }

        void moveActivity()
        {
            SessionEdit = session.Edit();
            SessionEdit.PutBoolean(nameof(gVarEnum.Stay), isStay);
            SessionEdit.PutString(nameof(gVarEnum.StayDate), MainDate.ToString("yyyyMMdd"));
            SessionEdit.Apply();
            Intent intent = new Intent(this, typeof(WifiActivity));
            intent.SetFlags(ActivityFlags.SingleTop);
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.abc_slide_in_top, Resource.Animation.abc_slide_in_bottom);
            Finish();
        }
    }
}