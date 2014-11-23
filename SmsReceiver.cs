using System;

using Android.Content;
using Android.Widget;
using Android.App;


namespace Cashflow_Monitor
{
	[BroadcastReceiver(Enabled = true, Label = "SMS Receiver")]
	[IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" })] 
	public class SmsReceiver : BroadcastReceiver
	{
		public override void OnReceive (Context context, Android.Content.Intent intent)
		{
			Console.Write ("Sms 1");
			//Toast.MakeText (context, "SMS received", ToastLength.Long).Show ();
			/*var nMgr = (NotificationManager)context.GetSystemService("android.provider.Telephony.SMS_RECEIVED");
			var notification = new Notification (Resource.Drawable.Icon, "Cashflow Monitor received SMS");
			var pendingIntent = PendingIntent.GetActivity (context, 0, new Intent (context, typeof(MainActivity)), 0);
			notification.SetLatestEventInfo (context, "Cashflow Monitor Notification", "Here is another SMS to process", pendingIntent);
			nMgr.Notify (0, notification);*/
			// Instantiate the builder and set notification elements:
			Notification.Builder builder = new Notification.Builder (context)
				.SetContentTitle ("Cashflow Monitor")
				.SetContentText ("Here is another SMS to process!")
				.SetSmallIcon (Resource.Drawable.Icon);

			// Build the notification:
			Notification notification = builder.Build();

			// Get the notification manager:
			NotificationManager notificationManager =
				context.GetSystemService (Context.NotificationService) as NotificationManager;

			// Publish the notification:
			const int notificationId = 0;
			notificationManager.Notify (notificationId, notification);
			Console.Write ("Sms 2");
			//((StockActivity)context).GetStocks ();

			//InvokeAbortBroadcast();
		}
	}
}

