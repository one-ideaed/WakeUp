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


namespace CashflowMonitor
{
	[Activity (Label = "CashflowMonitor", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Android.App.ListActivity//Activity
	{
	    public const int DATE_DIALOG_ID = 0;
		int count = 1;
		TransactionManager transManager;
		SmsReceiver smsReceiver;
		DateField dateFld1;
		DateField dateFld2;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
		
			// Create two date fields
			dateFld1 = (DateField)FindViewById<DateField> (Resource.Id.dateFld1);
			dateFld2 = (DateField)FindViewById<DateField> (Resource.Id.dateFld2);

			// Init Transaction list 
		    transManager = new TransactionManager ();
			IList<Transaction> tl= Manager.GetTransactions ();
			if (tl.Count == 0) {  // create two dummy transactions
				transManager.Save (transManager.CreateDummyTransaction ());
				transManager.Save (transManager.CreateDummyTransaction ());
			}
			ListAdapter = transManager.ShowAll (this);
		
			// Just a useless feature
			Button button = FindViewById<Button> (Resource.Id.myButton);
			button.Click += delegate {
				button.Text = string.Format ("{0} clicks!", count++);
			};

			RegisterSmsReceiver ();
		}

		protected override Dialog OnCreateDialog (int id)
		{
			switch (id) {
			case DATE_DIALOG_ID:
				return DateField.generateDatePicker(); 
			}
			return null;
		}

		private void RegisterSmsReceiver()
		{
			smsReceiver = new SmsReceiver ();
			smsReceiver.MainActivity = this;
			IntentFilter intentFilter = new IntentFilter("android.provider.Telephony.SMS_RECEIVED");
			this.RegisterReceiver(smsReceiver, intentFilter);
		}

		public void HandleNewTransaction(Transaction tr)
		{
			TransManager.Save (tr);
			ListAdapter = TransManager.ShowAll(this);
		}

		public TransactionManager TransManager {
			get {
				return transManager;
			}
		}
	}
}



