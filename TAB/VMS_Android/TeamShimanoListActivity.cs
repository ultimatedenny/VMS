using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using VMS.Core.Constants;
using VMS.Core.Model;
using VMS.Core.Repository;
using VMS_Android.Adapters;

namespace VMS_Android
{
    [Activity(Label = "Find Your Name",Theme = "@style/AppTheme")]
    public class TeamShimanoListActivity : Activity
    {
        ISharedPreferences session;
        VisitorRepository _visitor = new VisitorRepository();
        private List<Temp_VisitorTS> listVisitor;
        private GridView gv;
        private ImageView imageList;
        private Button btnSearch;
        private TextView txtSearch;
        public RelativeLayout relativeLayout;
        public string _Id;
        public string _status;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.Window.AddFlags(WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.Team_Shimano_List);
            session = PreferenceManager.GetDefaultSharedPreferences(this);
            BindData();

            // Create your application here
            SetCustomFont();

            
            

        }

        void SetCustomFont()
        {
            var tv1 = FindViewById<AutoCompleteTextView>(Resource.Id.txtSearch);
            var tv2 = FindViewById<Button>(Resource.Id.btnSearch);
           
            tv1.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            tv2.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            


        }


        protected override void OnResume()
        {
            base.OnResume();
            
        }
        private void BindData()
        {
            gv = FindViewById<GridView>(Resource.Id.GridView);
            txtSearch = FindViewById<TextView>(Resource.Id.txtSearch);
            btnSearch = FindViewById<Button>(Resource.Id.btnSearch);
            imageList = FindViewById<ImageView>(Resource.Id.ImageList);
            imageList.Visibility = ViewStates.Visible;
            gv.Visibility = ViewStates.Invisible;
            relativeLayout = FindViewById<RelativeLayout>(Resource.Id.Box_1);
            //var tv3 = FindViewById<TextView>(Resource.Id.lblName);
            //tv3.Typeface = Typeface.CreateFromAsset(Assets, "Quicksand_Regular.ttf");
            btnSearch.Click += btnSearch_Click;
            txtSearch.KeyPress += txtSearchKeyPress;

            _Id = session.GetString(nameof(gVarEnum.Id), "");
            _status = session.GetString(nameof(gVarEnum.Status), "");
            //_Id = Intent.GetStringExtra("Id") ?? string.Empty;
            //_status = Intent.GetStringExtra("status") ?? string.Empty;
            imageList.SetImageResource(Resource.Drawable.il_search_data);
        }
        

        private void txtSearchKeyPress(object sender, View.KeyEventArgs e)
        {
            if (e.Event.Action == KeyEventActions.Down && e.KeyCode == Keycode.Enter)
            {
                e.Handled = true;
                DismissKeyboard();
                btnSearch.PerformClick();
            }
            else
                e.Handled = false;
        }
        private void DismissKeyboard()
        {
            var view = CurrentFocus;
            if (view != null)
            {
                var imm = (InputMethodManager)GetSystemService(InputMethodService);
                imm.HideSoftInputFromWindow(view.WindowToken, 0);
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (btnSearch.Text.ToUpper() == cButton.Search)
            {
                //if (txtSearch.Text != "")
                //{
                    btnSearch.Text = cButton.Clear;
                    var text = txtSearch.Text;
                    Task _task = SetListVisitor(text);

                //}
                DismissKeyboard();
            }
            else
            {
                
                btnSearch.Text = cButton.Search;
                txtSearch.Text = "";
            }
            

        }
        protected async Task SetListVisitor(string text="")
        {
            listVisitor = await _visitor.GetListVisitorTemp(text);
            if (listVisitor.Count > 0)
            {
                gv.Visibility = ViewStates.Visible;
                imageList.Visibility = ViewStates.Invisible;
                gv.Adapter = new TeamShimanoAdapter(this, listVisitor, _Id, _status);
            }
            else
            {
                gv.Visibility = ViewStates.Invisible;
                imageList.Visibility = ViewStates.Visible;
                imageList.SetImageResource(Resource.Drawable.il_paper_data);
                DisplayMessage("Data Not Found");
            }
            
        }
        private void DisplayMessage(string message)
        {
            Toast.MakeText(this, message, ToastLength.Long).Show();
        }
    }
}