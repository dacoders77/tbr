using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Windows.Forms;


namespace TBR_noform
{
	/**
	 * Logging class which offers messages retrival from MySql database.
	 * Class is used as a communication service provider for C# Visual Studio projects located in the same solution
	 * which can not be linked together due to circular link restriction. 
	 * This class can pe copied to other projects in order to send messages (insert) to the DB which will be read by the
	 * main class, putputed or used as triggers for business logic.
	 */
	public class Log
	{
		private static MySqlConnection dbConn; // MySql connection variable
		private static string connectionString;
		private static string sqlQueryString;
		private static int basketId;

		public int Id { get; private set; }
		public DateTime Date { get; private set; }
		public string Source { get; private set; }
		public string Message { get; private set; }
		public string Color { get; private set; }
		public bool IsNew { get; private set; }

		// Class constructor
		public Log(int id, DateTime d, string s, string m, string c, bool i)
		{
			Id = id;
			Date = d;
			Source = s;
			Message = m;
			Color = c;
			IsNew = i;
			InitializeDB();
		}

		public static void InitializeDB()
		{
			//connectionString = "server=" + Settings.dbHost + ";user id=slinger;password=659111;database=tut_db";
			connectionString = "server=" + Settings.dbHost + ";user id=slinger;password=659111;database=tbr";
			dbConn = new MySqlConnection(connectionString);
			Console.WriteLine("Log.cs line 49. InitializeDB()");
		}

		public static List<Log> GetLogs() // List with custom class type - Log
		{
			List<Log> logs = new List<Log>();

			string query = "SELECT * from logs";

			MySqlCommand cmd = new MySqlCommand(query, dbConn);

			MySqlDataReader reader = cmd.ExecuteReader(); // Create reader and get all recoeds from DB
			while (reader.Read())
			{
				int id = (int)reader["id"]; // Type cast
				DateTime date = (DateTime)reader["date"]; // Cast it to integer. (int)reader["id"]
				string source = reader["source"].ToString();
				string message = reader["message"].ToString();
				string color = reader["color"].ToString();
				bool isNew = (bool)reader["is_new"]; // Type cast

				Log log = new Log(id, date, source, message, color, isNew);
				logs.Add(log);
			}

			return logs;
		}

		public static List<Log> GetNewLogs() // List with custom class type - DBLogging
		{
			List<Log> logs = new List<Log>(); // Created new empty List of custom type Log

			using (var mySqlConnection = new MySqlConnection(connectionString))
			{
				if (mySqlConnection.State == System.Data.ConnectionState.Closed)
				{
					//Console.WriteLine("Connection state: " + conn.State + " .Opening the connection!");
					//System.Windows.Forms.MessageBox.Show(connectionString);
					mySqlConnection.Open(); // If no connection to DB
				}
				else
				{
					//Console.WriteLine("Connection state: " + conn.State + " .Connection open, no need to connect");
				}

				sqlQueryString = "SELECT * from logs WHERE is_new = '1'";
				MySqlCommand cmd2 = new MySqlCommand(sqlQueryString, mySqlConnection);
				cmd2.ExecuteNonQuery();
				MySqlDataReader reader = cmd2.ExecuteReader(); // Create reader and get all recoeds from DB

				while (reader.Read())
				{
					int id = (int)reader["id"]; // Type cast. If after migration the Error: this type of conversion can not be done - go to phpMyAdmin and make it blank (no value). Value was UNSIGNED
					DateTime date = (DateTime)reader["date"]; // Cast it to integer. (int)reader["id"]
					string source = reader["source"].ToString();
					string message = reader["message"].ToString();
					string color = reader["color"].ToString();
					bool isNew = (bool)reader["is_new"]; // Type cast

					Console.WriteLine("log.cs line 110. reader['is_new']: " + (reader["is_new"].ToString()));

					if ((bool)reader["is_new"] == true)
					{
						// Read new record
						Log l = new Log(id, date, source, message, color, isNew); // Make a new instance of Log object type
						logs.Add(l); // Add it to the list. It will be outputed to the listview at the main form

						// Make a new connection and close it right after the query is executed
						var sqlConnectionUpdateRecord = new MySqlConnection(connectionString);
						sqlConnectionUpdateRecord.Open();
						sqlQueryString = string.Format("UPDATE `logs` SET `is_new` = '0' WHERE `logs`.`id` = {0}", id);
						MySqlCommand sqlCommandUpdateRecord = new MySqlCommand(sqlQueryString, sqlConnectionUpdateRecord);
						sqlCommandUpdateRecord.ExecuteNonQuery();
						sqlConnectionUpdateRecord.Close();
					}
				}
				reader.Close();

				// Baskets handle 

				// sqlQueryString = "SELECT * from baskets WHERE status = 'new'";
				sqlQueryString = "SELECT * from baskets";
				MySqlCommand cmd4 = new MySqlCommand(sqlQueryString, mySqlConnection); // Use the connection opened in the constructor
				cmd4.ExecuteNonQuery();
				MySqlDataReader readerBasketsTable = cmd4.ExecuteReader();
				

				while (readerBasketsTable.Read()) {

					// Calculate elapsed time and update eache record. Then it will be read with php and shown at the web form
					int id = (int)readerBasketsTable["id"]; // Get id of the record

					DateTime executionTime = (DateTime)readerBasketsTable["execution_time"];
					TimeSpan timeSpan = executionTime.Subtract(DateTime.Now);

					// Calculate elapsed_time only for not executed baskets
					// Calculate elapsed time and update eache record. Then it will be read with php and shown at the web form
					if (!(bool)readerBasketsTable["executed"]) {

						basketId = (int)readerBasketsTable["id"]; 

						var sqlConnectionUpdateRecord = new MySqlConnection(connectionString);
						sqlConnectionUpdateRecord.Open();
						sqlQueryString = string.Format("UPDATE `baskets` SET `elapsed_time` = '" + timeSpan.Days.ToString() + ":" + timeSpan.Hours.ToString() + ":" + timeSpan.Minutes.ToString() + ":" + timeSpan.Seconds.ToString() + "' WHERE `baskets`.`id` = {0}", id);
						MySqlCommand sqlCommandUpdateRecord = new MySqlCommand(sqlQueryString, sqlConnectionUpdateRecord);
						sqlCommandUpdateRecord.ExecuteNonQuery();
						sqlConnectionUpdateRecord.Close();
					}


					// Execution time is > current time. A basket needs to be executed
					if ((DateTime.Compare((DateTime)readerBasketsTable["execution_time"], DateTime.Now)) < 0 && !(bool)readerBasketsTable["executed"])
					{

						// Make a new reader. Assets table

						var sqlConnectionSelectAssets = new MySqlConnection(connectionString);
						sqlConnectionSelectAssets.Open();

						sqlQueryString = string.Format("SELECT * from assets WHERE basket_id = {0}", basketId); // (int)readerBasketsTable["id"]
						MySqlCommand cmd5 = new MySqlCommand(sqlQueryString, sqlConnectionSelectAssets); 
						cmd5.ExecuteNonQuery();
						MySqlDataReader readerAssetsTable = cmd5.ExecuteReader();

						while (readerAssetsTable.Read())
						{
							Console.WriteLine("x:" + readerAssetsTable["symbol"]);
						}

						sqlConnectionSelectAssets.Close();

						



						// Get all assets with basket_id = basket ID


						// Set executed flag to true
						var sqlConnectionUpdateRecord = new MySqlConnection(connectionString);
						sqlConnectionUpdateRecord.Open();
						sqlQueryString = string.Format("UPDATE `baskets` SET `executed` = '1', status='filled' WHERE `baskets`.`id` = {0}", id);
						MySqlCommand sqlCommandUpdateRecord = new MySqlCommand(sqlQueryString, sqlConnectionUpdateRecord);
						sqlCommandUpdateRecord.ExecuteNonQuery();
						sqlConnectionUpdateRecord.Close();


					

						// get basket it
						// make new sql request with this basket id

						// flag = true
						// loop through all basked id assets
						// first record
						// execute order (need to track timeout. if order freezes - need to catch it)
						// flag = false and continue looping
						// -- 
						// order filled
						// flag = true

						// update record
						// set executed flag to true(1)
					}

				}
				readerBasketsTable.Close();



				//Environment.Exit(1);
				
				//mySqlConnection.Close(); // No need to close the connection. It closes at the disposal 
			}

			return logs;
		}

		public static void Insert(DateTime d, String s, String m, string c) // Date, source, message, color
		{
			// CHECK iT? These lines are obsolete? 
			//string query = string.Format("INSERT INTO logs(date, source, message, color, is_new) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}' )", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), s, m, c, 1);
			//MySqlCommand cmd = new MySqlCommand(query, dbConn);

			// https://dev.mysql.com/doc/connector-net/en/connector-net-tutorials-sql-command.html 

			Console.WriteLine(s + " / " + m);

			using (var conn = new MySqlConnection(connectionString))
			{
				if (conn.State == System.Data.ConnectionState.Closed)
				{
					Console.WriteLine("Connection state: " + conn.State + "Opening connection!");
					conn.Open(); // If no connection to DB
				}
				else
				{
					Console.WriteLine("Connection state: " + conn.State + " Connection open, no need to connect");
				}

				string sql = string.Format("INSERT INTO logs(date, source, message, color, is_new) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}' )", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), s, m, "white", 1);
				MySqlCommand cmd2 = new MySqlCommand(sql, conn);
				cmd2.ExecuteNonQuery();

				//conn.Close(); // No need to close the connection. It closes at the disposal 
			}

		}

	}




}
