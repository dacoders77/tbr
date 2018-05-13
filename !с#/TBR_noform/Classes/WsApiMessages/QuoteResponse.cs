using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace TBR_noform
{
	/*
	 * Serialization class which creates a get quote response json object
	 * https://stackoverflow.com/questions/6201529/how-do-i-turn-a-c-sharp-object-into-a-json-string-in-net
	 */
	public class QuoteResponse
	{
		private string jsonQuoteObject;
		public double symbolPrice;
		public string symbolName;
		public int basketNum;

		public QuoteResponse() // Constructor 
		{
		}

		public string ReturnJson()
		{
			var obj = new SearchQuoteObject // Create object
			{
				messageType = "QuoteResponse",
				symbol = symbolName,
				price = symbolPrice,
				basketNumber = basketNum
			};

			var json = new JavaScriptSerializer().Serialize(obj); // Serialize object (convert to json)
			jsonQuoteObject = json;
			return jsonQuoteObject;
		}
	}

	// Object structure

	public class SearchQuoteObject
	{
		public string messageType;
		public string symbol;
		public double price;
		public int basketNumber;
	}
}
