using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Echovoice.JSON;
using System.Windows.Forms;

using System.Web.Script.Serialization;



// Nuget library for making a json string: 
// https://coderwall.com/p/vxspdq/simple-json-array-methods-in-c
// https://www.nuget.org/packages/Echovoice.JSON/ 
// https://github.com/echovoice/Echovoice.JSON

namespace TBR_noform
{
	public class Response
	{
		private string symbol { get; set; }
		private string localSymbol { get; set; }
		private string type { get; set; }
		private string currency { get; set; }
		private string exchange { get; set; }
		private string primaryExchange { get; set; }
		private int conId { get; set; }
		public List<Response> jsonResponse = new List<Response>();

		// Class constructor
		public Response(string symb, string loc, string typ, string cur, string exc, string pExc, int conI)
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
		public Response()
		{
		}

		public override string ToString()
		{

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


		public string Test()
		{
			string result = JSONEncoders.EncodeJsObjectArray(jsonResponse.ToArray());
			MessageBox.Show(result);
			Console.WriteLine(result);
			return result;
		}

		public void Add(string symb, string loc, string typ, string cur, string exc, string pExc, int conI)
		{
			jsonResponse.Add(new Response(symb, loc, typ, cur, exc, pExc, conI));
		}

		public void Clear()
		{
			jsonResponse.Clear(); // Empty collection
		}
	}
}
