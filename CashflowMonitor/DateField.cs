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
		private string prefix = "";

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
				UpdateText ();
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

		public string Prefix {
			get {
				return prefix;
			}
			set {
				prefix = value;
			}
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
			UpdateText ();
			this.Click += delegate {
				CurrentFld = this; 
				#pragma warning disable 0618	//we are guilty in using deprecated method
				(Context as Activity).ShowDialog (MainActivity.DATE_DIALOG_ID);	
			};
		}

		// the event received when the user "sets" the date in the dialog
		public void OnDateSet (object sender, DatePickerDialog.DateSetEventArgs e)
		{
			this.Date = e.Date;
			UpdateText ();
			(Context as MainActivity).OnDatePicked ();
		}

		public static DatePickerDialog generateDatePicker()
		{
			return new DatePickerDialog (CurrentFld.Context, CurrentFld.OnDateSet, CurrentFld.Year, CurrentFld.Month - 1, CurrentFld.Day);
		}

		private void UpdateText()
		{
			this.Text = prefix+Date.ToString ("d");
		}

	}
}

