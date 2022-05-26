using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using VMS.Core.Class;
using VMS.Core.Model;
using VMS.Core.Repository;

namespace VMS_Android
{
    [Activity(Label = "Sign Confirmation")]
    public class SignInOutActivity : Activity
    {
        VisitorRepository _visitor = new VisitorRepository();
        ISharedPreferences session;
        ISharedPreferencesEditor SessionEdit;
        Timer timer = new System.Timers.Timer();
        ImageView ImageSign;
        TextView txtName, txtPlant, txtWelcome, txtSubWelcome, txtSession;
        int counting = 0;
        Dictionary<welcome, string> Welcome = new Dictionary<welcome, string>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.SignInOut_Layout);
            // Create your application here
            session = PreferenceManager.GetDefaultSharedPreferences(this);
            PopulateWelcome();
            FindViews();
            CountDown();
            SetCustomFont();
        }

        void SetCustomFont()
        {
            var tv1 = FindViewById<TextView>(Resource.Id.txtSession);
            var tv2 = FindViewById<TextView>(Resource.Id.txtName);
            var tv3 = FindViewById<TextView>(Resource.Id.txtPlant);
            var tv4 = FindViewById<TextView>(Resource.Id.txtWelcome);
            var tv5 = FindViewById<TextView>(Resource.Id.txtSubWelcome);


            tv1.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv2.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Bold.ttf");
            tv3.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Bold.ttf");
            tv4.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Bold.ttf");
            tv5.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");


        }

        //protected async Task PostVisitorCheckin()
        //{
        //    var ShimanoBadge = session.GetString(nameof(gVarEnum.CardId), "");
        //    var Temp_Id = session.GetString(nameof(gVarEnum.Temp_VisitorId), "");
        //    var NeedLunch = session.GetBoolean(nameof(gVarEnum.Lunch), true);
        //    var NeedStay = session.GetBoolean(nameof(gVarEnum.Stay), true);
        //    var PhotoName = session.GetString(nameof(gVarEnum.Photo), "");
        //    var StayDate = session.GetString(nameof(gVarEnum.StayDate), "");
        //    if (NeedStay == false)
        //    {
        //        StayDate = DateTime.Now.ToString("yyyyMMdd");
        //    }
        //    var postData = new ShimanoBadgeModel
        //    {
        //        NeedLunch = NeedLunch,
        //        ShimanoBadge = ShimanoBadge,
        //        Temp_Visitor = Temp_Id,
        //        PhotoName = PhotoName,
        //        NeedStay = NeedStay,
        //        StayDate = StayDate

        //    };
        //    var Message = await _visitor.PostVisitorCheckin(postData);

        //}
        protected async Task PostVisitorCheckOut()
        {
            var Temp_Id = session.GetString(nameof(gVarEnum.Temp_VisitorId), "");
            var Message = await _visitor.PostVisitorCheckOut(Temp_Id);

        }

        void FindViews()
        {
            
            //===========================================================
            ImageSign = FindViewById<ImageView>(Resource.Id.ImageSign);
            txtName = FindViewById<TextView>(Resource.Id.txtName);
            txtPlant = FindViewById<TextView>(Resource.Id.txtPlant);
            txtWelcome = FindViewById<TextView>(Resource.Id.txtWelcome);
            txtSubWelcome = FindViewById<TextView>(Resource.Id.txtSubWelcome);
            txtSession = FindViewById<TextView>(Resource.Id.txtSession);
            var status = session.GetString(nameof(gVarEnum.Status), "");
            if (status == nameof(SttVisitor.PENDING))
            {
                txtWelcome.Text = Welcome[welcome.SignInWelcome];
                txtSubWelcome.Text = Welcome[welcome.SignInSubWelcome];
                ImageSign.SetImageResource(Resource.Drawable.sign_in_door);
                //Task t = PostVisitorCheckin();
            }
            else
            {
                txtWelcome.Text = Welcome[welcome.SignOutWelcome];
                txtSubWelcome.Text = Welcome[welcome.SignOutSubWelcome];
                ImageSign.SetImageResource(Resource.Drawable.sign_out_door);
                Task t = PostVisitorCheckOut();

            }
            txtName.Text = session.GetString(nameof(gVarEnum.Name), "");


        }
        void PopulateWelcome()
        {
            Welcome.Add(welcome.SignInWelcome, "WELCOME, YOU'RE IN!");
            Welcome.Add(welcome.SignInSubWelcome, "Your host has been notified and will be with you shortly, please take a seat!");
            Welcome.Add(welcome.SignOutWelcome, "THANK YOU, AND CHEERS!");
            Welcome.Add(welcome.SignOutSubWelcome, "Have a pleasant trip!");
        }

        async Task CountDown()
        {
            List<CodLst> CodLs = await _visitor.GetCodLst("EndRemainingTime");
            Int16 cnt = Convert.ToInt16(CodLs[0].Cod.ToString());
            if (cnt == 0) cnt = 3;

            counting = cnt;
            timer.Interval = 1000;
            timer.Enabled = true;
            timer.Elapsed += (sender, e) =>
            {
                RunOnUiThread(() =>
                {
                    txtSession.Text = "YOUR SESSION WILL BE FINISH IN - " +  counting.ToString();
                    if (counting == 0)
                    {
                        timer.Stop();
                        SessionEdit = session.Edit();
                        SessionEdit.Clear();
                        SessionEdit.Commit();
                        Finish();
                    }
                    counting--;

                });
            };
            timer.Start();

        }
    }
}