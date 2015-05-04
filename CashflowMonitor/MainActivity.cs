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


namespace CashflowMonitor
{
	[Activity (Label = "CashflowMonitor", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Android.App.ListActivity//Activity
	{
	    public const int DATE_DIALOG_ID = 0;
		int count = 1;
		DateField dateFld1;
		DateField dateFld2;
		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
		
			dateFld1 = (DateField)FindViewById<DateField> (Resource.Id.dateFld1);
			dateFld2 = (DateField)FindViewById<DateField> (Resource.Id.dateFld2);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			Transaction tr = new Transaction ();
			Random random = new Random ();
			tr.Amount = random.NextDouble() ;
			tr.Comment = "First transaction";
			tr.Source = TransactionSource.Manual;
			tr.Type = TransactionType.Expense;
			tr.TimeStamp = DateTime.Now;
			//Manager dbmanager = new Manager ();
			var res=Manager.SaveTransaction(tr);
			IList<Transaction>  translist = Manager.GetTransactions ();
			String[] tv = new string[translist.Count];
			for (int i = 0; i < translist.Count; i++) {
				String s = "Amount = " + Math.Round(translist [i].Amount, 2) + "   Date = " + translist [i].TimeStamp.ToString();
				tv.SetValue (s, i);
			}
			var adapter = new ArrayAdapter<String>(this, Android.Resource.Layout.SimpleListItemChecked,tv);
			ListAdapter = adapter;


			button.Click += delegate {
				button.Text = string.Format ("{0} clicks!", count++);
			};
		//	(FindViewById<ListView> (Resource.Id.list)).Adapter = adapter;
		}
		protected override Dialog OnCreateDialog (int id)
		{
			switch (id) {
			case DATE_DIALOG_ID:
				return DateField.generateDatePicker(); 
			}
			return null;
		}
	}
}


