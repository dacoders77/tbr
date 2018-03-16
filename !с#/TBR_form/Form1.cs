using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TBR_form.Classes;
using System.Threading;

namespace TBR_form
{
	public partial class Form1 : Form
	{
		bool conntectButtonFlag = true; // Turns to false when clicked
		private Thread logThread; // Thread variable

		public Form1()
		{
			InitializeComponent();
			Log.InitializeDB();
			logThread = new Thread(new ThreadStart(LogThread)); // Make instance of the thread and assign method name which will be executed in the thread

			// listView1 setup
			listView1.View = View.Details; // Shows the header
			listView1.FullRowSelect = true; // !!!Lets to select the whole row in the table!!!

		}

		private void Form1_Load(object sender, EventArgs e) // Form load
		{
			logThread.SetApartmentState(ApartmentState.MTA);
			logThread.Start();
		}

		public void LogThread()
		{ // ANother method is called because a parameter can not be passed while invoking

			while (true)
			{
				AddItemFromThread(this);
				Thread.Sleep(500);
			} 
		}

		private static void AddItemFromThread(Form1 form)
		{

			form.BeginInvoke(new Action(delegate ()
			{
				// DB watch. Get new records
				List<Log> logs = Log.GetNewLogs(); // Call a SQL QUERY method and assign its result to logs

				foreach (Log u in logs)
				{

					ListViewItem item = new ListViewItem(new string[] { u.Date.ToString(), u.Source, u.Message });
					item.Tag = u;
					form.listView1.Items.Add(item);
					form.listView1.Items[form.listView1.Items.Count - 1].EnsureVisible();

				}

				Console.WriteLine("------------------------------DB watch in progress! " + DateTime.Now);

			}));

		}

		private void Form1_FormClosed(object sender, FormClosedEventArgs e) // Form close
		{
			logThread.Abort(); // Thread stop
		}

		private void label1_Click(object sender, EventArgs e) // Clear log
		{
			listView1.Items.Clear();
		}
	}
}
