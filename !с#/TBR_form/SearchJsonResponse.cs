using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Echovoice.JSON;
using System.Windows.Forms;


// Nuget library for making a json string: 
// https://coderwall.com/p/vxspdq/simple-json-array-methods-in-c
// https://www.nuget.org/packages/Echovoice.JSON/ 

namespace TBR_form
{
	public class SearchJsonResponse
	{
		//public string fake { get; set; }
		//public int id { get; set; }
		public string symbol { get; set; }
		public string localSymbol { get; set; }
		public string type { get; set; }
		public string currency { get; set; }
		public string exchange { get; set; }
		public string primaryExchange { get; set; }
		public int conId { get; set; }

		private List<SearchJsonResponse> jsonResponse = new List<SearchJsonResponse>();

		// Class constructor
		public SearchJsonResponse(string symb, string loc, string typ, string cur, string exc, string pExc, int conI)
		{
			//fake = f;
			//id = i;

			symbol = symb;
			localSymbol = loc;
			type = typ;
			currency = cur;
			exchange = exc;
			primaryExchange = pExc;
			conId = conI;
		}

		// 2nd Class constructor. Used for creating the SearchJsonResponse object with no records in the List
		public SearchJsonResponse()
		{
		}

		public override string ToString()
		{
			//StringBuilder sb = new StringBuilder();
			//sb.Append('[');
			//sb.Append(id);
			//sb.Append(',');
			//sb.Append(JSONEncoders.EncodeJsString(fake));
			//sb.Append(']');

			StringBuilder sb = new StringBuilder();
			sb.Append('[');
			sb.Append(JSONEncoders.EncodeJsString(symbol));
			sb.Append(',');
			sb.Append(JSONEncoders.EncodeJsString(localSymbol));
			sb.Append(',');
			sb.Append(JSONEncoders.EncodeJsString(type));
			sb.Append(',');
			sb.Append(JSONEncoders.EncodeJsString(currency));
			sb.Append(',');
			sb.Append(JSONEncoders.EncodeJsString(exchange));
			sb.Append(',');
			sb.Append(JSONEncoders.EncodeJsString(primaryExchange));
			sb.Append(',');
			sb.Append(conId);
			sb.Append(']');

			return sb.ToString();
		}

		public string Test() {
			
			string result = JSONEncoders.EncodeJsObjectArray(jsonResponse.ToArray());
			Console.WriteLine(result);

			return result;
		}

		public void Add(string symb, string loc, string typ, string cur, string exc, string pExc, int conI) {

			jsonResponse.Add(new SearchJsonResponse(symb, loc, typ, cur, exc, pExc, conI));

		}

		public void Clear() {

			jsonResponse.Clear(); // Collection empty

		}

	}
}
