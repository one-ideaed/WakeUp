using System;
using System.Collections.Generic;

namespace CM.Core {
	/// <summary>
	/// Manager classes are an abstraction on the data access layers
	/// </summary>
	public static class Manager {

		static Manager ()
		{
		}
		
		public static Task GetTask(int id)
		{
			return TaskRepositoryADO.GetTask(id);
		}
		
		public static IList<Task> GetTasks ()
		{
			return new List<Task>(TaskRepositoryADO.GetTasks());
		}
		
		public static int SaveTask (Task item)
		{
			return TaskRepositoryADO.SaveTask(item);
		}
		
		public static int DeleteTask(int id)
		{
			return TaskRepositoryADO.DeleteTask(id);
		}
	}
}