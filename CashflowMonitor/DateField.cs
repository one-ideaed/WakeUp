using System;
using System.Windows;
using Android.App;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Runtime;

namespace CashflowMonitor
{
	public class DateField : TextView 
	{
		private static DateField currentFld = null; 
		private DateTime date;

		public static DateField CurrentFld {
			get {
				return currentFld;
			}
			set {
				currentFld = value;
			}
		}


		public DateTime Date {
			get {
				return date;
			}
			set {
				date = value;
			}
		}

		public int Year
		{
			get {return date.Year;}
		}
		public int Month
		{
			get {return date.Month;}
		}
		public int Day
		{
			get {return date.Day;}
		}

		public DateField (Context context) :base(context)
		{
			init ();
		}

		public DateField(Context context, IAttributeSet attrs) : base (context, attrs)
		{
			init ();
		}
			
		public DateField (Context context, IAttributeSet attrs, int defStyle) : base (context, attrs, defStyle)
		{
			init ();
		}

		protected DateField (IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
			init ();
		}

		protected void init()
		{
			this.date = DateTime.Today;
			this.Text = date.ToString ("d");
			this.Click += delegate {
				CurrentFld = this; 
				(Context as Activity).ShowDialog (MainActivity.DATE_DIALOG_ID);	
				//(Context as MainActivity)
			};
		}
		// the event received when the user "sets" the date in the dialog
		public void OnDateSet (object sender, DatePickerDialog.DateSetEventArgs e)
		{
			this.Date = e.Date;
			Text = CurrentFld.date.ToString ("d");
			(Context as MainActivity).RemoveDialog (MainActivity.DATE_DIALOG_ID);
		}

		public static DatePickerDialog generateDatePicker()
		{
			return new DatePickerDialog (CurrentFld.Context, CurrentFld.OnDateSet, CurrentFld.Year, CurrentFld.Month - 1, CurrentFld.Day);
		}
	}
}

