using System;
using System.Collections.Generic;
using CashflowMonitor.Core;
using CashflowMonitor.Core.BusinessEntities;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using TaskyAndroid.SMS;
using System.Globalization;
using Android.Telephony;
using Android.Database;
using Android.Provider;


namespace CashflowMonitor
{
	[Activity (Label = "CashflowMonitor", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Android.App.ListActivity
	{
	    public const int DATE_DIALOG_ID = 0;
		private int count = 1;
		private TransactionManager transManager;
		private SmsReceiver smsReceiver;
		private DateField dateFrom;
		private DateField dateTo;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Init Transaction list 
			transManager = new TransactionManager (this);

			// Create two date fields
			dateFrom = (DateField)FindViewById<DateField> (Resource.Id.dateFld1);
			dateFrom.Prefix = "From  ";
			dateFrom.Date = transManager.GetOldestTransactionDate ();
			dateTo = (DateField)FindViewById<DateField> (Resource.Id.dateFld2);
			dateTo.Prefix = "To  ";
			dateTo.Date = DateTime.Today;

			//Show transactions in a list
			IList<Transaction> tl= Manager.GetTransactions ();
			if (tl.Count == 0) {  
				RegisterExistingSms ();
			} else {
				transManager.ShowTransactions (dateFrom.Date, dateTo.Date);
			}

			//Allow us to proccess new incoming SMS
			RegisterSmsReceiver ();

			// Just a useless feature
			Button button = FindViewById<Button> (Resource.Id.myButton);
			button.Click += delegate {
				button.Text = string.Format ("{0} clicks!", count++);
			};
		}

		#pragma warning disable 0672	//we are guilty in overwriting deprecated method
		protected override Dialog OnCreateDialog (int id)
		{
			switch (id) {
			case DATE_DIALOG_ID:
				return DateField.generateDatePicker(); 
			}
			return null;
		}

		public void HandleNewTransaction(Transaction tr)
		{
			transManager.Save (tr);
			transManager.ShowTransactions(dateFrom.Date, dateTo.Date);
		}

		public void OnDatePicked()
		{
			#pragma warning disable 0618	//we are guilty in using deprecated method
			RemoveDialog (MainActivity.DATE_DIALOG_ID);
			transManager.ShowTransactions (dateFrom.Date, dateTo.Date);
		}

		public void UpdateList(List<string> transactions)
		{
			if (ListAdapter == null)
			{
				ListAdapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItem1, transactions);
			}
			else
			{
				ArrayAdapter aa = ListAdapter as ArrayAdapter;
				aa.Clear();
				aa.AddAll(transactions);
			}
		}

		private void RegisterSmsReceiver()
		{
			smsReceiver = new SmsReceiver ();
			smsReceiver.MainActivity = this;
			IntentFilter intentFilter = new IntentFilter("android.provider.Telephony.SMS_RECEIVED");
			this.RegisterReceiver(smsReceiver, intentFilter);
		}

		private void RegisterExistingSms()
		{
			SmsParser parser = new SmsParser ();

			ICursor c =  this.ContentResolver.Query (Telephony.Sms.Inbox.ContentUri, null, null, null, "date DESC");
			while(c.MoveToNext())
			{
				string body = c.GetString (c.GetColumnIndexOrThrow("body"));
				long time = c.GetLong (c.GetColumnIndexOrThrow("date"));
				string address = c.GetString(c.GetColumnIndexOrThrow("address"));
				Transaction tr = parser.Parse (body, time, address);
				if (tr != null) {
					transManager.Save (tr);
				}
			}
		}
	}
}



