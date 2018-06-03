using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Schema;

using Fleck; // Websocket project and a namespace 

using System.Threading;
using System.Globalization;
using IBApi;

namespace TBR_noform
{
	public partial class Form1 : Form
	{
		// Thread
		public Thread emailThread;
		private Thread logThread; // A thread of Logging class. From this thread DB is searched for new messages

		// Websocket
		private List<IWebSocketConnection> allSockets; // The list of all connected clients to the websocket server
		
		// API variables
		private IBClient ibClient;
		private EReaderMonitorSignal signal;
		private ApiManager apiManager; // Api features like search, place order, getQute etc.

		// Flags
		private bool isConnected = false; // Connection flag. Prevents connect button click when connected
		bool conntectButtonFlag = true; // Turns to false when connect button is clicked

		// Unknown
		private string messageFromBrowser; // Received 
		private Response response; // Ticker search results class which holds a collection passed to the websocket connection stream as a message. This collection contains search resuls 
		
		private Contract contract; // Contract variable

		// Api response
		private SearchResponse searchResponse; // Search response json object
		private QuoteResponse quoteResponse; // Quote response json object
		private AvailableFundsResponse availableFundsResponse; // AvailableFundsResponse json object


		public Form1()
		{
			InitializeComponent();

			// listView1 setup
			/*
			listView1.View = View.Details;
			listView1.GridLines = true; // Horizoltal lines
			listView1.Columns.Add("Time:");
			listView1.Columns[0].Width = 60;
			listView1.Columns.Add("Source:", -2, HorizontalAlignment.Left);
			listView1.Columns.Add("Message:");
			listView1.Columns[2].Width = 400;
			*/

			// listView1 setup
			listView1.View = View.Details; // Shows the header
			listView1.FullRowSelect = true; // !!!Lets to select the whole row in the table!!!

			// listView2 setup
			listView2.View = View.Details;
			listView2.GridLines = true; // Horizoltal lines
			listView2.Columns.Add("Time:");
			listView2.Columns[0].Width = 60;
			listView2.Columns.Add("Source:", -2, HorizontalAlignment.Left);
			listView2.Columns.Add("Message:");
			listView2.Columns[2].Width = 400;

			// DB log messages
			Log.InitializeDB(); // Connect to the database 
			logThread = new Thread(new ThreadStart(LogThread)); // Make an instance of the thread and assign method name which will be executed in the thread

			// IB API instances
			signal = new EReaderMonitorSignal();
			ibClient = new IBClient(signal);
			apiManager = new ApiManager(ibClient);

			// Json search object class instance
			searchResponse = new SearchResponse();
			quoteResponse = new QuoteResponse();
			availableFundsResponse = new AvailableFundsResponse();

			// Fleck socket server 
			FleckLog.Level = LogLevel.Debug;
			allSockets = new List<IWebSocketConnection>();
			var server = new WebSocketServer("ws://0.0.0.0:8181");

			server.SupportedSubProtocols = new[] { "superchat", "chat" };
			server.Start(socket =>
			{
				socket.OnOpen = () =>
				{;
					Log.Insert(DateTime.Now, "Form1.cs", string.Format("Websocket connection open!"), "white");
					allSockets.Add(socket);
				};
				socket.OnClose = () =>
				{
					allSockets.Remove(socket);
				};
				socket.OnMessage = message =>
				{
					// Output message to system log
					Log.Insert(DateTime.Now, "Form1.cs", string.Format("socket.OnMessage. A message received from a client: {0}", message), "white");
					//allSockets.ToList().ForEach(s => s.Send("Hello from websocket! Form1.cs line 95")); // Send a greeting message to all websocket clients


					// DETERMINE WHAT TYPE OF API REQUEST IS RECEVIED HERE
					// apiManager.Search
					// apiManager.GetQuote

					var jsonObject = Newtonsoft.Json.Linq.JObject.Parse(message);
					var requestBody = jsonObject["body"];

					switch (jsonObject["requestType"].ToString())
					{
						case "symbolSearch":
							apiManager.Search(requestBody["symbol"].ToString()); // Works good
							break;
						case "getQuote":
							//quoteResponse.symbolName = requestBody["symbol"].ToString(); 
							apiManager.GetQuote(requestBody["symbol"].ToString(), (int)requestBody["basketNumber"], requestBody["currency"].ToString());
							break;
						case "getAvailableFunds":
							ibClient.ClientSocket.reqAccountUpdates(true, "U2314623");
							break;
					}
				};
			});

			// Events
			ibClient.CurrentTime += IbClient_CurrentTime; // Get exchnage current time 
			// ibClient.MarketDataType += IbClient_MarketDataType;
			ibClient.Error += IbClient_Error; // Errors handling

			ibClient.TickPrice += IbClient_TickPrice; // reqMarketData
			ibClient.NextValidId += IbClient_NextValidId; // Fires when api is connected (connect button clicked)
			//ibClient.OrderStatus += IbClient_OrderStatus; // Order status
			ibClient.ContractDetails += IbClient_ContractDetails; // Ticker search
			ibClient.ContractDetailsEnd += IbClient_ContractDetailsEnd; // Fires up when the the search response feed is finished. One search request can contain multiple contracts
			ibClient.UpdateAccountValue += IbClient_UpdateAccountValue; // Account info
		}

		private void IbClient_UpdateAccountValue(IBSampleApp.messages.AccountValueMessage obj) // Account info event. https://interactivebrokers.github.io/tws-api/interfaceIBApi_1_1EWrapper.html#ae15a34084d9f26f279abd0bdeab1b9b5
		{
			if (obj.Key == "AvailableFunds-S")
			{
				ListViewLog.AddRecord(this, "brokerListBox", "Form1.cs", "UpdateAccountValue: " + obj.Key + " " + obj.Value, "white");
				availableFundsResponse.availableFunds = Convert.ToDouble(obj.Value);
				foreach (var socket in allSockets.ToList()) // Loop through all connections/connected clients and send each of them a message
				{
					socket.Send(availableFundsResponse.ReturnJson());
				}
			}
			ibClient.ClientSocket.reqAccountUpdates(false, "U2314623"); // Unsubscribe. Otherwise on the next call it is not gonna work
		}

		private void IbClient_CurrentTime(long obj) // Get exchnage current time event
		{
			ListViewLog.AddRecord(this, "brokerListBox", "Form1.cs", "Exchange current time:" + UnixTimeStampToDateTime(obj).ToString(), "white");
		}
		private void IbClient_Error(int arg1, int arg2, string arg3, Exception arg4) // Errors handling event
		{
			ListViewLog.AddRecord(this, "brokerListBox", "Form1.cs", "IbClient_Error: arg 1,2,3: " + arg1 + " " + arg2 + " " + arg3 + "exception: " + arg4, "white");
		}

		private void IbClient_TickPrice(IBSampleApp.messages.TickPriceMessage obj) // reqMktData. Get quote
		{
			//ListViewLog.AddRecord(this, "brokerListBox", "Form1.cs", "TickPriceMessage. tick type: " + TickType.getField(msg.Field) + " price: " + msg.Price, "white");
			if (TickType.getField(obj.Field) == "delayedLast")
			{
				ListViewLog.AddRecord(this, "brokerListBox", "Form1.cs", "IbClient_TickPrice. price: " + obj.Price , "white");

				quoteResponse.symbolPrice = obj.Price;
				quoteResponse.symbolName = apiManager.symbolPass; // We have to store symbol name and basket number as apiManager fields. Symbol name is not returned with IbClient_TickPrice response as well as basket number. Then basket number will be returnet to php and passed as the parameter to Quote.php class where price field will be updated. Symbol name and basket number are the key
				quoteResponse.basketNum = apiManager.basketNumber; // Pass basket number to api manager. First basket number was assigned to a class field basketNumber of apiManager class 

				Console.WriteLine(quoteResponse.ReturnJson());

				foreach (var socket in allSockets.ToList()) // Loop through all connections/connected clients and send each of them a message
				{
					socket.Send(quoteResponse.ReturnJson());
				}
			}
		}

		private void IbClient_NextValidId(IBSampleApp.messages.ConnectionStatusMessage obj) // Api connection established
		{
			ListViewLog.AddRecord(this, "brokerListBox", "Form1.cs", "API connected: " + obj.IsConnected, "white");

			// 1 - Realtime, 2 - Frozen, 3 - Delayed data, 4 - Delayed frozen
			ibClient.ClientSocket.reqMarketDataType(3); // https://interactivebrokers.github.io/tws-api/classIBApi_1_1EClient.html#ae03b31bb2702ba519ed63c46455872b6 

			isConnected = true;
			if (obj.IsConnected)
			{
				status_CT.Text = "Connected";
				button13.Text = "Disconnect";
			}
			// 1 - Realtime, 2 - Frozen, 3 - Delayed data, 4 - Delayed frozen
			ibClient.ClientSocket.reqMarketDataType(3); // https://interactivebrokers.github.io/tws-api/classIBApi_1_1EClient.html#ae03b31bb2702ba519ed63c46455872b6 
		}

		private void IbClient_ContractDetails(IBSampleApp.messages.ContractDetailsMessage obj) // Ticker search event reqContractDetails
		{
			ListViewLog.AddRecord(this, "brokerListBox", "Form1.cs", "Ticker search: IbClient_ContractDetails: " +
				obj.ContractDetails.Summary.Exchange + " " +
				obj.ContractDetails.Summary.LocalSymbol + " " +
				obj.ContractDetails.Summary.SecType + " " +
				obj.ContractDetails.Summary.Currency + " " +
				obj.ContractDetails.Summary.Exchange + " " +
				obj.ContractDetails.Summary.PrimaryExch + " " +
				obj.ContractDetails.LongName, "white");

			// Add all returned values as the element to the collection which will be transformed to json 
			searchResponse.searchResponseList.Add(new SearchResponseString
			{
				symbol = obj.ContractDetails.Summary.Symbol,
				localSymbol = obj.ContractDetails.Summary.LocalSymbol,
				type = obj.ContractDetails.Summary.SecType,
				currency = obj.ContractDetails.Summary.Currency,
				exchange = obj.ContractDetails.Summary.Exchange,
				primaryExchnage = obj.ContractDetails.Summary.PrimaryExch,
				conId = obj.ContractDetails.Summary.ConId,
				longName = obj.ContractDetails.LongName
			});
		}

		private void IbClient_ContractDetailsEnd(int obj) // When a search response is finished 
		{
			ListViewLog.AddRecord(this, "brokerListBox", "Form1.cs", "IbClient_ContractDetailsEnd. Search response finished ", "white");
			foreach (var socket in allSockets.ToList()) // Loop through all connections/connected clients and send each of them a message
			{
				socket.Send(searchResponse.ReturnJson());
			}
			searchResponse.searchResponseList.Clear(); // Erase all elements after it was transmitted to the websocket connection 

			Console.WriteLine("contract end: " + searchResponse.ReturnJson()); // searchResponse
		}


		private void button1_Click(object sender, EventArgs e) // Start bot button click
		{

		}

		private void button2_Click(object sender, EventArgs e) // Stop bot button click
		{

		}

		private void button13_Click(object sender, EventArgs e) // Api coonect button click 
		{
			if (!isConnected) // False on startup
			{
				try
				{
					ibClient.ClientId = 2; // Client id. Multiple clients can be connected to the same gateway with the same login/password
					ibClient.ClientSocket.eConnect("127.0.0.1", Settings.ibGateWayPort, ibClient.ClientId);

					//Create a reader to consume messages from the TWS. The EReader will consume the incoming messages and put them in a queue
					var reader = new EReader(ibClient.ClientSocket, signal);
					reader.Start();

					//Once the messages are in the queue, an additional thread can be created to fetch them
					new Thread(() =>
					{ while (ibClient.ClientSocket.IsConnected()) { signal.waitForSignal(); reader.processMsgs(); } })
					{ IsBackground = true }.Start(); // https://interactivebrokers.github.io/tws-api/connection.html#gsc.tab=0
				}
				catch (Exception exception)
				{
					ListViewLog.AddRecord(this, "brokerListBox", "Form1.cs", "Check your connection credentials. Exception: " + exception, "white");
				}

				try
				{
					ibClient.ClientSocket.reqCurrentTime();
				}
				catch (Exception exception)
				{
					ListViewLog.AddRecord(this, "brokerListBox", "Form1.cs", "req time. Exception: " + exception, "white");
				}
			}
			else
			{
				isConnected = false;
				ibClient.ClientSocket.eDisconnect();
				status_CT.Text = "Disconnected";
				button13.Text = "Connect";
				ListViewLog.AddRecord(this, "brokerListBox", "Form1.cs", "Disconnecting..", "white");
			}
		}

		private static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
		{
			System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTimeStamp);
			return dtDateTime;
		}

		public void LogThread() // This method is called because a parameter can not be passed while invoking
		{
			while (true)
			{
				AddItemFromThread(this);
				Thread.Sleep(1000);
			}
		}

		private static void AddItemFromThread(Form1 form)
		{
			List<Log> logs = Log.GetNewLogs(); // Call a SQL QUERY method and assign it's result to logs list
			form.BeginInvoke(new Action(delegate ()
			{
				// DB watch. Get new records
				foreach (Log u in logs)
				{
					ListViewItem item = new ListViewItem(new string[] { u.Id.ToString(), u.Date.ToString(), u.Source, u.Message });
					item.Tag = u;
					form.listView1.Items.Add(item);
					form.listView1.Columns[2].Width = -1;
					form.listView1.Columns[3].Width = -1;
					form.listView1.Items[form.listView1.Items.Count - 1].EnsureVisible();
					
				}
				Console.WriteLine("------------------------------DB watch in progress! " + DateTime.Now);
			}));
		}

		private void button3_Click(object sender, EventArgs e) // Websocket test message send button
		{
			foreach (var socket in allSockets.ToList()) // Loop through all connections/connected clients and send each of them a message
			{
				socket.Send("Test message: " + textBox1.Text + " Is sent to: " + socket.ConnectionInfo.ClientIpAddress + " port: " + socket.ConnectionInfo.ClientPort + " origin: " + socket.ConnectionInfo.Origin + " id: " + socket.ConnectionInfo.Id + " message from the browser: ");
			}
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			logThread.IsBackground = true;
			logThread.Start(); // Read log records from DB thread
		}

		private void search_Button2_Click(object sender, EventArgs e) // Ticker search button click
		{
			apiManager.Search(textBox1.Text);
		}

		private void button12_Click(object sender, EventArgs e) // Get quote button click
		{
			/*
			contract = new Contract();
			contract.Symbol = textBox3.Text;
			contract.SecType = "STK";
			contract.Currency = "USD";
			//In the API side, NASDAQ is always defined as ISLAND in the exchange field
			contract.Exchange = "SMART"; // SMART ISLAND
			ibClient.ClientSocket.reqMktData((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds, contract, "", true, false, null); ; // Request market data for a contract https://interactivebrokers.github.io/tws-api/classIBApi_1_1EClient.html#a7a19258a3a2087c07c1c57b93f659b63
			*/

			apiManager.GetQuote("aapl", 1, "USD"); // Symbol, Basket number, currency
		}

		private void button4_Click(object sender, EventArgs e) // Get account availilble funds for trading button click 
		{
			ibClient.ClientSocket.reqAccountUpdates(true, "U2314623"); 
		}
	}
}
