using System;
using System.Collections.Generic;
using CM.Core.BusinessEntities;

namespace CM.Core {
	/// <summary>
	/// Manager classes are an abstraction on the data access layers
	/// </summary>
	/// TODO: add methods for other BusinessEntities
	public static class Manager {

		static Manager ()
		{
		}
		
		//Transaction Interface
		public static Transaction GetTransaction(int id)
		{
			return Database.GetTransaction(id);
		}
		
		public static IList<Transaction> GetTransactions ()
		{
			return new List<Transaction>(Database.GetTransactions());
		}
		
		public static int SaveTransaction (Transaction item)
		{
			return Database.SaveTransaction(item);
		}
		
		public static int DeleteTransaction(int id)
		{
			return Database.DeleteTransaction(id);
		}

		/* TODO
		//Account Interface
		public static Account GetAccount(int id)
		{
			return Database.GetAccount(id);
		}

		public static IList<Account> GetAccounts ()
		{
			return new List<Account>(Database.GetAccounts());
		}

		public static int SaveAccount (Account item)
		{
			return Database.SaveAccount(item);
		}

		public static int DeleteAccount(int id)
		{
			return Database.DeleteAccount(id);
		}

		//Category Interface
		public static Category GetCategory(int id)
		{
			return Database.GetCategory(id);
		}

		public static IList<Category> GetCategories ()
		{
			return new List<Category>(Database.GetCategories());
		}

		public static int SaveCategory (Category item)
		{
			return Database.SaveCategory(item);
		}

		public static int DeleteCategory(int id)
		{
			return Database.DeleteCategory(id);
		}

*/
	}
}