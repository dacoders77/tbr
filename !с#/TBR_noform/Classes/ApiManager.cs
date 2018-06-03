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
		public int basketNumber; // When getQuote request is recevied, it contains: symbol and basket number. Then we need to determine the price of a requested sybol and return it with basket number to PHP. Then the mentioned symbol will be updated in basket № ... If there is no basket number, it is not gonna be possible to understand which price of the symbol must be updated. The same symbol can be located and a number of baskets
		public string symbolPass; // The same applies to symbol. We need to pass symbol name the same way we pass basket number. Because IbClient_TickPrice event does not return symbol name

		public ApiManager(IBClient IBClient) // Constructor
		{
			iBClient = IBClient;

			// Contract
			contract = new Contract(); // A contract is created. Then while calling a class method contract.Symbol field is assigned
			//contract.Symbol = "AAPL";
			contract.SecType = "STK";
			contract.Currency = ""; // USD
			//In the API side, NASDAQ is always defined as ISLAND in the exchange field
			contract.Exchange = "SMART"; // SMART. If no exchange specefied - all availible exchanges will be listed

		}

		public void Search(string symbol) // Ticker search
		{
			contract.Symbol = symbol;
			contract.Currency = ""; // Need to reset the currency because it remains the same after previous search
			iBClient.ClientSocket.reqContractDetails((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds, contract); 
		}

		public void GetQuote(string symbol, int basketNum, string currency) // Get ticker qute
		{
			basketNumber = basketNum;
			symbolPass = symbol;

			contract.Symbol = symbol;
			contract.Currency = currency;
			iBClient.ClientSocket.reqMktData((Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds, contract, "", true, false, null); // Request market data for a contract https://interactivebrokers.github.io/tws-api/classIBApi_1_1EClient.html#a7a19258a3a2087c07c1c57b93f659b63
		}

		public void PlaceOrder() // Send order to the exchnage 
		{
		}

	}
}
