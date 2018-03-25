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
		public string fake { get; set; }
		public int id { get; set; }

		private List<SearchJsonResponse> jsonResponse = new List<SearchJsonResponse>();

		// Class constructor
		public SearchJsonResponse(string f, int i)
		{
			fake = f;
			id = i;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append('[');
			sb.Append(id);
			sb.Append(',');
			sb.Append(JSONEncoders.EncodeJsString(fake));
			sb.Append(']');

			return sb.ToString();
		}

		public string Test() {


			//jsonResponse.Add(new SearchJsonResponse("moskva", 123));
			//jsonResponse.Add(new SearchJsonResponse("spb", 5566));
			//jsonResponse.Add(new SearchJsonResponse("kazan", 78999));
			//jsonResponse.Add(new SearchJsonResponse("nn", 1));
			//jsonResponse.Add(new SearchJsonResponse("omsk", 234));
			
			string result = JSONEncoders.EncodeJsObjectArray(jsonResponse.ToArray());
			//MessageBox.Show(result);
			Console.WriteLine(result);

			return result;
		}

		public void Add(string s, int i) {

			jsonResponse.Add(new SearchJsonResponse(s, i));

		}

	}
}
