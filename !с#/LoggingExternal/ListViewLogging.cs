using System;
using System.Drawing;
//using System.Windows.Forms;

namespace tbr_c_sharp_server
{
	public class ListViewLogging
	{

		public static void log_add(Form1 form, string logTo, string log_source, string log_message, string color) // Add a record to the listBox window
		{
			DateTime time = DateTime.Now;

			if (logTo == "mainListBox")
			{
				/*

				form.BeginInvoke(new Action(delegate ()
				{
					form.listView1.Items.Add(time.ToString("MM.dd HH:mm.ss"));
					form.listView1.Items[form.listView1.Items.Count - 1].SubItems.Add(log_source);
					form.listView1.Items[form.listView1.Items.Count - 1].SubItems.Add(log_message);
					form.listView1.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent); // Make column autosized to its content
					form.listView1.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
					form.listView1.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent);


					switch (color) // расскарска строчек разными цветами
					{
						case "white":
							form.listView1.Items[form.listView1.Items.Count - 1].BackColor = Color.White;
							break;

						case "green":
							form.listView1.Items[form.listView1.Items.Count - 1].BackColor = Color.Chartreuse;
							break;

						case "yellow":
							form.listView1.Items[form.listView1.Items.Count - 1].BackColor = Color.Yellow;
							break;

						case "red":
							form.listView1.Items[form.listView1.Items.Count - 1].BackColor = Color.Red;
							break;
					}


					form.listView1.EnsureVisible(form.listView1.Items.Count - 1); // Auto scroll


					if (form.listView1.Items.Count.ToString() == "100") // Quantity of records in the window
					{
						form.listView1.Items.RemoveAt(0);
					}

				})); // form.BeginInvoke

			} // if

			/*

			if (logTo == "brokerListBox")
			{

				form.BeginInvoke(new Action(delegate ()
				{
					form.listView2.Items.Add(time.ToString("MM.dd HH:mm.ss"));
					form.listView2.Items[form.listView2.Items.Count - 1].SubItems.Add(log_source);
					form.listView2.Items[form.listView2.Items.Count - 1].SubItems.Add(log_message);
					form.listView2.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent); // Make column autosized to its content
					form.listView2.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);
					form.listView2.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent);


					switch (color) // расскарска строчек разными цветами
					{
						case "white":
							form.listView2.Items[form.listView2.Items.Count - 1].BackColor = Color.White;
							break;

						case "green":
							form.listView2.Items[form.listView2.Items.Count - 1].BackColor = Color.Chartreuse;
							break;

						case "yellow":
							form.listView2.Items[form.listView2.Items.Count - 1].BackColor = Color.Yellow;
							break;

						case "red":
							form.listView2.Items[form.listView2.Items.Count - 1].BackColor = Color.Red;
							break;
					}


					form.listView2.EnsureVisible(form.listView2.Items.Count - 1); // Auto scroll


					if (form.listView2.Items.Count.ToString() == "100") // Quantity of records in the window
					{
						form.listView2.Items.RemoveAt(0);
					}

				})); // form.BeginInvoke

			} // if

			
			if (logTo == "messageTypeListBox") // For messages type output 
			{

				form.BeginInvoke(new Action(delegate ()
				{
					form.listView3.Items.Add(time.ToString("MM.dd HH:mm.ss"));
					//form.listView3.Items[form.listView3.Items.Count - 1].SubItems.Add(log_source);
					form.listView3.Items[form.listView3.Items.Count - 1].SubItems.Add(log_message);
					form.listView3.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent); // Make column autosized to its content
					form.listView3.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);



					switch (color)
					{
						case "white":
							form.listView3.Items[form.listView3.Items.Count - 1].BackColor = Color.White;
							break;

						case "green":
							form.listView3.Items[form.listView3.Items.Count - 1].BackColor = Color.Chartreuse;
							break;

						case "yellow":
							form.listView3.Items[form.listView3.Items.Count - 1].BackColor = Color.Yellow;
							break;

						case "red":
							form.listView3.Items[form.listView3.Items.Count - 1].BackColor = Color.Red;
							break;
					}


					form.listView3.EnsureVisible(form.listView3.Items.Count - 1); // Auto scroll


					if (form.listView3.Items.Count.ToString() == "100") // Quantity of records in the window
					{
						form.listView3.Items.RemoveAt(0);
					}

				})); // form.BeginInvoke

			} // if
			*/


			
		}

	}
}
