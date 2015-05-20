using System;
using CashflowMonitor.Core.BusinessEntities;
using Android.Telephony;

namespace CashflowMonitor
{
	public class SmsParser
	{
		private Transaction tr;

		public SmsParser() {}

		public Transaction Parse(SmsMessage sms)
		{
			bool ok = true;
			tr = null;  // refresh 
			String text = sms.DisplayMessageBody;
			if(text.Contains("UAH."))  // should we create transaction?
			{
				tr = new Transaction ();
				tr.Source = TransactionSource.SMS;
				DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				DateTime date= start.AddMilliseconds(sms.TimestampMillis).ToLocalTime();
				tr.TimeStamp = date;
				ok &= ParseTransactionType (text);
				ok &= ParseAmount (text);
				ok &= ParseComment (text);
			}
			return tr;
		}

		private bool ParseTransactionType(string text)
		{
			bool ok = true;

			if (text.Contains ("Oplata tovariv"))
				tr.Type = TransactionType.Expense;
			else if(text.Contains("Popovnennya rakhunku"))
				tr.Type = TransactionType.Income;
			else if (text.Contains ("Perekaz koshtiv"))
				tr.Type = TransactionType.Reallocation;
			else if (text.Contains("Blokuvannia koshtiv"))
				tr.Type = TransactionType.Blocking;
			else
				ok = false;
									
			return ok;
		}

		private bool ParseAmount(string text)
		{
			bool ok = false;

			string[] words = text.Split (' ');
			for (int i = 0; i < words.Length; i++) {
				string word = words [i];
				if (word.EndsWith ("UAH.")) // We expect to get smth like "12,345.67UAH."
				{
					tr.Amount = ExtractAmount (word);
					ok = true;
					break;
				}
			}
			return ok;
		}

		private bool ParseComment(string text)
		{
			bool ok = false;

			if (text.Contains ("Dostupniy zalyshok")) {
				string[] words = text.Split (' ');
				string lastword = words [words.Length - 1]; // expected to have the remaining balance amount in the last word
				double a = ExtractAmount(lastword); 
				tr.Comment = "Remaining amount = " + a.ToString();
				ok = true;	
			} 
			return ok;
		}

		private double ExtractAmount(string word)
		{
			word = word.Remove (word.LastIndexOf ('U'));  //Remove "UAH."
			return Double.Parse(word);
		}
	}
}

