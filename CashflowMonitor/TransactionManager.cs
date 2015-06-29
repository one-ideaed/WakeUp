using System;
using CashflowMonitor.Core.BusinessEntities;
using CashflowMonitor.Core;
using System.Collections.Generic;
using Android.Widget;
using Android.Content;

namespace CashflowMonitor
{
	public class TransactionManager
	{
		private List<Transaction> transList = null;
		private MainActivity mainActivity = null;

		public TransactionManager (MainActivity ma)
		{
			mainActivity = ma;
			transList = (List<Transaction>)Manager.GetTransactions ();
			transList.Sort (CompareByDate);
		}

		public Transaction CreateDummyTransaction()
		{
			Transaction tr = new Transaction ();
			tr.Id = GenerateId ();
			Random random = new Random ();
			tr.Amount = random.NextDouble();
			tr.Comment = "First transaction";
			tr.Source = TransactionSource.Manual;
			tr.Type = TransactionType.Expense;
			tr.TimeStamp = DateTime.Now;
			return tr;
		}
			
		public Transaction CreateTransaction(DateTime date, double amount, TransactionType type, TransactionSource source, string comment)
		{
			Transaction tr = new Transaction (GenerateId(), date, amount, type, source, comment);
			return tr;
		}

		public void Save(Transaction tr)
		{
			var res=Manager.SaveTransaction(tr);
			transList.Add (tr);
			transList.Sort (CompareByDate);
		}

		public void ShowAllTransactions()
		{
			ShowTransactions (GetOldestTransactionDate(), DateTime.Today);
		}

		public void ShowTransactions(DateTime fromDate, DateTime toDate)
		{
			List<String> trlist = new List<String>();
			for (int i = 0; i < transList.Count; i++) {
				if (transList[i].TimeStamp>=fromDate && transList[i].TimeStamp<(toDate.AddDays(1)))     // we add 1 day to 'toDay' to also show transacton wich was made entire day 
				{
					int sign = transList [i].Type == TransactionType.Expense ? -1 : 1;
					String s = "Amount = " + Math.Round(sign*transList [i].Amount, 2) + "   Date = " + transList [i].TimeStamp.ToString("d");
					trlist.Add (s);
				}
			}
			mainActivity.UpdateList(trlist);
		}	 

		private static int CompareByDate(Transaction tr1, Transaction tr2)
		{
			if (tr1.TimeStamp > tr2.TimeStamp) {
				return -1;
			} else if (tr1.TimeStamp < tr2.TimeStamp) {
				return 1;
			} else {
				return 0;
			}
		}

		private int GenerateId()
		{
			return 0;
		}

		public DateTime GetOldestTransactionDate ()
		{
			DateTime d = transList [transList.Count-1].TimeStamp;
			return d;
		}
	}
}

