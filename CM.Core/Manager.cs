using System;
using System.Collections.Generic;

namespace CM.Core {
	/// <summary>
	/// Manager classes are an abstraction on the data access layers
	/// </summary>
	/// TODO: add methods for other BusinessEntities
	public static class Manager {

		static Manager ()
		{
		}
		
		public static Transaction GetTransaction(int id)
		{
			return Database.GetItem("Transaction",id);
		}
		
		public static IList<Task> GetTransactions ()
		{
			return new List<Task>(Database.GetItems("Transaction"));
		}
		
		public static int SaveTransaction (Transaction item)
		{
			return Database.SaveItem("Transaction",item);
		}
		
		public static int DeleteTransaction(int id)
		{
			return Database.DeleteItem("Transaction",id);
		}
	}
}