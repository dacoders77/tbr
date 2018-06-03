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
	public class AvailableFundsResponse
	{
		public double availableFunds;
		private string jsonQuoteObject;

		public AvailableFundsResponse() // Constructor 
		{
		}

		public string ReturnJson()
		{
			var obj = new AvailableFundsResponseObject // Create object
			{
				messageType = "AvailableFundsResponse",
				funds = availableFunds
			};

			var json = new JavaScriptSerializer().Serialize(obj); // Serialize object (convert to json)
			jsonQuoteObject = json;
			return jsonQuoteObject;
		}
	}

	// Object structure
	public class AvailableFundsResponseObject
	{
		public string messageType;
		public double funds;
	}
}
