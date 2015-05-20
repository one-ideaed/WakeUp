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
using CashflowMonitor;
using CashflowMonitor.Core.BusinessEntities;


namespace TaskyAndroid.SMS
{
	//[BroadcastReceiver(Enabled = true, Label = "SMS Receiver")]
	//[IntentFilter(new string[] { "android.provider.Telephony.SMS_RECEIVED" })] 
	public class SmsReceiver : Android.Content.BroadcastReceiver 
	{
		public static readonly string INTENT_ACTION = "android.provider.Telephony.SMS_RECEIVED"; 
		private MainActivity mainActivity;

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
					SmsParser parser = new SmsParser ();

					for (int i = 0; i < msgs.Length; i++) {
						SmsMessage sms = SmsMessage.CreateFromPdu ((byte[])pdus [i]);
						msgs [i] = sms;

						if (IsFromBank (sms)) {
							Transaction tr = parser.Parse (sms);
							if (tr != null && mainActivity != null) {
								mainActivity.HandleNewTransaction (tr);
							}
						}	 
					}
						
					Log.Info("SmsReceiver", "SMS Received");
				}
			} 
		}

		private bool IsFromBank(SmsMessage sms)
		{
			return sms.OriginatingAddress.Equals ("729") ? true : false;
		}

		public MainActivity MainActivity {
			get {
				return mainActivity;
			}
			set {
				mainActivity = value;
			}
		}
	}
}