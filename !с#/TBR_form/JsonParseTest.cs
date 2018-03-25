using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;

namespace TBR_form
{
	public class JsonParseTest
	{
		public void Parse() {

			
			// https://www.newtonsoft.com/json/help/html/JsonSchemaParse.htm
			string schemaJson = @"{
				  'description': 'A person',
				  'type': 'object',
				  'properties': {
					'name': {'type':'string'},
					'hobbies': {
					  'type': 'array',
					  'items': {'type':'string'}
					},
				  }
				}";
			 
			JSchema schema = JSchema.Parse(schemaJson);
			//Console.WriteLine("properties_zhopa: " + schema.Properties.Count());
			// Object
			//Console.WriteLine(schema.Description);
			foreach (var property in schema.Properties)
			{
				//Console.WriteLine(property.Key + " - " + property.Value.Type);
				//Console.WriteLine(property.Value.ToString());
			}
			// name - String
			// hobbies - Array
			
			string schema2 = @"{'dome_name':'hrushevka', 'floors':'9',  'address':'dekabristov 21', 'city':'moscow'}";
			JSchema jsonParsed = JSchema.Parse(schema2);
			var x = Newtonsoft.Json.Linq.JObject.Parse(schema2);
			Console.WriteLine("parse: " + x.Count);
			x["dome_name"] = "new";
			Console.WriteLine("parse: " + x.ToString());
			//Console.WriteLine(jsonParsed.Properties);
			foreach (var z in jsonParsed.Properties)
			{
				//Console.WriteLine(z.ToString());
			}
			

		}

	}
}
