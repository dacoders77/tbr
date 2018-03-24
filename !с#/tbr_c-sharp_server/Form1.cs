using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Fleck;
using System.Threading;

namespace tbr_c_sharp_server // tbr_c_sharp_server
{

	public partial class Form1 : Form
	{
		bool conntectButtonFlag = true; // Turns to false when clicked
		private Classes.User db;

		private Classes.User currUser; 

		private Thread connectThread; // Thread variable

		public Form1()
		{
			InitializeComponent();

			//Classes.User.InitializeDB();
			Classes.DBLogging.InitializeDB();

			connectThread = new Thread(new ThreadStart(ConnectThread)); // Make instance of the thread and assign method name which will be executed in the thread

			// listView1 setup

			listView1.View = View.Details; // Shows the header
			listView1.FullRowSelect = true; // !!!Lets to select the whole row in the table!!!
			/*
			listView1.GridLines = true; // Horizoltal lines
			listView1.Columns.Add("Time:");
			listView1.Columns[0].Width = 60;
			listView1.Columns.Add("Source:", -2, HorizontalAlignment.Left);
			listView1.Columns.Add("Message:");
			listView1.Columns[2].Width = 400;
			*/

			// Create fleck socket server 

			FleckLog.Level = LogLevel.Debug;
			var allSockets = new List<IWebSocketConnection>();

			var server = new WebSocketServer("ws://0.0.0.0:8181");

			server.SupportedSubProtocols = new[] { "superchat", "chat" };
			server.Start(socket =>
			{
				socket.OnOpen = () =>
				{
					//ListViewLogging.log_add(this, "mainListBox", "Form1.cs", "Websocket connection open!", "white");
					allSockets.Add(socket);
				};
				socket.OnClose = () =>
				{
					//ListViewLogging.log_add(this, "mainListBox", "Form1.cs", "Websocket connection closed!", "white");
					allSockets.Remove(socket);
				};
				socket.OnMessage = message =>
				{
					//ListViewLogging.log_add(this, "mainListBox", "Form1.cs onMessage", "Message: " + message, "yellow");
					// Send message back to websocket 
					allSockets.ToList().ForEach(s => s.Send("Echo. Hellow from c#: " + message));
				};
			});


			db = new Classes.User(1,"1","1");
			

		}

		private void button1_Click(object sender, EventArgs e) // Start socket server
		{
			if (conntectButtonFlag)
			{
				//ListViewLogging.log_add(this, "mainListBox", "Form1.cs", "Start buttocn clicked", "white");
				button1.Text = "Stop socket server";
				button1.BackColor = Color.Tomato;
				conntectButtonFlag = false;
				

			}
			else
			{
				button1.Text = "Stop socket server";
				//ListViewLogging.log_add(this, "mainListBox", "Form1.cs", "Stop buttocn clicked", "white");
				button1.BackColor = Color.Lime;
				conntectButtonFlag = true; 

			}
			
		}

		private void label1_Click(object sender, EventArgs e) // Clear log window 
		{
			listView1.Items.Clear();
		}

		// DB Actions
		private void LoadAll() { // Load all records from user table from DB

			List<Classes.User> users = Classes.User.GetUsers();
			listView1.Items.Clear();

			foreach (Classes.User u in users) {

				ListViewItem item = new ListViewItem(new string[] { u.Id.ToString(), u.Username, u.Password });
				item.Tag = u;
				listView1.Items.Add(item);

			}
		}

		private void LoadAllLogs()
		{ // Load all records from logs table from DB

			List<Classes.DBLogging> logs = Classes.DBLogging.GetLogs();
			listView1.Items.Clear();

			foreach (Classes.DBLogging u in logs)
			{

				ListViewItem item = new ListViewItem(new string[] { u.Date.ToString(), u.Source, u.Message });
				item.Tag = u;
				listView1.Items.Add(item);

			}
		}

		private void button8_Click(object sender, EventArgs e) // Load all records from logs table from DB
		{
			List<Classes.DBLogging> logs = Classes.DBLogging.GetLogs();
			listView1.Items.Clear();

			foreach (Classes.DBLogging l in logs)
			{

				ListViewItem item = new ListViewItem(new string[] { l.Date.ToString(), l.Source, l.Message });
				item.Tag = l;
				listView1.Items.Add(item);

			}

		}

		private void button3_Click(object sender, EventArgs e)
		{
			LoadAll(); // Lead all records from DB
		}

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listView1.SelectedItems.Count > 0)
			{
				ListViewItem item = listView1.SelectedItems[0];
				currUser = (Classes.User)item.Tag;

				int id = currUser.Id;
				String u = currUser.Username;
				String p = currUser.Password;

				txtUsername.Text = u;
				txtId.Text = id.ToString();
				txtPassword.Text = p;
			}
		}

		private void button7_Click(object sender, EventArgs e) // Thread start and whatch for new records in DB
		{
			
			connectThread.SetApartmentState(ApartmentState.MTA);
			connectThread.Start();
		}

		public void ConnectThread() { // ANother method is called because a parameter can not be passed while invoking

			while (true) {

				AddItemFromThread(this);
				Thread.Sleep(500);
				
			} // while
		}

		private static void AddItemFromThread(Form1 form) {

			form.BeginInvoke(new Action(delegate ()
			{
				// DB watch. Get new records

				List<Classes.DBLogging> logs = Classes.DBLogging.GetNewLogs(); // Call a SQL QUERY method and assign its result to logs

				foreach (Classes.DBLogging u in logs)
				{

					ListViewItem item = new ListViewItem(new string[] { u.Date.ToString(), u.Source, u.Message });
					item.Tag = u;
					form.listView1.Items.Add(item);

				}

				//ListViewItem item = new ListViewItem(new string[] { "1", "2", "3" });
				//item.Tag = u; // Error is thrown because there is not tag assigned
				//form.listView1.Items.Add(item);

				Console.WriteLine("------------------------------DB watch in progress!");
				
			}));

		}

		private void button9_Click(object sender, EventArgs e) // Insert record to logs table
		{
			//currUser = User.Insert(u, p);
			//currUser = User.Insert(u, p);
			Classes.DBLogging.Insert(DateTime.Now, "ss", "xx---kkk-9999");

			//LoadAllLogs();

		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e) // on From closed event
		{
			connectThread.Abort(); // Thread stop
		}
	}
}
