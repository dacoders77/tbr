using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Windows.Forms;

namespace tbr_c_sharp_server.Classes
{
	class DBLogging
	{
		// DB Credentials
		private const string SERVER = "127.0.0.1";
		private const string DATABASE = "tut_db";
		private const string UID = "slinger";
		private const string PASSWORD = "659111";
		private static MySqlConnection dbConn; // MySql connection 

		// DB record feilds
		public DateTime Date { get; private set; }
		public string Source { get; private set; }
		public string Message { get; private set; }

		// Class constructor
		public DBLogging(DateTime d, string u, string p)
		{
			Date = d;
			Source = u;
			Message = p;

			Console.WriteLine("InitializeDB(); // ERROR goes from here");
			InitializeDB(); // ERROR goes from here
		}

		public static void InitializeDB()
		{
			MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
			builder.Server = SERVER;
			builder.UserID = UID;
			builder.Password = PASSWORD;
			builder.Database = DATABASE;

			string connString = builder.ToString();
			Console.WriteLine("Connection string :" + builder.ToString());
			builder = null;

			dbConn = new MySqlConnection(connString);

			DBConnectionOpen(); // DB Connection open 
		}

		public static void DBConnectionClose()
		{
			dbConn.Close();
		}

		public static void DBConnectionOpen()
		{
			dbConn.Open();
		}



		public static List<DBLogging> GetLogs() // List with custom class type - DBLogging
		{
			List<DBLogging> logs = new List<DBLogging>();

			string query = "SELECT * from logs";

			MySqlCommand cmd = new MySqlCommand(query, dbConn);
			//dbConn.Open();

			MySqlDataReader reader = cmd.ExecuteReader(); // Create reader and get all recoeds from DB
			while (reader.Read())
			{
				DateTime date = (DateTime)reader["date"]; // Cast it to integer. (int)reader["id"]
				string source = reader["source"].ToString();
				string message = reader["message"].ToString();

				DBLogging log = new DBLogging(date, source, message);
				logs.Add(log);
			}

			//dbConn.Close();

			return logs ;
		}



		public static List<DBLogging> GetNewLogs() // List with custom class type - DBLogging
		{
			List<DBLogging> logs = new List<DBLogging>();


			using (var conn = new MySqlConnection("server=127.0.0.1;user id=slinger;password=659111;database=tut_db"))
			{
				if (conn.State == System.Data.ConnectionState.Closed)
				{
					Console.WriteLine("Connection state: " + conn.State + " Opening connection!");
					conn.Open(); // If no connection to DB
				}
				else
				{
					Console.WriteLine("Connection state: " + conn.State + " Connection open, no need to connect");
				}

				string sql = "SELECT * from logs WHERE id = '5'";
				MySqlCommand cmd2 = new MySqlCommand(sql, conn);
				cmd2.ExecuteNonQuery();


				MySqlDataReader reader = cmd2.ExecuteReader(); // Create reader and get all recoeds from DB
				while (reader.Read())
				{
					DateTime date = (DateTime)reader["date"]; // Cast it to integer. (int)reader["id"]
					string source = reader["source"].ToString();
					string message = reader["message"].ToString();

					DBLogging l = new DBLogging(date, source, message);
					logs.Add(l);

					// get id of the current record 
					// update is_new flag from 1 to 0
					int id = (int)reader["id"]; // Type cast
				}

				//conn.Close(); // No need to close the connection. It closes at the disposal 
			}

			return logs;
		}



		public static DBLogging Insert(DateTime d, String s, String m) // Date, source, message
		{

			string query = string.Format("INSERT INTO logs(date, source, message) VALUES ('{0}', '{1}', '{2}' )", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), s, m);
			//string query = "INSERT INTO logs(date, source, message) VALUES ('" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" + s + "', '" + m + "' )";

			MySqlCommand cmd = new MySqlCommand(query, dbConn);


			//dbConn.Open();

			//MessageBox.Show("connection state: " + dbConn.State);

			//if (dbConn.State == System.Data.ConnectionState.Closed) // If no connection to DB
			//{
			//	Console.WriteLine("No DB connection! Connecting");
			//	DBConnectionOpen();
			//}

			// A. Panin https://dev.mysql.com/doc/connector-net/en/connector-net-tutorials-sql-command.html 
			using (var conn = new MySqlConnection("server=127.0.0.1;user id=slinger;password=659111;database=tut_db"))
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
				
				string sql = "INSERT INTO logs(date, source, message) VALUES ('" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" + s + "', '" + m + "' )";
				MySqlCommand cmd2 = new MySqlCommand(sql, conn);
				cmd2.ExecuteNonQuery();
				
				//conn.Close(); // No need to close the connection. It closes at the disposal 
			}


			//cmd.ExecuteNonQuery();


			int id = (int)cmd.LastInsertedId; // Id of the record which was inserted

			DateTime time = DateTime.Now;

			DBLogging log = new DBLogging(time, "x", "zz"); // The record will be inserted. The id of last inserted record will be assigned to currLog

			//dbConn.Close();

			return log;

		}


	}
}
