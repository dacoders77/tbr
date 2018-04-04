using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Globalization;


namespace Fleck
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
		private static MySqlConnection dbConn; // MySql connection 
		private static string connectionString;

		public static void Insert(DateTime d, String s, String m, string c) // Date, source, message, color
		{

			connectionString = "server=173.248.133.174;user id=slinger;password=659111;database=tut_db";
			dbConn = new MySqlConnection(connectionString);

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

				string sql = string.Format("INSERT INTO logs(date, source, message, color, is_new) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}' )", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"), s, m, "white", 1);
				
				MySqlCommand cmd2 = new MySqlCommand(sql, conn);
				cmd2.ExecuteNonQuery();

				//conn.Close(); // No need to close the connection. It closes at the disposal 
			}

		}

	}

}
