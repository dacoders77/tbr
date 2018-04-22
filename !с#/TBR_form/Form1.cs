using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Fleck;

// IB API namespaces
using IBSampleApp;
using IBSampleApp.ui;
using IBApi;
using IBSampleApp.types;

using IBSampleApp.messages;



namespace TBR_form
{
	public partial class Form1 : Form
	{
		bool conntectButtonFlag = true; // Turns to false when connect button is clicked
		private Thread logThread; // Thread variable. Logging class. From this thread DB is searched for new messages
		private List<IWebSocketConnection> allSockets; // The list all connected to the socket server clients
		private string messageFromBrowser; // Received 

		// IB API
		delegate void MessageHandlerDelegate(IBMessage message);

		private MarketDataManager marketDataManager;
		private DeepBookManager deepBookManager;
		private HistoricalDataManager historicalDataManager;
		private RealTimeBarsManager realTimeBarManager;
		private ScannerManager scannerManager;
		private OrderManager orderManager;
		private AccountManager accountManager;
		private ContractManager contractManager;
		private AdvisorManager advisorManager;
		private OptionsManager optionsManager;
		private AcctPosMultiManager acctPosMultiManager;

		protected IBClient ibClient; // Instance of the main class 

		private bool isConnected = false;

		private const int MAX_LINES_IN_MESSAGE_BOX = 200;
		private const int REDUCED_LINES_IN_MESSAGE_BOX = 100;
		private int numberOfLinesInMessageBox = 0;
		private List<string> linesInMessageBox = new List<string>(MAX_LINES_IN_MESSAGE_BOX);

		private EReaderMonitorSignal signal = new EReaderMonitorSignal(); // From IBApi name space 

		private SearchJsonResponse searchJsonResponse; // Json search results object

		public Form1()
		{
			InitializeComponent();
			
			Log.InitializeDB();
			logThread = new Thread(new ThreadStart(LogThread)); // Make instance of the thread and assign method name which will be executed in the thread
			
			

			// listView1 setup
			listView1.View = View.Details; // Shows the header
			listView1.FullRowSelect = true; // !!!Lets to select the whole row in the table!!!

			// Create fleck socket server 
			FleckLog.Level = LogLevel.Debug;

			// Json search object class instance
			searchJsonResponse = new SearchJsonResponse();

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
					// Send message back to websocket 
					Log.Insert(DateTime.Now, "Form1.cs", string.Format("socket.OnMessage. A message received from client: {0}", message), "white");
					//allSockets.ToList().ForEach(s => s.Send("Hello from websocket!"));

					// Contract search. The same code as in searchContractDetails_Click button handler
					ShowTab(contractInfoTab, contractDetailsPage);
					Contract contract = GetConDetContract(); // Read form fields
					contract.Symbol = message;
					contractManager.RequestContractDetails(contract);

					

				};
			});


			// IB Client new instance
			ibClient = new IBClient(signal);

			// IB Variables declaraton 
			// Other features can be added and connected
			orderManager = new OrderManager(ibClient, liveOrdersGrid, tradeLogGrid);
			accountManager = new AccountManager(ibClient, accountSelector, accSummaryGrid, accountValuesGrid, accountPortfolioGrid, positionsGrid);
			contractManager = new ContractManager(ibClient, fundamentalsOutput, contractDetailsGrid); //ibClient, form tab, form tab. https://interactivebrokers.github.io/tws-api/contract_details.html#gsc.tab=0

			conDetRight.Items.AddRange(ContractRight.GetAll());
			conDetRight.SelectedIndex = 0;

			fundamentalsReportType.Items.AddRange(FundamentalsReport.GetAll());
			fundamentalsReportType.SelectedIndex = 0;

			this.groupMethod.DataSource = AllocationGroupMethod.GetAsData();
			this.groupMethod.ValueMember = "Value";
			this.groupMethod.DisplayMember = "Name";

			this.profileType.DataSource = AllocationProfileType.GetAsData();
			this.profileType.ValueMember = "Value";
			this.profileType.DisplayMember = "Name";

			hdRequest_EndTime.Text = DateTime.Now.ToString("yyyyMMdd HH:mm:ss");

			DateTime execFilterDefault = DateTime.Now.AddHours(-1);
			execFilterTime.Text = execFilterDefault.ToString("yyyyMMdd HH:mm:ss");

			// Events liniking
			// All events belong to EWrapper interface and called Public member functions
			ibClient.Error += ibClient_Error;
			ibClient.ConnectionClosed += ibClient_ConnectionClosed;
			ibClient.CurrentTime += time => addTextToBox("Current Time: " + time + "\n");
			ibClient.TickPrice += ibClient_TickPrice;
			ibClient.TickSize += ibClient_TickSize;
			ibClient.TickString += (tickerId, tickType, value) => addTextToBox("Tick string. Ticker Id:" + tickerId + ", Type: " + TickType.getField(tickType) + ", Value: " + value + "\n");
			ibClient.TickGeneric += (tickerId, field, value) => addTextToBox("Tick Generic. Ticker Id:" + tickerId + ", Field: " + TickType.getField(field) + ", Value: " + value + "\n");
			ibClient.TickEFP += (tickerId, tickType, basisPoints, formattedBasisPoints, impliedFuture, holdDays, futureLastTradeDate, dividendImpact, dividendsToLastTradeDate) => addTextToBox("TickEFP. " + tickerId + ", Type: " + tickType + ", BasisPoints: " + basisPoints + ", FormattedBasisPoints: " + formattedBasisPoints + ", ImpliedFuture: " + impliedFuture + ", HoldDays: " + holdDays + ", FutureLastTradeDate: " + futureLastTradeDate + ", DividendImpact: " + dividendImpact + ", DividendsToLastTradeDate: " + dividendsToLastTradeDate + "\n");
			ibClient.TickSnapshotEnd += tickerId => addTextToBox("TickSnapshotEnd: " + tickerId + "\n");
			ibClient.NextValidId += ibClient_NextValidId; // Receives next valid order id. Will be invoked automatically upon successfull API client connection. Used for sending connection status
			ibClient.DeltaNeutralValidation += (reqId, underComp) =>
			   addTextToBox("DeltaNeutralValidation. " + reqId + ", ConId: " + underComp.ConId + ", Delta: " + underComp.Delta + ", Price: " + underComp.Price + "\n");

			// Accounts
			ibClient.ManagedAccounts += accountsList => HandleMessage(new ManagedAccountsMessage(accountsList));

			// Options
			ibClient.TickOptionCommunication += (tickerId, field, impliedVolatility, delta, optPrice, pvDividend, gamma, vega, theta, undPrice) =>
				HandleMessage(new TickOptionMessage(tickerId, field, impliedVolatility, delta, optPrice, pvDividend, gamma, vega, theta, undPrice));

			// Account info, portfolio
			ibClient.AccountSummary += (reqId, account, tag, value, currency) => HandleMessage(new AccountSummaryMessage(reqId, account, tag, value, currency));
			ibClient.AccountSummaryEnd += reqId => HandleMessage(new AccountSummaryEndMessage(reqId));
			ibClient.UpdateAccountValue += (key, value, currency, accountName) => HandleMessage(new AccountValueMessage(key, value, currency, accountName));
			ibClient.UpdatePortfolio += (contract, position, marketPrice, marketValue, averageCost, unrealisedPNL, realisedPNL, accountName) =>
				HandleMessage(new UpdatePortfolioMessage(contract, position, marketPrice, marketValue, averageCost, unrealisedPNL, realisedPNL, accountName));
			ibClient.UpdateAccountTime += timestamp => HandleMessage(new UpdateAccountTimeMessage(timestamp));
			ibClient.AccountDownloadEnd += account => HandleMessage(new AccountDownloadEndMessage(account));
			ibClient.OrderStatus += (orderId, status, filled, remaining, avgFillPrice, permId, parentId, lastFillPrice, clientId, whyHeld) =>
				HandleMessage(new OrderStatusMessage(orderId, status, filled, remaining, avgFillPrice, permId, parentId, lastFillPrice, clientId, whyHeld));

			ibClient.OpenOrder += (orderId, contract, order, orderState) => HandleMessage(new OpenOrderMessage(orderId, contract, order, orderState));
			ibClient.OpenOrderEnd += () => HandleMessage(new OpenOrderEndMessage());

			// Contracts, comission, fundamential data, historical data
			ibClient.ContractDetails += (reqId, contractDetails) => HandleMessage(new ContractDetailsMessage(reqId, contractDetails));
			ibClient.ContractDetailsEnd += (reqId) => HandleMessage(new ContractDetailsEndMessage());
			ibClient.ExecDetails += (reqId, contract, execution) => HandleMessage(new ExecutionMessage(reqId, contract, execution));
			ibClient.ExecDetailsEnd += reqId => addTextToBox("ExecDetailsEnd. " + reqId + "\n");
			ibClient.CommissionReport += commissionReport => HandleMessage(new CommissionMessage(commissionReport));
			ibClient.FundamentalData += (reqId, data) => HandleMessage(new FundamentalsMessage(data));
			ibClient.HistoricalData += (reqId, date, open, high, low, close, volume, count, WAP, hasGaps) =>
				HandleMessage(new HistoricalDataMessage(reqId, date, open, high, low, close, volume, count, WAP, hasGaps));
			ibClient.HistoricalDataEnd += (reqId, startDate, endDate) => HandleMessage(new HistoricalDataEndMessage(reqId, startDate, endDate));
			ibClient.MarketDataType += (reqId, marketDataType) => addTextToBox("MarketDataType. " + reqId + ", Type: " + marketDataType + "\n");
			ibClient.UpdateMktDepth += (tickerId, position, operation, side, price, size) => HandleMessage(new DeepBookMessage(tickerId, position, operation, side, price, size, ""));
			ibClient.UpdateMktDepthL2 += (tickerId, position, marketMaker, operation, side, price, size) => HandleMessage(new DeepBookMessage(tickerId, position, operation, side, price, size, marketMaker));
			ibClient.UpdateNewsBulletin += (msgId, msgType, message, origExchange) =>
				addTextToBox("News Bulletins. " + msgId + " - Type: " + msgType + ", Message: " + message + ", Exchange of Origin: " + origExchange + "\n");

			// Positions
			ibClient.Position += (account, contract, pos, avgCost) => HandleMessage(new PositionMessage(account, contract, pos, avgCost));
			ibClient.PositionEnd += () => addTextToBox("PositionEnd \n");

			// Bars, scanners
			ibClient.RealtimeBar += (reqId, time, open, high, low, close, volume, WAP, count) => HandleMessage(new RealTimeBarMessage(reqId, time, open, high, low, close, volume, WAP, count));
			ibClient.ScannerParameters += xml => HandleMessage(new ScannerParametersMessage(xml));
			ibClient.ScannerData += (reqId, rank, contractDetails, distance, benchmark, projection, legsStr) =>
				HandleMessage(new ScannerMessage(reqId, rank, contractDetails, distance, benchmark, projection, legsStr));
			ibClient.ScannerDataEnd += reqId => addTextToBox("ScannerDataEnd. " + reqId + "\r\n");
			ibClient.ReceiveFA += (faDataType, faXmlData) => HandleMessage(new AdvisorDataMessage(faDataType, faXmlData));
			ibClient.BondContractDetails += (requestId, contractDetails) => addTextToBox("Receiving bond contract details.");
			ibClient.VerifyMessageAPI += apiData => addTextToBox("verifyMessageAPI: " + apiData);
			ibClient.VerifyCompleted += (isSuccessful, errorText) => addTextToBox("verifyCompleted. IsSuccessfule: " + isSuccessful + " - Error: " + errorText);
			ibClient.VerifyAndAuthMessageAPI += (apiData, xyzChallenge) => addTextToBox("verifyAndAuthMessageAPI: " + apiData + " " + xyzChallenge);
			ibClient.VerifyAndAuthCompleted += (isSuccessful, errorText) => addTextToBox("verifyAndAuthCompleted. IsSuccessfule: " + isSuccessful + " - Error: " + errorText);
			ibClient.DisplayGroupList += (reqId, groups) => addTextToBox("DisplayGroupList. Request: " + reqId + ", Groups" + groups);
			ibClient.DisplayGroupUpdated += (reqId, contractInfo) => addTextToBox("displayGroupUpdated. Request: " + reqId + ", ContractInfo: " + contractInfo);

			// Multi positions
			ibClient.PositionMulti += (reqId, account, modelCode, contract, pos, avgCost) => HandleMessage(new PositionMultiMessage(reqId, account, modelCode, contract, pos, avgCost));
			ibClient.PositionMultiEnd += (reqId) => HandleMessage(new PositionMultiEndMessage(reqId));
			ibClient.AccountUpdateMulti += (reqId, account, modelCode, key, value, currency) => HandleMessage(new AccountUpdateMultiMessage(reqId, account, modelCode, key, value, currency));
			ibClient.AccountUpdateMultiEnd += (reqId) => HandleMessage(new AccountUpdateMultiEndMessage(reqId));
			ibClient.SecurityDefinitionOptionParameter += (reqId, exchange, underlyingConId, tradingClass, multiplier, expirations, strikes) => HandleMessage(new SecurityDefinitionOptionParameterMessage(reqId, exchange, underlyingConId, tradingClass, multiplier, expirations, strikes));
			ibClient.SecurityDefinitionOptionParameterEnd += (reqId) => HandleMessage(new SecurityDefinitionOptionParameterEndMessage(reqId));

			// Soft dollar tires
			ibClient.SoftDollarTiers += (reqId, tiers) => HandleMessage(new SoftDollarTiersMessage(reqId, tiers));

		}

		private void Form1_Load(object sender, EventArgs e) // Form load
		{
			//logThread.SetApartmentState(ApartmentState.MTA);
			logThread.IsBackground = true;
			logThread.Start(); // Reading log records from DB
		}

		public void LogThread() // ANother method is called because a parameter can not be passed while invoking
		{ 
			
			while (true)
			{
				AddItemFromThread(this);
				Thread.Sleep(2000);
			} 
			
		}

		private static void AddItemFromThread(Form1 form)
		{
			List<Log> logs = Log.GetNewLogs(); // Call SQL QUERY method and assign its result to logs list

			form.BeginInvoke(new Action(delegate ()
			{
				// DB watch. Get new records
				

				foreach (Log u in logs)
				{

					ListViewItem item = new ListViewItem(new string[] {u.Id.ToString(), u.Date.ToString(), u.Source, u.Message });
					item.Tag = u;
					form.listView1.Items.Add(item);
					form.listView1.Columns[2].Width = -1;
					form.listView1.Columns[3].Width = -1;
					form.listView1.Items[form.listView1.Items.Count - 1].EnsureVisible();

				}

				//Console.WriteLine("------------------------------DB watch in progress! " + DateTime.Now);

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

		private void listView1_SelectedIndexChanged(object sender, EventArgs e)
		{
			
		}

		private void button2_Click(object sender, EventArgs e) // Send message
		{

			foreach (var socket in allSockets.ToList()) // Loop through all connections/connected clients and send each of them a message
			{
				socket.Send("Test message: " + textBox1.Text + " Is sent to: " + socket.ConnectionInfo.ClientIpAddress + " port: " + socket.ConnectionInfo.ClientPort + " origin: " + socket.ConnectionInfo.Origin + " id: " + socket.ConnectionInfo.Id + " message from the browser: ");
			}

		}

		// IB HANDLERS AND FORM CONTROLS

		void ibClient_NextValidId(int orderId)
		{
			isConnected = true; 
			HandleMessage(new ConnectionStatusMessage(true));
		}

		void ibClient_TickSize(int tickerId, int field, int size)
		{
			addTextToBox("Tick Size. Ticker Id:" + tickerId + ", Type: " + TickType.getField(field) + ", Size: " + size + "\n");
			HandleMessage(new TickSizeMessage(tickerId, field, size));
		}

		void ibClient_TickPrice(int tickerId, int field, double price, int canAutoExecute)
		{
			addTextToBox("Tick Price. Ticker Id:" + tickerId + ", Type: " + TickType.getField(field) + ", Price: " + price + "\n");
			HandleMessage(new TickPriceMessage(tickerId, field, price, canAutoExecute));
		}

		void ibClient_ConnectionClosed()
		{

			IsConnected = false;
			HandleMessage(new ConnectionStatusMessage(false));
		}

		void ibClient_Error(int id, int errorCode, string str, Exception ex)
		{

			if (ex != null)
			{
				addTextToBox("Error: " + ex);

				return;
			}

			if (id == 0 || errorCode == 0)
			{
				addTextToBox("Error: " + str + "\n");

				return;
			}

			HandleMessage(new ErrorMessage(id, errorCode, str));
		}

		private void addTextToBox(string text)
		{
			HandleMessage(new ErrorMessage(-1, -1, text));
		}

		public bool IsConnected
		{
			get { return isConnected; }
			set { isConnected = value; }
		}

		public void HandleMessage(IBMessage message)
		{
			if (this.InvokeRequired)
			{
				MessageHandlerDelegate callback = new MessageHandlerDelegate(HandleMessage);
				this.Invoke(callback, new object[] { message });
			}
			else
			{
				UpdateUI(message);
			}
		}

		private void UpdateUI(IBMessage message)
		{

			ShowMessageOnPanel("(UpdateUI) Message type: " + message.Type.ToString());

			switch (message.Type)
			{
				case MessageType.ConnectionStatus:
					{
						ConnectionStatusMessage statusMessage = (ConnectionStatusMessage)message;
						if (statusMessage.IsConnected)
						{
							status_CT.Text = "Connected! Your client Id: " + ibClient.ClientId;
							connectButton.Text = "Disconnect";
						}
						else
						{
							status_CT.Text = "Disconnected...";
							connectButton.Text = "Connect";
						}
						break;
					}
				case MessageType.Error:
					{
						ErrorMessage error = (ErrorMessage)message;
						ShowMessageOnPanel("Request " + error.RequestId + ", Code: " + error.ErrorCode + " - " + error.Message);
						HandleErrorMessage(error);
						break;
					}
				case MessageType.TickOptionComputation:
				case MessageType.TickPrice:
				case MessageType.TickSize:
					{
						HandleTickMessage((MarketDataMessage)message);
						break;
					}
				case MessageType.MarketDepth:
				case MessageType.MarketDepthL2:
					{
						deepBookManager.UpdateUI(message);
						break;
					}
				case MessageType.HistoricalData:
				case MessageType.HistoricalDataEnd:
					{
						historicalDataManager.UpdateUI(message);
						break;
					}
				case MessageType.RealTimeBars:
					{
						realTimeBarManager.UpdateUI(message);
						break;
					}
				case MessageType.ScannerData:
				case MessageType.ScannerParameters:
					{
						scannerManager.UpdateUI(message);
						break;
					}
				case MessageType.OpenOrder:
				case MessageType.OpenOrderEnd:
				case MessageType.OrderStatus:
				case MessageType.ExecutionData:
				case MessageType.CommissionsReport:
					{
						orderManager.UpdateUI(message);
						break;
					}
				case MessageType.ManagedAccounts:
					{
						orderManager.ManagedAccounts = ((ManagedAccountsMessage)message).ManagedAccounts;
						accountManager.ManagedAccounts = ((ManagedAccountsMessage)message).ManagedAccounts;
						exerciseAccount.Items.AddRange(((ManagedAccountsMessage)message).ManagedAccounts.ToArray());
						break;
					}
				case MessageType.AccountSummaryEnd:
					{
						accSummaryRequest.Text = "Request";
						accountManager.UpdateUI(message);
						break;
					}
				case MessageType.AccountDownloadEnd:
					{
						break;
					}
				case MessageType.AccountUpdateTime:
					{
						accUpdatesLastUpdateValue.Text = ((UpdateAccountTimeMessage)message).Timestamp;
						break;
					}
				case MessageType.PortfolioValue:
					{
						accountManager.UpdateUI(message);
						if (exerciseAccount.SelectedItem != null)
							optionsManager.HandlePosition((UpdatePortfolioMessage)message);
						break;
					}
				case MessageType.AccountSummary:
				case MessageType.AccountValue:
				case MessageType.Position:
				case MessageType.PositionEnd:
					{
						accountManager.UpdateUI(message);
						break;
					}
				case MessageType.ContractDataEnd:
					{
						searchContractDetails.Enabled = true;
						contractManager.UpdateUI(message);

						foreach (var socket in allSockets.ToList()) // Loop through all connections/connected clients and send each of them a message
						{
							socket.Send(searchJsonResponse.Test());
						}

						// Send json object
						// After it is sent .clear it!
						//MessageBox.Show(searchJsonResponse.Test());
						searchJsonResponse.Clear(); // Collection empty

						break;
					}
				case MessageType.ContractData:
					{
						HandleContractDataMessage((ContractDetailsMessage)message);
						//MessageBox.Show("contract_start");

						foreach (var socket in allSockets.ToList()) // Loop through all connections/connected clients and send each of them a message
						{
							//var castedMessage = (ContractDetailsMessage)message;
							//ShowMessageOnPanel("SYMBOL: " + castedMessage.ContractDetails.Summary.Symbol + " successfully found at: " + castedMessage.ContractDetails.Summary.PrimaryExch + " exchange");
							//socket.Send("SYMBOL: " + castedMessage.ContractDetails.Summary.Symbol + " successfully found at: " + castedMessage.ContractDetails.Summary.Exchange + " exchange");
						}

						// Add new object to json array
						var z = (ContractDetailsMessage)message; // Type cast
						var x = z.ContractDetails.Summary;
						searchJsonResponse.Add(x.Exchange, x.LocalSymbol, x.SecType, x.Currency, x.Exchange, x.PrimaryExch, x.ConId);
						//Log.Insert(DateTime.Now, "Form1.cs", string.Format("ContractData: {0} {1} {2} {3} {4} {5} {6}", x.Exchange, x.LocalSymbol, x.SecType, x.Currency, x.Exchange, x.PrimaryExch, x.ConId), "white");

						break;
					}
				case MessageType.FundamentalData:
					{
						fundamentalsQueryButton.Enabled = true;
						contractManager.UpdateUI(message);
						break;
					}
				case MessageType.ReceiveFA:
					{
						advisorManager.UpdateUI((AdvisorDataMessage)message);
						break;
					}
				case MessageType.PositionMulti:
				case MessageType.AccountUpdateMulti:
				case MessageType.PositionMultiEnd:
				case MessageType.AccountUpdateMultiEnd:
					{
						acctPosMultiManager.UpdateUI(message);
						break;
					}

				case MessageType.SecurityDefinitionOptionParameter:
				case MessageType.SecurityDefinitionOptionParameterEnd:
					{
						optionsManager.UpdateUI(message);
						break;
					}
				case MessageType.SoftDollarTiers:
					{
						orderManager.UpdateUI(message);
						break;
					}

				default:
					{
						HandleMessage(new ErrorMessage(-1, -1, message.ToString()));
						break;
					}
			}
		} // private void UpdateUI

		private void HandleTickMessage(MarketDataMessage tickMessage)
		{
			if (tickMessage.RequestId < OptionsManager.OPTIONS_ID_BASE)
			{
				marketDataManager.UpdateUI(tickMessage);
			}
			else
			{
				if (!queryOptionChain.Enabled)
				{
					queryOptionChain.Enabled = true;
				}
				optionsManager.UpdateUI(tickMessage);
			}

		}

		private void HandleContractDataMessage(ContractDetailsMessage message)
		{
			if (message.RequestId > ContractManager.CONTRACT_ID_BASE && message.RequestId < OptionsManager.OPTIONS_ID_BASE)
			{
				contractManager.UpdateUI(message);
			}
			else if (message.RequestId >= OptionsManager.OPTIONS_ID_BASE)
			{
				optionsManager.UpdateUI(message);
			}
		}

		private void HandleErrorMessage(ErrorMessage message)
		{
			if (message.RequestId > MarketDataManager.TICK_ID_BASE && message.RequestId < DeepBookManager.TICK_ID_BASE)
				marketDataManager.NotifyError(message.RequestId);
			else if (message.RequestId > DeepBookManager.TICK_ID_BASE && message.RequestId < HistoricalDataManager.HISTORICAL_ID_BASE)
				deepBookManager.NotifyError(message.RequestId);
			else if (message.RequestId == ContractManager.CONTRACT_DETAILS_ID)
			{
				contractManager.HandleRequestError(message.RequestId);
				searchContractDetails.Enabled = true;
			}
			else if (message.RequestId == ContractManager.FUNDAMENTALS_ID)
			{
				contractManager.HandleRequestError(message.RequestId);
				fundamentalsQueryButton.Enabled = true;
			}
			else if (message.RequestId == OptionsManager.OPTIONS_ID_BASE)
			{
				optionsManager.Clear();
				queryOptionChain.Enabled = true;
			}
			else if (message.RequestId > OptionsManager.OPTIONS_ID_BASE)
			{
				queryOptionChain.Enabled = true;
			}
			if (message.ErrorCode == 202)
			{
			}
		}

		private void connectButton_Click(object sender, EventArgs e) // Connect button
		{


			if (!IsConnected)
			{
				int port;
				string host = null; // this.host_CT.Text;

				if (host == null || host.Equals(""))
					host = "127.0.0.1";
				try
				{
					port = 4002; // Int32.Parse(this.port_CT.Text);
					ibClient.ClientId = 2; // Int32.Parse(this.clientid_CT.Text);
					ibClient.ClientSocket.eConnect(host, port, ibClient.ClientId);

					var reader = new EReader(ibClient.ClientSocket, signal);

					reader.Start();

					new Thread(() => { // Create object
						while (ibClient.ClientSocket.IsConnected()) { // Params. Lambda expression  
							signal.waitForSignal();
							reader.processMsgs();
						}
					}) // Create
					{

						IsBackground = true

					}.Start(); // Start method call
				}

				catch (Exception)
				{

					HandleMessage(new ErrorMessage(-1, -1, "jopa Please check your connection attributes."));
				}
			}
			else
			{
				IsConnected = false;
				ibClient.ClientSocket.eDisconnect(); // Fiers ibClient.ConnectionClose event

			}
		}


		
		private void marketData_Click(object sender, EventArgs e)
		{
			if (isConnected)
			{
				Contract contract = GetMDContract();
				string genericTickList = this.genericTickList.Text;
				if (genericTickList == null)
					genericTickList = "";
				marketDataManager.AddRequest(contract, genericTickList);
				ShowTab(marketData_MDT, topMarketDataTab_MDT);
			}
		}

		private void closeMketDataTab_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			marketDataManager.StopActiveRequests(true);
			this.marketData_MDT.TabPages.Remove(topMarketDataTab_MDT);
		}

		private void deepBook_Click(object sender, EventArgs e)
		{
			if (isConnected)
			{
				Contract contract = GetMDContract();
				deepBookManager.AddRequest(contract, Int32.Parse(deepBookEntries.Text));
				//deepBookTab_MDT.Text = Utils.ContractToString(contract) + " (Book)";
				ShowTab(marketData_MDT, deepBookTab_MDT);
			}
		}

		private void closeDeepBookLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			deepBookManager.StopActiveRequests();
			deepBookTab_MDT.Text = "";
			this.marketData_MDT.TabPages.Remove(deepBookTab_MDT);
		}

		private void histDataButton_Click(object sender, EventArgs e)
		{
			if (isConnected)
			{
				Contract contract = GetMDContract();
				string endTime = hdRequest_EndTime.Text.Trim();
				string duration = hdRequest_Duration.Text.Trim() + " " + hdRequest_TimeUnit.Text.Trim();
				string barSize = hdRequest_BarSize.Text.Trim();
				string whatToShow = hdRequest_WhatToShow.Text.Trim();
				int outsideRTH = this.contractMDRTH.Checked ? 1 : 0;
				historicalDataManager.AddRequest(contract, endTime, duration, barSize, whatToShow, outsideRTH, 1);
				//historicalDataTab.Text = Utils.ContractToString(contract) + " (HD)";
				ShowTab(marketData_MDT, historicalDataTab);
			}
		}

		private void histDataTabClose_MDT_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			this.marketData_MDT.TabPages.Remove(historicalDataTab);
		}

		private void realTime_Button_Click(object sender, EventArgs e)
		{
			if (isConnected)
			{
				Contract contract = GetMDContract();
				string whatToShow = hdRequest_WhatToShow.Text.Trim();
				realTimeBarManager.AddRequest(contract, whatToShow, true);
				//rtBarsTab_MDT.Text = Utils.ContractToString(contract) + " (RTB)";
				ShowTab(marketData_MDT, rtBarsTab_MDT);
			}
		}

		private void rtBarsCloseLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			realTimeBarManager.Clear();
			this.marketData_MDT.TabPages.Remove(rtBarsTab_MDT);
		}

		private void scannerRequest_Button_Click(object sender, EventArgs e)
		{
			if (isConnected)
			{
				ScannerSubscription subscription = new ScannerSubscription();
				subscription.ScanCode = scanCode.Text;
				subscription.Instrument = scanInstrument.Text;
				subscription.LocationCode = scanLocation.Text;
				subscription.StockTypeFilter = scanStockType.Text;
				subscription.NumberOfRows = Int32.Parse(scanNumRows.Text);
				scannerManager.AddRequest(subscription);
				ShowTab(marketData_MDT, scannerTab);
			}
		}

		private void scannerParamsRequest_button_Click(object sender, EventArgs e)
		{
			scannerManager.RequestParameters();
			ShowTab(marketData_MDT, scannerParamsTab);
		}

		private void scannerTab_link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			scannerManager.Clear();
			marketData_MDT.TabPages.Remove(scannerTab);
		}


		private double stringToDouble(string number)
		{
			if (number != null && !number.Equals(""))
				return Double.Parse(number);
			else
				return 0;
		}

		private Contract GetMDContract()
		{
			Contract contract = new Contract();
			contract.SecType = this.secType_TMD_MDT.Text;
			contract.Symbol = this.symbol_TMD_MDT.Text;
			contract.Exchange = this.exchange_TMD_MDT.Text;
			contract.Currency = this.currency_TMD_MDT.Text;
			contract.LastTradeDateOrContractMonth = this.lastTradeDateOrContractMonth_TMD_MDT.Text;
			contract.PrimaryExch = this.primaryExchange.Text;
			contract.IncludeExpired = includeExpired.Checked;

			if (!mdContractRight.Text.Equals("") && !mdContractRight.Text.Equals("None"))
				contract.Right = (string)((IBType)mdContractRight.SelectedItem).Value;

			contract.Strike = stringToDouble(this.strike_TMD_MDT.Text);
			contract.Multiplier = this.multiplier_TMD_MDT.Text;
			contract.LocalSymbol = this.localSymbol_TMD_MDT.Text;

			return contract;
		}


		private void ShowTab(TabControl tabControl, TabPage page)
		{
			if (!tabControl.Contains(page))
			{
				tabControl.TabPages.Add(page);
			}
			tabControl.SelectedTab = page;
		}

		private void newOrderLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			orderManager.OpenNewOrderDialog();
		}

		private void refreshOrdersButton_Click(object sender, EventArgs e)
		{
			liveOrdersGrid.Rows.Clear();
			ibClient.ClientSocket.reqAllOpenOrders();
		}

		private void refreshExecutionsButton_Click(object sender, EventArgs e)
		{
			tradeLogGrid.Rows.Clear();

			ExecutionFilter execFilter = new ExecutionFilter();
			if (!execFilterClientId.Text.Equals(String.Empty))
				execFilter.ClientId = Int32.Parse(execFilterClientId.Text);
			execFilter.AcctCode = execFilterAccount.Text;
			execFilter.Time = execFilterTime.Text;
			execFilter.Symbol = execFilterSymbol.Text;
			execFilter.SecType = execFilterSecType.Text;
			execFilter.Exchange = execFilterExchange.Text;
			execFilter.Side = execFilterSide.Text;

			ibClient.ClientSocket.reqExecutions(1, execFilter);
		}

		private void bindOrdersButton_Click(object sender, EventArgs e)
		{
			ibClient.ClientSocket.reqAutoOpenOrders(true);
		}

		private void liveOrdersGrid_CellCoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			orderManager.EditOrder();
		}

		private void cancelOrdersButton_Click(object sender, EventArgs e)
		{
			orderManager.CancelSelection();
			liveOrdersGrid.Rows.Clear();
			ibClient.ClientSocket.reqAllOpenOrders();
		}

		private void clientOrdersButton_Click(object sender, EventArgs e)
		{
			liveOrdersGrid.Rows.Clear();
			ibClient.ClientSocket.reqOpenOrders();
		}

		private void globalCancelButton_Click(object sender, EventArgs e)
		{
			ibClient.ClientSocket.reqGlobalCancel();
		}

		private void accSummaryRequest_Click(object sender, EventArgs e)
		{
			accSummaryRequest.Text = "Cancel";
			accountManager.RequestAccountSummary();
		}

		private void accUpdatesSubscribe_Click(object sender, EventArgs e)
		{
			if (accUpdatesSubscribe.Text.Equals("Subscribe"))
			{
				accUpdatesSubscribedAccount.Text = accountSelector.SelectedItem.ToString();
				accUpdatesSubscribe.Text = "Unsubscribe";
			}
			else
			{
				accUpdatesSubscribe.Text = "Subscribe";
			}
			accountManager.SubscribeAccountUpdates();
		}

		private void positionRequest_Click(object sender, EventArgs e)
		{
			accountManager.RequestPositions();
		}

		private void searchContractDetails_Click(object sender, EventArgs e) // Search contract
		{
			ShowTab(contractInfoTab, contractDetailsPage);
			Contract contract = GetConDetContract(); // Read form fields
			//searchContractDetails.Enabled = false;
			contractManager.RequestContractDetails(contract);
		}

		private Contract GetConDetContract()
		{
			Contract contract = new Contract();
			contract.Symbol = this.conDetSymbol.Text;
			contract.SecType = this.conDetSecType.Text;
			contract.Exchange = this.conDetExchange.Text;
			contract.Currency = this.conDetCurrency.Text;
			contract.LastTradeDateOrContractMonth = this.conDetLastTradeDateOrContractMonth.Text;
			contract.Strike = stringToDouble(this.conDetStrike.Text);
			contract.Multiplier = this.conDetMultiplier.Text;
			contract.LocalSymbol = this.conDetLocalSymbol.Text;

			if (!conDetRight.Text.Equals("") && !conDetRight.Text.Equals("None"))
				contract.Right = (string)((IBType)conDetRight.SelectedItem).Value;

			return contract;
		}

		private void fundamentalsQueryButton_Click(object sender, EventArgs e)
		{
			//ShowTab(contractInfoTab, fundamentalsPage);
			fundamentalsQueryButton.Enabled = false;
			Contract contract = GetConDetContract();
			contractManager.RequestFundamentals(contract, (string)((IBType)fundamentalsReportType.SelectedItem).Value);
		}

		private void loadAliases_Click(object sender, EventArgs e)
		{
			advisorAliasesGrid.Rows.Clear();
			advisorManager.RequestFAData(FinancialAdvisorDataType.Aliases);
		}

		private void loadGroups_Click(object sender, EventArgs e)
		{
			advisorGroupsGrid.Rows.Clear();
			advisorManager.RequestFAData(FinancialAdvisorDataType.Groups);
		}

		private void loadProfiles_Click(object sender, EventArgs e)
		{
			advisorProfilesGrid.Rows.Clear();
			advisorManager.RequestFAData(FinancialAdvisorDataType.Profiles);
		}

		private void saveProfiles_Click(object sender, EventArgs e)
		{
			advisorManager.SaveProfiles();
		}

		private void saveGroups_Click(object sender, EventArgs e)
		{
			advisorManager.SaveGroups();
		}

		
		private void findComboContract_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			contractManager.IsComboLegRequest = true;
			contractManager.RequestContractDetails(GetComboContract());
		}

		private Contract GetComboContract()
		{
			Contract contract = new Contract();
			contract.Symbol = "AAPL"; // this.comboSymbol.Text;
			contract.SecType = "STK"; // this.comboSecType.Text;
			contract.Exchange = "SMART"; // this.comboExchange.Text
			contract.Currency = ""; // this.comboCurrency.Text
			contract.LastTradeDateOrContractMonth = ""; // this.comboLastTradeDate.Text
			contract.Strike = stringToDouble(""); // this.comboStrike.Text
			contract.Multiplier = ""; // this.comboMultiplier.Text
			contract.LocalSymbol = "";  // this.comboLocalSymbol.Text

			//if (!comboRight.Text.Equals("") && !comboRight.Text.Equals("None"))
			//	contract.Right = (string)((IBType)comboRight.SelectedItem).Value;

			return contract; 
		}
		

		private void queryOptionChain_Click(object sender, EventArgs e)
		{
			if (isConnected)
			{
				queryOptionChain.Enabled = false;
				Contract underlying = GetConDetContract();
				underlying.SecType = "OPT";
				optionsManager.AddOptionChainRequest(underlying, this.optionChainExchange.Text, optionChainUseSnapshot.Checked);
				ShowTab(contractInfoTab, optionChainPage);

			}
		}

		private void exerciseAccount_SelectedIndexChanged(object sender, EventArgs e)
		{
			accountSelector.SelectedItem = exerciseAccount.SelectedItem;
			accountManager.SubscribeAccountUpdates();
		}

		private void ShowMessageOnPanel(string message)
		{
			message = ensureMessageHasNewline(message);

			if (numberOfLinesInMessageBox >= MAX_LINES_IN_MESSAGE_BOX)
			{
				linesInMessageBox.RemoveRange(0, MAX_LINES_IN_MESSAGE_BOX - REDUCED_LINES_IN_MESSAGE_BOX);
				messageBox.Lines = linesInMessageBox.ToArray();
				numberOfLinesInMessageBox = REDUCED_LINES_IN_MESSAGE_BOX;
			}

			linesInMessageBox.Add(message);
			numberOfLinesInMessageBox += 1;
			this.messageBox.AppendText(message);
		}

		private string ensureMessageHasNewline(string message)
		{
			if (message.Substring(message.Length - 1) != "\n")
			{
				return message + "\n";
			}
			else
			{
				return message;
			}
		}

		private void cancelMarketDataRequests_Click(object sender, EventArgs e)
		{
			marketDataManager.StopActiveRequests(false);
		}

		private void exerciseOption_Click(object sender, EventArgs e)
		{
			int ovrd = overrideOption.Checked == true ? 1 : 0;
			string exchange = optionExchange.Text;
			optionsManager.ExerciseOptions(ovrd, Int32.Parse(optionExerciseQuan.Text), exchange, 1);
		}

		private void lapseOption_Click(object sender, EventArgs e)
		{
			int ovrd = overrideOption.Checked == true ? 1 : 0;
			string exchange = optionExchange.Text;
			optionsManager.ExerciseOptions(ovrd, Int32.Parse(optionExerciseQuan.Text), exchange, 2);
		}

		private void optionsTab_Click(object sender, EventArgs e)
		{

		}

		private void buttonRequestPositionsMulti_Click(object sender, EventArgs e)
		{
			string account = this.textAccount.Text;
			string modelCode = this.textModelCode.Text;
			acctPosMultiManager.RequestPositionsMulti(account, modelCode);
		}

		private void buttonRequestAccountUpdatesMulti_Click(object sender, EventArgs e)
		{
			string account = this.textAccount.Text;
			string modelCode = this.textModelCode.Text;
			Boolean ledgerAndNLV = this.cbLedgerAndNLV.Checked;
			acctPosMultiManager.RequestAccountUpdatesMulti(account, modelCode, ledgerAndNLV);
		}

		private void buttonCancelPositionsMulti_Click(object sender, EventArgs e)
		{
			acctPosMultiManager.CancelPositionsMulti();
		}

		private void buttonCancelAccountUpdatesMulti_Click(object sender, EventArgs e)
		{
			acctPosMultiManager.CancelAccountUpdatesMulti();
		}

		private void clearPositionsMulti_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			acctPosMultiManager.ClearPositionsMulti();
		}

		private void clearAccountUpdatesMulti_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			acctPosMultiManager.ClearAccountUpdatesMulti();
		}

		private void queryOptionParams_Click(object sender, EventArgs e)
		{
			string symbol = conDetSymbol.Text;
			string exchange = conDetExchange.Text;
			string secType = conDetSecType.SelectedItem + "";
			int conId = string.IsNullOrWhiteSpace(underlyingConId.Text) ? int.MaxValue : int.Parse(underlyingConId.Text);

			optionsManager.SecurityDefinitionOptionParametersRequest(symbol, exchange, secType, conId);
			ShowTab(contractInfoTab, optionParametersPage);
		}

		private void messageBoxClear_link_LinkClicked(object sender, EventArgs e)
		{
			numberOfLinesInMessageBox = 0;
			linesInMessageBox.Clear();
			messageBox.Clear();

		}

		private void button1_Click(object sender, EventArgs e) // Test parse json
		{
			//JsonParseTest.Parse();
			searchJsonResponse.Test();
		}

		private void button5_Click(object sender, EventArgs e)
		{
			MessageBox.Show(SettingsJson.dbHost.ToString());
		}
	} // Public class From1
}
