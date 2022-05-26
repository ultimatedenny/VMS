using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using VMS.Core.Model;
using VMS.Core.Class;
using Android.Preferences;

namespace VMS_Android.Adapters
{
    public class TeamShimanoCCAdapter : BaseAdapter<Visitor_TS>
    {
        ISharedPreferences session;
        ISharedPreferencesEditor SessionEdit;
        int count_int = 0;
        List<Visitor_TS> visitors;
        Activity context;
        private LayoutInflater inflater;

        View row;

        public TeamShimanoCCAdapter(Activity context, List<Visitor_TS> visitors) : base()
        {
            this.visitors = visitors;
            this.context = context;
        }
        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (inflater == null)
            {
                inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            }

            if (convertView == null)
            {
                convertView = inflater
                    .Inflate(Resource.Layout.ModelTS, parent, false);
            }
            else
            {

            }
            CustomAdapterViewHolder holder = new CustomAdapterViewHolder(convertView)
            {
                lblName = { Text = visitors[position].Name },

            };

            var Box_1 = convertView.FindViewById<RelativeLayout>(Resource.Id.Box_1);
            Box_1.Click += (sender, e) =>
            {
                if (count_int < 1)
                {
                    Intent intent = new Intent(context, typeof(RegisterChangeCardActivity));
                    session = PreferenceManager.GetDefaultSharedPreferences(context);
                    SessionEdit = session.Edit();
                    SessionEdit.PutString(nameof(gVarEnum.Name), visitors[position].Name);
                    SessionEdit.PutString(nameof(gVarEnum.Temp_VisitorId), visitors[position].Id.ToString());
                    SessionEdit.Apply();
                    intent.SetFlags(ActivityFlags.SingleTop);
                    context.StartActivity(intent);
                    context.Finish();
                    count_int++;
                }


            };

            return convertView;
        }

        public override int Count
        {
            get
            {
                return visitors.Count;
            }
        }

        public override Visitor_TS this[int position] => throw new NotImplementedException();

        class CustomAdapterViewHolder : Java.Lang.Object
        {
            //adapter views to re-use
            public TextView lblName;

            public CustomAdapterViewHolder(View itemView)
            {
                lblName = itemView.FindViewById<TextView>(Resource.Id.lblName);
            }
        }
    }
}