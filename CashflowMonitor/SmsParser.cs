using System;
using CashflowMonitor.Core.BusinessEntities;
using Android.Telephony;
using System.Globalization;

namespace CashflowMonitor
{
	public class SmsParser
	{
		private Transaction tr;
		private const string BANK_ADDRESS = "729";

		public Transaction Parse(SmsMessage sms)
		{
			return Parse (sms.DisplayMessageBody, sms.TimestampMillis, sms.OriginatingAddress);
		}
			

		public Transaction Parse(String text, long time, string address)
		{
			bool ok = true;
			tr = null;  // refresh 
			if(address == BANK_ADDRESS && text.Contains("UAH."))  // should we create transaction?
			{
				tr = new Transaction ();
				tr.Source = TransactionSource.SMS;
				DateTime start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
				DateTime date= start.AddMilliseconds(time).ToLocalTime();
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

			if (text.Contains ("Dostupnyi zalyshok")) {
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
			return Double.Parse(word, CultureInfo.GetCultureInfo("en-US"));
		}
	}
}

