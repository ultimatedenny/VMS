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
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using VMS.Core.Class;
using VMS.Core.Model;
using VMS.Core.Repository;

namespace VMS_Android
{
    [Activity(Label = "Identity Verification")]
    public class CameraActivity : Activity
    {
        VisitorRepository _visitor = new VisitorRepository();
        ISharedPreferences session;
        ISharedPreferencesEditor SessionEdit;
        Timer timer = new System.Timers.Timer();
        TextView txtTitle, txtSubTitle, txtCounter;
        Button btnCamera, btnNext;
        string _TempId, _Status, _Name, _ShimanoBadge;
        int countCamera = 0;
        private CameraFragment cameraFragment;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.Camera_Layout);
            // Create your application here\
            session = PreferenceManager.GetDefaultSharedPreferences(this);
            FindViews();
            CountDown();
            SetCustomFont();
        }

        void SetCustomFont()
        {
            var tv1 = FindViewById<TextView>(Resource.Id.txtTitle);
            var tv2 = FindViewById<TextView>(Resource.Id.txtSubTitle);
            var tv4 = FindViewById<TextView>(Resource.Id.txtSignIn);

            tv1.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv2.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv4.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");

        }

        void FindViews()
        {
            btnNext = FindViewById<Button>(Resource.Id.btnNext);
            txtCounter = FindViewById<TextView>(Resource.Id.txtCounter);
            txtTitle = FindViewById<TextView>(Resource.Id.txtTitle);
            btnCamera = FindViewById<Button>(Resource.Id.btnRetake);
            btnCamera.Click += btnCamera_Click;
            _TempId = session.GetString(nameof(gVarEnum.Temp_VisitorId), "");
            _Status = session.GetString(nameof(gVarEnum.Status), "");
            _Name = session.GetString(nameof(gVarEnum.Name), "");
            _ShimanoBadge = session.GetString(nameof(gVarEnum.CardId), "");
            txtTitle.Text = "Hi, " + _Name + " !";
            btnNext.Click += (sender, e) =>
            {
                moveActivity();
            };
            cameraFragment = new CameraFragment();
            FragmentManager.BeginTransaction()
               .Replace(Resource.Id.content_frame, cameraFragment)
               .Commit();
        }

        async void moveActivity()
        {
            var PhotoName = session.GetString(nameof(gVarEnum.Photo), "");
            var postData = new ShimanoBadgeModel
            {
                ShimanoBadge = _ShimanoBadge,
                Temp_Visitor = _TempId,
                PhotoName = PhotoName

            };
            var Message = await _visitor.PostNewShimanoBadge(postData);
            if (Message.isSuccess)
            {
                Intent intent = new Intent(this, typeof(LunchActivity));
                intent.SetFlags(ActivityFlags.SingleTop);
                StartActivity(intent);
                OverridePendingTransition(Resource.Animation.abc_slide_in_top, Resource.Animation.abc_slide_in_bottom);
                Finish();
            }

            
        }

        void CountDown()
        {
            countCamera = 3;
            timer.Interval = 1000;
            timer.Enabled = true;
            btnNext.Enabled = false;
            timer.Elapsed += (sender, e) =>
            {
                RunOnUiThread(() =>
                {
                    txtCounter.Text = countCamera.ToString();
                    if (countCamera == 0)
                    {
                        timer.Stop();
                        countCamera = 4;
                        txtCounter.Text = "END";
                        btnNext.Enabled = true;
                        cameraFragment.SnapWithTime();
                    }
                    countCamera--;

                });
            };
            timer.Start();
            
        }

        private void btnCamera_Click(object sender, EventArgs e)
        {
            btnNext.Enabled = false;
            //runFragment();
            timer.Start();
            cameraFragment.RetakePhoto();

        }

    }
}