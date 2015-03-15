using System;
using CM.Core;
using CM.Core.BusinessEntities;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace CashflowMonitor
{
	[Activity (Label = "CashflowMonitor", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		int count = 1;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myButton);
			Transaction tr = new Transaction ();
			tr.Amount = 0.111;
			tr.Comment = "First transaction";
			tr.Source = TransactionSource.Manual;
			tr.Type = TransactionType.Expense;
			tr.TimeStamp = DateTime.Now;
			//Manager dbmanager = new Manager ();
			var res=Manager.SaveTransaction(tr);
			var  ttt = Manager.GetTransactions ();
			button.Click += delegate {
				button.Text = string.Format ("{0} clicks!", count++);
			};
		}
	}
}


