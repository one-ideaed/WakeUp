using System;
using System.Collections.Generic;
using System.IO;

namespace CM.Core {
	public class Repository {

		// no need any more. dbPath moved to Database class
		Database db = null;
		protected static string dbLocation;		
		protected static Repository me;		

		static Repository ()
		{
			me = new Repository();
		}

		protected Repository ()
		{
			// set the db location
			dbLocation = DatabaseFilePath;

			// instantiate the database	
			db = new Database(dbLocation);
		}

		public static string DatabaseFilePath {
			get { 
				var sqliteFilename = "CashflowMonitorDB.db3";
				#if NETFX_CORE
				var path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sqliteFilename);
				#else

				#if SILVERLIGHT
				// Windows Phone expects a local path, not absolute
				var path = sqliteFilename;
				#else

				#if __ANDROID__
				// Just use whatever directory SpecialFolder.Personal returns
				string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); ;
				#else
				// we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
				// (they don't want non-user-generated data in Documents)
				string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
				string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
				#endif
				var path = Path.Combine (libraryPath, sqliteFilename);
				#endif

				#endif
				return path;	
			}
		}

		public static Account GetAccount(int id)
		{
			return me.db.GetItem(id);
		}

		public static IEnumerable<Account> GetAccounts ()
		{
			return me.db.GetItems();
		}

		public static int SaveTask (Task item)
		{
			return me.db.SaveItem(item);
		}

		public static int DeleteTask(int id)
		{
			return me.db.DeleteItem(id);
		}
	}
}

