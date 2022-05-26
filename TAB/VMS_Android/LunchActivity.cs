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
    [Activity(Label = "Lunch Confirmation")]
    public class LunchActivity : Activity
    {
        Button btnNext;
        ImageView ImageLunch;
        bool isLunch = true;
        ISharedPreferences session;
        ISharedPreferencesEditor SessionEdit;
        protected override void OnCreate(Bundle savedInstanceState)
        {
           
            base.OnCreate(savedInstanceState);
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.Lunch_Page);

            // Create your application here
            BindData();

            SetCustomFont();
        }

        void SetCustomFont()
        {
            var tv1 = FindViewById<TextView>(Resource.Id.txtTitle);
            var tv2 = FindViewById<TextView>(Resource.Id.txtSubTitle);
            var tv3 = FindViewById<TextView>(Resource.Id.footer_1);

            tv1.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Bold.ttf");
            tv2.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Bold.ttf");
            tv3.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Bold.ttf");
            

        }

        void BindData()
        {
            session = PreferenceManager.GetDefaultSharedPreferences(this);
            btnNext = FindViewById<Button>(Resource.Id.btnNext);
            ImageLunch = FindViewById<ImageView>(Resource.Id.ImageLunch);
            ImageLunch.Click += (sender, e) =>
            {
                isLunch = !isLunch;
                ImageLunch.SetImageResource(isLunch ? Resource.Drawable.box_btn_yes : Resource.Drawable.box_btn_no);
            };
            btnNext.Click += (sender, e) =>
            {
                moveActivity();
            };
        }
        void moveActivity()
        {
           

            var Temp_Id = session.GetString(nameof(gVarEnum.Temp_VisitorId), "");


            Intent intent = new Intent(this, typeof(StayActivity));
            SessionEdit = session.Edit();
            SessionEdit.PutBoolean(nameof(gVarEnum.Lunch), isLunch);
            if(Temp_Id !="0" )
            {
                intent = new Intent(this, typeof(WifiActivity));
            }
            SessionEdit.Apply();

            
            intent.SetFlags(ActivityFlags.SingleTop);
            StartActivity(intent);
            OverridePendingTransition(Resource.Animation.abc_slide_in_top, Resource.Animation.abc_slide_in_bottom);
            Finish();
        }
    }
}