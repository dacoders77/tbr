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
		// Threads
		public Thread emailThread;
		private Thread logThread; // A thread of Logging class. From this thread DB is searched for new messages
		private Thread executeBasketThread; // Basket items search and execution thread. Searchs for baskets which need to be executed

		// Websocket
		private List<IWebSocketConnection> allSockets; // The list of all connected clients to the websocket server
		
		// IB API variables
		private IBClient ibClient;
		private EReaderMonitorSignal signal;
		internal ApiManager apiManager; // Api features like search, place order, getQute etc.

		// Flags
		private bool isConnected = false; // Connection flag. Prevents connect button click when connected
		bool conntectButtonFlag = true; // Turns to false when connect button is clicked

		// Unknown-delete
		private string messageFromBrowser; // Received 
		private Response response; // Ticker search results class which holds a collection passed to the websocket connection stream as a message. This collection contains search resuls 
		private Contract contract; // Contract variable

		// Api response
		private SearchResponse searchResponse; // Search response json object
		private QuoteResponse quoteResponse; // Quote response json object
		private AvailableFundsResponse availableFundsResponse; // AvailableFundsResponse json object

		// Other
		public Basket basket; // Baskets execution 


		public Form1()
		{
			InitializeComponent();

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

			// Basket thread
			executeBasketThread = new Thread(new ThreadStart(ExecuteBasketThread));

			// IB API instances
			signal = new EReaderMonitorSignal();
			ibClient = new IBClient(signal);
			apiManager = new ApiManager(ibClient, this);

			// Json search object class instance
			searchResponse = new SearchResponse();
			quoteResponse = new QuoteResponse();
			availableFundsResponse = new AvailableFundsResponse();

			// Fleck socket server 
			FleckLog.Level = LogLevel.Debug;
			allSockets = new List<IWebSocketConnection>();
			var server = new WebSocketServer("ws://0.0.0.0:8181");

			// Other
			basket = new Basket(this, ibClient);

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
							apiManager.Search(requestBody["symbol"].ToString());
							break;
						case "getQuote":
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

			ibClient.TickPrice += IbClient_TickPrice; // reqMarketData. EWrapper Interface
			ibClient.OrderStatus += IbClient_OrderStatus; // Status of a placed order

			ibClient.NextValidId += IbClient_NextValidId; // Fires when api is connected. Connection status received here
			ibClient.ContractDetails += IbClient_ContractDetails; // Ticker search
			ibClient.ContractDetailsEnd += IbClient_ContractDetailsEnd; // Fires up when the the search response feed is finished. One search request can contain multiple contracts
			ibClient.UpdateAccountValue += IbClient_UpdateAccountValue; // Account info
		}

		private void IbClient_OrderStatus(IBSampleApp.messages.OrderStatusMessage obj)
		{
			ListViewLog.AddRecord(this, "brokerListBox", "Form1.cs", "IbClient_OrderStatus. line 153. avgFillprice, filled, orderID, orderStatus: " + obj.AvgFillPrice + " | " + obj.Filled + " | " + obj.OrderId + " | " + obj.Status , "white");

			basket.UpdateInfoJsonExecuteOrder(string.Format("Order executed! Info text: {0}", obj.Status), "executeOrder", "ok", obj.OrderId, obj.AvgFillPrice, obj.Filled); // Update json info feild in DB
			
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

			if (arg4 != null) // Show exception if it is not null. There are errors with no exceptions
			MessageBox.Show(
				"Form1.cs line 173. IbClient_Error" +
				"link: " + arg4.HelpLink + "\r" +
				"result" + arg4.HResult + "\r" +
				"inner exception: " + arg4.InnerException + "\r" +
				"message: " + arg4.Message + "\r" +
				"source: " + arg4.Source + "\r" +
				"stack trace: " + arg4.StackTrace + "\r" +
				"target site: " + arg4.TargetSite + "\r"
				);

			// Must be carefull with these ticks! While debugging - disable this filter. Otherwise you can miss important information 
			// https://interactivebrokers.github.io/tws-api/message_codes.html
			// 2104 - A market data farm is connected.
			// 2108 - A market data farm connection has become inactive but should be available upon demand.
			// 2106 - A historical data farm is connected. 
			// 10167 - Requested market data is not subscribed. Displaying delayed market data
			// .. Not all codes are listed

			if (arg2 != 2104 && arg2 != 2119 && arg2 != 2108 && arg2 != 2106 && arg2 != 10167)
			//if (true)
			{
				// arg1 - requestId
				ListViewLog.AddRecord(this, "brokerListBox", "Form1.cs", "IbClient_Error: args: " + arg1 + " " + arg2 + " " + arg3 + "exception: " + arg4, "white");
				// id, code, text
				
				//basket.UpdateInfoJson(string.Format("Place order error! Error text: '{2}'. Error code: {1} RequestID: {0}", arg1, arg2, arg3), "placeOrder", "error", arg1); // Update json info feild in DB
				basket.UpdateInfoJson(string.Format("Place order error! Error text: {2} . Error code:{1}  RequestID: {0}. ibClient.NextOrderId: {3}", arg1, arg2, arg3, ibClient.NextOrderId), "placeOrder", "error", arg1); // Update json info feild in DB
			}
		}

		private void IbClient_TickPrice(IBSampleApp.messages.TickPriceMessage obj) // ReqMktData. Get quote. Tick types https://interactivebrokers.github.io/tws-api/rtd_simple_syntax.html 
		{
			char requestCode = obj.RequestId.ToString()[0]; // First char is the code. C# requests: 5 - fx, 6 - stock. PHP: 7 - stock

			// FX quote. C# while executing a basket
			// When a fx quote is received, ExecuteBasketThread() checks it and requests a stock quote
			if (TickType.getField(obj.Field) == "close") // bidPrice = -1. This value returned when market is closed. https://interactivebrokers.github.io/tws-api/md_receive.html
			{
				basket.assetForexQuote = obj.Price;
				ListViewLog.AddRecord(this, "brokerListBox", "Form1.cs line 210", "IbClient_TickPrice. FX Quote: " + obj.Price + " " + obj.RequestId, "yellow");

				basket.UpdateInfoJson(string.Format("FX quote successfully recevied. FX quote: {0}. RequestID: {1}", obj.Price, obj.RequestId), "fxQuoteRequest", "ok", obj.RequestId); // Update json info feild in DB
				basket.addForexQuoteToDB(obj.RequestId, obj.Price); // Update fx quote in the BD
			}

			// For stock. A request from PHP. While adding an asset to a basket
			// In this case we do not record this price to the DB. It is recorded from PHP
			if (TickType.getField(obj.Field) == "delayedLast" && requestCode.ToString() == "7") // PHP. Stock quote request
			{

				ListViewLog.AddRecord(this, "brokerListBox", "Form1.cs line 221", "IbClient_TickPrice. PHP req. price: " + obj.Price + "reqId: " + obj.RequestId, "white");

				quoteResponse.symbolPrice = obj.Price;
				quoteResponse.symbolName = apiManager.symbolPass; // We have to store symbol name and basket number as apiManager fields. Symbol name is not returned with IbClient_TickPrice response as well as basket number. Then basket number will be returnet to php and passed as the parameter to Quote.php class where price field will be updated. Symbol name and basket number are the key
				quoteResponse.basketNum = apiManager.basketNumber; // Pass basket number to api manager. First basket number was assigned to a class field basketNumber of apiManager class 

				foreach (var socket in allSockets.ToList()) // Loop through all connections/connected clients and send each of them a message
				{
					socket.Send(quoteResponse.ReturnJson());
				}
			}

			// C#. ApiManager stock quote request
			if (TickType.getField(obj.Field) == "delayedLast" && requestCode.ToString() == "6") 
			{
				// Updte quote value in DB
				basket.UpdateStockQuoteValue(obj.RequestId, obj.Price, this);
				ListViewLog.AddRecord(this, "brokerListBox", "Form1.cs", "IbClient_TickPrice. C# 6 req. price: " + obj.Price + " " + obj.RequestId, "white");

				basket.UpdateInfoJson(string.Format("Stock quote successfully recevied. Stock quote: {0}. RequestID: {1}", obj.Price, obj.RequestId), "stockQuoteRequest", "ok", obj.RequestId); // Update json info feild in DB
			}
		}

		private void IbClient_NextValidId(IBSampleApp.messages.ConnectionStatusMessage obj) // Api connection established
		{
			ListViewLog.AddRecord(this, "brokerListBox", "Form1.cs", "API connected: " + obj.IsConnected + " Next valid req id: " + ibClient.NextOrderId, "white");

			// 1 - Realtime, 2 - Frozen, 3 - Delayed data, 4 - Delayed frozen
			ibClient.ClientSocket.reqMarketDataType(3); // https://interactivebrokers.github.io/tws-api/classIBApi_1_1EClient.html#ae03b31bb2702ba519ed63c46455872b6 

			isConnected = true;
			if (obj.IsConnected)
			{
				status_CT.Text = "Connected";
				button13.Text = "Disconnect";
			}
			// 1 - Realtime, 2 - Frozen, 3 - Delayed data, 4 - Delayed frozen
			//ibClient.ClientSocket.reqMarketDataType(3); // https://interactivebrokers.github.io/tws-api/classIBApi_1_1EClient.html#ae03b31bb2702ba519ed63c46455872b6 
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

			//Console.WriteLine("contract end: " + searchResponse.ReturnJson()); // searchResponse
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

		public void ExecuteBasketThread() // Basket execution thread
		{
			while (true)
			{
				basket.Watch(); // Watch baskets table. When the time comes - execute all assets from this basket and set executed flag to 1 in the DB
				basket.WatchFxQuoteRow(); // Watch when a fx quote is received and added to assets table
				basket.WatchVolumeRow(); // Watch when a volume is calculated. When calculated - execute the asset
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
				//Console.WriteLine("Form1.cs line 349 ------------------------------DB watch in progress! " + DateTime.Now);
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

			executeBasketThread.IsBackground = true;
			executeBasketThread.Start();
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

			apiManager.GetQuote(textBox3.Text, 1, textBox4.Text); // Symbol, Basket number, currency
			//apiManager.GetForexQuote("EUR", "USD", 7894561);
		}

		private void button4_Click(object sender, EventArgs e) // Get account availilble funds for trading button click 
		{
			ibClient.ClientSocket.reqAccountUpdates(true, "U2314623"); 
		}

		private void label5_Click(object sender, EventArgs e)
		{
			listView2.Items.Clear();
		}
	}
}
