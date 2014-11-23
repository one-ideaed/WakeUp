using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Cashflow_Monitor
{
	[Activity (Label = "Cashflow_Monitor", MainLauncher = true, Icon = "@drawable/icon")]
	public class MainActivity : Activity
	{
		int count = 0;

		protected override void OnCreate (Bundle bundle)
		{
			Console.WriteLine ("Step 1");
			base.OnCreate (bundle);
			Console.WriteLine ("Step 2");

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			Console.WriteLine ("Step 3");

			// Get our button from the layout resource,
			// and attach an event to it
			TextView counterText = FindViewById<TextView> (Resource.Id.countText);
			counterText.SetText("2t", TextView.BufferType.Normal);
			SmsReceiver smsReceiver = new SmsReceiver ();
			Console.WriteLine ("Step 4");

		}
	}
}


