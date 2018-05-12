using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBApi;

namespace TBR_noform
{
	class ApiManager
	{
		private IBClient iBClient;
		private Contract contract;

		public ApiManager(IBClient IBClient) // Constructor
		{
			iBClient = IBClient;

			contract = new Contract(); // A contract is created. Then while calling a class method contract.Symbol field is assigned
			//contract.Symbol = "AAPL";
			contract.SecType = "STK";
			contract.Currency = "USD";
			//In the API side, NASDAQ is always defined as ISLAND in the exchange field
			contract.Exchange = "SMART"; // SMART. If no exchange specefied - all availible exchanges will be listed
		}

		public void Search(string symbol) // Ticker search
		{
			contract.Symbol = symbol;
			iBClient.ClientSocket.reqContractDetails((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds, contract); 
		}

		public void GetQuote(string symbol) // Get ticker qute
		{
			contract.Symbol = symbol;
			iBClient.ClientSocket.reqMktData((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds, contract, "", true, false, null); ; // Request market data for a contract https://interactivebrokers.github.io/tws-api/classIBApi_1_1EClient.html#a7a19258a3a2087c07c1c57b93f659b63
		}

		public void PlaceOrder() // Send order to the exchnage 
		{
		}

	}
}
