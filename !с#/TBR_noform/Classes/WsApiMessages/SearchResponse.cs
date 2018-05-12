using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Windows.Forms;

namespace TBR_noform
{
	/*
	 * Serialization class which creates a search response json object
	 * 
	 * https://stackoverflow.com/questions/6201529/how-do-i-turn-a-c-sharp-object-into-a-json-string-in-net
	 *
	 */
	public class SearchResponse
	{
		public List<SearchResponseString> searchResponseList;
		private string jsonResponseObject;

		public SearchResponse() // Constructor 
		{
			searchResponseList = new List<SearchResponseString>();		
		}

		public string ReturnJson()
		{
			var obj = new SearchResponseObject // Create object
			{
				messageType = "SearchResponse",
				searchList = searchResponseList
			};
			var json = new JavaScriptSerializer().Serialize(obj); // Serialize object (convert to json)
			jsonResponseObject = json;
			return jsonResponseObject;
		}
	}

	// Object structure
	public class SearchResponseString
	{
		public string symbol;
		public string localSymbol;
		public string type;
		public string currency;
		public string exchange;
		public string primaryExchnage;
		public int conId; 
	}

	public class SearchResponseObject
	{
		public string messageType;
		public List<SearchResponseString> searchList;
	}
}
