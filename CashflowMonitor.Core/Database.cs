using System;
using System.Linq;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.IO;
using System.Data;
using CashflowMonitor.Core.BusinessEntities;
using Android.Util;

namespace CashflowMonitor.Core
{
	public class Database

	{

		static object locker=new object();
		protected static SqliteConnection connection;
		protected static string connectionString;
		protected static Database db;
		static Database()
		{

			db=new Database();

		}

		static string SelectTransactionStr ="SELECT[_id],[TType],[Amount],[TSource],[Comment],[TimeStamp]from[Transactions]";
		//static string SelectCategoryStr="";
		//static string SelectAccountStr="";

		//createsomewhereanumberofstringswithcommandtextstoswitchgetItem,getItems,saveItemanddeleteItemmethods

		protected Database()

		{
			connectionString="Data Source="+dbFilePath;
			//createthetables
			bool exists=File.Exists(dbFilePath);
			if(!exists){
				connection=new SqliteConnection(connectionString);
				connection.Open();
				var commands=new[]{
					"CREATE TABLE[Transactions](_id INTEGER PRIMARY KEY ASC AUTOINCREMENT,TType INTEGER,TSource INTEGER,Amount REAL,TimeStamp DATETIME,Comment NTEXT);"
//					"CREATE TABLE[Accounts](_id INTEGER PRIMARY KEY ASC AUTOINCREMENT,Balance REAL,Name varchar(50),Comment varchar(50))"+
					//addothertables
				};
				foreach(var command in commands){
					using(var c=connection.CreateCommand()){
						c.CommandText=command;
						var i=c.ExecuteNonQuery();
					}
				}

			}else{
				//alreadyexists,donothing.
			}
		}

		protected static string dbFilePath
		{

			get{
				var sqliteFilename="CashflowMonitorDB.db3";
				#if NETFX_CORE
				var path=Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path,sqliteFilename);
				#else
				#if SILVERLIGHT
				//WindowsPhoneexpectsalocalpath,notabsolute
				var path=sqliteFilename;
				#else
				#if __ANDROID__
				//JustusewhateverdirectorySpecialFolder.Personalreturns
				string libraryPath=Environment.GetFolderPath(Environment.SpecialFolder.Personal);
				#else
				//weneedtoputin/Library/oniOS5.1tomeetApple'siCloudterms
				//(theydon'twantnon-user-generateddatainDocuments)
				string documentsPath=Environment.GetFolderPath(Environment.SpecialFolder.Personal); //Documentsfolder
				string libraryPath=Path.Combine(documentsPath,"..","Library"); //Libraryfolder
			    #endif
				var path=Path.Combine(libraryPath,sqliteFilename);
				#endif
				#endif

				return path;	

			}

		}



		private string DateTimeSQLite(DateTime datetime)
		{
			string dateTimeFormat="{0}-{1}-{2}{3}:{4}:{5}.{6}";
			return string.Format(dateTimeFormat,datetime.Year,
				datetime.Month,datetime.Day,datetime.Hour,datetime.Minute,datetime.Second,datetime.Millisecond);
		}
		///<summary>ConvertfromDataReadertoTransactionobject</summary>
		private static Transaction TransFromReader(SqliteDataReader r)
		{
			var t=new Transaction();
			try
			{
				t.Id=Convert.ToInt32(r["_id"]);
				t.Type=(TransactionType)Enum.Parse(typeof(TransactionType),r["TType"].ToString());
				t.Amount=Convert.ToDouble(r["Amount"]);
				t.TimeStamp=DateTime.Parse(r["TimeStamp"].ToString());
				DateTime ttt=(DateTime)r["TimeStamp"];
				t.Source=(TransactionSource)Enum.Parse(typeof(TransactionSource),r["TSource"].ToString());
				t.Comment=r["Comment"].ToString();
			}
			catch (InvalidCastException e) 
			{
				Log.Error ("CashFlow", e.Message);
			}

			return t;

		}
			
		public static IEnumerable<Transaction> GetTransactions()
		{

			var tl=new List<Transaction>();

			lock(locker){
				connection=new SqliteConnection(connectionString);
				connection.Open();
				using(var contents=connection.CreateCommand()){
					contents.CommandText=SelectTransactionStr;
					var r=contents.ExecuteReader();
					while(r.Read()){
						tl.Add(TransFromReader(r));
					}
				}
				connection.Close();
			}
			return tl;
		}

		public static Transaction GetTransaction(int id)

		{

			var t=new Transaction();

			lock(locker){

				connection=new SqliteConnection(connectionString);

				connection.Open();

				using(var command=connection.CreateCommand()){

					command.CommandText=SelectTransactionStr+"WHERE[_id]=?";

					command.Parameters.Add(new SqliteParameter(DbType.Int32){Value=id});

					var r=command.ExecuteReader();

					while(r.Read()){

						t=TransFromReader(r);

						break;

					}

				}

				connection.Close();

			}

			return t;

		}



		public static int SaveTransaction(Transaction item)

		{

			int r;

			lock(locker){

				if(item.Id!=0){

					connection=new SqliteConnection(connectionString);

					connection.Open();

					using(var command=connection.CreateCommand()){

						command.CommandText="UPDATE[Transactions]SET[TType]=?,[Amount]=?,[TSource]=?,[Comment]=?,[TimeStamp]=?WHERE[_id]=?;";

						command.Parameters.Add(new SqliteParameter(DbType.Int32){Value=(int)item.Type});

						command.Parameters.Add(new SqliteParameter(DbType.Double){Value=item.Amount});

						command.Parameters.Add(new SqliteParameter(DbType.Int32){Value=(int)item.Source});

						command.Parameters.Add(new SqliteParameter(DbType.String){Value=item.Comment});

						command.Parameters.Add(new SqliteParameter(DbType.DateTime){Value=item.TimeStamp});

						command.Parameters.Add(new SqliteParameter(DbType.Int32){Value=item.Id});

						r=command.ExecuteNonQuery();

					}

					connection.Close();

					return r;

				}else{

					connection=new SqliteConnection(connectionString);

					connection.Open();

					using(var command=connection.CreateCommand()){

						command.CommandText="INSERT INTO[Transactions]([TType],[Amount],[TSource],[Comment],[TimeStamp])VALUES(?,?,?,?,?)";

						command.Parameters.Add(new SqliteParameter(DbType.Int32){Value=(int)item.Type});

						command.Parameters.Add(new SqliteParameter(DbType.Double){Value=item.Amount});

						command.Parameters.Add(new SqliteParameter(DbType.Int32){Value=(int)item.Source});

						command.Parameters.Add(new SqliteParameter(DbType.String){Value=item.Comment});

						command.Parameters.Add(new SqliteParameter(DbType.DateTime){Value=item.TimeStamp});

						r=command.ExecuteNonQuery();

					}

					connection.Close();

					return r;

				}

			}

		}



		public static int DeleteTransaction(int id)

		{

			lock(locker){

				int r;

				connection=new SqliteConnection(connectionString);

				connection.Open();

				using(var command=connection.CreateCommand()){

					command.CommandText="DELETE FROM[Transactions]WHERE[_id]=?;";

					command.Parameters.Add(new SqliteParameter(DbType.Int32){Value=id});

					r=command.ExecuteNonQuery();

				}

				connection.Close();

				return r;

			}

		}

	}

}