using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text.Format;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace VMS_Android
{
    public class DatePickerFragment : DialogFragment, DatePickerDialog.IOnDateSetListener
    {
        public static readonly string TAG = "MyDatePickerFragment";
        Action<DateTime> dateSelectedHandler = delegate { };

        public static DatePickerFragment NewInstance(Action<DateTime> onDateSelected)
        {
            DatePickerFragment frag = new DatePickerFragment();
            frag.dateSelectedHandler = onDateSelected;
            return frag;
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {

            DateTime currently = DateTime.Now.AddMonths(-1);
            DatePickerDialog dialog = new DatePickerDialog(Activity,
                                                           this,
                                                           currently.Year,
                                                           currently.Month,
                                                           currently.Day);

            dialog.DatePicker.MinDate = (long)(currently.AddMonths(1).AddDays(1) - new DateTime(1970, 1, 1)).TotalMilliseconds;
            //dialog.DatePicker.MaxDate = (long)(currently.AddDays(3) - new DateTime(1970, 1, 1)).TotalMilliseconds;
            return dialog;
        }

        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            DateTime currentTime = DateTime.Now;
            DateTime selectedDate = new DateTime();
            selectedDate = currentTime;
            try
            {
                selectedDate = new DateTime(year, month, 1);
                selectedDate = selectedDate.AddMonths(1);
                selectedDate = selectedDate.AddDays(dayOfMonth-1);
            }
            catch (Exception ex) { }
            
            Log.Debug(TAG, selectedDate.ToShortDateString());
            dateSelectedHandler(selectedDate);
        }

        
    }
}