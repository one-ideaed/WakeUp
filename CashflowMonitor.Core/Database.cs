using System;
using System.Linq;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.IO;
using System.Data;
using CashflowMonitor.Core.BusinessEntities;

namespace CashflowMonitor.Core
{
	public class Database

	{

		static object locker=new object();
		protected static SqliteConnection connection;
		protected static string dbPath;
		protected static Database db;
		static Database()
		{

			db=new Database();

		}

		static string SelectTransactionStr ="SELECT[_id],[TType],[Amount],[TSource],[Comment],[TimeStamp]from[Transactions]";
		static string SelectCategoryStr="";
		static string SelectAccountStr="";

		//createsomewhereanumberofstringswithcommandtextstoswitchgetItem,getItems,saveItemanddeleteItemmethods

		protected Database()

		{
			dbPath=dbFilePath;
			//createthetables
			bool exists=File.Exists(dbPath);
			if(!exists){
				connection=new SqliteConnection("URI=file:data\\data\\com.iso.cashflowmonitor\\CashFlowDB.db3");
				connection.Open();
				var commands=new[]{
					"CREATETABLE[Transactions](_idINTEGERPRIMARYKEYASCAUTOINCREMENT,TTypeINTEGER,TSourceINTEGER,AmountREAL,TimeStampDATETIME,CommentNTEXT);"
//					"CREATETABLE[Accounts](_idINTEGERPRIMARYKEYASCAUTOINCREMENT,BalanceREAL,Namevarchar(50),Commentvarchar(50))"+
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
			t.Id=Convert.ToInt32(r["_id"]);
			t.Type=(TransactionType)r["TType"];
			t.Amount=Convert.ToDouble(r["Amount"]);
			t.TimeStamp=(DateTime)r["TimeStamp"];
			t.Source=(TransactionSource)r["TSource"];
			t.Comment=r["Comment"].ToString();
			return t;

		}
			
		public static IEnumerable<Transaction> GetTransactions()
		{

			var tl=new List<Transaction>();

			lock(locker){
				connection=new SqliteConnection("DataSource="+dbPath);
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

				connection=new SqliteConnection("DataSource="+dbPath);

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

					connection=new SqliteConnection("DataSource="+dbPath);

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

					connection=new SqliteConnection("DataSource="+dbPath);

					connection.Open();

					using(var command=connection.CreateCommand()){

						command.CommandText="INSERTINTO[Transactions]([TType],[Amount],[TSource],[Comment],[TimeStamp])VALUES(?,?,?,?,?)";

						command.Parameters.Add(new SqliteParameter(DbType.Int32){Value=(int)item.Type});

						command.Parameters.Add(new SqliteParameter(DbType.Double){Value=item.Amount});

						command.Parameters.Add(new SqliteParameter(DbType.Int32){Value=(int)item.Source});

						command.Parameters.Add(new SqliteParameter(DbType.String){Value=item.Comment});

						command.Parameters.Add(new SqliteParameter(DbType.DateTime){Value=item.Comment});

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

				connection=new SqliteConnection("DataSource="+dbPath);

				connection.Open();

				using(var command=connection.CreateCommand()){

					command.CommandText="DELETEFROM[Transactions]WHERE[_id]=?;";

					command.Parameters.Add(new SqliteParameter(DbType.Int32){Value=id});

					r=command.ExecuteNonQuery();

				}

				connection.Close();

				return r;

			}

		}

	}

}