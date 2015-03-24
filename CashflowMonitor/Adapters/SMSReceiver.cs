using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using Android.Telephony;
using System.Text;
using CashflowMonitor.Core;


namespace TaskyAndroid.SMS
{
	[BroadcastReceiver(Enabled = true, Label = "SMS Receiver")]
	[IntentFilter(new string[] { "android.provider.Telephony.SMS_RECEIVED" })] 
	public class SmsReceiver : Android.Content.BroadcastReceiver 
	{
		public static readonly string INTENT_ACTION = "android.provider.Telephony.SMS_RECEIVED"; 

		public override void OnReceive(Context context, Intent intent)
		{
			if (intent.Action == INTENT_ACTION)
			{
				StringBuilder buffer = new StringBuilder();
				Bundle bundle = intent.Extras;

				if (bundle != null)
				{
					Java.Lang.Object[] pdus = (Java.Lang.Object[])bundle.Get("pdus");

					SmsMessage[] msgs;
					msgs = new SmsMessage[pdus.Length];

					for (int i = 0; i < msgs.Length; i++) {
						msgs [i] = SmsMessage.CreateFromPdu ((byte[])pdus [i]);

						//Log.Info("SmsReceiver", "SMS Received from: " + msgs[i].OriginatingAddress);
						//Log.Info("SmsReceiver", "SMS Data: " + msgs[i].MessageBody.ToString());
						//Transaction task = new Task ();
						//task.Name = msgs [i].OriginatingAddress;
						//task.Notes = msgs [i].MessageBody.ToString ();
						//TaskManager.SaveTask (task);
					}

					Log.Info("SmsReceiver", "SMS Received");
				}
			} 
		}
	}
}