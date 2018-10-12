using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBApi;
using System.Windows.Forms;

/*
 * Sends API requests to Interactive Brokers like quote requests, fx quote requests, order placement.
 * Callback is received in Form1.cs
 * Each request has an ID stored in DB
 */

namespace TBR_noform
{
	class ApiManager
	{
		internal IBClient iBClient;
		private Order order;
		public int basketNumber; // When getQuote request is recevied, it contains: symbol and basket number. Then we need to determine the price of a requested sybol and return it with basket number to PHP. Then the mentioned symbol will be updated in basket № ... If there is no basket number, it is not gonna be possible to understand which price of the symbol must be updated. The same symbol can be located and a number of baskets
		public string symbolPass; // The same applies to symbol. We need to pass symbol name the same way we pass basket number. Because IbClient_TickPrice event does not return symbol name
		private Form1 form;

		public ApiManager(IBClient IBClient, Form1 Form) // Constructor
		{
			iBClient = IBClient;
			form = Form;
		}

		public void Search(string symbol) // Ticker search
		{
			Contract contract;
			contract = new Contract();
			contract.SecType = "STK"; // "STK"
			contract.Currency = ""; // USD. In the API side, NASDAQ is always defined as ISLAND in the exchange field
			contract.Exchange = "SMART"; // SMART. If no exchange specefied - all availible exchanges will be listed

			contract.Symbol = symbol;
			contract.Currency = ""; // Need to reset the currency because it remains the same after previous search
			//iBClient.ClientSocket.reqContractDetails((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds, contract);
			iBClient.ClientSocket.reqContractDetails(form.nxtOrderID, contract);
		}

		// Get ticker quote. Called from frontend (browser - socket - fleck)
		public void GetQuote(string symbol, int basketNum, string currency) 
		{

			int requestId = form.nxtOrderID;
			requestId = Convert.ToInt32(requestId.ToString() + "7"); // C# requests: 5 - fx, 6 - stock. PHP get stock quote: 7 - stock. 8 - place order

			basketNumber = basketNum;
			symbolPass = symbol;

			Contract contract;
			contract = new Contract();
			contract.SecType = "STK"; 
			contract.Currency = ""; 
			contract.Exchange = "SMART"; 
			contract.Symbol = symbol; 
			contract.Currency = currency;

			iBClient.ClientSocket.reqMktData(requestId, contract, "", true, false, null); // Request market data for a contract https://interactivebrokers.github.io/tws-api/classIBApi_1_1EClient.html#a7a19258a3a2087c07c1c57b93f659b63
		}

		// Used from basket.cs while basket execution. The difference of this method is that we pass request number as a parameter but in GetQute -
		// the request id is generated in the method itself
		public void GetQuoteBasket(string symbol, int basketId, string currency) // Dont need to send request id
		{
			Contract contract;
			contract = new Contract();
			contract.SecType = "STK";
			contract.Currency = "";
			contract.Exchange = "SMART";

			contract.Symbol = symbol;
			contract.Currency = currency;

			System.Threading.Thread.Sleep(1000); // Make a request onece a second

			int requestId = form.nxtOrderID; 
			requestId = Convert.ToInt32(requestId.ToString() + "6");

			// The first action from which basket execution is started
			Console.WriteLine("GetQuoteBasket. ApiManager.cs line 72. " + DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss:fff") + " " + contract.Symbol + " | " + contract.Currency + " requestID: " + requestId);
			
			form.basket.UpdateInfoJson(string.Format("Stock quote requested. Symbol: {1}. Currency: {2}.  RequestID: {0}", requestId, contract.Symbol, contract.Currency), "stockQuoteRequest", "sent", requestId, "quote_request_id"); // Update json info feild in DB
			form.basket.UpdateRequestId("quote_request_id", requestId, basketId, currency, symbol);
			form.LogOrderIdToFile("ApiManager.cs GetQuoteBasket line 80. " + requestId.ToString());

			iBClient.ClientSocket.reqMktData(requestId, contract, "", true, false, null); 
		}

		// The first action from which basket execution is started.
		// Then when a fx quote is received - 
		public void GetForexQuote(string symbol, string currency, int id, int requestId) { 

			Contract contract_x = new Contract();
			contract_x.Symbol = symbol; 
			contract_x.SecType = "CASH";
			contract_x.Currency = currency; // USD. For all pairs except CAD, JPY, CHF which are converted backwards. https://groups.io/g/twsapi/topic/22665204?p=Created,,,20,2,0,0 
			contract_x.Exchange = "IDEALPRO";

			//Console.WriteLine("GetForexQuote. ApiManager.cs line 79. " + DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss:fff") + " " + contract_x + " " + requestId);

			form.basket.UpdateInfoJson(string.Format("FX quote requested. RequestID: {0}", requestId), "fxQuoteRequest", "ok", requestId, "fx_request_id"); // Update json info feild in DB. Log: time, fx request has been sent
			iBClient.ClientSocket.reqMktData(requestId, contract_x, "", true, false, null); 
		}



		public void PlaceOrder(string symbol, string currency, int volume, int requestId) // int volume
		{

			Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds; // Unix time in milleseconds is used as an order id


			Contract contract;
			contract = new Contract(); // New instance of the contract class
			contract.Symbol = symbol;
			contract.SecType = "STK";
			contract.Exchange = "SMART"; 
			contract.Currency = currency;
			contract.LocalSymbol = symbol;

			/* a sample contract from CS_Testbed
			Contract contract = new Contract();
			contract.Symbol = "IBKR";
			contract.SecType = "STK";
			contract.Currency = "USD";
			//In the API side, NASDAQ is always defined as ISLAND in the exchange field
			contract.Exchange = "ISLAND";
			*/

			order = new Order();
			order.OrderId = unixTimestamp; // 1
			order.Action = "BUY";
			order.OrderType = "MKT"; // MARKET
									 //if (!lmtPrice.Text.Equals(""))
									 //order.LmtPrice = Double.Parse(lmtPrice.Text); // Limit price
									 //if (!quantity.Text.Equals(""))
			order.TotalQuantity = volume; // QUANTITY
									 //order.Account = account.Text;
									 //order.ModelCode = modelCode.Text;
									 //order.Tif = timeInForce.Text; // TIME IN FORCE DAY
									 //if (!auxPrice.Text.Equals(""))
									 //order.AuxPrice = Double.Parse(auxPrice.Text);
									 //if (!displaySize.Text.Equals(""))
									 //order.DisplaySize = Int32.Parse(displaySize.Text);
			order.Tif = "DAY";

			System.Threading.Thread.Sleep(1000);

			Console.WriteLine("PlaceOrder. ApiManager.cs line 132. " + DateTime.Now.ToString("yyyy.MM.dd. HH:mm:ss:fff" + " ibClient.NextOrderId: " + iBClient.NextOrderId ) + " " + contract.Symbol + " | " + contract.Currency + " " + requestId);
			form.basket.UpdateInfoJson(string.Format("Order placed. RequestID: {0}", requestId), "placeOrder", "ok", requestId, "placeorder_request_id"); // Update json info feild in DB

			//iBClient.ClientSocket.placeOrder(unixTimestamp, contract, order);
			
			iBClient.ClientSocket.placeOrder(requestId, contract, order);

		}

	}
}
