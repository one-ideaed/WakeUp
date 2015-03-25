﻿using System;

namespace CashflowMonitor.Core.BusinessEntities
{

	/*
	Transaction types
	*/
	public enum TransactionType: int {
		Income=1,
		Expense=-1,
		Reallocation=0
	}

	/*
	 * Transaction Source
	 */
	public enum TransactionSource:int{
		Manual=0,
		SMS=1
	}

	/*
	 * Category types
	 */
	public enum CategoryType:int{
		Income=1,
		Expense=-1
	}

	/*
	 * Elementary transaction class
	 */
	public class Transaction
	{
		public Transaction()
		{
		}
		//public Transaction(int id, TransactionType type, TransactionSource source, double amount, DateTime timestamp, string Comment)
		//{

	//	}
		public int Id{ get; set;}
		public TransactionType Type{ get; set;}
		//- Account.Id
		public TransactionSource Source{ get; set;}
		public double Amount{ get; set;}
		public DateTime TimeStamp{ get; set;}
		public string Comment{ get; set;}
		//- Counterparty.Id
	}

	/*
	 * Income/Expense Category class
	 */
	public class Category{
		public Category()
		{

		}
		public int Id{ get; set;}
		public String Name{ get; set;}
		public CategoryType Type{get;set;}
		public string Comment{ get; set;}
	}

	/*
	 * Account class
	 */
	public class Account{
		public Account()
		{

		}
		public int Id{ get; set;}
		public double Balance{ get; set;}
		public string Name{ get; set;}
		public string Comment{ get; set;}
	}
}


