using System;
using System.Linq;
using System.Collections.Generic;

using Mono.Data.Sqlite;
using System.IO;
using System.Data;

namespace CM.Core
{
	public class Database 
	{
		static object locker = new object ();

		protected static SqliteConnection connection;

		protected static string dbPath;

		protected static Database db;

		static Database()
		{
			db = new Database ();
		}

		static string SelectTransactionStr="SELECT [_id], [TType], [Amount], [TSource],[Comment],[TimeStamp] from [Transactions]";
		static string SelectCategoryStr="";
		static string SelectAccountStr="";

		//create somewhere a number of strings with command texts to switch getItem,getItems,saveItem and deleteItem methods


		protected Database()
		{
			dbPath = dbFilePath;

			// create the tables
			bool exists = File.Exists (dbPath);

			if (!exists) {
				connection = new SqliteConnection ("Data Source=" + dbPath);

				connection.Open ();
				var commands = new[] {
					"CREATE TABLE [Transactions] (_id INTEGER PRIMARY KEY ASC AUTOINCREMENT, TType INTEGER, TSource INTEGER, Amount REAL,TimeStamp DATETIME, Comment NTEXT);"+
					"CREATE TABLE [Accounts](_id INTEGER PRIMARY KEY ASC AUTOINCREMENT, Balance REAL, Name varchar(50), Comment varchar(50))"+
					""
					//add other tables
				};
				foreach (var command in commands) {
					using (var c = connection.CreateCommand ()) {
						c.CommandText = command;
						var i = c.ExecuteNonQuery ();
					}
				}
			} else {
				// already exists, do nothing. 
			}
		}

		protected static string dbFilePath
		{
			get{ 
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

		private string DateTimeSQLite(DateTime datetime)
		{
			string dateTimeFormat = "{0}-{1}-{2} {3}:{4}:{5}.{6}";
			return string.Format(dateTimeFormat, datetime.Year,
				datetime.Month, datetime.Day, datetime.Hour, datetime.Minute, datetime.Second,datetime.Millisecond);
		}

		/// <summary>Convert from DataReader to Transaction object</summary>
		private Transaction  TransFromReader ( SqliteDataReader r) 
		{
			var t = new Transaction ();
			t.Id = Convert.ToInt32 (r ["_id"]);
			t.TransactionType = (TransactionType)r ["TType"];
			t.Amount = Convert.ToDouble( r ["Amount"]);
			t.DateTime =  (DateTime)r ["TimeStamp"];
			t.TransactionSource = (TransactionSource)r ["TSource"];
			t.Comment = r ["Comment"].toString ();
			return t;
		}

		public IEnumerable<Transaction> GetTransactions ()
		{
			var tl = new List<Transactions> ();

			lock (locker) {
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var contents = connection.CreateCommand ()) {
					contents.CommandText = SelectTransactionStr;
					var r = contents.ExecuteReader ();
					while (r.Read ()) {
						tl.Add (TransFromReader(r));
					}
				}
				connection.Close ();
			}
			return tl;
		}

		public object Gettransaction (int id) 
		{
			var t = new Transaction ();
			lock (locker) {
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var command = connection.CreateCommand ()) {
					command.CommandText = SelectTransactionStr+" WHERE [_id] = ?";
					command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = id });
					var r = command.ExecuteReader ();
					while (r.Read ()) {
						t = FromReader (r);
						break;
					}
				}
				connection.Close ();
			}
			return t;
		}

		public int SaveTransaction (Transaction item) 
		{
			int r;
			lock (locker) {
				if (item.ID != 0) {
					connection = new SqliteConnection ("Data Source=" + path);
					connection.Open ();
					using (var command = connection.CreateCommand ()) {
						command.CommandText = "UPDATE [Transactions] SET [TType] = ?, [Amount] = ?, [TSource] = ?, [Comment]=?, [TimeStamp]=? WHERE [_id] = ?;";
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = (int)item.TransactionType });
						command.Parameters.Add (new SqliteParameter (DbType.Real)  { Value = item.Amount });
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = (int)item.TransactionSource });
						command.Parameters.Add (new SqliteParameter (DbType.String) { Value=item.Comment});
						command.Parameters.Add (new SqliteParameter (DbType.DateTime) { Value=item.TimeStamp});
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.Id });
						r = command.ExecuteNonQuery ();
					}
					connection.Close ();
					return r;
				} else {
					connection = new SqliteConnection ("Data Source=" + path);
					connection.Open ();
					using (var command = connection.CreateCommand ()) {
						command.CommandText = "INSERT INTO [Transactions] ([TType], [Amount], [TSource],[Comment],[TimeStamp]) VALUES (? ,?, ?,?,?)";
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = (int)item.TransactionTypy });
						command.Parameters.Add (new SqliteParameter (DbType.Real) { Value = item.Amount });
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = (int)item.TransactionSource });
						command.Parameters.Add (new SqliteParameter (DbType.String) { Value = item.Comment });
						command.Parameters.Add (new SqliteParameter (DbType.DateTime) { Value = item.Comment });
						r = command.ExecuteNonQuery ();
					}
					connection.Close ();
					return r;
				}

			}
		}

		public int DeleteItem(int id) 
		{
			lock (locker) {
				int r;
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var command = connection.CreateCommand ()) {
					command.CommandText = "DELETE FROM [Items] WHERE [_id] = ?;";
					command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = id});
					r = command.ExecuteNonQuery ();
				}
				connection.Close ();
				return r;
			}
		}
	}
}