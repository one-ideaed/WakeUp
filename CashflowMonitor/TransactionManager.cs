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

		private IList<Transaction> translist;

		public TransactionManager ()
		{
			translist = null;
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
		}

		public ArrayAdapter ShowAll(Context context)
		{
			//Manager dbmanager = new Manager ();
			translist = Manager.GetTransactions ();
			String[] tv = new string[translist.Count];
			for (int i = 0; i < translist.Count; i++) {
				String s = "Amount = " + Math.Round(translist [i].Amount, 2) + "   Date = " + translist [i].TimeStamp.ToString();
				tv.SetValue (s, i);
			}
		    return new ArrayAdapter<String>(context, Android.Resource.Layout.SimpleListItemChecked,tv);
		}

		private int GenerateId()
		{
			return 0;
		}
	}
}

