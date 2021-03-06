﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Windows.Forms;

namespace TBR_form.Classes
{
	/**
	 * Logging class which offers messages retrival from MySql database.
	 * Class is used as a communication service provider for C# Visual Studio projects located in the same solution
	 * which can not be linked together due to circular link restriction. 
	 * This class can pe copied to other projects in order to send messages (insert) to the DB which will be read by the
	 * main class, putputed or used as triggers for business logic.
	 */
	class Log
	{
		private static MySqlConnection dbConn; // MySql connection 
		private static string connectionString; 

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
			Console.WriteLine("Log.cs InitializeDB()"); 
			InitializeDB(); 
		}

		public static void InitializeDB()
		{

			connectionString = "server=127.0.0.1;user id=slinger;password=659111;database=tut_db";
			dbConn = new MySqlConnection(connectionString);

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
			List<Log> logs = new List<Log>();

			using (var conn = new MySqlConnection(connectionString))
			{
				if (conn.State == System.Data.ConnectionState.Closed)
				{
					Console.WriteLine("Connection state: " + conn.State + " .Opening the connection!");
					conn.Open(); // If no connection to DB
				}
				else
				{
					Console.WriteLine("Connection state: " + conn.State + " .Connection open, no need to connect");
				}

				string sql = "SELECT * from logs WHERE is_new = '1'";
				MySqlCommand cmd2 = new MySqlCommand(sql, conn);
				cmd2.ExecuteNonQuery();

				
				MySqlDataReader reader = cmd2.ExecuteReader(); // Create reader and get all recoeds from DB

				while (reader.Read())
				{
					int id = (int)reader["id"]; // Type cast
					DateTime date = (DateTime)reader["date"]; // Cast it to integer. (int)reader["id"]
					string source = reader["source"].ToString();
					string message = reader["message"].ToString();
					string color = reader["color"].ToString();
					bool isNew = (bool)reader["is_new"]; // Type cast

					Console.WriteLine("zzz: " + (reader["is_new"].ToString()));

					if ((bool)reader["is_new"] == true)
					{
						// Read new record
						Log l = new Log(id, date, source, message, color, isNew); // Make a new instance of Log object type
						logs.Add(l); // Add it to the list. It will be outputed to the listview at the main form

						// Make a new connection
						var conn2 = new MySqlConnection(connectionString);
						string sql2 = string.Format("UPDATE `logs` SET `is_new` = '0' WHERE `logs`.`id` = {0}", id);
						conn2.Open();
						MySqlCommand cmd3 = new MySqlCommand(sql2, conn2);
						//cmd3.ExecuteNonQuery();
						conn2.Close();
					}

				}
				
				//conn.Close(); // No need to close the connection. It closes at the disposal 
			}

			return logs;
		}

		public static void Insert(DateTime d, String s, String m, string c) // Date, source, message
		{


			// https://dev.mysql.com/doc/connector-net/en/connector-net-tutorials-sql-command.html 

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

				string sql = "INSERT INTO logs(date, source, message) VALUES ('" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "', '" + s + "', '" + m + "' )";
				MySqlCommand cmd2 = new MySqlCommand(sql, conn);
				cmd2.ExecuteNonQuery();

				//conn.Close(); // No need to close the connection. It closes at the disposal 
			}

		}

	}
}
