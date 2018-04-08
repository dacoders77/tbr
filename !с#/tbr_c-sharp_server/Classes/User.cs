using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

namespace tbr_c_sharp_server.Classes
{
	// https://www.youtube.com/watch?v=deRSq-Fb2BM&t=0s&list=PL404-abh17ryiAEfPxJnh8RurxbE2JwJ5&index=45 

	class User
	{
		// DB Credentials
		private const string SERVER = "127.0.0.1"; // 173.248.133.174
		private const string DATABASE = "tut_db";
		private const string UID = "slinger";
		private const string PASSWORD = "659111";
		private static MySqlConnection dbConn; // MySql connection 

		// User info
		public int Id { get; private set; }
		public string Username { get; private set; }
		public string Password { get; private set; }

		// Constructor
		public User(int id, string u, string p) {
			Id = id;
			Username = u;
			Password = p;

			InitializeDB();
		}

		public static void InitializeDB() {
			MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder();
			builder.Server = SERVER;
			builder.UserID = UID;
			builder.Password = PASSWORD;
			builder.Database = DATABASE;

			string connString = builder.ToString();
			Console.WriteLine(builder.ToString());
			builder = null;

			dbConn = new MySqlConnection(connString);

		}

		public static List<User> GetUsers() {
			List<User> users = new List<User>();

			string query = "SELECT * from users";

			MySqlCommand cmd = new MySqlCommand(query, dbConn);
			dbConn.Open();

			MySqlDataReader reader = cmd.ExecuteReader(); // Create reader and get all recoeds from DB
			while (reader.Read()) {
				int id = (int)reader["id"]; // Cast it to integer 
				string username = reader["username"].ToString();
				string password = reader["password"].ToString();

				User u = new User(id, username, password);
				users.Add(u);
			}

			dbConn.Close();
			
			return users;
		}

		public static User Insert(string u, string p) {
			return null;
		}

		public void Update(string u, string p){

		}

		public void Delete() {

		}


		}
}
